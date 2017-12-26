using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class genrateEnv : MonoBehaviour {

    public uint droneCount;
    public uint targetCount;
    public uint obsCount;
    public float maxX;
    public float maxY;
    public float maxZ;
    public float minX;
    public float minY;
    public float minZ;
    // Use this for initialization
    private GameObject[] obs;
    public static GameObject[] tar;
    private GameObject[] drones;
    public static uint tarCount;
    MemoryStream startStateStream;
   

    void Start () {
        GameObject obsRoot,tarRoot,droneRoot;
        obsRoot = new GameObject("obs");
        tarRoot = new GameObject("tars");
        droneRoot = new GameObject("drons");
        startStateStream = new MemoryStream();
        string path = System.DateTime.Now.ToFileTimeUtc().ToString() + ".xml";
        Dictionary<string, string> x;
        
        System.IO.StreamWriter outputStream = new StreamWriter(startStateStream);
        XmlSerializer serializer =   new XmlSerializer(typeof(List<objectState>),new []{ typeof(visibleObjectState),typeof(uavObjectState),typeof(List<KeyValuePair<string,string>>)});
        tarCount = targetCount;
        obs = new GameObject[obsCount];
        drones = new GameObject[droneCount];
        tar = new GameObject[targetCount];
        
        List<objectState> stateList = new List<objectState>();
        for (int i = 0; i < obsCount; i++)
        {
            Vector3 randomPoint = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));

            objectState state = new objectState();
            state.position = randomPoint;
            state.name = "obs " + i.ToString();
            obs[i] = state.createGameObject(obsRoot.transform);

            // state.reportState(ref outputStream);
            stateList.Add(state);

        }
        for (int i = 0; i < targetCount; i++)
        {
            Vector3 randomPoint = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));


            visibleObjectState state = new visibleObjectState("tar " + i.ToString(), "Cube");
            state.position = randomPoint;

            tar[i] = state.createGameObject(tarRoot.transform);

            stateList.Add(state);


        }
        Dictionary<string, string> uavData = new Dictionary<string, string>();
        uavData["mass"] = "1";
        string []cmdList = new string[3];
        cmdList[1] = "goToObject s1";
        cmdList[2] = "move  x200  y13 z200";
        cmdList[0] = "setSpeed 30";
        for (int i = 0; i < droneCount; i++)
        {
            Vector3 randomPoint = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            uavObjectState state = new uavObjectState("drone " + i.ToString(), "simpleDrone");
            state.position = randomPoint;
            state.uavData = uavObjectState.makeListFromDictionary(uavData) ;
            state.cmdList = cmdList;
            drones[i] = state.createGameObject(droneRoot.transform);
            
            stateList.Add(state);


        }
        serializer.Serialize(outputStream, stateList);
        
        outputStream.Flush();
        writeSimultionStartState(path);
        // startStateStream;
    }
    public void writeSimultionStartState(string path)
    {
         
        Stream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);    
        startStateStream.WriteTo(fileStream);
        fileStream.Close();
    }
    // Update is called once per frame
    void Update () {

    }
}
