using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject obj;
    private Camera cm;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Sphere");
        cm = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        obj.transform.position = cm.ScreenToWorldPoint(new Vector3(256.0f, 256.0f, 5.0f));
    }
}
