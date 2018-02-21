using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

using System.IO;
public class loadEnv : MonoBehaviour {
   public static string folderName;
    // Use this for initialization
    double startTime;
    Stream outputStream;
    StreamWriter outputStreamWriter;
    Stream summaryStream;
    StreamWriter summaryStremWrite;
    void Start () {
        startTime =   Time.timeSinceLevelLoad;
        XmlSerializer serializer = new XmlSerializer(typeof(List<objectState>), new[] { typeof(visibleObjectState), typeof(uavObjectState), typeof(List<KeyValuePair<string, string>>) });
        string path = "conf.xml";
        if (PlayerPrefs.HasKey("configPath"))
            path = PlayerPrefs.GetString("configPath");      
        folderName = System.DateTime.Now.ToFileTimeUtc().ToString();
        if (PlayerPrefs.HasKey("outputFolder"))
            folderName = PlayerPrefs.GetString("outputFolder");
        if(Directory.Exists(folderName))
            Directory.Delete(folderName);
        
        Directory.CreateDirectory(folderName);
        Stream file = File.Open(path,FileMode.Open);
        List<objectState> list =  serializer.Deserialize(file) as List<objectState>;
        string summrayFilePath =folderName+ "/summary.txt";
        summaryStream = File.Open(summrayFilePath, FileMode.OpenOrCreate);
        summaryStremWrite = new StreamWriter(summaryStream);

        foreach(var i in list)
        {
            i.createGameObject();
          //  Debug.Log(i.name);
        }

    }
    private void FixedUpdate()
    {
        //Debug.Log("time " +   Time.timeSinceLevelLoad);
        if(PlayerPrefs.HasKey("simTime"))
            if((  Time.timeSinceLevelLoad-startTime) >= PlayerPrefs.GetFloat("simTime"))
            {
                Application.LoadLevel("test");
            }
        var droneList = FindObjectsOfType<UAV>();
        int numberOfDrones = droneList.Length;
        summaryStremWrite.WriteLine(  Time.timeSinceLevelLoad.ToString()+","+ numberOfDrones);
    }
    private void OnDestroy()
    {
        summaryStremWrite.Flush();
        summaryStremWrite.Close();
        
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
    // Update is called once per frame
    void Update () {
		
	}
}
