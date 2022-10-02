using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCreate : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 3f);
    }


}
