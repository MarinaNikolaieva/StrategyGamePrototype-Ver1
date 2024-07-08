using GlobalClasses;
using Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class CountriesGenerator
{
    private List<CountryBase> Countries;
    private List<Vector2> Capitals;
    private MapPart[,] mapParts;
    private int size;

    private int minDistance = 10;
    private int minHeightForCapital = 0;
    private int maxHeightForCapital = 3000;
    private int minHeightForExpantion = -1000;
    private System.Random rand;

    private int numberOfCountries = 0;

    private static Barrier barrier;

    public CountriesGenerator(CountryReader reader, MapPart[,] mapParts, int size)
    {
        Countries = reader.GetCountries();
        Capitals = new List<Vector2>();
        this.mapParts = mapParts;
        this.size = size;
        rand = new System.Random();
        numberOfCountries = Countries.Count;
        barrier = new Barrier(0);
    }

    public void ChangeMinDistance(int dist)
    {
        if (dist > 0)
            minDistance = 10;
    }

    public void ChangeMinCapitalHeight(int height)
    {
        minHeightForCapital = height;
    }

    public void ChangeMaxCapitalHeight(int height)
    {
        maxHeightForCapital = height;
    }

    public void ChangeMinExpantionHeight(int height)
    {
        minHeightForExpantion = height;
    }

    public void ChangeCountryNumber(int number)
    {
        numberOfCountries = number;
    }

    private double GetDistanceBetweenPoints(Vector2 start, Vector2 end)
    {
        double distance = Math.Pow(end.x - start.x, 2.0) + Math.Pow(end.y - start.y, 2.0);
        distance = Math.Sqrt(distance);
        return distance;
    }

    private bool CheckCapitalPresence(int candX, int candY, List<Vector2> confirmedCountries)
    {
        for (int i = 0; i < confirmedCountries.Count; i++)
        {
            double distance = GetDistanceBetweenPoints(confirmedCountries.ElementAt(i), new Vector2(candX, candY));
            if (distance <= minDistance)
                return true;
        }
        return false;
    }

    private List<Vector2> PutCapitals(int limit)
    {
        int itersLimit = 100;
        int currentLimit = 0;
        List<Vector2> confirmedCountries = new List<Vector2>();
        for (int i = 0; i < limit; i++)
        {
            while (currentLimit < itersLimit)
            {
                int x = rand.Next(size);
                int y = rand.Next(size);
                if (mapParts[y, x].Height <= maxHeightForCapital && mapParts[y, x].Height >= minHeightForCapital)
                {
                    if (!CheckCapitalPresence(x, y, confirmedCountries))
                    {
                        confirmedCountries.Add(new Vector2(x, y));
                        currentLimit++;
                        break;
                    }
                    currentLimit++;
                }
                currentLimit++;
            }
        }

        return confirmedCountries;
    }

    private void SetCountriesToCapitals(int limit)
    {
        for (int i = 0; i < limit; i++)
        {
            mapParts[(int)Capitals.ElementAt(i).y, (int)Capitals.ElementAt(i).x].Country = Countries.ElementAt(i);
            Countries.ElementAt(i).Capital = new Vector2((int)Capitals.ElementAt(i).x, (int)Capitals.ElementAt(i).y);
        }
    }

    private void CheckAndExpand(MapPart curPart, CountryBase country, int i, List<Vector2> edgeParts)
    {
        if (curPart.neighbors.ElementAt(i).Height > minHeightForExpantion)
        {
            curPart.neighbors.ElementAt(i).Country = country;
            edgeParts.Add(new Vector2(curPart.neighbors.ElementAt(i).X, curPart.neighbors.ElementAt(i).Y));
        }
    }

    private void FillCapitalSurrounding(object locker, List<Vector2> edgeParts, CountryBase country)
    {
        while (edgeParts.Count > 0)
        {
            Vector2 curCoords = edgeParts.ElementAt(rand.Next(edgeParts.Count));
            MapPart curPart = mapParts[(int)curCoords.y, (int)curCoords.x];
            lock (locker)
            {
                for (int i = 0; i < curPart.neighbors.Count; i++)
                    if (curPart.neighbors.ElementAt(i).Country == null)
                    {
                        double distance = GetDistanceBetweenPoints(curCoords, new Vector2(curPart.neighbors.ElementAt(i).X, curPart.neighbors.ElementAt(i).Y));
                        //It was just minDistance here, MAYBE divide by 2?
                        if (distance <= minDistance / 2)
                        {
                            CheckAndExpand(curPart, country, i, edgeParts);
                        }
                    }
            }
            edgeParts.Remove(curCoords);
        }
    }

    private void FillCapitalAround(object locker, List<Vector2> edgeParts, CountryBase country)
    {
        Vector2 cap = edgeParts.First();
        int maxDist = minDistance / 2;
        float maxDistMinusOne = (float)Math.Pow(maxDist - 1, 2);
        for (int x = (int)cap.x - maxDist; x <= (int)cap.x; x++)
        {
            for (int y = (int)cap.y - maxDist; y <= (int)cap.y; y++)
            {
                // we don't have to take the square root, it's slow
                float temp = (x - cap.x) * (x - cap.x) + (y - cap.y) * (y - cap.y);
                if (temp <= maxDist * maxDist)
                {
                    int xSym = (int)(cap.x - (x - cap.x));
                    int ySym = (int)(cap.y - (y - cap.y));
                    // (x, y), (x, ySym), (xSym , y), (xSym, ySym) are in the circle

                    lock (locker)
                    {
                        try
                        {
                            if (mapParts[y, x].Country == null)
                                mapParts[y, x].Country = country;
                            if (temp >= maxDistMinusOne)
                                edgeParts.Add(new Vector2(x, y));
                        }
                        catch (Exception ex) { }
                        try
                        {
                            if (mapParts[y, xSym].Country == null)
                                mapParts[y, xSym].Country = country;
                            if (temp >= maxDistMinusOne)
                                edgeParts.Add(new Vector2(xSym, y));
                        }
                        catch (Exception ex) { }
                        try
                        {
                            if (mapParts[ySym, x].Country == null)
                                mapParts[ySym, x].Country = country;
                            if (temp >= maxDistMinusOne)
                                edgeParts.Add(new Vector2(x, ySym));
                        }
                        catch (Exception ex) { }
                        try
                        {
                            if (mapParts[ySym, xSym].Country == null)
                                mapParts[ySym, xSym].Country = country;
                            if (temp >= maxDistMinusOne)
                                edgeParts.Add(new Vector2(xSym, ySym));
                        }
                        catch (Exception ex) { }
                    }
                }
            }
        }
    }

    private void FillTerritory(object locker, List<Vector2> edgeParts, CountryBase country)
    {
        while (edgeParts.Count > 0)
        {
            Vector2 curCoords = edgeParts.ElementAt(rand.Next(edgeParts.Count));
            MapPart curPart = mapParts[(int)curCoords.y, (int)curCoords.x];
            lock (locker)
            {
                for (int i = 0; i < curPart.neighbors.Count; i++)
                    if (curPart.neighbors.ElementAt(i).Country == null)
                    {
                        CheckAndExpand(curPart, country, i, edgeParts);
                    }
            }
            edgeParts.Remove(curCoords);
        }
    }

    //This method must be run in parallel, a thread for each country
    private void SpreadOneCountry(CountryBase country, Vector2 origin, bool multipleThreads)
    {
        //The origin already has its country added
        object locker = new object();  //This is the thing I'll use for locking the map

        List<Vector2> edgeParts = new List<Vector2> { origin };  //These are the parts able to spread

        //First thing - fill the circle around the Origin (capital)
        FillCapitalAround(locker, edgeParts, country);
        //Barrier is here so that all countries FIRST fill the place around the capitals
        //And THEN get to filling the territory around
        //With this, all countries will stay on the map and won't be deleted
        if (multipleThreads)
            barrier.SignalAndWait();
        //And then spread while possible
        FillTerritory(locker, edgeParts, country);
    }

    //One more method - to check if there are parts of land without the countries set
    private void CheckEmptyLand(int limit)
    {
        object locker = new object();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (mapParts[i, j].Height > minHeightForCapital && mapParts[i, j].Country == null)
                {
                    //If such part exists, select a random country
                    CountryBase country = Countries.ElementAt(rand.Next(limit));
                    mapParts[i, j].Country = country;
                    SpreadOneCountry(country, new Vector2(j, i), false);
                }
            }
        }
    }

    //And now the method to start the threads and make a map
    public int RunGeneration()
    {
        int limit = numberOfCountries == 0 ? Countries.Count : numberOfCountries;

        Capitals = PutCapitals(limit);
        if (Capitals.Count < limit)
            return -1;
        SetCountriesToCapitals(limit);
        if (Countries.Count == 1)
            SpreadOneCountry(Countries.ElementAt(0), Capitals.ElementAt(0), false);
        else
        {
            List<Thread> threads = new List<Thread>();
            foreach (var country in Countries)
            {
                barrier.AddParticipant();
                threads.Add(new Thread(() => SpreadOneCountry(country, country.Capital, true)));
                if (threads.Count == limit)
                    break;
            }
            for (int i = 0; i < threads.Count; i++)
                threads.ElementAt(i).Start();
            for (int i = 0; i < threads.Count; i++)
                threads.ElementAt(i).Join();
        }
        CheckEmptyLand(limit);
        return 0;
    }

    public Texture2D MakePicture()
    {
        Texture2D image = new Texture2D(size, size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                CountryBase country = mapParts[i, j].Country;
                if (country != null && Capitals.Contains(new Vector2(j, i)))
                    image.SetPixel(i, j, Color.black);
                else if (country != null)
                    image.SetPixel(i, j, country.Color);
                else
                    image.SetPixel(i, j, Color.white);  //White color = neutral
            }
        }
        image.Apply();
        return image;
    }
}
