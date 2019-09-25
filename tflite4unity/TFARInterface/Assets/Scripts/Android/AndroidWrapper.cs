using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// This is a wrapper to use Android TFLite code inside Unity through the plugin.
/// </summary>
public class AndroidWrapper
{
    /// <summary>
    /// See the com.bora.tflitehandtracking.HandTrackingInference class in the Java plugin.
    /// </summary>
    private AndroidJavaObject m_tfliteAndroidClass;

    /// <summary>
    /// See the com.bora.tflitehandtracking.Utils class in the Java plugin.
    /// Contains the static helper functions.
    /// </summary>
    private AndroidJavaClass m_utilsClassAndroid;

    #region Android Plugin Name Constants
    private readonly string UNITY_ACTIVITY_CLASS_NAME = "com.unity3d.player.UnityPlayer";
    private readonly string UTILS_CLASS_NAME = "com.bora.tflitehandtracking.Utils";
    private readonly string TFLITE_CLASS_ANDROID = "com.bora.tflitehandtracking.HandTrackingInference";
    #endregion

    /// <summary>
    /// Supports the GPUs with OpenGLES >=3.1.
    /// </summary>
    private bool m_gpuSupport = true;

    public AndroidWrapper() {
        var unityClass = new AndroidJavaClass(UNITY_ACTIVITY_CLASS_NAME);
        var currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");

        // TODO: Check if the current gpu supports OpenGLES >= 3.1

        m_tfliteAndroidClass = new AndroidJavaObject(TFLITE_CLASS_ANDROID, m_gpuSupport);
        m_utilsClassAndroid = new AndroidJavaClass(UTILS_CLASS_NAME);
        m_tfliteAndroidClass.Call("loadModelFile", currentActivity);
    }

    /// <summary>
    /// Runs inference.
    /// </summary>
    public float[] RunInference(byte[] input)
    {
        m_tfliteAndroidClass.Call<float[]>("RunInference", input);
        float[] keypoints = m_tfliteAndroidClass.Call<float[]>("getKeypoints");
        float handConfidence = m_tfliteAndroidClass.Call<float>("getHandscore");

        if (handConfidence >= 0.5)
        {
            return keypoints;
        }
        return null;
    }


}
