using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
public class testSenaroGentrater1 : MonoBehaviour {
	public float AreaWidth;
	float minX,minZ,maxX,maxZ;
	public int numberOfDrones;
	// Use this for initialization
	public float droneRaduis;
	public float Y;
	int maxNumberOfTrys = 1000;
	public int numberOfMoveCmds;
	List<Vector3> currentStartLocations;
    int numberOfDronesIndex;
    int[] droneNumbers = { 500,750,1000,1500 };
    void makeCase()
    {
        minX = -AreaWidth / 2;
        maxX = -minX;
        maxZ = maxX;
        minZ = minX;
        if (PlayerPrefs.HasKey("numberOfDronesIndex"))
            numberOfDronesIndex = PlayerPrefs.GetInt("numberOfDronesIndex");
        else
            numberOfDronesIndex = 0;
        numberOfDrones = droneNumbers[numberOfDronesIndex];
      
         
        currentStartLocations = new List<Vector3>();
        for (int i = 0; i < numberOfDrones; i++)
        {
            bool canAdd = false;
            Vector3 point = Vector3.zero;
            int trys = 0;
            while (!canAdd)
            {
                point = getRandomVector();
                canAdd = true;
                trys++;
                for (int j = 0; j < currentStartLocations.Count; j++)
                {
                    if ((currentStartLocations[j] - point).magnitude < droneRaduis * 2)
                    {
                        canAdd = false;
                        break;
                    }
                }
                if (trys >= maxNumberOfTrys)
                {
                    Debug.Log("satruation");
                    break;
                }


            }
            if (canAdd)
            {
                currentStartLocations.Add(point);
                //	Debug.Log (point);
            }
        }

        //Debug.Log("points Gnareted");
        List<objectState> stateList = new List<objectState>();
        int count = 0;
        
        foreach (var x in currentStartLocations)
        {

            Dictionary<string, string> uavData = new Dictionary<string, string>();
            uavData["mass"] = "4";
            string[] cmdList = new string[numberOfMoveCmds + 1];
            cmdList[0] = "setSpeed 15";
            for (int k = 0; k < numberOfMoveCmds; k++)
            {
                Vector3 endLocation = getRandomVector();
                cmdList[k + 1] = "move  x" + endLocation.x.ToString() + "  y" + endLocation.y.ToString() + " z" + endLocation.z.ToString();

            }

            uavObjectState state = new uavObjectState("drone " + count.ToString(), "basicDrone");
            state.position = x;
            state.uavData = uavObjectState.makeListFromDictionary(uavData);
            state.cmdList = cmdList;

            stateList.Add(state);

            count++;
        }
        string path = "collitionsCases/" + numberOfDrones + "/conf.xml";
        Directory.CreateDirectory("collitionsCases/" + numberOfDrones);
        PlayerPrefs.SetString("configPath", path);
        PlayerPrefs.SetString("outputFolder", "collitionsCases/" + numberOfDrones + "/output");

        Stream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);

        XmlSerializer serializer = new XmlSerializer(typeof(List<objectState>), new[] { typeof(visibleObjectState), typeof(uavObjectState), typeof(List<KeyValuePair<string, string>>) });
        var sw = new StreamWriter(fileStream);
        serializer.Serialize(sw, stateList);
        sw.Flush();
        sw.Close();
    }
    double lastSenceStartTime = 0;
	void Start () {

        Application.runInBackground = true;
        PlayerPrefs.SetFloat("simTime", 60 * 2);
        if (PlayerPrefs.HasKey("numberOfDronesIndex"))
            numberOfDronesIndex = PlayerPrefs.GetInt("numberOfDronesIndex");
        else
            numberOfDronesIndex = 0;
        if (numberOfDronesIndex == droneNumbers.Length)
        {
            PlayerPrefs.DeleteAll();
            Application.Quit();
        }
        else if (!Application.isLoadingLevel)
        {

            PlayerPrefs.SetInt("numberOfDronesIndex", numberOfDronesIndex);
            makeCase();
            Application.LoadLevel("emptyPlane");
        }
        numberOfDronesIndex++;
        PlayerPrefs.SetInt("numberOfDronesIndex", numberOfDronesIndex);

    }


    Vector3 getRandomVector()
	{
		return new Vector3 (UnityEngine.Random.Range(minX,maxX),Y,UnityEngine.Random.Range(minZ,maxZ));
	}
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
    public void FixedUpdate()
    {
     //   Debug.Log("time = " +   Time.timeSinceLevelLoad.ToString());
        //if(  Time.timeSinceLevelLoad-lastSenceStartTime>60f)
        //{
            
        //    Application.UnloadLevel("emptyPlane");
        //    numberOfDrones = PlayerPrefs.GetInt("numOfdrone");
        //    numberOfDrones += 5;
        //    if (numberOfDrones > 50) Application.Quit();
        //    makeCase();
        //    lastSenceStartTime =   Time.timeSinceLevelLoad;
        //    PlayerPrefs.SetInt("numOfdrone", numberOfDrones);
        //    Application.LoadLevel("emptyPlane");
        //}
    }
    // Update is called once per frame
    void Update () {
		
	}
}
