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
        entry.state = "sent";
        entry.from = sendr.name;
        entry.to = rcever.name;
		entry.time = Time.time;
        entry.distnace = (sendr.transform.position - rcever.transform.position).magnitude;
        rcever.readMessage(msg, sendr);
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
