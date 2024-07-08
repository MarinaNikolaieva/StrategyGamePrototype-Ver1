using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System.ComponentModel;

public class MainMenuPanelOpener : MonoBehaviour
{
    public GlobalDataContainer UIControlData;
    public GeneratorGlobalData GeneratorGlobalData;

    GameObject CallerPanel;
    //Related = the panel to be opened, Current = the panel invoking the method
    public void PanelOpener(string panelNames)
    {
        string[] panelNamesSeparate = panelNames.Split(',');

        List<GameObject> candidateObjects = FindObjectsOfType<GameObject>(true).ToList();
        GameObject RelatedPanel = candidateObjects.Where(o => o.name.Equals(panelNamesSeparate[0])).FirstOrDefault();
        GameObject CurrentPanel = GameObject.Find(panelNamesSeparate[1]);

        if (RelatedPanel != null)
        {
            RelatedPanel.SetActive(true);
            if (RelatedPanel.name == "ErrorMessagePanel")
                RelatedPanel.transform.Find("AgreeButton").GetComponent<Button>().interactable = true;
        }
        if (CurrentPanel != null)
        {
            if (CurrentPanel.name != "ErrorMessagePanel")
                CallerPanel = CurrentPanel;
            CurrentPanel.SetActive(false);
        }

        if (UIControlData.pauseSuccessful && RelatedPanel != null && CurrentPanel != null && RelatedPanel.transform.parent.gameObject != CurrentPanel.transform.parent.gameObject)
        {
            if (!RelatedPanel.transform.parent.gameObject.activeSelf)
            {
                RelatedPanel.transform.parent.gameObject.SetActive(true);
            }
            if (CurrentPanel.transform.parent.gameObject.activeSelf)
            {
                CurrentPanel.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void PanelDrawback(string currentPanelName)
    {
        GameObject CurrentPanel = GameObject.Find(currentPanelName);
        if (CurrentPanel != null && CallerPanel != null)
        {
            CurrentPanel.SetActive(false);
            CallerPanel.SetActive(true);
        }
        if (CallerPanel != null && CurrentPanel != null && CallerPanel.transform.parent.gameObject != CurrentPanel.transform.parent.gameObject)
        {
            if (!CallerPanel.transform.parent.gameObject.activeSelf)
            {
                CallerPanel.transform.parent.gameObject.SetActive(true);
            }
            if (CurrentPanel.transform.parent.gameObject.activeSelf)
            {
                CurrentPanel.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void PanelStepBack(string errorPanelName)
    {
        GameObject ErrorPanel = GameObject.Find(errorPanelName);
        if (ErrorPanel != null)
            ErrorPanel.SetActive(false);

        string callerPanelName = CallerPanel.name;
        if (callerPanelName.Contains("Clone"))
            callerPanelName = callerPanelName.Substring(0, callerPanelName.Length - 7);

        List<GameObject> candidateObjects = FindObjectsOfType<GameObject>(false).ToList();
        GameObject panel = candidateObjects.Where(o => o.name.Contains("Panel") && !o.name.Equals(CallerPanel.name)).First();
        
        switch (panel.name)
        {
            case "SavePanel":
                UIControlData.isPreparingNext = false;
                UIControlData.isLoadingNext = false;
                break;
            case "LoadPanel":
                UIControlData.isPreparingNext = true;
                UIControlData.isLoadingNext = true;
                break;
            case "RestartWorldPanel":
                UIControlData.isPreparingNext = true;
                UIControlData.isLoadingNext = false;
                break;
            case "LaunchPreparePanel":
                UIControlData.isPreparingNext = false;
                UIControlData.isLoadingNext = false;
                break;
            default:
                UIControlData.isPreparingNext = false;
                UIControlData.isLoadingNext = false;
                break;
        }

        if (!UIControlData.isPreparingNext && !UIControlData.isLoadingNext)
        {
            if (panel != null && CallerPanel != null)
            {
                panel.SetActive(false);
                CallerPanel.SetActive(true);
            }
        }

        if (CallerPanel != null && panel != null && CallerPanel.transform.parent.gameObject != panel.transform.parent.gameObject)
        {
            if (!CallerPanel.transform.parent.gameObject.activeSelf)
            {
                CallerPanel.transform.parent.gameObject.SetActive(true);
            }
            if (panel.transform.parent.gameObject.activeSelf)
            {
                panel.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void LoadGeneratorScene()
    {
        GameObject Canvas = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("GeneratorCanvas")).FirstOrDefault();
        GameObject CurrentCanvas = GameObject.Find("MenuCanvas");

        if (Canvas != null && CurrentCanvas != null)
        {
            CurrentCanvas.SetActive(false);
            Canvas.SetActive(true);
        }
    }

    public void AllowLoading()
    {
        UIControlData.isLoadingNext = true;
    }

    public void LoadGameplayScene()
    {
        if (UIControlData.isLoadingNext && UIControlData.isPreparingNext)
        {
            GameObject Canvas = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("GameplayCanvas")).FirstOrDefault();
            GameObject CurrentCanvas = null;
            if (UIControlData.countryUnderControl == "")
                CurrentCanvas = GameObject.Find("LaunchPreparingCanvas");
            else
                CurrentCanvas = GameObject.Find("MenuCanvas");

            if (Canvas != null && CurrentCanvas != null)
            {
                if (UIControlData.countryUnderControl == "")
                {
                    Dropdown countryDropdown = GameObject.Find("CountrySelectDropdown").transform.GetComponent<Dropdown>();
                    string country = countryDropdown.options[countryDropdown.value].text;
                    UIControlData.countryUnderControl = country;
                    UIControlData.reInitContainer = true;
                }
                else
                    UIControlData.reInitContainer = false;

                CurrentCanvas.SetActive(false);
                Canvas.SetActive(true);
            }
        }
    }

    public void LoadSaveFiles(string CurrentPanelName)
    {
        GameObject CurrentPanel = GameObject.Find(CurrentPanelName);
        if (CurrentPanel == null) return;

        string temp = Application.streamingAssetsPath;
        string DataPath = temp + "/Saves";

        List<string> Files = new List<string>();
        var info = new DirectoryInfo(DataPath);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            string fileName = file.Name.Split('.')[0];
            if (file.Name.Split('.')[1] == "tsv")
                Files.Add(fileName);
        }
        Files = Files.Distinct().ToList();

        if (CurrentPanel.name.Equals("SavePanel"))
        {
            GameObject scrollViewContent = CurrentPanel.transform.Find("SavesScrollView").Find("Viewport").transform.Find("Content").gameObject;
            GameObject example = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("SaveTextExample")).FirstOrDefault();

            List<Transform> children = scrollViewContent.transform.GetComponentsInChildren<Transform>().ToList();
            foreach (Transform child in children)
            {
                if (child.name != "Content" && child.name != "SaveTextExample")
                    GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < Files.Count; i++)
            {
                GameObject textField = Instantiate(example);
                textField.transform.SetParent(scrollViewContent.transform, false);
                textField.SetActive(true);

                textField.GetComponent<Text>().text = Files.ElementAt(i);

                EventTrigger trigger = textField.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => OnTextFieldClickSaves(textField.GetComponent<Text>()));
                trigger.triggers.Add(entry);
            }
        }

        else if (CurrentPanel.name.Equals("LoadPanel"))
        {
            GameObject scrollViewContent = CurrentPanel.transform.Find("LoadsScrollView").Find("Viewport").transform.Find("Content").gameObject;
            GameObject example = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("LoadTextExample")).FirstOrDefault();

            List<Transform> children = scrollViewContent.transform.GetComponentsInChildren<Transform>().ToList();
            foreach (Transform child in children)
            {
                if (child.name != "Content" && child.name != "LoadTextExample")
                    GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < Files.Count; i++)
            {
                GameObject textField = Instantiate(example);
                textField.transform.SetParent(scrollViewContent.transform, false);
                textField.SetActive(true);

                textField.GetComponent<Text>().text = Files.ElementAt(i);

                EventTrigger trigger = textField.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => OnTextFieldClickLoads(textField.GetComponent<Text>()));
                trigger.triggers.Add(entry);
            }
        }

        else if (CurrentPanel.name.Equals("RestartWorldPanel"))
        {
            GameObject scrollViewContent = CurrentPanel.transform.Find("WorldsScrollView").Find("Viewport").transform.Find("Content").gameObject;
            GameObject example = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals("RestTextExample")).FirstOrDefault();

            List<Transform> children = scrollViewContent.transform.GetComponentsInChildren<Transform>().ToList();
            foreach (Transform child in children)
            {
                if (child.name != "Content" && child.name != "RestTextExample")
                    GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < Files.Count; i++)
            {
                GameObject textField = Instantiate(example);
                textField.transform.SetParent(scrollViewContent.transform, false);
                textField.SetActive(true);

                textField.GetComponent<Text>().text = Files.ElementAt(i);

                EventTrigger trigger = textField.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => OnTextFieldClickRestarts(textField.GetComponent<Text>()));
                trigger.triggers.Add(entry);
            }
        }
    }

