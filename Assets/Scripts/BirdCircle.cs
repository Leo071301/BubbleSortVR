using UnityEngine;

public class BirdCircle : MonoBehaviour
{
    private Vector3 center;
    private float speed;

    void Start()
    {
        center = transform.position; // orbit around start position
        speed = Random.Range(20f, 50f); // degrees per second
    }

    void Update()
    {
        // rotate around center on Y axis (horizontal circle)
        transform.RotateAround(center, Vector3.up, speed * Time.deltaTime);
    }
}