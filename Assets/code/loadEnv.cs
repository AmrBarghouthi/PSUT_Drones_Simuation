using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

using System.IO;
public class loadEnv : MonoBehaviour {
   public static string folderName;
    // Use this for initialization

    Stream outputStream;
    StreamWriter outputStreamWriter;
    void Start () {
        XmlSerializer serializer = new XmlSerializer(typeof(List<objectState>), new[] { typeof(visibleObjectState), typeof(uavObjectState), typeof(List<KeyValuePair<string, string>>) });
        string path = "conf.xml";
        folderName = System.DateTime.Now.ToFileTimeUtc().ToString();
 
        Directory.CreateDirectory(folderName);
        Stream file = File.Open(path,FileMode.Open);
        List<objectState> list =  serializer.Deserialize(file) as List<objectState>;
        foreach(var i in list)
        {
            i.createGameObject();
        }


    }

    // Update is called once per frame
    void Update () {
		
	}
}
