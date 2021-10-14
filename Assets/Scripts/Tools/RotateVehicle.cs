using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateVehicle : MonoBehaviour
{
    private float desiredRot;
    public float rotSpeed = 250;
    public float damping = 10;

    private void Start()
    {
        desiredRot = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        desiredRot += rotSpeed * Time.deltaTime;

        Quaternion desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, desiredRot, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * damping);
    }
}
