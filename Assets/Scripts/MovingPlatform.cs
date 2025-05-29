using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Transform pointA;
    public Transform pointB;
    public float speed = 2.0f;

    private Vector3 target;


    // Start is called before the first frame update
    void Start()
    {

        // start by moving toward point b
        target = pointB.position;
        
    }

    // Update is called once per frame
    void Update()
    {

        // Move the platform toward the current target 
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // if platform reaches the target, switch direction 
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;

        }
        
    }
}
