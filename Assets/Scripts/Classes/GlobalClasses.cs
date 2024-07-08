using EconomicsClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilityClasses;
using LogisticsClasses;
using WarfareClasses;
using Readers;
using WarfareMechanics;
using System.Xml.Linq;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;

namespace GlobalClasses
{
    public class CountryBase : IEquatable<CountryBase>
    {
        public string Name { get; }
        public Color Color { get; }
        public Vector2 Capital { get; set; }
        private Dictionary<IResource, bool> resourceAvailability { get; set; }
        public Dictionary<IResource, int> resourceAmmounts { get; set; }
        public Dictionary<Product, int> productAmounts { get; set; }
        public Dictionary<Product, int> productMultipliers { get; set; }
        public Dictionary<Product, Industry> productIndustriesDependency { get; set; }
        public List<Industry> industriesPoints { get; set; }
        public List<Storage> storages { get; set; }
        public List<PathwayType> pathwayTypes { get; set; }
        public List<Pathway> pathways { get; set; }
        public List<City> cities { get; set; }

        public List<ContainerObject> warfareContainerObjects { get; set; }
        public List<BattlePosition> battlePositions { get; set; }
        public ObjectBank objectBank { get; set; }

        private static object Lock = new object();

        public int lastStorageID;
        public int lastFacilityID;
        public int lastWarfareObjID;
        public int lastBattlePositionID;


        public CountryBase(string Name, Color Color)
        {
            this.Name = Name;
            this.Color = Color;
            Capital = new Vector2(-1, -1);
            resourceAvailability = new Dictionary<IResource, bool>();
            resourceAmmounts = new Dictionary<IResource, int>();
            productAmounts = new Dictionary<Product, int>();
            productMultipliers = new Dictionary<Product, int>();
            productIndustriesDependency = new Dictionary<Product, Industry>();
            industriesPoints = new List<Industry>();
            storages = new List<Storage>();
            pathwayTypes = new List<PathwayType>();
            pathways = new List<Pathway>();
            cities = new List<City>();
            warfareContainerObjects = new List<ContainerObject>();
            battlePositions = new List<BattlePosition>();
            objectBank = new ObjectBank();
            lastStorageID = 0;
            lastFacilityID = 0;
            lastWarfareObjID = 0;
            lastBattlePositionID = 0;
        }

        public void InitResources(ResourceReader resourceReader)
        {

            for (int i = 0; i < resourceReader.getResources().Count; i++)
            {
                resourceAvailability.Add(resourceReader.getResources()[i], false);
                resourceAmmounts.Add(resourceReader.getResources()[i], 0);
            }
        }

        public void InitWarfareObjects(WarfareObjectsReader warfareObjectsReader)
        {
            objectBank.InitBank(warfareObjectsReader.GetWarfareObjects());
            objectBank.InitDependencies(warfareObjectsReader.GetWarfareDependencies());
            objectBank.InitTimes(warfareObjectsReader.GetCreateTimeValues());
        }

        public void AddIndustry(Industry industry)
        {
            if (!industriesPoints.Contains(industry))
                industriesPoints.Add(industry);
        }

        public void AddPathwayType(PathwayType type)
        {
            if (!pathwayTypes.Contains(type))
                pathwayTypes.Add(type);
        }

        public void SetPathwayTypes(List<PathwayType> types)
        {
            foreach (PathwayType type in types)
            {
                if (!pathwayTypes.Contains(type))
                    pathwayTypes.Add(type);
            }
        }

        public object IsObjectInPlace(Vector2 coords)
        {
            //First check all storages
            object obj = storages.Where(s => s.Coords == coords).FirstOrDefault();
            if (obj != null)
                return obj;  //Found a storage
            //Else - failed, check all cities
            obj = cities.Where(c => c.Coords == coords).FirstOrDefault();
            if (obj != null)
                return obj;  //Found a city
            //Else - failed, go to the longest part - check all industries and their facilities
            for (int i = 0; i < industriesPoints.Count; i++)
            {
                obj = industriesPoints[i].facilities.Where(f => f.Coords == coords).FirstOrDefault();
                if (obj != null)
                    return obj;  //Found a facility
            }
            //Else - failed, check warfare objects
            obj = warfareContainerObjects.Where(c => c.Coords == coords).FirstOrDefault();
            if (obj != null)
                return obj;  //Found a warfare container - hospital / workshop / headquaters
            //Else - failed, check prsent battle positions
            obj = battlePositions.Where(p => p.Position == coords).FirstOrDefault();
            if (obj != null)
                return obj;  //Found a battle position
            //Else - failed, check the pathways
            obj = pathways.Where(p => p.Parts.Contains(coords)).FirstOrDefault();
            if (obj != null)
                return obj;  //Found a pathway

