#define DEBUG

using UnityEngine;
using UnityEngine.Networking;
using TFLite;

public class TestScriptTFLite : MonoBehaviour
{
    #if DEBUG
    public string model_name = "hand_3d.tflite";
    private Interpreter m_interpreter;

    [SerializeField]
    private Camera m_camera;

    [SerializeField]
    private LineRenderer[] m_keypoints;

    // Input image width for the tracking model is 256.
    private float RATIO_X;
    // Input image height for the tracking model is 256.
    private float RATIO_Y;

    // Start is called before the first frame update
    void Start()
    {
        RATIO_X = (Screen.currentResolution.width / 256);
        RATIO_Y = (Screen.currentResolution.height / 256);
        
        m_interpreter = new Interpreter(model_name);
        string version = Interpreter.GetTFLiteVersion();
        Debug.LogWarning("TFLite Version: " + version);
    }

    /// <summary>
    /// Capture the current frame and run the inference.
    /// </summary>
    private void LateUpdate()
    {
        RenderTexture renderTexture = new RenderTexture(256, 256, 24);
        m_camera.targetTexture = renderTexture;
        Texture2D cameraImageTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        m_camera.Render();
        RenderTexture.active = renderTexture;
        cameraImageTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);

        m_camera.targetTexture = null;
        RenderTexture.active = null;

        var pixels = cameraImageTexture.GetPixels();
        float[,,] inputData = new float[256, 256, 3];
        for(int i = 0; i < pixels.Length; i++){
            int x = i % 256;
            int y = i / 256;
            inputData[y,x,0] = pixels[i].r / 255f;
            inputData[y,x,1] = pixels[i].g / 255f;
            inputData[y,x,2] = pixels[i].b / 255f;
        }
        Destroy(renderTexture);

        // Inference
        float[,] keypoints = new float[1,63];
        m_interpreter.InvokeModel<float>(inputData, keypoints, inputShape: new int[]{1, 256, 256, 3});
        if (keypoints != null)
        {
            //SetHandLines(keypoints[0]);
        }

        Destroy(cameraImageTexture);
    }

    /// <summary>
    /// TODO: ONLY FOR DEBUG PURPOSES. Visualize with shader later
    /// Draw the hand lines by connecting the output keypoints.
    /// <paramref name="keypoints" All the keypoints in the current frame. 
    /// </summary>
    private void SetHandLines(float[] keypoints)
    {
        Vector3[] keypointPositions = new Vector3[21];
        int j = 0;
        for (int i = 0; i < keypoints.Length; i += 3)
        {
            // TODO: adapt to the current screen size.
            keypointPositions[j] = m_camera.ScreenToWorldPoint(new Vector3(keypoints[i]*RATIO_X, 1080.0f - keypoints[i+1]*RATIO_Y, 1));
            j++;
        }

        // 0-1
        m_keypoints[0].SetPosition(0, keypointPositions[0]);
        m_keypoints[0].SetPosition(1, keypointPositions[1]);

        // 1-2
        m_keypoints[1].SetPosition(0, keypointPositions[1]);
        m_keypoints[1].SetPosition(1, keypointPositions[2]);

        // 2-3
        m_keypoints[2].SetPosition(0, keypointPositions[2]);
        m_keypoints[2].SetPosition(1, keypointPositions[3]);

        // 3-4
        m_keypoints[3].SetPosition(0, keypointPositions[3]);
        m_keypoints[3].SetPosition(1, keypointPositions[4]);

        // 0-5
        m_keypoints[4].SetPosition(0, keypointPositions[0]);
        m_keypoints[4].SetPosition(1, keypointPositions[5]);

        // 5-6
        m_keypoints[5].SetPosition(0, keypointPositions[5]);
        m_keypoints[5].SetPosition(1, keypointPositions[6]);

        // 6-7
        m_keypoints[6].SetPosition(0, keypointPositions[6]);
        m_keypoints[6].SetPosition(1, keypointPositions[7]);

        // 7-8
        m_keypoints[7].SetPosition(0, keypointPositions[7]);
        m_keypoints[7].SetPosition(1, keypointPositions[8]);

        // 5-9
        m_keypoints[8].SetPosition(0, keypointPositions[5]);
        m_keypoints[8].SetPosition(1, keypointPositions[9]);

        // 9-10
        m_keypoints[9].SetPosition(0, keypointPositions[9]);
        m_keypoints[9].SetPosition(1, keypointPositions[10]);

        // 10-11
        m_keypoints[10].SetPosition(0, keypointPositions[10]);
        m_keypoints[10].SetPosition(1, keypointPositions[11]);

        // 11-12
        m_keypoints[11].SetPosition(0, keypointPositions[11]);
        m_keypoints[11].SetPosition(1, keypointPositions[12]);

        // 9-13
        m_keypoints[12].SetPosition(0, keypointPositions[9]);
        m_keypoints[12].SetPosition(1, keypointPositions[13]);

        // 13-14
        m_keypoints[13].SetPosition(0, keypointPositions[13]);
        m_keypoints[13].SetPosition(1, keypointPositions[14]);

        // 14-15
        m_keypoints[14].SetPosition(0, keypointPositions[14]);
        m_keypoints[14].SetPosition(1, keypointPositions[15]);

        // 15-16
        m_keypoints[15].SetPosition(0, keypointPositions[15]);
        m_keypoints[15].SetPosition(1, keypointPositions[16]);

        // 13-17
        m_keypoints[16].SetPosition(0, keypointPositions[13]);
        m_keypoints[16].SetPosition(1, keypointPositions[17]);

        // 0-17
        m_keypoints[17].SetPosition(0, keypointPositions[0]);
        m_keypoints[17].SetPosition(1, keypointPositions[17]);

        // 17-18
        m_keypoints[18].SetPosition(0, keypointPositions[17]);
        m_keypoints[18].SetPosition(1, keypointPositions[18]);

        // 18-19
        m_keypoints[19].SetPosition(0, keypointPositions[18]);
        m_keypoints[19].SetPosition(1, keypointPositions[19]);
        
        // 19-20
        m_keypoints[20].SetPosition(0, keypointPositions[19]);
        m_keypoints[20].SetPosition(1, keypointPositions[20]);
    }
    #endif
}
