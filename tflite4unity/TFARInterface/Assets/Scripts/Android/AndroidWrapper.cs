using UnityEngine;

/// <summary>
/// This is a wrapper to use Android TFLite code inside Unity through the plugin.
/// TODO: import the c++ library later https://www.tensorflow.org/lite/performance/gpu_advanced#android_cc
/// </summary>
public class AndroidWrapper
{
    /// <summary>
    /// The class object representing java inference class.
    /// </summary>
    public AndroidJavaObject TFLiteInferenceClass{ get; private set; }

    /// <summary>
    /// See the com.bora.tflitehandtracking.Utils class in the Java plugin.
    /// Contains the static helper functions.
    /// </summary>
    private AndroidJavaClass m_utilsClassAndroid;

    #region Android Plugin Class Name Constants
    private readonly string UNITY_ACTIVITY_CLASS_NAME = "com.unity3d.player.UnityPlayer";
    private readonly string UTILS_CLASS_NAME = "com.bora.tflitehandtracking.Utils";
    #endregion

    /// <summary>
    /// TODO: Warn the customers about this!
    /// TODO: remove later.
    /// </summary>
    private bool m_gpuSupport = true;

    /// <summary>
    /// <paramref name="inferenceClassName" : name of the java inference class together with its package name.
    /// </summary>
    public AndroidWrapper(string inferenceClassName) {
        var unityClass = new AndroidJavaClass(UNITY_ACTIVITY_CLASS_NAME);
        var currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Prepare the 3d hand tracking model for the current scene.
        TFLiteInferenceClass = new AndroidJavaObject(inferenceClassName, m_gpuSupport);
        m_utilsClassAndroid = new AndroidJavaClass(UTILS_CLASS_NAME);
        TFLiteInferenceClass.Call("loadModelFile", currentActivity);
    }

}
