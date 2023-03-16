using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChassisControl : MonoBehaviour
{
    private Rigidbody rb;
    private Transform yaw_transform;
    public int max_speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        var yaw_link = this.name + "/base_link/yaw_link";
        yaw_transform = GameObject.Find(yaw_link).transform;
    }

    void FixedUpdate()
    {
        // Keyboard controls
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            // Move towards yaw_link's -z-axis and x-axis
            var direction = yaw_transform.TransformDirection(new Vector3(0, 0, -1));
            var forword_speed = direction * z * max_speed;

            direction = yaw_transform.TransformDirection(new Vector3(-1, 0, 0));
            var side_speed = direction * x * max_speed;

            rb.velocity = forword_speed + side_speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
}
