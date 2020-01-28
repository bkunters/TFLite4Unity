#define DEBUG

using UnityEngine;
using UnityEngine.Networking;

using TFLite;

public class TestScriptTFLite : MonoBehaviour
{
    #if DEBUG
    public string model_name = "hand_2d.tflite";
    private Interpreter m_interpreter;

    // Start is called before the first frame update
    void Start()
    {
        m_interpreter = new Interpreter(model_name);
        string version = Interpreter.GetTFLiteVersion();
        Debug.LogWarning("TFLite Version: " + version);
    }
    #endif
}
