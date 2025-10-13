using System;
using UnityEngine;
using UnityEngine.UIElements;


/*
 *  A script to simplify floating movement behaviour.
 *  Simply attach this to any gameobject u'd like. Looks nice dont ya think?
 */

[ExecuteInEditMode]
public class AttentionArrow : MonoBehaviour
{  

    [Range(10, 150)]
    [SerializeField] public int Rotate_Range = 70;
    [Range(0.1f, 0.5f)]
    [SerializeField] public float Scale_Range = 0.2f;

    private int randomSEED;

    private void Start()
    {
        // the "transform" variable is derived from "MonoBehaviour". I need to check if it exists 
        if (transform == null)
        {
            Debug.Log("WARNING from Float.cs: Unable to retrieve Transform Componet. This will break this script!");
        }

        // random number
        System.Random r = new System.Random();

        randomSEED = r.Next(1, 10);
    }


    void Update()
    {
        // Scale factor of 10 for Unity
        // Scale factor of 1 for Spatial VR   (because Spatial.io execute's code slower) 
#if UNITY_EDITOR
        const int SCALE = 10;
#else
        const int SCALE = 1;
#endif

        transform.localScale = new Vector3 (    transform.localScale.x,
                                                transform.localScale.y - ( ( Scale_Range / 10 * Mathf.Cos( Time.timeSinceLevelLoad / 5.0f) / SCALE) ),
                                                transform.localScale.z);        

        transform.Rotate(new Vector3(0, Rotate_Range * ( Mathf.Sin( Time.timeSinceLevelLoad / 3.0f) / SCALE ), 0));
    }

    
}
