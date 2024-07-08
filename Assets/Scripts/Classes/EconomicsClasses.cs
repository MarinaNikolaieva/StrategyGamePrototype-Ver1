using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilityClasses;
using GlobalClasses;

namespace EconomicsClasses
{
    #region EconomicsClasses

    public class Product : IEquatable<Product>
    {
        public int ID { get; }
        public string Name { get; }
        public float Price { get; set; }
        public int AmountProduced { get; }  //How many products is made with one iter
        public int SecondsToMakeFirst { get; }  //When making the product for the first time, it's longer...
        public int SecondsToMakeAll { get; }  //Than making all the next ones
        public IResource OriginalResource { get; }

        public Product(int ID, string Name, float Price, int AmountProduced, IResource resource, int timerFirst,
            int timerAll)
        {
            this.ID = ID;
            this.Name = Name;
            this.Price = Price;
            this.AmountProduced = AmountProduced;
            OriginalResource = resource;
            SecondsToMakeFirst = timerFirst;
            SecondsToMakeAll = timerAll;
        }

        public void SetPrice(float price)
        {
            if (price >= 0.0)
                Price = price;
        }

        public string GetInfo()
        {
            StringBuilder build = new StringBuilder();
            build.Append("Name: " + Name);
            build.Append("; Price: " + Price);
            build.Append("; Resource: ");
            build.Append(OriginalResource == null ? "Multiple" : OriginalResource.Name.ToString());
            return build.ToString();
        }

        public bool Equals(Product other)
        {
            if (other == null) return false;
            else return this.ID == other.ID && this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            int hashCode = -1675370237;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + AmountProduced.GetHashCode();
            return hashCode;
        }
    }

    public class Storage : IEquatable<Storage>
    {
        public int ID { get; }
        public string Name;
        public Vector2 Coords { get; }
        public int Cost { get; }
        private int Capacity;
        private int FreeCapacity;
        //Here, int is the AMOUNT of products kept in the current storage
        private Dictionary<Product, int> Products;

        public Storage(int id, string name, Vector2 coords, int cost, int capacity)
        {
            ID = id;
            Name = name;
            Coords = coords;
            Cost = cost;
            Capacity = capacity;
            FreeCapacity = capacity;
            Products = new Dictionary<Product, int>();
        }

        public int GetCapacity()
        {
            return Capacity;
        }

        public int SetCapacity(int capacity)
        {
            //If you've got 50 spots, 40 of them occupied, and you want to cut spots to 30 - you can't
            //Free the space to 30 occupied beforehand
            if (capacity > 0 && capacity >= Capacity - FreeCapacity)
            {
                FreeCapacity = capacity - (Capacity - FreeCapacity);
                Capacity = capacity;
                return 0;
            }
            return 1;
        }

        public string GetName()
        {
            return Name;
        }

        public int SetName(string name)
        {
            //If you've got 50 spots, 40 of them occupied, and you want to cut spots to 30 - you can't
            //Free the space to 30 occupied beforehand
            if (string.IsNullOrEmpty(name))
                return 1;

            Name = name;
            return 0;
        }

        public int GetFreeCapacity()
        {
            return FreeCapacity;
        }

        public void IncreaseFreeCapacity(int amount)
        {
            if (FreeCapacity + amount <= Capacity)
                FreeCapacity += amount;
        }

        public void DecreaseFreeCapacity(int amount)
        {
            if (FreeCapacity - amount >= 0)
                FreeCapacity -= amount;
        }

        public Dictionary<Product, int> GetProducts()
        {
            return Products;
        }

        public int GetProductAmount(Product product)
        {
            if (!Products.ContainsKey(product))
                return 0;
            else
                return Products[product];
        }

        public int AddProduct(Product product, int amount)
        {
            if (amount <= FreeCapacity)
            {
                if (!Products.ContainsKey(product))
                {
                    Products.Add(product, amount);
                    DecreaseFreeCapacity(amount);
                }
                else
                {
                    Products[product] = amount;
                    DecreaseFreeCapacity(amount);
                }
                return 0;
            }
            return 1;
        }

        public int RemoveProduct(Product product, int amount)
        {
            if (!Products.ContainsKey(product))
                return 1;
            else if (amount <= Products[product])
            {
                Products[product] -= amount;
                IncreaseFreeCapacity(amount);
                if (Products[product] == 0)
                    Products.Remove(product);
                return 0;
            }
            return 2;
        }

