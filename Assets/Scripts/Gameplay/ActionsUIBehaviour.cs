using EconomicsClasses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LogisticsClasses;
using System;
using WarfareClasses;
using WarfareMechanics;

public class ActionsUIBehaviour : MonoBehaviour
{
    public GlobalDataContainer GlobalDataContainer;
    public GlobalDataController GlobalDataController;
    public GameObject ErrorPanel;
    private string errorLine;

    public void ErrorPanelOpen()
    {
        ErrorPanel.SetActive(true);
        ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text = errorLine;
        ErrorPanel.transform.Find("ErrorConfirmButton").gameObject.SetActive(true);
    }

    public void ErrorPanelClose(GameObject PanelToClose)
    {
        if (PanelToClose != null)
        {
            PanelToClose.SetActive(false);
        }
    }

    public void TryOpenPauseMenu()
    {
        List<GameObject> candidateObjects = FindObjectsOfType<GameObject>(false).ToList();
        GameObject panel = candidateObjects.Where(o => o.name.Contains("GamePanel")).First();
        if (panel != null && !panel.name.Equals("MainGamePanel"))
        {
            errorLine = "ERROR!\nPlease pause the game from main panel!";
            ErrorPanelOpen();
            ErrorPanel.transform.Find("ErrorConfirmButton").gameObject.SetActive(false);
            GlobalDataContainer.pauseSuccessful = false;
        }
        else if (panel != null)
        {
            GlobalDataContainer.pauseSuccessful = true;
        }
    }

    public void NumericInputValueChanged(GameObject inputPanel)
    {
        int newValue = 100;
        //Min and Max values, in order. When changing, change them here as well!
        if (int.TryParse(inputPanel.GetComponent<InputField>().text, out newValue) && newValue >= 1 && newValue <= 1000)
        {
            inputPanel.GetComponent<InputField>().text = newValue.ToString();
        }
        else
        {
            newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
            inputPanel.GetComponent<InputField>().text = newValue.ToString();
        }
    }

    public void ProductDropdownValueChanged(GameObject actPanel)
    {
        Dropdown dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ProductDropdown")).First().transform.GetComponent<Dropdown>();
        dropdown.RefreshShownValue();
    }

