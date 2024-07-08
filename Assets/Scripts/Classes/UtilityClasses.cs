using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityClasses
{
    public class BasicResourceComparer : IEqualityComparer<BasicResource>
    {
        public bool Equals(BasicResource x, BasicResource y)
        {
            return x.Name == y.Name && x.Category.Equals(y.Category) && x.GetType() == y.GetType();
        }

        public int GetHashCode(BasicResource obj)
        {
            if (obj == null) return 0;

            return obj.Name.GetHashCode() + obj.Category.GetHashCode();//Or whatever way to get hash code
        }
    }

    public interface IResource : IEquatable<IResource>
    {
        string Name { get; }  //This is a PROPERTY, not a field
        string Category { get; }  //Same here

        string GetInfo();

        bool SameType(IResource other);

        bool SameName(IResource other);

        bool Equals(IResource other);
    }

    public class BasicResource : IResource, IEquatable<IResource>
    {
        private int ID { get; }
        public string Name { get; }  //Implementing the property
        public string Category { get; }  //Same here
        public Color Color { get; }

        public BasicResource(int ID, string Name, Color color, string Category)
        {
            this.ID = ID;
            this.Name = Name;
            this.Color = color;
            this.Category = Category;
        }

        public BasicResource(BasicResource other)  //Sort of a copy constructor
        {
            this.ID = other.getID();
            this.Name = other.getName();
            this.Color = other.getColor();
        }

        public int getID()
        {
            return ID;
        }

        public string getName()
        {
            return Name;
        }

        public Color getColor()
        {
            return Color;
        }

        public string GetInfo()
        {
            return "Name: " + Name + "; Category: " + Category;
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

    public class Soil : IEquatable<Soil>
    {
        public string Name { get; }
        public Color Color { get; }
        public int Price { get; set; }

        public Soil(string name, Color color, int price)
        {
            Name = name;
            Color = color;
            Price = price;
        }

        public string getName()
        {
            return Name;
        }

        public Color getColor()
        {
            return Color;
        }

        public int getPrice()
        {
            return Price;
        }

        public bool Equals(Soil other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 97887982;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }
    }

    public class HeightObject
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public float TemperatureCoeffitient { get; set; }
        public float MoistureCoeffitient { get; set; }

        public HeightObject(string name, Color color, float temperatureCoeffitient, float moistureCoeffitient)
        {
            Name = name;
            Color = color;
            TemperatureCoeffitient = temperatureCoeffitient;
            MoistureCoeffitient = moistureCoeffitient;
        }
    }

    public class TemperatureObject : IEquatable<TemperatureObject>
    {
        public string Name { get; }
        public Color Color { get; }

        public TemperatureObject(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public bool Equals(TemperatureObject other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 97887982;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }
    }

    public class MoistureObject : IEquatable<MoistureObject>
    {
        public string Name { get; }
        public Color Color { get; }

        public MoistureObject(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public bool Equals(MoistureObject other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 97887982;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }
    }

    public class Biome : IEquatable<Biome>
    {
        private string Name { get; }
        private Color Color { get; }
        private List<BasicResource> Resources { get; set; }
        private List<int> HeightLowerLimits { get; set; }  //0 = the effect doesn't matter
        private List<TemperatureObject> Temperatures { get; set; }  //Empty list = doesn't matter
        private List<MoistureObject> Moistures { get; set; }

        private List<Soil> Soils { get; set; }

        public Biome(string name, Color color)
        {
            Name = name;
            Color = color;
            Resources = new List<BasicResource>();
            HeightLowerLimits = new List<int>();
            Temperatures = new List<TemperatureObject>();
            Moistures = new List<MoistureObject>();
            Soils = new List<Soil>();
        }

        public string getName()
        {
            return Name;
        }

        public Color getColor()
        {
            return Color;
        }

        public List<BasicResource> getResources()
        {
            return Resources;
        }

        public void addResource(BasicResource resource)
        {
            if (resource != null && !Resources.Contains(resource))
                Resources.Add(resource);
        }

        public List<int> getHeights()
        {
            return HeightLowerLimits;
        }

        public void addHeight(int height)
        {
            if (!HeightLowerLimits.Contains(height))
                HeightLowerLimits.Add(height);
        }

        public List<TemperatureObject> getTemperatures()
        {
            return Temperatures;
        }

        public void addTemperature(TemperatureObject temper)
        {
            if (!Temperatures.Contains(temper))
                Temperatures.Add(temper);
        }

        public List<MoistureObject> getMoistures()
        {
            return Moistures;
        }

        public void addMoisture(MoistureObject moist)
        {
            if (!Moistures.Contains(moist))
                Moistures.Add(moist);
        }

        public List<Soil> getSoils()
        {
            return Soils;
        }

        public void addSoil(Soil soil)
        {
            if (!Soils.Contains(soil))
                Soils.Add(soil);
        }

        public bool Equals(Biome other)
        {
            if (other != null)
                return this.Name.Equals(other.Name);
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 97887982;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }
    }
}
