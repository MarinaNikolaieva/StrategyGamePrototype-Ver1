using System.Collections.Generic;
using System;
using UnityEngine;

namespace LogisticsClasses
{
    public class LogisticObject : IEquatable<LogisticObject>
    {
        public int ID { get; }
        public string Type { get; }
        public Tuple<Vector2, Vector2> Coordinates { get; set; }

        public LogisticObject(int iD, string type, Tuple<Vector2, Vector2> coordinates)
        {
            ID = iD;
            Type = type;
            Coordinates = coordinates;
        }

        public bool Equals(LogisticObject other)
        {
            return this.ID == other.ID && this.Type.Equals(other.Type);
        }

        public override int GetHashCode()
        {
            int hashCode = 430596813;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            return hashCode;
        }
    }

    public class City : IEquatable<City>
    {
        public Vector2 Coords { get; }
        public string Name { get; set; }

        public City(Vector2 coords, string name)
        {
            Coords = coords;
            Name = name;
        }

        public string GetInfo()
        {
            return "Name: " + Name + "; Location: X " + Coords.x + ", Y " + Coords.y;
        }

        public bool Equals(City other)
        {
            return this.Name == other.Name && this.Coords == other.Coords;
        }

        public override int GetHashCode()
        {
            return -319820321 + Coords.GetHashCode();
        }
    }

    public class PathwayType : IEquatable<PathwayType>
    {
        public string Name { get; }
        private double Price { get; set; }

        public PathwayType(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public double GetPrice()
        {
            return Price;
        }

        public void SetPrice(double price)
        {
            if (price > 0)
                Price = price;
        }

        public string GetInfo()
        {
            return "Name: " + Name + "; Price: " + Price;
        }
        public bool Equals(PathwayType other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }

    public class PathwayCalculator
    {
        public Vector2 BeginCoordinates { get; }
        public Vector2 EndCoordinates { get; }

        public PathwayCalculator(Vector2 beginCoordinates, Vector2 endCoordinates)
        {
            BeginCoordinates = beginCoordinates;
            EndCoordinates = endCoordinates;
        }

        public List<Vector2> Calculate()
        {
            var result = new List<Vector2>();
            int dx = Math.Abs((int)EndCoordinates.x - (int)BeginCoordinates.x), sx = (int)BeginCoordinates.x < (int)EndCoordinates.x ? 1 : -1;
            int dy = Math.Abs((int)EndCoordinates.y - (int)BeginCoordinates.y), sy = (int)BeginCoordinates.y < (int)EndCoordinates.y ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            int x = (int)BeginCoordinates.x;
            int y = (int)BeginCoordinates.y;
            for (; ; )
            {
                result.Add(new Vector2(x, y));
                if (x == (int)EndCoordinates.x && y == (int)EndCoordinates.y)
                    break;
                e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dy)
                {
                    err += dx;
                    y += sy;
                }
            }
            return result;
        }
    }

    public class Pathway : IEquatable<Pathway>
    {
        public Vector2 BeginCoordinates { get; }
        public Vector2 EndCoordinates { get; }
        public PathwayType Type { get; set; }
        public double Length { get; }

        public List<Vector2> Parts { get; }

        public Pathway(Vector2 beginCoordinates, Vector2 endCoordinates, PathwayType type)
        {
            BeginCoordinates = beginCoordinates;
            EndCoordinates = endCoordinates;
            Type = type;
            //Length = Math.Sqrt(Math.Pow(EndCoordinates.X - BeginCoordinates.X, 2.0) + Math.Pow(EndCoordinates.Y - BeginCoordinates.Y, 2.0));
            Parts = new List<Vector2>();
            //NEEDED add a Bresenford's algorithm for calculating the path parts

            PathwayCalculator pathwayCalculator = new PathwayCalculator(beginCoordinates, endCoordinates);
            Parts = pathwayCalculator.Calculate();

            Length = Parts.Count;
        }

        public double GetPrice()
        {
            //This must be more complex and include the impact of the SOIL the path is placed on
            return Length * Type.GetPrice();
        }

        public string GetInfo()
        {
            return "Begin: X " + BeginCoordinates.x + ", Y " + BeginCoordinates.y + "; End: X" +
                EndCoordinates.x + ", Y " + EndCoordinates.y + "; Length: " + Length + "; Type: " + Type.Name +
                "; Total price: " + GetPrice();
        }

        public bool Equals(Pathway other)
        {
            return (this.BeginCoordinates == other.BeginCoordinates &&
                this.EndCoordinates == other.EndCoordinates) || (this.BeginCoordinates == other.EndCoordinates &&
                this.EndCoordinates == other.BeginCoordinates);
        }

        public override int GetHashCode()
        {
            int hashCode = 2097634782;
            hashCode = hashCode * -1521134295 + BeginCoordinates.GetHashCode();
            hashCode = hashCode * -1521134295 + EndCoordinates.GetHashCode();
            return hashCode;
        }
    }
}