            //And if everything failed, return null
            return null;
        }

        #region Production management

        public void AddNewProduct(Product product, string industryName)
        {
            Industry industry = industriesPoints.Where(ind => ind.Name.Equals(industryName)).FirstOrDefault();
            if (industry != null)
            {
                productAmounts.Add(product, 0);
                productMultipliers.Add(product, 0);
                productIndustriesDependency.Add(product, industry);
            }
        }

        //This will be the function for both increasing and decreasing!
        public void ChangeProductAmount(Product product, int amount)
        {
            if (productAmounts.ContainsKey(product))
                lock (Lock)
                {
                    productAmounts[product] += amount;
                    if (objectBank.Bank.Keys.Where(p => p.Name == product.Name).FirstOrDefault() != null)
                    {
                        objectBank.Bank[objectBank.Bank.Keys.Where(p => p.Name == product.Name).First()] += amount;
                    }
                }
        }

        public void StartProducingProduct(Facility facility, Product product)
        {
            if (productAmounts.ContainsKey(product))
            {
                if (facility.parentIndustry.Grade == 1)
                {
                    resourceAmmounts[product.OriginalResource] -= facility.parentIndustry.GetNeeded()[product.OriginalResource];
                }
                else
                {
                    for (int i = 0; i < facility.parentIndustry.GetNeeded().Count; i++)
                    {
                        if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is BasicResource)
                            resourceAmmounts[facility.parentIndustry.GetNeeded().ElementAt(i).Key] -= facility.parentIndustry.GetNeeded().ElementAt(i).Value;
                        else if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is Industry)
                            industriesPoints.Where(ind => ind.Equals(facility.parentIndustry.GetNeeded().ElementAt(i).Key)).First().SubtractFreePoints(facility.parentIndustry.GetNeeded().ElementAt(i).Value);
                    }
                }
                for (int i = 0; i < facility.parentIndustry.GetRequired().Count; i++)
                    resourceAmmounts[facility.parentIndustry.GetRequired().ElementAt(i).Key] -= facility.parentIndustry.GetRequired().ElementAt(i).Value;
                industriesPoints.Where(i => facility.parentIndustry == i).First().StartProducingProduct(facility, product);
                productMultipliers[product]++;
            }
        }

        public void StopProducingProduct(Facility facility, Product product)
        {
            if (productAmounts.ContainsKey(product))
            {
                productMultipliers[product] -= industriesPoints.Where(i => facility.parentIndustry == i).First().facilities.Where(f => f.Equals(facility)).First().GetProductProduction(product);
                industriesPoints.Where(i => facility.parentIndustry == i).First().StopProducingProduct(facility, product);
            }
        }

        public void StopProducingProductGlobally(Product product)
        {
            if (productAmounts.ContainsKey(product))
            {
                for (int i = 0; i < industriesPoints.Count; i++)
                {
                    if (industriesPoints.ElementAt(i).GetTotalProductProduction(product) > 0)
                    {
                        for (int j = 0; j < industriesPoints.ElementAt(i).facilities.Count; j++)
                        {
                            industriesPoints.ElementAt(i).StopProducingProduct(industriesPoints.ElementAt(i).facilities[j], product);
                        }
                    }
                }
            }
        }

        public void IncreaseProductProduction(Facility facility, Product product)
        {
            int hasEnoughResources = 0;
            int hasEnoughRequiredResources = 0;
            int amountOfNeeded = facility.parentIndustry.GetNeeded().Count;
            int amountOfRequired = facility.parentIndustry.GetRequired().Count;
            //With this, I now make the Grade 2 and 3 industries, where both Needed and Required are to be present
            //NEEDED add the Grade 1 industries, where there must be enough resources for the product desired ONLY  DONE
            if (facility.parentIndustry.Grade == 1)
            {
                if (resourceAmmounts[product.OriginalResource] >= facility.parentIndustry.GetNeeded()[product.OriginalResource])
                    hasEnoughResources++;
            }
            else
            {
                for (int i = 0; i < facility.parentIndustry.GetNeeded().Count; i++)
                {
                    if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is BasicResource)
                        if (resourceAmmounts[facility.parentIndustry.GetNeeded().ElementAt(i).Key] >= facility.parentIndustry.GetNeeded().ElementAt(i).Value)
                            hasEnoughResources++;
                        else if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is Industry)
                            if (industriesPoints.Where(ind => ind.Equals(facility.parentIndustry.GetNeeded().ElementAt(i).Key)).First().FreePoints >= facility.parentIndustry.GetNeeded().ElementAt(i).Value)
                                hasEnoughResources++;
                }
            }
            for (int i = 0; i < facility.parentIndustry.GetRequired().Count; i++)
                if (resourceAmmounts[facility.parentIndustry.GetRequired().ElementAt(i).Key] >= facility.parentIndustry.GetRequired().ElementAt(i).Value)
                    hasEnoughRequiredResources++;

            if (hasEnoughRequiredResources == amountOfRequired)
            {
                if (facility.parentIndustry.Grade == 1)
                {
                    resourceAmmounts[product.OriginalResource] -= facility.parentIndustry.GetNeeded()[product.OriginalResource];
                }
                else
                {
                    for (int i = 0; i < facility.parentIndustry.GetNeeded().Count; i++)
                    {
                        if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is BasicResource)
                            resourceAmmounts[facility.parentIndustry.GetNeeded().ElementAt(i).Key] -= facility.parentIndustry.GetNeeded().ElementAt(i).Value;
                        else if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is Industry)
                            industriesPoints.Where(ind => ind.Equals(facility.parentIndustry.GetNeeded().ElementAt(i).Key)).First().SubtractFreePoints(facility.parentIndustry.GetNeeded().ElementAt(i).Value);
                    }
                }
                for (int i = 0; i < facility.parentIndustry.GetRequired().Count; i++)
                    resourceAmmounts[facility.parentIndustry.GetRequired().ElementAt(i).Key] -= facility.parentIndustry.GetRequired().ElementAt(i).Value;
                industriesPoints.Where(i => facility.parentIndustry == i).First().IncreaseProductProduction(facility, product, 1);
                productMultipliers[product]++;
            }
        }

        public void DecreaseProductProduction(Facility facility, Product product)
        {
            //For Grade 1 industries, ADD THE PRODUCT RESOURCE DEPENDENCY TOO  DONE
            if (facility.parentIndustry.Grade == 1)
            {
                resourceAmmounts[product.OriginalResource] += facility.parentIndustry.GetNeeded()[product.OriginalResource];
            }
            else
            {
                for (int i = 0; i < facility.parentIndustry.GetNeeded().Count; i++)
                {
                    if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is BasicResource)
                        resourceAmmounts[facility.parentIndustry.GetNeeded().ElementAt(i).Key] += facility.parentIndustry.GetNeeded().ElementAt(i).Value;
                    else if (facility.parentIndustry.GetNeeded().ElementAt(i).Key is Industry)
                        industriesPoints.Where(ind => ind.Equals(facility.parentIndustry.GetNeeded().ElementAt(i).Key)).First().AddFreePoints(facility.parentIndustry.GetNeeded().ElementAt(i).Value);

                    //resourceAmmounts[facility.parentIndustry.GetNeeded().ElementAt(i).Key] += facility.parentIndustry.GetNeeded().ElementAt(i).Value;
                }
            }
            for (int i = 0; i < facility.parentIndustry.GetRequired().Count; i++)
                resourceAmmounts[facility.parentIndustry.GetRequired().ElementAt(i).Key] += facility.parentIndustry.GetRequired().ElementAt(i).Value;
            industriesPoints.Where(i => facility.parentIndustry == i).First().DecreaseProductProduction(facility, product, 1);
            productMultipliers[product]--;
        }

        public void ChangeProductPrice(Product product, int price)
        {
            productAmounts.Where(p => p.Key.Equals(product)).First().Key.SetPrice(price);
        }

        #endregion

        #region Facility management
        public void AddFacility(Industry industry, int id, string name, Vector2 coords, int cost)
        {
            industriesPoints.Where(i => i == industry).First().AddFacility(id, name, coords, cost);
        }

        public void RemoveFacility(Industry industry, int id)
        {
            Facility facil = industry.facilities.Where(i => i.ID == id).First();
            for (int j = 0; j < facil.GetProduction().Count; j++)
            {
                if (facil.parentIndustry.Grade == 1)
                {
                    resourceAmmounts[facil.GetProduction().ElementAt(j).Key.OriginalResource] += facil.parentIndustry.GetNeeded()[facil.GetProduction().ElementAt(j).Key.OriginalResource] * facil.GetProduction().ElementAt(j).Value;
                }
                else
                {
                    for (int i = 0; i < facil.parentIndustry.GetNeeded().Count; i++)
                    {
                        if (facil.parentIndustry.GetNeeded().ElementAt(i).Key is BasicResource)
                            resourceAmmounts[facil.parentIndustry.GetNeeded().ElementAt(i).Key] += facil.parentIndustry.GetNeeded().ElementAt(i).Value * facil.GetProduction().ElementAt(j).Value;
                        else if (facil.parentIndustry.GetNeeded().ElementAt(i).Key is Industry)
                            industriesPoints.Where(ind => ind.Equals(facil.parentIndustry.GetNeeded().ElementAt(i).Key)).First().AddFreePoints(facil.parentIndustry.GetNeeded().ElementAt(i).Value * facil.GetProduction().ElementAt(j).Value);

                    }
                }
                for (int i = 0; i < facil.parentIndustry.GetRequired().Count; i++)
                    resourceAmmounts[facil.parentIndustry.GetRequired().ElementAt(i).Key] += facil.parentIndustry.GetRequired().ElementAt(i).Value;

                productMultipliers[facil.GetProduction().ElementAt(j).Key] -= facil.GetProduction().ElementAt(j).Value;
            }
            industry.RemoveFacility(id);
        }

        public string GetIndustryOfFacility(int facilityID)
        {
            for (int i = 0; i < industriesPoints.Count; i++)
            {
                if (industriesPoints.ElementAt(i).facilities.Select(f => f.ID).ToList().Contains(facilityID))
                    return industriesPoints.ElementAt(i).Name;
            }
            return "";
        }

        public List<Product> GetProductsOfFacility(int facilityID)
        {
            for (int i = 0; i < industriesPoints.Count; i++)
            {
                Facility facility = industriesPoints.ElementAt(i).facilities.Where(f => f.ID == facilityID).FirstOrDefault();
                if (facility != null)
                    return industriesPoints.ElementAt(i).GetProductWithinFacility(facilityID);
            }
            return null;
        }

        #endregion

        #region Resource management
        //Both Increasing and Decreasing, if negative
        public void ChangeResourceAmount(IResource resource, int amount)
        {
            if (resourceAmmounts.ContainsKey(resource))
            {
                resourceAmmounts[resource] += amount;
                if (resourceAmmounts[resource] > 0)
                    resourceAvailability[resource] = true;
                else
                    resourceAvailability[resource] = false;
            }
            else if (amount > 0)
            {
                resourceAmmounts.Add(resource, amount);
                resourceAvailability.Add(resource, true);

                resourceAmmounts = resourceAmmounts.OrderBy(r => r.Key.Category).ToDictionary(r => r.Key, r => r.Value);
                resourceAvailability = resourceAvailability.OrderBy(r => r.Key.Category).ToDictionary(r => r.Key, r => r.Value);
            }
        }

        public void UnblockResource(IResource resource)
        {
            if (resourceAmmounts.ContainsKey(resource) && !resourceAvailability[resource])
                resourceAvailability[resource] = true;
        }
        //Do I need to block the resources as well?
        #endregion

        #region Storage management
        public void AddStorage(int id, string name, Vector2 coords, int cost, int capacity)
        {
            storages.Add(new Storage(id, name, coords, cost, capacity));
        }

        public bool RemoveStorage(int id)
        {
            Storage storage = storages.Where(s => s.ID == id).First();
            if (storage != null)
            {
                if (storage.GetFreeCapacity() != storage.GetCapacity())
                {
                    foreach (var product in storage.GetProducts())
                    {
                        productAmounts[product.Key] -= product.Value;
                    }
                    storages.Remove(storage);
                    return false;
                }
                else
                {
                    storages.Remove(storage);
                    return true;
                }
            }
            return false;
        }

        public int SetStorageCapacity(int id, int capacity)
        {
            Storage storage = storages.Where(s => s.ID == id).First();
            if (storage != null)
            {
                return storage.SetCapacity(capacity);
            }
            return 2;
        }

        public Dictionary<Product, int> GetProductsInStorage(Storage storage)
        {
            return storages.Where(s => s == storage).First().GetProducts();
        }

        public int GetProductAmountInStorage(Storage storage, Product product)
        {
            return storages.Where(s => s == storage).First().GetProductAmount(product);
        }

        public int AddProductAmountToStorage(Storage storage, Product product, int amount)
        {
            return storages.Where(s => s == storage).First().AddProduct(product, amount);
        }

        public int RemoveProductAmountFromStorage(Storage storage, Product product, int amount)
        {
            return storages.Where(s => s == storage).First().RemoveProduct(product, amount);
        }

        #endregion

        #region Pathway management
        public void AddPathway(Vector2 start, Vector2 end, PathwayType type)
        {
            Pathway pathway = new Pathway(start, end, type);
            if (!pathways.Contains(pathway))
                pathways.Add(pathway);
        }

        public void RemovePathway(Vector2 start, Vector2 end)
        {
            pathways.Remove(pathways.Where(p => p.BeginCoordinates == start && p.EndCoordinates == end).First());
        }

        public void ChangePathwayTypePrice(PathwayType type, int price)
        {
            pathwayTypes.Where(t => t == type).First().SetPrice(price);
        }

        public void ChangePathwayType(Vector2 start, Vector2 end, PathwayType type)
        {
            pathways.Where(p => p.BeginCoordinates == start && p.EndCoordinates == end).First().Type = type;
        }

        #endregion

        #region City management

        public void AddCity(Vector2 coords, string name)
        {
            City city = new City(coords, name);
            if (!cities.Contains(city))
                cities.Add(city);
        }

        public void RemoveCity(Vector2 coords, string name)
        {
            cities.Remove(cities.Where(c => c.Name.Equals(name) && c.Coords == coords).First());
        }

        public void RenameCity(Vector2 coords, string newName)
        {
            cities.Where(c => c.Coords == coords).First().Name = newName;
        }

        public string GetCityByCoords(Vector2 coords)
        {
            return cities.Where(c => c.Coords == coords).First().Name;
        }

        #endregion

        #region Warfare containers and positions management
        
        public void AddWarfareContainer(string marker, string name, Vector2 coords, int capacity)
        {
            switch (marker)
            {
                case "HP":
                    warfareContainerObjects.Add(new Hospital(objectBank, name, capacity, coords));
                    (warfareContainerObjects.Last() as Hospital).Heal();
                    break;
                case "HQ":
                    warfareContainerObjects.Add(new Headquaters(objectBank, name, capacity, coords));
                    break;
                case "W":
                    warfareContainerObjects.Add(new Workshop(objectBank, name, capacity, coords));
                    (warfareContainerObjects.Last() as Workshop).Fix();
                    break;
                case "TP":
                    warfareContainerObjects.Add(new TrainingPlace(objectBank, name, capacity, coords));
                    break;
            }
        }

        public bool RemoveWarfareContainer(string name)
        {
            ContainerObject container = warfareContainerObjects.Where(o => o.CustomName == name).FirstOrDefault();
            
            if (container != null)
            {
                if (container.CurrentCapacity != container.Capacity)
                {
                    switch (container.GetType().Name)
                    {
                        case "Hospital":
                            (container as Hospital).StopHealingAction();
                            foreach (var obj in (container as Hospital).Fixed)
                            {
                                objectBank.Bank[obj.Key] += obj.Value.Item3;
                            }
                            warfareContainerObjects.Remove(container);
                            break;
                        case "Headquaters":
                            foreach (var obj in (container as Headquaters).Contained)
                            {
                                if (obj.Key.Resource == 100F)
                                {
                                    objectBank.Bank[obj.Key] += obj.Value.Item3;
                                    if (productAmounts.Keys.Where(p => p.Name == obj.Key.Name).FirstOrDefault() != null)
                                    {
                                        productAmounts[productAmounts.Keys.Where(p => p.Name == obj.Key.Name).First()] += obj.Value.Item3;
                                    }
                                }
                            }
                            warfareContainerObjects.Remove(container);
                            break;
                        case "Workshop":
                            (container as Workshop).StopFixingAction();
                            foreach (var obj in (container as Workshop).Fixed)
                            {
                                objectBank.Bank[obj.Key] += obj.Value.Item3;
                                if (productAmounts.Keys.Where(p => p.Name == obj.Key.Name).FirstOrDefault() != null)
                                {
                                    productAmounts[productAmounts.Keys.Where(p => p.Name == obj.Key.Name).First()] += obj.Value.Item3;
                                }
                            }
                            warfareContainerObjects.Remove(container);
                            break;
                        case "TrainingPlace":
                            (container as TrainingPlace).StopTraining();
                            warfareContainerObjects.Remove(container);
                            break;
                    }
                    return false;
                }
                else
                {
                    if (container is Hospital)
                        (container as Hospital).StopHealingAction();
                    else if (container is Workshop)
                        (container as Workshop).StopFixingAction();
                    else if (container is TrainingPlace)
                        (container as TrainingPlace).StopTraining();
                    warfareContainerObjects.Remove(container);
                    return true;
                }
            }
            return false;
        }

        public int SetWarfareContainerCapacity(string marker, string name, int capacity)
        {
            ContainerObject container = warfareContainerObjects.Where(o => o.CustomName == name).FirstOrDefault();

            if (container != null)
            {
                switch (marker)
                {
                    case "HP":
                        (container as Hospital).SetCapacity(capacity);
                        break;
                    case "HQ":
                        (container as Headquaters).SetCapacity(capacity);
                        break;
                    case "W":
                        (container as Workshop).SetCapacity(capacity);
                        break;
                }
            }
            return 0;
        }

        public Dictionary<IObject, Tuple<bool, double, int>> GetFixedUnitsInWarfareContainer(ContainerObject container)
        {
            switch (container.GetType().Name)
            {
                case "Hospital":
                    return (warfareContainerObjects.Where(c => c == container).First() as Hospital).Fixed;
                case "Headquaters":
                    return (warfareContainerObjects.Where(c => c == container).First() as Headquaters).Contained;
                case "Workshop":
                    return (warfareContainerObjects.Where(c => c == container).First() as Workshop).Fixed;
                default: 
                    return null;
            }
        }

        public int GetUnitAmountInWarfareContainer(ContainerObject container, IObject unit)
        {
            switch (container.GetType().Name)
            {
                case "Hospital":
                    return (warfareContainerObjects.Where(c => c == container).First() as Hospital).Fixed.Where(u => u.Key == unit).FirstOrDefault().Value.Item3;
                case "Headquaters":
                    return (warfareContainerObjects.Where(c => c == container).First() as Headquaters).Contained.Where(u => u.Key == unit).FirstOrDefault().Value.Item3;
                case "Workshop":
                    return (warfareContainerObjects.Where(c => c == container).First() as Workshop).Fixed.Where(u => u.Key == unit).FirstOrDefault().Value.Item3;
                default:
                    return 0;
            }
        }

        public void AddUnitAmountToWarfareContainer(ContainerObject container, IObject unit, int amount)
        {
            switch (container.GetType().Name)
            {
                case "Hospital":
                    (warfareContainerObjects.Where(c => c == container).First() as Hospital).TryAddObject(unit, 100.0, amount, true);
                    break;
                case "Headquaters":
                    (warfareContainerObjects.Where(c => c == container).First() as Headquaters).TryAddObject(unit, amount, true);
                    break;
                case "Workshop":
                    (warfareContainerObjects.Where(c => c == container).First() as Workshop).TryAddObject(unit, 100.0, amount, true);
                    break;

            }
        }

        public void RemoveUnitAmountFromWarfareContainer(ContainerObject container, IObject unit, int amount)
        {
            switch (container.GetType().Name)
            {
                case "Hospital":
                    (warfareContainerObjects.Where(c => c == container).First() as Hospital).TryRemoveObject(unit, amount, true);
                    break;
                case "Headquaters":
                    (warfareContainerObjects.Where(c => c == container).First() as Headquaters).TryRemoveObject(unit, amount, true);
                    break;
                case "Workshop":
                    (warfareContainerObjects.Where(c => c == container).First() as Workshop).TryRemoveObject(unit, amount, true);
                    break;

            }
        }

        public void AddBattlePosition(Vector2 coords)
        {
            battlePositions.Add(new BattlePosition((int)coords.x, (int)coords.y));
        }

        public void RemoveBattlePosition(Vector2 coords)
        {
            BattlePosition position = battlePositions.Where(o => o.Position == coords).FirstOrDefault();

            if (position != null)
            {
                position.StopPressure();
                foreach (var obj in position.HumanAndTechResources)
                {
                    if (obj.Key.Resource == 100F)
                    {
                        objectBank.Bank[obj.Key] += obj.Value.Item3;
                        if (productAmounts.Keys.Where(p => p.Name == obj.Key.Name).FirstOrDefault() != null)
                        {
                            productAmounts[productAmounts.Keys.Where(p => p.Name == obj.Key.Name).First()] += obj.Value.Item3;
                        }
                    }
                }
                battlePositions.Remove(position);
            }
        }

        public void AddUnitAmountToBattlePosition(Vector2 coords, IObject unit, int amount)
        {
            battlePositions.Where(b => b.Position == coords).First().TryAddResource(unit, amount);
            objectBank.Bank[unit] -= amount;
        }

        public void RemoveUnitAmountFromBattlePosition(Vector2 coords, IObject unit, int amount)
        {
            battlePositions.Where(b => b.Position == coords).First().TryRemoveResource(unit, amount);
            objectBank.Bank[unit] += amount;
        }

        public void TransferUnitAmountFromPositionToContainer(Vector2 positionCoords, object container, IObject unit, int amount, double resource)
        {
            battlePositions.Where(b => b.Position == positionCoords).First().TryRemoveTemp(unit, amount);
            if (container is BattlePosition)
            {
                battlePositions.Where(b => b.Position == (container as BattlePosition).Position).First().TryAddTemp(unit, resource, amount);
            }
            else
            {
                switch ((container as ContainerObject).GetType().Name)
                {
                    case "Hospital":
                        (warfareContainerObjects.Where(c => c == container).First() as Hospital).TryAddObject(unit, resource, amount, false);
                        break;
                    case "Headquaters":
                        (warfareContainerObjects.Where(c => c == container).First() as Headquaters).TryAddObject(unit, amount, false);
                        break;
                    case "Workshop":
                        (warfareContainerObjects.Where(c => c == container).First() as Workshop).TryAddObject(unit, resource, amount, false);
                        break;

                }
            }
        }

        #endregion

        #region Output functions

        public List<string> GetAllResources()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < resourceAmmounts.Count; i++)
            {
                res.Add("Resource: " + resourceAmmounts.ElementAt(i).Key.Name + "; Available: " +
                    resourceAvailability.ElementAt(i).Value.ToString() + "; Amount (free): " +
                    resourceAmmounts.ElementAt(i).Value);
            }
            return res;
        }

        public List<string> GetAllProducts()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < productAmounts.Count; i++)
            {
                int id = productAmounts.ElementAt(i).Key.ID;
                string name = productAmounts.ElementAt(i).Key.Name;
                string resu = productAmounts.ElementAt(i).Key.OriginalResource == null ? "Multiple" : productAmounts.ElementAt(i).Key.OriginalResource.Name;
                int mult = productMultipliers.ElementAt(i).Value;
                int amount = productAmounts.ElementAt(i).Value;
                res.Add("ID: " + id + "; Name: " + name + "; Original resource: " + resu +
                    "; Amount available: " + amount + "; Total production multiplier: " + mult);
            }
            return res;
        }

        public List<string> GetAllIndustries()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < industriesPoints.Count; i++)
            {
                res.Add(industriesPoints.ElementAt(i).GetInfo());
            }
            return res;
        }

        public List<string> GetIndustriesByResource(string resourceName)
        {
            List<string> res = new List<string>();
            IResource resource = resourceAmmounts.Keys.Where(k => k.Name.Equals(resourceName)).First();
            for (int i = 0; i < industriesPoints.Count; i++)
            {
                if (industriesPoints.ElementAt(i).GetNeeded().ContainsKey(resource))
                    res.Add(industriesPoints.ElementAt(i).Name);
            }
            return res;
        }

        public List<string> GetIndustryWithProducts(string industryName)
        {
            List<string> res = new List<string>();
            Industry industry = industriesPoints.Where(i => i.Name.Equals(industryName)).First();
            res.Add("Name: " + Name + "; Grade: " + industry.Grade.ToString());
            res.Add("Products:");
            foreach (var item in productIndustriesDependency)
                if (item.Value.Equals(industry))
                    res.Add(item.Key.GetInfo());
            return res;
        }

        public List<string> GetProductsOfIndustry(string industryName)
        {
            List<string> res = new List<string>();
            Industry industry = industriesPoints.Where(i => i.Name.Equals(industryName)).First();
            foreach (var item in productIndustriesDependency)
                if (item.Value.Equals(industry))
                    res.Add(item.Key.Name);
            return res;
        }

        public List<string> GetAllFacilitiesInfo()
        {
            List<string> res = new List<string>();
            foreach (Industry industry in industriesPoints)
            {
                foreach (Facility facility in industry.facilities)
                    res.Add(facility.GetInfoShort());
            }
            return res;
        }

        public List<string> GetStorages()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < storages.Count; i++)
            {
                res.Add(storages.ElementAt(i).GetInfoShort());
            }
            return res;
        }

        public List<string> GetPathwayTypes()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < pathwayTypes.Count; i++)
            {
                res.Add(pathwayTypes.ElementAt(i).GetInfo());
            }
            return res;
        }

        public List<string> GetPathways()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < pathways.Count; i++)
            {
                res.Add(pathways.ElementAt(i).GetInfo());
            }
            return res;
        }

        public List<string> GetCities()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < cities.Count; i++)
            {
                res.Add(cities.ElementAt(i).GetInfo());
            }
            return res;
        }

        #endregion

        public bool Equals(CountryBase other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }

        public string Output()
        {
            StringBuilder build = new StringBuilder();
            build.Append(Name);
            build.Append(";");
            build.Append("#" + ColorUtility.ToHtmlStringRGB(Color));
            build.Append(";");
            build.Append("Capital: X:");
            build.Append(Capital.x);
            build.Append(";Y:");
            build.Append(Capital.y);
            return build.ToString();
        }

        public override int GetHashCode()
        {
            int hashCode = 97887982;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }
    }

    public class MapPart : IEquatable<MapPart>
    {
        public int ID = -1;  //These will be UNIQUE! The other parameters may be the same
        public int X = -1;
        public int Y = -1;
        public int Height = 0;
        public TemperatureObject TemperatureFinal = null;
        public TemperatureObject TemperatureBasic = null;
        public MoistureObject Moisture = null;
        public Biome Biome = null;
        public bool NoHeightSpecific = false;
        public Soil Soil = null;
        public List<IResource> Resources = new List<IResource>();
        public CountryBase Country = null;
        public List<MapPart> neighbors = new List<MapPart>();
        public List<int> neighborIDs = new List<int>();

        public MapPart(int ID)
        {
            this.ID = ID;
        }

        public string Output()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ID + "\t");
            stringBuilder.Append(X + "\t");
            stringBuilder.Append(Y + "\t");
            stringBuilder.Append(Height + "\t");
            stringBuilder.Append(TemperatureFinal.Name + "\t");
            stringBuilder.Append(Moisture.Name + "\t");
            stringBuilder.Append(Biome.getName() + "\t");
            stringBuilder.Append(Soil.getName() + "\t");
            if (Resources.Count == 0)
                stringBuilder.Append("Empty");
            for (int i = 0; i < Resources.Count; i++)
            {
                stringBuilder.Append(Resources.ElementAt(i).Name);
                if (i != Resources.Count - 1)
                    stringBuilder.Append(",");
            }
            stringBuilder.Append("\t");
            if (Country == null)
                stringBuilder.Append("Empty\t");
            else
                stringBuilder.Append(Country.Name + "\t");
            for (int i = 0; i < neighborIDs.Count; i++)
            {
                stringBuilder.Append(neighborIDs.ElementAt(i));
                if (i != neighborIDs.Count - 1)
                    stringBuilder.Append(",");
            }
            return stringBuilder.ToString();
        }

        public bool Equals(MapPart other)
        {
            if (other == null) return false;
            return ID == other.ID;
        }

        public override int GetHashCode()
        {
            int hashCode = -1829227213;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
