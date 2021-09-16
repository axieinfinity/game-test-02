using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class FPSCounter : Singleton<FPSCounter>
{
    
    public int granularity = 5;
    List<double> times = new List<double>();
    int counter = 5;

    public double FPS { get; set; } = 60;

    public void Start ()
    {
        times = new List<double>();
    }

    public void Update ()
    {
        if (counter <= 0)
        {
            CalcFPS ();
            counter = granularity;
        } 

        times.Add (Time.unscaledDeltaTime);
        counter--; 
    }

    public void CalcFPS ()
    {
        var deltaTime = times.Min();// times.Sum() / times.Count; // 
        FPS = 1f / deltaTime;
        // Debug.LogError(FPS);
        times.Clear();
    }
}
