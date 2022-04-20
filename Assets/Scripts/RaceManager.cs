using Assets.Scripts;
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
    private CarCam _camParent;
    private Vector3 _positionCameraBase;
    private Camera _startingCamera;
    private Camera _endCamera;
    private GameObject _startingCamParent;
    private GameObject _startPanel;
    private GameObject[] _listCameraSpawn;
    private int _indexListCameraSpawn;
    private float _timerCameraSpawn;
    private GameObject _gameHUD;
    private List<string> _listCheckpointsTime;

    #endregion

    #region Properties
    public APITime WrTime { get => wrTime; set => wrTime = value; }
    public APITime PbTime { get => pbTime; set => pbTime = value; }

    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        _listCheckpointsTime = new List<string>();
        _timerCameraSpawn = 0;
        _indexListCameraSpawn = 0;
        _speed = -10;
        endPanelPanelOpener = GameObject.Find("EndPanel").GetComponent<PanelOpener>();
        endRaceManager = GameObject.Find("EndManager").GetComponent<EndRaceManager>();
        _startPanel = GameObject.Find("StartPanel");
        _gameHUD = GameObject.Find("GameHUD");

        if (_gameHUD != null)
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
        currMapScript.CheckPointTimer += () => CheckPointTimerWorkflow();

        displayScript.UpdateLapData(currMapScript.CurrentLap.ToString(), currMapScript.TotLap.ToString());

        RetrievePBAndWR();
        _endCamera = GameObject.Find("EndCamera").GetComponent<Camera>();
        _startingCamera = GameObject.Find("StartingCamera")?.GetComponent<Camera>();
        if (_endCamera != null)
        {
            _endCamera.enabled = false;
        }
        _isCameraRotating = true;
    }

    // Update is called once per frame
    void Update()
    {
        StartRunning = displayScript.IsRunning;

        if (_startingCamera != null && _isCameraRotating && _startingCamera.isActiveAndEnabled)
        {
            SpawnCamera();
        }

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
            _startingCamera.transform.eulerAngles += new Vector3(0, _speed * Time.deltaTime, 0);
        } 
        else if(_startingCamera != null && _isCameraRotating == false)
        {
            _startingCamera.transform.position = _positionCameraBase;
        }
    }

    private void SpawnCamera()
    {
        _timerCameraSpawn += Time.deltaTime;

        if(_timerCameraSpawn >= 5.0f && _listCameraSpawn.Length > 0)
        {
            GetNextCameraSpawn();
            _startingCamera.transform.position = _listCameraSpawn[_indexListCameraSpawn].transform.position;
            _startingCamera.transform.rotation = _listCameraSpawn[_indexListCameraSpawn].transform.rotation;
            _timerCameraSpawn = 0;
        } 
    }

    private void GetNextCameraSpawn()
    {
        if(_indexListCameraSpawn >= _listCameraSpawn.Length - 1)
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
            _camParent = GameObject.Find("CamParent").GetComponent<CarCam>();
            this.currVehicle.GetComponent<Rigidbody>().isKinematic = true;
            GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;

            if (_startingCamera != null)
            {
                if (_endCamera != null)
                {
                    _endCamera.enabled = false;
                }

                _startingCamera.transform.position = new Vector3(this.currVehicle.transform.position.x, this.currVehicle.transform.position.y + 15, this.currVehicle.transform.position.z);
                _startingCamera.transform.rotation = Quaternion.Euler(20, 0, 0);
                _positionCameraBase = this.currVehicle.transform.position;

              /*  if(_camParent != null)
                {
                    _camParent.GetComponent<CarCam>().enabled = false;
                }*/
                switchCamera(CameraEnum.StartAnimationCam);

                //_startingCamera.enabled = true;

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

    private void switchCamera(CameraEnum cameraEnum)
    {
        switch(cameraEnum)
        {
            case CameraEnum.StartAnimationCam:
                _startingCamera.enabled = true;
                _camParent.enabled = false;
                _endCamera.enabled = false;
                break;
            case CameraEnum.CarCam:
                _startingCamera.enabled = false;
                _camParent.enabled = true;
                _endCamera.enabled = false;
                break;
            case CameraEnum.EndCam:
                _startingCamera.enabled = false;
                _camParent.enabled = false;
                _endCamera.enabled = true;
                break;
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
        Camera endCamera = GameObject.Find("EndCamera").GetComponent<Camera>();

        if (endCamera != null)
        {
            _camParent.GetComponent<CarCam>().enabled = false;
            endCamera.enabled = true;
        }

        displayScript.IsRunning = false;
        //currVehicle.GetComponent<VehicleController>().enabled = false;
        currVehicle.GetComponent<VehicleController>().DisableInputs();
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
    }

    /// <summary>
    /// Function called when the checkpoint is passed
    /// </summary>
    private void CheckPointTimerWorkflow()
    {
       string checkpointTime = displayScript.UpdateCheckPointLapTime(_listCheckpointsTime.Count );

        _listCheckpointsTime.Add(checkpointTime);
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
