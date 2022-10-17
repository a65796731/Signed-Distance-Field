using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class SignedDistanceFieldShadow : MonoBehaviour
{

  public  Camera camera;
    public Material material;
    private void Start()
    {
        
    }
    private void Update()
    {
       
       
    }
    public Matrix4x4 CalculateFrustumCorners()
    {
      Transform camTrans= camera.transform;
      Vector3[] frustumCorners = new Vector3[4];
       camera.CalculateFrustumCorners(new Rect(0,0,1,1),camera.nearClipPlane,camera.stereoActiveEye,frustumCorners);
        var bottomLeft = camTrans.TransformVector(frustumCorners[0]);
        var topLeft = camTrans.TransformVector(frustumCorners[1]);
        var topRight = camTrans.TransformVector(frustumCorners[2]);
        var bottomRight = camTrans.TransformVector(frustumCorners[3]);
        Matrix4x4 frustumCornersArray = Matrix4x4.identity;
        frustumCornersArray.SetRow(0, bottomLeft);
        frustumCornersArray.SetRow(1, bottomRight);
        frustumCornersArray.SetRow(2, topLeft);
        frustumCornersArray.SetRow(3, topRight);


        for (int i = 0; i < 4; i++)
        {
            var worldSpaceCorner = camera.transform.TransformVector(frustumCorners[i]);
            Debug.DrawRay(camera.transform.position, worldSpaceCorner*10, Color.blue);
        }
        return frustumCornersArray;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material)
        {
            RenderTexture temp1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
           

            //复制泛光图
          //  Graphics.Blit(source, temp1);

            material.SetMatrix("_Corners", CalculateFrustumCorners());
            material.SetVector("camPos", camera.transform.position);
            //根据阈值提取高亮部分,使用pass0进行高亮提取
           
        

            //使用pass2进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中
            Graphics.Blit(source, destination, material);

            //释放申请的RT
            //RenderTexture.ReleaseTemporary(temp1);
            //RenderTexture.ReleaseTemporary(temp2);
        }


    }
}
