using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;
using WarfareClasses;

namespace WarfareMechanics
{
    public interface ContainerObject
    {
        public ObjectBank ObjectBank { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
        public Vector2 Coords { get; set; }

        public bool isTimerActive { get; set; }
        public Timer actionTimer { get; set; }
    }

    public class TrainingPlace : ContainerObject, IEquatable<ContainerObject>
    {
        public ObjectBank ObjectBank { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
        public Vector2 Coords { get; set; }
        public bool isTimerActive { get; set; }
        public Timer actionTimer { get; set; }

        public TrainingPlace(ObjectBank objectBank, string name, int capacity, Vector2 coords)
        {
            ObjectBank = objectBank;
            this.Name = "Training Field";
            this.Capacity = capacity;
            CurrentCapacity = capacity;
            CustomName = name;
            Coords = coords;
            isTimerActive = false;
            actionTimer = new Timer();
        }

        private static void OnTimedEvent(object? sender, System.Timers.ElapsedEventArgs e, TimeSpan delta, ref TimeSpan main, System.Timers.Timer caller,
            MovableObject neededObject, int quantity, ObjectBank objectBank)
        {
            if (main > TimeSpan.Zero)
            {
                main = main.Subtract(delta);
            }
            else
            {
                caller.Stop();
                objectBank.TryAddResource(neededObject, quantity);
            }
        }

        //MAYBE make a special Thread-handler here?
        //This one MUST be run as a Thread
        public void Train(MovableObject neededObject, int quantity)
        {
            if (ObjectBank.TryGetResource(neededObject.Name) != null)
            {
                int time = ObjectBank.TryGetCreateTimeValues(neededObject, quantity);

                TimeSpan timespan = TimeSpan.FromSeconds(time);
                TimeSpan delta = new TimeSpan(0, 0, 0, 1);
                actionTimer = new System.Timers.Timer(timespan.TotalSeconds);
                actionTimer.Interval = 1000;
                actionTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e, delta, ref timespan, actionTimer, neededObject, quantity, this.ObjectBank);
                actionTimer.Start();
                isTimerActive = true;
            }
        }

        public void StopTraining()
        {
            actionTimer.Stop();
            isTimerActive = false;
        }

        public string GetInfo()
        {
            return "Class: " + Name + "; Name: " + CustomName + "; Coords: X " + Coords.x + ", Y " + Coords.y;
        }

        public bool Equals(ContainerObject other)
        {
            if (other == null)
                return false;
            return this.CustomName.Equals(other.CustomName);
        }
    }

