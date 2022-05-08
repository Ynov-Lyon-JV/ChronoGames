using UnityEngine;

public class MapValidator : MonoBehaviour
{
    public enum MapState
    {
        Validated,
        Edited,
        Empty
    }

    private SaveLoadManager _loadManager;

    private MapState _mapState;
    private Transform _fanwickTransform;
    public Map map;
    public GameObject fanwick;
    public Camera cam;
    public GameObject ui;

    public MapState MapStateValue { get => _mapState; set => _mapState = value; }

    private void Start()
    {
        _loadManager = FindObjectOfType<SaveLoadManager>();
    }

    public void StartValidation()
    {
        map.SetMapDatas();

        //Instantiate Fanwick
        _fanwickTransform = Instantiate(fanwick, map.StartBlock.transform.position, map.StartBlock.transform.rotation).GetComponent<Transform>();
        
        cam.enabled = false;

        //Disable UI
        ui.SetActive(false);

        //Enable Fanwick's stuff (cam, camscript, controller script)
        _fanwickTransform.GetComponentInChildren<VehicleController>().enabled = true;
        _fanwickTransform.GetComponentInChildren<Camera>().enabled = true;
        _fanwickTransform.GetComponentInChildren<CarCam>().enabled = true;
    }

    public void ValidateMap()
    {
        _loadManager.IsMapValidated = true;
        cam.enabled = true;
        ui.SetActive(true);


        _fanwickTransform.GetComponentInChildren<VehicleController>().enabled = false;
        _fanwickTransform.GetComponentInChildren<Camera>().enabled = false;
        _fanwickTransform.GetComponentInChildren<CarCam>().enabled = false;

        GameObject.Destroy(_fanwickTransform.gameObject);
    }
}
