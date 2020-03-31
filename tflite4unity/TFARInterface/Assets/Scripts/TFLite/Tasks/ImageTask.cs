using System;
using System.Collections.Generic;
using UnityEngine;
using TFLite;

/// <summary>
/// Represents a single image task with the given image data type T
/// T can be float or sbyte
/// </summary>
public class ImageTask<T>{

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
    public void Inference(){

        var pixels = ImageUtils.TakeScreenshot(m_width: m_width, m_height: m_height, m_camera: m_camera);
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
            // TODO: handle the sbyte inputs.
        }
        
        //Destroy(renderTexture);
        //Destroy(cameraImageTexture);
    }
}