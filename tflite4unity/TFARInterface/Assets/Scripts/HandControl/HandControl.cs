using UnityEngine;

/// <summary>
/// Inference class for 3D hand tracking.
/// </summary>
public class HandControl : MonoBehaviour, IInference
{
    
    #region Public Fields
    // TODO: Classify the hand gestures at each frame.
    // https://www.manomotion.com/supported-gestures/
    public enum Gestures
    {
        GrabHold,           // 0
        GrabReleased,       // 1
        ClickClicked,       // 2
        ClickFree,          // 3
        PinchPicked,        // 4
        PinchDropped        // 5
    } 
    #endregion

    #region Private Fields
    /// <summary>
    /// Wrapper to be used for inference at each frame.
    /// </summary>
    private AndroidWrapper m_androidWrapper;

    /// <summary>
    /// Name of the inference class.
    /// </summary>
    private readonly string HANDTRACKING2D_CLASS_NAME = "com.bora.tflitehandtracking.HandTrackingInference2D";

    /// <summary>
    /// The AR camera rendering the scene.
    /// </summary>
    [SerializeField]
    private Camera m_camera;

    /// <summary>
    /// The material to be used to visualize the keypoints
    /// </summary>
    [SerializeField]
    private Material m_keypointMaterial;

    // TODO: Remove it later.
    [SerializeField]
    private LineRenderer[] m_keypoints;

    // Input image width for the tracking model is 256.
    private float RATIO_X;
    // Input image height for the tracking model is 256.
    private float RATIO_Y;

    private RenderTexture renderTexture;
    private Texture2D cameraImageTexture;
    #endregion

    #region Lifetime Methods
    void Awake()
    {
        RATIO_X = (Screen.currentResolution.width / 256);
        RATIO_Y = (Screen.currentResolution.height / 256);

        // Set the android wrapper.
        m_androidWrapper = new AndroidWrapper(HANDTRACKING2D_CLASS_NAME);
    }

    //void OnRenderImage(RenderTexture source, RenderTexture destination){
    //    renderTexture = new RenderTexture(256, 256, 24);
    //    cameraImageTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
    //    RenderTexture.active = renderTexture;
    //    cameraImageTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
//
    //    byte[] imageData = cameraImageTexture.EncodeToPNG();
    //    float[] keypoints = RunInference(imageData);
    //    if (keypoints != null)
    //    {
    //        m_keypointMaterial.SetFloatArray("m_keypointBuffer", keypoints);
    //    }
//
    //    m_camera.targetTexture = null;
    //    Graphics.Blit(renderTexture, null, m_keypointMaterial);
//
    //    RenderTexture.active = null;
    //    renderTexture.Release();
    //}

    /// <summary>
    /// Capture the current frame and run the inference.
    /// </summary>
    private void LateUpdate()
    {
        PostProcessAndInference();
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// TODO: ONLY FOR DEBUG PURPOSES.
    /// Draw the hand lines by connecting the output keypoints.
    /// <paramref name="keypoints" All the keypoints in the current frame. 
    /// </summary>
    private void SetHandLines(float[] keypoints)
    {
        Vector3[] keypointPositions = new Vector3[21];
        int j = 0;
        for (int i = 0; i < keypoints.Length; i += 2)
        {
            keypointPositions[j] = m_camera.ScreenToWorldPoint(new Vector3(keypoints[i]*RATIO_X, 
                                                                           Screen.currentResolution.height - keypoints[i+1]*RATIO_Y,
                                                                           0.5f));
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

    private void PostProcessAndInference(){
        renderTexture = new RenderTexture(256, 256, 24);
        Texture2D cameraImageTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        m_camera.Render();
        RenderTexture.active = renderTexture;
        cameraImageTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);

        byte[] imageData = cameraImageTexture.EncodeToPNG();
        float[] keypoints = RunInference(imageData);
        if (keypoints != null)
        {
            SetHandLines(keypoints);
            //Graphics.Blit(renderTexture, null, m_keypointMaterial);
        }

        m_camera.targetTexture = null;
        RenderTexture.active = null;
        renderTexture.Release();
        Destroy(renderTexture);
        Destroy(cameraImageTexture);
    }

    /// <summary>
    /// Runs the inference on device.
    /// </summary>
    public float[] RunInference(byte[] input){
        m_androidWrapper.TFLiteInferenceClass.Call<float[]>("RunInference", input);
        float[] keypoints = m_androidWrapper.TFLiteInferenceClass.Call<float[]>("getKeypoints");
        float handConfidence = m_androidWrapper.TFLiteInferenceClass.Call<float>("getHandscore");

        if(handConfidence >= 0.5){
            return keypoints;
        }
        return null;
    }
    #endregion

}