    private void OnTextFieldClickSaves(Text clickedTextField)
    {
        // Change the color of the clicked Text field to red
        clickedTextField.color = Color.red;

        Color prevColor;
        ColorUtility.TryParseHtmlString("#FFBA00", out prevColor);

        GameObject CurrentPanel = GameObject.Find("SavePanel");
        GameObject scrollViewContent = CurrentPanel.transform.Find("SavesScrollView").Find("Viewport").transform.Find("Content").gameObject;

        Transform[] transformFields = scrollViewContent.GetComponentsInChildren<Transform>().Where(n => n.name == "SaveTextExample(Clone)").ToArray();
        List<Text> textFields = new List<Text>();
        foreach (Transform child in transformFields)
        {
            textFields.Add(child.GetComponent<Text>());
        }

        // Change the color of other Text fields to yellow
        foreach (Text textField in textFields)
        {
            if (textField != clickedTextField)
            {
                textField.color = prevColor;
            }
        }

        CurrentPanel.transform.Find("SaveNameInput").GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text = clickedTextField.text;
    }

    private void OnTextFieldClickLoads(Text clickedTextField)
    {
        // Change the color of the clicked Text field to red
        clickedTextField.color = Color.red;

        Color prevColor;
        ColorUtility.TryParseHtmlString("#FFBA00", out prevColor);

        GameObject CurrentPanel = GameObject.Find("LoadPanel");
        GameObject scrollViewContent = CurrentPanel.transform.Find("LoadsScrollView").Find("Viewport").transform.Find("Content").gameObject;

        Transform[] transformFields = scrollViewContent.GetComponentsInChildren<Transform>().Where(n => n.name == "LoadTextExample(Clone)").ToArray();
        List<Text> textFields = new List<Text>();
        foreach (Transform child in transformFields)
        {
            textFields.Add(child.GetComponent<Text>());
        }

        // Change the color of other Text fields to yellow
        foreach (Text textField in textFields)
        {
            if (textField != clickedTextField)
            {
                textField.color = prevColor;
            }
        }

        CurrentPanel.transform.Find("LoadNameInput").GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text = clickedTextField.text;
    }

    private void OnTextFieldClickRestarts(Text clickedTextField)
    {
        // Change the color of the clicked Text field to red
        clickedTextField.color = Color.red;

        Color prevColor;
        ColorUtility.TryParseHtmlString("#FFBA00", out prevColor);

        GameObject CurrentPanel = GameObject.Find("RestartWorldPanel");
        GameObject scrollViewContent = CurrentPanel.transform.Find("WorldsScrollView").Find("Viewport").transform.Find("Content").gameObject;

        Transform[] transformFields = scrollViewContent.GetComponentsInChildren<Transform>().Where(n => n.name == "RestTextExample(Clone)").ToArray();
        List<Text> textFields = new List<Text>();
        foreach (Transform child in transformFields)
        {
            textFields.Add(child.GetComponent<Text>());
        }

        // Change the color of other Text fields to yellow
        foreach (Text textField in textFields)
        {
            if (textField != clickedTextField)
            {
                textField.color = prevColor;
            }
        }

        CurrentPanel.transform.Find("RestartNameInput").GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text = clickedTextField.text;
    }
}
