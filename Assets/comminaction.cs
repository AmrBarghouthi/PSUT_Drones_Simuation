using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
public class comminaction : MonoBehaviour {


	// Use this for initialization
	void Start () {
        loger = new comminactionLogger();
	}
    comminactionLogger loger;
	public void routeMsg(UAV sendr,UAV rcever,object msg)
    {


        comminactionLogEntry entry = new comminactionLogEntry();
        entry.msg = msg.ToString();

        entry.from = sendr.name;
        
        entry.time =   Time.timeSinceLevelLoad;


        if (rcever!=null)
        {
            float distnace = (sendr.transform.position - rcever.transform.position).magnitude;
            entry.to = rcever.name;
            entry.recivedby.Add(new reviverData(rcever.name, distnace));
            rcever.readMessage(msg, sendr);
          
       }else{
            var allUav = GameObject.FindObjectsOfType<UAV>();
            entry.to = "ALL";
            foreach (var x in allUav)
            {
                float distnace = (sendr.transform.position - x.transform.position).magnitude;
                if (x == sendr) continue;
                if (sendr.inRageObjects.Contains(x.gameObject))
                    entry.recivedby.Add(new reviverData(x.name,distnace));
                else
                    entry.faildToReciveBy.Add(new reviverData(x.name,distnace));
            }
        
       }
        loger.logs.Add(entry);
    }
    void OnDestroy()
    {
        Stream outStream = new FileStream(loadEnv.folderName + "/comm.xml", FileMode.Create);
        Stream outStreamJson = new FileStream(loadEnv.folderName + "/comm.json",FileMode.Create);
        StreamWriter jsonWriter = new StreamWriter(outStreamJson);
        jsonWriter.Write(JsonUtility.ToJson(loger,true));
        XmlSerializer temp = new XmlSerializer(typeof(comminactionLogger));
        temp.Serialize(outStream, loger);
        jsonWriter.Flush();
        jsonWriter.Close();
        jsonWriter.Dispose();
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
    }
    void Update () {

	}
}
