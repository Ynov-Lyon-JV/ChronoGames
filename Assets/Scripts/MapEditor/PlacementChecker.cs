using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementChecker : MonoBehaviour
{
    private bool _isPlaceable = true;
    public bool IsPlaceable { get => _isPlaceable; set => _isPlaceable = value; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Module"))
        {
            IsPlaceable = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Module"))
        {
            IsPlaceable = true;
        }
    }
}
