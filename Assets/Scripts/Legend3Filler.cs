using GlobalClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilityClasses;

public class Legend3Filler : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject panelTemplate;

    public void DataFill(List<object> data, List<object> additionalData)
    {
        //I expect Temperatures, Moistures, Soils, Global Res, OnGround Res, Underground Res, Politics
        string objType = data[0].GetType().Name;
        int counter = -1;
        switch (objType)
        {
            case "TemperatureObject":
                for (int i = -1; i < data.Count; i++)
                {
                    GameObject panel = (GameObject)Instantiate(panelTemplate);
                    panel.transform.SetParent(scrollViewContent.transform, false);
                    foreach (Transform child in panel.transform)
                    {
                        switch (child.gameObject.name)
                        {
                            case "ColorImage":
                                if (counter != -1)
                                {
                                    Color color = (data[i] as TemperatureObject).Color;
                                    child.GetComponent<Image>().color = color;
                                }
                                break;
                            case "NameText":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Temperature group name";
                                else
                                    child.GetComponent<Text>().text = (data[i] as TemperatureObject).Name;
                                break;
                            case "AdditTextScrollView":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "No additional info";
                                else
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "";
                                break;
                        }
                    }
                    counter++;
                }
                break;

            case "MoistureObject":
                for (int i = -1; i < data.Count; i++)
                {
                    GameObject panel = (GameObject)Instantiate(panelTemplate);
                    panel.transform.SetParent(scrollViewContent.transform, false);
                    foreach (Transform child in panel.transform)
                    {
                        switch (child.gameObject.name)
                        {
                            case "ColorImage":
                                if (counter != -1)
                                {
                                    Color color = (data[i] as MoistureObject).Color;
                                    child.GetComponent<Image>().color = color;
                                }
                                break;
                            case "NameText":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Moisture group name";
                                else
                                    child.GetComponent<Text>().text = (data[i] as MoistureObject).Name;
                                break;
                            case "AdditTextScrollView":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "No additional info";
                                else
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "";
                                break;
                        }
                    }
                    counter++;
                }
                break;

            case "Soil":
                //Here we have the additional data - biomes the soils appear in
                for (int i = -1; i < data.Count; i++)
                {
                    GameObject panel = (GameObject)Instantiate(panelTemplate);
                    panel.transform.SetParent(scrollViewContent.transform, false);
                    foreach (Transform child in panel.transform)
                    {
                        switch (child.gameObject.name)
                        {
                            case "ColorImage":
                                if (counter != -1)
                                {
                                    Color color = (data[i] as Soil).Color;
                                    child.GetComponent<Image>().color = color;
                                }
                                break;
                            case "NameText":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Soil name";
                                else
                                    child.GetComponent<Text>().text = (data[i] as Soil).Name;
                                break;
                            case "AdditTextScrollView":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "Biomes";
                                else
                                {
                                    StringBuilder build = new StringBuilder();
                                    for (int j = 0; j < additionalData.Count; j++)
                                    {
                                        if ((additionalData.ElementAt(j) as Biome).getSoils().Select(s => s.Name).Contains((data[i] as Soil).Name))
                                            build.Append((additionalData.ElementAt(j) as Biome).getName() + ", ");
                                    }
                                    string biomes = build.ToString();
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = biomes;
                                }
                                break;
                        }
                    }
                    counter++;
                }
                break;

            case "BasicResource":
                //For these, the additional data is the single-element list specifying the category
                bool keepCategory = true;
                string category = additionalData[0].ToString();
                if (category != "G" && category != "O")
                    keepCategory = false;
                GameObject panelDef = (GameObject)Instantiate(panelTemplate);
                panelDef.transform.SetParent(scrollViewContent.transform, false);
                foreach (Transform child in panelDef.transform)
                {
                    switch (child.gameObject.name)
                    {
                        case "ColorImage":
                            //Empty for now
                            break;
                        case "NameText":
                            child.GetComponent<Text>().text = "Resource name";
                            break;
                        case "AdditTextScrollView":
                            child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "Category";
                            break;
                    }
                }

                for (int i = 0; i < data.Count; i++)
                {
                    if (keepCategory && (data.ElementAt(i) as BasicResource).Category.Equals(category)) {
                        GameObject panel = (GameObject)Instantiate(panelTemplate);
                        panel.transform.SetParent(scrollViewContent.transform, false);
                        foreach (Transform child in panel.transform)
                        {
                            switch (child.gameObject.name)
                            {
                                case "ColorImage":
                                    Color color = (data[i] as BasicResource).Color;
                                    child.GetComponent<Image>().color = color;
                                    break;
                                case "NameText":
                                    child.GetComponent<Text>().text = (data[i] as BasicResource).Name;
                                    break;
                                case "AdditTextScrollView":
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = category;
                                    break;
                            }
                        }
                        counter++;
                    }
                    else if (!keepCategory && (data.ElementAt(i) as BasicResource).Category.Contains(category))
                    {
                        GameObject panel = (GameObject)Instantiate(panelTemplate);
                        panel.transform.SetParent(scrollViewContent.transform, false);
                        foreach (Transform child in panel.transform)
                        {
                            switch (child.gameObject.name)
                            {
                                case "ColorImage":
                                    Color color = (data[i] as BasicResource).Color;
                                    child.GetComponent<Image>().color = color;
                                    break;
                                case "NameText":
                                    child.GetComponent<Text>().text = (data[i] as BasicResource).Name;
                                    break;
                                case "AdditTextScrollView":
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = (data[i] as BasicResource).Category;
                                    break;
                            }
                        }
                        counter++;
                    }
                }
                break;

            case "CountryBase":
                for (int i = -1; i < data.Count; i++)
                {
                    GameObject panel = (GameObject)Instantiate(panelTemplate);
                    panel.transform.SetParent(scrollViewContent.transform, false);
                    foreach (Transform child in panel.transform)
                    {
                        switch (child.gameObject.name)
                        {
                            case "ColorImage":
                                if (counter != -1)
                                {
                                    Color color = (data[i] as CountryBase).Color;
                                    child.GetComponent<Image>().color = color;
                                }
                                break;
                            case "NameText":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Country name";
                                else
                                    child.GetComponent<Text>().text = (data[i] as CountryBase).Name;
                                break;
                            case "AdditTextScrollView":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "No additional info";
                                else
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "";
                                break;
                        }
                    }
                    counter++;
                }
                break;
        }
    }
}
