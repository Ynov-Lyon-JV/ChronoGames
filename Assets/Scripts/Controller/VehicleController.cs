using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleController : MonoBehaviour
{
    internal enum DriveType
    {
        frontWheel,
        rearWheel,
        allWheel
    }

    internal enum GearboxType
    {
        automatic,
        manual
    }

    #region Fields

    [Header("Managers references")]
    private InputManager im;
    [Space]

    [Header("Components and child components")]
    private Rigidbody rb;
    private RaceManager raceManager;
    private ParticleSystem[] particulesSmoke;
    private ExplodingRear[] myExplodingRears;
    [HideInInspector] public GameObject wheelMeshes, wheelColliders;

    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelsMesh = new GameObject[4];

    private GameObject centerOfMass;

    [Header("Variables")]
    public float handBrakeFrictionMultiplier = 2f;
    [HideInInspector] public bool isFrontDir = true;
    [HideInInspector] public int kph;
    public float smoothTime;
    public float maxRPM, minRPM;
    public AnimationCurve CoefRotationOverSpeed;
    public float CoefRotation;
    [HideInInspector] public float wheelsRPM;
    public float engineRPM;
    public bool isReverse = false;
    [SerializeField] private DriveType driveType;
    [SerializeField] private GearboxType gearboxType;
    [HideInInspector] public float totalPower;
    public float[] gears;
    public float[] gearChangeSpeed;
    public int gearNum;
    public AnimationCurve enginePower;

    private WheelFrictionCurve forwardFriction, sidewaysFriction;

    //Hardcoded values
    private float radius = 6, brakePower, downforceValue = 10f, driftFactor;

    [Space]
    [Header("Custom gravity")]
    private float airTime;
    [Tooltip("The multiplier applied to the gravity. If the car is on the ground, the gravity modifier will be at time = 0")]
    public AnimationCurve myGravityOverAirtime;
    [Space]

    [Header("DEBUG")]
    [HideInInspector] public float[] slip = new float[4];
    float DriftMultiplier = 0;
    #endregion

    [Header("Effets")]
    RearLights myRearLights;

    #region Coroutines
    private IEnumerator TimedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + kph / 20;
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        myRearLights = transform.GetChild(0).GetComponent<RearLights>();

        particulesSmoke = new ParticleSystem[2];
        particulesSmoke[0] = transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<ParticleSystem>();
        particulesSmoke[1] = transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<ParticleSystem>();

        myExplodingRears = GetComponentsInChildren<ExplodingRear>();
    }

    void Start()
    {
        GetObjects();
        StartCoroutine(TimedLoop());
        if (SceneManager.GetActiveScene().name != "MapEditorScene" && SceneManager.GetActiveScene().name != "Test")
        {
            raceManager = GameObject.Find("RaceManager").GetComponent<RaceManager>(); 
        }
        ActiveSmokeOnDrift();

        // Unlock Vehicule to drive :
        rb.isKinematic = false;
    }

    private void StopSmokeOnDrift()
    {
        foreach (ParticleSystem smoke in particulesSmoke)
        {
            smoke.Stop(true);
        }
    }
    private void ActiveSmokeOnDrift()
    {
        foreach (ParticleSystem smoke in particulesSmoke)
        {
            smoke.gameObject.SetActive(true);
        }
    }
    private void StartSmokeOnDrift()
    {
        if (kph >= 40 && IsGrounded())
        {
            foreach (ParticleSystem smoke in particulesSmoke)
            {
                smoke.Play(true);
            }
        } else
        {
            foreach (ParticleSystem smoke in particulesSmoke)
            {
                smoke.Stop(true);
            }
        }
    }

    private void resetForce()
    {
        rb.isKinematic = true;
        rb.isKinematic = false;
    }

    void FixedUpdate()
    {
        if(im.RespawnInput && raceManager.getcurrMapScript().LastCP != null)
        {
            resetForce();
            gameObject.transform.position = raceManager.getcurrMapScript().LastCP.transform.position;
            gameObject.transform.rotation = raceManager.getcurrMapScript().LastCP.transform.rotation;
        }
        AnimateWheels();
        SimulateGravity();
        SteerVehicle();
        AdjustTraction();
        CalculateEnginePower();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            myRearLights.ChangeLights(true);
        else if (Input.GetKeyUp(KeyCode.S))
        {
            myRearLights.ChangeLights(false);
        }
    }
    #endregion

    #region Private Methods
    #region Vehicle physics
    private void MoveVehicle()
    {
        BrakeVehicle();

        switch (driveType)
        {
            case DriveType.frontWheel:
                for (int i = 0; i < wheels.Length - 2; i++)
                {
                    wheels[i].motorTorque = totalPower / 2;
                    wheels[i].brakeTorque = brakePower;
                }
                break;
            case DriveType.rearWheel:
                for (int i = 2; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = totalPower / 2;
                    wheels[i].brakeTorque = brakePower;
                }
                break;
            case DriveType.allWheel:
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = totalPower / 4;
                    wheels[i].brakeTorque = brakePower;
                }
                break;
        }
       
        kph = (int) Math.Round(rb.velocity.magnitude * 3.6f, MidpointRounding.AwayFromZero);
    }

    private void BrakeVehicle()
    {

        if (im.VerticalInput < 0 || im.VerticalInputController < 0)
        {
            brakePower = (kph >= 10) ? 500 : 0;
        }
        else if ((im.VerticalInput == 0 && im.VerticalInputController == 0) && (kph <= 10 || kph >= -10))
        {
            brakePower = 10;
        }
        else
        {
            brakePower = 0;
        }
    }

    private void SteerVehicle()
    {
        CoefRotation = CoefRotationOverSpeed.Evaluate(Mathf.Clamp(kph / 200, 0, 1));

        float newInput = im.HorizontalInput * CoefRotation;

        if (isFrontDir)
        {
            if (newInput > 0)
            {
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * newInput;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * newInput;
            }
            else if (newInput < 0)
            {
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * newInput;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * newInput;
            }
            else
            {
                wheels[0].steerAngle = 0;
                wheels[1].steerAngle = 0;
            }
        }
        else
        {
            if (newInput > 0)
            {
                wheels[2].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * -newInput;
                wheels[3].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * -newInput;
            }
            else if (newInput < 0)
            {
                wheels[2].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * -newInput;
                wheels[3].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * -newInput;
            }
            else
            {
                wheels[2].steerAngle = 0;
                wheels[3].steerAngle = 0;
            }
        }
    }

    private void SimulateGravity()
    {
        bool isGrounded = IsGrounded();
        if (isGrounded && airTime != 0)
            airTime = 0;
        else if(!isGrounded)
            airTime += Time.fixedDeltaTime;
        rb.AddForce(-Vector3.up * downforceValue * 1000 * myGravityOverAirtime.Evaluate((airTime)));
    }

    private void AnimateWheels()
    {
        Vector3 wheelPos = Vector3.zero;
        Quaternion wheelRot = Quaternion.identity;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].GetWorldPose(out wheelPos, out wheelRot);
            wheelsMesh[i].transform.position = wheelPos;
            wheelsMesh[i].transform.rotation = wheelRot;
        }
    }

    private void CalculateEnginePower()
    {
        GetWheelRPM();
        if (im.VerticalInput != 0 && im.VerticalInputController != 0)
        {
            rb.drag = 0.005f;
        }
        if (im.VerticalInput == 0 && im.VerticalInputController == 0)
        {
            rb.drag = 0.1f;
        }
        totalPower = 3.6f * enginePower.Evaluate(engineRPM) * Mathf.Clamp(im.VerticalInput + im.VerticalInputController, -1, 1);

        float velocity = 0.0f;
        if (engineRPM >= maxRPM)
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, maxRPM - 250, ref velocity, smoothTime);
        }
        else
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, minRPM - 100 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);
        }
        engineRPM = Mathf.Clamp(engineRPM, 0, maxRPM + 100);

        MoveVehicle();
        ShifterPro();
    }

    private void GetWheelRPM()
    {
        float sum = 0;
        int r = 0;

        for (int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            r++;
        }

        wheelsRPM = (r != 0) ? (int) sum / r : 0;

        if (wheelsRPM < 0 && !isReverse)
        {
            isReverse = true;
        }
        else if (wheelsRPM > 0 && isReverse)
        {
            isReverse = false;
        }
    }

    private bool CheckGears()
    {
        if (kph >= gearChangeSpeed[gearNum]) return true;
        else return false;
    }

    int index_kph = 0;

    private void ShifterPro()
    {
        if (!IsGrounded())
        {
            return;
        }

        if(index_kph < gearChangeSpeed.Length && kph > gearChangeSpeed[index_kph])
        {
            ExplodeRears();
            index_kph++;
        }
        else if (index_kph > 0 && kph < gearChangeSpeed[index_kph - 1])
        {
            index_kph--;
        }


        switch (gearboxType)
        {
            //Automatic
            case GearboxType.automatic:
                if (engineRPM > maxRPM && gearNum < gears.Length - 1 && !isReverse && CheckGears())
                {
                    gearNum++;
                }
                if (engineRPM < minRPM && gearNum > 0)
                {
                    gearNum--;
                }
                break;
            //Manual
            case GearboxType.manual:
                if (Input.GetKeyDown(KeyCode.E) && gearNum < gears.Length - 1)
                {
                    gearNum++;
                }
                if (Input.GetKeyDown(KeyCode.A) && gearNum > 0)
                {
                    gearNum--;
                }
                break;
        }
    }

    private bool IsGrounded()
    {
        if (wheels[0].isGrounded || wheels[1].isGrounded || wheels[2].isGrounded || wheels[3].isGrounded)
            return true;
        else
            return false;
    }

    private void AdjustTraction()
    {
        //tine it takes to go from normal drive to drift 
        float driftSmothFactor = .7f * Time.deltaTime;

        if (im.IsHandbrake || im.IsHandbrakeController)
        {
            DriftMultiplier += Time.deltaTime /10;

           
            StartSmokeOnDrift();

            sidewaysFriction = wheels[0].sidewaysFriction;
            forwardFriction = wheels[0].forwardFriction;

            float velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakeFrictionMultiplier, ref velocity, driftSmothFactor);

            sidewaysFriction.stiffness -= 0.01f;
            sidewaysFriction.stiffness = Mathf.Clamp(sidewaysFriction.stiffness, 0.2f, 1f);

            for (int i = 0; i < 4; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            //extra grip for the front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            float multiplier = 0;
            if (im.HorizontalInput != 0)
                multiplier = 1;
            else
                multiplier = 0;

            //GetComponent<Rigidbody>().AddForce(transform.forward * (kph / 400) * 12000 * multiplier * DriftMultiplier);
        }
        //executed when handbrake is being held
        else
        {
            StopSmokeOnDrift();

            forwardFriction = wheels[0].forwardFriction;
            sidewaysFriction = wheels[0].sidewaysFriction;

            sidewaysFriction.stiffness += 0.01f;
            sidewaysFriction.stiffness = Mathf.Clamp(sidewaysFriction.stiffness, 0.2f, 1f);

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                ((kph * handBrakeFrictionMultiplier) / 300) + 1;

            for (int i = 0; i < 4; i++)
            {
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;

            }

            if (DriftMultiplier != 0)
                DriftMultiplier = 0;
        }

        //checks the amount of slip to control the drift
        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);

            if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -im.HorizontalInput) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + im.HorizontalInput) * Mathf.Abs(wheelHit.sidewaysSlip);
        }

    }

    private void ExplodeRears()
    {
        foreach (ExplodingRear element in myExplodingRears)
            element.Explode();
    }
    #endregion

    private void GetObjects()
    {
        //General components
        im = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();

        //Wheels
        wheelColliders = GameObject.Find("WheelColliders");
        wheels[0] = wheelColliders.transform.Find("FrontLeft").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("FrontRight").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("RearLeft").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("RearRight").gameObject.GetComponent<WheelCollider>();

        //Wheel meshes
        wheelMeshes = GameObject.Find("WheelMeshes");
        wheelsMesh[0] = wheelMeshes.transform.Find("FrontLeftMesh").gameObject;
        wheelsMesh[1] = wheelMeshes.transform.Find("FrontRightMesh").gameObject;
        wheelsMesh[2] = wheelMeshes.transform.Find("RearLeftMesh").gameObject;
        wheelsMesh[3] = wheelMeshes.transform.Find("RearRightMesh").gameObject;

        //Center of mass
        centerOfMass = GameObject.Find("CenterOfMass");
        rb.centerOfMass = centerOfMass.transform.localPosition;
    }

    public void DisableInputs() => im.ControlsBlocked = true;
    #endregion
}