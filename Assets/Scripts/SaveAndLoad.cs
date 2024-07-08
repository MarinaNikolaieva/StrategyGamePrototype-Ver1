using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using GlobalClasses;
using System.Linq;
using System.Threading;
using UtilityClasses;
using EconomicsClasses;
using LogisticsClasses;
using WarfareClasses;
using WarfareMechanics;
using System.Text;
using System;

public class SaveAndLoad
{
    public GeneratorGlobalData generatorGlobalData;
    public GlobalDataContainer globalDataContainer;
    public GlobalDataController globalDataController;

    public GameObject CallerPanel;  //Either Save or Load panel
    public GameObject ErrorPanel;

    public SaveAndLoad() {}

    public void ResetData(bool reset)
    {
        if (reset)
            generatorGlobalData.ResetAll();
    }

    public void Save()
    {
        //Assuming that by now the map is fully generated and the file name is confirmed
        //Output map to .tsv file
        string fileName;
        if (CallerPanel.transform.Find("SaveNameInput").GetComponent<InputField>().text == "")
            fileName = CallerPanel.transform.Find("SaveNameInput").GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text;
        else
            fileName = CallerPanel.transform.Find("SaveNameInput").GetComponent<InputField>().text;

        string outputPath = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + ".tsv";
        StreamWriter streamWriter = new StreamWriter(outputPath);
        streamWriter.WriteLine(generatorGlobalData.size.ToString());

        for (int i = 0; i < generatorGlobalData.size; i++)  //Y coord
        {
            for (int j = 0; j < generatorGlobalData.size; j++)  //X coord
            {
                string str = generatorGlobalData.ComplexMap[i, j].Output();
                streamWriter.WriteLine(str);
            }
        }
        streamWriter.Close();
    }
    public void SaveTotal()
    {
        //This function saves not the map, BUT all the objects on it
        //Output the maps to .txt files as they aren't displayed on the load screen
        string fileName;
        if (CallerPanel.transform.Find("SaveNameInput").GetComponent<InputField>().text == "")
            fileName = CallerPanel.transform.Find("SaveNameInput").GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text;
        else
            fileName = CallerPanel.transform.Find("SaveNameInput").GetComponent<InputField>().text;

        string outputPathE = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Economics.txt";
        string outputPathL = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Logistics.txt";
        string outputPathU = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Utility.txt";
        string outputPathW = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Warfare.txt";

        if (globalDataContainer == null || globalDataContainer.countryUnderControl == "")
            return;

        //Economics
        StreamWriter streamWriter = new StreamWriter(outputPathE);
        streamWriter.WriteLine(globalDataContainer.countryUnderControl);

        foreach (CountryBase country in globalDataContainer.countries)
        {
            streamWriter.WriteLine(country.Name);
            for (int i = 0; i < country.storages.Count; i++)
            {
                StringBuilder buildS = new StringBuilder();
                buildS.Append("Storage\t");
                buildS.Append(country.storages[i].Name);
                buildS.Append("\t");
                buildS.Append(country.storages[i].Coords.x);
                buildS.Append("\t");
                buildS.Append(country.storages[i].Coords.y);
                buildS.Append("\t");
                buildS.Append(country.storages[i].GetCapacity());
                buildS.Append("\t");
                buildS.Append(country.storages[i].GetFreeCapacity());
                streamWriter.WriteLine(buildS.ToString());

                foreach (var prod in country.storages[i].GetProducts())
                {
                    StringBuilder buildProd = new StringBuilder();
                    buildProd.Append("ProductS\t");
                    buildProd.Append(prod.Key.ID);
                    buildProd.Append("\t");
                    buildProd.Append(prod.Key.Name);
                    buildProd.Append("\t");
                    buildProd.Append(prod.Value);
                    streamWriter.WriteLine(buildProd.ToString());
                }
            }

            for (int i = 0; i < country.cities.Count; i++)
            {
                StringBuilder buildC = new StringBuilder();
                buildC.Append("City\t");
                buildC.Append(country.cities[i].Name);
                buildC.Append("\t");
                buildC.Append(country.cities[i].Coords.x);
                buildC.Append("\t");
                buildC.Append(country.cities[i].Coords.y);
                streamWriter.WriteLine(buildC.ToString());
            }

            foreach (Industry industry in country.industriesPoints)
            {
                StringBuilder buildI = new StringBuilder();
                buildI.Append("Industry\t");
                buildI.Append(industry.Name);
                buildI.Append("\t");
                buildI.Append(industry.Points);
                buildI.Append("\t");
                buildI.Append(industry.FreePoints);
                streamWriter.WriteLine(buildI.ToString());

                foreach (Facility facility in industry.facilities)
                {
                    StringBuilder buildF = new StringBuilder();
                    buildF.Append("Facility\t");
                    buildF.Append(facility.Name);
                    buildF.Append("\t");
                    buildF.Append(facility.Coords.x);
                    buildF.Append("\t");
                    buildF.Append(facility.Coords.y);
                    buildF.Append("\t");
                    buildF.Append(facility.parentIndustry.Name);
                    buildF.Append("\t");
                    buildF.Append(facility.GetTotalProductionPoints());
                    streamWriter.WriteLine(buildF.ToString());

                    foreach (var prod in facility.GetProduction())
                    {
                        StringBuilder buildProd = new StringBuilder();
                        buildProd.Append("Production\t");
                        buildProd.Append(prod.Key.ID);
                        buildProd.Append("\t");
                        buildProd.Append(prod.Key.Name);
                        buildProd.Append("\t");
                        buildProd.Append(prod.Value);
                        streamWriter.WriteLine(buildProd.ToString());
                    }
                }
            }
        }
        streamWriter.Close();

        //Logistics
        streamWriter = new StreamWriter(outputPathL);
        foreach (CountryBase country in globalDataContainer.countries)
        {
            streamWriter.WriteLine(country.Name);
            foreach (var pathway in country.pathways)
            {
                StringBuilder buildPath = new StringBuilder();
                buildPath.Append("Pathway\t");
                buildPath.Append(pathway.BeginCoordinates.x);
                buildPath.Append("\t");
                buildPath.Append(pathway.BeginCoordinates.y);
                buildPath.Append("\t");
                buildPath.Append(pathway.EndCoordinates.x);
                buildPath.Append("\t");
                buildPath.Append(pathway.EndCoordinates.y);
                buildPath.Append("\t");
                buildPath.Append(pathway.Type.Name);
                streamWriter.WriteLine(buildPath.ToString());
            }
        }
        foreach (var path in globalDataContainer.Routes)
        {
            StringBuilder buildPath = new StringBuilder();
            buildPath.Append("Path\t");
            buildPath.Append(path.First().Coordinates.Item1.x);
            buildPath.Append("\t");
            buildPath.Append(path.First().Coordinates.Item1.y);
            buildPath.Append("\t");
            buildPath.Append(path.Last().Coordinates.Item1.x);
            buildPath.Append("\t");
            buildPath.Append(path.Last().Coordinates.Item1.y);
            streamWriter.WriteLine(buildPath.ToString());
        }
        streamWriter.Close();

        //Utility
        streamWriter = new StreamWriter(outputPathU);
        foreach (CountryBase country in globalDataContainer.countries)
        {
            streamWriter.WriteLine(country.Name);
            foreach (var product in country.productAmounts)
            {
                StringBuilder buildProd = new StringBuilder();
                buildProd.Append("Product\t");
                buildProd.Append(product.Key.ID);
                buildProd.Append("\t");
                buildProd.Append(product.Key.Name);
                buildProd.Append("\t");
                buildProd.Append(product.Key.Price);
                buildProd.Append("\t");
                buildProd.Append(product.Value);
                streamWriter.WriteLine(buildProd.ToString());
            }
            foreach (var type in country.pathwayTypes)
            {
                StringBuilder buildProd = new StringBuilder();
                buildProd.Append("PathwayType\t");
                buildProd.Append(type.Name);
                buildProd.Append("\t");
                buildProd.Append(type.GetPrice());
                streamWriter.WriteLine(buildProd.ToString());
            }
        }
        streamWriter.Close();

        //Warfare
        streamWriter = new StreamWriter(outputPathW);
        foreach (CountryBase country in globalDataContainer.countries)
        {
            streamWriter.WriteLine(country.Name);
            foreach (ContainerObject container in country.warfareContainerObjects)
            {
                StringBuilder build = new StringBuilder();
                switch (container.GetType().Name)
                {
                    case "Hospital":
                        build.Append("Hospital\t");
                        break;
                    case "Headquaters":
                        build.Append("Headquaters\t");
                        break;
                    case "Workshop":
                        build.Append("Workshop\t");
                        break;
                    case "TrainingPlace":
                        build.Append("Training Field\t");
                        break;
                }
                build.Append(container.CustomName);
                build.Append("\t");
                build.Append(container.Coords.x);
                build.Append("\t");
                build.Append(container.Coords.y);
                build.Append("\t");
                switch (container.GetType().Name)
                {
                    case "Hospital":
                        build.Append((container as Hospital).Capacity + "\t");
                        build.Append((container as Hospital).CurrentCapacity);
                        break;
                    case "Headquaters":
                        build.Append((container as Headquaters).Capacity + "\t");
                        build.Append((container as Headquaters).CurrentCapacity);
                        break;
                    case "Workshop":
                        build.Append((container as Workshop).Capacity + "\t");
                        build.Append((container as Workshop).CurrentCapacity);
                        break;
                    case "TrainingPlace":
                        build.Append((container as TrainingPlace).Capacity);
                        break;
                }
                streamWriter.WriteLine(build.ToString());

                switch (container.GetType().Name)
                {
                    case "Hospital":
                        Hospital hosp = (Hospital)container;
                        foreach (var unit in hosp.ToFix)
                        {
                            StringBuilder buildU = new StringBuilder();
                            buildU.Append("UnitHP\t");
                            buildU.Append(unit.Key.Name);
                            buildU.Append("\t");
                            buildU.Append(unit.Key.Resource.ToString("0.####"));
                            buildU.Append("\t");
                            buildU.Append(unit.Value);
                            streamWriter.WriteLine(buildU.ToString());
                        }
                        foreach (var unit in hosp.Fixed)
                        {
                            StringBuilder buildU = new StringBuilder();
                            buildU.Append("UnitHP\t");
                            buildU.Append(unit.Key.Name);
                            buildU.Append("\t");
                            buildU.Append(unit.Key.Resource.ToString("0.####"));
                            buildU.Append("\t");
                            buildU.Append(unit.Value);
                            streamWriter.WriteLine(buildU.ToString());
                        }
                        break;
                    case "Workshop":
                        Workshop work = (Workshop)container;
                        foreach (var unit in work.ToFix)
                        {
                            StringBuilder buildU = new StringBuilder();
                            buildU.Append("UnitW\t");
                            buildU.Append(unit.Key.Name);
                            buildU.Append("\t");
                            buildU.Append(unit.Key.Resource.ToString("0.####"));
                            buildU.Append("\t");
                            buildU.Append(unit.Value);
                            streamWriter.WriteLine(buildU.ToString());
                        }
                        foreach (var unit in work.Fixed)
                        {
                            StringBuilder buildU = new StringBuilder();
                            buildU.Append("UnitW\t");
                            buildU.Append(unit.Key.Name);
                            buildU.Append("\t");
                            buildU.Append(unit.Key.Resource.ToString("0.####"));
                            buildU.Append("\t");
                            buildU.Append(unit.Value);
                            streamWriter.WriteLine(buildU.ToString());
                        }
                        break;
                    case "Headquaters":
                        Headquaters head = (Headquaters)container;
                        foreach (var unit in head.Contained)
                        {
                            StringBuilder buildU = new StringBuilder();
                            buildU.Append("UnitHQ\t");
                            buildU.Append(unit.Key.Name);
                            buildU.Append("\t");
                            buildU.Append(unit.Key.Resource.ToString("0.####"));
                            buildU.Append("\t");
                            buildU.Append(unit.Value);
                            streamWriter.WriteLine(buildU.ToString());
                        }
                        break;
                }
            }
            foreach (var item in country.battlePositions)
            {
                StringBuilder buildBP = new StringBuilder();
                buildBP.Append("BattlePosition\t");
                buildBP.Append(item.Position.x);
                buildBP.Append("\t");
                buildBP.Append(item.Position.y);
                streamWriter.WriteLine(buildBP.ToString());
                foreach (var unit in item.HumanAndTechResources)
                {
                    StringBuilder buildU = new StringBuilder();
                    buildU.Append("UnitBPHT\t");
                    buildU.Append(unit.Key.Name);
                    buildU.Append("\t");
                    buildU.Append(unit.Key.Resource.ToString("0.####"));
                    buildU.Append("\t");
                    buildU.Append(unit.Value);
                    streamWriter.WriteLine(buildU.ToString());
                }
                foreach (var unit in item.StationaryPlaces)
                {
                    StringBuilder buildU = new StringBuilder();
                    buildU.Append("UnitBPSP\t");
                    buildU.Append(unit.Key.Name);
                    buildU.Append("\t");
                    buildU.Append(unit.Key.Resource.ToString("0.####"));
                    buildU.Append("\t");
                    buildU.Append(unit.Value);
                    streamWriter.WriteLine(buildU.ToString());
                }
                foreach (var unit in item.TempOutOfActionObjects)
                {
                    StringBuilder buildU = new StringBuilder();
                    buildU.Append("UnitBPOA\t");
                    buildU.Append(unit.Key.Name);
                    buildU.Append("\t");
                    buildU.Append(unit.Key.Resource.ToString("0.####"));
                    buildU.Append("\t");
                    buildU.Append(unit.Value);
                    streamWriter.WriteLine(buildU.ToString());
                }
            }
            foreach (var item in country.objectBank.Bank)
            {
                StringBuilder buildU = new StringBuilder();
                buildU.Append("BankUnit\t");
                buildU.Append(item.Key.Name);
                buildU.Append("\t");
                buildU.Append(item.Key.Resource.ToString("0.####"));
                buildU.Append("\t");
                buildU.Append(item.Value);
                streamWriter.WriteLine(buildU.ToString());
            }
        }
        streamWriter.Close();
    }

