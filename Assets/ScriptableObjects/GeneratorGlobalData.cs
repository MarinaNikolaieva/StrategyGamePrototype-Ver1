using GlobalClasses;
using Readers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GeneratorData")]
public class GeneratorGlobalData : ScriptableObject
{
    string DataPath = "";

    CultureInfo culture = CultureInfo.InvariantCulture;
    //This is the section for all parameters needed for generation
    public int power = 10;
    public int smallPower = 8;
    public int size = 1025;
    public int smallSize = 257;
    public int loadSize = 513;

    public int minHeight = -15000;
    public int maxHeight = 10000;
    public int heightRange;

    public int noiseOctaves = 5;

    public float minimumMapVal = 0.75F;
    public float maxMoistureCoef = 0.5F;
    public float maxTemperatureCoef = 0.55F;

    public int biomeIterNumber = 7;
    public int soilIterNumber = 5;
    public int GOResourceIterNumber = 5;

    public int biomeNeighborNum = 5;
    public int soilNeighborNum = 5;
    public int GOResourceNeighborNum = 5;
    public int UResourceNeighborNum = 5;

    public int numOfCountries = 8;
    public int minDistanceBetweenCapitals = 15;
    public int minHeightForCapital = 0;
    public int maxHeightForCapital = 3000;
    public int minHeightForExpantion = -1000;

    public ResourceReader resourceReader;
    public TemperatureReader temperatureReader;
    public MoistureReader moistureReader;
    public HeightReader heightReader;
    public BiomeReader biomeReader;
    public SoilReader soilReader;
    public CountryReader countryReader;
    public IndustryReader industryReader;
    public ProductReader productReader;
    public PathwayTypeReader pathwayTypeReader;

    public TemperMoistPartialGenerator TemperMoistPartialGenerator;

    public string selectedFolderAddress;
    public string fileName;

    public bool sizeConfirmed = false;
    public bool heightGenerated = false;
    public bool temperatureGenerated = false;
    public bool moistureGenerated = false;
    public bool biomeGenerated = false;
    public bool soilGenerated = false;
    public bool globalResourcesGenerated = false;
    public bool ongroundResourcesGenerated = false;
    public bool undergroundResourcesGenerated = false;
    public bool politicGenerated = false;
    public bool preparationDone = false;

    public bool ResConfigSuccess = true;

    public List<Tuple<bool, float>> undergroundResCoefs = new List<Tuple<bool, float>>
        {
            new Tuple<bool, float>(true, 0.5F),
            new Tuple < bool, float >(true, 0.675F),
            new Tuple < bool, float >(true, 0.65F),
            new Tuple < bool, float >(true, 0.65F),
            new Tuple < bool, float >(true, 0.675F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.825F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.85F),
            new Tuple < bool, float >(true, 0.675F),
            new Tuple < bool, float >(true, 0.725F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.725F),
            new Tuple < bool, float >(true, 0.725F)
        };

    public int[,] HeightMap;
    public MapPart[,] ComplexMap;

    public Dictionary<string, Texture2D> MapTextures;

    public int politicGenNum;

