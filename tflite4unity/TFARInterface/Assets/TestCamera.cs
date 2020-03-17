using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestCamera : MonoBehaviour
{
    public Material keypoint;

    private RenderTexture renderTexture;
    private Texture2D cameraImageTexture;
    void OnRenderImage(RenderTexture source, RenderTexture destination){
        //Graphics.Blit(source, destination, keypoint);
        renderTexture = new RenderTexture(256, 256, 24);
        cameraImageTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        cameraImageTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        GetComponent<Camera>().targetTexture = null;
        Graphics.Blit(renderTexture, null, keypoint);
//
        RenderTexture.active = null;
        renderTexture.Release();
    }

}
