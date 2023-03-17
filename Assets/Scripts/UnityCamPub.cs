using UnityEngine;
using ROS2;
using UnityEngine.Rendering;

public class UnityCamPub : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<sensor_msgs.msg.Image> camImagePub;
    private IPublisher<sensor_msgs.msg.CameraInfo> camInfoPub;
    private Camera cam;
    private sensor_msgs.msg.CameraInfo camInfoMsg;

    // Start is called before the first frame update
    void Start()
    {
        ros2Unity = GetComponent<ROS2UnityComponent>();
        cam = GameObject.Find(name + "/base_link/yaw_link/pitch_link/camera").GetComponent<Camera>();

        // Calculate camera info
        var fx = cam.fieldOfView * Mathf.Deg2Rad * cam.pixelWidth / (2 * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad / 2));
        var fy = fx;
        var cx = cam.pixelWidth / 2;
        var cy = cam.pixelHeight / 2;

        camInfoMsg = new sensor_msgs.msg.CameraInfo();
        camInfoMsg.K[0] = fx;
        camInfoMsg.K[2] = cx;
        camInfoMsg.K[4] = fy;
        camInfoMsg.K[5] = cy;
        camInfoMsg.K[8] = 1;
        camInfoMsg.D = new double[5];
    }

    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += endCamRedenringCB;
    }

    // Update is called once per frame
    void Update()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("ROS2UnityCamNode");
                camImagePub = ros2Node.CreateSensorPublisher<sensor_msgs.msg.Image>("/image_raw");
                camInfoPub = ros2Node.CreateSensorPublisher<sensor_msgs.msg.CameraInfo>("/camera_info");
            }
        }
    }

    private void endCamRedenringCB(ScriptableRenderContext context, Camera camera)
    {
        // Get timestamp
        var timestamp = new builtin_interfaces.msg.Time();
        ros2Node.clock.UpdateROSClockTime(timestamp);

        // Inverse Y axis
        var tmpRT = RenderTexture.GetTemporary(cam.targetTexture.descriptor);
        Graphics.Blit(cam.targetTexture, tmpRT, new Vector2(1, -1), new Vector2(0, 1));

        // Read the camera image from render texture
        var req = UnityEngine.Rendering.AsyncGPUReadback.Request(tmpRT);
        req.WaitForCompletion();

        // Convert to ROS2 message
        var msg = new sensor_msgs.msg.Image();
        msg.Header.Frame_id = "camera_optical_frame";
        msg.Header.Stamp = timestamp;
        msg.Height = (uint)tmpRT.height;
        msg.Width = (uint)tmpRT.width;
        msg.Encoding = "rgb8";
        msg.Step = (uint)(tmpRT.width * 3);
        ApplyData(ref msg, req.GetData<byte>().ToArray());

        // Update camera info timestamp
        camInfoMsg.Header = msg.Header;

        // Publish the message
        camImagePub.Publish(msg);
        camInfoPub.Publish(camInfoMsg);

        // Release the temporary render texture
        RenderTexture.ReleaseTemporary(tmpRT);
    }

    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= endCamRedenringCB;
    }

    private void ApplyData(ref sensor_msgs.msg.Image msg, byte[] data)
    {
        // RGBA to RGB
        var rgbData = new byte[data.Length / 4 * 3];
        for (int i = 0; i < data.Length / 4; i++)
        {
            rgbData[i * 3] = data[i * 4];
            rgbData[i * 3 + 1] = data[i * 4 + 1];
            rgbData[i * 3 + 2] = data[i * 4 + 2];
        }
        msg.Data = rgbData;
    }
}
