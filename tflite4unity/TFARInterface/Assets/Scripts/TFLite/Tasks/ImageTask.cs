using System;
using System.Collections.Generic;
using UnityEngine;
using TFLite;

/// <summary>
/// Represents a single image task with the given image data type T
/// T can be float or sbyte
/// </summary>
public class ImageTask<T> : MonoBehaviour{

    #region Public Fields
    /// <summary>
    /// List containing the input image tensors
    /// </summary>
    public IList<T[,,]> ImageTensors{get; private set;}

    /// <summary>
    /// The name of the tflite model(e.g "model.tflite")
    /// </summary>
    public string ModelName;

    /// <summary>
    /// Interpreter for this image task
    /// </summary>
    public Interpreter ImageTaskInterpreter{get; private set;}
    #endregion

    #region Private Fields
    // Input image width for the tracking model is 256.
    private float RATIO_X;
    // Input image height for the tracking model is 256.
    private float RATIO_Y;
    [SerializeField]
    private Camera m_camera;
    [SerializeField]
    private int m_height;
    [SerializeField]
    private int m_width;
    #endregion

    void Start(){
        ImageTaskInterpreter = new Interpreter(ModelName);
        RATIO_X = (Screen.currentResolution.width / m_width);
        RATIO_Y = (Screen.currentResolution.height / m_height);
    }

    /// <summary>
    /// Capture the current frame and run the inference.
    /// </summary>
    private void LateUpdate()
    {
        // Takes the screenshot of the scene.
        RenderTexture renderTexture = new RenderTexture(m_width, m_height, 24);
        m_camera.targetTexture = renderTexture;
        Texture2D cameraImageTexture = new Texture2D(m_width, m_height, TextureFormat.ARGB32, false);
        m_camera.Render();
        RenderTexture.active = renderTexture;
        cameraImageTexture.ReadPixels(new Rect(0, 0, m_width, m_height), 0, 0);
        m_camera.targetTexture = null;
        RenderTexture.active = null;

        var pixels = cameraImageTexture.GetPixels();
        
        if(typeof(T) == typeof(float)){
            float[,,] inputData = new float[m_width, m_height, 3];
            for(int i = 0; i < pixels.Length; i++){
                int x = i % m_width;
                int y = i / m_height;
                inputData[y,x,0] = pixels[i].r / 255f;
                inputData[y,x,1] = pixels[i].g / 255f;
                inputData[y,x,2] = pixels[i].b / 255f;
            }

            // Inference
            float[,] keypoints = new float[1,63];
            ImageTaskInterpreter.InvokeModel<float>(inputData, keypoints, inputShape: new int[]{1, m_width, m_height, 3});
            if (keypoints != null)
            {
                // TODO: do something with the output.
            }
        }
        else{

        }
        
        Destroy(renderTexture);
        Destroy(cameraImageTexture);
    }
}