        public string GetInfoShort()
        {
            return "ID: " + ID + "; Name: " + Name + "; Capacity: " + Capacity + ", free: " + FreeCapacity + "; Amount of different products: " +
                Products.Count;
        }

        public List<string> GetInfoDetailed()
        {
            List<string> res = new List<string>();
            res.Add("ID: " + ID + "; Name: " + Name + "; Capacity: " + Capacity + ", free: " + FreeCapacity);
            for (int i = 0; i < Products.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(Products.ElementAt(i).Key.Name);
                build.Append(", init resource: ");
                build.Append(Products.ElementAt(i).Key.OriginalResource == null ? "Multiple" : Products.ElementAt(i).Key.OriginalResource.Name);
                build.Append(", amount contained: ");
                build.Append(Products.ElementAt(i).Value);
                res.Add(build.ToString());
            }
            return res;
        }

        public bool Equals(Storage other)
        {
            return this.ID == other.ID && this.Coords == other.Coords;
        }

        public override int GetHashCode()
        {
            int hashCode = 1497224743;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + Coords.GetHashCode();
            return hashCode;
        }
    }

    public class Industry : IResource, IEquatable<IResource>
    {
        public CountryBase Country { get; }
        public int ID { get; }
        public string Name { get; set; }
        public string Category { get; }
        public int Grade { get; set; }
        public int Points { get; set; }
        public int FreePoints { get; set; }

        //IResource is the resource/industry, int is the quantity
        private Dictionary<IResource, int> Required { get; }  //NO CHANGES, the resources required to begin production
        private Dictionary<IResource, int> Needed { get; }  //NO CHANGES, the resources needed to begin production

        public List<Facility> facilities { get; set; }

        public Industry(int ID, string Name, int Grade, CountryBase country)
        {
            this.ID = ID;
            this.Name = Name;
            this.Grade = Grade;
            Category = "I";
            Points = 0;
            FreePoints = 0;

            Required = new Dictionary<IResource, int>();
            Needed = new Dictionary<IResource, int>();
            facilities = new List<Facility>();

            Country = country;
        }

        public void AddPoints(int points)
        {
            if (points > 0)
            {
                Points += points;
                FreePoints += points;
            }
        }

        public void SubtractPoints(int points)
        {
            if (points <= FreePoints)
            {
                Points -= points;
                FreePoints -= points;
            }
        }

        public void AddFreePoints(int points)
        {
            if (points > 0)
            {
                FreePoints += points;
            }
        }

        public void SubtractFreePoints(int points)
        {
            if (points <= FreePoints)
            {
                FreePoints -= points;
            }
        }

        public Dictionary<IResource, int> GetRequired()
        {
            Dictionary<IResource, int> resources = new Dictionary<IResource, int>();
            foreach (var resource in Required)
            {
                resources.Add(resource.Key, resource.Value);
            }
            return resources;
        }

        public Dictionary<IResource, int> GetNeeded()
        {
            Dictionary<IResource, int> resources = new Dictionary<IResource, int>();
            foreach (var resource in Needed)
            {
                resources.Add(resource.Key, resource.Value);
            }
            return resources;
        }

        public bool HasResource(IResource res)
        {
            return Needed.ContainsKey(res);
        }

        public void AddRequired(IResource resource, int frequency)
        {
            if (!Required.ContainsKey(resource))
            {
                Required.Add(resource, frequency);
            }
        }

        public void AddNeeded(IResource resource, int frequency)
        {
            if (!Needed.ContainsKey(resource))
            {
                Needed.Add(resource, frequency);
            }
        }

        public void SendProductToCountry(Product product, int amount)
        {
            if (product != null)
                Country.ChangeProductAmount(product, amount);
        }

        public void StartProducingProduct(Facility facility, Product product)
        {
            if (facilities.Contains(facility))
            {
                facility.AddProduct(product);
                AddPoints(1);
                //AddFreePoints(1);
            }
        }

        public void StopProducingProduct(Facility facility, Product product)
        {
            if (facilities.Contains(facility))
            {
                if (facility.GetProducts().Contains(product) && facility.GetProductProduction(product) <= FreePoints)
                {
                    SubtractPoints(facility.GetProductProduction(product));
                    //SubtractFreePoints(facility.GetProductProduction(product));
                    facility.RemoveProduct(product);
                }
            }
        }

        public void IncreaseProductProduction(Facility facility, Product product, int amount)
        {
            if (facilities.Contains(facility))
            {
                if (facility.GetProducts().Contains(product))
                {
                    facility.IncreaseProduct(product, amount);
                    AddPoints(amount);
                    //AddFreePoints(amount);
                }
            }
        }

