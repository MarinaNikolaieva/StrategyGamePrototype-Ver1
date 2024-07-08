using GlobalClasses;
using Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilityClasses;

public class HeightGenerator
{
    private int size;
    private int smallSize;
    private float[,] map;
    private float[,] scaledMap;
    public int[,] intMap;

    public HeightGenerator(int size, int smallSize, float[,] map)
    {
        this.size = size;
        this.smallSize = smallSize;
        this.map = map;
        scaledMap = new float[size, size];
        intMap = new int[size, size];
    }

    private void PreDSAInit(int power)
    {
        int scaledX = 0;
        int scaledY = 0;
        for (int i = 0; i < smallSize; i++)
        {
            for (int j = 0; j < smallSize; j++)
            {
                scaledMap[scaledY, scaledX] = map[i, j];
                scaledX += power;
            }
            scaledX = 0;
            scaledY += power;
        }
    }

    private void SquareStep(int hs, int iter)
    {
        float corner1, corner2, corner3, corner4;
        for (int y = hs; y < size; y += iter)
        {
            for (int x = hs; x < size; x += iter)
            {
                //Calculate corners
                corner1 = scaledMap[y - hs, x - hs];
                corner2 = scaledMap[y + hs, x - hs];
                corner3 = scaledMap[y - hs, x + hs];
                corner4 = scaledMap[y + hs, x + hs];

                //Center point will be average without offscale
                scaledMap[y, x] = (corner1 + corner2 + corner3 + corner4) / 4;
            }
        }
    }

    private void DiamondStep(int hs, int iter)
    {
        for (int y = 0; y < size; y += hs)
        {
            for (int x = y % iter == 0 ? hs : 0; x < size; x += iter)  // getting offset of x in function of y
            {
                double sum = 0;
                double denominator = 0;

                //Calculate border points
                try
                {
                    sum += scaledMap[y + hs, x];
                    denominator++;
                }
                catch (Exception) { }
                try
                {
                    sum += scaledMap[y - hs, x];
                    denominator++;
                }
                catch (Exception) { }
                try
                {
                    sum += scaledMap[y, x + hs];
                    denominator++;
                }
                catch (Exception) { }
                try
                {
                    sum += scaledMap[y, x - hs];
                    denominator++;
                }
                catch (Exception) { }

                //The result is sum average
                scaledMap[y, x] = (float)(sum / denominator);
            }
        }
    }

    public void ScaleMap()
    {
        //This will be the Diamond-Square algorithm
        //That means the maps will be square with the sides' size of 2^power + 1

        //First - prepare the map for scaling (it will be filled with 0.0 upon init, we init the values here)
        int power = (size - 1) / (smallSize - 1);
        if (power <= 1)  //no point in scaling if the power is 1 or less
            return;
        PreDSAInit(power);
        //And now - the D-SA
        int hs;
        for (int iter = power; iter > 1; iter /= 2)
        {
            hs = iter / 2;
            //Square step
            SquareStep(hs, iter);
            //Diamond step
            DiamondStep(hs, iter);
        }
    }

    public void MapValues(float newMin, float newMax)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float temp = 0.0F;
                if (scaledMap[i, j] < 0.5F)  //0.5F is the middle value from the Perlin noise Unity function
                {
                    temp = Mathf.InverseLerp(0F, 0.5F, scaledMap[i, j]);
                    intMap[i, j] = (int)Mathf.Lerp(newMin, 0F, temp);
                }
                else
                {
                    temp = Mathf.InverseLerp(0.5F, 1F, scaledMap[i, j]);
                    intMap[i, j] = (int)Mathf.Lerp(0F, newMax, temp);
                }
            }
        }
    }

    private float UseGaussFilter(float[,] filter, int x, int y)
    {
        float sum = 0.0F;
        int denominator = 0;

        int filterX = 0;
        int filterY = 0;
        for (int i = y - 3; i <= y + 3; i++)
        {
            for (int j = x - 3; j <= x + 3; j++)
            {
                if (i != y || j != x)
                {
                    try
                    {
                        float temp = scaledMap[i, j] * filter[filterY, filterX];
                        sum += temp;
                        denominator++;
                        filterX++;
                    }
                    catch (Exception)
                    {
                        filterX++;
                    }
                }
            }
            filterX = 0;
            filterY++;
        }
        return sum;
    }

    public float GaussBlur(int x, int y)
    {
        float[,] filter = new float[7, 7]
        {
                {0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F},
                {0.00F, 0.00F, 0.01F, 0.01F, 0.01F, 0.00F, 0.00F},
                {0.00F, 0.01F, 0.05F, 0.11F, 0.05F, 0.01F, 0.00F},
                {0.00F, 0.01F, 0.11F, 0.25F, 0.11F, 0.01F, 0.00F},
                {0.00F, 0.01F, 0.05F, 0.11F, 0.05F, 0.01F, 0.00F},
                {0.00F, 0.00F, 0.01F, 0.01F, 0.01F, 0.00F, 0.00F},
                {0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F}
        };

        return UseGaussFilter(filter, x, y);
    }

    public void BalanceDirectly()
    {
        float[,] balanceMap = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                balanceMap[i, j] = GaussBlur(i, j);
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                scaledMap[i, j] = balanceMap[i, j];
            }
        }
    }

    public Texture2D MakeBitmapFromPremade(MapPart[,] map, HeightReader heightReader)
    {
        Dictionary<int, HeightObject> heightChart = heightReader.getHeightChart();
        Texture2D image = new Texture2D(size, size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < heightChart.Count; h++)
                {
                    int temp = map[i, j].Height;
                    if (map[i, j].Height > heightChart.ElementAt(h).Key)
                    {
                        image.SetPixel(i, j, heightChart.ElementAt(h).Value.Color);
                        break;
                    }
                }
            }
        }
        image.Apply();
        return image;
    }

    public Texture2D MakeTextureBW()
    {
        Texture2D image = new Texture2D(size, size, TextureFormat.RGB24, false);
        Color[] colorMap = new Color[size * size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Color tempC = Color.Lerp(Color.black, Color.white, scaledMap[i, j]);
                colorMap[i * size + j] = tempC;
            }
        }
        image.SetPixels(colorMap);
        image.Apply();
        byte[] bytes = image.EncodeToPNG();
        System.IO.File.WriteAllBytes("C:\\Users\\Marina\\Documents\\TestImages\\ImageBW.png", bytes);
        return image;
    }

    public Texture2D MakeTextureColor(HeightReader heightReader)
    {
        Dictionary<int, HeightObject> heightChart = heightReader.getHeightChart();
        Texture2D image = new Texture2D(size, size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < heightChart.Count; h++)
                {
                    int temp = intMap[i, j];
                    if (intMap[i, j] > heightChart.ElementAt(h).Key)
                    {
                        image.SetPixel(i, j, heightChart.ElementAt(h).Value.Color);
                        break;
                    }
                }
            }
        }
        image.Apply();
        return image;
    }

    public Texture2D RunHeightGeneration(HeightReader heightReader, float min, float max)
    {
        ScaleMap();
        MapValues(min, max);
        BalanceDirectly();
        return MakeTextureColor(heightReader);
        //return MakeTextureBW();
    }

    public void MakeMapParts(MapPart[,] mapParts)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                mapParts[i, j].Height = intMap[i, j];
            }
        }
    }
}
