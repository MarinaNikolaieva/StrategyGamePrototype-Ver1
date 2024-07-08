using GlobalClasses;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UtilityClasses;

public class GeneratorUIBehavior : MonoBehaviour
{
    GeneratorGlobalData generatorGlobalData;

    GameObject PreviousPanel;  //Used for tab switching

    GameObject ErrorPanel;
    string errorLine = "";
    Color mainPictureColor;
    Texture mainPictureTexture;

    public void Begin(GeneratorGlobalData globalData)
    {
        generatorGlobalData = globalData;
        generatorGlobalData.ResetAll();
        mainPictureColor = GameObject.Find("MapRawImage").GetComponent<RawImage>().color;
        mainPictureTexture = GameObject.Find("MapRawImage").GetComponent<RawImage>().texture;

        HashSet<string> categories = new HashSet<string>();
        for (int i = 0; i < generatorGlobalData.resourceReader.getResources().Count; i++)
        {
            categories.Add(generatorGlobalData.resourceReader.getResources().ElementAt(i).Category);
        }
        categories.Remove("G");
        categories.Remove("O");
        Dropdown dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UResCategoryDropdown")).First().transform.GetComponent<Dropdown>(); 
        dropdown.options.Clear();
        for (int i = 0; i < categories.Count; i++)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = categories.ElementAt(i) });
        }
        dropdown.value = 0;
        dropdown.RefreshShownValue();


        Slider countryNum = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("PoliticsCountryNumSlider")).First().transform.GetComponent<Slider>();
        countryNum.maxValue = generatorGlobalData.countryReader.GetCountries().Count;
        countryNum.value = countryNum.maxValue;
    }

    public void Restart()
    {
        generatorGlobalData.ResetAll(); 
        HashSet<string> categories = new HashSet<string>();
        for (int i = 0; i < generatorGlobalData.resourceReader.getResources().Count; i++)
        {
            categories.Add(generatorGlobalData.resourceReader.getResources().ElementAt(i).Category);
        }
        categories.Remove("G");
        categories.Remove("O");
        Dropdown dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UResCategoryDropdown")).First().transform.GetComponent<Dropdown>();
        for (int i = 0; i < categories.Count; i++)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = categories.ElementAt(i) });
        }
        dropdown.value = 0;
        dropdown.RefreshShownValue();


        Slider countryNum = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("PoliticsCountryNumSlider")).First().transform.GetComponent<Slider>();
        countryNum.maxValue = generatorGlobalData.countryReader.GetCountries().Count;
        countryNum.value = countryNum.maxValue;

        GameObject.Find("MapsDropdown").GetComponent<Dropdown>().options.Clear();
        GameObject.Find("MapsDropdown").GetComponent<Dropdown>().value = -1;
        GameObject.Find("MapsDropdown").GetComponent<Dropdown>().RefreshShownValue();
        GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = mainPictureTexture;
        GameObject.Find("MapRawImage").GetComponent<RawImage>().color = mainPictureColor;
    }

    public void SetErrorPanel(GameObject errorPanel)
    {
        ErrorPanel = errorPanel;
    }

    public void TabChanger(string names)
    {
        //Names are "PressedButtonName,CurrentPanelName"
        string[] NamesSeparate = names.Split(',');

        if (NamesSeparate[0].Length == 0) 
        {
            PreviousPanel = GameObject.Find(NamesSeparate[1]);
            return;
        }

        List<GameObject> CandidateObjects = FindObjectsOfType<GameObject>(true).ToList();
        string PanelName = NamesSeparate[0].Substring(0, NamesSeparate[0].Length - 6);  //Remove the word "Button" - make the panel name
        string PrevButtonName = PreviousPanel.name + "Button";
        
        GameObject PressedButton = CandidateObjects.Where(o => o.name.Equals(NamesSeparate[0])).FirstOrDefault();
        GameObject PrevButton = CandidateObjects.Where(o => o.name.Equals(PrevButtonName)).FirstOrDefault();
        GameObject NeededPanel = CandidateObjects.Where(o => o.name.Equals(PanelName)).FirstOrDefault();
        if (PressedButton != null && NeededPanel != null && PrevButton != null)
        {
            NeededPanel.SetActive(true);
            PreviousPanel.SetActive(false);
            PreviousPanel = NeededPanel;

            Color prevColor;
            Color newColor;
            ColorUtility.TryParseHtmlString("#EAAC00", out prevColor);
            ColorUtility.TryParseHtmlString("#FFF300", out newColor);
            PressedButton.GetComponentInChildren<Text>().color = newColor;
            PrevButton.GetComponentInChildren<Text>().color = prevColor;
        }
    }

    //Called when the Slider was moved, and the corresponding input must be changed
    public void SliderPanelValueChanger(string inputNames)
    {
        //The names are "InputPanelName,SliderName"
        string[] NamesSeparate = inputNames.Split(',');
        GameObject inputPanel = GameObject.Find(NamesSeparate[0]);
        GameObject inputSlider = GameObject.Find(NamesSeparate[1]);
        if (inputPanel != null && inputSlider != null)
        {
            inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
        }

        //Sometimes you need to put the value somewhere else
        if (NamesSeparate.Length > 2 && NamesSeparate[2].Length > 0)
        {
            GameObject textPanel = GameObject.Find(NamesSeparate[2]);
            if (textPanel != null)
            {
                string text = textPanel.GetComponent<Text>().text;
                text = text.Substring(0, text.IndexOf(":") + 2);
                text += Mathf.Pow(2, inputSlider.GetComponent<Slider>().value) + 1;
                textPanel.GetComponent<Text>().text = text;
            }
        }
    }

    //Called when the Input was changed, and the corresponding slider must get the same value
    public void SliderValueChanger(string inputNames)
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        //The names are "InputPanelName,SliderName"
        string[] NamesSeparate = inputNames.Split(',');
        GameObject inputPanel = GameObject.Find(NamesSeparate[0]);
        GameObject inputSlider = GameObject.Find(NamesSeparate[1]);
        if (inputPanel != null && inputSlider != null)
        {
            //There must be different parsers for whole/floating-point numbers
            //And that depends on slider's parameters

            if (inputSlider.GetComponent<Slider>().wholeNumbers)
            {
                int newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
                if (int.TryParse(inputPanel.GetComponent<InputField>().text, out newValue) && newValue <= inputSlider.GetComponent<Slider>().maxValue &&
                    newValue >= inputSlider.GetComponent<Slider>().minValue)
                {
                    inputSlider.GetComponent<Slider>().value = newValue;
                    inputPanel.GetComponent<InputField>().text = newValue.ToString();
                }
                else
                {
                    newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
                    inputSlider.GetComponent<Slider>().value = newValue;
                    inputPanel.GetComponent<InputField>().text = newValue.ToString();
                }
            }
            else
            {
                float newValue = float.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
                if (float.TryParse(inputPanel.GetComponent<InputField>().text, out newValue) && newValue <= inputSlider.GetComponent<Slider>().maxValue &&
                    newValue >= inputSlider.GetComponent<Slider>().minValue)
                {
                    inputSlider.GetComponent<Slider>().value = newValue;
                    inputPanel.GetComponent<InputField>().text = newValue.ToString();
                }
                else
                {
                    newValue = float.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
                    inputSlider.GetComponent<Slider>().value = newValue;
                    inputPanel.GetComponent<InputField>().text = newValue.ToString();
                }
            }
        }
    }

    #region Legend Button functionality

    //NOTE Height and Biome are 5-column, others are 3-column
    public void HeightLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Height groups legend";

            //We must also clear the children left from the previous run
            List<Transform> children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().ToList();
            foreach (Transform child in children)
            {
                if (child.name != "Content")
                    GameObject.Destroy(child.gameObject);
            }

            Legend5Filler legendFiller = LegendPanelToOpen.GetComponent<Legend5Filler>();
            List<object> list = new List<object>();
            List<object> addit = new List<object>();
            Dictionary<int, HeightObject> temp = generatorGlobalData.heightReader.getHeightChart();
            for (int i = 0; i < temp.Count; i++)
            {
                list.Add(temp.Values.ElementAt(i));
                addit.Add(temp.Keys.ElementAt(i));
            }

            legendFiller.DataFill(list, addit);
        }
    }

    public void TemperatureLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Temperature groups legend";
            Legend3Filler legendFiller = LegendPanelToOpen.GetComponent<Legend3Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "3ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            List<TemperatureObject> temp = generatorGlobalData.temperatureReader.getTemperatureChart().Values.ToList();
            List<object> list = new List<object>();
            for (int i = 0; i < temp.Count; i++)
                list.Add(temp.ElementAt(i));
            legendFiller.DataFill(list, null);
        }
    }

    public void MoistureLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Moisture groups legend";
            Legend3Filler legendFiller = LegendPanelToOpen.GetComponent<Legend3Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "3ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            List<MoistureObject> temp = generatorGlobalData.moistureReader.getMoistureChart().Values.ToList();
            List<object> list = new List<object>();
            for (int i = 0; i < temp.Count; i++)
                list.Add(temp.ElementAt(i));
            legendFiller.DataFill(list, null);
        }
    }

    public void BiomeLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Biomes legend";
            Legend5Filler legendFiller = LegendPanelToOpen.GetComponent<Legend5Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "5ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                 Object.DestroyImmediate(child.gameObject);
            }

            List<object> list = new List<object>();
            List<Biome> temp = generatorGlobalData.biomeReader.getBiomes().ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                list.Add(temp.ElementAt(i));
            }
            legendFiller.DataFill(list, null);
        }
    }

    public void SoilLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Soils legend";
            Legend3Filler legendFiller = LegendPanelToOpen.GetComponent<Legend3Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "3ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            List<object> list = new List<object>();
            List<object> addit = new List<object>();
            List<Biome> tempBiomes = generatorGlobalData.biomeReader.getBiomes().ToList();
            List<Soil> tempSoils = generatorGlobalData.soilReader.getSoils().ToList();
            for (int i = 0; i < tempSoils.Count; i++)
                list.Add(tempSoils.ElementAt(i));
            for (int i = 0; i < tempBiomes.Count; i++)
                addit.Add(tempBiomes.ElementAt(i));
            legendFiller.DataFill(list, addit);
        }
    }

    public void GlobalResLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Global resources legend";
            Legend3Filler legendFiller = LegendPanelToOpen.GetComponent<Legend3Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "3ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            List<BasicResource> temp = generatorGlobalData.resourceReader.getResources();
            List<object> list = new List<object>();
            for (int i = 0; i < temp.Count; i++)
                list.Add(temp.ElementAt(i));
            List<object> categ = new List<object> { "G" };
            legendFiller.DataFill(list, categ);
        }
    }

    public void OnGroundResLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "On-Ground resources legend";
            Legend3Filler legendFiller = LegendPanelToOpen.GetComponent<Legend3Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "3ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            List<BasicResource> temp = generatorGlobalData.resourceReader.getResources();
            List<object> list = new List<object>();
            for (int i = 0; i < temp.Count; i++)
                list.Add(temp.ElementAt(i));
            List<object> categ = new List<object> { "O" };
            legendFiller.DataFill(list, categ);
        }
    }

    public void UndergroundResLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Underground resources legend";
            Legend3Filler legendFiller = LegendPanelToOpen.GetComponent<Legend3Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "3ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            List<BasicResource> temp = generatorGlobalData.resourceReader.getResources();
            List<object> list = new List<object>();
            for (int i = 0; i < temp.Count; i++)
                list.Add(temp.ElementAt(i));
            List<object> categ = new List<object> { "U" };
            legendFiller.DataFill(list, categ);
        }
    }

    public void CountriesLegendButtonClick(GameObject LegendPanelToOpen)
    {
        if (LegendPanelToOpen != null)
        {
            LegendPanelToOpen.SetActive(true);
            LegendPanelToOpen.transform.Find("HeaderText").gameObject.GetComponent<Text>().text = "Countries legend";
            Legend3Filler legendFiller = LegendPanelToOpen.GetComponent<Legend3Filler>();

            //We must also clear the children left from the previous run
            var children = LegendPanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "3ColumnTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            List<CountryBase> temp = generatorGlobalData.countryReader.GetCountries();
            List<object> list = new List<object>();
            for (int i = 0; i < temp.Count; i++)
                list.Add(temp.ElementAt(i));
            legendFiller.DataFill(list, null);
        }
    }

    #endregion

    public void UndergroundResConfigButtonClick(GameObject PanelToOpen)
    {
        if (PanelToOpen != null)
        {
            PanelToOpen.SetActive(true);
            ConfigFiller legendFiller = PanelToOpen.GetComponent<ConfigFiller>();

            var children = PanelToOpen.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponentsInChildren<Transform>().Where(n => n.name == "ResConfigTableElement(Clone)").ToArray();
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }
            List<BasicResource> temp = generatorGlobalData.resourceReader.getResources();

            List<object> list = new List<object>();
            for (int i = 0; i < temp.Count; i++)
                if (temp.ElementAt(i).Category.Contains("U"))
                    list.Add(temp.ElementAt(i));

            legendFiller.DataFill(list, generatorGlobalData.undergroundResCoefs);
        }
    }

    public void ErrorPanelOpen()
    {
        ErrorPanel.SetActive(true);
        ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text = errorLine;
    }

    public void ClosePanelNoEffect(GameObject PanelToClose)
    {
        if (PanelToClose != null)
        {
            if (PanelToClose.name != "ResConfigPanel")
                PanelToClose.SetActive(false);
            else if (generatorGlobalData.ResConfigSuccess)
                PanelToClose.SetActive(false);
        }
    }

    public void ChangeDropdownValue(Dropdown dropdown)
    {
        if (dropdown.name == "MapsDropdown")
        {
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = generatorGlobalData.MapTextures[dropdown.options[dropdown.value].text];
            dropdown.RefreshShownValue();
        }
        else if (dropdown.name == "UResCategoryDropdown")
        {
            dropdown.RefreshShownValue();
        }
        else if (dropdown.name == "CountrySelectDropdown")
        {
            dropdown.RefreshShownValue();
        }
    }

    #region Generation buttons reactions

    //In here, panels are the panels the button was clicked on

    public void ConfirmSizeButtonClick(GameObject panel)
    {
        Slider bigSlider = panel.transform.Find("MapPowerSlider").GetComponent<Slider>();
        Slider smallSlider = panel.transform.Find("SmallMapPowerSlider").GetComponent<Slider>();

        if (bigSlider.value < smallSlider.value)
        {
            errorLine = "ERROR!\nPower must be greater than Small Power!";
            generatorGlobalData.sizeConfirmed = false;
            ErrorPanelOpen();
        }
        else
        {
            //Reset everything after this - clear the maps etc

            GameObject.Find("MapsDropdown").GetComponent<Dropdown>().options.Clear();
            GameObject.Find("MapsDropdown").GetComponent<Dropdown>().value = -1;
            generatorGlobalData.MapTextures.Clear();
            generatorGlobalData.ResetAll();

            generatorGlobalData.sizeConfirmed = true;
            generatorGlobalData.SetPowersAndSizes((int)bigSlider.value, (int)smallSlider.value);

            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = mainPictureTexture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = mainPictureColor;
        }
    }

    public void GenerateHeightButtonClick(GameObject panel)
    {
        Slider minSlider = panel.transform.Find("MinHeightSlider").GetComponent<Slider>();
        Slider maxSlider = panel.transform.Find("MaxHeightSlider").GetComponent<Slider>();

        if (!generatorGlobalData.sizeConfirmed)
        {
            errorLine = "ERROR!\nConfirm the sizes of the maps you want to make!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "Height Map";

            generatorGlobalData.SetMinMaxHeights((int)minSlider.value, (int)maxSlider.value);
            //Generate here
            System.Random rand = new System.Random();
            float[,] basis = new PerlinNoiseGenerator().GenerateNoiseMap(generatorGlobalData.smallSize, rand.Next(), 5);
            HeightGenerator heightGenerator = new HeightGenerator(generatorGlobalData.size, generatorGlobalData.smallSize, basis);
            Texture2D texture = heightGenerator.RunHeightGeneration(generatorGlobalData.heightReader, generatorGlobalData.minHeight, generatorGlobalData.maxHeight);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;

            heightGenerator.MakeMapParts(generatorGlobalData.ComplexMap);
            generatorGlobalData.HeightMap = heightGenerator.intMap;

            //Reset everything after this - clear the maps etc
            generatorGlobalData.MapTextures.Clear();
            generatorGlobalData.MapTextures.Add(subLine, texture);

            generatorGlobalData.temperatureGenerated = false;
            generatorGlobalData.moistureGenerated = false;
            generatorGlobalData.biomeGenerated = false;
            generatorGlobalData.soilGenerated = false;
            generatorGlobalData.globalResourcesGenerated = false;
            generatorGlobalData.ongroundResourcesGenerated = false;
            generatorGlobalData.undergroundResourcesGenerated = false;
            generatorGlobalData.politicGenerated = false;
            generatorGlobalData.preparationDone = false;

            generatorGlobalData.heightGenerated = true;

            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();
            dropdown.options.Clear();
            dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();
        }
    }

    public void GenerateTemperatureButtonClick(GameObject panel)
    {
        Slider maxTemper = panel.transform.Find("MaxTemperCoefSlider").GetComponent<Slider>();
        float maxTemperSet = generatorGlobalData.heightReader.getHeightChart().First().Value.TemperatureCoeffitient;

        if (!generatorGlobalData.heightGenerated)
        {
            errorLine = "ERROR!\nGenerate height map first!";
            ErrorPanelOpen();
        }
        else if (maxTemper.value < maxTemperSet)
        {
            errorLine = "ERROR!\nMax temperature coefficient must be bigger than the maximum set in Height legend!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "Temperature Map";

            generatorGlobalData.SetTemperParams(maxTemper.value, panel.transform.Find("MappingFactorSlider").GetComponent<Slider>().value);
            //Generate here
            generatorGlobalData.TemperMoistPartialGenerator = new TemperMoistPartialGenerator(generatorGlobalData.size);
            TemperatureGenerator temperMapGenerator = new TemperatureGenerator(generatorGlobalData.size, generatorGlobalData.temperatureReader,
                generatorGlobalData.TemperMoistPartialGenerator, generatorGlobalData.minimumMapVal);
            temperMapGenerator.MakeMapPartsBasic(generatorGlobalData.ComplexMap);
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(generatorGlobalData.heightReader, generatorGlobalData.HeightMap, 
                generatorGlobalData.minHeight, generatorGlobalData.heightRange, generatorGlobalData.maxTemperatureCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(generatorGlobalData.ComplexMap);
            Texture2D texture = temperMapGenerator.RunGeneration();
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            //If we run the generation anew, all maps AFTER this one must be cleared, including moisture
            if (generatorGlobalData.moistureGenerated)
            {
                generatorGlobalData.ClearTexturesRangeWithStart(1);
                dropdown.options.RemoveRange(1, dropdown.options.Count - 1);

                generatorGlobalData.moistureGenerated = false;
                generatorGlobalData.biomeGenerated = false;
                generatorGlobalData.soilGenerated = false;
                generatorGlobalData.globalResourcesGenerated = false;
                generatorGlobalData.ongroundResourcesGenerated = false;
                generatorGlobalData.undergroundResourcesGenerated = false;
                generatorGlobalData.politicGenerated = false;
                generatorGlobalData.preparationDone = false;
            }

            generatorGlobalData.MapTextures.Add(subLine, texture);
            dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();

            generatorGlobalData.heightGenerated = true;
            generatorGlobalData.temperatureGenerated = true;
        }
    }

    public void GenerateMoistureButtonClick(GameObject panel)
    {
        Slider maxMoist = panel.transform.Find("MaxMoistCoefSlider").GetComponent<Slider>();
        float maxMoistSet = generatorGlobalData.heightReader.getHeightChart().First().Value.MoistureCoeffitient;

        if (!generatorGlobalData.temperatureGenerated)
        {
            errorLine = "ERROR!\nGenerate temperature map first!";
            ErrorPanelOpen();
        }
        else if (maxMoist.value < maxMoistSet)
        {
            errorLine = "ERROR!\nMax moisture coefficient must be bigger than the maximum set in Height legend!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "Moisture Map";

            generatorGlobalData.SetMoistParams(maxMoist.value);
            //Generate here
            MoistureGenerator moistureMapGenerator = new MoistureGenerator(generatorGlobalData.size, generatorGlobalData.moistureReader, generatorGlobalData.TemperMoistPartialGenerator,
                generatorGlobalData.minimumMapVal, generatorGlobalData.heightReader, generatorGlobalData.HeightMap, generatorGlobalData.minHeight, generatorGlobalData.heightRange,
                generatorGlobalData.maxMoistureCoef);
            Texture2D texture = moistureMapGenerator.RunGeneration();
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
            moistureMapGenerator.MakeMapParts(generatorGlobalData.ComplexMap);

            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            //If we run the generation anew, all maps AFTER this one must be cleared
            if (generatorGlobalData.biomeGenerated)
            {
                generatorGlobalData.ClearTexturesRangeWithStart(2);
                dropdown.options.RemoveRange(2, dropdown.options.Count - 2);

                generatorGlobalData.biomeGenerated = false;
                generatorGlobalData.soilGenerated = false;
                generatorGlobalData.globalResourcesGenerated = false;
                generatorGlobalData.ongroundResourcesGenerated = false;
                generatorGlobalData.undergroundResourcesGenerated = false;
                generatorGlobalData.politicGenerated = false;
                generatorGlobalData.preparationDone = false;
            }
            generatorGlobalData.MapTextures.Add(subLine, texture);
            dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();

            generatorGlobalData.heightGenerated = true;
            generatorGlobalData.temperatureGenerated = true;
            generatorGlobalData.moistureGenerated = true;
        }
    }

    public void GenerateBiomesButtonClick(GameObject panel)
    {
        Slider iterNum = panel.transform.Find("BiomeIterNumSlider").GetComponent<Slider>();
        Slider neighborNum = panel.transform.Find("BiomeNeighborNumSlider").GetComponent<Slider>();

        if (!generatorGlobalData.moistureGenerated)
        {
            errorLine = "ERROR!\nGenerate moisture map first!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "Biome Map";

            generatorGlobalData.SetBiomeParams((int)iterNum.value, (int)neighborNum.value);
            //Generate here
            BiomeGenerator biomeMapGenerator = new BiomeGenerator(generatorGlobalData.biomeReader, generatorGlobalData.size, generatorGlobalData.ComplexMap);
            biomeMapGenerator.ChangeItersNum(generatorGlobalData.biomeIterNumber);
            biomeMapGenerator.ChangeStateShiftLimit(generatorGlobalData.biomeNeighborNum);
            biomeMapGenerator.RunGeneration();
            Texture2D texture = biomeMapGenerator.GetPicture();
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            //If we run the generation anew, all maps AFTER this one must be cleared
            if (generatorGlobalData.soilGenerated)
            {
                generatorGlobalData.ClearTexturesRangeWithStart(3);
                dropdown.options.RemoveRange(3, dropdown.options.Count - 3);

                generatorGlobalData.soilGenerated = false;
                generatorGlobalData.globalResourcesGenerated = false;
                generatorGlobalData.ongroundResourcesGenerated = false;
                generatorGlobalData.undergroundResourcesGenerated = false;
                generatorGlobalData.politicGenerated = false;
                generatorGlobalData.preparationDone = false;
            }

            generatorGlobalData.MapTextures.Add(subLine, texture);
            dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();

            generatorGlobalData.heightGenerated = true;
            generatorGlobalData.temperatureGenerated = true;
            generatorGlobalData.moistureGenerated = true;
            generatorGlobalData.biomeGenerated = true;
        }
    }

    public void GenerateSoilsButtonClick(GameObject panel)
    {
        Slider iterNum = panel.transform.Find("SoilIterNumSlider").GetComponent<Slider>();
        Slider neighborNum = panel.transform.Find("SoilNeighborNumSlider").GetComponent<Slider>();

        if (!generatorGlobalData.biomeGenerated)
        {
            errorLine = "ERROR!\nGenerate biome map first!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "Soil Map";

            generatorGlobalData.SetSoilParams((int)iterNum.value, (int)neighborNum.value);
            //Generate here
            SoilGenerator soilMapGenerator = new SoilGenerator(generatorGlobalData.size, generatorGlobalData.ComplexMap);
            soilMapGenerator.ChangeItersNum(generatorGlobalData.soilIterNumber);
            soilMapGenerator.ChangeStateShiftLimit(generatorGlobalData.soilNeighborNum);
            soilMapGenerator.RunGeneration();
            Texture2D texture = soilMapGenerator.GetPicture();
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            //If we run the generation anew, ONLY THIS map must be cleared
            if (generatorGlobalData.soilGenerated)
            {
                generatorGlobalData.MapTextures[subLine] = texture;
            }
            else
            {
                generatorGlobalData.MapTextures.Add(subLine, texture);
                dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            }

            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();

            generatorGlobalData.soilGenerated = true;
        }
    }

    public void GenerateGResourcesButtonClick(GameObject panel)
    {
        if (!generatorGlobalData.soilGenerated)
        {
            errorLine = "ERROR!\nGenerate soil map first!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "Global Resource Map";

            //Generate here
            ResourcesGenerator resourceMapsGenerator = new ResourcesGenerator(generatorGlobalData.ComplexMap, generatorGlobalData.size, generatorGlobalData.resourceReader);
            resourceMapsGenerator.GenerateGlobalMap();
            Texture2D texture = resourceMapsGenerator.MakePicture("G");
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            //If we run the generation anew, ONLY THIS map must be cleared
            if (generatorGlobalData.globalResourcesGenerated)
            {
                generatorGlobalData.MapTextures[subLine] = texture;
            }
            else
            {
                generatorGlobalData.MapTextures.Add(subLine, texture);
                dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            }

            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();

            generatorGlobalData.globalResourcesGenerated = true;

        }
    }

    public void GenerateOResourcesButtonClick(GameObject panel)
    {
        Slider iterNum = panel.transform.Find("GOResIterNumSlider").GetComponent<Slider>();
        Slider neighborNum = panel.transform.Find("GOResNeighborNumSlider").GetComponent<Slider>();

        if (!generatorGlobalData.globalResourcesGenerated)
        {
            errorLine = "ERROR!\nGenerate global resources map first!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "OnGround Resource Map";

            generatorGlobalData.SetGOResourcesParams((int)iterNum.value, (int)neighborNum.value);
            //Generate here
            ResourcesGenerator resourceMapsGenerator = new ResourcesGenerator(generatorGlobalData.ComplexMap, generatorGlobalData.size, generatorGlobalData.resourceReader);
            resourceMapsGenerator.ChangeItersNum(generatorGlobalData.GOResourceIterNumber);
            resourceMapsGenerator.ChangeStateShiftLimit(generatorGlobalData.GOResourceNeighborNum);
            resourceMapsGenerator.GenerateOnGroundMap();
            Texture2D texture = resourceMapsGenerator.MakePicture("O");
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            //If we run the generation anew, ONLY THIS map must be cleared
            if (generatorGlobalData.ongroundResourcesGenerated)
            {
                generatorGlobalData.MapTextures[subLine] = texture;
            }
            else
            {
                generatorGlobalData.MapTextures.Add(subLine, texture);
                dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            }

            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();

            generatorGlobalData.ongroundResourcesGenerated = true;
        }
    }

    public void GenerateCurCategUResourcesButtonClick(GameObject panel)
    {
        Slider neighborNum = panel.transform.Find("UResNeighborNumSlider").GetComponent<Slider>();
        Dropdown categoryDropdown = panel.transform.Find("UResCategoryDropdown").GetComponent<Dropdown>();
        string category = categoryDropdown.options[panel.GetComponentInChildren<Dropdown>().value].text;

        if (!generatorGlobalData.ongroundResourcesGenerated)
        {
            errorLine = "ERROR!\nGenerate on-ground resources map first!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = category + " Resource Map";

            generatorGlobalData.SetUResourcesParam((int)neighborNum.value);
            //Generate here
            ResourcesGenerator resourceMapsGenerator = new ResourcesGenerator(generatorGlobalData.ComplexMap, generatorGlobalData.size, generatorGlobalData.resourceReader);
            List<BasicResource> resources = generatorGlobalData.resourceReader.getResources().Where(r => !r.Category.Equals("G") && !r.Category.Equals("O")).ToList();
            List<float> coefs = new List<float>();
            for (int i = 0; i < resources.Count; i++)
            {
                if (generatorGlobalData.undergroundResCoefs.ElementAt(i).Item1)
                    coefs.Add(generatorGlobalData.undergroundResCoefs.ElementAt(i).Item2);
                else
                    coefs.Add(1.1F);
            }
            resourceMapsGenerator.GenerateConcreteUnderGroundMap(category, coefs);
            Texture2D texture = resourceMapsGenerator.MakePicture(category); 
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
            GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            //If we run the generation anew, ONLY THIS map must be cleared
            List<string> tempList = categoryDropdown.options.Select(option => option.text).ToList();
            for (int i = 0; i < tempList.Count; i++)
                tempList[i] = tempList[i] + " Resource Map";

            if (generatorGlobalData.MapTextures.Keys.ToList().IndexOf(subLine) != -1)
            {
                generatorGlobalData.MapTextures[subLine] = texture;
            }
            else
            {
                generatorGlobalData.MapTextures.Add(subLine, texture);
                dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
            }
            dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
            dropdown.RefreshShownValue();

            int undergroundResMapsGenerated = 0;
            foreach (string item in dropdown.options.Select(option => option.text).ToList())
            {
                if (item.Contains("U"))
                    undergroundResMapsGenerated++;
            }
            List<string> categories = generatorGlobalData.resourceReader.getResources().GroupBy(r => r.Category).Select(r => r.FirstOrDefault()).Select(r => r.Category).ToList();
            categories.Remove("G");
            categories.Remove("O");
            if (undergroundResMapsGenerated == categories.Count)
                generatorGlobalData.undergroundResourcesGenerated = true;
        }
    }

    public void GenerateAllUResourcesButtonClick(GameObject panel)
    {
        Slider neighborNum = panel.transform.Find("UResNeighborNumSlider").GetComponent<Slider>();
        Dropdown categoryDropdown = panel.transform.Find("UResCategoryDropdown").GetComponent<Dropdown>();

        if (!generatorGlobalData.ongroundResourcesGenerated)
        {
            errorLine = "ERROR!\nGenerate on-ground resources map first!";
            ErrorPanelOpen();
        }
        else
        {
            generatorGlobalData.SetUResourcesParam((int)neighborNum.value);
            //Generate here
            ResourcesGenerator resourceMapsGenerator = new ResourcesGenerator(generatorGlobalData.ComplexMap, generatorGlobalData.size, generatorGlobalData.resourceReader);
            resourceMapsGenerator.ChangeStateShiftLimit(generatorGlobalData.UResourceNeighborNum);
            List<string> categories = generatorGlobalData.resourceReader.getResources().GroupBy(r => r.Category).Select(r => r.FirstOrDefault()).Select(r => r.Category).ToList();
            categories.Remove("G");
            categories.Remove("O");

            resourceMapsGenerator.GenerateUnderGroundMap(generatorGlobalData.undergroundResCoefs.Select(r => r.Item2).ToList());
            Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

            for (int i = 0; i < categories.Count; i++)
            {
                Texture2D texture = resourceMapsGenerator.MakePicture(categories.ElementAt(i));
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.Apply();
                string subLine = categories.ElementAt(i) + " Resource Map";

                //If we run the generation anew, ONLY THIS map must be cleared
                if (generatorGlobalData.MapTextures.Keys.ToList().IndexOf(subLine) != -1)
                {
                    generatorGlobalData.MapTextures[subLine] = texture;
                }
                else
                {
                    generatorGlobalData.MapTextures.Add(subLine, texture);
                    dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
                }

                if (i == categories.Count - 1)
                {
                    GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
                    GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
                    dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
                    dropdown.RefreshShownValue();
                }
            }

            generatorGlobalData.undergroundResourcesGenerated = true;
        }
    }

    public void GeneratePoliticsButtonClick(GameObject panel)
    {
        Slider countryNum = panel.transform.Find("PoliticsCountryNumSlider").GetComponent<Slider>();
        Slider minCapHeight = panel.transform.Find("PoliticsMinCapHeightSlider").GetComponent<Slider>();
        Slider maxCapHeight = panel.transform.Find("PoliticsMaxCapHeightSlider").GetComponent<Slider>();
        Slider minDist = panel.transform.Find("PoliticsMinDistBetwCapSlider").GetComponent<Slider>();
        Slider minExpHeight = panel.transform.Find("PoliticsMinTerHeightSlider").GetComponent<Slider>();

        if (!generatorGlobalData.undergroundResourcesGenerated)
        {
            errorLine = "ERROR!\nGenerate underground resources maps first!";
            ErrorPanelOpen();
        }
        else if (maxCapHeight.value < minCapHeight.value)
        {
            errorLine = "ERROR!\nMaximal height for capital placement must be greater than minimal!";
            ErrorPanelOpen();
        }
        else
        {
            string subLine = "Political Map";

            for (int i = 0; i < generatorGlobalData.size; i++)  //Y coord
            {
                for (int j = 0; j < generatorGlobalData.size; j++)  //X coord
                {
                    if (i != 0)
                    {
                        generatorGlobalData.ComplexMap[i, j].neighbors.Add(generatorGlobalData.ComplexMap[i - 1, j]);
                        generatorGlobalData.ComplexMap[i, j].neighborIDs.Add(generatorGlobalData.ComplexMap[i - 1, j].ID);
                    }
                    if (j != 0)
                    {
                        generatorGlobalData.ComplexMap[i, j].neighbors.Add(generatorGlobalData.ComplexMap[i, j - 1]);
                        generatorGlobalData.ComplexMap[i, j].neighborIDs.Add(generatorGlobalData.ComplexMap[i, j - 1].ID);
                    }
                    if (i != generatorGlobalData.size - 1)
                    {
                        generatorGlobalData.ComplexMap[i, j].neighbors.Add(generatorGlobalData.ComplexMap[i + 1, j]);
                        generatorGlobalData.ComplexMap[i, j].neighborIDs.Add(generatorGlobalData.ComplexMap[i + 1, j].ID);
                    }
                    if (j != generatorGlobalData.size - 1)
                    {
                        generatorGlobalData.ComplexMap[i, j].neighbors.Add(generatorGlobalData.ComplexMap[i, j + 1]);
                        generatorGlobalData.ComplexMap[i, j].neighborIDs.Add(generatorGlobalData.ComplexMap[i, j + 1].ID);
                    }
                }
            }

            generatorGlobalData.SetPoliticsParams((int)countryNum.value, (int)minDist.value, (int)minCapHeight.value, (int)maxCapHeight.value, (int)minExpHeight.value);
            generatorGlobalData.ResetCountriesCapitals();

            //Generate here
            CountriesGenerator politicalMapGenerator = new CountriesGenerator(generatorGlobalData.countryReader, generatorGlobalData.ComplexMap, generatorGlobalData.size);
            politicalMapGenerator.ChangeMinDistance(generatorGlobalData.minDistanceBetweenCapitals);
            politicalMapGenerator.ChangeMinCapitalHeight(generatorGlobalData.minHeightForCapital);
            politicalMapGenerator.ChangeMaxCapitalHeight(generatorGlobalData.maxHeightForCapital);
            politicalMapGenerator.ChangeMinExpantionHeight(generatorGlobalData.minHeightForExpantion);
            politicalMapGenerator.ChangeCountryNumber(generatorGlobalData.numOfCountries);
            int success = politicalMapGenerator.RunGeneration();
            if (success == -1)
            {
                errorLine = "ERROR!\nCan not put the selected number of countries on the map!\nPlease make the map or the number of countries smaller!";
                ErrorPanelOpen();
            }
            else
            {
                Texture2D texture = politicalMapGenerator.MakePicture();
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.Apply();
                GameObject.Find("MapRawImage").GetComponent<RawImage>().texture = texture;
                GameObject.Find("MapRawImage").GetComponent<RawImage>().color = Color.white;
                Dropdown dropdown = GameObject.Find("MapsDropdown").GetComponent<Dropdown>();

                //If we run the generation anew, ONLY THIS map must be cleared
                if (generatorGlobalData.politicGenerated)
                {
                    generatorGlobalData.MapTextures[subLine] = texture;
                }
                else
                {
                    generatorGlobalData.MapTextures.Add(subLine, texture);
                    dropdown.options.Add(new Dropdown.OptionData() { text = subLine });
                }

                dropdown.value = dropdown.options.Select(option => option.text).ToList().IndexOf(subLine);
                dropdown.RefreshShownValue();

                generatorGlobalData.politicGenNum++;
                generatorGlobalData.politicGenerated = true;
            }
        }
    }

    #endregion
    

}
