using GlobalClasses;
using Readers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilityClasses;

public class TemperatureGenerator
{
    private int size;
    private float[,] map;
    //CommonGenerator is the place where all the generations take place, including the height influence
    public TemperMoistPartialGenerator commonGenerator;
    private Dictionary<float, TemperatureObject> temperatureChart;

    //If you need the map without heights, run this
    public TemperatureGenerator(int size, TemperatureReader temperReader,
        TemperMoistPartialGenerator temperMoistPartialGenerator, float minMapVal)
    {
        commonGenerator = temperMoistPartialGenerator;
        map = commonGenerator.RunPartialGeneration(minMapVal);
        this.size = size;
        temperatureChart = temperReader.getTemperatureChart();
    }

    //If you need the map with heights included, run this
    public TemperatureGenerator(int size, TemperatureReader temperReader,
        TemperMoistPartialGenerator temperMoistPartialGenerator, float minMapVal, HeightReader heightReader,
        int[,] heightMap, int minHeight, int heightRange, float maxValueCoef)
    {
        commonGenerator = temperMoistPartialGenerator;
        map = commonGenerator.RunFullGeneration(minMapVal, heightReader, heightMap, minHeight, heightRange, maxValueCoef, 0);
        this.size = size;
        temperatureChart = temperReader.getTemperatureChart();
    }

    public Texture2D MakePicture()
    {
        Texture2D image = new Texture2D(size, size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < temperatureChart.Count; h++)
                {
                    float temp = map[i, j];
                    if (map[i, j] >= temperatureChart.ElementAt(h).Key)
                    {
                        image.SetPixel(i, j, temperatureChart.ElementAt(h).Value.Color);
                        break;
                    }
                }
            }
        }
        image.Apply();
        return image;
    }

    public Texture2D MakePictureFromPremade(MapPart[,] map)
    {
        Texture2D image = new Texture2D(size, size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                image.SetPixel(i, j, map[i, j].TemperatureFinal.Color);
            }
        }
        image.Apply();
        return image;
    }

    public void MakeMapPartsBasic(MapPart[,] mapParts)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < temperatureChart.Count; h++)
                {
                    float temp = map[i, j];
                    if (map[i, j] >= temperatureChart.ElementAt(h).Key)
                    {
                        mapParts[i, j].TemperatureBasic = temperatureChart.ElementAt(h).Value;
                        break;
                    }
                }
            }
        }
    }

    public void MakeMapPartsFinal(MapPart[,] mapParts)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < temperatureChart.Count; h++)
                {
                    float temp = map[i, j];
                    if (map[i, j] >= temperatureChart.ElementAt(h).Key)
                    {
                        mapParts[i, j].TemperatureFinal = temperatureChart.ElementAt(h).Value;
                        break;
                    }
                }
            }
        }
    }

    public Texture2D RunGeneration()
    {
        return MakePicture();
    }

    public void RunGenerationMapPartsFinal(MapPart[,] mapParts)
    {
        MakeMapPartsFinal(mapParts);
    }

    public void RunGenerationMapPartsBasic(MapPart[,] mapParts)
    {
        MakeMapPartsBasic(mapParts);
    }
}