    //Called when the Slider was moved, and the corresponding input must be changed
    public void GameplaySliderValueChanger(GameObject actPanel)
    {
        //The names are "InputPanelName,SliderName"
        string panelName = actPanel.name.Substring(0, actPanel.name.Length - 7);
        GameObject inputPanel;
        GameObject inputSlider;

        switch (panelName)
        {
            case "EStorageProductAdd":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                break;
            case "EStorageProductRemove":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("RAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("RAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                break;
            case "EImpactLaunch":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("PowerInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("PowerSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                break;
            case "WContainerUnitAdd":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                break;
            case "WContainerUnitRemove":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                break;
            case "WContainerUnitTransfer":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                break;
            case "WPressureStart":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("StrengthInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("StrengthSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("TimeInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("TimeSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                break;
        }
    }

    //Called when the Input was changed, and the corresponding slider must get the same value
    public void GameplayInputValueChanger(GameObject actPanel)
    {
        string panelName = actPanel.name.Substring(0, actPanel.name.Length - 7);
        GameObject inputPanel;
        GameObject inputSlider;
        int newValue = 1;

        switch (panelName)
        {
            case "EStorageProductAdd":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                break;
            case "EStorageProductRemove":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("RAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("RAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                break;
            case "EImpactLaunch":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("PowerInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("PowerSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                break;
            case "WContainerUnitAdd":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                break;
            case "WContainerUnitRemove":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                break;
            case "WContainerUnitTransfer":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                break;
            case "WPressureStart":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("StrengthInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("StrengthSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("TimeInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("TimeSlider")).First().gameObject;
                inputPanel.GetComponentInChildren<Text>().text = inputSlider.GetComponent<Slider>().value.ToString();
                newValue = int.Parse(inputPanel.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
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
                break;
        }
    }

    private void ProductAmountSliderInputChanged(GameObject actPanel, int maxVal)
    {
        string panelName = actPanel.name.Substring(0, actPanel.name.Length - 7);
        GameObject inputPanel;
        GameObject inputSlider;
        switch (panelName)
        {
            case "EStorageProductAdd":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountSlider")).First().gameObject;
                inputSlider.GetComponent<Slider>().maxValue = maxVal;
                break;
            case "EStorageProductRemove":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("RAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("RAmountSlider")).First().gameObject;
                inputSlider.GetComponent<Slider>().maxValue = maxVal;
                break;
            case "WContainerUnitAdd":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountSlider")).First().gameObject;
                inputSlider.GetComponent<Slider>().maxValue = maxVal;
                break;
            case "WContainerUnitRemove":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountSlider")).First().gameObject;
                inputSlider.GetComponent<Slider>().maxValue = maxVal;
                break;
            case "WContainerUnitTransfer":
                inputPanel = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountInput")).First().gameObject;
                inputSlider = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountSlider")).First().gameObject;
                inputSlider.GetComponent<Slider>().maxValue = maxVal;
                break;

        }
    }

    public void ProductAmountDropdownValueChanged(GameObject actPanel)
    {
        Dropdown dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("ProductDropdown")).First().transform.GetComponent<Dropdown>();
        string amount = dropdown.options[dropdown.value].text.Split(' ').ToList().Last();
        int number = int.Parse(amount);
        ProductAmountSliderInputChanged(actPanel, number);
        dropdown.RefreshShownValue();
    }

    public void UnitAmountDropdownValueChanged(GameObject actPanel)
    {
        Dropdown dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("UnitDropdown")).First().transform.GetComponent<Dropdown>();
        string amount = dropdown.options[dropdown.value].text.Split(' ').ToList().Last();
        int number = int.Parse(amount);
        ProductAmountSliderInputChanged(actPanel, number);
        dropdown.RefreshShownValue();
    }

    public void PlaceDropdownValueChanged(GameObject inputPanel)
    {
        Dropdown dropdown = inputPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTPlaceDropdown")).First().transform.GetComponent<Dropdown>();
        string[] parts = dropdown.options[dropdown.value].text.Split(' ');

        int x = 0;
        if (!int.TryParse(parts[4], out x) || x < 0)
        {
            return;
        }
        int y = 0;
        if (!int.TryParse(parts[7], out y) || y < 0)
        {
            return;
        }
        Text text = inputPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Contains("CoordsText")).First().gameObject.GetComponent<Text>();
        text.text = "Coordinates: X " + x + " , Y " + y;

        dropdown.RefreshShownValue();
    }

    public void PriceEditDropdownValueChanged(GameObject actPanel)
    {
        string panelName = actPanel.name.Substring(0, actPanel.name.Length - 7);
        Dropdown dropdown;
        Text text;
        InputField input;
        double price;
        switch (panelName)
        {
            case "EProductEdit":
                text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Contains("EOldPriceText")).First().gameObject.GetComponent<Text>();
                dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Contains("EProductDropdown")).First().gameObject.GetComponent<Dropdown>();
                input = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("EPriceInputField")).First().gameObject.GetComponent<InputField>();
                dropdown.RefreshShownValue();
                price = GlobalDataController.GetProductPrice(GlobalDataContainer.countryUnderControl, dropdown.options[dropdown.value].text);
                text.text = "Old price: " + price.ToString();
                input.placeholder.GetComponentInChildren<Text>().text = price.ToString();
                break;
            case "LPathwayTypeEdit":
                text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Contains("LOldPriceText")).First().gameObject.GetComponent<Text>();
                dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Contains("LTypeDropdown")).First().gameObject.GetComponent<Dropdown>();
                input = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("LPriceInputField")).First().GetComponent<InputField>();
                dropdown.RefreshShownValue();
                price = GlobalDataController.GetPathwayTypePrice(GlobalDataContainer.countryUnderControl, dropdown.options[dropdown.value].text);
                text.text = "Old price: " + price.ToString();
                input.placeholder.GetComponentInChildren<Text>().text = price.ToString();
                break;
        }
    }

    public void PathwayPointChanged(Image panel)
    {
        Vector2 coordsStart;
        Vector2 coordsEnd;

        Dropdown dropdownStart = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("FromObjectDropdown")).First().transform.GetComponent<Dropdown>();
        Dropdown dropdownEnd = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ToObjectDropdown")).First().transform.GetComponent<Dropdown>();
        Dropdown dropdownType = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
        string textStart = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("FromObjectText")).First().gameObject.GetComponent<Text>().text;
        string textEnd = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("ToObjectText")).First().gameObject.GetComponent<Text>().text;

        if (panel.name.Contains("LPathwayCreate"))
        {
            string[] arrayStart = dropdownStart.options[dropdownStart.value].text.Split(' ');
            string[] arrayEnd = dropdownEnd.options[dropdownEnd.value].text.Split(' ');
            coordsStart = new Vector2(int.Parse(arrayStart[3]), int.Parse(arrayStart[6]));
            coordsEnd = new Vector2(int.Parse(arrayEnd[3]), int.Parse(arrayEnd[6]));

            PathwayType pathType = GlobalDataController.GetPathwayType(GlobalDataContainer.selectedCoordinates, dropdownType.options[dropdownType.value].text);

            PathwayCalculator pathwayCalculator = new PathwayCalculator(coordsStart, coordsEnd);
            int length = pathwayCalculator.Calculate().Count;
            panel.gameObject.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("LengthInputField")).First().gameObject.GetComponent<InputField>().text =
                length.ToString();
            panel.gameObject.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("SumInputField")).First().gameObject.GetComponent<InputField>().text =
                (length * pathType.GetPrice()).ToString();
        }
        else if (panel.name.Contains("LPathwayEdit"))
        {
            string[] arrayStart = textStart.Split(' ');
            string[] arrayEnd = textEnd.Split(' ');
            coordsStart = new Vector2(int.Parse(arrayStart[4]), int.Parse(arrayStart[7]));
            coordsEnd = new Vector2(int.Parse(arrayEnd[4]), int.Parse(arrayEnd[7]));

            PathwayType pathType = GlobalDataController.GetPathwayType(GlobalDataContainer.selectedCoordinates, dropdownType.options[dropdownType.value].text);
            string oldType = panel.gameObject.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldTypeText")).First().gameObject.GetComponent<Text>().text.Split(' ')[2];

            double pathwayPrice = oldType.Equals(pathType.Name) ? 0 : pathType.GetPrice();

            PathwayCalculator pathwayCalculator = new PathwayCalculator(coordsStart, coordsEnd);
            int length = pathwayCalculator.Calculate().Count;
            panel.gameObject.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("SumInputField")).First().gameObject.GetComponent<InputField>().text =
                (length * pathwayPrice).ToString();
        }
    }

    public void LayeredActionPanelOpen(Button callerButton)
    {
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();

        string actionPanelName = "ActionPanelLayer";

        GameObject actionPanel = CandidateObjects.Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            string scrollViewName = "MainActionScrollView";
            GameObject scrollViewContent = actionPanel.transform.Find(scrollViewName).Find("Viewport").Find("Content").gameObject;

            var children = scrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in children)
            {
                if (child.name.Contains("Clone") || child.name != "Content")
                    UnityEngine.Object.Destroy(child.gameObject);
            }

            string callerButtonName = callerButton.name;
            string callerPanelName = callerButton.transform.parent.name;
            GameObject callerPanel = CandidateObjects.Where(o => o.name.Equals(callerPanelName)).FirstOrDefault();
            Dropdown productDropdown = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ProductionDropdown")).First().GetComponent<Dropdown>();
            GameObject actPanel;
            GameObject oldNameText;
            Dropdown dropdown;
            List<Product> products;
            List<string> info;
            GameObject additScrollViewContent;

            switch (callerButtonName)
            {
                case "AddProductionButton":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EProductionCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Facility name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Facility).GetName();
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Parent industry: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Facility).parentIndustry.Name;
                    
                    products = GlobalDataController.GetUnusedProducts(GlobalDataContainer.selectedCoordinates, 
                        (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Facility).parentIndustry.Name);
                    products = products.Except(GlobalDataController.GetProductsOfFacility(GlobalDataContainer.selectedCoordinates)).ToList();
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ProductDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < products.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = products.ElementAt(i).Name });
                        if (products[i].OriginalResource != null)
                            dropdown.options[i].text += " (" + products[i].OriginalResource.Name + ")";
                        else
                            dropdown.options[i].text += " (Multiple)";
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "EditProductionButton":
                    if (productDropdown.options.Count == 0)
                    {
                        errorLine = "ERROR!\nYou can't edit a non-existing production!";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EProductionEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    additScrollViewContent = actPanel.transform.Find("InfoScrollView").Find("Viewport").Find("Content").gameObject;
                    info = GlobalDataController.GetProductOfFacility(GlobalDataContainer.selectedCoordinates, productDropdown.options[productDropdown.value].text.Split(' ')[0]);
                    for (int i = 0; i < info.Count; i++)
                    {
                        GameObject textField = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("InfoText")).FirstOrDefault());
                        textField.transform.SetParent(additScrollViewContent.transform, false);
                        textField.SetActive(true);
                        textField.GetComponent<Text>().text = info[i];
                    }
                    break;
                case "RemoveProductionButton":
                    if (productDropdown.options.Count == 0)
                    {
                        errorLine = "ERROR!\nYou can't remove a non-existing production!";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EProductionRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    additScrollViewContent = actPanel.transform.Find("InfoScrollView").Find("Viewport").Find("Content").gameObject;
                    info = GlobalDataController.GetProductOfFacility(GlobalDataContainer.selectedCoordinates, productDropdown.options[productDropdown.value].text.Split(' ')[0]);
                    for (int i = 0; i < info.Count; i++)
                    {
                        GameObject textField = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("InfoText")).FirstOrDefault());
                        textField.transform.SetParent(additScrollViewContent.transform, false);
                        textField.SetActive(true);
                        textField.GetComponent<Text>().text = info[i];
                    }
                    break;
                case "AddProductButton":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EStorageProductAdd")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Facility name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Storage).GetName();
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    info = GlobalDataController.GetAllProductsNotInStorage(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("AProductDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    ProductAmountDropdownValueChanged(actPanel.gameObject);
                    break;
                case "RemoveProductButton":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EStorageProductRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Facility name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Storage).GetName();
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    info = GlobalDataController.GetProductsInStorage(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("RProductDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    ProductAmountDropdownValueChanged(actPanel.gameObject);
                    break;
                case "EditProductButton":
                    //if (GlobalDataContainer.selectedCoordinates == new Vector2(-1, -1))
                    //{
                    //    errorLine = "ERROR!\nPlease click the map at least once!";
                    //    ErrorPanelOpen();
                    //    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    //    GlobalDataContainer.actionValid = false;
                    //    break;
                    //}
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EProductEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    info = GlobalDataController.GetProductsNoAmounts(GlobalDataContainer.countryUnderControl);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("EProductDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    PriceEditDropdownValueChanged(actPanel.gameObject);
                    break;
            }
        }
    }

    public void GeneralActionPanelOpen(Image callerPanel)
    {
        string actionPanelName = "ActionPanel";

        GameObject actionPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            string scrollViewName = "MainActionScrollView";
            GameObject scrollViewContent = actionPanel.transform.Find(scrollViewName).Find("Viewport").Find("Content").gameObject;

            var children = scrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in children)
            {
                if (child.name.Contains("Clone") || child.name != "Content")
                    UnityEngine.Object.Destroy(child.gameObject);
            }

            string callerPanelName = callerPanel.name.Substring(0, callerPanel.name.Length - 7);
            GameObject actPanel;
            GameObject coordsText;
            GameObject oldNameText;
            Dropdown dropdown;
            switch (callerPanelName)
            {
                case "ACreateStorage":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EStorageCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    break;
                case "ACreateCity":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ECityCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    break;
                case "ACreateFacility":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EFacilityCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    //Get object coordinates
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    //Get possible parent industries
                    List<string> industries = GlobalDataController.GetIndustriesByResources(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("IndustryDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < industries.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = industries.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ACreatePathway":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("LPathwayCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    //Get pathway types
                    List<string> pathwayTypes = GlobalDataController.GetPathwayTypesNames(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < pathwayTypes.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = pathwayTypes.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    //Get objects, both start...
                    List<string> objects = GlobalDataController.GetObjectsForPathForming(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("FromObjectDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = objects.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    //...and end
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ToObjectDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = objects.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "AEditStorage":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EStorageEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Old name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Storage).GetName();
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldCapacityText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Old capacity: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Storage).GetCapacity();
                    List<string> info = GlobalDataController.GetProductsInStorage(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ProductsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "AEditCity":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ECityEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Old name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as City).Name;
                    break;
                case "AEditFacility":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EFacilityEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Old name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Facility).GetName();
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Parent industry: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Facility).parentIndustry.Name;
                    List<Product> products = GlobalDataController.GetProductsOfFacility(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ProductionDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < products.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = products.ElementAt(i).Name });
                        if (products[i].OriginalResource != null)
                            dropdown.options[i].text += " (" + products[i].OriginalResource.Name + ")";
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "AEditPathway":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("LPathwayEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    Pathway pathway = GlobalDataController.HasObjectInPlace(GlobalDataContainer.selectedCoordinates) as Pathway;

                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("FromObjectText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "From: " + GlobalDataController.GetObjectGeneralInfo(pathway.BeginCoordinates);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("ToObjectText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "To: " + GlobalDataController.GetObjectGeneralInfo(pathway.EndCoordinates);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldTypeText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Pathway type: " + pathway.Type.Name;

                    List<string> pathwayTypesE = GlobalDataController.GetPathwayTypesNames(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < pathwayTypesE.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = pathwayTypesE.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemoveStorage":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EStorageRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Storage name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Storage).GetName();
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CapacityText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Storage).GetCapacity().ToString();
                    List<string> contained = GlobalDataController.GetProductsInStorage(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ProductsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < contained.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = contained.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemoveCity":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("ECityRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "City name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as City).Name;
                    break;
                case "ARemoveFacility":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EFacilityRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x + 
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Facility name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Facility).GetName();
                    List<string> product = GlobalDataController.GetProductionOfFacility(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ProductionDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < product.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = product.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemovePathway":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("LPathwayRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    Pathway pathw = GlobalDataController.HasObjectInPlace(GlobalDataContainer.selectedCoordinates) as Pathway;

                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("FromObjectText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "From: " + GlobalDataController.GetObjectGeneralInfo(pathw.BeginCoordinates);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("ToObjectText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "To: " + GlobalDataController.GetObjectGeneralInfo(pathw.EndCoordinates);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TypeText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Pathway type: " + pathw.Type.Name;

                    GameObject lengthInput = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("LengthInputField")).First().gameObject;
                    lengthInput.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text = pathw.Length.ToString();
                    break;

                case "ACreatePath":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("LPathCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    //Get objects, both start...
                    List<string> objs = GlobalDataController.GetObjectsForPathForming(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("FromObjectDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = objs.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    //...and end
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ToObjectDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = objs.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemovePath":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("LPathRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("LogisticsRouteSelectDropdown")).First().transform.GetComponent<Dropdown>();
                    string[] routeInfo = dropdown.options[dropdown.value].text.Split(' ');
                    Vector2 start = new Vector2(int.Parse(routeInfo[2]), int.Parse(routeInfo[4]));
                    Vector2 end = new Vector2(int.Parse(routeInfo[8]), int.Parse(routeInfo[10]));
                    List<string> objes = GlobalDataController.GetRouteMidwayObjects(start, end);

                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("FromObjectText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "From: " + GlobalDataController.GetObjectGeneralInfo(start);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("ToObjectText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "To: " + GlobalDataController.GetObjectGeneralInfo(end);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("MidwayObjectsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < objes.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = objes.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ACreateImpact":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("EImpactLaunch")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    break;
                case "AEditPathwayType":
                    if (GlobalDataContainer.selectedCoordinates == new Vector2(-1, -1))
                    {
                        errorLine = "ERROR!\nPlease click the map at least once!";
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        ErrorPanelOpen();
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("LPathwayTypeEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    info = GlobalDataController.GetPathwayTypesNames(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("LTypeDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    PriceEditDropdownValueChanged(actPanel.gameObject);
                    break;
            }
        }
    }

    public void WarfareLayerPanelOpen(Button callerButton)
    {
        List<GameObject> CandidateObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true).ToList();

        string actionPanelName = "ActionPanelLayer";

        GameObject actionPanel = CandidateObjects.Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            string scrollViewName = "MainActionScrollView";
            GameObject scrollViewContent = actionPanel.transform.Find(scrollViewName).Find("Viewport").Find("Content").gameObject;

            var children = scrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in children)
            {
                if (child.name.Contains("Clone") || child.name != "Content")
                    UnityEngine.Object.Destroy(child.gameObject);
            }

            string callerButtonName = callerButton.name;
            string callerPanelName = callerButton.transform.parent.name;
            GameObject callerPanel = CandidateObjects.Where(o => o.name.Equals(callerPanelName)).FirstOrDefault();
            Dropdown productDropdown = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("UnitsDropdown")).First().GetComponent<Dropdown>();
            GameObject actPanel;
            GameObject oldNameText;
            Dropdown dropdown;
            List<string> units;
            List<string> info;

            switch (callerButtonName) 
            {
                case "AddUnitButton":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerUnitAdd")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Container name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as ContainerObject).CustomName;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Coordinates: X " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as ContainerObject).Coords.x +
                        ", Y " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as ContainerObject).Coords.y;

                    units = GlobalDataController.GetUnitsInBank(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("WAUnitDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < units.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = units.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    UnitAmountDropdownValueChanged(actPanel.gameObject);
                    break;
                case "RemoveUnitButton":
                    if (productDropdown.options.Count == 0)
                    {
                        errorLine = "ERROR!\nYou can't remove a non-existing unit!";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerUnitRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Container name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as ContainerObject).CustomName;
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Coordinates: X " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as ContainerObject).Coords.x +
                        ", Y " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as ContainerObject).Coords.y;

                    units = GlobalDataController.GetUnitsInContainer(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("WRUnitDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < units.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = units.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    UnitAmountDropdownValueChanged(actPanel.gameObject);
                    break;
                case "AddStationaryUnitButton":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WStationaryObjectCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    units = GlobalDataController.GetStationaryObjects(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < units.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = units.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "AddSelectedUnitButton":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerUnitAdd")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Container name: Battle position";
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Coordinates: X " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as BattlePosition).Position.x +
                        ", Y " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as BattlePosition).Position.y;

                    units = GlobalDataController.GetUnitsInBank(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("WAUnitDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < units.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = units.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    UnitAmountDropdownValueChanged(actPanel.gameObject);
                    break;
                case "RemoveSelectedUnitButton":
                    if (productDropdown.options.Count == 0)
                    {
                        errorLine = "ERROR!\nYou can't remove a non-existing unit!";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    if (GlobalDataController.GetUnitCategory(productDropdown.options[productDropdown.value].text.Split(' ')[0]) == "S")
                    {
                        errorLine = "ERROR!\nYou can't remove a stationary object!\nThese objects are removed together with a battle position";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerUnitRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Container name: Battle position";
                    oldNameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    oldNameText.GetComponent<Text>().text = "Coordinates: X " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as BattlePosition).Position.x +
                        ", Y " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as BattlePosition).Position.y;

                    units = GlobalDataController.GetUnitsOnBattlePosition(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("WRUnitDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < units.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = units.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    UnitAmountDropdownValueChanged(actPanel.gameObject);
                    break;
                case "TransferSelectedUnitButton":
                    if (productDropdown.options.Count == 0)
                    {
                        errorLine = "ERROR!\nYou can't transfer a non-existing unit!";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    if (GlobalDataController.GetUnitCategory(productDropdown.options[productDropdown.value].text.Split(' ')[0]) == "S")
                    {
                        errorLine = "ERROR!\nYou can't transfer a stationary object!\nThese objects are removed together with a battle position";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    if (float.Parse(productDropdown.options[productDropdown.value].text.Split(' ')[3]) == 100F)
                    {
                        errorLine = "ERROR!\nYou can't transfer a non-defective unit!\nConsider removing a unit instead";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerUnitTransfer")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    units = GlobalDataController.GetDefectiveUnitsOnBattlePosition(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("WTUnitDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < units.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = units.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    UnitAmountDropdownValueChanged(actPanel.gameObject);

                    info = GlobalDataController.GetWarfareObjectsInfo(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("WTPlaceDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    PlaceDropdownValueChanged(actPanel.gameObject);
                    break;
            }
        }
    }

    public void WarfareActionPanelOpen(Image callerPanel)
    {
        string actionPanelName = "ActionPanel";

        GameObject actionPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            string scrollViewName = "MainActionScrollView";
            GameObject scrollViewContent = actionPanel.transform.Find(scrollViewName).Find("Viewport").Find("Content").gameObject;

            var children = scrollViewContent.GetComponentsInChildren<Transform>().ToArray();
            foreach (var child in children)
            {
                if (child.name.Contains("Clone") || child.name != "Content")
                    UnityEngine.Object.Destroy(child.gameObject);
            }

            string callerPanelName = callerPanel.name.Substring(0, callerPanel.name.Length - 7);
            GameObject actPanel;
            GameObject coordsText;
            GameObject text;
            Dropdown dropdown;
            List<string> info;
            switch (callerPanelName)
            {
                case "ACreateHeadquaters":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "CREATE HEADQUATERS";
                    break;
                case "ACreateHospital":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "CREATE A HOSPITAL";
                    break;
                case "ACreateWorkshop":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "CREATE A WORKSHOP";
                    break;
                case "ACreateTrainingField":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "CREATE TRAINING FIELD";
                    break;
                case "ACreateArmyman":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WSoldierCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    //Get possible soldier specialities
                    List<string> specialities = GlobalDataController.GetSoldierSpecialities(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < specialities.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = specialities.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "AEditHeadquaters":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "EDIT HEADQUATERS";
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().gameObject;
                    text.GetComponent<Text>().text = "Old name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Headquaters).CustomName;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldCapacityText")).First().gameObject;
                    text.GetComponent<Text>().text = "Old capacity: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Headquaters).Capacity;
                    info = GlobalDataController.GetUnitsInWarfareContainer(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "AEditHospital":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "EDIT HOSPITAL";
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().gameObject;
                    text.GetComponent<Text>().text = "Old name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Hospital).CustomName;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldCapacityText")).First().gameObject;
                    text.GetComponent<Text>().text = "Old capacity: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Hospital).Capacity;
                    info = GlobalDataController.GetUnitsInWarfareContainer(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "AEditWorkshop":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "EDIT WORKSHOP";
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().gameObject;
                    text.GetComponent<Text>().text = "Old name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Workshop).CustomName;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldCapacityText")).First().gameObject;
                    text.GetComponent<Text>().text = "Old capacity: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Workshop).Capacity;
                    info = GlobalDataController.GetUnitsInWarfareContainer(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemoveHeadquaters":
                    actionPanel.SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("CapacityText")).First().SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsText")).First().SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsDropdown")).First().SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "ARE YOU SURE YOU WANT\nTO REMOVE THIS HEADQUATERS?";
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    text.GetComponent<Text>().text = "Name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Headquaters).CustomName;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CapacityText")).First().gameObject;
                    text.GetComponent<Text>().text = "Capacity: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Headquaters).Capacity;
                    info = GlobalDataController.GetUnitsInWarfareContainer(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemoveHospital":
                    actionPanel.SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("CapacityText")).First().SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsText")).First().SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsDropdown")).First().SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "ARE YOU SURE YOU WANT\nTO REMOVE THIS HOSPITAL?";
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    text.GetComponent<Text>().text = "Name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Hospital).CustomName;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CapacityText")).First().gameObject;
                    text.GetComponent<Text>().text = "Capacity: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Hospital).Capacity;
                    info = GlobalDataController.GetUnitsInWarfareContainer(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemoveWorkshop":
                    actionPanel.SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("CapacityText")).First().SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsText")).First().SetActive(true);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsDropdown")).First().SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "ARE YOU SURE YOU WANT\nTO REMOVE THIS WORKSHOP?";
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    text.GetComponent<Text>().text = "Name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Workshop).CustomName;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CapacityText")).First().gameObject;
                    text.GetComponent<Text>().text = "Capacity: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Workshop).Capacity;
                    info = GlobalDataController.GetUnitsInWarfareContainer(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemoveTrainingField":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WContainerTemplateRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().gameObject;
                    text.GetComponent<Text>().text = "ARE YOU SURE YOU WANT\nTO REMOVE THIS TRAINING FIELD?";
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    text = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().gameObject;
                    text.GetComponent<Text>().text = "Name: " + (GlobalDataController.GetObjectOnCoords(GlobalDataContainer.selectedCoordinates) as Workshop).CustomName;
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("CapacityText")).First().SetActive(false);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsText")).First().SetActive(false);
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("UnitsDropdown")).First().SetActive(false);
                    break;
                case "ACreateBattlePosition":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WBattlePositionCreate")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    break;
                case "AEditBattlePosition":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WBattlePositionEdit")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    info = GlobalDataController.GetUnitsOnBattlePosition(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "ARemoveBattlePosition":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WBattlePositionRemove")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    info = GlobalDataController.GetUnitsOnBattlePosition(GlobalDataContainer.selectedCoordinates);
                    dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("UnitsDropdown")).First().transform.GetComponent<Dropdown>();
                    dropdown.options.Clear();
                    for (int i = 0; i < info.Count; i++)
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = info.ElementAt(i) });
                    }
                    dropdown.value = 0;
                    dropdown.RefreshShownValue();
                    break;
                case "APutPressure":
                    actionPanel.SetActive(true);
                    actPanel = Instantiate(FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("WPressureStart")).FirstOrDefault());
                    actPanel.transform.SetParent(scrollViewContent.transform, false);
                    coordsText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CoordsText")).First().gameObject;
                    coordsText.GetComponent<Text>().text = "Coordinates: X " + (int)GlobalDataContainer.selectedCoordinates.x +
                        ", Y " + (int)GlobalDataContainer.selectedCoordinates.y;
                    break;
            }
        }
    }

    public void GeneralActionPanelClose()
    {
        string actionPanelName = "ActionPanel";
        GameObject actionPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    public void GeneralActionLayerPanelClose()
    {
        string actionPanelName = "ActionPanelLayer";
        GameObject actionPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    public void ActionPanelLayerValidateData(Button callerButton)
    {
        string callerPanelName = callerButton.transform.parent.name.Substring(0, callerButton.transform.parent.name.Length - 7);
        GameObject actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(callerButton.transform.parent.name)).FirstOrDefault();
        GameObject prevPanel = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("EFacilityEdit")).First();
        bool resourcesPresent;

        switch (callerPanelName)
        {
            case "EProductionCreate":
                resourcesPresent = GlobalDataController.HasEnoughResourcesForCreate();
                if (!resourcesPresent)
                {
                    errorLine = "ERROR!\nThere aren't enough free resources to start this production!\nPlease check the amounts of resources " +
                        "for the production\nyou want to start";
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    ErrorPanelOpen();
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "EProductionEdit":
                //NEEDED add the Increase limit when there's not enough resources nearby / transfered when I make this  DONE
                Dropdown productDropdown = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("ProductionDropdown")).First().GetComponent<Dropdown>();
                List<string> infoArray = GlobalDataController.GetProductOfFacility(GlobalDataContainer.selectedCoordinates, productDropdown.options[productDropdown.value].text.Split(' ')[0]);
                string info = infoArray.Last();
                int power = int.Parse(info.Split(' ')[info.Split(' ').Length - 1]);
                resourcesPresent = GlobalDataController.HasEnoughResourcesForUpgrade();
                if (power == 1 && callerButton.name.Equals("DecreaseButton"))
                {
                    errorLine = "ERROR!\nThe production power is 1 and can't be decreased!\nConsider removing the production instead";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                if (!resourcesPresent && callerButton.name.Equals("IncreaseButton"))
                {
                    errorLine = "ERROR!\nThere aren't enough free resources to make an upgrade!\nPlease check the amounts of resources for the production\nyou want to increase";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "EProductionRemove":
                GlobalDataContainer.actionValid = true;
                break;
            case "EStorageProductAdd":
                Dropdown dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("AProductDropdown")).First().transform.GetComponent<Dropdown>();
                string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
                string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
                int amount = int.Parse(actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
                Storage storage = GlobalDataController.HasObjectInPlace(GlobalDataContainer.selectedCoordinates) as Storage;
                if (amount > storage.GetFreeCapacity())
                {
                    errorLine = "ERROR!\nThe storage doesn't have enough free space!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "EStorageProductRemove":
                GlobalDataContainer.actionValid = true;
                break;
            case "EProductEdit":
                if (GlobalDataContainer.selectedCoordinates == new Vector2(-1, -1))
                {
                    errorLine = "ERROR!\nPlease click the map at least once!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
        }
    }

    public void GeneralActionPanelValidateData(Image callerPanel)
    {
        string callerPanelName = callerPanel.name.Substring(0, callerPanel.name.Length - 7);
        GameObject actPanel;
        string nameText;
        switch (callerPanelName)
        {
            case "EStorageCreate":
                actPanel = callerPanel.gameObject;
                if (GlobalDataController.HasObjectNearby(GlobalDataContainer.selectedCoordinates) != null)
                {
                    errorLine = "ERROR!\nAt least one neighboring cell is already occupied!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (string.IsNullOrEmpty(nameText))
                {
                    errorLine = "ERROR!\nThe name must not be empty!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                else if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "ECityCreate":
                actPanel = callerPanel.gameObject;
                if (GlobalDataController.HasObjectNearby(GlobalDataContainer.selectedCoordinates) != null)
                {
                    errorLine = "ERROR!\nAt least one neighboring cell is already occupied!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (string.IsNullOrEmpty(nameText))
                {
                    errorLine = "ERROR!\nThe name must not be empty!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                else if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "EFacilityCreate":
                actPanel = callerPanel.gameObject;
                if (GlobalDataController.HasObjectNearby(GlobalDataContainer.selectedCoordinates) != null)
                {
                    errorLine = "ERROR!\nAt least one neighboring cell is already occupied!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (string.IsNullOrEmpty(nameText))
                {
                    errorLine = "ERROR!\nThe name must not be empty!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                else if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "LPathwayCreate":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("LPathwayCreate")).FirstOrDefault();
                Dropdown from = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("FromObjectDropdown")).First().transform.GetComponent<Dropdown>();
                Dropdown to = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ToObjectDropdown")).First().transform.GetComponent<Dropdown>();
                if (from.options[from.value].text == to.options[to.value].text)
                {
                    errorLine = "ERROR!\nThe start and end point of the pathway\nmust be different!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }

                string[] arrayStart = from.options[from.value].text.Split(' ');
                string[] arrayEnd = to.options[to.value].text.Split(' ');
                Vector2 coordsStart = new Vector2(int.Parse(arrayStart[3]), int.Parse(arrayStart[6]));
                Vector2 coordsEnd = new Vector2(int.Parse(arrayEnd[3]), int.Parse(arrayEnd[6]));
                List<Vector2> path = new PathwayCalculator(coordsStart, coordsEnd).Calculate();
                bool cont = true;
                for (int i = 0; i < path.Count; i++)
                {
                    try
                    {
                        if (!GlobalDataContainer.ComplexMap[(int)path.ElementAt(i).x, (int)path.ElementAt(i).y].Country.Name.Equals(GlobalDataContainer.countryUnderControl))
                        {
                            errorLine = "ERROR!\nThe path will go on a territory of other country!\nPlease find another end point";
                            ErrorPanelOpen();
                            FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                            GlobalDataContainer.actionValid = false;
                            cont = false;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLine = "EROR!\nThe path will go on a territory of other country!\nPlease find another end point";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        cont = false;
                        break;
                    }
                    if (i != 0 && i != path.Count - 1)
                    {
                        object obj = GlobalDataController.HasObjectInPlace(path.ElementAt(i));
                        if (obj != null && obj.GetType().Name.ToString() != "Pathway")
                        {
                            errorLine = "ERROR!\nAt least one pathway cell is already occupied!";
                            ErrorPanelOpen();
                            FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                            GlobalDataContainer.actionValid = false;
                            cont = false;
                            break;
                        }
                    }
                }
                if (!cont)
                    break;

                GlobalDataContainer.actionValid = true;
                break;
            case "EStorageEdit":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("EStorageEdit")).FirstOrDefault();
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                int capacity = GlobalDataController.GetStorageOccupiedCapacity(GlobalDataContainer.selectedCoordinates);
                int capacityToSet;
                if (string.IsNullOrEmpty(actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).First().
                    gameObject.GetComponent<InputField>().text))
                    capacityToSet = int.Parse(actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).First().
                    gameObject.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
                else
                    capacityToSet = int.Parse(actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).First().
                    gameObject.GetComponent<InputField>().text);
                if (capacity > capacityToSet)
                {
                    errorLine = "ERROR!\nThe capacity to set is smaller\nthan this storage's occupied capacity!\n" +
                        "Please free the storage or set a bigger capacity";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "ECityEdit":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("ECityEdit")).FirstOrDefault();
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "EFacilityEdit":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("EFacilityEdit")).FirstOrDefault();
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "LPathwayEdit":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("LPathwayEdit")).FirstOrDefault();
                GlobalDataContainer.actionValid = true;
                break;
            case "EStorageRemove":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("EStorageRemove")).FirstOrDefault();
                int capac = GlobalDataController.GetStorageCapacity(GlobalDataContainer.selectedCoordinates);
                int freeCapac = GlobalDataController.GetStorageFreeCapacity(GlobalDataContainer.selectedCoordinates);
                if (capac != freeCapac)
                {
                    errorLine = "ERROR!\nThe storage is not empty!\nIf you proceed, the products contained in it will be removed!\n" +
                        "Are you sure you want to proceed?";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(true);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "ECityRemove":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("ECityRemove")).FirstOrDefault();
                GlobalDataContainer.actionValid = true;
                break;
            case "EFacilityRemove":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("EFacilityRemove")).FirstOrDefault();
                int production = GlobalDataController.GetFacilityTotalProduction(GlobalDataContainer.selectedCoordinates);
                if (production != 0)
                {
                    errorLine = "ERROR!\nThis facility has working productions!\nIf you proceed, all its productions will be stopped!\n" + 
                        "Are you sure you want to proceed?";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(true);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "LPathwayRemove":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("LPathwayRemove")).FirstOrDefault();
                Pathway pathw = GlobalDataController.HasObjectInPlace(GlobalDataContainer.selectedCoordinates) as Pathway;
                if (GlobalDataController.HasRouteWithEdge(pathw.BeginCoordinates, pathw.EndCoordinates))
                {
                    errorLine = "ERROR!\nThis pathway is a part of at least one logistic route!\n If you proceed, the routes will be broken!\n" + 
                        "Are you sure you want to proceed?";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(true);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;

            case "LPathCreate":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("LPathCreate")).FirstOrDefault();
                Dropdown fromO = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("FromObjectDropdown")).First().transform.GetComponent<Dropdown>();
                Dropdown toO = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("ToObjectDropdown")).First().transform.GetComponent<Dropdown>();
                if (fromO.options[fromO.value].text == toO.options[toO.value].text)
                {
                    errorLine = "ERROR!\nThe start and end point of the path\nmust be different!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }

                string[] arrayStartC = fromO.options[fromO.value].text.Split(' ');
                string[] arrayEndC = toO.options[toO.value].text.Split(' ');
                Vector2 coordsStartC = new Vector2(int.Parse(arrayStartC[3]), int.Parse(arrayStartC[6]));
                Vector2 coordsEndC = new Vector2(int.Parse(arrayEndC[3]), int.Parse(arrayEndC[6]));
                if (GlobalDataController.HasRoute(coordsStartC, coordsEndC))
                {
                    errorLine = "ERROR!\nThe path with the set\nstart and end points already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "LPathRemove":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("LPathRemove")).FirstOrDefault();
                GlobalDataContainer.actionValid = true;
                break;
            case "EImpactCreate":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("EImpactCreate")).FirstOrDefault();
                GlobalDataContainer.actionValid = true;
                break;
            case "LPathwayTypeEdit":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("LPathwayTypeEdit")).FirstOrDefault();
                if (GlobalDataContainer.selectedCoordinates == new Vector2(-1, -1))
                {
                    errorLine = "ERROR!\nPlease click the map at least once!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
        }
    }

    public void WarfareLayerValidateData(Button callerButton)
    {
        string callerPanelName = callerButton.transform.parent.name.Substring(0, callerButton.transform.parent.name.Length - 7);
        GameObject actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(callerButton.transform.parent.name)).FirstOrDefault();
        GameObject prevPanelContainer = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("WContainerTemplateEdit(Clone)")).FirstOrDefault();
        GameObject prevPanelPosition = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("WBattlePositionEdit(Clone)")).FirstOrDefault();
        Dropdown dropdown;
        string productName;
        ContainerObject container;
        int amount;

        switch (callerPanelName)
        {
            case "WContainerUnitAdd":
                if (!GlobalDataController.DoesContainerFitUnit(actPanel))
                {
                    errorLine = "ERROR!\nThis unit doesn't belong to the selected container!\nPlease reconsider your choice";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WAUnitDropdown")).First().transform.GetComponent<Dropdown>();
                productName = dropdown.options[dropdown.value].text.Split(' ')[0];
                amount = int.Parse(actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
                if (prevPanelContainer != null)
                {
                    container = GlobalDataController.HasObjectInPlace(GlobalDataContainer.selectedCoordinates) as ContainerObject;
                    if (amount > container.CurrentCapacity)
                    {
                        errorLine = "ERROR!\nThere's not enough free space!";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    GlobalDataContainer.unitForPosition = false;
                }
                else
                    GlobalDataContainer.unitForPosition = true;
                GlobalDataContainer.actionValid = true;
                break;
            case "WContainerUnitRemove":
                if (prevPanelContainer != null)
                    GlobalDataContainer.unitForPosition = false;
                else
                    GlobalDataContainer.unitForPosition = true;
                GlobalDataContainer.actionValid = true;
                break;
            case "WContainerUnitTransfer":
                if (!GlobalDataController.DoesContainerFitUnit(actPanel))
                {
                    errorLine = "ERROR!\nThis unit doesn't belong to the selected container!\nPlease reconsider your choice";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WTUnitDropdown")).First().transform.GetComponent<Dropdown>();
                productName = dropdown.options[dropdown.value].text.Split(' ')[0];
                amount = int.Parse(actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
                dropdown = actPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WTPlaceDropdown")).First().transform.GetComponent<Dropdown>();
                int x = int.Parse(dropdown.options[dropdown.value].text.Split(' ')[4]);
                int y = int.Parse(dropdown.options[dropdown.value].text.Split(' ')[7]);
                object obj = GlobalDataController.HasObjectInPlace(new Vector2(x, y));
                if (obj is BattlePosition)
                {
                    BattlePosition battlePos = (BattlePosition)obj;
                    GlobalDataContainer.unitForPosition = true;
                }
                else
                {
                    container = obj as ContainerObject;
                    if (amount > container.CurrentCapacity)
                    {
                        errorLine = "ERROR!\nThere's not enough free space!";
                        ErrorPanelOpen();
                        FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                        GlobalDataContainer.actionValid = false;
                        break;
                    }
                    GlobalDataContainer.unitForPosition = false;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "WStationaryObjectCreate":
                GlobalDataContainer.actionValid = true;
                break;
        }
    }

    public void WarfareActionPanelValidateData(Image callerPanel)
    {
        string callerPanelName = callerPanel.name.Substring(0, callerPanel.name.Length - 7);
        GameObject actPanel;
        Dropdown dropdown;
        string nameText;
        switch (callerPanelName) 
        {
            case "WContainerTemplateCreate":
                actPanel = callerPanel.gameObject;
                if (GlobalDataController.HasObjectNearby(GlobalDataContainer.selectedCoordinates) != null)
                {
                    errorLine = "ERROR!\nAt least one neighboring cell is already occupied!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (string.IsNullOrEmpty(nameText))
                {
                    errorLine = "ERROR!\nThe name must not be empty!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                else if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "WContainerTemplateEdit":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("WContainerTemplateEdit")).FirstOrDefault();
                nameText = actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;
                if (GlobalDataController.HasSameName(GlobalDataContainer.selectedCoordinates, nameText))
                {
                    errorLine = "ERROR!\nThe object with this name already exists!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                int capacity = GlobalDataController.GetContainerOccupiedCapacity(GlobalDataContainer.selectedCoordinates);
                int capacityToSet;
                if (string.IsNullOrEmpty(actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).First().
                    gameObject.GetComponent<InputField>().text))
                    capacityToSet = int.Parse(actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).First().
                    gameObject.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text);
                else
                    capacityToSet = int.Parse(actPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).First().
                    gameObject.GetComponent<InputField>().text);
                if (capacity > capacityToSet)
                {
                    errorLine = "ERROR!\nThe capacity to set is smaller\nthan this place's occupied capacity!\n" +
                        "Please free the place or set a bigger capacity";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "WContainerTemplateRemove":
                actPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Contains("WContainerTemplateRemove")).FirstOrDefault();
                int capac = GlobalDataController.GetContainerCapacity(GlobalDataContainer.selectedCoordinates);
                int freeCapac = GlobalDataController.GetContainerFreeCapacity(GlobalDataContainer.selectedCoordinates);
                if (capac != freeCapac)
                {
                    errorLine = "ERROR!\nThe place is not empty!\nIf you proceed, the defective units contained in it will be removed!\n" +
                        "Are you sure you want to proceed?";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(true);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "WSoldierCreate":
                dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
                string[] parts = dropdown.options[dropdown.value].text.Split(' ');
                int resourcesPresent = GlobalDataController.HasEnoughUnitsToTrain(GlobalDataContainer.selectedCoordinates, parts[0], int.Parse(parts[2]), 0).Count;
                if (resourcesPresent == 0)
                {
                    errorLine = "ERROR!\nThere's not enough resources to start the training\nof the unit you want!\nPlease check the amount " +
                        "of resources needed for the unit";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "WBattlePositionCreate":
                if (GlobalDataController.HasObjectNearby(GlobalDataContainer.selectedCoordinates) != null)
                {
                    errorLine = "ERROR!\nAt least one neighboring cell is already occupied!";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(false);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "WBattlePositionEdit":
                GlobalDataContainer.actionValid = true;
                break;
            case "WBattlePositionRemove":
                int unitsPresent = GlobalDataController.GetUnitsOnBattlePosition(GlobalDataContainer.selectedCoordinates).Count();
                if (unitsPresent != 0)
                {
                    errorLine = "ERROR!\nThe position still has units on it!\nIf you proceed, the defective units contained in it will be removed!\n" +
                        "Are you sure you want to proceed?";
                    ErrorPanelOpen();
                    FindObjectsOfType<GameObject>(true).Where(o => o.name.Equals("ErrorConfirmButton")).First().SetActive(true);
                    GlobalDataContainer.actionValid = false;
                    break;
                }
                GlobalDataContainer.actionValid = true;
                break;
            case "WPressureStart":
                GlobalDataContainer.actionValid = true;
                break;
        }
    }
}
