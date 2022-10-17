using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDXYTest : MonoBehaviour
{
    public RawImage srcImage;
    public RawImage destImage;
    public Material ddxyMat;

    void Start()
    {
        Texture2D srctex = new Texture2D(2, 2, TextureFormat.RGB24, false);
        Color srcp0 = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Color srcp1 = new Color(0.8f, 0.7f, 0.3f, 0.8f);
        Color srcp2 = new Color(0.9f, 0.4f, 1f, 0.9f);
        Color srcp3 = new Color(1f, 1f, 1f, 1.0f);
#if UNITY_EDITOR
        Debug.LogFormat("srcp0 = {0} srcp1 = {1} srcp2 = {2} srcp3 = {3}", srcp0, srcp1, srcp2, srcp3);
#endif
        srctex.SetPixel(0, 0, srcp0);
        srctex.SetPixel(1, 0, srcp1);
        srctex.SetPixel(0, 1, srcp2);
        srctex.SetPixel(1, 1, srcp3);
        srctex.Apply();
        srcImage.texture = srctex;
        RenderTexture destrt = new RenderTexture(2, 2, 0);
        Graphics.Blit(srctex, destrt, ddxyMat);
        Texture2D desttex = RT2Tex2D(destrt);
        destImage.texture = desttex;
        Color destp0 = desttex.GetPixel(0, 0);
        Color destp1 = desttex.GetPixel(1, 0);
        Color destp2 = desttex.GetPixel(0, 1);
        Color destp3 = desttex.GetPixel(1, 1);
#if UNITY_EDITOR
        Debug.LogFormat("destp0 = {0} destp1 = {1} destp2 = {2} destp3 = {3}", destp0, destp1, destp2, destp3);
#endif
    }

    private Texture2D RT2Tex2D(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        return tex;
    }
}