using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;

namespace WarfareClasses
{
    //4 Ints: days, hours, minutes, seconds
    public interface IObject : IEquatable<IObject>
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public double Resource { get; set; }
        public double ResourceDepletionCoef { get; set; }
        public Tuple<float, float> ResourceRestorageCoefs { get; set; }

        public bool Broken { get; set; }

        public bool Equals(IObject? other)
        {
            if (other == null)
                return false;
            else
                return Name.Equals(other.Name);
        }

        public int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    //ALL changes made to the Movable- and StationaryObject structs must come through here
    public interface IStructModifier
    {
        public bool Broken { get; set; }
        public double Resource { get; set; }
    }

    //People, personal weapons, vehicles, tech etc
    public struct MovableObject : IObject, IStructModifier, IEquatable<MovableObject>
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Resource { get; set; }
        public double ResourceDepletionCoef { get; set; }  //soldier gets tired and equipment breaks after some time
        public Tuple<float, float> ResourceRestorageCoefs { get; set; }  //First - for (0, 100), second - for (-1, 0)

        public bool Broken { get; set; }

        public MovableObject(string name, string category, double resourceDepletionCoef, float restoreLight, float restoreHeavy)
        {
            this.Name = name;
            this.Category = category;
            this.ResourceDepletionCoef = resourceDepletionCoef;
            Resource = 100.0;
            Broken = false;
            ResourceRestorageCoefs = new Tuple<float, float>(restoreLight, restoreHeavy);
        }

        public void ResetResource()
        {
            Resource = 100.0;
            Broken = false;
        }

        public void DepleteResource(double pressureCoef, bool broken)
        //If the soldier is just tired, it won't go below 0
        //But if he's wounded, it will drop. Resource = -1 => killed
        {
            Resource -= ResourceDepletionCoef * pressureCoef;
            if (broken)
                Broken = true;
            if (Resource <= 0 && !Broken)
                Resource = 0.0001;
        }

        public void AddResource(double toAdd)
        {
            if (toAdd > 0)
                Resource += toAdd;
            if (Resource > 100.0)
                ResetResource();
        }

        public bool Equals(MovableObject other)
        {
            return other.Name.Equals(Name);
        }

