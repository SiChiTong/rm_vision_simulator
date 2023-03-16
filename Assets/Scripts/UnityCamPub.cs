using UnityEngine;
using ROS2;
using UnityEngine.Rendering;

public class UnityCamPub : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<sensor_msgs.msg.Image> camPub;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        ros2Unity = GetComponent<ROS2UnityComponent>();
        cam = GameObject.Find("camera").GetComponent<Camera>();
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
                camPub = ros2Node.CreatePublisher<sensor_msgs.msg.Image>("/image_raw",
                    new QualityOfServiceProfile(QosPresetProfile.SENSOR_DATA));
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
        msg.Header.Frame_id = "camera";
        msg.Header.Stamp = timestamp;
        msg.Height = (uint)tmpRT.height;
        msg.Width = (uint)tmpRT.width;
        msg.Encoding = "rgb8";
        msg.Step = (uint)(tmpRT.width * 3);
        ApplyData(ref msg, req.GetData<byte>().ToArray());

        // Publish the message
        camPub.Publish(msg);

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
