using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UtilityClasses;

public class ConfigFiller : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject panelTemplate;
    public GameObject errorPanel;

    public GeneratorGlobalData GeneratorGlobalData;  //Needed here for getting the data and sending it there

    public void DataFill(List<object> data, List<Tuple<bool, float>> coefs)
    {        
        int counter = -1;
        //Here, I expect Underground resources
        //By default, they all are present and their coefs are given in the generator data
        //Here is where we can change the values
        for (int i = -1; i < data.Count; i++)
        {
            GameObject panel = (GameObject)Instantiate(panelTemplate);
            panel.transform.SetParent(scrollViewContent.transform, false);
            foreach (Transform child in panel.transform)
            {
                switch (child.gameObject.name)
                {
                    case "NameText":
                        if (counter == -1)
                            child.GetComponent<Text>().text = "Resource name";
                        else
                            child.GetComponent<Text>().text = (data[i] as BasicResource).Name;
                        break;
                    case "CategoryText":
                        if (counter == -1)
                            child.GetComponent<Text>().text = "Category";
                        else
                            child.GetComponent<Text>().text = (data[i] as BasicResource).Category;
                        break;
                    case "ResConfigInput":
                        if (counter == -1)
                            child.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text = "Coefficient";
                        else
                        {
                            child.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text = coefs.ElementAt(i).Item2.ToString();
                        }
                        break;
                    case "CheckBox":
                        if (counter == -1)
                        {
                            child.transform.Find("Label").GetComponentInChildren<Text>().text = "Is present?";
                            child.GetComponent<Toggle>().isOn = true;
                            child.GetComponent<Toggle>().interactable = false;
                        }
                        else
                        {
                            child.transform.Find("Label").GetComponentInChildren<Text>().text = "";
                            child.GetComponent<Toggle>().isOn = coefs.ElementAt(i).Item1;
                        }
                        break;
                }
            }
            counter++;
        }
    }

    private bool CheckData()
    {
        //Check if all the coefs are between 0 and 1 and that all categories are present
        bool errorPresent = false;
        string errorLine = "";

        List<string> categories = new List<string>();
        List<bool> presentCategories = new List<bool>();

        Transform[] children = scrollViewContent.GetComponentsInChildren<Transform>().Where(n => n.name == "ResConfigTableElement(Clone)").ToArray();
        for (int i = 1; i < children.Count(); i++)
        {
            Transform child = children[i];
            string category = "";
            foreach (Transform childIn in child)
            {
                switch (childIn.gameObject.name)
                {
                    case "CategoryText":
                        category = childIn.GetComponent<Text>().text;
                        if (!categories.Contains(childIn.GetComponent<Text>().text))
                            categories.Add(category);
                        if (presentCategories.Count < categories.Count)
                            presentCategories.Add(false);
                        break;
                    case "ResConfigInput":
                        float value = 0.0F;
                        if (childIn.GetComponent<InputField>().text == "" && !float.TryParse(childIn.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text, out value))
                        {
                            errorLine = "ERROR!\nAll coefficients must be floating-point numbers!";
                            errorPresent = true;
                        }
                        else if (childIn.GetComponent<InputField>().text != "" && !float.TryParse(childIn.GetComponent<InputField>().text, out value))
                        {
                            errorLine = "ERROR!\nAll coefficients must be floating-point numbers!";
                            errorPresent = true;
                        }
                        else if (value < 0 || value > 1)
                        {
                            errorLine = "ERROR!\nAll coefficients must be in between 0 and 1 (included)!";
                            errorPresent = true;
                        }
                        break;
                    case "CheckBox":
                        bool check = childIn.GetComponent<Toggle>().isOn;
                        int index = categories.IndexOf(category);
                        if (!presentCategories.ElementAt(index) && check)
                            presentCategories[index] = check;
                        break;
                }
            }
        }

        for (int i = 0; i < presentCategories.Count; i++)
        {
            if (!presentCategories[i])
            {
                errorLine = "ERROR!\nAt least one category has no selected resources!";
                errorPresent = true;
                break;
            }
        }

        if (errorPresent)
        {
            errorPanel.SetActive(true);
            errorPanel.transform.Find("ErrorMessage").gameObject.GetComponentInChildren<Text>().text = errorLine;
            GeneratorGlobalData.ResConfigSuccess = false;
            return false;
        }
        return true;
    }

    public void SendResourcesConfig()
    {
        if (CheckData())
        {
            List<Tuple<bool, float>> results = new List<Tuple<bool, float>>();

            Transform[] children = scrollViewContent.GetComponentsInChildren<Transform>().Where(n => n.name == "ResConfigTableElement(Clone)").ToArray();
            for (int i = 1; i < children.Count(); i++)
            {
                Transform child = children[i];
                float val = 0.0F;
                bool presence = true;
                foreach (Transform childIn in child)
                {
                    switch (childIn.gameObject.name)
                    {
                        case "ResConfigInput":
                            if (childIn.GetComponent<InputField>().text != "")
                                float.TryParse(childIn.GetComponent<InputField>().text, out val);
                            else
                                float.TryParse(childIn.GetComponent<InputField>().placeholder.GetComponentInChildren<Text>().text, out val);
                            break;
                        case "CheckBox":
                            presence = childIn.GetComponent<Toggle>().isOn;
                            break;
                    }
                }
                results.Add(new Tuple<bool, float>(presence, val));
            }

            GeneratorGlobalData.ResConfigSuccess = true;
            GeneratorGlobalData.SetUndergroundCoefs(results);
        }
    }
}
