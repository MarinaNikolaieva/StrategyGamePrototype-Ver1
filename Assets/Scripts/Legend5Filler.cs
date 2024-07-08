using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilityClasses;

public class Legend5Filler : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject panelTemplate;

    public void DataFill(List<object> data, List<object> additionalData)
    {
        //Here, I expect the Height and Biome tables
        string objType = data[0].GetType().Name;
        int counter = -1;
        switch (objType)
        {
            case "Biome":
                for (int i = -1; i < data.Count; i++)
                {
                    GameObject panel = (GameObject)Instantiate(panelTemplate);
                    panel.transform.SetParent(scrollViewContent.transform, false);
                    foreach (Transform child in panel.transform)
                    {
                        switch (child.gameObject.name) {
                            case "ColorImage":
                                if (counter != -1)
                                {
                                    Color color = (data[i] as Biome).getColor();
                                    child.GetComponent<Image>().color = color;
                                }
                                break;

                            case "NameText":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Biome name";
                                else
                                    child.GetComponent<Text>().text = (data[i] as Biome).getName();
                                break;

                            case "AdditTextScrollView1":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "Temperatures";
                                else
                                {
                                    StringBuilder build = new StringBuilder();
                                    for (int j = 0; j < (data[i] as Biome).getTemperatures().Count; j++)
                                    {
                                        build.Append((data[i] as Biome).getTemperatures()[j].Name + ", ");
                                    }
                                    string temper = build.ToString();
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = temper;
                                }
                                break;

                            case "AdditTextScrollView2":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "Moistures";
                                else
                                {
                                    StringBuilder build = new StringBuilder();
                                    for (int j = 0; j < (data[i] as Biome).getMoistures().Count; j++)
                                    {
                                        build.Append((data[i] as Biome).getMoistures()[j].Name + ", ");
                                    }
                                    string moist = build.ToString();
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = moist;
                                }
                                break;

                            case "AdditionalText3":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Minimum heights";
                                else
                                {
                                    StringBuilder build = new StringBuilder();
                                    for (int j = 0; j < (data[i] as Biome).getHeights().Count; j++)
                                    {
                                        build.Append((data[i] as Biome).getHeights()[j] + ", ");
                                    }
                                    string heightVals = build.ToString();
                                    child.GetComponent<Text>().text = heightVals;
                                }
                                break;
                        }
                    }
                    counter++;
                }
                break;

            case "HeightObject":
                for (int i = -1; i < data.Count; i++)
                {
                    GameObject panel = (GameObject)Instantiate(panelTemplate);
                    panel.transform.SetParent(scrollViewContent.transform, false);
                    foreach (Transform childIn in panel.transform)
                    {
                        GameObject child = childIn.gameObject;
                        switch (child.name) {
                            case "ColorImage":
                                if (counter != -1)
                                {
                                    Color color = (data[i] as HeightObject).Color;
                                    child.GetComponent<Image>().color = color;
                                }
                                break;

                            case "NameText":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Height group name";
                                else
                                    child.GetComponent<Text>().text = (data[i] as HeightObject).Name;
                                break;

                            case "AdditTextScrollView1":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "Minimum Height Value";
                                else
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = ((int)additionalData[i]).ToString();
                                break;

                            case "AdditTextScrollView2":
                                if (counter == -1)
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = "Temperature Impact Coef";
                                else
                                    child.transform.Find("Viewport").Find("Content").GetComponentInChildren<Text>().text = (data[i] as HeightObject).TemperatureCoeffitient.ToString();
                                break;

                            case "AdditionalText3":
                                if (counter == -1)
                                    child.GetComponent<Text>().text = "Moisture Impact Coef";
                                else
                                    child.GetComponent<Text>().text = (data[i] as HeightObject).MoistureCoeffitient.ToString();
                                break;
                        }
                    }
                    counter++;
                }
                break;
        }
    }
}