    private void CorruptedDataErrorOpen()
    {
        ErrorPanel.SetActive(false);
        ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text =
            "ERROR!\nThe data is corrupted!\nImport can't be completed!";
        UnityEngine.Object.FindObjectsOfType<Button>(true).Where(o => o.name.Equals("AgreeButton")).First().GetComponent<Button>().interactable = false;
        //ErrorPanel.transform.Find("AgreeButton").GetComponent<Button>().interactable = false;
        globalDataContainer.isLoadSuccessful = false;
        globalDataContainer.isLoadingNext = false;
        globalDataContainer.isPreparingNext = false;
        ErrorPanel.SetActive(true);
    }

    public void Load(string panelElementName)
    {
        //Here the .tsv file is READ and the errors are handled, too
        //And here the ErrorPanel is needed
        string inputName = panelElementName + "Input";

        string fileName;
        if (CallerPanel.transform.Find(inputName).GetComponent<InputField>().text == "")
            fileName = CallerPanel.transform.Find(inputName).GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text;
        else
            fileName = CallerPanel.transform.Find(inputName).GetComponent<InputField>().text;

        string filePath = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + ".tsv";
        //Clear all maps that were prior
        generatorGlobalData.ResetAll();
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
        bool firstLineRead = false;

        float maxIndex = 0;
        List<string> countriesFound = new List<string>();

        using (StreamReader streamReader = new StreamReader(filePath))  //open file
        {
            while (streamReader.Peek() != -1)
            {
                if (!firstLineRead)
                {
                    firstLineRead = true;
                    string line = streamReader.ReadLine();
                    int size = 0;
                    if (!int.TryParse(line, out size))
                    {
                        ErrorPanel.SetActive(true);
                        ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text =
                            "ERROR!\nThe size of the map is not a number!\nImport can't be completed!";
                        generatorGlobalData.ResetAll();
                        globalDataContainer.isLoadSuccessful = false;
                        return;
                    }
                    else
                    {
                        generatorGlobalData.size = size;
                        generatorGlobalData.ComplexMap = new MapPart[size, size];
                        generatorGlobalData.sizeConfirmed = true;
                        maxIndex = generatorGlobalData.size * generatorGlobalData.size;
                        firstLineRead = true;
                    }
                }
                else
                {
                    for (int y = 0; y < generatorGlobalData.size; y++)
                    {
                        for (int x = 0; x < generatorGlobalData.size; x++)
                        {
                            string line = streamReader.ReadLine();
                            string[] lineParts = line.Split('\t', ' ');

                            if (int.Parse(lineParts[1]) != x || int.Parse(lineParts[2]) != y)
                            {
                                ErrorPanel.SetActive(true);
                                ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text =
                                    "ERROR!\nThe size of the map is corrupted!\nImport can't be completed!";
                                generatorGlobalData.ResetAll();
                                globalDataContainer.isLoadSuccessful = false;
                                return;
                            }

                            //NEEDED add exceptions here!  GUESS DONE
                            int id = -1;
                            if (!int.TryParse(lineParts[0], out id))
                            {
                                CorruptedDataErrorOpen();
                                generatorGlobalData.ResetAll();
                                return;
                            }
                            generatorGlobalData.ComplexMap[y, x] = new MapPart(id);
                            if (!int.TryParse(lineParts[1], out generatorGlobalData.ComplexMap[y, x].X))
                            {
                                CorruptedDataErrorOpen();
                                generatorGlobalData.ResetAll();
                                return;
                            }
                            //generatorGlobalData.ComplexMap[y, x].X = int.Parse(lineParts[1]);
                            if (!int.TryParse(lineParts[2], out generatorGlobalData.ComplexMap[y, x].Y))
                            {
                                CorruptedDataErrorOpen();
                                generatorGlobalData.ResetAll();
                                return;
                            }
                            //generatorGlobalData.ComplexMap[y, x].Y = int.Parse(lineParts[2]);
                            if (!int.TryParse(lineParts[3], out generatorGlobalData.ComplexMap[y, x].Height))
                            {
                                CorruptedDataErrorOpen();
                                generatorGlobalData.ResetAll();
                                return;
                            }
                            //generatorGlobalData.ComplexMap[y, x].Height = int.Parse(lineParts[3]);
                            generatorGlobalData.ComplexMap[y, x].TemperatureFinal = generatorGlobalData.temperatureReader.getTemperatureChart().Values.Where(c => c.Name.Equals(lineParts[4])).FirstOrDefault();
                            generatorGlobalData.ComplexMap[y, x].Moisture = generatorGlobalData.moistureReader.getMoistureChart().Values.Where(c => c.Name.Equals(lineParts[5])).FirstOrDefault();
                            generatorGlobalData.ComplexMap[y, x].Biome = generatorGlobalData.biomeReader.getBiomes().Where(c => c.getName().Equals(lineParts[6])).FirstOrDefault();
                            generatorGlobalData.ComplexMap[y, x].Soil = generatorGlobalData.soilReader.getSoils().Where(c => c.getName().Equals(lineParts[7])).FirstOrDefault();
                            if (generatorGlobalData.ComplexMap[y, x].TemperatureFinal == null ||
                                generatorGlobalData.ComplexMap[y, x].Moisture == null || generatorGlobalData.ComplexMap[y, x].Biome == null ||
                                generatorGlobalData.ComplexMap[y, x].Soil == null)
                            {
                                CorruptedDataErrorOpen();
                                generatorGlobalData.ResetAll();
                                return;
                            }
                            generatorGlobalData.ComplexMap[y, x].Country = generatorGlobalData.countryReader.GetCountries().Where(c => c.Name.Equals(lineParts[9])).FirstOrDefault();

                            if (generatorGlobalData.ComplexMap[y, x].Country != null && !countriesFound.Contains(generatorGlobalData.ComplexMap[y, x].Country.Name))
                                countriesFound.Add(generatorGlobalData.ComplexMap[y, x].Country.Name);

                            string[] tempResources = lineParts[8].Split(',');
                            for (int i = 0; i < tempResources.Length; i++)
                            {
                                BasicResource resource = generatorGlobalData.resourceReader.getResources().Where(r => r.Name.Equals(tempResources[i])).FirstOrDefault();
                                if (resource != null)
                                {
                                    generatorGlobalData.ComplexMap[y, x].Resources.Add(resource);
                                    if (generatorGlobalData.ComplexMap[y, x].Country != null)
                                        generatorGlobalData.ComplexMap[y, x].Country.ChangeResourceAmount(resource, 1);

                                }
                            }

                            string[] tempIndexes = lineParts[10].Split(',');
                            for (int i = 0; i < tempIndexes.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(tempIndexes[i]))
                                {
                                    int neighborIndex = -1;
                                    if (!int.TryParse(tempIndexes[i], out neighborIndex))
                                    {
                                        CorruptedDataErrorOpen();
                                        generatorGlobalData.ResetAll();
                                        return;
                                    }
                                    if (neighborIndex < 0 || neighborIndex >= maxIndex)
                                    {
                                        ErrorPanel.SetActive(true);
                                        ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text =
                                            "ERROR!\nThe cell neighbors in the data don't fit the map size!\nImport can't be completed!";
                                        generatorGlobalData.ResetAll();
                                        globalDataContainer.isLoadSuccessful = false;
                                        return;
                                    }
                                    generatorGlobalData.ComplexMap[y, x].neighborIDs.Add(neighborIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        //When loading / restarting the world, the user MUST NOT enter the Generator!
        generatorGlobalData.TemperMoistPartialGenerator = new TemperMoistPartialGenerator(generatorGlobalData.size);
        //This is the PREPARING part where the data goes
        Dropdown dropdown = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("CountrySelectDropdown")).First().transform.GetComponent<Dropdown>();
        for (int i = 0; i < countriesFound.Count; i++)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = countriesFound.ElementAt(i) });
        }
        dropdown.value = 0;
        dropdown.RefreshShownValue();

        Texture2D hTexture = new HeightGenerator(generatorGlobalData.size, generatorGlobalData.size, null).MakeBitmapFromPremade(generatorGlobalData.ComplexMap, generatorGlobalData.heightReader);
        hTexture.wrapMode = TextureWrapMode.Clamp;
        hTexture.Apply();
        Texture2D tTexture = new TemperatureGenerator(generatorGlobalData.size, generatorGlobalData.temperatureReader, generatorGlobalData.TemperMoistPartialGenerator, generatorGlobalData.minimumMapVal).MakePictureFromPremade(generatorGlobalData.ComplexMap);
        tTexture.wrapMode = TextureWrapMode.Clamp;
        tTexture.Apply();
        Texture2D mTexture = new MoistureGenerator(generatorGlobalData.size, generatorGlobalData.moistureReader, generatorGlobalData.TemperMoistPartialGenerator, generatorGlobalData.minimumMapVal).MakePictureFromPremade(generatorGlobalData.ComplexMap);
        mTexture.wrapMode = TextureWrapMode.Clamp;
        mTexture.Apply();
        Texture2D bTexture = new BiomeGenerator(generatorGlobalData.biomeReader, generatorGlobalData.size, generatorGlobalData.ComplexMap).MakePictureFromPremade();
        bTexture.wrapMode = TextureWrapMode.Clamp;
        bTexture.Apply();
        Texture2D sTexture = new SoilGenerator(generatorGlobalData.size, generatorGlobalData.ComplexMap).MakePictureFromPremade();
        sTexture.wrapMode = TextureWrapMode.Clamp;
        sTexture.Apply();

        generatorGlobalData.MapTextures.Add("Height Map", hTexture);
        generatorGlobalData.MapTextures.Add("Temperature Map", tTexture);
        generatorGlobalData.MapTextures.Add("Moisture Map", mTexture);
        generatorGlobalData.MapTextures.Add("Biome Map", bTexture);
        generatorGlobalData.MapTextures.Add("Soil Map", sTexture);

        List<string> categs = generatorGlobalData.resourceReader.getResources().Select(c => c.Category).Distinct().ToList();
        for (int i = 0; i < categs.Count; i++)
        {
            Texture2D texture = new ResourcesGenerator(generatorGlobalData.ComplexMap, generatorGlobalData.size, generatorGlobalData.resourceReader).MakePicture(categs.ElementAt(i));
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            if (categs.ElementAt(i).Equals("G"))
                generatorGlobalData.MapTextures.Add("Global Resource Map", texture);
            else if (categs.ElementAt(i).Equals("O"))
                generatorGlobalData.MapTextures.Add("OnGround Resource Map", texture);
            else
                generatorGlobalData.MapTextures.Add(categs.ElementAt(i) + " Resource Map", texture);
        }
        Texture2D pTexture = new CountriesGenerator(generatorGlobalData.countryReader, generatorGlobalData.ComplexMap, generatorGlobalData.size).MakePicture();
        pTexture.wrapMode = TextureWrapMode.Clamp;
        pTexture.Apply();
        generatorGlobalData.MapTextures.Add("Political Map", pTexture);

        generatorGlobalData.heightGenerated = true;
        generatorGlobalData.temperatureGenerated = true;
        generatorGlobalData.moistureGenerated = true;
        generatorGlobalData.biomeGenerated = true;
        generatorGlobalData.soilGenerated = true;
        generatorGlobalData.globalResourcesGenerated = true;
        generatorGlobalData.ongroundResourcesGenerated = true;
        generatorGlobalData.undergroundResourcesGenerated = true;
        generatorGlobalData.politicGenerated = true;

        globalDataContainer.countryUnderControl = "";
        globalDataContainer.isLoadSuccessful = true;
    }

    //NEEDED Add battle positions here!  DONE
    public void LoadTotal(string panelElementName, GlobalDataController dataController)
    {
        //This function loads the map objects
        //The map is already loaded
        string fileName;
        string inputName = panelElementName + "Input";
        if (CallerPanel.transform.Find(inputName).GetComponent<InputField>().text == "")
            fileName = CallerPanel.transform.Find(inputName).GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text;
        else
            fileName = CallerPanel.transform.Find(inputName).GetComponent<InputField>().text;

        string outputPathE = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Economics.txt";
        string outputPathL = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Logistics.txt";
        string outputPathU = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Utility.txt";
        string outputPathW = Application.streamingAssetsPath + "/Saves" + "\\" + fileName + "Warfare.txt";

        if (globalDataContainer == null)
            return;

        globalDataContainer.Init();
        globalDataController = dataController;
        globalDataController.InitDataContainer(globalDataContainer);

        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
        int lineCounter = 0;
        int countryIndex = 0;
        int industryIndex = 0;
        int temp = 0;
        int id = 0;
        int x = 0;
        int y = 0;

        int index;

        using (StreamReader streamReader = new StreamReader(outputPathE))  //Read economics
        {
            while (streamReader.Peek() != -1)
            {
                string line = streamReader.ReadLine();
                if (lineCounter == 0)
                {
                    lineCounter++;
                    if (!globalDataContainer.countries.Select(c => c.Name).ToList().Contains(line))
                    {
                        CorruptedDataErrorOpen();
                        return;
                    }
                    else
                        globalDataContainer.countryUnderControl = line;
                }
                else
                {
                    List<string> parts = line.Split('\t').ToList();
                    if (parts[0].Contains("Country") && !globalDataContainer.countries.Select(c => c.Name).Contains(parts[0]))
                    {
                        CorruptedDataErrorOpen();
                        return;
                    }
                    else if (parts[0].Contains("Country"))
                    {
                        countryIndex = globalDataContainer.countries.Select(c => c.Name).ToList().FindIndex(c => c == parts[0]);
                    }
                    else
                    {
                        switch (parts[0])
                        {
                            case "Storage":
                                x = 0;
                                if (!int.TryParse(parts[2], out x) || x < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                y = 0;
                                if (!int.TryParse(parts[3], out y) || y < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                temp = 0;
                                if (!int.TryParse(parts[4], out temp) || temp < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                globalDataContainer.countries[countryIndex].AddStorage(globalDataContainer.countries[countryIndex].lastStorageID, parts[1],
                                    new Vector2(x, y), 0, temp);
                                globalDataContainer.countries[countryIndex].lastStorageID++;
                                index = (int)y * globalDataContainer.size + (int)x;
                                globalDataContainer.economicsColors[index] = globalDataContainer.objectColors["Storage"];
                                globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                                globalDataContainer.RemakeTextures();
                                globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "Storage",
                                    new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                                globalDataContainer.currentLogisticID++;
                                break;
                            case "ProductS":
                                id = 0;
                                if (!int.TryParse(parts[1], out id))
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                if (!globalDataContainer.countries[countryIndex].productAmounts.Keys.Select(p => p.Name).Contains(parts[2]) ||
                                    globalDataContainer.countries[countryIndex].productAmounts.Keys.Where(p => p.Name == parts[2]).First().ID != id)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                temp = 0;
                                if (!int.TryParse(parts[3], out temp) || temp < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                globalDataContainer.countries[countryIndex].storages.Last().AddProduct(globalDataContainer.countries[countryIndex].productAmounts.Keys.Where(p => p.Name == parts[2]).First(), temp);
                                break;
                            case "City":
                                x = 0;
                                if (!int.TryParse(parts[2], out x) || x < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                y = 0;
                                if (!int.TryParse(parts[3], out y) || y < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                globalDataContainer.countries[countryIndex].AddCity(new Vector2(x, y), parts[1]);
                                index = (int)y * globalDataContainer.size + (int)x;
                                globalDataContainer.economicsColors[index] = globalDataContainer.objectColors["City"];
                                globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                                globalDataContainer.RemakeTextures();
                                globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "City",
                                    new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                                globalDataContainer.currentLogisticID++;
                                break;
                            case "Industry":
                                if (!globalDataContainer.countries[countryIndex].industriesPoints.Select(i => i.Name).Contains(parts[1]))
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                industryIndex = globalDataContainer.countries[countryIndex].industriesPoints.Select(i => i.Name).ToList().FindIndex(c => c == parts[1]);
                                temp = 0;
                                if (!int.TryParse(parts[2], out temp) || temp < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                globalDataContainer.countries[countryIndex].industriesPoints[industryIndex].Points = temp;
                                if (!int.TryParse(parts[3], out temp) || temp < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                globalDataContainer.countries[countryIndex].industriesPoints[industryIndex].FreePoints = temp;
                                break;
                            case "Facility":
                                if (!globalDataContainer.countries[countryIndex].industriesPoints.Select(i => i.Name).Contains(parts[4]) ||
                                    globalDataContainer.countries[countryIndex].industriesPoints.Select(i => i.Name).ToList().FindIndex(c => c == parts[4]) != industryIndex)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                industryIndex = globalDataContainer.countries[countryIndex].industriesPoints.Select(i => i.Name).ToList().FindIndex(c => c == parts[4]);
                                x = 0;
                                if (!int.TryParse(parts[2], out x) || x < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                y = 0;
                                if (!int.TryParse(parts[3], out y) || y < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                temp = 0;
                                if (!int.TryParse(parts[5], out temp) || temp < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                globalDataContainer.countries[countryIndex].AddFacility(globalDataContainer.countries[countryIndex].industriesPoints[industryIndex],
                                    globalDataContainer.countries[countryIndex].lastFacilityID, parts[1], new Vector2(x, y), 0);
                                globalDataContainer.countries[countryIndex].lastFacilityID++;
                                index = (int)y * globalDataContainer.size + (int)x;
                                globalDataContainer.economicsColors[index] = globalDataContainer.objectColors["Facility"];
                                globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                                globalDataContainer.RemakeTextures();
                                globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "Facility",
                                    new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                                globalDataContainer.currentLogisticID++;
                                break;
                            case "Production":
                                id = 0;
                                if (!int.TryParse(parts[1], out id) || id < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                if (!globalDataContainer.countries[countryIndex].productAmounts.Keys.Select(p => p.Name).Contains(parts[2]) ||
                                    !globalDataContainer.countries[countryIndex].productAmounts.Keys.Where(p => p.Name.Equals(parts[2])).Select(p => p.ID).Contains(id) ||
                                    globalDataContainer.countries[countryIndex].productIndustriesDependency.Where(p => p.Key.ID == id).First().Value.ID != industryIndex)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                temp = 0;
                                if (!int.TryParse(parts[3], out temp) || temp < 0)
                                {
                                    CorruptedDataErrorOpen();
                                    return;
                                }
                                globalDataContainer.countries[countryIndex].industriesPoints[industryIndex].facilities.Last().AddProduct(globalDataContainer.countries[countryIndex].productAmounts.Keys.Where(p => p.ID == id).First());
                                globalDataContainer.countries[countryIndex].industriesPoints[industryIndex].facilities.Last().IncreaseProduct(globalDataContainer.countries[countryIndex].productAmounts.Keys.Where(p => p.ID == id).First(), temp - 1);
                                break;
                        }
                    }
                }
            }
        }

        using (StreamReader streamReader = new StreamReader(outputPathL))  //Read logistics
        {
            while (streamReader.Peek() != -1)
            {
                string line = streamReader.ReadLine();
                Vector2 start;
                Vector2 end;
                List<string> parts = line.Split('\t').ToList();
                if (parts[0].Contains("Country") && !globalDataContainer.countries.Select(c => c.Name).Contains(parts[0]))
                {
                    CorruptedDataErrorOpen();
                    return;
                }
                else if (parts[0].Contains("Country"))
                {
                    countryIndex = globalDataContainer.countries.Select(c => c.Name).ToList().FindIndex(c => c == parts[0]);
                }
                else
                {
                    switch (parts[0])
                    {
                        case "Pathway":
                            x = 0;
                            if (!int.TryParse(parts[1], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[2], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            start = new Vector2(x, y);
                            x = 0;
                            if (!int.TryParse(parts[3], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[4], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            end = new Vector2(x, y);
                            if (!globalDataContainer.countries[countryIndex].pathwayTypes.Select(t => t.Name).Contains(parts[4]))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].AddPathway(start, end, globalDataContainer.countries[countryIndex].pathwayTypes.Where(t => t.Name == parts[4]).First());
                            globalDataController.AddEdge(start, end, globalDataContainer.countries[countryIndex].pathways.Last().Parts.Count);
                            for (int i = 0; i < globalDataContainer.countries[countryIndex].pathways.Last().Parts.Count; i++)
                            {
                                if (i != 0 && i != globalDataContainer.countries[countryIndex].pathways.Last().Parts.Count - 1)
                                {
                                    index = (int)globalDataContainer.countries[countryIndex].pathways.Last().Parts.ElementAt(i).y * globalDataContainer.size +
                                        (int)globalDataContainer.countries[countryIndex].pathways.Last().Parts.ElementAt(i).x;
                                    globalDataContainer.economicsColors[index] = globalDataContainer.objectColors["Pathway"];
                                    globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Path"];
                                }
                            }
                            break;
                        case "Path":
                            x = 0;
                            if (!int.TryParse(parts[1], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[2], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            start = new Vector2(x, y);
                            x = 0;
                            if (!int.TryParse(parts[3], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[4], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            end = new Vector2(x, y);
                            if (!globalDataContainer.countries[countryIndex].pathwayTypes.Select(t => t.Name).Contains(parts[4]))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataController.MakePath(start, end, -1);
                            break;
                    }
                }
            }
        }

        using (StreamReader streamReader = new StreamReader(outputPathU))  //Read utility
        {
            while (streamReader.Peek() != -1)
            {
                string line = streamReader.ReadLine();
                List<string> parts = line.Split('\t').ToList();
                int price = 0;
                if (parts[0].Contains("Country") && !globalDataContainer.countries.Select(c => c.Name).Contains(parts[0]))
                {
                    CorruptedDataErrorOpen();
                    return;
                }
                else if (parts[0].Contains("Country"))
                {
                    countryIndex = globalDataContainer.countries.Select(c => c.Name).ToList().FindIndex(c => c == parts[0]);
                }
                else
                {
                    switch (parts[0])
                    {
                        case "Product":
                            id = 0;
                            if (!int.TryParse(parts[1], out id) || id < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].productAmounts.Keys.Select(p => p.Name).Contains(parts[2]) ||
                                !globalDataContainer.countries[countryIndex].productAmounts.Keys.Where(p => p.Name == parts[2]).Select(p => p.ID).ToList().Contains(id))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            price = 0;
                            if (!int.TryParse(parts[3], out price) || price < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[4], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].productAmounts.Keys.Where(p => p.ID == id).First().Price = price;
                            globalDataContainer.countries[countryIndex].productAmounts[globalDataContainer.countries[countryIndex].productAmounts.Where(p => p.Key.ID == id).First().Key] = temp;
                            break;
                        case "PathwayType":
                            if (!globalDataContainer.countries[countryIndex].pathwayTypes.Select(t => t.Name).Contains(parts[1]))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[2], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].pathwayTypes.Where(p => p.Name == parts[1]).First().SetPrice(temp);
                            break;
                    }
                }
            }
        }

        using (StreamReader streamReader = new StreamReader(outputPathW))  //Read warfare
        {
            float unitResource = 0.0F;
            while (streamReader.Peek() != -1)
            {
                string line = streamReader.ReadLine();
                List<string> parts = line.Split('\t').ToList();
                if (parts[0].Contains("Country") && !globalDataContainer.countries.Select(c => c.Name).Contains(parts[0]))
                {
                    CorruptedDataErrorOpen();
                    return;
                }
                else if (parts[0].Contains("Country"))
                {
                    countryIndex = globalDataContainer.countries.Select(c => c.Name).ToList().FindIndex(c => c == parts[0]);
                }
                else
                {
                    switch (parts[0])
                    {
                        case "Hospital":
                            x = 0;
                            if (!int.TryParse(parts[2], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[3], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[4], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].AddWarfareContainer("HP", parts[1], new Vector2(x, y), temp);
                            globalDataContainer.countries[countryIndex].lastWarfareObjID++;
                            index = (int)y * globalDataContainer.size + (int)x;
                            globalDataContainer.warfareColors[index] = globalDataContainer.objectColors["WarfareContainer"];
                            globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                            globalDataContainer.RemakeTextures();
                            globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "WarfareContainer",
                                new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                            globalDataContainer.currentLogisticID++;
                            break;
                        case "Workshop":
                            x = 0;
                            if (!int.TryParse(parts[2], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[3], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[4], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].AddWarfareContainer("W", parts[1], new Vector2(x, y), temp);
                            globalDataContainer.countries[countryIndex].lastWarfareObjID++;
                            index = (int)y * globalDataContainer.size + (int)x;
                            globalDataContainer.warfareColors[index] = globalDataContainer.objectColors["WarfareContainer"];
                            globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                            globalDataContainer.RemakeTextures();
                            globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "WarfareContainer",
                                new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                            globalDataContainer.currentLogisticID++;
                            break;
                        case "Headquaters":
                            x = 0;
                            if (!int.TryParse(parts[2], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[3], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[4], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].AddWarfareContainer("HQ", parts[1], new Vector2(x, y), temp);
                            globalDataContainer.countries[countryIndex].lastWarfareObjID++;
                            index = (int)y * globalDataContainer.size + (int)x;
                            globalDataContainer.warfareColors[index] = globalDataContainer.objectColors["WarfareContainer"];
                            globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                            globalDataContainer.RemakeTextures();
                            globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "WarfareContainer",
                                new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                            globalDataContainer.currentLogisticID++;
                            break;
                        case "TrainingPlace":
                            x = 0;
                            if (!int.TryParse(parts[2], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[3], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[4], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].AddWarfareContainer("TP", parts[1], new Vector2(x, y), temp);
                            globalDataContainer.countries[countryIndex].lastWarfareObjID++;
                            index = (int)y * globalDataContainer.size + (int)x;
                            globalDataContainer.warfareColors[index] = globalDataContainer.objectColors["WarfareContainer"];
                            globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                            globalDataContainer.RemakeTextures();
                            globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "WarfareContainer",
                                new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                            globalDataContainer.currentLogisticID++;
                            break;
                        case "UnitHP":
                            if (!float.TryParse(parts[2], out unitResource) || unitResource < 0.0F)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].objectBank.Bank.Select(p => p.Key.Name).Contains(parts[1]) ||
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "H")
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[3], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (unitResource == 100F)
                                (globalDataContainer.countries[countryIndex].warfareContainerObjects.Last() as Hospital).Fixed.Add(globalDataContainer.countries[countryIndex].
                                    objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            else
                            {
                                (globalDataContainer.countries[countryIndex].warfareContainerObjects.Last() as Hospital).ToFix.Add(globalDataContainer.countries[countryIndex].
                                    objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            }
                            break;
                        case "UnitW":
                            if (!float.TryParse(parts[2], out unitResource) || unitResource < 0.0F)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].objectBank.Bank.Select(p => p.Key.Name).Contains(parts[1]) ||
                                (globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "T" ||
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "V"))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[3], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (unitResource == 100F)
                                (globalDataContainer.countries[countryIndex].warfareContainerObjects.Last() as Workshop).Fixed.Add(globalDataContainer.countries[countryIndex].
                                    objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            else
                            {
                                (globalDataContainer.countries[countryIndex].warfareContainerObjects.Last() as Workshop).ToFix.Add(globalDataContainer.countries[countryIndex].
                                    objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            }
                            break;
                        case "UnitHQ":
                            if (!float.TryParse(parts[2], out unitResource) || unitResource < 0.0F)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].objectBank.Bank.Select(p => p.Key.Name).Contains(parts[1]) ||
                                (globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "T" ||
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "V" ||
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "H"))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[3], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            (globalDataContainer.countries[countryIndex].warfareContainerObjects.Last() as Headquaters).Contained.Add(globalDataContainer.countries[countryIndex].
                                objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            break;
                        case "BankUnit":
                            if (!float.TryParse(parts[2], out unitResource) || unitResource < 0.0F)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].objectBank.Bank.Select(p => p.Key.Name).Contains(parts[1]))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[3], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].objectBank.Bank[globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key] = temp;
                            break;
                        case "BattlePosition":
                            x = 0;
                            if (!int.TryParse(parts[1], out x) || x < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            y = 0;
                            if (!int.TryParse(parts[2], out y) || y < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].battlePositions.Add(new BattlePosition(x, y));
                            globalDataContainer.countries[countryIndex].lastBattlePositionID++;
                            index = (int)y * globalDataContainer.size + (int)x;
                            globalDataContainer.warfareColors[index] = globalDataContainer.objectColors["BattlePosition"];
                            globalDataContainer.logisticsColors[index] = globalDataContainer.objectColors["Node"];
                            globalDataContainer.RemakeTextures();
                            globalDataController.AddNode(new LogisticObject(globalDataContainer.currentLogisticID, "BattlePosition",
                                new Tuple<Vector2, Vector2>(new Vector2(x, y), new Vector2(-1, -1))));
                            globalDataContainer.currentLogisticID++;
                            break;
                        case "UnitBPHT":
                            if (!float.TryParse(parts[2], out unitResource) || unitResource < 0.0F)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].objectBank.Bank.Select(p => p.Key.Name).Contains(parts[1]) ||
                                (globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "T" ||
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "V" ||
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "H"))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[3], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].battlePositions.Last().HumanAndTechResources.Add(globalDataContainer.countries[countryIndex].
                                objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            break;
                        case "UnitBPSP":
                            if (!float.TryParse(parts[2], out unitResource) || unitResource < 0.0F)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].objectBank.Bank.Select(p => p.Key.Name).Contains(parts[1]) ||
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "S")
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[3], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].battlePositions.Last().StationaryPlaces.Add(globalDataContainer.countries[countryIndex].
                                objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            break;
                        case "UnitBPOA":
                            if (!float.TryParse(parts[2], out unitResource) || unitResource < 0.0F)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            if (!globalDataContainer.countries[countryIndex].objectBank.Bank.Select(p => p.Key.Name).Contains(parts[1]) ||
                                (globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "T" &&
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "V" &&
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "H" &&
                                globalDataContainer.countries[countryIndex].objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key.Category != "S"))
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            temp = 0;
                            if (!int.TryParse(parts[3], out temp) || temp < 0)
                            {
                                CorruptedDataErrorOpen();
                                return;
                            }
                            globalDataContainer.countries[countryIndex].battlePositions.Last().TempOutOfActionObjects.Add(globalDataContainer.countries[countryIndex].
                                objectBank.Bank.Where(p => p.Key.Name == parts[1]).First().Key, new Tuple<bool, double, int>(false, unitResource, temp));
                            break;
                    }
                }
            }
        }
        globalDataContainer.isLoadingNext = true;
    }
}
