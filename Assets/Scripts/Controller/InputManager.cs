using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Fields
    private float verticalInput;
    private float horizontalInput;
    private bool isHandbrake = false;
    private bool respawnInput = false;

    private float verticalInputController;
    private bool isHandbrakeController = false;
    #endregion

    #region Properties
    public float VerticalInput { get => verticalInput; set => verticalInput = value; }
    public float HorizontalInput { get => horizontalInput; set => horizontalInput = value; }
    public bool IsHandbrake { get => isHandbrake; set => isHandbrake = value; }
    public bool RespawnInput { get => respawnInput; set => respawnInput = value; }
    public float VerticalInputController { get => verticalInputController; set => verticalInputController = value; }
    public bool IsHandbrakeController { get => isHandbrakeController; set => isHandbrakeController = value; }
    #endregion

    public bool ControlsBlocked = false;

    private void FixedUpdate()
    {
        if (!ControlsBlocked)
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
            isHandbrake = Input.GetAxis("Handbrake") != 0;
            RespawnInput = Input.GetAxis("Respawn") != 0;

            verticalInputController = Input.GetAxis("VerticalController");
            isHandbrakeController = Input.GetAxis("HandbrakeController") != 0;
        }
        else
        {
            verticalInput = 0;
            horizontalInput = 0;
            isHandbrake = false;
            RespawnInput = false;

            verticalInputController = 0;
            isHandbrakeController = false;
        }
    }
}