    public class Hospital : ContainerObject, IEquatable<ContainerObject> //For people ONLY, it must be filtered elsewhere
    {
        public ObjectBank ObjectBank { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
        public Vector2 Coords { get; set; }
        public bool isTimerActive { get; set; }
        public Timer actionTimer { get; set; }
        private TimeSpan delta = new TimeSpan(0, 0, 0, 1);

        public Dictionary<IObject, Tuple<bool, double, int>> ToFix { get; set; }  //Object, quantity
        public Dictionary<IObject, Tuple<bool, double, int>> Fixed { get; set; }


        public Hospital(ObjectBank objectBank, string name, int capacity, Vector2 coords)
        {
            ToFix = new Dictionary<IObject, Tuple<bool, double, int>>();
            Fixed = new Dictionary<IObject, Tuple<bool, double, int>>();
            ObjectBank = objectBank;
            this.Name = "Hospital";
            this.Capacity = capacity;
            CurrentCapacity = capacity;
            CustomName = name;
            Coords = coords;
            isTimerActive = false;
            actionTimer = new Timer();
        }
        public void SetCapacity(int c)
        {
            int occupiedCapacity = Capacity - CurrentCapacity;
            if (c > 0 && c >= occupiedCapacity)
            {
                Capacity = c;
                CurrentCapacity = Capacity - occupiedCapacity;
            }
        }

        public void ResetCapacity()
        {
            CurrentCapacity = Capacity;
        }

        public void OccupyCapacity(int amount)
        {
            if (amount >= CurrentCapacity)
                CurrentCapacity -= amount;
        }

        public void FreeCapacity(int amount)
        {
            if (CurrentCapacity + amount <= Capacity)
                CurrentCapacity += amount;
        }

        public bool Equals(ContainerObject other)
        {
            if (other == null)
                return false;
            return CustomName.Equals(other.CustomName);
        }

        public override int GetHashCode()
        {
            return CustomName.GetHashCode();
        }


        //Here are the methods common to all objects, both living and not
        public void TryAddObject(IObject obj, double resource, int quantity, bool removeFromBank)
        {
            if (resource != 100)
            {
                if (!ToFix.ContainsKey(obj))
                    ToFix.Add(obj, new Tuple<bool, double, int>(obj.Broken, resource, quantity));
                else
                    ToFix[obj] = new Tuple<bool, double, int>(ToFix[obj].Item1, ToFix[obj].Item2, ToFix[obj].Item3 + quantity);
            }
            else
            {
                if (!Fixed.ContainsKey(obj))
                    Fixed.Add(obj, new Tuple<bool, double, int>(obj.Broken, resource, quantity));
                else
                    Fixed[obj] = new Tuple<bool, double, int>(Fixed[obj].Item1, Fixed[obj].Item2, Fixed[obj].Item3 + quantity);
            }
            if (removeFromBank)
                ObjectBank.TryRemoveResource(obj, quantity);
        }

        public void TryMoveObject(IObject obj, int quantity)
        {
            if (!Fixed.ContainsKey(obj))
            {
                int quant = ToFix[obj].Item3 >= quantity ? quantity : ToFix[obj].Item3;
                if (quant == ToFix[obj].Item3)
                    ToFix.Remove(obj);
                else
                    ToFix[obj] = new Tuple<bool, double, int>(ToFix[obj].Item1, ToFix[obj].Item2, ToFix[obj].Item3 - quantity);
                Fixed.Add(obj, new Tuple<bool, double, int>(obj.Broken, obj.Resource, quant));
            }
            else
            {
                int quant = ToFix[obj].Item3 >= quantity ? quantity : ToFix[obj].Item3;
                if (quant == ToFix[obj].Item3)
                    ToFix.Remove(obj);
                else
                    ToFix[obj] = new Tuple<bool, double, int>(ToFix[obj].Item1, ToFix[obj].Item2, ToFix[obj].Item3 - quantity);
                Fixed[obj] = new Tuple<bool, double, int>(Fixed[obj].Item1, Fixed[obj].Item2, Fixed[obj].Item3 + quant);
            }
        }

        public void TryRemoveObject(IObject obj, int quantity, bool addToBank)
        {
            if (Fixed.ContainsKey(obj))
            {
                int quant = Fixed[obj].Item3 >= quantity ? quantity : Fixed[obj].Item3;
                if (quant == Fixed[obj].Item3)
                    Fixed.Remove(obj);
                else
                    Fixed[obj] = new Tuple<bool, double, int>(Fixed[obj].Item1, Fixed[obj].Item2, Fixed[obj].Item3 - quantity);
                if (addToBank)
                    ObjectBank.TryAddResource(obj, quant);
            }
        }

        //ALL the prev methods need to be combined into the monitor process to run as a thread
        //Restoring from -0.(9) to 100 is also a common method, also run as a thread
        //public void RestoreHeal()
        //{
        //    while (ToFix.Count > 0)
        //    {
        //        //NEEDED make the next loop repeatable ONLY SOMETIMES, when the list is updated
        //        //resourceRestorages.Clear();
        //        //for (int i = 0; i < ToFix.Count; i++)
        //        //{
        //        //    resourceRestorages.Add(ObjectBank.TryGetRestoreTimeValues(ToFix.ElementAt(i).Key));
        //        //}
        //        for (int i = 0; i < ToFix.Count; i++)
        //        {
        //            if (ToFix.ElementAt(i).Value.Item2 == 100.0)
        //                TryMoveObject(ToFix.ElementAt(i).Key, ToFix.ElementAt(i).Value.Item3);
        //            else if (ToFix.ElementAt(i).Key is MovableObject)
        //            {
        //                if (ToFix.ElementAt(i).Value.Item2 < 0)
        //                    ToFix[ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(ToFix.ElementAt(i).Value.Item1, ToFix.ElementAt(i).Value.Item2 + ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item2, ToFix.ElementAt(i).Value.Item3);
        //                else
        //                    ToFix[ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(ToFix.ElementAt(i).Value.Item1, ToFix.ElementAt(i).Value.Item2 + ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item1, ToFix.ElementAt(i).Value.Item3);

        //                if (ToFix.ElementAt(i).Value.Item2 > 100)
        //                    ToFix[ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(ToFix.ElementAt(i).Value.Item1, 100.0, ToFix.ElementAt(i).Value.Item3);

        //            }
        //        }
        //    }
        //}

        private static void OnTimedEvent(object? sender, System.Timers.ElapsedEventArgs e, TimeSpan delta, ref TimeSpan main,
            System.Timers.Timer caller, Hospital hospital)
        {
            if (main > TimeSpan.Zero)
            {
                main = main.Subtract(delta);

                for (int i = 0; i < hospital.ToFix.Count; i++)
                {
                    if (hospital.ToFix.ElementAt(i).Value.Item2 == 100.0)
                        hospital.TryMoveObject(hospital.ToFix.ElementAt(i).Key, hospital.ToFix.ElementAt(i).Value.Item3);
                    else if (hospital.ToFix.ElementAt(i).Key is MovableObject)
                    {
                        if (hospital.ToFix.ElementAt(i).Value.Item2 < 0)
                            hospital.ToFix[hospital.ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(hospital.ToFix.ElementAt(i).Value.Item1, hospital.ToFix.ElementAt(i).Value.Item2 + hospital.ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item2, hospital.ToFix.ElementAt(i).Value.Item3);
                        else
                            hospital.ToFix[hospital.ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(hospital.ToFix.ElementAt(i).Value.Item1, hospital.ToFix.ElementAt(i).Value.Item2 + hospital.ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item1, hospital.ToFix.ElementAt(i).Value.Item3);

                        if (hospital.ToFix.ElementAt(i).Value.Item2 > 100)
                            hospital.ToFix[hospital.ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(hospital.ToFix.ElementAt(i).Value.Item1, 100.0, hospital.ToFix.ElementAt(i).Value.Item3);

                    }
                }
            }
            else
            {
                main = TimeSpan.FromSeconds(10);
            }
        }

        public void Heal()
        {
            TimeSpan timespan = TimeSpan.FromSeconds(10);
            actionTimer = new System.Timers.Timer(timespan.TotalSeconds);
            actionTimer.Interval = 1000;
            actionTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e, delta, ref timespan, actionTimer, this);
            isTimerActive = true;
            actionTimer.Start();
        }

        public void StopHealingAction()
        {
            isTimerActive = false;
            actionTimer.Stop();
        }

        public string GetInfoShort()
        {
            return "Class: " + Name + "; Name: " + CustomName + "; Coords: X " + Coords.x + ", Y " + Coords.y + "; Capacity: " + Capacity + ", free: " + CurrentCapacity + "; Amount of wounded people profiles: " +
                ToFix.Count;
        }

        public List<string> GetInfoDetailed()
        {
            List<string> res = new List<string>();
            res.Add("Class: " + Name + "; Name: " + CustomName + "; Coords: X " + Coords.x + ", Y " + Coords.y + "; Capacity: " + Capacity + ", free: " + CurrentCapacity + "\nHealing:");
            for (int i = 0; i < ToFix.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(ToFix.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(ToFix.ElementAt(i).Value.Item3);
                build.Append(" in state: ");
                build.Append(ToFix.ElementAt(i).Value.Item2.ToString("0.####"));
                res.Add(build.ToString());
            }
            res.Add("\nHealed:");
            for (int i = 0; i < Fixed.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(Fixed.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(Fixed.ElementAt(i).Value.Item3);
                build.Append(" in state: ");
                build.Append(Fixed.ElementAt(i).Value.Item2.ToString("0.####"));
                res.Add(build.ToString());
            }
            return res;
        }
    }

    public class Workshop : ContainerObject, IEquatable<ContainerObject> //For StationaryObjects and tech ONLY
    {
        public ObjectBank ObjectBank { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
        public Vector2 Coords { get; set; }
        public bool isTimerActive { get; set; }
        public Timer actionTimer { get; set; }
        private TimeSpan delta = new TimeSpan(0, 0, 0, 1);

        public Dictionary<IObject, Tuple<bool, double, int>> ToFix { get; set; }
        public Dictionary<IObject, Tuple<bool, double, int>> Fixed { get; set; }

        //private List<double> resourceRestorages = new List<double>();

        public Workshop(ObjectBank objectBank, string name, int capacity, Vector2 coords)
        {
            ToFix = new Dictionary<IObject, Tuple<bool, double, int>>();
            Fixed = new Dictionary<IObject, Tuple<bool, double, int>>();
            ObjectBank = objectBank;
            this.Name = "Workshop";
            this.Capacity = capacity;
            CurrentCapacity = capacity;
            CustomName = name;
            Coords = coords;
            isTimerActive = false;
            actionTimer = null; ;
        }
        public void SetCapacity(int c)
        {
            int occupiedCapacity = Capacity - CurrentCapacity;
            if (c > 0 && c >= occupiedCapacity)
            {
                Capacity = c;
                CurrentCapacity = Capacity - occupiedCapacity;
            }
        }

        public void ResetCapacity()
        {
            CurrentCapacity = Capacity;
        }

        public void OccupyCapacity(int amount)
        {
            if (amount >= CurrentCapacity)
                CurrentCapacity -= amount;
        }

        public void FreeCapacity(int amount)
        {
            if (CurrentCapacity + amount <= Capacity)
                CurrentCapacity += amount;
        }

        public bool Equals(ContainerObject other)
        {
            if (other == null)
                return false;
            return CustomName.Equals(other.CustomName);
        }

        public override int GetHashCode()
        {
            return CustomName.GetHashCode();
        }

        //Here are the methods common to all objects, both living and not
        public void TryAddObject(IObject obj, double resource, int quantity, bool removeFromBank)
        {
            if (resource != 100)
            {
                if (!ToFix.ContainsKey(obj))
                    ToFix.Add(obj, new Tuple<bool, double, int>(obj.Broken, resource, quantity));
                else
                    ToFix[obj] = new Tuple<bool, double, int>(ToFix[obj].Item1, ToFix[obj].Item2, ToFix[obj].Item3 + quantity);
            }
            else
            {
                if (!Fixed.ContainsKey(obj))
                    Fixed.Add(obj, new Tuple<bool, double, int>(obj.Broken, resource, quantity));
                else
                    Fixed[obj] = new Tuple<bool, double, int>(Fixed[obj].Item1, Fixed[obj].Item2, Fixed[obj].Item3 + quantity);
            }
            if (removeFromBank)
                ObjectBank.TryRemoveResource(obj, quantity);
        }

        public void TryMoveObject(IObject obj, int quantity)
        {
            if (!Fixed.ContainsKey(obj))
            {
                int quant = ToFix[obj].Item3 >= quantity ? quantity : ToFix[obj].Item3;
                if (quant == ToFix[obj].Item3)
                    ToFix.Remove(obj);
                else
                    ToFix[obj] = new Tuple<bool, double, int>(ToFix[obj].Item1, ToFix[obj].Item2, ToFix[obj].Item3 - quantity);
                Fixed.Add(obj, new Tuple<bool, double, int>(obj.Broken, obj.Resource, quant));
            }
            else
            {
                int quant = ToFix[obj].Item3 >= quantity ? quantity : ToFix[obj].Item3;
                if (quant == ToFix[obj].Item3)
                    ToFix.Remove(obj);
                else
                    ToFix[obj] = new Tuple<bool, double, int>(ToFix[obj].Item1, ToFix[obj].Item2, ToFix[obj].Item3 - quantity);
                Fixed[obj] = new Tuple<bool, double, int>(Fixed[obj].Item1, Fixed[obj].Item2, Fixed[obj].Item3 + quant);
            }
        }

        public void TryRemoveObject(IObject obj, int quantity, bool addToBank)
        {
            if (Fixed.ContainsKey(obj))
            {
                int quant = Fixed[obj].Item3 >= quantity ? quantity : Fixed[obj].Item3;
                if (quant == Fixed[obj].Item3)
                    Fixed.Remove(obj);
                else
                    Fixed[obj] = new Tuple<bool, double, int>(Fixed[obj].Item1, Fixed[obj].Item2, Fixed[obj].Item3 - quantity);
                if (addToBank)
                    ObjectBank.TryAddResource(obj, quant);
            }
        }

        //ALL the prev methods need to be combined into the monitor process to run as a thread
        //Restoring from 0 to 100 is also a common method, also run as a thread
        //public void RestoreFix()
        //{
        //    while (ToFix.Count > 0)
        //    {
        //        //NEEDED make the next loop repeatable ONLY SOMETIMES, when the list is updated
        //        //resourceRestorages.Clear();
        //        //for (int i = 0; i < ToFix.Count; i++)
        //        //{
        //        //    resourceRestorages.Add(ObjectBank.TryGetRestoreTimeValues(ToFix.ElementAt(i).Key).Item1);
        //        //}

        //        for (int i = 0; i < ToFix.Count; i++)
        //        {
        //            if (ToFix.ElementAt(i).Value.Item2 == 100.0)
        //                TryMoveObject(ToFix.ElementAt(i).Key, ToFix.ElementAt(i).Value.Item3);
        //            if (ToFix.ElementAt(i).Value.Item2 < 0)
        //                ToFix[ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(ToFix.ElementAt(i).Value.Item1, ToFix.ElementAt(i).Value.Item2 + ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item2, ToFix.ElementAt(i).Value.Item3);
        //            else
        //                ToFix[ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(ToFix.ElementAt(i).Value.Item1, ToFix.ElementAt(i).Value.Item2 + ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item1, ToFix.ElementAt(i).Value.Item3);

        //            if (ToFix.ElementAt(i).Value.Item2 > 100)
        //                ToFix[ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(ToFix.ElementAt(i).Value.Item1, 100.0, ToFix.ElementAt(i).Value.Item3);

        //        }
        //    }
        //}

        private static void OnTimedEvent(object? sender, System.Timers.ElapsedEventArgs e, TimeSpan delta, ref TimeSpan main,
            System.Timers.Timer caller, Workshop workshop)
        {
            if (main > TimeSpan.Zero)
            {
                main = main.Subtract(delta);

                for (int i = 0; i < workshop.ToFix.Count; i++)
                {
                    if (workshop.ToFix.ElementAt(i).Value.Item2 == 100.0)
                        workshop.TryMoveObject(workshop.ToFix.ElementAt(i).Key, workshop.ToFix.ElementAt(i).Value.Item3);
                    if (workshop.ToFix.ElementAt(i).Value.Item2 < 0)
                        workshop.ToFix[workshop.ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(workshop.ToFix.ElementAt(i).Value.Item1, workshop.ToFix.ElementAt(i).Value.Item2 + workshop.ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item2, workshop.ToFix.ElementAt(i).Value.Item3);
                    else
                        workshop.ToFix[workshop.ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(workshop.ToFix.ElementAt(i).Value.Item1, workshop.ToFix.ElementAt(i).Value.Item2 + workshop.ToFix.ElementAt(i).Key.ResourceRestorageCoefs.Item1, workshop.ToFix.ElementAt(i).Value.Item3);
                    if (workshop.ToFix.ElementAt(i).Value.Item2 > 100)
                        workshop.ToFix[workshop.ToFix.ElementAt(i).Key] = new Tuple<bool, double, int>(workshop.ToFix.ElementAt(i).Value.Item1, 100.0, workshop.ToFix.ElementAt(i).Value.Item3);

                }
            }
            else
            {
                main = TimeSpan.FromSeconds(10);
            }
        }

        public void Fix()
        {
            TimeSpan timespan = TimeSpan.FromSeconds(10);
            actionTimer = new System.Timers.Timer(timespan.TotalSeconds);
            actionTimer.Interval = 1000;
            actionTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e, delta, ref timespan, actionTimer, this);
            isTimerActive = true;
            actionTimer.Start();
        }

        public void StopFixingAction()
        {
            isTimerActive = false;
            actionTimer.Stop();
        }

        public string GetInfoShort()
        {
            return "Class: " + Name + "; Name: " + CustomName + "; Coords: X " + Coords.x + ", Y " + Coords.y + "; Capacity: " + Capacity + ", free: " + 
                CurrentCapacity + "; Amount of different unit types: " + ToFix.Count;
        }

        public List<string> GetInfoDetailed()
        {
            List<string> res = new List<string>();
            res.Add("Class: " + Name + "; Name: " + CustomName + "; Coords: X " + Coords.x + ", Y " + Coords.y + "; Capacity: " + Capacity + ", free: " + CurrentCapacity + "\nIn progress:");
            for (int i = 0; i < ToFix.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(ToFix.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(ToFix.ElementAt(i).Value.Item3);
                build.Append(" in state: ");
                build.Append(ToFix.ElementAt(i).Value.Item2.ToString("0.####"));
                res.Add(build.ToString());
            }
            res.Add("\nDone:");
            for (int i = 0; i < Fixed.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(Fixed.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(Fixed.ElementAt(i).Value.Item3);
                build.Append(" in state: ");
                build.Append(Fixed.ElementAt(i).Value.Item2.ToString("0.####"));
                res.Add(build.ToString());
            }
            return res;
        }
    }

    public class Headquaters : ContainerObject, IEquatable<ContainerObject> //For StationaryObjects and tech ONLY
    {
        public ObjectBank ObjectBank { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
        public Vector2 Coords { get; set; }
        public bool isTimerActive { get; set; }
        public Timer actionTimer { get; set; }

        public Dictionary<IObject, Tuple<bool, double, int>> Contained { get; set; }

        public Headquaters(ObjectBank objectBank, string name, int capacity, Vector2 coords)
        {
            Contained = new Dictionary<IObject, Tuple<bool, double, int>>();
            ObjectBank = objectBank;
            this.Name = "Headquaters";
            this.Capacity = capacity;
            CurrentCapacity = capacity;
            CustomName = name;
            Coords = coords;
            isTimerActive = false;
            actionTimer = null;
        }
        public void SetCapacity(int c)
        {
            int occupiedCapacity = Capacity - CurrentCapacity;
            if (c > 0 && c >= occupiedCapacity)
            {
                Capacity = c;
                CurrentCapacity = Capacity - occupiedCapacity;
            }
        }

        public void ResetCapacity()
        {
            CurrentCapacity = Capacity;
        }

        public void OccupyCapacity(int amount)
        {
            if (amount >= CurrentCapacity)
                CurrentCapacity -= amount;
        }

        public void FreeCapacity(int amount)
        {
            if (CurrentCapacity + amount <= Capacity)
                CurrentCapacity += amount;
        }

        public bool Equals(ContainerObject other)
        {
            if (other == null)
                return false;
            return CustomName.Equals(other.CustomName);
        }

        public override int GetHashCode()
        {
            return CustomName.GetHashCode();
        }

        //Here are the methods common to all objects, both living and not
        public void TryAddObject(IObject obj, int quantity, bool removeFromBank)
        {
            if (!Contained.ContainsKey(obj))
                Contained.Add(obj, new Tuple<bool, double, int>(obj.Broken, obj.Resource, quantity));
            else
                Contained[obj] = new Tuple<bool, double, int>(Contained[obj].Item1, Contained[obj].Item2, Contained[obj].Item3 + quantity);
            if (removeFromBank)
                ObjectBank.TryRemoveResource(obj, quantity);
        }

        public void TryRemoveObject(IObject obj, int quantity, bool addToBank)
        {
            if (Contained.ContainsKey(obj))
            {
                int quant = Contained[obj].Item3 >= quantity ? quantity : Contained[obj].Item3;
                if (quant == Contained[obj].Item3)
                    Contained.Remove(obj);
                else
                    Contained[obj] = new Tuple<bool, double, int>(Contained[obj].Item1, Contained[obj].Item2, Contained[obj].Item3 - quantity);
                if (addToBank)
                    ObjectBank.TryAddResource(obj, quant);
            }
        }

        public string GetInfoShort()
        {
            return "Class: " + Name + "; Name: " + CustomName + "; Coords: X " + Coords.x + ", Y " + Coords.y + "; Capacity: " + Capacity + ", free: " + CurrentCapacity + "; Amount of different unit types: " +
                Contained.Count;
        }

        public List<string> GetInfoDetailed()
        {
            List<string> res = new List<string>();
            res.Add("Class: " + Name + "; Name: " + CustomName + "; Coords: X " + Coords.x + ", Y " + Coords.y + "; Capacity: " + Capacity + ", free: " + CurrentCapacity + "\nUnits:");
            for (int i = 0; i < Contained.Count; i++)
            {
                StringBuilder build = new StringBuilder();
                build.Append("\n");
                build.Append(Contained.ElementAt(i).Key.Name);
                build.Append(", number: ");
                build.Append(Contained.ElementAt(i).Value);
                res.Add(build.ToString());
            }
            return res;
        }
    }
}