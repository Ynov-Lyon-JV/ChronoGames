using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    #region Fields
    private bool isAccelerating = false;
    private bool isBraking = false;
    private bool isBrakingLightOn = false;
    private bool isBackingUp = false;
    private int backingUpCoef = 1;
    private bool isBackUpLightOn = false;

    [SerializeField] private bool isFrontWheelsDir;
    [SerializeField] private bool isTraction;
    private int steeringCoef;
    private float horAxis;
    private float verAxis;
    private float brakeAxis;

    [SerializeField] private float motorTorqueCoef;
    [SerializeField] private int currSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float brakeCoef;

    private WheelCollider frontLeft;
    private WheelCollider frontRight;
    private WheelCollider rearLeft;
    private WheelCollider rearRight;

    [SerializeField] private Vector3 centerOfMassOffset;

    private Transform frontLeftWheelMesh;
    private Transform frontRightWheelMesh;
    private Transform rearLeftWheelMesh;
    private Transform rearRightWheelMesh;

    private Vector3 flWheelPosition;
    private Vector3 frWheelPosition;
    private Vector3 rlWheelPosition;
    private Vector3 rrWheelPosition;

    private Quaternion flWheelRotation;
    private Quaternion frWheelRotation;
    private Quaternion rlWheelRotation;
    private Quaternion rrWheelRotation;

    [SerializeField] private GameObject stopLights;
    [SerializeField] private GameObject frontLights;
    [SerializeField] private GameObject backUpLights;


    //private float motorTorqueValue;
    private float brakeTorqueValue;

    private PauseManager pauseManager;
    #endregion



    #region Properties
    //public float MotorTorque { get => motorTorqueValue; }
    public float BrakeTorque { get => brakeTorqueValue; }
    public float CurrSpeed { get => currSpeed; }
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        pauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>();

        WheelCollider[] wheelColliders = this.GetComponentsInChildren<WheelCollider>();
        foreach (WheelCollider wc in wheelColliders)
        {
            switch (wc.name)
            {
                case "FrontLeftWheel":
                    frontLeft = wc;
                    break;
                case "FrontRightWheel":
                    frontRight = wc;
                    break;
                case "RearLeftWheel":
                    rearLeft = wc;
                    break;
                case "RearRightWheel":
                    rearRight = wc;
                    break;
                default:
                    break;
            }
        }

        MeshRenderer[] wheelMeshes = FindObjectsOfType<MeshRenderer>();
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            switch (wheelMeshes[i].name)
            {
                case "FrontLeftWheelMesh":
                    frontLeftWheelMesh = wheelMeshes[i].transform;
                    break;
                case "FrontRightWheelMesh":
                    frontRightWheelMesh = wheelMeshes[i].transform;
                    break;
                case "RearLeftWheelMesh":
                    rearLeftWheelMesh = wheelMeshes[i].transform;
                    break;
                case "RearRightWheelMesh":
                    rearRightWheelMesh = wheelMeshes[i].transform;
                    break;
                default:
                    break;
            }
        }

        if (isFrontWheelsDir)
            steeringCoef = 1;
        else
            steeringCoef = -1;
        this.GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseManager.IsPaused)
        {
            isAccelerating = false;
            isBraking = false;
            isBackUpLightOn = false;
            isBrakingLightOn = false;
        }

        if (!pauseManager.IsPaused)
        {
            verAxis = Input.GetAxisRaw("Vertical");
            // Acceleration
            if (verAxis != 0)
                isAccelerating = true;
            else
                isAccelerating = false;

            // Braking
            brakeAxis = Input.GetAxisRaw("Brake");
            if (brakeAxis > 0)
                isBraking = true;
            else
                isBraking = false;
            stopLights.SetActive(isBrakingLightOn);

            // Backing
            if (isBackingUp)
                backingUpCoef = -1;
            else
                backingUpCoef = 1;
            backUpLights.SetActive(isBackUpLightOn);

            //Steering
            horAxis = Input.GetAxis("Horizontal");

            // Front lights
            if (Input.GetKeyDown(KeyCode.L))
                frontLights.SetActive(!frontLights.activeSelf);


            // Wheel animation
            AnimateWheels();
        }
    }

    void FixedUpdate()
    {
        currSpeed = Mathf.FloorToInt(GetComponent<Rigidbody>().velocity.magnitude * 3.6f * backingUpCoef);

        // Acceleration
        if (isAccelerating && currSpeed < maxSpeed && !pauseManager.IsPaused)
        {
            //if (currSpeed > -0.1f)
            //{
            //    isBackingUp = false;
            //    isBrakingLightOn = false;
            //}
            //else
            //{
            //    isBackUpLightOn = false;
            //    isBrakingLightOn = true;
            //}

            Accelerate(); //false
        }

        // Deceleration
        if (!isAccelerating && !isBraking || currSpeed > maxSpeed || currSpeed < (-maxSpeed / 4))
        {
            if (!isAccelerating)
                isBrakingLightOn = false;
            if (!isBraking)
                isBackUpLightOn = false;
            Decelerate();
        }

        // Brake
        if (isBraking && !pauseManager.IsPaused)
            Brake();

        Steer();

    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Makes the vehicle move forward or backward
    /// </summary>
    /// <param name="isReverse">Bool to decide wether to go forward or backward</param>
    private void Accelerate() //bool isReverse
    {
        if (!isTraction)
        {
            frontLeft.brakeTorque = 0;
            frontRight.brakeTorque = 0;
            rearLeft.brakeTorque = 0;
            rearRight.brakeTorque = 0;
            rearLeft.motorTorque = verAxis * motorTorqueCoef;
            rearRight.motorTorque = verAxis * motorTorqueCoef;
        }
        else
        {
            frontLeft.brakeTorque = 0;
            frontRight.brakeTorque = 0;
            rearLeft.brakeTorque = 0;
            rearRight.brakeTorque = 0;
            frontLeft.motorTorque = verAxis * motorTorqueCoef;
            frontRight.motorTorque = verAxis * motorTorqueCoef;
        }
    }

    /// <summary>
    /// Makes the vehicle slow down over time when there is no input to go forward or backward from the user
    /// </summary>
    private void Decelerate()
    {
        frontLeft.motorTorque = 0;
        frontRight.motorTorque = 0;
        rearLeft.motorTorque = 0;
        rearRight.motorTorque = 0;

        frontLeft.brakeTorque = brakeCoef * Time.fixedDeltaTime;
        frontRight.brakeTorque = brakeCoef * Time.fixedDeltaTime;
        rearLeft.brakeTorque = brakeCoef * Time.fixedDeltaTime;
        rearRight.brakeTorque = brakeCoef * Time.fixedDeltaTime; brakeTorqueValue = frontLeft.brakeTorque;
    }


    /// <summary>
    /// Steers the vehicle left or right
    /// </summary>
    private void Steer()
    {
        if (isFrontWheelsDir)
        {
            frontLeft.steerAngle = horAxis * maxSteerAngle * steeringCoef;
            frontRight.steerAngle = horAxis * maxSteerAngle * steeringCoef;
        }
        else
        {
            rearLeft.steerAngle = horAxis * maxSteerAngle * steeringCoef;
            rearRight.steerAngle = horAxis * maxSteerAngle * steeringCoef;
        }
    }

    /// <summary>
    /// Brakes the vehicle
    /// </summary>
    private void Brake()
    {
        rearLeft.brakeTorque = brakeAxis * brakeCoef;
        rearRight.brakeTorque = brakeAxis * brakeCoef;
        frontLeft.brakeTorque = brakeAxis * brakeCoef;
        frontRight.brakeTorque = brakeAxis * brakeCoef;
    }

    /// <summary>
    /// Animates the wheels based on the wheel colliders movements
    /// </summary>
    private void AnimateWheels()
    {
        // Gets the wheel collider's position and rotation as Vector3 and Quaternion
        frontLeft.GetWorldPose(out flWheelPosition, out flWheelRotation);
        frontRight.GetWorldPose(out frWheelPosition, out frWheelRotation);
        rearLeft.GetWorldPose(out rlWheelPosition, out rlWheelRotation);
        rearRight.GetWorldPose(out rrWheelPosition, out rrWheelRotation);

        // Sets the position of the mesh as its wheel collider's "transform.position" Vector3
        frontLeftWheelMesh.transform.position = flWheelPosition;
        frontRightWheelMesh.transform.position = frWheelPosition;
        rearLeftWheelMesh.transform.position = rlWheelPosition;
        rearRightWheelMesh.transform.position = rrWheelPosition;

        // Sets the rotation of the mesh as its wheel collider's "transform.rotation" Quaternion
        frontLeftWheelMesh.transform.rotation = flWheelRotation;
        frontRightWheelMesh.transform.rotation = frWheelRotation;
        rearLeftWheelMesh.transform.rotation = rlWheelRotation;
        rearRightWheelMesh.transform.rotation = rrWheelRotation;
    }

    #endregion
}