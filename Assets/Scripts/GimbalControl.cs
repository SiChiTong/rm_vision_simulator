using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROS2;

public class GimbalControl : MonoBehaviour
{
    public float max_pitch_angle = 35;
    public float min_pitch_angle = -25;
    public bool enable_mouse_control = true;

    private Transform pitch_transform;
    private Transform yaw_transform;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<sensor_msgs.msg.JointState> jointStatePub;

    // Start is called before the first frame update
    void Start()
    {
        var yaw_link = this.name + "/base_link/yaw_link";
        var pitch_link = yaw_link + "/pitch_link";
        pitch_transform = GameObject.Find(pitch_link).transform;
        yaw_transform = GameObject.Find(yaw_link).transform;

        Cursor.lockState = CursorLockMode.Locked;

        ros2Unity = GetComponent<ROS2UnityComponent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Press ALT to change mouse control
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            enable_mouse_control = !enable_mouse_control;
            Cursor.lockState = enable_mouse_control ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (enable_mouse_control)
        {

            // Mouse controls
            var mouse_x = Input.GetAxis("Mouse X");
            var mouse_y = Input.GetAxis("Mouse Y");
            yaw_transform.Rotate(0, mouse_x, 0);
            pitch_transform.Rotate(-mouse_y, 0, 0);

            var pitch_angle = pitch_transform.localEulerAngles.x;
            if (pitch_angle > 180)
            {
                pitch_angle -= 360;
            }
            pitch_angle = Mathf.Clamp(pitch_angle, min_pitch_angle, max_pitch_angle);
            pitch_transform.localEulerAngles = new Vector3(pitch_angle, 180, 0);
        }

        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("ROS2UnityJointNode");
                var qos = new QualityOfServiceProfile();
                qos.SetHistory(HistoryPolicy.QOS_POLICY_HISTORY_KEEP_LAST, 1);
                jointStatePub = ros2Node.CreatePublisher<sensor_msgs.msg.JointState>(
                    "/joint_states", qos);
            }
        }

        if (jointStatePub != null)
        {
            // Get timestamp
            var timestamp = new builtin_interfaces.msg.Time();
            ros2Node.clock.UpdateROSClockTime(timestamp);

            var msg = new sensor_msgs.msg.JointState();
            msg.Header.Stamp = timestamp;
            msg.Name = new string[] { "yaw_joint", "pitch_joint" };
            msg.Position = new double[] {
                // radians
                -yaw_transform.localEulerAngles.y / 180 * Mathf.PI,
                pitch_transform.localEulerAngles.x / 180 * Mathf.PI
            };
            jointStatePub.Publish(msg);
        }
    }
}
