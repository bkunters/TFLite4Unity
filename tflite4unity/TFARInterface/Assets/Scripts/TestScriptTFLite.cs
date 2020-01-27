#define DEBUG

using UnityEngine;
using UnityEngine.Networking;

using TFLite;

public class TestScriptTFLite : MonoBehaviour
{

    public string model_name = "hand_2d.tflite";
    private Interpreter m_interpreter;

    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID
        string path = "jar:file://" + Application.dataPath + "!/assets/" + model_name;
        var request = UnityWebRequest.Get(path);
        request.SendWebRequest();
        while (!request.isDone){}
        byte[] modelData = request.downloadHandler.data;
        m_interpreter = new Interpreter(modelData);
        #endif
    }
}
