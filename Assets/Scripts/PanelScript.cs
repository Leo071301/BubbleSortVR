using System;
using System.Collections;
using UnityEngine;

/* * * * * * * * * * * * * * * * * 
 * 
 *  PanelScript - a simple script which impliments animations for "spawning in" and "despawning"
 * 
 */

public class PanelScript : MonoBehaviour
{
    [NonSerialized]
    private Vector3 visible_scale;
    private Vector3 invisible_scale = Vector3.zero;
    private const float speed = 5.0f;

    void Start()
    {
        visible_scale = transform.localScale;           // store scale before the game starts,
        transform.localScale = invisible_scale;         // this will make it seem like its hidden

    }

    /* * * * * * * * * * * * * * * * * 
     * 
     * Usefull Coroutine Animations
     * 
     */

    // Spawn in via linear interpolation, the panel will pop-up from nothing -> into its original size
    public IEnumerator SpawnIn()
    {
        gameObject.SetActive(true);

        float move_time = Time.deltaTime * speed; // seconds
        const float fTHRESHOLD = 0.1f;

        // keep moving until reaches destination
        while (Vector3.Distance(transform.localScale, visible_scale) > fTHRESHOLD)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, visible_scale, move_time);
            yield return null;  // pause, and come back next frame
        }
    }
    
    public IEnumerator Despawn()
    {
        float move_time = Time.deltaTime * speed; // seconds
        const float fTHRESHOLD = 0.1f;

        // keep moving until reaches destination
        while (Vector3.Distance(transform.localScale, invisible_scale) > fTHRESHOLD)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, invisible_scale, move_time);
            yield return null;  // pause, and come back next frame
        }

        gameObject.SetActive(false);
    }
}

