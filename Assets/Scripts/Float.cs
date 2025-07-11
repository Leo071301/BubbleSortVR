using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;


/*
 *  A script to simplify floating movement behaviour.
 *  Simply attach this to any gameobject u'd like. Looks nice dont ya think?
 */

public class Float : MonoBehaviour
{

    // Scale factor of 2500 for Unity
    // Scale factor of 250 for Spatial VR   (because Spatial.io execute's code slower) 
    [Range(1, 400)]
    [SerializeField] public int Slowness = 250;

    [SerializeField] public bool enable_x = true;
    [SerializeField] public bool enable_y = true;
    [SerializeField] public bool enable_z = true;

    private int randomSEED;

    private void Start()
    {
        // the "transform" variable is derived from "MonoBehaviour". I need to check if it exists 
        if (transform == null )
        {
            Debug.Log("WARNING from Float.cs: Unable to retrieve Transform Componet. This will break this script!");
        }

        // random number
        System.Random r = new System.Random();

        randomSEED = r.Next(1, 10);
    }
    

    void Update()
    {
        int SCALE = Slowness;

#if UNITY_EDITOR
        SCALE *= 10;
#endif

        Vector3 offset = Vector3.zero;

        if (enable_x) offset.x += Mathf.Sin(Time.time + randomSEED) / SCALE;
        if (enable_y) offset.y += Mathf.Sin(Time.time  + randomSEED / 2)  / SCALE;
        if (enable_z) offset.z += Mathf.Sin(Time.time  + randomSEED / 3) / SCALE;

        transform.position += offset;
    }
}
