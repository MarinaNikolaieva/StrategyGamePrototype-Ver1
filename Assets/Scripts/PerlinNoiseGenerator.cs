using UnityEngine;

public class PerlinNoiseGenerator
{
    //Code taken from this tutorial: https://www.youtube.com/watch?v=MRNFcywkUSA&ab_channel=SebastianLague

    float persistance = 0.5F;
    float lacunarity = 2F;
    float scale = 10.3F;

    //The maps generated are square ONLY
    public float[,] GenerateNoiseMap(int size, int seed, int octaves)
    {
        float[,] map = new float[size, size];
        System.Random rand = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000);
            float offsetY = rand.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {

                float amplitude = 1F;
                float frequency = 1F;
                float noiseHeight = 0F;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (float)x / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (float)y / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2F - 1F;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                map[y, x] = noiseHeight;
            }
        }

        for (int y = 0; y < size; y++)  //height
        {
            for (int x = 0; x < size; x++)  //width
            {
                map[y, x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[y, x]);
            }
        }

        return map;
    }
}