    //Throw all parameters back to default
    public void ResetAll()
    {
        power = 10;
        smallPower = 8;
        size = 1025;
        smallSize = 257;
        loadSize = 513;

        minHeight = -15000;
        maxHeight = 10000;
        heightRange = maxHeight - minHeight;

        noiseOctaves = 5;

        minimumMapVal = 0.75F;
        maxMoistureCoef = 0.5F;
        maxTemperatureCoef = 0.55F;

        biomeIterNumber = 7;
        soilIterNumber = 5;
        GOResourceIterNumber = 5;

        biomeNeighborNum = 5;
        soilNeighborNum = 5;
        GOResourceNeighborNum = 5;
        UResourceNeighborNum = 5;

        numOfCountries = 8;
        minDistanceBetweenCapitals = 15;
        minHeightForCapital = 0;
        maxHeightForCapital = 3000;
        minHeightForExpantion = -1000;

        sizeConfirmed = false;
        heightGenerated = false;
        temperatureGenerated = false;
        moistureGenerated = false;
        biomeGenerated = false;
        soilGenerated = false;
        globalResourcesGenerated = false;
        ongroundResourcesGenerated = false;
        undergroundResourcesGenerated = false;
        politicGenerated = false;
        preparationDone = false;

        ResConfigSuccess = true;

        TemperMoistPartialGenerator = null;

        DataPath = Application.streamingAssetsPath + "/DataFilesForUnity";

        heightReader = new HeightReader(DataPath + "/HeightChart.txt");
        temperatureReader = new TemperatureReader(DataPath + "/TemperatureChart.txt");
        moistureReader = new MoistureReader(DataPath + "/MoistureChart.txt");
        resourceReader = new ResourceReader(DataPath + "/ResourceChart.txt");
        biomeReader = new BiomeReader(DataPath + "/BiomeChart.txt", DataPath + "/BiomeHeightTable.txt", DataPath + "/BiomeTemperatureMoistureTable.txt");
        soilReader = new SoilReader(DataPath + "/SoilBiomeChart.txt");
        countryReader = new CountryReader(DataPath + "/CountryList.txt");
        industryReader = new IndustryReader(DataPath + "/IndustriesTable.txt");
        pathwayTypeReader = new PathwayTypeReader(DataPath + "/PathwayTypes.txt");
        productReader = new ProductReader(DataPath + "/ProductChart.txt");

        temperatureReader.readTemperatures();
        moistureReader.readMoistures();
        heightReader.ReadHeights();
        resourceReader.readResources();
        biomeReader.readBiomes(resourceReader);
        biomeReader.readBiomeHeights(temperatureReader);
        biomeReader.readMoistTempers(temperatureReader, moistureReader);
        soilReader.readSoils(biomeReader);
        countryReader.readCountries();
        for (int i = 0; i < countryReader.GetCountries().Count; i++)
        {
            resourceReader = new ResourceReader(DataPath + "/ResourceChart.txt");
            resourceReader.readResources();
            countryReader.GetCountries()[i].InitResources(resourceReader);
            industryReader.ReadIndustries(resourceReader, countryReader.GetCountries()[i]);
            productReader = new ProductReader(DataPath + "/ProductChart.txt");
            productReader.Read(countryReader.GetCountries()[i]);
            pathwayTypeReader = new PathwayTypeReader(DataPath + "/PathwayTypes.txt");
            pathwayTypeReader.ReadPathwayTypes();
            countryReader.GetCountries()[i].SetPathwayTypes(pathwayTypeReader.GetPathwayTypes());
        }
        numOfCountries = countryReader.GetCountries().Count;

        undergroundResCoefs = new List<Tuple<bool, float>>
        {
            new Tuple<bool, float>(true, 0.5F),
            new Tuple < bool, float >(true, 0.675F),
            new Tuple < bool, float >(true, 0.65F),
            new Tuple < bool, float >(true, 0.65F),
            new Tuple < bool, float >(true, 0.675F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.825F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.85F),
            new Tuple < bool, float >(true, 0.675F),
            new Tuple < bool, float >(true, 0.725F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.75F),
            new Tuple < bool, float >(true, 0.725F),
            new Tuple < bool, float >(true, 0.725F)
        };

        HeightMap = new int[size, size];
        ComplexMap = new MapPart[size, size];
        MapTextures = new Dictionary<string, Texture2D>();

        politicGenNum = 0;
    }

    public void ClearTexturesRangeWithStart(int start)
    {
        for (int i = MapTextures.Count - 1; i >= start; i--)
        {
            MapTextures.Remove(MapTextures.Keys.ElementAt(i));
        }
    }

    public void SetUndergroundCoefs(List<Tuple<bool, float>> newCoefs)
    {
        undergroundResCoefs.Clear();
        undergroundResCoefs.AddRange(newCoefs);
    }

    public void SetPowersAndSizes(int Power, int SmallPower)
    {
        power = Power;
        smallPower = SmallPower;
        size = (int)Math.Pow(2, power) + 1;
        smallSize = (int)Math.Pow(2, smallPower) + 1;
        ComplexMap = new MapPart[size, size];
        int counter = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                ComplexMap[i, j] = new MapPart(counter);
                ComplexMap[i, j].X = j;
                ComplexMap[i, j].Y = i;
                counter++;
            }
        }
        HeightMap = new int[size, size];
    }

    public void SetMinMaxHeights(int min, int max)
    {
        minHeight = min;
        maxHeight = max;
        heightRange = max - min;
    }

    public void SetTemperParams(float temper, float mapCoef)
    {
        maxTemperatureCoef = temper;
        minimumMapVal = mapCoef;
    }

    public void SetMoistParams(float moist)
    {
        maxMoistureCoef = moist;
    }

    public void SetBiomeParams(int iterNum, int neighborNum)
    {
        biomeIterNumber = iterNum;
        biomeNeighborNum = neighborNum;
    }

    public void SetSoilParams(int iterNum, int neighborNum)
    {
        soilIterNumber = iterNum;
        soilNeighborNum = neighborNum;
    }

    public void SetGOResourcesParams(int iterNum, int neighborNum)
    {
        GOResourceIterNumber = iterNum;
        GOResourceNeighborNum = neighborNum;
    }

    public void SetUResourcesParam(int neighborNum)
    {
        UResourceNeighborNum = neighborNum;
    }

    public void SetPoliticsParams(int countriesNum, int minDist, int minCapHeight, int maxCapHeight, int minExpHeight)
    {
        numOfCountries = countriesNum;
        minDistanceBetweenCapitals = minDist;
        minHeightForCapital = minCapHeight;
        maxHeightForCapital = maxCapHeight;
        minHeightForExpantion = minExpHeight;
    }

    public void ResetCountriesCapitals()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                ComplexMap[i, j].Country = null;
            }
        }

        foreach (CountryBase country in countryReader.GetCountries())
            country.Capital = new Vector2(-1, -1);
    }

    public bool CheckFullGeneration()
    {
        return sizeConfirmed && heightGenerated && temperatureGenerated && moistureGenerated && biomeGenerated && soilGenerated &&
            globalResourcesGenerated && ongroundResourcesGenerated && undergroundResourcesGenerated && politicGenerated;
    }
}
