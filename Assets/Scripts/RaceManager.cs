using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    #region Fields
    private GameObject currMap;
    private Map currMapScript;
    private GameObject currVehicle;
    [SerializeField] private DisplayData displayScript;
    private GameManager gameManager;
    private PanelOpener endPanelPanelOpener;
    private EndRaceManager endRaceManager;
    private APIController apiController;

    //DEBUG ONLY
    [SerializeField] private GameObject ghostPrefab;

    private APITime wrTime;
    private APITime pbTime;

    //Ghost
    private GhostManager ghostManager;
    private GameObject currGhost;

    public bool StartRunning { get; private set; }
    private float _speed { get; set; }
    private bool _isCameraRotating { get; set; }
    private GameObject? _camParent;
    private Vector3 _positionCameraBase;
    private Camera _startingCamera;
    private GameObject? _startingCamParent;
    private GameObject? _startPanel;
    private GameObject[] _listCameraSpawn;
    private int _indexListCameraSpawn;
    private float _timerCameraSpawn;
    private GameObject _gameHUD;

    #endregion

    #region Properties
    public APITime WrTime { get => wrTime; set => wrTime = value; }
    public APITime PbTime { get => pbTime; set => pbTime = value; }

    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        _timerCameraSpawn = 0;
        _indexListCameraSpawn = 0;
        _speed = -10;
        endPanelPanelOpener = GameObject.Find("EndPanel").GetComponent<PanelOpener>();
        endRaceManager = GameObject.Find("EndManager").GetComponent<EndRaceManager>();
        _startPanel = GameObject.Find("StartPanel");
        _gameHUD = GameObject.Find("GameHUD");

        if(_gameHUD != null)
        {
            _gameHUD.SetActive(false);
        }

        if (_startPanel != null)
        {
            _startPanel.SetActive(true);
        }

        ghostManager = FindObjectOfType<GhostManager>();

        gameManager = FindObjectOfType<GameManager>();

        this.currMap = Instantiate(gameManager.SelectedMapPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        currMapScript = currMap.GetComponent<Map>();

        // Get all camera's spawn for starting animation 
        _listCameraSpawn = GameObject.FindGameObjectsWithTag("CameraSpawn");

        apiController = FindObjectOfType<APIController>();
        apiController.PBUpdated += () => UpdatePB();
        apiController.WRUpdated += () => UpdateWR();
        apiController.GhostDataUpdated += () => LoadGhostData(apiController.GhostData);

        displayScript.CountdownFinished += () => currVehicle.GetComponent<VehicleController>().enabled = true;
        currMapScript.EndRace += () => EndRaceWorkflow();
        currMapScript.EndLap += () => EndLapWorkflow();
        
        displayScript.UpdateLapData(currMapScript.CurrentLap.ToString(), currMapScript.TotLap.ToString());

        RetrievePBAndWR();
    }

    // Update is called once per frame
    void Update()
    {
        StartRunning = displayScript.IsRunning;

        if (currVehicle != null)
        {
            AnimationCameraBeforeStart();

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                RespawnToLastCP();
            }
            displayScript.UpdateSpeed(currVehicle.GetComponent<VehicleController>().kph);
            displayScript.UpdateGear(currVehicle.GetComponent<VehicleController>().isReverse, currVehicle.GetComponent<VehicleController>().gearNum);
            displayScript.UpdateRPM(currVehicle.GetComponent<VehicleController>().engineRPM);

            if (_startingCamera != null && _isCameraRotating)
            {
                SpawnCamera();
            }
        }
    }
    #endregion

    public Map getcurrMapScript()
    {
        return this.currMapScript;
    }

    public void StartRace()
    {
        displayScript.IsCountdown = true;

        _startPanel.SetActive(false);
        _startingCamera.enabled = false;

        _camParent.GetComponent<CarCam>().enabled = true;

        if (_gameHUD != null)
        {
            _gameHUD.SetActive(true);
        }
        //_camParent.SetActive(true);
    }


    #region Private Methods
    /// <summary>
    /// Rotate vehicule's camera around to create an animation before starting the race
    /// </summary>
    private void AnimationCameraBeforeStart()
    {
        if(_startingCamera != null && _isCameraRotating)
        {
            _startingCamera.transform.Rotate(0, _speed * Time.deltaTime, 0);
        } 
        else if(_startingCamera != null && _isCameraRotating == false)
        {
            _startingCamera.transform.position = _positionCameraBase;
        }
    }

    private void SpawnCamera()
    {
        _timerCameraSpawn += Time.deltaTime;

        if(_timerCameraSpawn >= 5.0f)
        {
            GetNextCameraSpawn();
            _startingCamera.transform.position = _listCameraSpawn[_indexListCameraSpawn].transform.position;
            _timerCameraSpawn = 0;
        } 
    }

    private void GetNextCameraSpawn()
    {
        if(_indexListCameraSpawn == _listCameraSpawn.Length - 1)
        {
            _indexListCameraSpawn = 0;
        } else
        {
            _indexListCameraSpawn ++;
        }
    }

    /// <summary>
    /// Spawns the vehicle with the starting block's position and rotation
    /// </summary>
    private void SpawnVehicle(GhostData ghostData)
    {
        try
        {
            this.currVehicle = Instantiate(this.gameManager.SelectedVehiclePrefab, currMapScript.StartBlock.transform.position, currMapScript.StartBlock.transform.rotation);
            this.currVehicle.GetComponent<Rigidbody>().isKinematic = true;
           // GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;
            _camParent = GameObject.Find("CamParent");
            _startingCamera = GameObject.Find("StartingCamera")?.GetComponent<Camera>();

            if (_startingCamera != null)
            {
                _startingCamera.transform.position = new Vector3(this.currVehicle.transform.position.x, this.currVehicle.transform.position.y + 15, this.currVehicle.transform.position.z);
                _startingCamera.transform.rotation = Quaternion.Euler(20, 0, 0);
                _positionCameraBase = this.currVehicle.transform.position;
                //_camParent.GetComponent<CarCam>().enabled = true;

                _startingCamera.enabled = true;
                _isCameraRotating = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Exception while instantiating the vehicle.\r\nException message : {e.Message}");
        }

        try
        {
            //Instantiate ghost
            if (gameManager.IsGhostSelected)
            {
                this.currGhost = Instantiate(ghostPrefab, currMapScript.StartBlock.transform.position, currMapScript.StartBlock.transform.rotation);
                currGhost.GetComponent<GhostController>().SetDatas(ghostData);
                ghostManager.PlayerTransform = currVehicle.transform;
                ghostManager.IsPlayerSpawned = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Exception while instantiating the ghist.\r\nException message : {e.Message}");
        }
    }

    /// <summary>
    /// Spawns the vehicle with the last checkpoint's position and rotation
    /// </summary>
    private void RespawnToLastCP()
    {
        this.currVehicle.transform.position = currMapScript.LastCP.SpawnPosition;
        this.currVehicle.transform.rotation = currMapScript.LastCP.SpawnRotation;
        this.currVehicle.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    /// <summary>
    /// Function called when the race ends
    /// </summary>
    private void EndRaceWorkflow()
    {
        displayScript.IsRunning = false;
        currVehicle.GetComponent<VehicleController>().enabled = false;
        endRaceManager.SetLastTime();
        endPanelPanelOpener.OpenClosePanel();
        byte[] file = ghostManager.SaveToByteArray();
        apiController.SendTimeToApi(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, displayScript.CurrTime, file);
    }

    /// <summary>
    /// Function called when the lap ends
    /// </summary>
    private void EndLapWorkflow()
    {
        displayScript.UpdateLapData(currMapScript.CurrentLap.ToString(), currMapScript.TotLap.ToString());
        displayScript.UpdateLastLapTime();
    }

    private void RetrievePBAndWR()
    {
        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, null);
        apiController.GetTimeFromAPI(gameManager.SelectedMapIndex + 1, gameManager.SelectedVehicleIndex + 1, gameManager.UserName);
    }

    private void UpdatePB()
    {
        pbTime = apiController.Pb;
        CheckIfRecordsUpdated();
    }

    private void UpdateWR()
    {
        wrTime = apiController.Wr;
        CheckIfRecordsUpdated();
    }

    private void CheckIfRecordsUpdated()
    {
        if (wrTime != null && pbTime != null)
        {
            InitialiseRace();
        }
    }

    private void InitialiseRace()
    {
        if (gameManager.IsGhostSelected)
        {
            switch (gameManager.GhostType)
            {
                case "World Record":
                    apiController.GetGhostFromAPI(apiController.Wr.ghostUrl);
                    break;
                case "Personnal Best":
                    apiController.GetGhostFromAPI(apiController.Pb.ghostUrl);
                    break;
                default:
                    break;
            }
        }
        else
        {
            SpawnVehicle(null);
        }
    }

    private void LoadGhostData(byte[] data)
    {
        GhostData ghostData = ghostManager.LoadFromByteArray(data);
        SpawnVehicle(ghostData);
    }

    #endregion
}