        public bool Equals(IObject? other)
        {
            if (other == null)
                return false;
            return Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public struct StationaryObject : IObject, IStructModifier, IEquatable<StationaryObject>
    {
        //Fortifications etc
        public string Name { get; set; }
        public string Category { get; set; }
        public double Resource { get; set; }
        public double ResourceDepletionCoef { get; set; }
        public Tuple<float, float> ResourceRestorageCoefs { get; set; }  //First - for (0, 100), second - for (-1, 0)

        public bool Broken { get; set; }

        public StationaryObject(string name, string category, double resourceDepletionCoef, float restoreLight, float restoreHeavy)
        {
            this.Name = name;
            this.Category = category;
            this.ResourceDepletionCoef = resourceDepletionCoef;
            Resource = 100.0;
            Broken = false;
            ResourceRestorageCoefs = new Tuple<float, float>(restoreLight, restoreHeavy);
        }

        public void ResetResource()
        {
            Resource = 100.0;
            Broken = false;
        }

        public void DepleteResource(double pressureCoef, bool broken)  //Here, Resource <= 0 => destroyed beyond repair
        {
            Resource -= ResourceDepletionCoef * pressureCoef;
            if (broken)
                Broken = true;
            if (Resource <= 0 && !Broken)
                Resource = 0.0001;
        }

        public void AddResource(double toAdd)
        {
            if (toAdd > 0)
                Resource += toAdd;
            if (Resource > 100.0)
                ResetResource();
        }

        public bool Equals(StationaryObject other)
        {
            return other.Name.Equals(Name);
        }

        public bool Equals(IObject? other)
        {
            if (other == null)
                return false;
            return Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class BattlePosition
    {
        private static System.Random rand;
        public bool isPressureActive;
        private Timer pressureTimer;
        private TimeSpan delta = new TimeSpan(0, 0, 0, 1);
        public Vector2 Position;
        public Dictionary<IObject, Tuple<bool, double, int>> HumanAndTechResources;  //<Object, <can_be_broken, resource, quantity>>
        public Dictionary<IObject, Tuple<bool, double, int>> StationaryPlaces;  //Same here
        public Dictionary<IObject, Tuple<bool, double, int>> TempOutOfActionObjects;  //Same here

        public BattlePosition(int x, int y)
        {
            Position = new Vector2(x, y);
            HumanAndTechResources = new Dictionary<IObject, Tuple<bool, double, int>>();
            StationaryPlaces = new Dictionary<IObject, Tuple<bool, double, int>>();
            TempOutOfActionObjects = new Dictionary<IObject, Tuple<bool, double, int>>();
            rand = new System.Random();
            isPressureActive = false;
            pressureTimer = new Timer();
        }

        public void ChangeResourceValue(IObject neededObject, double newValue, bool broken)
        {
            if (neededObject is MovableObject && HumanAndTechResources.ContainsKey((MovableObject)neededObject))
            {
                if (broken)
                    HumanAndTechResources[neededObject] = new Tuple<bool, double, int>(true, HumanAndTechResources[neededObject].Item2 - newValue, HumanAndTechResources[neededObject].Item3);
                if (newValue <= 0 && !HumanAndTechResources[neededObject].Item1)
                    HumanAndTechResources[neededObject] = new Tuple<bool, double, int>(true, 0.0001, HumanAndTechResources[neededObject].Item3);
            }
            else if (neededObject is StationaryObject && StationaryPlaces.ContainsKey((StationaryObject)neededObject))
            {
                if (broken)
                    StationaryPlaces[neededObject] = new Tuple<bool, double, int>(true, StationaryPlaces[neededObject].Item2 - newValue, StationaryPlaces[neededObject].Item3);
                if (newValue <= 0 && !StationaryPlaces[neededObject].Item1)
                    StationaryPlaces[neededObject] = new Tuple<bool, double, int>(true, 0.0001, StationaryPlaces[neededObject].Item3);
            }
            else if (TempOutOfActionObjects.ContainsKey(neededObject))
            {
                if (broken)
                    TempOutOfActionObjects[neededObject] = new Tuple<bool, double, int>(true, TempOutOfActionObjects[neededObject].Item2 - newValue, TempOutOfActionObjects[neededObject].Item3);
                if (newValue <= 0 && !TempOutOfActionObjects[neededObject].Item1)
                    TempOutOfActionObjects[neededObject] = new Tuple<bool, double, int>(true, 0.0001, TempOutOfActionObjects[neededObject].Item3);
            }
        }

        public void TryAddStationary(IObject stationaryObject, int quantity)
        {
            if (!StationaryPlaces.ContainsKey(stationaryObject))
                StationaryPlaces.Add(stationaryObject, new Tuple<bool, double, int>(stationaryObject.Broken, stationaryObject.Resource, quantity));
            else
                StationaryPlaces[stationaryObject] = new Tuple<bool, double, int>(StationaryPlaces[stationaryObject].Item1, StationaryPlaces[stationaryObject].Item2, 
                    StationaryPlaces[stationaryObject].Item3 + quantity);
        }

        public bool TryRemoveStationary(IObject stationaryObject, int quantity)
        {
            if (StationaryPlaces.ContainsKey(stationaryObject))
            {
                if (StationaryPlaces[stationaryObject].Item3 > quantity)
                {
                    StationaryPlaces[stationaryObject] = new Tuple<bool, double, int>(StationaryPlaces[stationaryObject].Item1, StationaryPlaces[stationaryObject].Item2,
                        StationaryPlaces[stationaryObject].Item3 - quantity);
                    return true;
                }
                else
                {
                    StationaryPlaces.Remove(stationaryObject);
                    return true;
                }
            }
            return false;
        }

        public void TryAddTemp(IObject iObject, double resource, int quantity)
        {
            if (!TempOutOfActionObjects.ContainsKey(iObject))
                TempOutOfActionObjects.Add(iObject, new Tuple<bool, double, int>(iObject.Broken, resource, quantity));
            else
                TempOutOfActionObjects[iObject] = new Tuple<bool, double, int>(TempOutOfActionObjects[iObject].Item1, TempOutOfActionObjects[iObject].Item2,
                    TempOutOfActionObjects[iObject].Item3 + quantity);
        }

        public bool TryRemoveTemp(IObject iObject, int quantity)
        {
            if (TempOutOfActionObjects.ContainsKey(iObject))
            {
                if (TempOutOfActionObjects[iObject].Item3 > quantity)
                {
                    TempOutOfActionObjects[iObject] = new Tuple<bool, double, int>(TempOutOfActionObjects[iObject].Item1, TempOutOfActionObjects[iObject].Item2,
                        TempOutOfActionObjects[iObject].Item3 - quantity);
                    return true;
                }
                else
                {
                    TempOutOfActionObjects.Remove(iObject);
                    return true;
                }
            }
            return false;
        }

        public void TryAddResource(IObject movableObject, int quantity)
        {
            if (!HumanAndTechResources.ContainsKey(movableObject))
                HumanAndTechResources.Add(movableObject, new Tuple<bool, double, int>(movableObject.Broken, movableObject.Resource, quantity));
            else
                HumanAndTechResources[movableObject] = new Tuple<bool, double, int>(HumanAndTechResources[movableObject].Item1, HumanAndTechResources[movableObject].Item2,
                    HumanAndTechResources[movableObject].Item3 + quantity);
        }

        public bool TryRemoveResource(IObject movableObject, int quantity)
        {
            if (HumanAndTechResources.ContainsKey(movableObject))
            {
                if (HumanAndTechResources[movableObject].Item3 > quantity)
                {
                    HumanAndTechResources[movableObject] = new Tuple<bool, double, int>(HumanAndTechResources[movableObject].Item1, HumanAndTechResources[movableObject].Item2,
                        HumanAndTechResources[movableObject].Item3 - quantity);
                    return true;
                }
                else
                {
                    HumanAndTechResources.Remove(movableObject);
                    return true;
                }
            }
            return false;
        }

        public int TrySendResource(IObject movableObject, int quantity)
        {
            int result = 0;
            if (HumanAndTechResources.ContainsKey(movableObject))
            {
                if (HumanAndTechResources[movableObject].Item3 > quantity)
                {
                    HumanAndTechResources[movableObject] = new Tuple<bool, double, int>(HumanAndTechResources[movableObject].Item1, HumanAndTechResources[movableObject].Item2,
                        HumanAndTechResources[movableObject].Item3 - quantity);
                    result = quantity;
                }
                else
                {
                    result = HumanAndTechResources[movableObject].Item3;
                    //HumanAndTechResources[movableObject] = 0;
                    HumanAndTechResources.Remove(movableObject);
                }
            }
            return result;
        }

        public int TryTransferResource(IObject movableObject, int quantity)
        {
            int result = 0;
            if (HumanAndTechResources.ContainsKey(movableObject))
            {
                if (HumanAndTechResources[movableObject].Item3 > quantity)
                {
                    HumanAndTechResources[movableObject] = new Tuple<bool, double, int>(HumanAndTechResources[movableObject].Item1, HumanAndTechResources[movableObject].Item2,
                        HumanAndTechResources[movableObject].Item3 - quantity);
                    TryAddTemp(movableObject, HumanAndTechResources[movableObject].Item2, quantity);
                    result = quantity;
                }
                else
                {
                    result = HumanAndTechResources[movableObject].Item3;
                    TryAddTemp(movableObject, HumanAndTechResources[movableObject].Item2, quantity);
                    //HumanAndTechResources[movableObject] = 0;
                    HumanAndTechResources.Remove(movableObject);
                }
            }
            return result;
        }

        public int TryTransferStationary(IObject stationaryObject, int quantity)
        {
            int result = 0;
            if (StationaryPlaces.ContainsKey(stationaryObject))
            {
                if (StationaryPlaces[stationaryObject].Item3 > quantity)
                {
                    StationaryPlaces[stationaryObject] = new Tuple<bool, double, int>(StationaryPlaces[stationaryObject].Item1, StationaryPlaces[stationaryObject].Item2,
                        StationaryPlaces[stationaryObject].Item3 - quantity);
                    TryAddTemp(stationaryObject, StationaryPlaces[stationaryObject].Item2, quantity);
                    result = quantity;
                }
                else
                {
                    result = StationaryPlaces[stationaryObject].Item3;
                    TryAddTemp(stationaryObject, StationaryPlaces[stationaryObject].Item2, quantity);
                    //StationaryPlaces[stationaryObject] = 0;
                    StationaryPlaces.Remove(stationaryObject);
                }
            }
            return result;
        }
        public string GetInfoShort()
        {
            return "Class: Battle position; Coords: X " + Position.x + ", Y " + Position.y + "; Amount of fine units profiles: " + HumanAndTechResources.Count +
                "; Amount of defective units profiles: " + TempOutOfActionObjects.Count + "; Amount of stationary objects classes: " + StationaryPlaces.Count;
        }

        public List<string> GetInfoDetailed()
        {
            List<string> res = new List<string>();
            res.Add("Class: Battle position; Coords: X " + Position.x + ", Y " + Position.y + "\nFine units:");
            for (int i = 0; i < HumanAndTechResources.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(HumanAndTechResources.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(HumanAndTechResources.ElementAt(i).Value.Item3);
                build.Append(" in state: ");
                build.Append(HumanAndTechResources.ElementAt(i).Value.Item2.ToString("0.####"));
                res.Add(build.ToString());
            }
            res.Add("\nDefective units:");
            for (int i = 0; i < TempOutOfActionObjects.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(TempOutOfActionObjects.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(TempOutOfActionObjects.ElementAt(i).Value.Item3);
                build.Append(" in state: ");
                build.Append(TempOutOfActionObjects.ElementAt(i).Value.Item2.ToString("0.####"));
                res.Add(build.ToString());
            }
            res.Add("\nStationary units:");
            for (int i = 0; i < StationaryPlaces.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(StationaryPlaces.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(StationaryPlaces.ElementAt(i).Value.Item3);
                build.Append(" in state: ");
                build.Append(StationaryPlaces.ElementAt(i).Value.Item2.ToString("0.####"));
                res.Add(build.ToString());
            }
            return res;
        }

        private static void OnTimedEvent(object? sender, System.Timers.ElapsedEventArgs e, TimeSpan delta, ref TimeSpan main,
            System.Timers.Timer caller, double pressureCoef, BattlePosition position)
        {
            if (main > TimeSpan.Zero)
            {
                main = main.Subtract(delta);

                for (int i = 0; i < position.HumanAndTechResources.Count; i++)
                {
                    int movableBreakProb = rand.Next(0, 101);
                    if (movableBreakProb > 50)  //20?
                    {
                        double newValue = position.HumanAndTechResources.ElementAt(i).Value.Item3 -
                            position.HumanAndTechResources.ElementAt(i).Key.ResourceDepletionCoef * pressureCoef;
                        position.ChangeResourceValue(position.HumanAndTechResources.ElementAt(i).Key, newValue, true);
                    }
                    else
                    {
                        double newValue = position.HumanAndTechResources.ElementAt(i).Value.Item3 -
                            position.HumanAndTechResources.ElementAt(i).Key.ResourceDepletionCoef * pressureCoef;
                        position.ChangeResourceValue(position.HumanAndTechResources.ElementAt(i).Key, newValue, false);
                    }
                }
                for (int i = 0; i < position.StationaryPlaces.Count; i++)
                {
                    int stationaryBreakProb = rand.Next(0, 101);
                    if (stationaryBreakProb > 50)
                    {
                        double newValue = position.StationaryPlaces.ElementAt(i).Value.Item3 -
                            position.StationaryPlaces.ElementAt(i).Key.ResourceDepletionCoef * pressureCoef;
                        position.ChangeResourceValue(position.StationaryPlaces.ElementAt(i).Key, newValue, true);
                    }
                    else
                    {
                        double newValue = position.StationaryPlaces.ElementAt(i).Value.Item3 -
                            position.StationaryPlaces.ElementAt(i).Key.ResourceDepletionCoef * pressureCoef;
                        position.ChangeResourceValue(position.StationaryPlaces.ElementAt(i).Key, newValue, false);
                    }
                }
                for (int i = 0; i < position.TempOutOfActionObjects.Count; i++)
                {
                    int breakProb = rand.Next(0, 101);
                    if (breakProb > 50)
                    {
                        double newValue = position.TempOutOfActionObjects.ElementAt(i).Value.Item3 -
                            position.TempOutOfActionObjects.ElementAt(i).Key.ResourceDepletionCoef * pressureCoef;
                        position.ChangeResourceValue(position.TempOutOfActionObjects.ElementAt(i).Key, newValue, true);
                    }
                    else
                    {
                        double newValue = position.TempOutOfActionObjects.ElementAt(i).Value.Item3 -
                            position.TempOutOfActionObjects.ElementAt(i).Key.ResourceDepletionCoef * pressureCoef;
                        position.ChangeResourceValue(position.TempOutOfActionObjects.ElementAt(i).Key, newValue, false);
                    }
                }
            }
            else
            {
                caller.Stop();
                position.isPressureActive = false;
            }
        }

        //The pressure method is to be run as a thread
        //TEST
        public void Pressure(float pressureCoef, int timeToPress)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(timeToPress);
            //TimeSpan delta = new TimeSpan(0, 0, 0, 1);  //If event will take too long, make the delta longer!
            pressureTimer = new System.Timers.Timer(timespan.TotalSeconds);
            pressureTimer.Interval = 1000;
            pressureTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e, delta, ref timespan, pressureTimer, pressureCoef, this);
            isPressureActive = true;
            pressureTimer.Start();
        }

        public void StopPressure()
        {
            isPressureActive = false;
            pressureTimer.Stop();
        }
    }

    public class ObjectBank
    {
        //All objects, both Movable and Stationary
        public Dictionary<IObject, int> Bank;
        //<KeyObjectName, List<DependentObjectName, quantity>>
        public Dictionary<string, List<Tuple<string, int>>> Dependencies;
        //The first value in tuple is the QUANTITY, the other is seconds to create
        public Dictionary<string, Tuple<int, int>> TimeValuesForCreation;
        //The first Tuple value is the coef for (0, 100) interval, the second is for (-0.(9), 0), which takes longer
        //private Dictionary<string, Tuple<double, double>> TimeFactorsForRestoring;

        public ObjectBank()
        {
            Bank = new Dictionary<IObject, int>();
            Dependencies = new Dictionary<string, List<Tuple<string, int>>>();
            TimeValuesForCreation = new Dictionary<string, Tuple<int, int>>();
            //TimeFactorsForRestoring = new Dictionary<string, Tuple<double, double>>();
        }

        public void InitBank(List<IObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
                Bank.Add(objects[i], 0);
        }

        public void InitDependencies(Dictionary<string, List<Tuple<string, int>>> pairs)
        {
            for (int i = 0; i < pairs.Count; i++)
                Dependencies.Add(pairs.ElementAt(i).Key, pairs.ElementAt(i).Value);
        }

        public void InitTimes(Dictionary<string, Tuple<int, int>> create)
        {
            for (int i = 0; i < create.Count; i++)
                TimeValuesForCreation.Add(create.ElementAt(i).Key, create.ElementAt(i).Value);
            //for (int i = 0; i < restore.Count; i++)
            //    TimeFactorsForRestoring.Add(restore.ElementAt(i).Key, restore.ElementAt(i).Value);
        }
        
        public IObject TryGetResource(string objName)
        {
            IObject obj = Bank.Where(o => o.Key.Name.Equals(objName)).FirstOrDefault().Key;
            if (obj != null)
                return obj;
            return null;
        }

        public int TryGetResourceAmount(string objName)
        {
            IObject obj = Bank.Where(o => o.Key.Name.Equals(objName)).FirstOrDefault().Key;
            if (obj != null)
                return Bank[obj];
            return -1;
        }

        public int TryGetCreateTimeValues(IObject obj, int quantity)
        {
            if (obj != null)
            {
                List<Tuple<int, int>> times = TimeValuesForCreation.Where(t => t.Key.Equals(obj.Name)).OrderBy(t => t.Value.Item1).Select(t => t.Value).ToList();
                for (int i = 0; i < times.Count; i++)
                    if (times.ElementAt(i).Item1 >= quantity)
                        return times.ElementAt(i).Item2;
                return times.Last().Item2;
            }
            return -1;
        }

        //public Tuple<double, double> TryGetRestoreTimeValues(IObject obj)
        //{
        //    if (obj != null)
        //    {
        //        Tuple<double, double> times = TimeFactorsForRestoring.Where(t => t.Key.Equals(obj.Name)).FirstOrDefault().Value;
        //        if (times != null)
        //            return times;
        //    }
        //    return null;
        //}

        public void TryAddResource(IObject obj, int quantity)
        {
            if (!Bank.ContainsKey(obj))
                Bank.Add(obj, quantity);
            else
                Bank[obj] += quantity;
        }

        public bool TryRemoveResource(IObject obj, int quantity)
        {
            if (Bank.ContainsKey(obj) && Bank[obj] >= quantity)
            {
                Bank[obj] = Bank[obj] >= quantity ? Bank[obj] - quantity : 0;
                return true;
            }
            return false;
        }

        //NEEDED Add a wrong-amount responce!
        public List<Tuple<IObject, int>> TryTransferResource(IObject obj, int quantity)
        {
            List<Tuple<IObject, int>> list = new List<Tuple<IObject, int>>();
            if (Bank.ContainsKey(obj) && Bank[obj] >= quantity)
            {
                List<Tuple<string, int>> dependencies = Dependencies.Where(d => d.Key.Equals(obj.Name)).FirstOrDefault().Value;
                if (dependencies == null)
                    dependencies = new List<Tuple<string, int>>();
                if (dependencies.Count > 0)
                {
                    for (int i = 0; i < dependencies.Count; i++)
                    {
                        int tempQuantity = quantity * dependencies.ElementAt(i).Item2;
                        list.AddRange(TryTransferResource(Bank.Where(b => b.Key.Name.Equals(dependencies.ElementAt(i).Item1)).First().Key,
                            tempQuantity));
                    }
                }
                if (list.Count < dependencies.Count)
                {
                    list.Clear();
                    return list;
                }
                list.Add(new Tuple<IObject, int>((IObject)obj, quantity));
                Bank[obj] -= quantity;
            }
            return list;
        }
    }
}