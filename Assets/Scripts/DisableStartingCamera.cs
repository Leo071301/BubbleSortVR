using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

public class DisableStartingCamera : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(d());
    }

    public IEnumerator d()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
}