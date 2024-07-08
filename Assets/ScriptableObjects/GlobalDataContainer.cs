using GlobalClasses;
using LogisticsClasses;
using Readers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GlobalDataContainer")]
public class GlobalDataContainer : ScriptableObject
{
    public GeneratorGlobalData generatorGlobalData;

    public bool isLoadSuccessful;
    public bool isPreparingNext;
    public bool isLoadingNext;
    public bool pauseSuccessful;

    public bool reInitContainer = false;

    public int size;

    public MapPart[,] ComplexMap;
    public Dictionary<string, Texture2D> MapTextures;
    public Texture2D pathsTexture;

    public Color[] economicsColors;
    public Color[] logisticsColors;
    public Color[] warfareColors;
    public Color[] pathsColors;

    public List<CountryBase> countries;

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
    public WarfareObjectsReader warfareObjectsReader;

    public int numberOfCountries;

    public string countryUnderControl = "";

    public Vector2 selectedCoordinates;
    public bool actionValid;
    public bool unitForPosition = false;

    public int iters;  //REMOVE AFTER EVERYTHING IS DONE

    public int logisticsSize;
    public int currentLogisticID;
    public List<List<int>> logisticsMap;
    public List<LogisticObject> Nodes;
    public List<List<LogisticObject>> Routes;

    public Dictionary<string, Color> objectColors = new Dictionary<string, Color>
    {
        { "Facility", new Color32(0, 98, 255, 255) },
        { "Storage", new Color32(210, 123, 0, 255) },
        { "City", Color.green },
        { "Pathway", Color.black },
        { "Node", Color.red },
        { "Path", new Color32(7, 214, 238, 255) },
        { "LogisticsPath", new Color32(0, 47, 32, 255) },
        { "WarfareContainer", Color.green },
        { "BattlePosition", Color.red }
    };

    private string DataPath = Application.streamingAssetsPath + "/DataFilesForUnity";

    public void SetGlobalData(GeneratorGlobalData generator)
    {
        generatorGlobalData = generator;
    }

    public void Init() {
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
            warfareObjectsReader = new WarfareObjectsReader(DataPath + "/WarfareObjects.txt", DataPath + "/WarfareDependencies.txt");
            warfareObjectsReader.Read();
            resourceReader = new ResourceReader(DataPath + "/ResourceChart.txt");
            resourceReader.readResources();
            countryReader.GetCountries()[i].InitResources(resourceReader);
            industryReader.ReadIndustries(resourceReader, countryReader.GetCountries()[i]);
            productReader = new ProductReader(DataPath + "/ProductChart.txt");
            productReader.Read(countryReader.GetCountries()[i]);
            pathwayTypeReader = new PathwayTypeReader(DataPath + "/PathwayTypes.txt");
            pathwayTypeReader.ReadPathwayTypes();
            countryReader.GetCountries()[i].SetPathwayTypes(pathwayTypeReader.GetPathwayTypes());
            countryReader.GetCountries()[i].InitWarfareObjects(warfareObjectsReader);
        }

        size = generatorGlobalData.size;

        economicsColors = new Color[size * size];
        logisticsColors = new Color[size * size];
        warfareColors = new Color[size * size];
        pathsColors = new Color[size * size];

        numberOfCountries = generatorGlobalData.numOfCountries;

        //NEEDED add other economic parts when I make them into SaveLoader
        for (int i = 0; i < numberOfCountries; i++)
        {
            foreach (var resource in generatorGlobalData.countryReader.GetCountries()[i].resourceAmmounts)
                countryReader.GetCountries()[i].resourceAmmounts[resource.Key] = resource.Value;
        }

        selectedCoordinates = new Vector2(-1, -1);
        actionValid = true;
        unitForPosition = false;

        ComplexMap = new MapPart[size, size];
        MapTextures = new Dictionary<string, Texture2D>();
        pathsTexture = new Texture2D(size, size);
        pathsTexture.wrapMode = TextureWrapMode.Clamp;
        pathsTexture.Apply();

        for (int i = 0; i < generatorGlobalData.MapTextures.Count; i++)
        {
            Texture2D texture = new Texture2D(size, size);
            texture.SetPixels(generatorGlobalData.MapTextures.ElementAt(i).Value.GetPixels());
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            MapTextures.Add(string.Copy(generatorGlobalData.MapTextures.ElementAt(i).Key), texture);
        }

        //Also make the Economics and Logistics, for the new game - empty transparent textures
        Texture2D economicsTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        economicsTexture.wrapMode = TextureWrapMode.Clamp;
        Texture2D logisticsTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        logisticsTexture.wrapMode = TextureWrapMode.Clamp;
        Texture2D warfareTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        warfareTexture.wrapMode = TextureWrapMode.Clamp;
        Color basicColor = new Color(1, 1, 1, 1F);
        var fillColorArray = new Color[size * size];
        for (var i = 0; i < fillColorArray.Length; i++)
        {
            fillColorArray[i] = basicColor;
            economicsColors[i] = basicColor;
            logisticsColors[i] = basicColor;
            warfareColors[i] = basicColor;
            pathsColors[i] = Color.clear;
        }
        economicsTexture.SetPixels(economicsColors);
        logisticsTexture.SetPixels(logisticsColors);
        warfareTexture .SetPixels(warfareColors);
        pathsTexture.SetPixels(pathsColors);

