using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PreparingPanelOpener : MonoBehaviour
{
    public GlobalDataContainer UIControlData;

    GameObject CallerPanel;
    GeneratorGlobalData GeneratorGlobalData;
    //Related = the panel to be opened, Current = the panel invoking the method
    public void PanelOpenFromGenerator(GeneratorGlobalData globalData)
    {
        GeneratorGlobalData = globalData;

        List<GameObject> candidateObjects = FindObjectsOfType<GameObject>(true).ToList();
        GameObject RelatedPanel = candidateObjects.Where(o => o.name.Equals("LaunchPreparingCanvas")).FirstOrDefault();
        GameObject CurrentPanel = GameObject.Find("GeneratorCanvas");
        GameObject ErrorPanel = candidateObjects.Where(o => o.name.Equals("GenErrorMessagePanel")).FirstOrDefault();

        if (!GeneratorGlobalData.CheckFullGeneration() && ErrorPanel != null)
        {
            string errorLine = "ERROR!\nGenerate all maps first!";
            ErrorPanel.SetActive(true);
            ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text = errorLine;
            return;
        }
        if (RelatedPanel != null)
        {
            RelatedPanel.SetActive(true);
            GameObject.Find("PrepGenMap").GetComponent<RawImage>().texture = GeneratorGlobalData.MapTextures["Political Map"];
            GameObject.Find("PrepGenMap").GetComponent<RawImage>().color = Color.white;

            List<string> countries = GeneratorGlobalData.countryReader.GetCountries().Select(c => c.Name).ToList();
            Dropdown dropdown = GameObject.Find("CountrySelectDropdown").GetComponent<Dropdown>();
            for (int i = 0; i < GeneratorGlobalData.numOfCountries; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = countries.ElementAt(i) });
            }
            dropdown.value = 0;
            dropdown.RefreshShownValue();
        }
        if (CurrentPanel != null)
        {
            CallerPanel = CurrentPanel;
            CurrentPanel.SetActive(false);
        }
    }

    public void PanelOpenFromMenu(GeneratorGlobalData globalData)
    {
        if (!UIControlData.isPreparingNext)
        {
            UIControlData.isPreparingNext = true;
            return;
        }

        GeneratorGlobalData = globalData;

        List<GameObject> candidateObjects = FindObjectsOfType<GameObject>(true).ToList();
        GameObject RelatedPanel = candidateObjects.Where(o => o.name.Equals("LaunchPreparingCanvas")).FirstOrDefault();
        GameObject CurrentPanel = GameObject.Find("MenuCanvas");
        GameObject ErrorPanel = candidateObjects.Where(o => o.name.Equals("ErrorMessagePanel")).FirstOrDefault();

        bool fromLoad = candidateObjects.Where(o => o.name.Equals("RestartWorldPanel")).FirstOrDefault().activeInHierarchy;

        if (fromLoad && UIControlData.isLoadSuccessful)
        {
            if (!GeneratorGlobalData.CheckFullGeneration() && ErrorPanel != null)
            {
                string errorLine = "ERROR!\nAn error occured while reading the maps!\nIt looks like some maps have errors in them!\nPlease select a different world";
                ErrorPanel.SetActive(true);
                ErrorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text = errorLine;
                return;
            }
            if (RelatedPanel != null)
            {
                RelatedPanel.SetActive(true);

                GameObject.Find("PrepGenMap").GetComponent<RawImage>().texture = GeneratorGlobalData.MapTextures["Political Map"];
                GameObject.Find("PrepGenMap").GetComponent<RawImage>().color = Color.white;
            }
            if (CurrentPanel != null)
            {
                GameObject restartPanel = GameObject.Find("RestartWorldPanel");
                if (restartPanel != null)
                {
                    restartPanel.SetActive(false);
                }
                CallerPanel = CurrentPanel;
                CurrentPanel.SetActive(false);
            }
        }
    }

    public void PanelDrawback(string currentPanelName)
    {
        GameObject CurrentPanel = GameObject.Find(currentPanelName);
        //GameObject RelatedPanel = GameObject.Find(callerPanelName);
        if (CurrentPanel != null && CallerPanel != null)
        {
            CurrentPanel.SetActive(false);
            CallerPanel.SetActive(true);
        }
    }
}