        public void DecreaseProductProduction(Facility facility, Product product, int amount)
        {
            if (facilities.Contains(facility))
            {
                if (facility.GetProducts().Contains(product))
                {
                    facility.DecreaseProduct(product, amount);
                    SubtractPoints(amount);
                    //SubtractFreePoints(amount);
                }
            }
        }

        public List<Product> GetProductWithinFacility(int facilityID)
        {
            List<Product> res = new List<Product>();
            Facility facility = facilities.Where(f => f.ID == facilityID).First();
            for (int i = 0; i < facility.GetProducts().Count; i++)
                res.Add(facility.GetProducts().ElementAt(i));
            return res;
        }

        public Dictionary<Product, int> GetFacilityProduction(Facility facility)
        {
            if (facilities.Contains(facility))
                return facility.GetProduction();
            return null;
        }

        public int GetTotalProductProduction(Product product)
        {
            int sum = 0;
            for (int i = 0; i < facilities.Count; i++)
            {
                if (facilities.ElementAt(i).GetProducts().Contains(product))
                {
                    sum += facilities.ElementAt(i).GetProductProduction(product);
                }
            }
            return sum;
        }

        public void AddFacility(int id, string name, Vector2 coords, int cost)
        {
            if (!facilities.Select(f => f.ID).ToList().Contains(id) &&
                !facilities.Select(f => f.Coords).ToList().Contains(coords))
            {
                facilities.Add(new Facility(id, name, coords, cost, null, this));
                //The empty facility brings the industry no points
            }
        }

        public void RemoveFacility(int id)
        {
            if (facilities.Select(f => f.ID).ToList().Contains(id) && facilities.Where(f => f.ID == id).First().GetTotalProductionPoints() <= FreePoints)
            {
                //Removing the facility removes the points it gave the industry
                SubtractPoints(facilities.Where(f => f.ID == id).First().GetTotalProductionPoints());
                //SubtractFreePoints(facilities.Where(f => f.ID == id).First().GetTotalProductionPoints());
                facilities.Where(f => f.ID == id).First().StopFacility();
                facilities.Remove(facilities.Where(f => f.ID == id).First());
            }
        }

        public string GetInfo()
        {
            return "Name: " + Name + "; Grade: " + Grade.ToString() + "; Points: " + Points.ToString() +
                ", Free: " + FreePoints.ToString();
        }

        public List<string> GetInfoDetailed()
        {
            List<string> res = new List<string>();
            res.Add(Name + "; Grade: " + Grade.ToString() + ";\nPoints: " + Points.ToString() +
                ", Free: " + FreePoints.ToString());
            if (Grade == 1)
                res.Add("\nONE needed resource must fit!");
            else
                res.Add("\nALL needed resources must fit!");
            foreach (IResource item in Needed.Keys)
            {
                res.Add("\nNeeded resource: " + item.Name + " in amount of " + Needed[item]);
            }
            foreach (IResource item in Required.Keys)
            {
                res.Add("\nRequired resource: " + item.Name + " in amount of " + Required[item]);
            }
            return res;
        }

        public List<string> GetInfoAboutFacilities()
        {
            List<string> res = new List<string>();
            res.Add("Name: " + Name + "; Grade: " + Grade.ToString());
            foreach (Facility item in facilities)
                res.Add(item.GetInfoShort());
            return res;
        }

        public bool SameType(IResource other)
        {
            if (this.GetType() == other.GetType())
            {
                return true;
            }
            return false;
        }

        public bool SameName(IResource other)
        {
            if (this.Name.Equals(other.Name))
                return true;
            return false;
        }

