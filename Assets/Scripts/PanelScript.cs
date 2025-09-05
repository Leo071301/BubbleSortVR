using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/* * * * * * * * * * * * * * * * * 
 * 
 *  PanelScript - a simple script which impliments animations for "spawning in" and "despawning"
 * 
 */

public class PanelScript : MonoBehaviour
{
    [NonSerialized]
    private Vector3 visible_position;

    [SerializeField]
    public Vector3 hiding_position;

    void Start()
    {
        visible_position = transform.position;  // store position before the game starts,
        transform.position = hiding_position;   // move out of sight, at the start of the game
    }

    /* * * * * * * * * * * * * * * * * 
     * 
     * Usefull Coroutine Animations
     * 
     */

    public IEnumerator SpawnIn()
    {
        yield return StartCoroutine(CubeUtility.moveCube(gameObject, visible_position));

        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator Despawn()
    {
        yield return StartCoroutine(CubeUtility.moveCube(gameObject, visible_position));

        yield return new WaitForSeconds(0.2f);
    }
}
