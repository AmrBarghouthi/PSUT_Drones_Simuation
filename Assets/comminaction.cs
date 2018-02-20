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

        XmlSerializer temp = new XmlSerializer(typeof(comminactionLogger));
        temp.Serialize(outStream, loger);

        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
    }
    void Update () {

	}
}
