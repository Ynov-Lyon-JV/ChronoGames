using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingRear : MonoBehaviour
{
    ParticleSystem myParticleSystem;

    // Start is called before the first frame update
    void Awake()
    {
        myParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void Explode() => myParticleSystem.Play();
}
