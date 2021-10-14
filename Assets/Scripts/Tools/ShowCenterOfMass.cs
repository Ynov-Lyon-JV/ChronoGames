using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCenterOfMass : MonoBehaviour
{
    public GameObject vehicle;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = vehicle.transform.position + vehicle.GetComponent<Rigidbody>().centerOfMass;
    }
}
