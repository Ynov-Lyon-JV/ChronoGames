using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Fields
    private float verticalInput;
    private float horizontalInput;
    private bool isHandbrake = false;

    private float verticalInputController;
    private bool isHandbrakeController = false;
    #endregion

    #region Properties
    public float VerticalInput { get => verticalInput; set => verticalInput = value; }
    public float HorizontalInput { get => horizontalInput; set => horizontalInput = value; }
    public bool IsHandbrake { get => isHandbrake; set => isHandbrake = value; }
    public float VerticalInputController { get => verticalInputController; set => verticalInputController = value; }
    public bool IsHandbrakeController { get => isHandbrakeController; set => isHandbrakeController = value; }
    #endregion

    private void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        isHandbrake = Input.GetAxis("Handbrake") != 0;

        verticalInputController = Input.GetAxis("VerticalController");
        isHandbrakeController = Input.GetAxis("HandbrakeController") != 0;
    }
}
