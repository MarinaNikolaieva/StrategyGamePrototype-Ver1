using EconomicsClasses;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilityClasses;
using GlobalClasses;
using System.Linq;
using LogisticsClasses;
using System.Text;
using WarfareClasses;
using WarfareMechanics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using System.ComponentModel;
using System.Xml.Linq;

[CreateAssetMenu(menuName = "GlobalDataController")]
public class GlobalDataController : ScriptableObject
{
    GlobalDataContainer Container;

    public void InitDataContainer(GlobalDataContainer container)
    {
        Container = container;
    }

    public void ActionPanelClose()
    {
        string actionPanelName = "ActionPanel";
        GameObject actionPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    public void LayeredActionPanelClose()
    {
        string actionPanelName = "ActionPanelLayer";
        GameObject actionPanel = FindObjectsOfType<GameObject>(true).ToList().Where(o => o.name.Equals(actionPanelName)).FirstOrDefault();
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    public void ErrorPanelClose(Image errorPanel)
    {
        if (errorPanel != null)
            errorPanel.gameObject.SetActive(false);
    }

    private Dictionary<IResource, int> GetAllResourcesAroundSelectedPoint(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        Dictionary<IResource, int> results = new Dictionary<IResource, int>();
        try
        {
            //3x3 square
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!Container.ComplexMap[(int)coords.x + j, (int)coords.y + i].Country.Name.Equals(countryName))
                        throw new Exception();

                    List<IResource> res = Container.ComplexMap[(int)coords.x + j, (int)coords.y + i].Resources;
                    for (int k = 0; k < res.Count; k++)
                    {
                        if (results.ContainsKey(res[k]))
                            results[res[k]]++;
                        else
                        {
                            results.Add(res[k], 1);
                        }
                    }
                }
            }
        }
        catch (Exception ex) { }
        return results;
    }

    //This CAN'T be called from neutral territory!
    public List<string> GetIndustriesByResources(Vector2 coords)
    {
        Dictionary<IResource, int> resources = GetAllResourcesAroundSelectedPoint(coords);
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<string> results = new List<string>();
        foreach (Industry industry in country.industriesPoints)
        {
            bool isEnoughRequired = true;
            bool isEnoughNeeded = true;
            for (int i = 0; i < industry.GetRequired().Count; i++)
            {
                if (industry.GetRequired().ElementAt(i).Key is Industry)
                {
                    Industry ind = country.industriesPoints.Where(indu => indu.Equals(industry.GetRequired().ElementAt(i).Key)).First();
                    if (ind.Points < industry.GetRequired().ElementAt(i).Value)
                    {
                        isEnoughRequired = false;
                        break;
                    }
                }
                else if (!resources.ContainsKey(industry.GetRequired().ElementAt(i).Key) || resources[industry.GetRequired().ElementAt(i).Key] < industry.GetRequired().ElementAt(i).Value)
                {
                    isEnoughRequired = false;
                    break;
                }
            }

            int neededNum = industry.GetNeeded().Count;
            for (int i = 0; i < industry.GetNeeded().Count; i++)
            {
                if (industry.GetNeeded().ElementAt(i).Key is Industry)
                {
                    Industry ind = country.industriesPoints.Where(indu => indu.Equals(industry.GetNeeded().ElementAt(i).Key)).First();
                    if (ind.Points < industry.GetNeeded().ElementAt(i).Value)
                    {
                        isEnoughRequired = false;
                        break;
                    }
                }
                else if (!resources.ContainsKey(industry.GetNeeded().ElementAt(i).Key) || resources[industry.GetNeeded().ElementAt(i).Key] < industry.GetNeeded().ElementAt(i).Value)
                {
                    if (industry.Grade != 1)
                    {
                        isEnoughNeeded = false;
                        break;
                    }
                    else
                        neededNum--;
                }
            }
            if (industry.Grade == 1 && neededNum == 0)
                isEnoughNeeded = false;

            if (isEnoughRequired && isEnoughNeeded)
                results.Add(industry.Name);
        }
        return results;
    }

    //This CAN'T be called from neutral territory!
    public object GetObjectOnCoords(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return obj;
    }

    public object HasObjectInPlace(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return obj;
    }

    public object HasObjectNearby(Vector2 coords)
    {
        for (int i = (int)coords.x - 1; i < (int)coords.x + 2; i++)
        {
            for (int j = (int)coords.y - 1; j < (int)coords.y + 2; j++)
            {
                try
                {
                    string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
                    CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
                    object obj = country.IsObjectInPlace(new Vector2(i, j));
                    if (obj != null)
                    {
                        return obj;
                    }
                }
                catch (Exception ex) { }
            }
        }
        return null;
    }

    public int GetStorageCapacity(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return (obj as Storage).GetCapacity();
    }

    public int GetStorageFreeCapacity(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return (obj as Storage).GetFreeCapacity();
    }

    public int GetStorageOccupiedCapacity(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return (obj as Storage).GetCapacity() - (obj as Storage).GetFreeCapacity();
    }

    //NEEDED Fuse these two function blocks together!
    public int GetContainerCapacity(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return (obj as ContainerObject).Capacity;
    }

    public int GetContainerFreeCapacity(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return (obj as ContainerObject).CurrentCapacity;
    }

    public int GetContainerOccupiedCapacity(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return (obj as ContainerObject).Capacity - (obj as ContainerObject).CurrentCapacity;
    }

    public List<Product> GetProductsOfFacility(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = country.IsObjectInPlace(coords);
        return country.GetProductsOfFacility((obj as Facility).ID);
    }

    public List<string> GetProductsOfIndustry(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = HasObjectInPlace(Container.selectedCoordinates);
        return country.GetProductsOfIndustry((obj as Facility).parentIndustry.Name);
    }

    public List<string> GetProductOfFacility(Vector2 coords, string ProductName)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = HasObjectInPlace(Container.selectedCoordinates);
        List<string> info = new List<string>();
        Product prod = (obj as Facility).GetProduction().Where(p => p.Key.Name.Equals(ProductName)).FirstOrDefault().Key;
        int power = (obj as Facility).GetProduction().Where(p => p.Key.Name.Equals(ProductName)).FirstOrDefault().Value;

        info.Add("Product: " + prod.Name);
        info.Add("ID: " + prod.ID);
        info.Add("Price: " + prod.Price.ToString());
        if (prod.OriginalResource == null)
            info.Add("Original resouce: Multiple");
        else
            info.Add("Original resource: " + prod.OriginalResource.Name);
        info.Add("Production power within this facility: " + power.ToString());
        return info;
    }

    //Some products have the same name, but different resources to make them
    public List<string> GetResourcesForProductName(Vector2 coords, string productName, string industryName)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName && pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
        List<string> resources = new List<string>();
        foreach (Product product in products)
            if (product.OriginalResource == null)
                resources.Add("Multiple");
            else resources.Add(product.OriginalResource.Name);
        return resources;
    }

    public List<string> FilterResourcesByPlace(Vector2 coords, List<string> resourceNames)
    {
        if (resourceNames[0] == "Multiple")
            return resourceNames;
        List<string> filter = GetAllResourcesAroundSelectedPoint(coords).Select(c => c.Key.Name).ToList();
        List<string> results = resourceNames.Intersect(filter).ToList();
        return results;
    }

    public List<string> GetProductsInStorage(Vector2 coords)
    {
        Storage storage = HasObjectInPlace(coords) as Storage;
        Dictionary<Product, int> products = storage.GetProducts();
        List<string> results = new List<string>();
        foreach (var product in products)
        {
            StringBuilder build = new StringBuilder();
            build.Append(product.Key.Name + " ");
            if (product.Key.OriginalResource == null)
                build.Append("(Multiple) - ");
            else build.Append("(" + product.Key.OriginalResource.Name + ") - ");
            build.Append(product.Value.ToString());
            results.Add(build.ToString());
        }
        return results;
    }

    public List<string> GetProductionOfFacility(Vector2 coords)
    {
        Facility facility = HasObjectInPlace(coords) as Facility;
        Dictionary<Product, int> products = facility.GetProduction();
        List<string> results = new List<string>();
        foreach (var product in products)
        {
            StringBuilder build = new StringBuilder();
            build.Append(product.Key.Name + " ");
            if (product.Key.OriginalResource == null)
                build.Append("(Multiple) - x ");
            else build.Append("(" + product.Key.OriginalResource.Name + ") - x ");
            build.Append(product.Value.ToString());
            results.Add(build.ToString());
        }
        return results;
    }

    public int GetFacilityTotalProduction(Vector2 coords)
    {
        Facility facility = HasObjectInPlace(coords) as Facility;
        int res = facility.GetTotalProductionPoints();
        return res;
    }

    //NEEDED Add filtering of how many products are NOT IN ANOTHER storage yet!
    //For now, it will be the filter by just ONE CURRENT storage
    //DONE
    public List<string> GetAllProductsNotInStorage(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        Dictionary<Product, int> products = country.productAmounts;
        List<string> results = new List<string>();

        foreach (var product in products)
        {
            int value = 0;
            StringBuilder build = new StringBuilder();
            build.Append(product.Key.Name + " ");
            if (product.Key.OriginalResource == null)
                build.Append("(Multiple) - ");
            else build.Append("(" + product.Key.OriginalResource.Name + ") - ");
            value = product.Value;

            foreach(Storage storage in country.storages)
            {
                if (storage.GetProducts().ContainsKey(product.Key))
                    value -= storage.GetProducts()[product.Key];
            }

            if (value > 0)
            {
                build.Append(value.ToString());
                results.Add(build.ToString());
            }
        }
        return results;
    }

    public List<string> GetUnitsInBank(Vector2 coords)
    {
        string countryName;
        if (coords == new Vector2(-1, -1))
            countryName = Container.countryUnderControl;
        else
            countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        Dictionary<IObject, int> units = country.objectBank.Bank.Where(o => o.Key.Category != "S").ToDictionary(dict => dict.Key, dict => dict.Value);
        List<string> results = new List<string>();

        foreach (var unit in units)
        {
            StringBuilder build = new StringBuilder();
            build.Append(unit.Key.Name + " - ");
            build.Append(unit.Value.ToString());
            results.Add(build.ToString());
        }
        return results;
    }

    public List<string> GetStationaryObjects(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        Dictionary<IObject, int> units = country.objectBank.Bank.Where(o => o.Key.Category == "S").ToDictionary(dict => dict.Key, dict => dict.Value);
        List<string> results = new List<string>();

        foreach (var unit in units)
        {
            StringBuilder build = new StringBuilder();
            build.Append(unit.Key.Name);
            results.Add(build.ToString());
        }
        return results;
    }

    public List<string> GetUnitsInContainer(Vector2 coords)
    {
        ContainerObject container = HasObjectInPlace(coords) as ContainerObject;
        List<string> results = new List<string>();

        switch (container.GetType().Name)
        {
            case "Hospital":
                foreach (var unit in (container as Hospital).Fixed)
                {
                    StringBuilder build = new StringBuilder();
                    build.Append(unit.Key.Name + " in state ");
                    build.Append(unit.Value.Item2.ToString("0.####") + " - ");
                    build.Append(unit.Value.Item3.ToString());
                    results.Add(build.ToString());
                }
                return results;
            case "Headquaters":
                foreach (var unit in (container as Headquaters).Contained)
                {
                    StringBuilder build = new StringBuilder();
                    build.Append(unit.Key.Name + " in state ");
                    build.Append(unit.Value.Item2.ToString("0.####") + " - ");
                    build.Append(unit.Value.Item3.ToString());
                    results.Add(build.ToString());
                }
                return results;
            case "Workshop":
                foreach (var unit in (container as Workshop).Fixed)
                {
                    StringBuilder build = new StringBuilder();
                    build.Append(unit.Key.Name + " in state ");
                    build.Append(unit.Value.Item2.ToString("0.####") + " - ");
                    build.Append(unit.Value.Item3.ToString());
                    results.Add(build.ToString());
                }
                return results;
            default:
                return results;
        }
    }

    public string GetUnitCategory(string unitName)
    {
        CountryBase country = Container.countries.Where(c => c.Name.Equals(Container.countryUnderControl)).FirstOrDefault();
        return country.objectBank.Bank.Where(u => u.Key.Name.Equals(unitName)).FirstOrDefault().Key.Category;
    }

    public List<string> GetUnitsOnBattlePosition(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

        BattlePosition position = country.battlePositions.Where(p => p.Position.Equals(coords)).First();

        List<string> results = new List<string>();

        foreach (var unit in position.HumanAndTechResources)
        {
            StringBuilder build = new StringBuilder();
            build.Append(unit.Key.Name + " in state ");
            build.Append(unit.Value.Item2.ToString("0.####") + " - ");
            build.Append(unit.Value.Item3.ToString());
            results.Add(build.ToString());
        }
        foreach (var unit in position.StationaryPlaces)
        {
            StringBuilder build = new StringBuilder();
            build.Append(unit.Key.Name + " in state ");
            build.Append(unit.Value.Item2.ToString("0.####") + " - ");
            build.Append(unit.Value.Item3.ToString());
            results.Add(build.ToString());
        }
        foreach (var unit in position.TempOutOfActionObjects)
        {
            StringBuilder build = new StringBuilder();
            build.Append(unit.Key.Name + " in state ");
            build.Append(unit.Value.Item2.ToString("0.####") + " - ");
            build.Append(unit.Value.Item3.ToString());
            results.Add(build.ToString());
        }
        return results;
    }

    public List<string> GetDefectiveUnitsOnBattlePosition(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

        BattlePosition position = country.battlePositions.Where(p => p.Position.Equals(coords)).First();

        List<string> results = new List<string>();

        foreach (var unit in position.TempOutOfActionObjects)
        {
            StringBuilder build = new StringBuilder();
            build.Append(unit.Key.Name + " in state ");
            build.Append(unit.Value.Item2.ToString("0.####") + " - ");
            build.Append(unit.Value.Item3.ToString());
            results.Add(build.ToString());
        }
        return results;
    }

    public List<string> GetWarfareObjectsInfo(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<string> results = new List<string>();
        foreach (var unit in country.warfareContainerObjects)
        {
            StringBuilder build = new StringBuilder();
            build.Append(unit.CustomName + " ");
            switch (unit.GetType().Name)
            {
                case "Hospital":
                    build.Append("(Hospital) - X ");
                    break;
                case "Headquaters":
                    build.Append("(Headquaters) - X ");
                    break;
                case "Workshop":
                    build.Append("(Workshop) - X ");
                    break;
                case "TrainingPlace":
                    build.Append("(Training_Field) - X ");
                    break;
            }
            build.Append(unit.Coords.x + " , Y " + unit.Coords.y);
            results.Add(build.ToString());
        }
        foreach (var unit in country.battlePositions)
        {
            StringBuilder build = new StringBuilder();
            build.Append("Position (Battle_position) - X ");
            build.Append(unit.Position.x + " , Y " + unit.Position.y);
            results.Add(build.ToString());
        }
        return results;
    }

    //FOR PLAYER ONLY!
    public List<string> GetAllResourcesAmounts()
    {
        string countryName = Container.countryUnderControl;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        Dictionary<IResource, int> resources = country.resourceAmmounts;
        List<string> results = new List<string>();

        foreach (var resource in resources)
        {
            StringBuilder build = new StringBuilder();
            build.Append(resource.Key.Name + " - ");
            build.Append(resource.Value);
            results.Add(build.ToString());
        }
        return results;
    }

    //FOR PLAYER ONLY!
    public List<string> GetAllProductsAvailable()
    {
        string countryName = Container.countryUnderControl;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        Dictionary<Product, int> products = country.productAmounts;
        List<string> results = new List<string>();

        foreach (var product in products)
        {
            StringBuilder build = new StringBuilder();
            build.Append(product.Key.Name + " ");
            if (product.Key.OriginalResource == null)
                build.Append("(Multiple) - ");
            else build.Append("(" + product.Key.OriginalResource.Name + ") - ");
            build.Append(product.Value.ToString());
            results.Add(build.ToString());
        }
        return results;
    }

    //FOR PLAYER ONLY!
    public List<string> GetAllIndustriesDevelopment()
    {
        string countryName = Container.countryUnderControl;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<Industry> industries = country.industriesPoints;
        List<string> results = new List<string>();

        foreach (var industry in industries)
        {
            StringBuilder build = new StringBuilder();
            build.Append(industry.Name + " - ");
            build.Append(industry.Points);
            results.Add(build.ToString());
        }
        return results;
    }

    public List<List<string>> GetAllIndustriesDetailed()
    {
        string countryName = Container.countryUnderControl;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<Industry> industries = country.industriesPoints;
        List<List<string>> results = new List<List<string>>();

        foreach (var industry in industries)
        {
            results.Add(industry.GetInfoDetailed());
        }
        return results;
    }

    public List<string> GetProductsNoAmounts(string countryName)
    {
        //string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        Dictionary<Product, int> products = country.productAmounts;
        List<string> results = new List<string>();

        foreach (var product in products)
        {
            StringBuilder build = new StringBuilder();
            build.Append(product.Key.Name + " ");
            if (product.Key.OriginalResource == null)
                build.Append("(Multiple)");
            else build.Append("(" + product.Key.OriginalResource.Name + ")");
            results.Add(build.ToString());
        }
        return results;
    }

    public double GetProductPrice(string countryName, string info)
    {
        //string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        string productName = info.Split(' ')[0];
        string resourceName = info.Split(' ')[1].Substring(1, info.Split(' ')[1].Length - 2);
        List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName).Select(pi => pi.Key).ToList();
        Product product;
        if (resourceName != "Multiple")
            product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
        else
            product = products.First();
        return product.Price;
    }

    public List<Product> FilterProductsByPlace(Vector2 coords, string industryName)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<Product> products = country.productIndustriesDependency.Where(pi => pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
        List<Product> results = new List<Product>();
        foreach (Product product in products)
        {
            if (FilterResourcesByPlace(coords, GetResourcesForProductName(coords, product.Name, industryName)).Count != 0)
                results.Add(product);
        }
        results.Distinct();
        return results;
    }

    //When we have a product, we may want to produce the same product with different resources
    public List<Product> GetUnusedProducts(Vector2 coords, string industryName)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<Product> inputProducts = (HasObjectInPlace(coords) as Facility).GetProduction().Where(p => p.Value > 0).Select(p => p.Key).ToList();
        List<Product> products = FilterProductsByPlace(coords, industryName);
        List<Product> results = products.Except(inputProducts).ToList();
        return results;
    }

    public List<string> GetPathwayTypesInfo(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        return country.GetPathwayTypes();
    }

    public List<string> GetPathwayTypesNames(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        return country.pathwayTypes.Select(p => p.Name).ToList();
    }

    public PathwayType GetPathwayType(Vector2 coords, string name)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        return country.pathwayTypes.Where(n => n.Name == name).First();
    }

    public double GetPathwayTypePrice(string countryName, string name)
    {
        //string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        return country.pathwayTypes.Where(n => n.Name == name).First().GetPrice();
    }

    public string GetObjectGeneralInfo(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        object obj = HasObjectInPlace(coords);

        StringBuilder build = new StringBuilder();
        if (obj == null)
            return "";
        else
            switch (obj.GetType().Name)
            {
                case "Storage":
                    build.Append((obj as Storage).Name);
                    build.Append(" (Storage; X ");
                    build.Append((obj as Storage).Coords.x);
                    build.Append(" , Y ");
                    build.Append((obj as Storage).Coords.y);
                    build.Append(" )");
                    return build.ToString();
                case "City":
                    build.Append((obj as City).Name);
                    build.Append(" (City; X ");
                    build.Append((obj as City).Coords.x);
                    build.Append(" , Y ");
                    build.Append((obj as City).Coords.y);
                    build.Append(" )");
                    return build.ToString();
                case "Facility":
                    build.Append((obj as Facility).Name);
                    build.Append(" (Facility; X ");
                    build.Append((obj as Facility).Coords.x);
                    build.Append(" , Y ");
                    build.Append((obj as Facility).Coords.y);
                    build.Append(" )");
                    return build.ToString(); 
                case "Hospital":
                    build.Append((obj as Hospital).CustomName);
                    build.Append(" (Hospital; ");
                    build.Append("X ");
                    build.Append((obj as Hospital).Coords.x);
                    build.Append(" , Y ");
                    build.Append((obj as Hospital).Coords.y);
                    build.Append(" )");
                    return build.ToString();
                case "Headquaters":
                    build.Append((obj as Headquaters).CustomName);
                    build.Append(" (Headquaters; ");
                    build.Append("X ");
                    build.Append((obj as Headquaters).Coords.x);
                    build.Append(" , Y ");
                    build.Append((obj as Headquaters).Coords.y);
                    build.Append(" )");
                    return build.ToString();
                case "Workshop":
                    build.Append((obj as Workshop).CustomName);
                    build.Append(" (Workshop; ");
                    build.Append("X ");
                    build.Append((obj as Workshop).Coords.x);
                    build.Append(" , Y ");
                    build.Append((obj as Workshop).Coords.y);
                    build.Append(" )");
                    return build.ToString();
                case "TrainingPlace":
                    build.Append((obj as TrainingPlace).CustomName);
                    build.Append(" (Training_Field; ");
                    build.Append("X ");
                    build.Append((obj as TrainingPlace).Coords.x);
                    build.Append(" , Y ");
                    build.Append((obj as TrainingPlace).Coords.y);
                    build.Append(" )");
                    return build.ToString();
                case "BattlePosition":
                    build.Append("Position");
                    build.Append(" (Battle_Position; ");
                    build.Append("X ");
                    build.Append((obj as BattlePosition).Position.x);
                    build.Append(" , Y ");
                    build.Append((obj as BattlePosition).Position.y);
                    build.Append(" )");
                    return build.ToString();
            }
        return "";
    }

    public List<string> GetObjectsForPathForming(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<string> objects = new List<string>();

        for (int i = 0; i < country.storages.Count; i++)
        {
            StringBuilder build = new StringBuilder();
            build.Append(country.storages[i].Name);
            build.Append(" (Storage; X ");
            build.Append(country.storages[i].Coords.x);
            build.Append(" , Y ");
            build.Append(country.storages[i].Coords.y);
            build.Append(" )");
            objects.Add(build.ToString());
        }

        for (int i = 0; i < country.cities.Count; i++)
        {
            StringBuilder build = new StringBuilder();
            build.Append(country.cities[i].Name);
            build.Append(" (City; X ");
            build.Append(country.cities[i].Coords.x);
            build.Append(" , Y ");
            build.Append(country.cities[i].Coords.y);
            build.Append(" )");
            objects.Add(build.ToString());
        }

        foreach (Industry industry in country.industriesPoints)
        {
            foreach (Facility facility in industry.facilities)
            {
                StringBuilder build = new StringBuilder();
                build.Append(facility.Name);
                build.Append(" (Facility; X ");
                build.Append(facility.Coords.x);
                build.Append(" , Y ");
                build.Append(facility.Coords.y);
                build.Append(" )");
                objects.Add(build.ToString());
            }
        }

        foreach (ContainerObject container in country.warfareContainerObjects)
        {
            StringBuilder build = new StringBuilder();
            build.Append(container.CustomName);
            switch (container.GetType().Name)
            {
                case "Hospital":
                    build.Append(" (Hospital; ");
                    break;
                case "Headquaters":
                    build.Append(" (Headquaters; ");
                    break;
                case "Workshop":
                    build.Append(" (Workshop; ");
                    break;
                case "TrainingPlace":
                    build.Append(" (Training_Field; ");
                    break;
            }
            build.Append("X ");
            build.Append(container.Coords.x);
            build.Append(" , Y ");
            build.Append(container.Coords.y);
            build.Append(" )");
            objects.Add(build.ToString());
        }

        foreach (BattlePosition battlePosition in country.battlePositions)
        {
            StringBuilder build = new StringBuilder();
            build.Append("Position");
            build.Append(" (Battle_position; X ");
            build.Append(battlePosition.Position.x);
            build.Append(" , Y ");
            build.Append(battlePosition.Position.y);
            build.Append(" )");
            objects.Add(build.ToString());
        }

        return objects;
    }

    public List<string> GetRoutes(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        List<string> results = new List<string>();

        for (int i = 0; i < Container.Routes.Count; i++)
        {
            int startX = (int)Container.Routes[i].First().Coordinates.Item1.x;
            int startY = (int)Container.Routes[i].First().Coordinates.Item1.y;
            if (Container.ComplexMap[startX, startY].Country.Name.Equals(countryName))
            {
                StringBuilder build = new StringBuilder();
                object obj = HasObjectInPlace(Container.Routes[i].First().Coordinates.Item1);
                switch (obj.GetType().Name)
                {
                    case "Storage":
                        build.Append((obj as Storage).Name);
                        build.Append(" X ");
                        build.Append(startX);
                        build.Append(" Y ");
                        build.Append(startY);
                        build.Append(" - ");
                        break;
                    case "Facility":
                        build.Append((obj as Facility).Name);
                        build.Append(" X ");
                        build.Append(startX);
                        build.Append(" Y ");
                        build.Append(startY);
                        build.Append(" - ");
                        break;
                    case "City":
                        build.Append((obj as City).Name);
                        build.Append(" X ");
                        build.Append(startX);
                        build.Append(" Y ");
                        build.Append(startY);
                        build.Append(" - ");
                        break;
                    case "Hospital":
                        build.Append((obj as Hospital).CustomName);
                        build.Append(" X ");
                        build.Append(startX);
                        build.Append(" Y ");
                        build.Append(startY);
                        build.Append(" - ");
                        break;
                    case "Headquaters":
                        build.Append((obj as Headquaters).CustomName);
                        build.Append(" X ");
                        build.Append(startX);
                        build.Append(" Y ");
                        build.Append(startY);
                        build.Append(" - ");
                        break;
                    case "Workshop":
                        build.Append((obj as Workshop).CustomName);
                        build.Append(" X ");
                        build.Append(startX);
                        build.Append(" Y ");
                        build.Append(startY);
                        build.Append(" - ");
                        break;
                    case "TrainingPlace":
                        build.Append((obj as TrainingPlace).CustomName);
                        build.Append(" X ");
                        build.Append(startX);
                        build.Append(" Y ");
                        build.Append(startY);
                        build.Append(" - ");
                        break;
                    case "BattlePosition":
                        build.Append("Position X ");
                        build.Append(startX);
                        build.Append(" , Y ");
                        build.Append(startY);
                        build.Append(" )");
                        break;
                }
                obj = HasObjectInPlace(Container.Routes[i].Last().Coordinates.Item1);
                switch (obj.GetType().Name)
                {
                    case "Storage":
                        build.Append((obj as Storage).Name);
                        build.Append(" X ");
                        build.Append((obj as Storage).Coords.x);
                        build.Append(" Y ");
                        build.Append((obj as Storage).Coords.y);
                        break;
                    case "Facility":
                        build.Append((obj as Facility).Name);
                        build.Append(" X ");
                        build.Append((obj as Facility).Coords.x);
                        build.Append(" Y ");
                        build.Append((obj as Facility).Coords.y);
                        break;
                    case "City":
                        build.Append((obj as City).Name);
                        build.Append(" X ");
                        build.Append((obj as City).Coords.x);
                        build.Append(" Y ");
                        build.Append((obj as City).Coords.y);
                        break;
                    case "Hospital":
                        build.Append((obj as Hospital).CustomName);
                        build.Append(" X ");
                        build.Append((obj as Hospital).Coords.x);
                        build.Append(" Y ");
                        build.Append((obj as Hospital).Coords.x);
                        break;
                    case "Headquaters":
                        build.Append((obj as Headquaters).CustomName);
                        build.Append(" X ");
                        build.Append((obj as Headquaters).Coords.x);
                        build.Append(" Y ");
                        build.Append((obj as Headquaters).Coords.x);
                        break;
                    case "Workshop":
                        build.Append((obj as Workshop).CustomName);
                        build.Append(" X ");
                        build.Append((obj as Workshop).Coords.x);
                        build.Append(" Y ");
                        build.Append((obj as Workshop).Coords.x);
                        break;
                    case "TrainingPlace":
                        build.Append((obj as TrainingPlace).CustomName);
                        build.Append(" X ");
                        build.Append((obj as TrainingPlace).Coords.x);
                        build.Append(" Y ");
                        build.Append((obj as TrainingPlace).Coords.x);
                        break;
                    case "BattlePosition":
                        build.Append("Position X ");
                        build.Append((obj as BattlePosition).Position.x);
                        build.Append(" , Y ");
                        build.Append((obj as BattlePosition).Position.y);
                        build.Append(" )");
                        break;
                }
                results.Add(build.ToString());
            }
        }
        return results;
    }

    public List<string> GetRouteMidwayObjects(Vector2 start, Vector2 end)
    {
        List<string> results = new List<string>();
        if (HasRoute(start, end))
        {
            List<LogisticObject> route = GetRoute(start, end);
            for (int i = 1; i < route.Count - 1; i++)
            {
                if (route[i].Coordinates.Item2 == new Vector2(-1, -1))
                {
                    object obj = HasObjectInPlace(route[i].Coordinates.Item1);
                    if (obj != null)
                        results.Add(GetObjectGeneralInfo(route[i].Coordinates.Item1));
                }
            }
        }
        return results;
    }

    public List<string> GetSoldierSpecialities(Vector2 coords)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

        List<IObject> objects = country.objectBank.Bank.Where(o => o.Key.Category == "H").Select(o => o.Key).ToList();
        List<Tuple<int, int>> amounts = country.objectBank.TimeValuesForCreation.Where(o => objects.Select(o => o.Name).Contains(o.Key)).Select(o => o.Value).ToList();
        List<string> results = new List<string>();

        for (int i = 0; i < objects.Count; i++)
        {
            results.Add(objects[i].Name + " - " + amounts[i].Item1 + " units in " + amounts[i].Item2 + " seconds");
        }
        
        return results;
    }

    public List<string> GetUnitsInWarfareContainer(Vector2 coords)
    {
        ContainerObject container = HasObjectInPlace(coords) as ContainerObject;
        List<string> info = new List<string>();
        switch (container.GetType().Name)
        {
            case "Hospital":
                info = (container as Hospital).GetInfoDetailed();
                break;
            case "Workshop":
                info = (container as Workshop).GetInfoDetailed();
                break;
            case "Headquaters":
                info = (container as Headquaters).GetInfoDetailed();
                break;
        }
        List<string> results = new List<string>();
        for (int i = 1; i < info.Count; i++)
        {
            if (info[i].Any(char.IsDigit))
                results.Add(info[i]);
        }
        return results;
    }

    public List<Tuple<string, int>> HasEnoughUnitsToTrain(Vector2 coords, string unitName, int quantity, int index)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

        List<Tuple<string, int>> list = new List<Tuple<string, int>>();
        if (index == 0 || (country.objectBank.Bank.Select(o => o.Key.Name).Contains(unitName) && 
            country.objectBank.Bank[country.objectBank.Bank.Where(o => o.Key.Name.Equals(unitName)).First().Key] >= quantity))
        {
            List<Tuple<string, int>> dependencies = country.objectBank.Dependencies.Where(d => d.Key.Equals(unitName)).FirstOrDefault().Value;
            if (dependencies == null)
                dependencies = new List<Tuple<string, int>>();
            if (dependencies.Count > 0)
            {
                for (int i = 0; i < dependencies.Count; i++)
                {
                    int tempQuantity = quantity * dependencies.ElementAt(i).Item2;
                    list.AddRange(HasEnoughUnitsToTrain(coords, country.objectBank.Bank.Where(b => b.Key.Name.Equals(dependencies.ElementAt(i).Item1)).First().Key.Name,
                        tempQuantity, ++index));
                }
            }
            if (list.Count < dependencies.Count)
            {
                list.Clear();
                return list;
            }
            list.Add(new Tuple<string, int>(unitName, quantity));
        }
        return list;
    }

    public bool DoesContainerFitUnit(GameObject panel)
    {
        string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

        Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("UnitDropdown")).First().transform.GetComponent<Dropdown>();
        string unitName = dropdown.options[dropdown.value].text.Split(' ')[0];
        int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
        IObject obj = country.objectBank.Bank.Where(o => o.Key.Name == unitName).First().Key;

        object objH;
        if (panel.name.Contains("Transfer"))
        {
            dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WTPlaceDropdown")).First().transform.GetComponent<Dropdown>();
            string coords = dropdown.options[dropdown.value].text;
            int x = int.Parse(coords.Split(' ')[4]);
            int y = int.Parse(coords.Split(' ')[7]);
            objH = HasObjectInPlace(new Vector2(x, y));
        }
        else
        {
            objH = HasObjectInPlace(Container.selectedCoordinates);
        }
        if (objH is BattlePosition)
            return true;

        ContainerObject container = objH as ContainerObject;

        if (container.GetType().Name.Equals("Hospital") && obj.Category == "H")
            return true;
        if (container.GetType().Name.Equals("Workshop") && (obj.Category == "T" || obj.Category == "V"))
            return true;
        if (container.GetType().Name.Equals("Headquaters"))
            return true;
        return false;
    }

    public bool HasSameName(Vector2 coords, string name)
    {
        string countryName = Container.ComplexMap[(int)coords.x, (int)coords.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

        for (int i = 0; i < country.storages.Count; i++)
        {
            if (country.storages[i].Name.Equals(name))
                return true;
        }

        for (int i = 0; i < country.cities.Count; i++)
        {
            if (country.cities[i].Name.Equals(name))
                return true;
        }

        foreach (Industry industry in country.industriesPoints)
        {
            foreach (Facility facility in industry.facilities)
            {
                if (facility.Name.Equals(name))
                    return true;
            }
        }

        for (int i = 0; i < country.warfareContainerObjects.Count; i++)
        {
            if (country.warfareContainerObjects[i].CustomName.Equals(name))
                return true;
        }

        return false;
    }


    #region Economics actions (for player)

    public bool HasEnoughResourcesForCreate()
    {
        string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        GameObject prevPanel = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("EFacilityEdit")).First();
        GameObject callerPanel = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("EProductionCreate")).First();

        string name = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().
            gameObject.GetComponent<Text>().text.Split(' ')[2];
        string industryName = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().
            gameObject.GetComponent<Text>().text.Split(' ')[2];
        Facility facility = country.industriesPoints.Where(c => c.Name.Equals(industryName)).First().facilities.Where(c => c.Name.Equals(name)).First();
        Dropdown dropdown = callerPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ProductDropdown")).First().transform.GetComponent<Dropdown>();
        string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
        string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
        List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName && pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
        Product product;
        if (resourceName != "Multiple")
            product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
        else
            product = products.First();

        int hasEnoughResources = 0;
        int hasEnoughRequiredResources = 0;
        int amountOfNeeded = facility.parentIndustry.GetNeeded().Count;
        int amountOfRequired = facility.parentIndustry.GetRequired().Count;

        if (facility.parentIndustry.Grade == 1)
        {
            if (country.resourceAmmounts[product.OriginalResource] >= facility.parentIndustry.GetNeeded()[product.OriginalResource])
                hasEnoughResources++;
        }
        else
        {
            for (int i = 0; i < facility.parentIndustry.GetNeeded().Count; i++)
            {
                if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is BasicResource)
                {
                    if (country.resourceAmmounts[facility.parentIndustry.GetNeeded().ElementAt(i).Key] >= facility.parentIndustry.GetNeeded().ElementAt(i).Value)
                        hasEnoughResources++;
                }
                else if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is Industry)
                {
                    if (country.industriesPoints.Where(ind => ind.Equals(facility.parentIndustry.GetNeeded().ElementAt(i).Key)).First().FreePoints >= facility.parentIndustry.GetNeeded().ElementAt(i).Value)
                        hasEnoughResources++;
                }
            }
        }
        for (int i = 0; i < facility.parentIndustry.GetRequired().Count; i++)
            if (country.resourceAmmounts[facility.parentIndustry.GetRequired().ElementAt(i).Key] >= facility.parentIndustry.GetRequired().ElementAt(i).Value)
                hasEnoughRequiredResources++;

        if (hasEnoughRequiredResources == amountOfRequired)
        {
            if (facility.parentIndustry.Grade == 1 && hasEnoughResources > 0)
                return true;
            else if (facility.parentIndustry.Grade > 1 && hasEnoughResources == amountOfNeeded)
                return true;
            else
                return false;
        }
        return false;
    }

    public bool HasEnoughResourcesForUpgrade()
    {
        string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
        GameObject prevPanel = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("EFacilityEdit")).First();

        string name = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().
            gameObject.GetComponent<Text>().text.Split(' ')[2];
        string industryName = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().
            gameObject.GetComponent<Text>().text.Split(' ')[2];
        Facility facility = country.industriesPoints.Where(c => c.Name.Equals(industryName)).First().facilities.Where(c => c.Name.Equals(name)).First();
        Dropdown dropdown = prevPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ProductionDropdown")).First().transform.GetComponent<Dropdown>();
        string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
        string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
        List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName && pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
        Product product;
        if (resourceName != "Multiple")
            product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
        else
            product = products.First();

        int hasEnoughResources = 0;
        int hasEnoughRequiredResources = 0;
        int amountOfNeeded = facility.parentIndustry.GetNeeded().Count;
        int amountOfRequired = facility.parentIndustry.GetRequired().Count;
        
        if (facility.parentIndustry.Grade == 1)
        {
            if (country.resourceAmmounts[product.OriginalResource] >= facility.parentIndustry.GetNeeded()[product.OriginalResource])
                hasEnoughResources++;
        }
        else
        {
            for (int i = 0; i < facility.parentIndustry.GetNeeded().Count; i++)
            {
                if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is BasicResource)
                    if (country.resourceAmmounts[facility.parentIndustry.GetNeeded().ElementAt(i).Key] >= facility.parentIndustry.GetNeeded().ElementAt(i).Value)
                        hasEnoughResources++;
                    else if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is Industry)
                        if (country.industriesPoints.Where(ind => ind.Equals(facility.parentIndustry.GetNeeded().ElementAt(i).Key)).First().FreePoints >= facility.parentIndustry.GetNeeded().ElementAt(i).Value)
                            hasEnoughResources++;
            }
        }
        for (int i = 0; i < facility.parentIndustry.GetRequired().Count; i++)
            if (country.resourceAmmounts[facility.parentIndustry.GetRequired().ElementAt(i).Key] >= facility.parentIndustry.GetRequired().ElementAt(i).Value)
                hasEnoughRequiredResources++;

        if (hasEnoughRequiredResources == amountOfRequired)
        {
            if (facility.parentIndustry.Grade == 1 && hasEnoughResources > 0)
                return true;
            else if (facility.parentIndustry.Grade > 1 && hasEnoughResources == amountOfNeeded)
                return true;
            else
                return false;
        }
        return false;
    }

    public void AddStorage(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
            InputField capacityInput = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CapacityInputField")).
                First().gameObject.GetComponent<InputField>();
            int capacity;
            if (string.IsNullOrEmpty(capacityInput.text))
                capacity = int.Parse(capacityInput.placeholder.GetComponentInChildren<Text>().text);
            else
                capacity = int.Parse(capacityInput.text);

            //0 will be replaced by money when I make the financial system
            country.AddStorage(country.lastStorageID, name, Container.selectedCoordinates, 0, capacity);

            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.economicsColors[index] = Container.objectColors["Storage"];
            Container.logisticsColors[index] = Container.objectColors["Node"];
            Container.RemakeTextures();
            AddNode(new LogisticObject(Container.currentLogisticID, "Storage",
                new Tuple<Vector2, Vector2>(Container.selectedCoordinates, new Vector2(-1, -1))));
            Container.currentLogisticID++;
            country.lastStorageID++;
            ActionPanelClose();
        }
    }

    public void AddCity(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
            
            country.AddCity(Container.selectedCoordinates, name);

            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.economicsColors[index] = Container.objectColors["City"];
            Container.logisticsColors[index] = Container.objectColors["Node"];
            Container.RemakeTextures();
            AddNode(new LogisticObject(Container.currentLogisticID, "City",
                new Tuple<Vector2, Vector2>(Container.selectedCoordinates, new Vector2(-1, -1))));
            Container.currentLogisticID++;
            ActionPanelClose();
        }
    }

    public void AddFacility(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
            Dropdown dropdown = FindObjectsOfType<GameObject>(true).ToList().Where(n => n.name.Equals("IndustryDropdown")).First().transform.GetComponent<Dropdown>();
            string industryName = dropdown.options[dropdown.value].text;

            //0 will be replaced by money when I make the financial system
            country.AddFacility(country.industriesPoints.Where(i => i.Name.Equals(industryName)).First(), country.lastFacilityID, name, Container.selectedCoordinates, 0);

            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.economicsColors[index] = Container.objectColors["Facility"];
            Container.logisticsColors[index] = Container.objectColors["Node"];
            Container.RemakeTextures();
            AddNode(new LogisticObject(Container.currentLogisticID, "Facility",
                new Tuple<Vector2, Vector2>(Container.selectedCoordinates, new Vector2(-1, -1))));
            Container.currentLogisticID++;
            country.lastFacilityID++;
            ActionPanelClose();
        }
    }

    public void AddPathway(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Dropdown from = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("FromObjectDropdown")).First().transform.GetComponent<Dropdown>();
            Dropdown to = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ToObjectDropdown")).First().transform.GetComponent<Dropdown>();
            Dropdown type = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
            string[] arrayStart = from.options[from.value].text.Split(' ');
            string[] arrayEnd = to.options[to.value].text.Split(' ');
            Vector2 coordsStart = new Vector2(int.Parse(arrayStart[3]), int.Parse(arrayStart[6]));
            Vector2 coordsEnd = new Vector2(int.Parse(arrayEnd[3]), int.Parse(arrayEnd[6]));
            List<Vector2> path = new PathwayCalculator(coordsStart, coordsEnd).Calculate();

            country.AddPathway(coordsStart, coordsEnd, country.pathwayTypes.Where(t => t.Name.Equals(type.options[type.value].text)).First());
            AddEdge(coordsStart, coordsEnd, path.Count);
            for (int i = 0; i < path.Count; i++)
            {
                if (i != 0 && i != path.Count - 1)
                {
                    int index = (int)path.ElementAt(i).y * Container.size + (int)path.ElementAt(i).x;
                    Container.economicsColors[index] = Container.objectColors["Pathway"];
                    Container.logisticsColors[index] = Container.objectColors["Path"];
                }
            }
            Container.RemakeTextures();
            ActionPanelClose();
        }
    }

    public void EditStorage(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;
            InputField capacityInput = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).
                First().gameObject.GetComponent<InputField>();
            int capacity;
            if (string.IsNullOrEmpty(capacityInput.text))
                capacity = int.Parse(capacityInput.placeholder.GetComponentInChildren<Text>().text);
            else
                capacity = int.Parse(capacityInput.text);

            //0 will be replaced by money when I make the financial system
            country.SetStorageCapacity((HasObjectInPlace(Container.selectedCoordinates) as Storage).ID, capacity);
            if (!string.IsNullOrEmpty(name))
                country.storages.Where(s => s.ID == (HasObjectInPlace(Container.selectedCoordinates) as Storage).ID).First().Name = name;

            ActionPanelClose();
        }
    }

    public void AddProductToStorage(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Storage storage = HasObjectInPlace(Container.selectedCoordinates) as Storage;

            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("AProductDropdown")).First().transform.GetComponent<Dropdown>();
            string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
            string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
            int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("AAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
            List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName).Select(pi => pi.Key).ToList();
            Product product;
            if (resourceName != "Multiple")
                product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
            else
                product = products.First();
            country.AddProductAmountToStorage(storage, product, amount);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void RemoveProductFromStorage(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Storage storage = HasObjectInPlace(Container.selectedCoordinates) as Storage;

            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("RProductDropdown")).First().transform.GetComponent<Dropdown>();
            string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
            string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
            int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("RAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
            List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName).Select(pi => pi.Key).ToList();
            Product product;
            if (resourceName != "Multiple")
                product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
            else
                product = products.First();
            country.RemoveProductAmountFromStorage(storage, product, amount);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void EditCity(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;

            if (!string.IsNullOrEmpty(name))
                country.cities.Where(c => c.Coords == Container.selectedCoordinates).First().Name = name;

            ActionPanelClose();
        }
    }

    //This function is for Rename only!
    public void EditFacility(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string industryName = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().gameObject.GetComponent<Text>().text.Split(' ')[2];
            string oldName = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().gameObject.GetComponent<Text>().text.Split(' ')[2];
            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;

            if (!string.IsNullOrEmpty(name))
                country.industriesPoints.Where(c => c.Name.Equals(industryName)).First().facilities.Where(c => c.Name.Equals(oldName)).First().Name = name;

            ActionPanelClose();
        }
    }

    public void AddProduction(Image callerPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = callerPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            string industryName = callerPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            Facility facility = country.industriesPoints.Where(c => c.Name.Equals(industryName)).First().facilities.Where(c => c.Name.Equals(name)).First();
            Dropdown dropdown = callerPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ProductDropdown")).First().transform.GetComponent<Dropdown>();
            string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
            string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
            List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName && pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
            Product product;
            if (resourceName != "Multiple")
                product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
            else
                product = products.First();

            country.StartProducingProduct(facility, product);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void IncreaseProductionPower(Image callerPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            GameObject prevPanel = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("EFacilityEdit")).First();

            string name = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            string industryName = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            Facility facility = country.industriesPoints.Where(c => c.Name.Equals(industryName)).First().facilities.Where(c => c.Name.Equals(name)).First();
            Dropdown dropdown = prevPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ProductionDropdown")).First().transform.GetComponent<Dropdown>();
            string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
            string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
            List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName && pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
            Product product;
            if (resourceName != "Multiple")
                product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
            else
                product = products.First();

            country.IncreaseProductProduction(facility, product);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void DecreaseProductionPower(Image callerPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            GameObject prevPanel = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("EFacilityEdit")).First();

            string name = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            string industryName = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            Facility facility = country.industriesPoints.Where(c => c.Name.Equals(industryName)).First().facilities.Where(c => c.Name.Equals(name)).First();
            Dropdown dropdown = prevPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ProductionDropdown")).First().transform.GetComponent<Dropdown>();
            string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
            string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
            List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName && pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
            Product product;
            if (resourceName != "Multiple")
                product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
            else
                product = products.First();

            country.DecreaseProductProduction(facility, product);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void RemoveProduction(Image callerPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            GameObject prevPanel = FindObjectsOfType<GameObject>().Where(o => o.name.Contains("EFacilityEdit")).First();

            string name = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("OldNameText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            string industryName = prevPanel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("IndustryText")).First().
                gameObject.GetComponent<Text>().text.Split(' ')[2];
            Facility facility = country.industriesPoints.Where(c => c.Name.Equals(industryName)).First().facilities.Where(c => c.Name.Equals(name)).First();
            Dropdown dropdown = prevPanel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ProductionDropdown")).First().transform.GetComponent<Dropdown>();
            string productName = dropdown.options[dropdown.value].text.Split(' ')[0];
            string resourceName = dropdown.options[dropdown.value].text.Split(' ')[1].Substring(1, dropdown.options[dropdown.value].text.Split(' ')[1].Length - 2);
            List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName && pi.Value.Name == industryName).Select(pi => pi.Key).ToList();
            Product product;
            if (resourceName != "Multiple")
                product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
            else
                product = products.First();

            country.StopProducingProduct(facility, product);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void EditPathway(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Pathway pathway = HasObjectInPlace(Container.selectedCoordinates) as Pathway;
            Dropdown type = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
            country.ChangePathwayType(pathway.BeginCoordinates, pathway.EndCoordinates, country.pathwayTypes.Where(t => t.Name.Equals(type.options[type.value].text)).First());
            ActionPanelClose();
        }
    }

    public void RemoveStorage(Image errorPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Storage storage = HasObjectInPlace(Container.selectedCoordinates) as Storage;
            country.RemoveStorage(storage.ID);
            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.economicsColors[index] = Color.white;
            //Removing just the node doesn't have impact on logistics - when we remove the factory, we don't remove the roads around it
            Container.RemakeTextures();
            ErrorPanelClose(errorPanel);
            ActionPanelClose();
        }
    }

    public void RemoveCity(Image errorPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            City city = HasObjectInPlace(Container.selectedCoordinates) as City;
            country.RemoveCity(Container.selectedCoordinates, city.Name);
            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.economicsColors[index] = Color.white;
            //Removing just the node doesn't have impact on logistics - when we remove the factory, we don't remove the roads around it
            Container.RemakeTextures();
            ErrorPanelClose(errorPanel);
            ActionPanelClose();
        }
    }

    public void RemoveFacility(Image errorPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Facility facility = HasObjectInPlace(Container.selectedCoordinates) as Facility;
            country.RemoveFacility(facility.parentIndustry, facility.ID);
            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.economicsColors[index] = Color.white;
            //Removing just the node doesn't have impact on logistics - when we remove the factory, we don't remove the roads around it
            Container.RemakeTextures();
            ErrorPanelClose(errorPanel);
            ActionPanelClose();
        }
    }

    public void RemovePathway(Image errorPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Pathway pathway = HasObjectInPlace(Container.selectedCoordinates) as Pathway;

            //But removing the pathway is removing the Edge in the logistic map
            RemoveEdge(pathway.BeginCoordinates, pathway.EndCoordinates);
            for (int i = 0; i < pathway.Parts.Count; i++)
            {
                if (i != 0 && i != pathway.Parts.Count - 1)
                {
                    int index = (int)pathway.Parts[i].y * Container.size + (int)pathway.Parts[i].x;
                    Container.economicsColors[index] = Color.white;
                    Container.logisticsColors[index] = Color.white;
                }
            }
            country.RemovePathway(pathway.BeginCoordinates, pathway.EndCoordinates);
            Container.RemakeTextures();
            ErrorPanelClose(errorPanel);
            ActionPanelClose();
        }
    }

    public void RepeatDeleteAction(Image errorPanel)
    {
        if (Container.actionValid)
        {
            object obj = HasObjectInPlace(Container.selectedCoordinates);
            switch (obj.GetType().Name)
            {
                case "Facility":
                    RemoveFacility(errorPanel);
                    break;
                case "Storage":
                    RemoveStorage(errorPanel);
                    break;
                case "Pathway":
                    RemovePathway(errorPanel);
                    break;
            }
        }
    }

    public void PaintRoute(Vector2 coordsStart, Vector2 coordsEnd)
    {
        List<LogisticObject> route = GetRoute(coordsStart, coordsEnd);
        if (route == null)
            return;
        for (int i = 0; i < route.Count; i++)
        {
            if (i != route.Count - 1)
            {
                Vector2 start = route[i].Coordinates.Item1;
                Vector2 end = route[i + 1].Coordinates.Item1;
                List<Vector2> path = new PathwayCalculator(start, end).Calculate();
                for (int j = 0; j < path.Count; j++)
                {
                    int index = (int)path.ElementAt(j).y * Container.size + (int)path.ElementAt(j).x;
                    Container.pathsColors[index] = Container.objectColors["LogisticsPath"];
                }
            }
        }
    }

    public void CreatePath(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            Dropdown from = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("FromObjectDropdown")).First().transform.GetComponent<Dropdown>();
            Dropdown to = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("ToObjectDropdown")).First().transform.GetComponent<Dropdown>();
            string[] arrayStart = from.options[from.value].text.Split(' ');
            string[] arrayEnd = to.options[to.value].text.Split(' ');
            Vector2 coordsStart = new Vector2(int.Parse(arrayStart[3]), int.Parse(arrayStart[6]));
            Vector2 coordsEnd = new Vector2(int.Parse(arrayEnd[3]), int.Parse(arrayEnd[6]));
            int res = MakePath(coordsStart, coordsEnd, -1);
            Container.ClearPathTexture();
            PaintRoute(coordsStart, coordsEnd);            
            Container.RemakePathTexture();
            ActionPanelClose();
        }
    }

    //FOR NOW, the start & end points are left intact when removing objects!
    //NEEDED when removing Facility/Storage/City which is a start/end route point, destroy the routes as well!  DONE
    public void RemovePath(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            string startText = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("FromObjectText")).First().gameObject.GetComponent<Text>().text;
            string endText = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("ToObjectText")).First().gameObject.GetComponent<Text>().text;
            string[] arrayStart = startText.Split(' ');
            string[] arrayEnd = endText.Split(' ');
            Vector2 coordsStart = new Vector2(int.Parse(arrayStart[4]), int.Parse(arrayStart[7]));
            Vector2 coordsEnd = new Vector2(int.Parse(arrayEnd[4]), int.Parse(arrayEnd[7]));
            RemoveRoute(coordsStart, coordsEnd);
            Container.ClearPathTexture();
            ActionPanelClose();
        }
    }

    public void ChangeProductPrice(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            InputField priceInput = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("EPriceInputField")).
                First().gameObject.GetComponent<InputField>();
            int price;
            if (string.IsNullOrEmpty(priceInput.text))
                price = int.Parse(priceInput.placeholder.GetComponentInChildren<Text>().text);
            else
                price = int.Parse(priceInput.text);
            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Contains("EProductDropdown")).First().gameObject.GetComponent<Dropdown>();
            string info = dropdown.options[dropdown.value].text;
            string productName = info.Split(' ')[0];
            string resourceName = info.Split(' ')[1].Substring(1, info.Split(' ')[1].Length - 2);
            List<Product> products = country.productIndustriesDependency.Where(pi => pi.Key.Name == productName).Select(pi => pi.Key).ToList();
            Product product;
            if (resourceName != "Multiple")
                product = products.Where(p => p.OriginalResource.Name.Equals(resourceName)).FirstOrDefault();
            else
                product = products.First();
            country.ChangeProductPrice(product, price);
            LayeredActionPanelClose();
        }
    }

    public void ChangePathwayTypePrice(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            InputField priceInput = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("LPriceInputField")).
                First().gameObject.GetComponent<InputField>();
            int price;
            if (string.IsNullOrEmpty(priceInput.text))
                price = int.Parse(priceInput.placeholder.GetComponentInChildren<Text>().text);
            else
                price = int.Parse(priceInput.text);
            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Contains("LTypeDropdown")).First().gameObject.GetComponent<Dropdown>();
            PathwayType type = country.pathwayTypes.Where(t => t.Name.Equals(dropdown.options[dropdown.value].text)).First();
            country.ChangePathwayTypePrice(type, price);
            ActionPanelClose();
        }
    }

    #endregion

    #region Warfare actions (for player)

    public void AddContainer(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NameInputField")).First().gameObject.GetComponent<InputField>().text;
            InputField capacityInput = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("CapacityInputField")).
                First().gameObject.GetComponent<InputField>();
            int capacity;
            if (string.IsNullOrEmpty(capacityInput.text))
                capacity = int.Parse(capacityInput.placeholder.GetComponentInChildren<Text>().text);
            else
                capacity = int.Parse(capacityInput.text);

            Text containerClass = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().GetComponent<Text>();
            if (containerClass.text.Contains("Hospital", StringComparison.OrdinalIgnoreCase))
                country.AddWarfareContainer("HP", name, Container.selectedCoordinates, capacity);
            else if (containerClass.text.Contains("Headquaters", StringComparison.OrdinalIgnoreCase))
                country.AddWarfareContainer("HQ", name, Container.selectedCoordinates, capacity);
            else if (containerClass.text.Contains("Workshop", StringComparison.OrdinalIgnoreCase))
                country.AddWarfareContainer("W", name, Container.selectedCoordinates, capacity);
            else if (containerClass.text.Contains("Training", StringComparison.OrdinalIgnoreCase))
                country.AddWarfareContainer("TP", name, Container.selectedCoordinates, capacity);


            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.warfareColors[index] = Container.objectColors["WarfareContainer"];
            Container.logisticsColors[index] = Container.objectColors["Node"];
            Container.RemakeTextures();
            AddNode(new LogisticObject(Container.currentLogisticID, "WarfareContainer",
                new Tuple<Vector2, Vector2>(Container.selectedCoordinates, new Vector2(-1, -1))));
            Container.currentLogisticID++;
            country.lastWarfareObjID++;
            ActionPanelClose();
        }
    }

    public void EditContainer(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            string name = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewNameInputField")).First().gameObject.GetComponent<InputField>().text;
            InputField capacityInput = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("NewCapacityInputField")).
                First().gameObject.GetComponent<InputField>();
            int capacity;
            if (string.IsNullOrEmpty(capacityInput.text))
                capacity = int.Parse(capacityInput.placeholder.GetComponentInChildren<Text>().text);
            else
                capacity = int.Parse(capacityInput.text);

            Text containerClass = panel.transform.GetComponentsInChildren<Transform>().Where(o => o.name.Equals("TitleText")).First().GetComponent<Text>();
            if (containerClass.text.Contains("Hospital", StringComparison.OrdinalIgnoreCase))
                country.SetWarfareContainerCapacity("HP", (HasObjectInPlace(Container.selectedCoordinates) as ContainerObject).CustomName, capacity);
            else if (containerClass.text.Contains("Headquaters", StringComparison.OrdinalIgnoreCase))
                country.SetWarfareContainerCapacity("HQ", (HasObjectInPlace(Container.selectedCoordinates) as ContainerObject).CustomName, capacity);
            else if (containerClass.text.Contains("Workshop", StringComparison.OrdinalIgnoreCase))
                country.SetWarfareContainerCapacity("W", (HasObjectInPlace(Container.selectedCoordinates) as ContainerObject).CustomName, capacity);
            else if (containerClass.text.Contains("Training", StringComparison.OrdinalIgnoreCase))
                country.SetWarfareContainerCapacity("TP", (HasObjectInPlace(Container.selectedCoordinates) as ContainerObject).CustomName, capacity);
            if (!string.IsNullOrEmpty(name))
                country.warfareContainerObjects.Where(s => s == (HasObjectInPlace(Container.selectedCoordinates) as ContainerObject)).First().CustomName = name;

            ActionPanelClose();
        }
    }

    public void RemoveContainer(Image errorPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            ContainerObject container = HasObjectInPlace(Container.selectedCoordinates) as ContainerObject;
            country.RemoveWarfareContainer(container.CustomName);
            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.warfareColors[index] = Color.white;
            //Removing just the node doesn't have impact on logistics - when we remove the hospital, we don't remove the roads around it
            Container.RemakeTextures();
            ErrorPanelClose(errorPanel);
            ActionPanelClose();
        }
    }

    public void AddBattlePosition(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            country.AddBattlePosition(Container.selectedCoordinates);
            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.warfareColors[index] = Container.objectColors["BattlePosition"];
            Container.logisticsColors[index] = Container.objectColors["Node"];
            Container.RemakeTextures();
            AddNode(new LogisticObject(Container.currentLogisticID, "BattlePosition",
                new Tuple<Vector2, Vector2>(Container.selectedCoordinates, new Vector2(-1, -1))));
            Container.currentLogisticID++;
            country.lastBattlePositionID++;
            ActionPanelClose();
        }
    }

    public void RemoveBattlePosition(Image errorPanel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            country.RemoveBattlePosition(Container.selectedCoordinates);
            int index = (int)Container.selectedCoordinates.y * Container.size + (int)Container.selectedCoordinates.x;
            Container.warfareColors[index] = Color.white;
            //Removing just the node doesn't have impact on logistics - when we remove the hospital, we don't remove the roads around it
            Container.RemakeTextures();
            ErrorPanelClose(errorPanel);
            ActionPanelClose();
        }
    }

    public void TrainSoldier(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            TrainingPlace train = HasObjectInPlace(Container.selectedCoordinates) as TrainingPlace;
            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
            string objectName = dropdown.options[dropdown.value].text.Split(' ')[0];
            int quantity = int.Parse(dropdown.options[dropdown.value].text.Split(' ')[2]);

            train.Train((MovableObject)country.objectBank.Bank.Where(o => o.Key.Name == objectName).First().Key, quantity);
            ErrorPanelClose(FindObjectsOfType<Image>(false).ToList().Where(o => o.name.Contains("GameplayErrorPanel")).FirstOrDefault());
            ActionPanelClose();
        }
    }

    public void AddStationaryObjectToBattlePosition(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            BattlePosition battlePos = HasObjectInPlace(Container.selectedCoordinates) as BattlePosition;
            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("TypeDropdown")).First().transform.GetComponent<Dropdown>();
            string objectName = dropdown.options[dropdown.value].text.Split(' ')[0];
            StationaryObject stationaryObject = (StationaryObject)country.objectBank.Bank.Where(o => o.Key.Name == objectName).First().Key;
            battlePos.TryAddStationary(stationaryObject, 1);
            ErrorPanelClose(FindObjectsOfType<Image>(false).ToList().Where(o => o.name.Contains("GameplayErrorPanel")).FirstOrDefault());
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void AddObjectToContainer(Image panel)
    {
        if (Container.actionValid && !Container.unitForPosition)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            ContainerObject container = HasObjectInPlace(Container.selectedCoordinates) as ContainerObject;

            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WAUnitDropdown")).First().transform.GetComponent<Dropdown>();
            string unitName = dropdown.options[dropdown.value].text.Split(' ')[0];
            int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
            IObject obj = country.objectBank.Bank.Where(o => o.Key.Name == unitName).First().Key;
            country.AddUnitAmountToWarfareContainer(container, obj, amount);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void RemoveObjectFromContainer(Image panel)
    {
        if (Container.actionValid && !Container.unitForPosition)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            ContainerObject container = HasObjectInPlace(Container.selectedCoordinates) as ContainerObject;

            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WRUnitDropdown")).First().transform.GetComponent<Dropdown>();
            string unitName = dropdown.options[dropdown.value].text.Split(' ')[0];
            int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
            IObject obj = country.objectBank.Bank.Where(o => o.Key.Name == unitName).First().Key;
            country.RemoveUnitAmountFromWarfareContainer(container, obj, amount);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void AddObjectToBattlePosition(Image panel)
    {
        if (Container.actionValid && Container.unitForPosition)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WAUnitDropdown")).First().transform.GetComponent<Dropdown>();
            string unitName = dropdown.options[dropdown.value].text.Split(' ')[0];
            int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WAAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
            IObject obj = country.objectBank.Bank.Where(o => o.Key.Name == unitName).First().Key;
            country.AddUnitAmountToBattlePosition(Container.selectedCoordinates, obj, amount);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void RemoveObjectFromBattlePosition(Image panel)
    {
        if (Container.actionValid && Container.unitForPosition)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WRUnitDropdown")).First().transform.GetComponent<Dropdown>();
            string unitName = dropdown.options[dropdown.value].text.Split(' ')[0];
            int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WRAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
            IObject obj = country.objectBank.Bank.Where(o => o.Key.Name == unitName).First().Key;
            country.RemoveUnitAmountFromBattlePosition(Container.selectedCoordinates, obj, amount);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    public void TransferUnitFromPositionToContainer(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();

            Dropdown dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WTUnitDropdown")).First().transform.GetComponent<Dropdown>();
            string unitName = dropdown.options[dropdown.value].text.Split(' ')[0];
            int amount = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("WTAmountInput")).First().gameObject.GetComponentInChildren<Text>().text);
            double resource = double.Parse(dropdown.options[dropdown.value].text.Split(' ')[3]);
            IObject obj = country.objectBank.Bank.Where(o => o.Key.Name == unitName).First().Key;

            dropdown = panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Equals("WTPlaceDropdown")).First().transform.GetComponent<Dropdown>();
            string coords = dropdown.options[dropdown.value].text;
            int x = int.Parse(coords.Split(' ')[4]);
            int y = int.Parse(coords.Split(' ')[7]);
            object objH = HasObjectInPlace(new Vector2(x, y));

            country.TransferUnitAmountFromPositionToContainer(Container.selectedCoordinates, objH, obj, amount, resource);
            ActionPanelClose();
            LayeredActionPanelClose();
        }
    }

    

    public void PutPressure(Image panel)
    {
        if (Container.actionValid)
        {
            string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x, (int)Container.selectedCoordinates.y].Country.Name;
            CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
            BattlePosition pos = HasObjectInPlace(Container.selectedCoordinates) as BattlePosition;
            int strength = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("StrengthInput")).First().gameObject.GetComponentInChildren<Text>().text); ;
            int seconds = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("TimeInput")).First().gameObject.GetComponentInChildren<Text>().text); ;

            pos.Pressure(strength, seconds);
            ErrorPanelClose(FindObjectsOfType<Image>(false).ToList().Where(o => o.name.Contains("GameplayErrorPanel")).FirstOrDefault());
            ActionPanelClose();
        }
    }

    #endregion

    #region Logistics control

    public bool HasRoute(Vector2 start, Vector2 end)
    {
        LogisticObject indexS = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(start)).First();
        LogisticObject indexE = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(end)).First();
        for (int i = 0; i < Container.Routes.Count; i++)
        {
            if (Container.Routes[i].First().Coordinates.Item1.Equals(start) && Container.Routes[i].Last().Coordinates.Item1.Equals(end))
                return true;
        }
        return false;
    }

    public bool HasRouteWithEdge(Vector2 start, Vector2 end)
    {
        LogisticObject indexS = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(start)).First();
        LogisticObject indexE = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(end)).First();
        List<LogisticObject> brokenPart = new List<LogisticObject> { indexS, indexE };
        List<LogisticObject> brokenPartReverse = new List<LogisticObject> { indexE, indexS };
        for (int i = 0; i < Container.Routes.Count; i++)
        {
            for (int j = 0; j <= Container.Routes.ElementAt(i).Count - brokenPart.Count; j++)
            {
                if (Container.Routes.ElementAt(i).Skip(j).Take(brokenPart.Count).SequenceEqual(brokenPart))
                {
                    return true;
                }
                if (Container.Routes.ElementAt(i).Skip(j).Take(brokenPartReverse.Count).SequenceEqual(brokenPartReverse))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddNode(LogisticObject node)
    {
        //When adding a Node, you add a row and a column
        Container.Nodes.Add(node);
        Container.logisticsMap.Add(new List<int>());
        for (int i = 0; i < Container.Nodes.Count - 1; i++)
        {
            Container.logisticsMap.ElementAt(Container.logisticsSize).Add(-1);
        }
        Container.logisticsSize++;
        for (int i = 0; i < Container.logisticsMap.Count; i++)
        {
            if (i == Container.logisticsMap.Count - 1)
                Container.logisticsMap.ElementAt(i).Add(0);
            else
                Container.logisticsMap.ElementAt(i).Add(-1);
        }
    }

    public void AddEdge(Vector2 nodeStart, Vector2 nodeEnd, int weight)
    {
        //When adding an Edge, you add a number between row and column
        int indexS = Container.Nodes.FindIndex(n => n.Coordinates.Item1.Equals(nodeStart));
        int indexE = Container.Nodes.FindIndex(n => n.Coordinates.Item1.Equals(nodeEnd));
        //The matrix is symmetric
        Container.logisticsMap.ElementAt(indexS)[indexE] = weight;
        Container.logisticsMap.ElementAt(indexE)[indexS] = weight;
    }

    public int miniDist(int[] distance, bool[] tset)
    {
        int minimum = int.MaxValue;
        int index = 0;
        for (int k = 0; k < distance.Length; k++)
        {
            if (!tset[k] && distance[k] <= minimum)
            {
                minimum = distance[k];
                index = k;
            }
        }
        return index;
    }

    public int MakePath(Vector2 start, Vector2 end, int index)
    {
        int indexS = Container.Nodes.FindIndex(n => n.Coordinates.Item1.Equals(start));
        int indexE = Container.Nodes.FindIndex(n => n.Coordinates.Item1.Equals(end));

        //Use Dijkstra algorithm for pathmaking
        int length = Container.logisticsSize;
        int[] distance = new int[length];
        bool[] used = new bool[length];
        int[] prev = new int[length];

        for (int i = 0; i < length; i++)
        {
            distance[i] = int.MaxValue;
            used[i] = false;
            prev[i] = -1;
        }
        distance[indexS] = 0;

        for (int k = 0; k < length - 1; k++)
        {
            int minNode = miniDist(distance, used);
            used[minNode] = true;
            for (int i = 0; i < length; i++)
            {
                if (Container.logisticsMap[minNode][i] > -1)
                {
                    int shortestToMinNode = distance[minNode];
                    int? distanceToNextNode = Container.logisticsMap[minNode][i];
                    int? totalDistance = shortestToMinNode + distanceToNextNode;
                    if (totalDistance < distance[i])
                    {
                        distance[i] = (int)totalDistance;
                        prev[i] = minNode;
                    }
                }
            }
        }
        if (distance[indexE] != int.MaxValue)
        {
            var path = new LinkedList<LogisticObject>();
            int currentNode = indexE;
            while (currentNode != -1)
            {
                path.AddFirst(Container.Nodes.Where(n => n.ID == currentNode).First());
                currentNode = prev[currentNode];
            }

            //There may already be a route with these nodes
            if (Container.Routes.Contains(path.ToList()))
                return 1;
            //if index == -1, put the last, else - replace the route on index
            if (index == -1)
                Container.Routes.Add(path.ToList());
            else
                Container.Routes[index] = path.ToList();
            return 0;
        }
        return 1;
    }

    public Tuple<List<LogisticObject>, int> GetPath(Vector2 start, Vector2 end)
    {
        List<LogisticObject> path = Container.Routes.Where(l => l.First().Coordinates.Item1.Equals(start) &&
        l.Last().Coordinates.Item1.Equals(end)).First();
        if (path == null)
        {
            return new Tuple<List<LogisticObject>, int>(null, 0);
        }
        int length = 0;
        int prev = Container.Nodes.IndexOf(path.ElementAt(0));
        int next = Container.Nodes.IndexOf(path.ElementAt(0));
        int curIndex = 0;
        while (true)
        {
            length += Container.logisticsMap[prev][next];
            curIndex++;
            if (curIndex == path.Count)
                break;
            prev = next;
            next = Container.Nodes.IndexOf(path.ElementAt(curIndex));
        }
        return new Tuple<List<LogisticObject>, int>(path, length);
    }

    public int RemoveEdge(Vector2 nodeStart, Vector2 nodeEnd)
    {
        //When removing a Node and just a Node, do nothing
        //But when removing an Edge, set its weight to -1
        LogisticObject indexS = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(nodeStart)).First();
        LogisticObject indexE = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(nodeEnd)).First();
        //The matrix is symmetric
        //Reset only if there's an edge
        if (Container.logisticsMap.ElementAt(indexS.ID)[indexE.ID] != -1)
        {
            Container.logisticsMap.ElementAt(indexS.ID)[indexE.ID] = -1;
            Container.logisticsMap.ElementAt(indexE.ID)[indexS.ID] = -1;
            //Find all routes that were broken because of this edge
            int[] brokenRoutes = new int[Container.Routes.Count];
            List<LogisticObject> brokenPart = new List<LogisticObject> { indexS, indexE };
            List<LogisticObject> brokenPartReverse = new List<LogisticObject> { indexE, indexS };
            for (int i = 0; i < Container.Routes.Count; i++)
            {
                for (int j = 0; j <= Container.Routes.ElementAt(i).Count - brokenPart.Count; j++)
                {
                    if (Container.Routes.ElementAt(i).Skip(j).Take(brokenPart.Count).SequenceEqual(brokenPart))
                    {
                        brokenRoutes[i] = 1;
                        break;
                    }
                    if (Container.Routes.ElementAt(i).Skip(j).Take(brokenPartReverse.Count).SequenceEqual(brokenPartReverse))
                    {
                        brokenRoutes[i] = 1;
                        break;
                    }
                }
                if (brokenRoutes[i] != 1)
                {
                    brokenRoutes[i] = 0;
                }
            }
            //And find new routes
            for (int i = 0; i < Container.Routes.Count; i++)
            {
                if (brokenRoutes[i] != 0)
                {
                    return MakePath(Container.Routes.ElementAt(i).First().Coordinates.Item1, Container.Routes.ElementAt(i).Last().Coordinates.Item1, i);
                }
            }
        }
        return 0;
    }

    public int[] RemoveNodeAndEdge(Vector2 node)
    {
        //When removing a Node with edges attached to it, set the Node's edges weight to -1
        int index = Container.Nodes.FindIndex(n => n.Coordinates.Item1.Equals(node));
        List<List<LogisticObject>> brokenEdges = new List<List<LogisticObject>>();
        List<List<LogisticObject>> brokenEdgesReverse = new List<List<LogisticObject>>();
        for (int i = 0; i < Container.logisticsMap.Count; i++)
        {
            //Find all the edges that were broken
            if(Container.logisticsMap.ElementAt(i)[index] != -1 && i != index)
            {
                LogisticObject indexS = Container.Nodes.Where(n => n.ID == i).First();
                LogisticObject indexE = Container.Nodes.Where(n => n.ID == index).First();
                brokenEdges.Add(new List<LogisticObject> { indexS, indexE });
                brokenEdgesReverse.Add(new List<LogisticObject> { indexE, indexS });
                Container.logisticsMap.ElementAt(i)[index] = -1;
                Container.logisticsMap.ElementAt(index)[i] = -1;
            }
        }

        int[] brokenRoutes = new int[Container.Routes.Count];
        for (int i = 0; i < Container.Routes.Count; i++)
            brokenRoutes[i] = 0;

        for (int k = 0; k < brokenEdges.Count; k++)
        {
            for (int i = 0; i < Container.Routes.Count; i++)
            {
                List<LogisticObject> brokenPart = brokenEdges[k];
                List<LogisticObject> brokenPartReverse = brokenEdgesReverse[k];
                for (int j = 0; j <= Container.Routes.ElementAt(i).Count - brokenPart.Count; j++)
                {
                    if (Container.Routes.ElementAt(i).Skip(j).Take(brokenPart.Count).SequenceEqual(brokenPart))
                    {
                        brokenRoutes[i]++;
                    }
                    if (Container.Routes.ElementAt(i).Skip(j).Take(brokenPartReverse.Count).SequenceEqual(brokenPartReverse))
                    {
                        brokenRoutes[i]++;
                    }
                }
            }
        }
        for (int i = 0; i < Container.Routes.Count; i++)
        {
            if (brokenRoutes[i] != 0)
            {
                brokenRoutes[i] = MakePath(Container.Routes.ElementAt(i).First().Coordinates.Item1, Container.Routes.ElementAt(i).Last().Coordinates.Item1, i);
                if (brokenRoutes[i] != 0)
                {
                    Container.Routes.Remove(Container.Routes.ElementAt(i));
                }
            }
        }
        return brokenRoutes;
    }

    public List<LogisticObject> GetRoute(Vector2 start, Vector2 end)
    {
        LogisticObject indexS = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(start)).First();
        LogisticObject indexE = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(end)).First();

        return Container.Routes.Where(r => r.First().Coordinates.Item1.Equals(start) && r.Last().Coordinates.Item1.Equals(end)).First();
    }

    public List<string> GetRoutesInfo()
    {
        List<string> res = new List<string>();
        for (int i = 0; i < Container.Routes.Count; i++)
        {
            res.Add("From: X " + Container.Routes.ElementAt(i).First().Coordinates.Item1.x +
                " Y " + Container.Routes.ElementAt(i).First().Coordinates.Item1.y +
                " To X " + Container.Routes.ElementAt(i).Last().Coordinates.Item1.x +
                " Y " + Container.Routes.ElementAt(i).Last().Coordinates.Item1.y);
        }
        return res;
    }

    public void RemoveRoute(Vector2 start, Vector2 end)
    {
        LogisticObject indexS = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(start)).First();
        LogisticObject indexE = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(end)).First();

        List<LogisticObject> route = Container.Routes.Where(r => r.First().Coordinates.Item1.Equals(start) && r.Last().Coordinates.Item1.Equals(end)).First();
        Container.Routes.Remove(route);

    }

    public void TryRemoveRoutesWithBeginEnd(Vector2 node)
    {
        LogisticObject index = Container.Nodes.Where(n => n.Coordinates.Item1.Equals(node)).First();

        for (int i = 0; i < Container.Routes.Count; i++)
        {
            if (Container.Routes[i].First().ID == index.ID || Container.Routes[i].Last().ID == index.ID)
            {
                List<LogisticObject> route = Container.Routes.Where(r => r.First().ID == index.ID || r.Last().ID == index.ID).First();
                Container.Routes.Remove(route);
            }
        }
    }

    #endregion

    #region Impact control

    public void LaunchImpact(Image panel)
    {
        //When Impacting, we can destroy multiple nodes and edges on Logistics map
        //And destroying means the Objects are erased, and pathways are cut in pieces
        int radius = int.Parse(panel.transform.GetComponentsInChildren<Transform>().Where(n => n.name.Contains("PowerInput")).First().gameObject.GetComponentInChildren<Text>().text); ;
        List<Vector2> destroyedNodes = new List<Vector2>();
        List<Pathway> destroyedPathways = new List<Pathway>();
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                try
                {
                    double distance = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0));
                    if (distance <= radius)
                    {
                        string countryName = Container.ComplexMap[(int)Container.selectedCoordinates.x + x, (int)Container.selectedCoordinates.y + y].Country.Name;
                        CountryBase country = Container.countries.Where(c => c.Name.Equals(countryName)).FirstOrDefault();
                        object obj = HasObjectInPlace(new Vector2(Container.selectedCoordinates.x + x, Container.selectedCoordinates.y + y));

                        if (obj != null && obj.GetType().Name.Equals("Pathway"))
                        {
                            //RemoveEdge((obj as Pathway).BeginCoordinates, (obj as Pathway).EndCoordinates);
                            destroyedPathways.Add(obj as Pathway);
                            country.RemovePathway((obj as Pathway).BeginCoordinates, (obj as Pathway).EndCoordinates);
                        }
                        else if (obj != null)
                        {
                            switch (obj.GetType().Name)
                            {
                                case "Facility":
                                    destroyedNodes.Add((obj as Facility).Coords);
                                    country.RemoveFacility((obj as Facility).parentIndustry, (obj as Facility).ID);
                                    break;
                                case "City":
                                    destroyedNodes.Add((obj as City).Coords);
                                    country.RemoveCity((obj as City).Coords, (obj as City).Name);
                                    break;
                                case "Storage":
                                    destroyedNodes.Add((obj as Storage).Coords);
                                    country.RemoveStorage((obj as Storage).ID);
                                    break;
                                case "Hospital":
                                    destroyedNodes.Add((obj as Hospital).Coords);
                                    country.RemoveWarfareContainer((obj as Hospital).CustomName);
                                    break;
                                case "Headquaters":
                                    destroyedNodes.Add((obj as Headquaters).Coords);
                                    country.RemoveWarfareContainer((obj as Headquaters).CustomName);
                                    break;
                                case "Workshop":
                                    destroyedNodes.Add((obj as Workshop).Coords);
                                    country.RemoveWarfareContainer((obj as Workshop).CustomName);
                                    break;
                                case "TrainingPlace":
                                    destroyedNodes.Add((obj as TrainingPlace).Coords);
                                    country.RemoveWarfareContainer((obj as TrainingPlace).CustomName);
                                    break;
                                case "BattlePosition":
                                    destroyedNodes.Add((obj as BattlePosition).Position);
                                    country.RemoveBattlePosition((obj as BattlePosition).Position);
                                    break;
                            }
                        }

                        int index = ((int)Container.selectedCoordinates.y + y) * Container.size + ((int)Container.selectedCoordinates.x + x);
                        Container.economicsColors[index] = Color.white;
                        Container.logisticsColors[index] = Color.white;
                        Container.warfareColors[index] = Color.white;
                    }
                }
                catch (Exception ex) { }
            }
        }

        for (int i = 0; i < destroyedNodes.Count; i++)
        {
            TryRemoveRoutesWithBeginEnd(destroyedNodes[i]);
            RemoveNodeAndEdge(destroyedNodes.ElementAt(i));
        }
        for (int i = 0; i < destroyedPathways.Count; i++)
        {
            RemoveEdge(destroyedPathways[i].BeginCoordinates, destroyedPathways[i].EndCoordinates);
        }
        Container.RemakeTextures();
        ActionPanelClose();
    }

    #endregion
}
