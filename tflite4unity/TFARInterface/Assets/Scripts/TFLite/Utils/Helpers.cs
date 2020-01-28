using System;
using UnityEngine;
using UnityEngine.Networking;

public class Helpers{

    /// <summary>
    /// Returns the byte buffer of the model from the model location
    /// The .tflite file must be stored in the Assets/StreamingAssets folder
    /// </summary>
    public static byte[] GetModelDataFromModelName(string model_name){
        #if UNITY_ANDROID
        string path = "jar:file://" + Application.dataPath + "!/assets/" + model_name;
        var request = UnityWebRequest.Get(path);
        request.SendWebRequest();
        while (!request.isDone){}
        byte[] modelData = request.downloadHandler.data;
        return modelData;
        #endif
    }

}