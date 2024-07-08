using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityClasses;
using UnityEngine;
using UnityEngine.UI;
using EconomicsClasses;
using LogisticsClasses;
using WarfareClasses;
using WarfareMechanics;

public class GameplayUIBehaviour : MonoBehaviour
{
    public GlobalDataContainer GlobalDataContainer;
    public GlobalDataController GlobalDataController;
    GameObject PreviousPanel;
    int framesCounter;
    int useless = 0;

    public void Start()
    {
        framesCounter = 0;
    }

    public void Update()
    {
        framesCounter++;
        if (framesCounter == 119)
        {
            ResetResourcesInfo();
            framesCounter = 0;
        }
    }

    public void SetDataContainer(GlobalDataContainer GlobalDataContainer)
    {
        this.GlobalDataContainer = GlobalDataContainer;
    }

    public void InitDataContainer()
    {
        GlobalDataContainer.Init();
    }

    public void InitDataController()
    {
        GlobalDataController.InitDataContainer(GlobalDataContainer);
    }

    public void PrepareImages()
    {
        Dropdown dropdown = FindObjectsOfType<GameObject>(false).ToList().Where(n => n.name.Equals("MainMapSelectDropdown")).First().transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        int politicalIndex = 0;
        for (int i = 0; i < GlobalDataContainer.MapTextures.Count; i++)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = GlobalDataContainer.MapTextures.ElementAt(i).Key });
            if (GlobalDataContainer.MapTextures.ElementAt(i).Key.Contains("Political"))
                politicalIndex = i;
        }
        dropdown.value = politicalIndex;
        dropdown.RefreshShownValue();
        GameObject.Find("MainMapRawImage").GetComponent<RawImage>().texture = GlobalDataContainer.MapTextures[dropdown.options[dropdown.value].text];
        GameObject.Find("MainMapRawImage").GetComponent<RawImage>().color = Color.white;

        dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("LogisticsRouteSelectDropdown")).First().transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = "None" });
        dropdown.value = 0;
        dropdown.RefreshShownValue();
        FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("LogisticsMapShowRouteRawImage")).First().gameObject.GetComponent<RawImage>().texture =
            GlobalDataContainer.pathsTexture;
    }

    public void SetTexturesForPanels()
    {
        FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("DiplomacyMapRawImage")).First().gameObject.GetComponent<RawImage>().texture = GlobalDataContainer.MapTextures["Political Map"];
        FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("EconomyMapRawImage")).First().gameObject.GetComponent<RawImage>().texture = GlobalDataContainer.MapTextures["Economics Map"];
        FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("WarfareMapRawImage")).First().gameObject.GetComponent<RawImage>().texture = GlobalDataContainer.MapTextures["Warfare Map"];
        FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("LogisticsMapRawImage")).First().gameObject.GetComponent<RawImage>().texture = GlobalDataContainer.MapTextures["Logistics Map"];
        //FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("SocialMapRawImage")).First().gameObject.GetComponent<RawImage>().texture = GlobalDataContainer.MapTextures["Social Map"];
    }

    public void ResetResourcesInfo()
    {
        List<GameObject> CandidateObjects = FindObjectsOfType<GameObject>(true).ToList();
        GameObject panel = CandidateObjects.Where(o => o.name.Equals("ResourcesGamePanel")).FirstOrDefault();
        GameObject warfareResPanel = CandidateObjects.Where(o => o.name.Equals("WarfareResourcesGamePanel")).FirstOrDefault();

        GameObject exampleRes = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("MapResAmountTextExample")).FirstOrDefault();
        GameObject scrollViewContentRes = panel.transform.Find("ResourcesAmountScrollView").Find("Viewport").Find("Content").gameObject;
        GameObject exampleProd = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("MapProdAmountTextExample")).FirstOrDefault();
        GameObject scrollViewContentProd = panel.transform.Find("ResourcesProductAmountScrollView").Find("Viewport").Find("Content").gameObject;
        GameObject exampleIndNew = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("IndustryInfoPanel")).FirstOrDefault();
        GameObject scrollViewContentInd = panel.transform.Find("ResourcesIndustriesScrollView").Find("Viewport").Find("Content").gameObject;
        GameObject exampleWar = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WarMapResAmountTextExample")).FirstOrDefault();
        GameObject scrollViewContentWar = warfareResPanel.transform.Find("WarfareResourcesAmountScrollView").Find("Viewport").Find("Content").gameObject;

        List<string> resAmountInfo = GlobalDataController.GetAllResourcesAmounts();
        List<string> prodAmountInfo = GlobalDataController.GetAllProductsAvailable();
        List<List<string>> industries = GlobalDataController.GetAllIndustriesDetailed();
        List<string> warfareRes = GlobalDataController.GetUnitsInBank(new Vector2(-1, -1));

        var children = scrollViewContentRes.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }
        children = scrollViewContentProd.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }
        int counter = 0;
        children = scrollViewContentInd.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("IndustryInfoPanel"))
            {
                Text text = child.gameObject.transform.Find("InfoText").gameObject.GetComponent<Text>();
                text.text = industries[counter][0] + industries[counter][1];
                counter++;
            }
        }
        children = scrollViewContentWar.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }

        foreach (string info in resAmountInfo)
        {
            GameObject textField = Instantiate(exampleRes);
            textField.transform.SetParent(scrollViewContentRes.transform, false);
            textField.SetActive(true);
            textField.GetComponent<Text>().text = info;
        }
        foreach (string info in prodAmountInfo)
        {
            GameObject textField = Instantiate(exampleProd);
            textField.transform.SetParent(scrollViewContentProd.transform, false);
            textField.SetActive(true);
            textField.GetComponent<Text>().text = info;
        }
        foreach (string info in warfareRes)
        {
            GameObject textField = Instantiate(exampleWar);
            textField.transform.SetParent(scrollViewContentWar.transform, false);
            textField.SetActive(true);
            textField.GetComponent<Text>().text = info;
        }
    }

    public void SetResourcesInfo()
    {
        List<GameObject> CandidateObjects = FindObjectsOfType<GameObject>(true).ToList();
        GameObject panel = CandidateObjects.Where(o => o.name.Equals("ResourcesGamePanel")).FirstOrDefault();
        GameObject warfareResPanel = CandidateObjects.Where(o => o.name.Equals("WarfareResourcesGamePanel")).FirstOrDefault();

        GameObject exampleRes = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("MapResAmountTextExample")).FirstOrDefault();
        GameObject scrollViewContentRes = panel.transform.Find("ResourcesAmountScrollView").Find("Viewport").Find("Content").gameObject;
        GameObject exampleProd = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("MapProdAmountTextExample")).FirstOrDefault();
        GameObject scrollViewContentProd = panel.transform.Find("ResourcesProductAmountScrollView").Find("Viewport").Find("Content").gameObject;
        //GameObject exampleInd = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("MapIndustryTextExample")).FirstOrDefault();
        GameObject exampleIndNew = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("IndustryInfoPanel")).FirstOrDefault();
        GameObject scrollViewContentInd = panel.transform.Find("ResourcesIndustriesScrollView").Find("Viewport").Find("Content").gameObject;

        GameObject exampleWar = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WarMapResAmountTextExample")).FirstOrDefault();
        GameObject scrollViewContentWar = warfareResPanel.transform.Find("WarfareResourcesAmountScrollView").Find("Viewport").Find("Content").gameObject;

        List<string> resAmountInfo = GlobalDataController.GetAllResourcesAmounts();
        List<string> prodAmountInfo = GlobalDataController.GetAllProductsAvailable();
        //List<string> industriesInfo = GlobalDataController.GetAllIndustriesDevelopment();
        List<List<string>> industries = GlobalDataController.GetAllIndustriesDetailed();
        List<string> warfareRes = GlobalDataController.GetUnitsInBank(new Vector2(-1, -1));

        var children = scrollViewContentRes.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }
        children = scrollViewContentProd.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }
        children = scrollViewContentInd.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }
        children = scrollViewContentWar.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }

        foreach (string info in resAmountInfo)
        {
            GameObject textField = Instantiate(exampleRes);
            textField.transform.SetParent(scrollViewContentRes.transform, false);
            textField.SetActive(true);
            textField.GetComponent<Text>().text = info;
        }
        foreach (string info in prodAmountInfo)
        {
            GameObject textField = Instantiate(exampleProd);
            textField.transform.SetParent(scrollViewContentProd.transform, false);
            textField.SetActive(true);
            textField.GetComponent<Text>().text = info;
        }
        foreach (List<string> info in industries)
        {
            GameObject inform = Instantiate(exampleIndNew);
            inform.transform.SetParent(scrollViewContentInd.transform, false);
            inform.SetActive(true);
            Text text = inform.transform.Find("InfoText").gameObject.GetComponent<Text>();
            text.text = info[0] + info[1];
            Dropdown dropdown = inform.transform.Find("ResourcesDropdown").transform.GetComponent<Dropdown>();
            dropdown.options.Clear();
            for (int i = 2; i < info.Count; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = info[i] });
            }
            dropdown.value = 0;
            dropdown.RefreshShownValue();
        }
        foreach (string info in warfareRes)
        {
            GameObject textField = Instantiate(exampleWar);
            textField.transform.SetParent(scrollViewContentWar.transform, false);
            textField.SetActive(true);
            textField.GetComponent<Text>().text = info;
        }
    }


    public void GameplayStart(GlobalDataContainer container)
    {
        if (container.isLoadingNext && container.isPreparingNext)
        {
            TabChanger(",MainGamePanel");
            SetDataContainer(container);
            if (container.reInitContainer)
            {
                InitDataContainer();
                InitDataController();
            }
            PrepareImages();
            SetTexturesForPanels();
            SetResourcesInfo();
        }
        else
            return;
    }


    public void ChangeMapDropdownValue(Dropdown dropdown)
    {
        if (dropdown.name == "MainMapSelectDropdown")
        {
            GameObject.Find("MainMapRawImage").GetComponent<RawImage>().texture = GlobalDataContainer.MapTextures[dropdown.options[dropdown.value].text];
            dropdown.RefreshShownValue();
        }
    }

    public void DropdownValueChanged(Dropdown dropdown)
    {
        dropdown.RefreshShownValue();
    }

    public void RouteDisplayDropdownValueChanged(Dropdown dropdown)
    {
        dropdown.RefreshShownValue();

        string[] parts = dropdown.options[dropdown.value].text.Split(' ');
        GlobalDataContainer.ClearPathTexture();

        if (parts[0] != "None")
        {
            Vector2 start = new Vector2(int.Parse(parts[2]), int.Parse(parts[4]));
            Vector2 end = new Vector2(int.Parse(parts[8]), int.Parse(parts[10]));

            GlobalDataController.PaintRoute(start, end);
            GlobalDataContainer.RemakePathTexture();
        }

        FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("LogisticsMapShowRouteRawImage")).First().gameObject.GetComponent<RawImage>().texture = 
            GlobalDataContainer.pathsTexture;
        FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("LogisticsMapShowRouteRawImage")).First().SetActive(true);
    }

    public void RouteDropdownRefill(Dropdown dropdown)
    {
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = "None" });
        List<string> routes = GlobalDataController.GetRoutes(GlobalDataContainer.selectedCoordinates);
        for (int i = 0; i < routes.Count; i++)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = routes.ElementAt(i) });
        }
        dropdown.value = 0;
        RouteDisplayDropdownValueChanged(dropdown);
    }

    private void ActionPanelClear(Image panel)
    {
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();

        string search = "Game";
        int mIndex = panel.name.IndexOf(search);
        string panelName = panel.name.Substring(0, mIndex);
        string scrollViewName = panel.name.Substring(0, mIndex) + "MapActionScrollView";

        GameObject scrollViewContent = panel.transform.Find(scrollViewName).Find("Viewport").Find("Content").gameObject;

        var children = scrollViewContent.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    private void ActionPanelDisplay(Image panel, object obj)
    {
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();

        //If obj is null, the ActionPanel must put Create options in place
        //Otherwise, it must put the corresponding Edit options in place
        string search = "Game";
        int mIndex = panel.name.IndexOf(search);
        string panelName = panel.name.Substring(0, mIndex);
        string scrollViewName = panel.name.Substring(0, mIndex) + "MapActionScrollView";

        GameObject scrollViewContent = panel.transform.Find(scrollViewName).Find("Viewport").Find("Content").gameObject;

        var children = scrollViewContent.GetComponentsInChildren<Transform>().ToArray();
        foreach (var child in children)
        {
            if (child.name.Contains("Clone") || child.name != "Content")
                UnityEngine.Object.Destroy(child.gameObject);
        }

        switch (panelName)
        {
            case "Main":
                if (obj == null)
                {
                    GameObject actionPanel1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateStorage")).FirstOrDefault());
                    actionPanel1.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateCity")).FirstOrDefault());
                    actionPanel2.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel3 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateFacility")).FirstOrDefault());
                    actionPanel3.transform.SetParent(scrollViewContent.transform, false);

                    GameObject actionPanel4 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateHospital")).FirstOrDefault());
                    actionPanel4.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel5 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateWorkshop")).FirstOrDefault());
                    actionPanel5.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel6 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateHeadquaters")).FirstOrDefault());
                    actionPanel6.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel7 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateTrainingField")).FirstOrDefault());
                    actionPanel7.transform.SetParent(scrollViewContent.transform, false);

                    GameObject actionPanel8 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateImpact")).FirstOrDefault());
                    actionPanel8.transform.SetParent(scrollViewContent.transform, false);
                    //NEEDED add Warfare Create panels here when I make them  DONE
                }
                else
                {
                    switch (obj.GetType().Name)
                    {
                        case "City":
                            GameObject actionPanelC1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditCity")).FirstOrDefault());
                            actionPanelC1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelC2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveCity")).FirstOrDefault());
                            actionPanelC2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Facility":
                            GameObject actionPanelF1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditFacility")).FirstOrDefault());
                            actionPanelF1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelF2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveFacility")).FirstOrDefault());
                            actionPanelF2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Storage":
                            GameObject actionPanelS1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditStorage")).FirstOrDefault());
                            actionPanelS1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelS2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveStorage")).FirstOrDefault());
                            actionPanelS2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Hospital":
                            GameObject actionPanelH1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditHospital")).FirstOrDefault());
                            actionPanelH1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelH2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveHospital")).FirstOrDefault());
                            actionPanelH2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Workshop":
                            GameObject actionPanelW1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditWorkshop")).FirstOrDefault());
                            actionPanelW1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelW2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveWorkshop")).FirstOrDefault());
                            actionPanelW2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Headquaters":
                            GameObject actionPanelHQ1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditHeadquaters")).FirstOrDefault());
                            actionPanelHQ1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelHQ2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveHeadquaters")).FirstOrDefault());
                            actionPanelHQ2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "TrainingPlace":
                            GameObject actionPanelTP1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateArmyman")).FirstOrDefault());
                            actionPanelTP1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelTP2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveTrainingField")).FirstOrDefault());
                            actionPanelTP2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                    }

                    GameObject actionPanel4 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateImpact")).FirstOrDefault());
                    actionPanel4.transform.SetParent(scrollViewContent.transform, false);
                }
                break;
            case "Diplomacy":
                if (obj == null)
                {
                    GameObject actionPanel1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateAlliance")).FirstOrDefault());
                    actionPanel1.transform.SetParent(scrollViewContent.transform, false);
                }
                else
                {
                    //NEEDED change this to a separate panel to LOOK THROUGH all alliances present
                    GameObject actionPanel1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveAlliance")).FirstOrDefault());
                    actionPanel1.transform.SetParent(scrollViewContent.transform, false);
                }
                break;
            case "Economy":
                if (obj == null)
                {
                    GameObject actionPanel1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateStorage")).FirstOrDefault());
                    actionPanel1.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateCity")).FirstOrDefault());
                    actionPanel2.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel3 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateFacility")).FirstOrDefault());
                    actionPanel3.transform.SetParent(scrollViewContent.transform, false);

                    GameObject actionPanel4 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateImpact")).FirstOrDefault());
                    actionPanel4.transform.SetParent(scrollViewContent.transform, false);
                }
                else
                {
                    switch (obj.GetType().Name)
                    {
                        case "City":
                            GameObject actionPanelC1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditCity")).FirstOrDefault());
                            actionPanelC1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelC2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveCity")).FirstOrDefault());
                            actionPanelC2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Facility":
                            GameObject actionPanelF1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditFacility")).FirstOrDefault());
                            actionPanelF1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelF2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveFacility")).FirstOrDefault());
                            actionPanelF2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Storage":
                            GameObject actionPanelS1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditStorage")).FirstOrDefault());
                            actionPanelS1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelS2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveStorage")).FirstOrDefault());
                            actionPanelS2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                    }

                    GameObject actionPanel4 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateImpact")).FirstOrDefault());
                    actionPanel4.transform.SetParent(scrollViewContent.transform, false);
                }
                break;
            case "Warfare":
                if (obj == null)
                {
                    GameObject actionPanel1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateHospital")).FirstOrDefault());
                    actionPanel1.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateWorkshop")).FirstOrDefault());
                    actionPanel2.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel3 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateHeadquaters")).FirstOrDefault());
                    actionPanel3.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel4 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateTrainingField")).FirstOrDefault());
                    actionPanel4.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanel5 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateBattlePosition")).FirstOrDefault());
                    actionPanel5.transform.SetParent(scrollViewContent.transform, false);
                }
                else
                {
                    switch (obj.GetType().Name)
                    {
                        case "Hospital":
                            GameObject actionPanelH1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditHospital")).FirstOrDefault());
                            actionPanelH1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelH2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveHospital")).FirstOrDefault());
                            actionPanelH2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Workshop":
                            GameObject actionPanelW1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditWorkshop")).FirstOrDefault());
                            actionPanelW1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelW2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveWorkshop")).FirstOrDefault());
                            actionPanelW2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "Headquaters":
                            GameObject actionPanelHQ1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditHeadquaters")).FirstOrDefault());
                            actionPanelHQ1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelHQ2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveHeadquaters")).FirstOrDefault());
                            actionPanelHQ2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "TrainingPlace":
                            GameObject actionPanelTP1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreateArmyman")).FirstOrDefault());
                            actionPanelTP1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelTP2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveTrainingField")).FirstOrDefault());
                            actionPanelTP2.transform.SetParent(scrollViewContent.transform, false);
                            break;
                        case "BattlePosition":
                            GameObject actionPanelBP1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditBattlePosition")).FirstOrDefault());
                            actionPanelBP1.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelBP2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemoveBattlePosition")).FirstOrDefault());
                            actionPanelBP2.transform.SetParent(scrollViewContent.transform, false);
                            GameObject actionPanelBP3 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("APutPressure")).FirstOrDefault());
                            actionPanelBP3.transform.SetParent(scrollViewContent.transform, false);
                            break;
                    }
                }
                break;
            case "Logistics":
                if (obj == null)
                {
                    GameObject actionPanelP1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreatePathway")).FirstOrDefault());
                    actionPanelP1.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanelP2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreatePath")).FirstOrDefault());
                    actionPanelP2.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanelP3 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemovePath")).FirstOrDefault());
                    actionPanelP3.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanelP4 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditPathwayType")).FirstOrDefault());
                    actionPanelP4.transform.SetParent(scrollViewContent.transform, false);
                }
                else
                {
                    GameObject actionPanelP1 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditPathway")).FirstOrDefault());
                    actionPanelP1.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanelP2 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemovePathway")).FirstOrDefault());
                    actionPanelP2.transform.SetParent(scrollViewContent.transform, false);

                    GameObject actionPanelP3 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ACreatePath")).FirstOrDefault());
                    actionPanelP3.transform.SetParent(scrollViewContent.transform, false);
                    GameObject actionPanelP4 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ARemovePath")).FirstOrDefault());
                    actionPanelP4.transform.SetParent(scrollViewContent.transform, false);

                    GameObject actionPanelP5 = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("AEditPathwayType")).FirstOrDefault());
                    actionPanelP5.transform.SetParent(scrollViewContent.transform, false);
                }
                break;
        }
    }

    private void InfoPanelDisplay(Image panel, Vector2 clickCoords)
    {
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();
        string search = "Game";
        int mIndex = panel.name.IndexOf(search);
        string scrollViewName = panel.name.Substring(0, mIndex) + "MapInfoScrollView";

        GameObject example = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("MapInfoTextExample")).FirstOrDefault();
        GameObject scrollViewContent = panel.transform.Find(scrollViewName).Find("Viewport").Find("Content").gameObject;

        var children = scrollViewContent.GetComponentsInChildren<Transform>().Where(n => n.name == "MapInfoTextExample(Clone)").ToArray();
        foreach (var child in children)
        {
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }

        int counter = 0;
        bool cont = true;
        while (cont)
        {
            GameObject textField = Instantiate(example);
            textField.transform.SetParent(scrollViewContent.transform, false);
            textField.SetActive(true);

            switch (counter) {
                case 0:  //Height
                    textField.GetComponent<Text>().text = "Height: " + GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Height;
                    counter++;
                    break;
                case 1:  //Temperature
                    textField.GetComponent<Text>().text = "Temperature zone: " + GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].TemperatureFinal.Name;
                    counter++;
                    break;
                case 2:  //Moisture
                    textField.GetComponent<Text>().text = "Moisture zone: " + GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Moisture.Name;
                    counter++;
                    break;
                case 3:  //Biome
                    textField.GetComponent<Text>().text = "Biome: " + GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Biome.getName();
                    counter++;
                    break;
                case 4:  //Soil
                    textField.GetComponent<Text>().text = "Soil type: " + GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Soil.getName();
                    counter++;
                    break;
                case 5:  //Country
                    textField.GetComponent<Text>().text = "Country: ";
                    if (GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Country == null)
                        textField.GetComponent<Text>().text += "None";
                    else
                        textField.GetComponent<Text>().text += GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Country.Name;

                    if (GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Country == null ||
                        GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Country.Name != GlobalDataContainer.countryUnderControl)
                    {
                        ActionPanelClear(panel);
                        cont = false;
                    }
                    counter++;
                    break;
                case 6:  //ACTIVATE ONLY IF YOUR COUNTRY; Economic object, if present - and Resources, at all times
                                       
                    textField.GetComponent<Text>().text = "";

                    StringBuilder build = new StringBuilder();
                    build.Append("\nResources present:\n");
                    if (GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Resources.Count == 0)
                        build.Append("None");
                    else
                    {
                        List<IResource> resources = GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Resources;
                        for (int i = 0; i < resources.Count; i++)
                        {
                            build.Append(resources[i].Name);
                            if (i != resources.Count - 1)
                                build.Append("\n");
                        }
                    }
                    build.Append("\n");
                    textField.GetComponent<Text>().text += build.ToString();

                    if (GlobalDataContainer.MapTextures.ContainsKey("Economics Map"))
                    {
                        object obj = GlobalDataContainer.ComplexMap[(int)clickCoords.x, (int)clickCoords.y].Country.IsObjectInPlace(clickCoords);
                        if (obj != null)
                        {
                            //The only object options are Storage, Pathway, City, Facility
                            switch (obj.GetType().Name.ToString())
                            {
                                case "Storage":
                                    textField.GetComponent<Text>().text += "\nStorage \n";
                                    for (int i = 0; i < (obj as Storage).GetInfoDetailed().Count; i++)
                                    {
                                        textField.GetComponent<Text>().text += (obj as Storage).GetInfoDetailed().ElementAt(i) + "\n";
                                    }
                                    break;
                                case "Pathway":
                                    textField.GetComponent<Text>().text += "\nPathway \n";
                                    textField.GetComponent<Text>().text += (obj as Pathway).GetInfo();
                                    break;
                                case "City":
                                    textField.GetComponent<Text>().text += "\nCity \n";
                                    textField.GetComponent<Text>().text += (obj as City).GetInfo();
                                    break;
                                case "Facility":
                                    textField.GetComponent<Text>().text += "\nFacility \n";
                                    for (int i = 0; i < (obj as Facility).GetInfoDetailed().Count; i++)
                                    {
                                        textField.GetComponent<Text>().text += (obj as Facility).GetInfoDetailed().ElementAt(i) + "\n";
                                    }
                                    break;
                                case "Hospital":
                                    textField.GetComponent<Text>().text += "\nWarfare Object \n";
                                    for (int i = 0; i < (obj as Hospital).GetInfoDetailed().Count; i++)
                                    {
                                        textField.GetComponent<Text>().text += (obj as Hospital).GetInfoDetailed().ElementAt(i) + "\n";
                                    }
                                    break;
                                case "Headquaters":
                                    textField.GetComponent<Text>().text += "\nWarfare Object \n";
                                    for (int i = 0; i < (obj as Headquaters).GetInfoDetailed().Count; i++)
                                    {
                                        textField.GetComponent<Text>().text += (obj as Headquaters).GetInfoDetailed().ElementAt(i) + "\n";
                                    }
                                    break;
                                case "Workshop":
                                    textField.GetComponent<Text>().text += "\nWarfare Object \n";
                                    for (int i = 0; i < (obj as Workshop).GetInfoDetailed().Count; i++)
                                    {
                                        textField.GetComponent<Text>().text += (obj as Workshop).GetInfoDetailed().ElementAt(i) + "\n";
                                    }
                                    break;
                                case "TrainingPlace":
                                    textField.GetComponent<Text>().text += "\nWarfare Object \n";
                                    textField.GetComponent<Text>().text += (obj as TrainingPlace).GetInfo() + "\n";
                                    break;
                                case "BattlePosition":
                                    textField.GetComponent<Text>().text += "\nWarfare Object \n";
                                    for (int i = 0; i < (obj as BattlePosition).GetInfoDetailed().Count; i++)
                                    {
                                        textField.GetComponent<Text>().text += (obj as BattlePosition).GetInfoDetailed().ElementAt(i) + "\n";
                                    }
                                    break;
                            }

                            ActionPanelDisplay(panel, obj);
                        }
                        else
                            ActionPanelDisplay(panel, null);
                    }
                    else
                    {
                        ActionPanelDisplay(panel, null);
                    }
                    cont = false;
                    break;
            }

        }
    }

    public void OnClickMap(Image panel)
    {
        var imageRt = (RectTransform)panel.transform;
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();
        string search = "Game";
        int mIndex = panel.name.IndexOf(search);
        string MapName = panel.name.Substring(0, mIndex) + "MapRawImage";
        RawImage mapClicked = CandidateObjects.Where(o => o.name.Equals(MapName)).FirstOrDefault().GetComponent<RawImage>();

        //Find the corresponding Selector map
        string toSearch = "Map";
        int index = mapClicked.name.IndexOf(toSearch);
        string selectorMapName = mapClicked.name.Substring(0, index + toSearch.Length) + "Selector" + mapClicked.name.Substring(index + toSearch.Length);

        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // zero z

        bool inside = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)mapClicked.transform, mouseWorldPos);
        bool outside = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)panel.transform, mouseWorldPos);

        if (inside)  //We clicked the map
        {
            //Find the point in rectangle
            var rt = (RectTransform)mapClicked.transform;
            Vector2 pointInRect = new Vector2(-1, -1);

            bool inner = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)mapClicked.transform,
                Input.mousePosition,
                GameObject.Find("Menu Camera").GetComponent<Camera>(),
                out pointInRect
            );

            //Find the coordinates clicked
            Vector2 textureCoord = pointInRect - rt.rect.min;

            float ratioWidth = mapClicked.texture.width / rt.rect.width;
            textureCoord.x *= ratioWidth;
            textureCoord.y *= ratioWidth;

            textureCoord.x = Mathf.Floor(textureCoord.x);
            textureCoord.y = Mathf.Floor(textureCoord.y);

            //Send them to Global Container
            GlobalDataContainer.selectedCoordinates = textureCoord;

            //Turn on the selector
            GameObject selectorMap = CandidateObjects.Where(o => o.name.Equals(selectorMapName)).FirstOrDefault();
            if (selectorMap != null)
            {
                selectorMap.SetActive(true);

                //Form the transparent texture
                int size = mapClicked.texture.width;  //Since the textures are square, we can use this
                Texture2D selectorTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
                selectorTexture.wrapMode = TextureWrapMode.Clamp;

                Color basicColor = new Color(1, 1, 1, 0.2F);
                var fillColorArray = new Color[size * size];
                for (var i = 0; i < fillColorArray.Length; i++)
                {
                    fillColorArray[i] = basicColor;
                }
                selectorTexture.SetPixels(fillColorArray);
                selectorTexture.SetPixel((int)textureCoord.x, (int)textureCoord.y, new Color(0, 0, 0, 1));
                selectorTexture.Apply();
                //selectorMap.GetComponent<RawImage>().color = Color.clear;
                selectorMap.GetComponent<RawImage>().texture = selectorTexture;
            }
            //Fill Info and Action tabs
            InfoPanelDisplay(panel, textureCoord);
        }
        else if (outside)
        {
            //Turn the selector off
            GameObject selectorMap = CandidateObjects.Where(o => o.name.Equals(selectorMapName)).FirstOrDefault();
            if (selectorMap != null)
            {
                selectorMap.SetActive(false);
            }
            //Clear the info and action panels
            string infoScrollViewName = panel.name.Substring(0, mIndex) + "MapInfoScrollView";
            GameObject iScrollViewContent = panel.transform.Find(infoScrollViewName).Find("Viewport").Find("Content").gameObject;
            var ichildren = iScrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in ichildren)
            {
                if (child.name.Contains("Clone") || !child.name.Equals("Content"))
                    UnityEngine.Object.Destroy(child.gameObject);
            }

            string actionScrollViewName = panel.name.Substring(0, mIndex) + "MapActionScrollView";
            GameObject aScrollViewContent = panel.transform.Find(actionScrollViewName).Find("Viewport").Find("Content").gameObject;
            var achildren = aScrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in achildren)
            {
                if (child.name.Contains("Clone") || !child.name.Equals("Content"))
                    UnityEngine.Object.Destroy(child.gameObject);
            }
            GlobalDataContainer.selectedCoordinates = new Vector2(-1, -1);
        }
    }

    public void TabChanger(string names)
    {
        //Names are "PressedButtonName,CurrentPanelName"
        string[] NamesSeparate = names.Split(',');
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();

        if (NamesSeparate[0].Length == 0)
        {
            PreviousPanel = GameObject.Find(NamesSeparate[1]);
            return;
        }
        //Main panel opens with the buttons including the word "Back"
        else if (NamesSeparate[0].Contains("Back"))
        {
            GameObject pressedButton = CandidateObjects.Where(o => o.name.Equals(NamesSeparate[0])).FirstOrDefault();
            string prevButtonName = PreviousPanel.name.Substring(0, PreviousPanel.name.Length - 9) + "Button";
            GameObject prevButton = CandidateObjects.Where(o => o.name.Equals(prevButtonName)).FirstOrDefault();
            GameObject neededPanel = CandidateObjects.Where(o => o.name.Equals("MainGamePanel")).FirstOrDefault();

            if (pressedButton != null && neededPanel != null && prevButton != null)
            {
                neededPanel.SetActive(true);
                PreviousPanel.SetActive(false);
                PreviousPanel = neededPanel;

                Color prevColor;
                ColorUtility.TryParseHtmlString("#FFA101", out prevColor);
                prevButton.GetComponentInChildren<Text>().color = prevColor;
            }
            return;
        }

        string PanelName = NamesSeparate[0].Substring(0, NamesSeparate[0].Length - 6) + "GamePanel";  //Remove the word "Button" and make the panel name
        string PrevButtonName = PreviousPanel.name.Substring(0, PreviousPanel.name.Length - 9) + "Button";  //Here, remove the "GamePanel" words

        GameObject PressedButton = CandidateObjects.Where(o => o.name.Equals(NamesSeparate[0])).FirstOrDefault();
        GameObject PrevButton = CandidateObjects.Where(o => o.name.Equals(PrevButtonName)).FirstOrDefault();
        GameObject NeededPanel = CandidateObjects.Where(o => o.name.Equals(PanelName)).FirstOrDefault();
        if (PressedButton != null && NeededPanel != null)
        {
            NeededPanel.SetActive(true);
            PreviousPanel.SetActive(false);
            PreviousPanel = NeededPanel;

            Color prevColor;
            Color newColor;
            ColorUtility.TryParseHtmlString("#FFA101", out prevColor);
            ColorUtility.TryParseHtmlString("#FFF300", out newColor);
            PressedButton.GetComponentInChildren<Text>().color = newColor;

            if (PrevButton != null && !PrevButtonName.Equals("MainButton"))
            {
                PrevButton.GetComponentInChildren<Text>().color = prevColor;
            }
        }
    }

    public void SelectorOff()
    {
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();
        string search = "Game";
        int mIndex = PreviousPanel.name.IndexOf(search);
        string MapName = PreviousPanel.name.Substring(0, mIndex) + "MapRawImage";
        GameObject mapClickedG = CandidateObjects.Where(o => o.name.Equals(MapName)).FirstOrDefault();

        if (mapClickedG != null)
        {
            //Find the corresponding Selector map
            RawImage mapClicked = mapClickedG.GetComponent<RawImage>();
            string toSearch = "Map";
            int index = mapClicked.name.IndexOf(toSearch);
            string selectorMapName = mapClicked.name.Substring(0, index + toSearch.Length) + "Selector" + mapClicked.name.Substring(index + toSearch.Length);

            //Turn the selector off
            GameObject selectorMap = CandidateObjects.Where(o => o.name.Equals(selectorMapName)).FirstOrDefault();
            if (selectorMap != null)
            {
                selectorMap.SetActive(false);
            }
            //Clear the info and action panels
            string infoScrollViewName = PreviousPanel.name.Substring(0, mIndex) + "MapInfoScrollView";
            GameObject iScrollViewContent = PreviousPanel.transform.Find(infoScrollViewName).Find("Viewport").Find("Content").gameObject;
            var ichildren = iScrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in ichildren)
            {
                if (child.name.Contains("Clone") || child.name != "Content")
                    UnityEngine.Object.Destroy(child.gameObject);
            }

            string actionScrollViewName = PreviousPanel.name.Substring(0, mIndex) + "MapActionScrollView";
            GameObject aScrollViewContent = PreviousPanel.transform.Find(actionScrollViewName).Find("Viewport").Find("Content").gameObject;
            var achildren = aScrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in achildren)
            {
                if (child.name.Contains("Clone") || child.name != "Content")
                    UnityEngine.Object.Destroy(child.gameObject);
            }
            GlobalDataContainer.selectedCoordinates = new Vector2(-1, -1);
        }
    }

    public void OpenWarfareResPanel()
    {
        GameObject panel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WarfareResourcesGamePanel")).FirstOrDefault();
        if (panel != null)
            panel.SetActive(true);
    }

    public void CloseWarfareResPanel()
    {
        GameObject panel = FindObjectsOfType<GameObject>(false).ToList().Where(o => o.name.Equals("WarfareResourcesGamePanel")).FirstOrDefault();
        if (panel != null)
            panel.SetActive(false);
    }
}
