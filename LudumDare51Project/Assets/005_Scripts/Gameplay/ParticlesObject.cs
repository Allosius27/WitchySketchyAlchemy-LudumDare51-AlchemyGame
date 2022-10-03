using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ParticlesObject : MonoBehaviour
{
    [SerializeField] private ParticleSystem _system;

    [SerializeField] private bool hasLifeDuration;

    private void Start()
    {
        if(_system != null && hasLifeDuration)
        {
            Destroy(gameObject, _system.main.duration);
        }
    }
}
