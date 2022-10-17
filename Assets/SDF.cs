using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class Pixel
{
   public bool isIn;
    public float distance;
}

public class SDF : MonoBehaviour
{
    private static Pixel[,] pixels;
    private static Pixel[,] targetPixels;

    
 
   
    public Texture2D source;
  //  public Texture2D destination;

    private void Start()
    {
       Debug.Log("SmoothStep:"+Mathf.SmoothStep(0.1F, -0.1F, -0.5F));
        WriteTexture2D(source);
        //Texture2D destination = new Texture2D(source.width, source.height,TextureFormat.BGRA32,false);
        //GenerateSDF(source,destination);
    }
    void WriteTexture2D(Texture2D source)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        int x, y;
        string[] str =  new string[sourceHeight];
        
        for (y = 0; y < sourceHeight; y++)
        {
            for (x = 0; x < sourceWidth; x++)
            {
                str[y]+= source.GetPixel(x, y).r+" ";
            }
            
        }
        File.WriteAllLines(Application.dataPath+"/texture2D.txt", str);
    }
    public static void GenerateSDF(Texture2D source, Texture2D destination)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        int targetWidth = destination.width;
        int targetHeight = destination.height;

        pixels = new Pixel[sourceWidth, sourceHeight];
        targetPixels = new Pixel[targetWidth, targetHeight];
        Debug.Log("sourceWidth" + sourceWidth);
        Debug.Log("sourceHeight" + sourceHeight);
        int x, y;
        Color targetColor = Color.white;
        for (y = 0; y < sourceWidth; y++)
        {
            for (x = 0; x < sourceHeight; x++)
            {
                pixels[x, y] = new Pixel();
                if (source.GetPixel(x, y) == Color.white)
                    pixels[x, y].isIn = true;
                else
                    pixels[x, y].isIn = false;
            }
        }


        int gapX = sourceWidth / targetWidth;
        int gapY = sourceHeight / targetHeight;
        int MAX_SEARCH_DIST = 512;
        int minx, maxx, miny, maxy;
        float max_distance = -MAX_SEARCH_DIST;
        float min_distance = MAX_SEARCH_DIST;

        for (x = 0; x < targetWidth; x++)
        {
            for (y = 0; y < targetHeight; y++)
            {
                targetPixels[x, y] = new Pixel();
                int sourceX = x * gapX;
                int sourceY = y * gapY;
                int min = MAX_SEARCH_DIST;
                minx = sourceX - MAX_SEARCH_DIST;
                if (minx < 0)
                {
                    minx = 0;
                }
                miny = sourceY - MAX_SEARCH_DIST;
                if (miny < 0)
                {
                    miny = 0;
                }
                maxx = sourceX + MAX_SEARCH_DIST;
                if (maxx > (int)sourceWidth)
                {
                    maxx = sourceWidth;
                }
                maxy = sourceY + MAX_SEARCH_DIST;
                if (maxy > (int)sourceHeight)
                {
                    maxy = sourceHeight;
                }
                int dx, dy, iy, ix, distance;
                bool sourceIsInside = pixels[sourceX, sourceY].isIn;
                if (sourceIsInside)
                {
                    for (iy = miny; iy < maxy; iy++)
                    {
                        dy = iy - sourceY;
                        dy *= dy;
                        for (ix = minx; ix < maxx; ix++)
                        {
                            bool targetIsInside = pixels[ix, iy].isIn;
                            if (targetIsInside)
                            {
                                continue;
                            }
                            dx = ix - sourceX;
                            distance = (int)Mathf.Sqrt(dx * dx + dy);
                            if (distance < min)
                            {
                                min = distance;
                            }
                        }
                    }

                    if (min > max_distance)
                    {
                        max_distance = min;
                    }
                    targetPixels[x, y].distance = min;
                }
                else
                {
                    for (iy = miny; iy < maxy; iy++)
                    {
                        dy = iy - sourceY;
                        dy *= dy;
                        for (ix = minx; ix < maxx; ix++)
                        {
                            bool targetIsInside = pixels[ix, iy].isIn;
                            if (!targetIsInside)
                            {
                                continue;
                            }
                            dx = ix - sourceX;
                            distance = (int)Mathf.Sqrt(dx * dx + dy);
                            if (distance < min)
                            {
                                min = distance;
                            }
                        }
                    }

                    if (-min < min_distance)
                    {
                        min_distance = -min;
                    }
                    targetPixels[x, y].distance = -min;
                }
            }
        }

        //EXPORT texture
        float clampDist = max_distance - min_distance;
        for (x = 0; x < targetWidth; x++)
        {
            for (y = 0; y < targetHeight; y++)
            {
                targetPixels[x, y].distance -= min_distance;
                float value = targetPixels[x, y].distance / clampDist;
                destination.SetPixel(x, y, new Color(1, 1, 1, value));
            }
        }
        destination.Apply();
        File.WriteAllBytes(Application.dataPath+"/test.png",destination.EncodeToPNG());
    }
}

