using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
public class testSenaroGentrater2: MonoBehaviour
{
    public int numberOfDrones;
    public float y0;
    public float yFinal;
    public float placementRadius, tragetRadius;
    void Start()
    {

       

        Debug.Log("points Gnareted");
        List<objectState> stateList = new List<objectState>();
        float eps = Mathf.PI * 2 / numberOfDrones;
        int count = 0;
        for (float i = 0; i < 2 * Mathf.PI; i += eps)
        {
            Vector3 unit = new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i));
            Vector3 startLoc = unit * placementRadius;
            Vector3 endLoc = unit * tragetRadius;
            startLoc.y = y0;
            endLoc.y = yFinal;

            

            Dictionary<string, string> uavData = new Dictionary<string, string>();
            uavData["mass"] = "4";
            string[] cmdList = new string[7];
            cmdList[0] = "powerOFF";
            cmdList[1] = "setSpeed 15";
            cmdList[2] = "wait s" + Random.Range(0, 2 * 60);
            cmdList[3] = "powerON";
            cmdList[4] = "move x" + endLoc.x + " y" + endLoc.y + " z" + endLoc.z;
            cmdList[5] = "move x" + endLoc.x + " y" + "0.8" + " z" + endLoc.z;

            cmdList[6] = "powerOFF";

            uavObjectState state = new uavObjectState("drone " + count.ToString(), "basicDrone");
            state.position = startLoc;
            state.uavData = uavObjectState.makeListFromDictionary(uavData);
            state.cmdList = cmdList;

            stateList.Add(state);
            count++;



        }


        string path = "coedeGenrtatedCase" + System.DateTime.Now.ToFileTimeUtc().ToString() + ".xml";
        Stream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);

        XmlSerializer serializer = new XmlSerializer(typeof(List<objectState>), new[] { typeof(visibleObjectState), typeof(uavObjectState), typeof(List<KeyValuePair<string, string>>) });
        var sw = new StreamWriter(fileStream);
        serializer.Serialize(sw, stateList);
        sw.Flush();
        sw.Close();

    }

 
    // Update is called once per frame
    void Update()
    {

    }
}
