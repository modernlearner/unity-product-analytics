// 1. Create a new GameObject and attach both the SegmentTracking and ExampleTracker scripts to it
// 2. For a UI Button, find the ExampleTracker script and the function to call, TrackTestButton
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTracker : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("ExampleTracker.Awake");
    }

    public void TrackTestButton()
    {
        Hashtable properties = new Hashtable();
        properties.Add("Name1", "Value1");
        properties.Add("Name2", "Value2");
        SegmentTracking.Instance.Track("Hello world", properties);
    }
}
