using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Utilities class for hand control.
/// </summary>
public class HandControl : MonoBehaviour
{

    #region Public Fields
    // TODO: 6-classed classification problem.
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
    /// Wrapper to be used for inference in each frame.
    /// </summary>
    private AndroidWrapper m_androidWrapper;

    /// <summary>
    /// A single image frame to be processed further.
    /// </summary>
    private XRCameraImage m_cameraImage;

    /// <summary>
    /// The unique camera manager of the scene.
    /// </summary>
    [SerializeField]
    private ARCameraManager m_cameraManager;

    /// <summary>
    /// The AR camera rendering the scene.
    /// </summary>
    [SerializeField]
    private Camera m_camera;

    /// <summary>
    /// The conversion params of the frame image to create a native byte buffer from it.
    /// </summary>
    private XRCameraImageConversionParams m_imageConversionParams;
    #endregion

    #region Lifetime Methods
    void Awake()
    {
        m_androidWrapper = new AndroidWrapper();

        // Set the image conversion params.
        m_imageConversionParams = new XRCameraImageConversionParams
        {
            outputFormat = TextureFormat.RGB24,
            outputDimensions = new Vector2Int(x: 256, y: 256)
        };
    }

    void Start()
    {
        m_cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnEnable()
    {
        m_cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        m_cameraManager.frameReceived -= OnCameraFrameReceived;
    }
    #endregion

    #region Custom Methods

    /// <summary>
    /// Event for management of the current frame image.
    /// </summary>
    /// <param name="eventArgs"></param>
    private unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        // Get the frame image.
        if (!m_cameraManager.TryGetLatestImage(out m_cameraImage))
        {
            return;
        }

        // TODO: Isn't it a bottleneck ??
        m_imageConversionParams.inputRect = new RectInt(0, 0, m_cameraImage.width, m_cameraImage.height);
        m_cameraImage.ConvertAsync(m_imageConversionParams, ProcessImage);

        // Release the memory.
        m_cameraImage.Dispose();
    }

    /// <summary>
    /// Runs the inference async.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="conversionParams"></param>
    /// <param name="data"></param>
    private void ProcessImage(AsyncCameraImageConversionStatus status, XRCameraImageConversionParams conversionParams, NativeArray<byte> data)
    {
        if (status != AsyncCameraImageConversionStatus.Ready)
        {
            Debug.LogErrorFormat("Async request failed with status {0}", status);
            return;
        }

        m_androidWrapper.RunInference(data.ToArray());
    }
    #endregion

}
