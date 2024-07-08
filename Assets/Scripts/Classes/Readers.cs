using EconomicsClasses;
using GlobalClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UtilityClasses;
using LogisticsClasses;
using WarfareClasses;

namespace Readers
{
    public class HeightReader
    {
        private Dictionary<int, HeightObject> heights;
        private string folderPath = "";

        public HeightReader(string path)
        {
            heights = new Dictionary<int, HeightObject>();
            folderPath = path;
        }

        public Dictionary<int, HeightObject> getHeightChart()
        {
            return heights;
        }

        //The data is stored in the .tsv file which is to be read here
        public void ReadHeights()
        {
            int limitIndex = 0;
            int nameIndex = 0;
            int colorIndex = 0;
            int temperIndex = 0;
            int moistIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        limitIndex = cellValues.FindIndex(c => c.Equals("Lower Limit"));
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                        temperIndex = cellValues.FindIndex(c => c.Equals("Temperature Coef"));
                        moistIndex = cellValues.FindIndex(c => c.Equals("Moisture Coef"));
                    }
                    else
                    {
                        int lowerLimit = 0;
                        if (cellValues.ElementAt(limitIndex).ToString().Equals("MinInf"))
                            lowerLimit = int.MinValue;
                        else
                            lowerLimit = int.Parse(cellValues.ElementAt(limitIndex).ToString());
                        Color color;
                        ColorUtility.TryParseHtmlString(cellValues.ElementAt(colorIndex).ToString(), out color);
                        float temperatureCoef = (float)double.Parse(cellValues.ElementAt(temperIndex).ToString());
                        float moistureCoef = (float)double.Parse(cellValues.ElementAt(moistIndex).ToString());
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        heights.Add(lowerLimit, new HeightObject(name, color, temperatureCoef, moistureCoef));
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class TemperatureReader
    {
        //Temperature coefficient - Name - Color
        private Dictionary<float, TemperatureObject> temperatures;
        private string folderPath = "";

        public TemperatureReader(string path)
        {
            temperatures = new Dictionary<float, TemperatureObject>();
            folderPath = path;
        }

        public Dictionary<float, TemperatureObject> getTemperatureChart()
        {
            return temperatures;
        }

        public void readTemperatures()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            int limitIndex = 0;
            int nameIndex = 0;
            int colorIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        limitIndex = cellValues.FindIndex(c => c.Equals("Lower Limit"));
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                    }
                    else
                    {
                        float lowerLimit = 0F;
                        if (cellValues.ElementAt(limitIndex).ToString().Equals("MinInf"))
                            lowerLimit = float.MinValue;
                        else
                            lowerLimit = (float)double.Parse(cellValues.ElementAt(limitIndex).ToString());
                        Color color;
                        ColorUtility.TryParseHtmlString(cellValues.ElementAt(colorIndex).ToString(), out color);
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        temperatures.Add(lowerLimit, new TemperatureObject(name, color));
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class MoistureReader
    {
        //Lower limit for generation - Name - Color
        private Dictionary<float, MoistureObject> moistures;
        private string folderPath = "";

        public MoistureReader(string path)
        {
            moistures = new Dictionary<float, MoistureObject>();
            folderPath = path;
        }

        public Dictionary<float, MoistureObject> getMoistureChart()
        {
            return moistures;
        }

        public void readMoistures()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            int limitIndex = 0;
            int nameIndex = 0;
            int colorIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        limitIndex = cellValues.FindIndex(c => c.Equals("Lower Limit"));
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                    }
                    else
                    {
                        float lowerLimit = 0;
                        if (cellValues.ElementAt(limitIndex).ToString().Equals("MinInf"))
                            lowerLimit = float.MinValue;
                        else
                            lowerLimit = (float)double.Parse(cellValues.ElementAt(limitIndex).ToString());
                        Color color;
                        ColorUtility.TryParseHtmlString(cellValues.ElementAt(colorIndex).ToString(), out color);
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        moistures.Add(lowerLimit, new MoistureObject(name, color));
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class ResourceReader
    {
        private List<BasicResource> resources;
        private string folderPath;

        public ResourceReader(string folderPath)
        {
            this.folderPath = folderPath;
            resources = new List<BasicResource>();
        }

        public List<BasicResource> getResources()
        {
            return resources;
        }

        public void readResources()
        {
            int idIndex = 0;
            int nameIndex = 0;
            int colorIndex = 0;
            int categIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        idIndex = cellValues.FindIndex(c => c.Equals("ID"));
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                        categIndex = cellValues.FindIndex(c => c.Equals("Category"));
                    }
                    else
                    {
                        int id = int.Parse(cellValues.ElementAt(idIndex).ToString());
                        string category = cellValues.ElementAt(categIndex).ToString();
                        Color color;
                        ColorUtility.TryParseHtmlString(cellValues.ElementAt(colorIndex).ToString(), out color);
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        resources.Add(new BasicResource(id, name, color, category));
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class BiomeReader
    {
        private List<Biome> biomes;  //List of biomes
        private Dictionary<int, List<Biome>> biomeHeightsDictionary;  //List of biomes with specified heights;
                                                                      //made for simplier reading later
        private string folderPathDefault;
        private string folderPathHeights;
        private string folderPathTemperMoist;

        //This is a complex reader
        //It reads the biome list itself, with resources it provides  DONE
        //It also reads the biome-height table and assigns the corresponding values  DONE
        //And it reads the moisture-temperature table and assigns the values  DONE
        public BiomeReader(string folderPath, string folderPathHeights, string folderPathTemperMoist)
        {
            this.folderPathDefault = folderPath;
            this.folderPathHeights = folderPathHeights;
            this.folderPathTemperMoist = folderPathTemperMoist;
            biomes = new List<Biome>();
            biomeHeightsDictionary = new Dictionary<int, List<Biome>>();
        }

        public List<Biome> getBiomes()
        {
            return biomes;
        }

        public Dictionary<int, List<Biome>> getHeightFixedBiomes()
        {
            return biomeHeightsDictionary;
        }

        public void readBiomes(ResourceReader resourceReader)
        {
            List<BasicResource> resources = resourceReader.getResources();

            int nameIndex = 0;
            int colorIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPathDefault))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                    }
                    else
                    {
                        Color color;
                        ColorUtility.TryParseHtmlString(cellValues.ElementAt(colorIndex).ToString(), out color);
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        biomes.Add(new Biome(name, color));
                        //Then go the cells with the resources the biome has
                        //If the cell is not the resource name, it will be skipped
                        for (int j = 0; j < cellValues.Count; j++)
                        {
                            BasicResource resource = resources.Where(r => r.Name.Equals(cellValues.ElementAt(j))).FirstOrDefault();
                            if (resource != null)
                                biomes.ElementAt(lineCounter - 1).addResource(resource);
                        }
                    }
                    lineCounter++;
                }
            }
        }

        public void readBiomeHeights(TemperatureReader temperatureReader)
        {
            int biomeIndex = 0;
            int limitIndex = 0;
            int temperIndex = 0;
            List<string> cellValues = new List<string>();
            List<TemperatureObject> temperatures = temperatureReader.getTemperatureChart().Values.ToList();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPathHeights))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        biomeIndex = cellValues.FindIndex(c => c.Equals("Biome"));
                        limitIndex = cellValues.FindIndex(c => c.Equals("Lower Limit"));
                        temperIndex = cellValues.FindIndex(c => c.Equals("Temperatures"));
                    }
                    else
                    {
                        //There may be multiple biomes in one cell, so split them to process
                        string[] names = cellValues.ElementAt(biomeIndex).ToString().Split(new char[] { ',', ' ' });
                        string[] tempers;
                        try
                        {
                            tempers = cellValues.ElementAt(temperIndex).ToString().Split(new char[] { ',', ' ' });
                        }
                        catch (Exception)
                        {
                            tempers = new string[0];
                        }
                        int heightLimit;
                        string limit = cellValues.ElementAt(limitIndex).ToString();
                        if (limit.Equals("MinInf"))
                            heightLimit = int.MinValue;
                        else
                            heightLimit = int.Parse(limit);
                        List<Biome> tempList = new List<Biome>();
                        for (int k = 0; k < names.Length; k++)
                        {
                            if (!string.IsNullOrEmpty(names[k]))
                            {
                                //Find needed biomes by their names
                                Biome biome = biomes.Where(b => b.getName().Equals(names[k])).FirstOrDefault();
                                if (biome != null)  //If found
                                {
                                    biome.addHeight(heightLimit);  //Set the min height to the biome
                                    for (int t = 0; t < tempers.Length; t++)  //Set the temperatures
                                    {
                                        if (!string.IsNullOrEmpty(tempers[t]))
                                        {
                                            TemperatureObject temper = temperatures.Where(tem => tem.Name.Equals(tempers[t])).
                                                FirstOrDefault();
                                            if (temper != null)  //If found, apply
                                                biome.addTemperature(temper);
                                        }
                                    }
                                    tempList.Add(biome);  //Add the biome to the list
                                }
                            }
                        }
                        biomeHeightsDictionary.Add(heightLimit, tempList);
                    }
                    lineCounter++;
                }
            }

            for (int i = 0; i < biomes.Count; i++)  //Some biomes may not aquire height until now
            {
                if (biomes.ElementAt(i).getHeights().Count() == 0)
                {
                    biomeHeightsDictionary[0].Add(biomes.ElementAt(i));  //Add the biome to the list
                    biomes.ElementAt(i).addHeight(0);  //Set the height to 0
                }
            }
        }

        public void readMoistTempers(TemperatureReader temperatureReader, MoistureReader moistureReader)
        {
            //We need to ALREADY know what temperature and moisture objects we are dealing with
            Dictionary<float, TemperatureObject> temperatures = temperatureReader.getTemperatureChart();
            Dictionary<float, MoistureObject> moistures = moistureReader.getMoistureChart();

            List<string> cellValues = new List<string>();
            List<TemperatureObject> temperatureOrder = new List<TemperatureObject>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPathTemperMoist))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        //The object 0 is dropped as it's the term-setting cell
                        //All others are temperature values and must be inspected
                        for (int j = 1; j < cellValues.Count(); j++)
                        {
                            //Find the corresponding temperature object
                            TemperatureObject temper = temperatures.Values.Where(t => t.Name.Equals(cellValues.ElementAt(j)))
                                .FirstOrDefault();
                            if (temper != null)
                                temperatureOrder.Add(temper);
                            else  //Or set to None if not found - "None" is the REQUIRED value to be in the table
                                temperatureOrder.Add(temperatures.Values.Where(t => t.Name.Equals("None")).FirstOrDefault());
                        }
                    }
                    else
                    {
                        //Same with moistures as with temperatures
                        MoistureObject moist = moistures.Values.Where(m => m.Name.Equals(cellValues.ElementAt(0))).FirstOrDefault();
                        if (moist == null)
                            moist = moistures.Values.Where(m => m.Name.Equals("None")).FirstOrDefault();

                        for (int j = 1; j < cellValues.Count(); j++)
                        {
                            //Multiple biomes must be split
                            string[] names = cellValues.ElementAt(j).Split(new char[] { ',', ' ' });
                            for (int k = 0; k < names.Length; k++)
                            {
                                if (!string.IsNullOrEmpty(names[k]))
                                {
                                    //Find the biome by the name
                                    Biome biome = biomes.Where(b => b.getName().Equals(names[k])).FirstOrDefault();
                                    if (biome != null)
                                    {
                                        biome.addTemperature(temperatureOrder.ElementAt(j - 1));  //Set the temperature
                                        biome.addMoisture(moist);  //Set the moisture
                                    }
                                }
                            }
                        }
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class SoilReader
    {
        private List<Soil> Soils;

        private string folderPath;

        public SoilReader(string folderPath)
        {
            this.folderPath = folderPath;
            Soils = new List<Soil>();
        }

        public List<Soil> getSoils()
        {
            return Soils;
        }

        public void readSoils(BiomeReader biomeReader)
        {
            List<Biome> biomes = biomeReader.getBiomes();

            int nameIndex = 0;
            int colorIndex = 0;
            int priceIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                        priceIndex = cellValues.FindIndex(c => c.Equals("Price"));
                    }
                    else
                    {
                        Color color;
                        ColorUtility.TryParseHtmlString(cellValues.ElementAt(colorIndex).ToString(), out color);
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        int price = int.Parse(cellValues.ElementAt(priceIndex).ToString());
                        Soil curSoil = new Soil(name, color, price);
                        Soils.Add(curSoil);
                        //Then go the cells with the resources the biome has
                        //If the cell is not the resource name, it will be skipped
                        for (int j = 0; j < cellValues.Count; j++)
                        {
                            Biome biome = biomes.Where(r => r.getName().Equals(cellValues.ElementAt(j))).FirstOrDefault();
                            if (biome != null)
                                biome.addSoil(curSoil);
                        }
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class CountryReader
    {
        private List<CountryBase> countries;
        private string folderPath;

        public CountryReader(string folderPath)
        {
            this.folderPath = folderPath;
            countries = new List<CountryBase>();
        }

        public List<CountryBase> GetCountries()
        {
            return countries;
        }

        public void readCountries()
        {
            int nameIndex = 0;
            int colorIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                    }
                    else
                    {
                        Color color;
                        ColorUtility.TryParseHtmlString(cellValues.ElementAt(colorIndex).ToString(), out color);
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        CountryBase curCountry = new CountryBase(name, color);
                        countries.Add(curCountry);
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class PathwayTypeReader
    {
        private List<PathwayType> pathwayTypes;
        private string folderPath;

        public PathwayTypeReader(string folderPath)
        {
            this.folderPath = folderPath;
            pathwayTypes = new List<PathwayType>();
        }

        public List<PathwayType> GetPathwayTypes()
        {
            return pathwayTypes;
        }

        public void ReadPathwayTypes()
        {
            int nameIndex = 0;
            int priceIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        priceIndex = cellValues.FindIndex(c => c.Equals("Price"));
                    }
                    else
                    {
                        int price = int.Parse(cellValues.ElementAt(priceIndex).ToString());
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        PathwayType type = new PathwayType(name, price);
                        pathwayTypes.Add(type);
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class ProductReader
    {
        private Dictionary<Product, string> products = new Dictionary<Product, string>();

        private string folderPath = "";

        public ProductReader(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public Dictionary<Product, string> getProducts()
        {
            return products;
        }

        public void Read(CountryBase country)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            int idIndex = 0;
            int nameIndex = 0;
            int priceIndex = 0;
            int amountIndex = 0;
            int timeFirstIndex = 0;
            int timeAllIndex = 0;
            int resIndex = 0;
            int indIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        idIndex = cellValues.FindIndex(c => c.Equals("ID"));
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        priceIndex = cellValues.FindIndex(c => c.Equals("Price"));
                        amountIndex = cellValues.FindIndex(c => c.Equals("Amount"));
                        timeFirstIndex = cellValues.FindIndex(c => c.Equals("Time First"));
                        timeAllIndex = cellValues.FindIndex(c => c.Equals("Time All"));
                        resIndex = cellValues.FindIndex(c => c.Equals("Resource"));
                        indIndex = cellValues.FindIndex(c => c.Equals("Industry"));
                    }
                    else
                    {
                        bool ready = true;

                        int id = int.Parse(cellValues.ElementAt(idIndex).ToString());
                        float price = float.Parse(cellValues.ElementAt(priceIndex).ToString());
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        int amount = int.Parse(cellValues.ElementAt(amountIndex).ToString());
                        IResource res = country.resourceAmmounts.Where(r => r.Key.Name.Equals(cellValues.ElementAt(resIndex))).FirstOrDefault().Key;

                        int timeFirst = 0;  //in seconds
                        int timeAll = 0;  //same here
                        int[] times = new int[4];
                        string[] parts = cellValues.ElementAt(timeFirstIndex).ToString().Split(',');
                        if (parts.Length == 4)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                times[j] = int.Parse(parts[j]);
                            }
                            timeFirst += times[3];  //seconds
                            timeFirst += times[2] * 60;  //minutes
                            timeFirst += times[1] * 3600;  //hours
                            timeFirst += times[0] * 3600 * 24;  //days
                        }
                        else
                            ready = false;
                        parts = cellValues.ElementAt(timeAllIndex).ToString().Split(',');
                        if (parts.Length == 4)
                        {
                            for (int j = 0; j < 4; j++)
                                times[j] = int.Parse(parts[j]);
                            timeAll += times[3];  //seconds
                            timeAll += times[2] * 60;  //minutes
                            timeAll += times[1] * 3600;  //hours
                            timeAll += times[0] * 3600 * 24;  //days
                        }
                        else
                            ready = false;
                        if (country.industriesPoints.Where(ind => ind.Name.Equals(cellValues.ElementAt(indIndex))).FirstOrDefault() == null)
                            ready = false;

                        if (ready)
                            country.AddNewProduct(new Product(id, name, price, amount, res, timeFirst, timeAll), cellValues.ElementAt(indIndex));
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class IndustryReader
    {
        private List<Industry> industries;
        private string folderPath;

        public IndustryReader(string folderPath)
        {
            this.folderPath = folderPath;
            industries = new List<Industry>();
        }

        public void ReadIndustries(ResourceReader resourceReader, CountryBase country)
        {
            int counter = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(folderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter != 0)
                    {
                        int grade = 0;
                        Industry industry = new Industry(counter, "Empty", grade, country);
                        IResource resource = new BasicResource(-1, "Name", Color.black, "N");
                        char letter1 = 'a';
                        bool resAddReady = true;
                        for (int k = 0; k < cellValues.Count; k++)
                        {
                            string text = cellValues.ElementAt(k);
                            if (text == "")
                                break;
                            if (k == 0)
                            {
                                grade = int.Parse(text);
                                industry.Grade = grade;
                            }
                            else if (k == 1)
                                industry.Name = text;
                            else
                            {
                                if (k % 2 == 0)  //even indexes contain resource names
                                {
                                    string resName = cellValues.ElementAt(k);
                                    letter1 = resName.ElementAt(0);
                                    if (!char.IsLetter(resName, 0))
                                        resName = resName.Remove(0, 1);
                                    BasicResource res = resourceReader.getResources().Where(r => (r as BasicResource).getName().Equals(resName)).FirstOrDefault();
                                    Industry ind = industries.Where(r => (r as Industry).Name.Equals(resName)).FirstOrDefault() as Industry;
                                    if (res != null)
                                    {
                                        resource = res;
                                    }
                                    else if (ind != null)
                                    {
                                        resource = ind;
                                    }
                                    else
                                    {
                                        resAddReady = false;
                                    }
                                }
                                else  //odd indexes contain resource ammounts
                                {
                                    string ammount = cellValues.ElementAt(k);
                                    string[] words = ammount.Split(',');
                                    int item1 = int.Parse(words[0]);
                                    int item2 = int.Parse(words[0]);
                                    if (words.Length > 1)
                                        item2 = int.Parse(words[1]);
                                    int amount = item1;
                                    if (letter1 == '!' && resAddReady)
                                    {
                                        letter1 = 'a';
                                        industry.AddRequired(resource, amount);
                                        resource = new BasicResource(-1, "Name", Color.black, "N");
                                    }
                                    else if (resAddReady)
                                    {
                                        industry.AddNeeded(resource, amount);
                                        resource = new BasicResource(-1, "Name", Color.black, "N");
                                    }
                                    else
                                        resAddReady = true;
                                }
                            }
                        }
                        country.AddIndustry(industry);
                        industries.Add(industry);
                        counter++;
                    }
                    lineCounter++;
                }
            }
        }
    }

    public class WarfareObjectsReader
    {
        private List<IObject> warfareObjects;
        private Dictionary<string, List<Tuple<string, int>>> warfareDependencies;
        private Dictionary<string, Tuple<int, int>> createTimeValues;
        private string objectFolderPath = "";
        private string dependencyFolderPath = "";

        public WarfareObjectsReader(string path, string dependencyFolderPath)
        {
            objectFolderPath = path;
            warfareObjects = new List<IObject>();
            warfareDependencies = new Dictionary<string, List<Tuple<string, int>>>();
            createTimeValues = new Dictionary<string, Tuple<int, int>>();
            this.dependencyFolderPath = dependencyFolderPath;
        }

        public List<IObject> GetWarfareObjects()
        {
            return warfareObjects;
        }

        public Dictionary<string, List<Tuple<string, int>>> GetWarfareDependencies()
        {
            return warfareDependencies;
        }

        public Dictionary<string, Tuple<int, int>> GetCreateTimeValues()
        {
            return createTimeValues;
        }

        //NEEDED MAYBE include the Product detection here?
        private void ReadObjects()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            int nameIndex = 0;
            int categoryIndex = 0;
            int timeCreateIndex = 0;
            int restoreLightIndex = 0;
            int restoreHeavyIndex = 0;
            int depleteIndex = 0;
            List<string> cellValues = new List<string>();
            int lineCounter = 0;

            using (StreamReader streamReader = new StreamReader(objectFolderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    if (lineCounter == 0)
                    {
                        nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                        categoryIndex = cellValues.FindIndex(c => c.Equals("Category"));
                        timeCreateIndex = cellValues.FindIndex(c => c.Equals("Creation Amount & Time"));
                        restoreLightIndex = cellValues.FindIndex(c => c.Equals("Fix Coef light"));
                        restoreHeavyIndex = cellValues.FindIndex(c => c.Equals("Fix Coef heavy"));
                        depleteIndex = cellValues.FindIndex(c => c.Equals("Depletion Coef"));
                    }
                    else
                    {
                        bool ready = true;

                        string category = cellValues.ElementAt(categoryIndex).ToString();
                        string name = cellValues.ElementAt(nameIndex).ToString();
                        float restoreLight = float.Parse(cellValues.ElementAt(restoreLightIndex).ToString());
                        float restoreHeavy = float.Parse(cellValues.ElementAt(restoreHeavyIndex).ToString());
                        float deplete = float.Parse(cellValues.ElementAt(depleteIndex).ToString());
                        int amount = 0;
                        int timeFirst = 0;  //in seconds

                        int[] vals = new int[5];
                        string[] parts = cellValues.ElementAt(timeCreateIndex).ToString().Split(',');
                        if (parts.Length == 5)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                vals[j] = int.Parse(parts[j]);
                            }
                            timeFirst += vals[4];  //seconds
                            timeFirst += vals[3] * 60;  //minutes
                            timeFirst += vals[2] * 3600;  //hours
                            timeFirst += vals[1] * 3600 * 24;  //days
                            amount = vals[0];
                        }
                        else
                            ready = false;
                        
                        if (ready)
                        {
                            if (category != "S")
                                warfareObjects.Add(new MovableObject(name, category, deplete, restoreLight, restoreHeavy));
                            else
                                warfareObjects.Add(new StationaryObject(name, category, deplete, restoreLight, restoreHeavy));
                            createTimeValues.Add(name, new Tuple<int, int>(amount, timeFirst));
                        }
                            
                    }
                    lineCounter++;
                }
            }
        }

        private void ReadDependencies()
        {
            List<string> cellValues = new List<string>();

            using (StreamReader streamReader = new StreamReader(dependencyFolderPath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    cellValues = line.Split("\t").ToList();
                    string objToInspect = "";
                    string nextObjToAdd = "";
                    int nextAmountToAdd = 0;
                    bool resAddReady = true;
                    for (int k = 0; k < cellValues.Count; k++)
                    {
                        string text = cellValues.ElementAt(k);
                        if (text == "")
                            break;
                        if (k == 0)
                        {
                            if (warfareObjects.Where(o => o.Name == text).FirstOrDefault() != null)
                                objToInspect = text;
                            else
                                resAddReady = false;
                        }
                        else
                        {
                            if (k % 2 != 0)  //odd indexes contain object names
                            {
                                string resName = cellValues.ElementAt(k);
                                if (warfareObjects.Where(o => o.Name == resName).FirstOrDefault() != null)
                                    nextObjToAdd = text;
                                else
                                    resAddReady = false;
                            }
                            else  //even indexes contain object ammounts
                            {
                                string ammount = cellValues.ElementAt(k);
                                if (!int.TryParse(ammount, out nextAmountToAdd))
                                    resAddReady = false;

                                if (resAddReady)
                                {
                                    if (!warfareDependencies.ContainsKey(objToInspect))
                                    {
                                        warfareDependencies.Add(objToInspect, new List<Tuple<string, int>> { new Tuple<string, int>(nextObjToAdd, nextAmountToAdd) });
                                    }
                                    else
                                    {
                                        warfareDependencies[objToInspect].Add(new Tuple<string, int>(nextObjToAdd, nextAmountToAdd));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Read()
        {
            ReadObjects();
            ReadDependencies();
        }
    }
}