        economicsTexture.Apply();
        logisticsTexture.Apply();
        warfareTexture.Apply();
        pathsTexture.Apply();

        MapTextures.Add("Economics Map", economicsTexture);
        MapTextures.Add("Logistics Map", logisticsTexture);
        MapTextures.Add("Warfare Map", warfareTexture);

        //To copy the map, we need deep copying
        //Or MAYBE not?
        int counter = 0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                MapPart part = new MapPart(counter);
                part.X = x;
                part.Y = y;
                //First copy those that copy the value
                part.Height = generatorGlobalData.ComplexMap[y, x].Height;
                part.NoHeightSpecific = generatorGlobalData.ComplexMap[y, x].NoHeightSpecific;
                //Then FIND, not copy, those that copy the link
                part.TemperatureFinal = temperatureReader.getTemperatureChart().Values.Where(o => o.Name.Equals(generatorGlobalData.ComplexMap[y, x].TemperatureFinal.Name)).First();
                part.Moisture = moistureReader.getMoistureChart().Values.Where(o => o.Name.Equals(generatorGlobalData.ComplexMap[y, x].Moisture.Name)).First();
                part.Biome = biomeReader.getBiomes().Where(o => o.getName().Equals(generatorGlobalData.ComplexMap[y, x].Biome.getName())).First();
                part.Soil = soilReader.getSoils().Where(o => o.getName().Equals(generatorGlobalData.ComplexMap[y, x].Soil.getName())).First();
                if (generatorGlobalData.ComplexMap[y, x].Country != null)
                    part.Country = countryReader.GetCountries().Where(o => o.Name.Equals(generatorGlobalData.ComplexMap[y, x].Country.Name)).First();
                else
                    part.Country = null;
                for (int k = 0; k < generatorGlobalData.ComplexMap[y, x].Resources.Count; k++)
                {
                    part.Resources.Add(resourceReader.getResources().Where(o => o.Name.Equals(generatorGlobalData.ComplexMap[y, x].Resources.ElementAt(k).Name)).First());
                }
                ComplexMap[y, x] = part;
            }
        }

        for (int i = 0; i < size; i++)  //Y coord
        {
            for (int j = 0; j < size; j++)  //X coord
            {
                if (i != 0)
                {
                    ComplexMap[i, j].neighbors.Add(ComplexMap[i - 1, j]);
                    ComplexMap[i, j].neighborIDs.Add(ComplexMap[i - 1, j].ID);
                }
                if (j != 0)
                {
                    ComplexMap[i, j].neighbors.Add(ComplexMap[i, j - 1]);
                    ComplexMap[i, j].neighborIDs.Add(ComplexMap[i, j - 1].ID);
                }
                if (i != size - 1)
                {
                    ComplexMap[i, j].neighbors.Add(ComplexMap[i + 1, j]);
                    ComplexMap[i, j].neighborIDs.Add(ComplexMap[i + 1, j].ID);
                }
                if (j != size - 1)
                {
                    ComplexMap[i, j].neighbors.Add(ComplexMap[i, j + 1]);
                    ComplexMap[i, j].neighborIDs.Add(ComplexMap[i, j + 1].ID);
                }
            }
        }
        iters = 0;
        pauseSuccessful = true;
        isPreparingNext = false;
        isLoadingNext = false;

        countries = countryReader.GetCountries();
        logisticsSize = 0;
        currentLogisticID = 0;
        Nodes = new List<LogisticObject>();
        logisticsMap = new List<List<int>>();
        Routes = new List<List<LogisticObject>>();
    }

    public void RemakeTextures()
    {
        Texture2D economicsTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        economicsTexture.wrapMode = TextureWrapMode.Clamp;
        Texture2D logisticsTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        logisticsTexture.wrapMode = TextureWrapMode.Clamp;
        Texture2D warfareTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        logisticsTexture.wrapMode = TextureWrapMode.Clamp;

        //(0, 0) is BOTTOM LEFT CORNER
        economicsTexture.SetPixels(economicsColors);
        logisticsTexture.SetPixels(logisticsColors);
        warfareTexture.SetPixels(warfareColors);
        economicsTexture.Apply();
        logisticsTexture.Apply();
        warfareTexture.Apply();

        MapTextures["Economics Map"] = economicsTexture;
        MapTextures["Logistics Map"] = logisticsTexture;
        MapTextures["Warfare Map"] = warfareTexture;
    }

    public void ClearPathTexture()
    {
        Color basicColor = new Color(1, 1, 1, 0F);
        int sizeSquare = size * size;
        for (var i = 0; i < sizeSquare; i++)
        {
            pathsColors[i] = basicColor;
        }
        pathsTexture.SetPixels(pathsColors);
        pathsTexture.Apply();
    }

    public void RemakePathTexture()
    {
        //(0, 0) is BOTTOM LEFT CORNER
        pathsTexture.SetPixels(pathsColors);
        pathsTexture.Apply();
    }
}
