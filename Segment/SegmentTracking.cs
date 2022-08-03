using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SegmentTracking : MonoBehaviour
{
#if DEVELOPMENT_BUILD
    private string _segmentApiKey = "DEV ENV SEGMENT WRITE KEY GOES HERE";
#else
    private string _segmentApiKey = "PROD ENV SEGMENT WRITE KEY GOES HERE";
#endif

    public static SegmentTracking Instance;
    private static string _authorizationHeaderValue;
    private static string _trackUrl = "https://api.segment.io/v1/track";
    // private static string _trackUrl = "http://localhost:8080/"; // use netcat to inspect the payload being sent

    public int FrameFlushSchedule = 30;

    private Queue<Item> _queue = new Queue<Item>(20);

    public class Item
    {
        public Item(string name, Hashtable properties)
        {
            Name = name;
            Properties = properties;
        }

        public string Name { get; }
        public Hashtable Properties { get; }
    }

    void Awake()
    {
        Debug.Log("Awake");
        Instance = this;

        byte[] segmentWriteKeyAsBytes = Encoding.ASCII.GetBytes(_segmentApiKey + ":");
        Debug.Log(_segmentApiKey);
        Debug.Log(segmentWriteKeyAsBytes);
        _authorizationHeaderValue = "Basic " + System.Convert.ToBase64String(segmentWriteKeyAsBytes);
    }

    void Update()
    {
        if (Time.frameCount % FrameFlushSchedule == 0)
        {
            Flush();
        }
    }

    public void Track(Item item)
    {
        Debug.Log("Tracking.Track " + item.Name);
        _queue.Enqueue(item);
    }

    public void Track(string name, Hashtable properties)
    {
        Debug.Log("Tracking.Track " + name);
        var item = new Item(name, properties);
        _queue.Enqueue(item);
    }

    public void Flush()
    {
        while (_queue.Count > 0)
        {
            var item = _queue.Dequeue();
            StartCoroutine(SendTrack(item));
        }
    }

    private class RequestPostData
    {
        public string anonymousId;
        public string _event;

        public RequestPostData(string name)
        {
            anonymousId = "anonymousId";
            _event = name;
        }
    }

    IEnumerator SendTrack(Item item)
    {
        // TODO: postData should be constructed from the Item's name and properties
        string postData = "{\"anonymousId\":\"anonymousId123\",\"event\":\"event\"}";
        byte[] postDataAsBytes = Encoding.UTF8.GetBytes(postData);

        UnityWebRequest request = new UnityWebRequest(_trackUrl);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.timeout = 30;
        request.SetRequestHeader("Authorization", _authorizationHeaderValue);
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(postDataAsBytes);
        request.uploadHandler.contentType = "application/json";

        Debug.Log(request.GetRequestHeader("Authorization"));
        Debug.Log(request.GetRequestHeader("Content-Type"));
        Debug.Log(postData);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Tracked");
        }
        request.Dispose();
    }
}
