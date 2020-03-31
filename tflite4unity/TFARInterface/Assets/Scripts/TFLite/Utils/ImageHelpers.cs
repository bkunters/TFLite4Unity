using System;
using UnityEngine;

public class ImageUtils{

    public static Color[] TakeScreenshot(int m_width, int m_height, Camera m_camera){
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
        return pixels;
    }

}