        public bool Equals(IResource other)
        {
            if (this is IResource && other is IResource)
                return (this as IResource).SameType(other) && (this as IResource).SameName(other);
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = -203775769;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Category);
            return hashCode;
        }
    }

    public class Facility : IEquatable<Facility>
    {
        public int ID { get; }
        public string Name;
        public Vector2 Coords { get; }
        public int Cost { get; }
        private List<Product> products;
        //Here, int is the multiplier = production POWER increasing the amount of product made
        private Dictionary<Product, int> production;
        private Dictionary<Product, System.Timers.Timer> productTimers { get; set; }
        private TimeSpan delta = new TimeSpan(0, 0, 0, 1);

        public Industry parentIndustry { get; }

        public Facility(int id, string name, Vector2 coords, int cost, List<Product> prods, Industry industry)
        {
            ID = id;
            Name = name;
            Coords = coords;
            Cost = cost;
            products = new List<Product>();
            //products.AddRange(prods);
            production = new Dictionary<Product, int>();
            productTimers = new Dictionary<Product, System.Timers.Timer>();
            parentIndustry = industry;
        }

        void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e, TimeSpan delta, ref TimeSpan main,
            System.Timers.Timer caller, Product origProduct)
        {
            if (main > TimeSpan.Zero)
            {
                main = main.Subtract(delta);
            }
            else
            {
                //The Facility sends the product to Industry, which transfers it to Country
                main = TimeSpan.FromSeconds(origProduct.SecondsToMakeAll);
                parentIndustry.SendProductToCountry(origProduct, origProduct.AmountProduced * production[origProduct]);
            }
        }

        public void InitTimers()
        {
            for (int i = 0; i < products.Count; i++)
            {
                TimeSpan timespan = TimeSpan.FromSeconds(products.ElementAt(i).SecondsToMakeFirst);
                System.Timers.Timer _timer = new System.Timers.Timer();
                _timer.Interval = 1000;
                _timer.AutoReset = true;
                _timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, delta, ref timespan, _timer, products.ElementAt(i));
                productTimers.Add(products.ElementAt(i), _timer);
                _timer.Start();
            }
        }

        public string GetName()
        {
            return Name;
        }

        public int SetName(string name)
        {
            //If you've got 50 spots, 40 of them occupied, and you want to cut spots to 30 - you can't
            //Free the space to 30 occupied beforehand
            if (string.IsNullOrEmpty(name))
                return 1;

            Name = name;
            return 0;
        }

        public List<Product> GetProducts()
        {
            return products;
        }

        public void AddProduct(Product product)
        {
            if (!products.Contains(product))
            {
                products.Add(product);
                production.Add(product, 1);

                TimeSpan timespan = TimeSpan.FromSeconds(product.SecondsToMakeFirst);
                System.Timers.Timer _timer = new System.Timers.Timer();
                _timer.Interval = 1000;
                _timer.AutoReset = true;
                _timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, delta, ref timespan, _timer, product);
                productTimers.Add(product, _timer);
                _timer.Start();
            }
        }

        public void RemoveProduct(Product product)
        {
            if (products.Contains(product))
            {
                products.Remove(product);
                production.Remove(product);

                productTimers[product].Stop();
                productTimers.Remove(product);
            }
        }

        public void IncreaseProduct(Product product, int power)
        {
            if (products.Contains(product))
            {
                if (production[product] == 0)
                    productTimers[product].Start();
                production[product] += power;
            }
        }

        public void DecreaseProduct(Product product, int power)
        {
            if (products.Contains(product))
            {
                production[product] -= power;
                if (production[product] < 0)
                    production[product] = 0;
                if (production[product] == 0)
                    productTimers[product].Stop();
            }
        }

        public void StopFacility()
        {
            for (int i = 0; i < products.Count; i++)
            {
                productTimers.ElementAt(i).Value.Stop();
            }
            products.Clear();
            productTimers.Clear();
            production.Clear();
        }

        public Dictionary<Product, int> GetProduction()
        {
            return production;
        }

        public int GetProductProduction(Product product)
        {
            if (production.ContainsKey(product))
                return production[product];
            return 0;
        }

        public int GetTotalProductionPoints()
        {
            int sum = 0;
            for (int i = 0; i < production.Count; i++)
            {
                sum += production.ElementAt(i).Value;
            }
            return sum;
        }

        public string GetInfoShort()
        {
            return "ID: " + ID + "; Name: " + Name + "; Coords: X: " + Coords.x + ", Y: " + Coords.y + "; Cost: " + Cost;
        }

        public List<string> GetInfoDetailed()
        {
            List<string> res = new List<string>();
            res.Add("ID: " + ID + "; Name: " + Name + "; Coords: X: " + Coords.x + ", Y: " + Coords.y + "; Cost: " + Cost);
            for (int i = 0; i < products.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(products[i].Name);
                build.Append(", init resource: ");
                build.Append(products[i].OriginalResource == null ? "Multiple" : products[i].OriginalResource.Name);
                build.Append(", production multiplier: ");
                build.Append(production.ElementAt(i).Value);
                res.Add(build.ToString());
            }
            return res;
        }

        public bool Equals(Facility other)
        {
            return this.ID == other.ID && this.Coords == other.Coords;
        }

        public override int GetHashCode()
        {
            int hashCode = 1497224743;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + Coords.GetHashCode();
            return hashCode;
        }
    }

    #endregion
}
