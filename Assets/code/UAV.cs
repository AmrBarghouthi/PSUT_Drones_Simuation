using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;

public class UAV : MonoBehaviour
{

    // Use this for initialization
   // public Image mapIcon;
    public string[] cmdList { set; get; }
    public uint currentCmdIndex = 0;
    // public string droneId {set; get;} using the bultin unity monoBehavior name attr

    protected droneEventsLogger eventLogger;
    protected Stream outputStream;
	protected StreamWriter outputStreamWriter;
    protected Stream eventsOutputStream;
   // protected StreamWriter eventsOutputStreamWriter;

    protected bool cmdDone = true;

    protected List<GameObject> inRageObjects;
    public string lastFetchedCmd;
    protected string fetchCmad()
    { 
        if (cmdList.Length <= currentCmdIndex)
        {
            lastFetchedCmd = "wait";
            return lastFetchedCmd;
            //     Debug.Log("no new commd");
        }
        else lastFetchedCmd = cmdList[currentCmdIndex];
        currentCmdIndex++;
        return lastFetchedCmd;
    }
	public void  Start()
    {  
		//Debug.Log ("base Start exexuting");
       
		outputStream = File.Open(loadEnv.folderName + "/"+this.name+".txt",FileMode.CreateNew);
		outputStreamWriter = new StreamWriter(outputStream);
        eventsOutputStream = File.Open(loadEnv.folderName + "/" + this.name + "_events.xml", FileMode.CreateNew);
        //eventsOutputStreamWriter = new StreamWriter(eventsOutputStream);
        currentCmdIndex = 0;
        //    MiniMapController.RegisterMapObject(this.gameObject, mapIcon);
        //   Dropdown dd = GameObject.Find("dronesDropDown").GetComponent<Dropdown>();
        // dd.options.Add(new Dropdown.OptionData(this.name,mapIcon.sprite));
        eventLogger = new droneEventsLogger();
        inRageObjects = new List<GameObject>();
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
         //each drone should implenmintes its own code
    }
    public virtual void setupFromDictionary(Dictionary<string, string> src)
    {
        
    }
    public virtual Dictionary<string, string> makeDictionary()
    {
        
        Dictionary <string, string> UAVData =  new Dictionary<string, string>();
      /*  UAVData["mass"] = (this.GetComponent<Rigidbody>() as Rigidbody).mass.ToString();
        UAVData["drag"] = (this.GetComponent<Rigidbody>() as Rigidbody).drag.ToString();*/
        return UAVData;
    }
    public virtual void sendMessage(object msg,UAV target)
    {
        comminaction air = FindObjectOfType<comminaction>();
        air.routeMsg(this, target, msg);
       // target.readMessage(msg, this);
        droneEventLogEntry entry = new droneEventLogEntry();
        entry.cmd = lastFetchedCmd;
        entry.entryType = "new message sent ";
        entry.position = this.transform.position;
        entry.rotation = this.transform.rotation.eulerAngles;
        Rigidbody rb = GetComponent<Rigidbody>();
        entry.velocity = rb.velocity;
        entry.time = Time.time;
        //  entry.relativeVelocity = collision.relativeVelocity;
        entry.info = msg.ToString();
        entry.otherName = target.name;
        UAV otherUAV = target.gameObject.GetComponent<UAV>();
        if (otherUAV != null)
            entry.otherCmd = otherUAV.lastFetchedCmd;
        eventLogger.logs.Add(entry);
    }
    public virtual void readMessage(object msg,UAV src)
    { 
        droneEventLogEntry entry = new droneEventLogEntry();
        entry.cmd = lastFetchedCmd;
        entry.entryType = "new message recived ";
        entry.position = this.transform.position;
        entry.rotation = this.transform.rotation.eulerAngles;
        Rigidbody rb = GetComponent<Rigidbody>();
        entry.velocity = rb.velocity;
        entry.time = Time.time;
        //  entry.relativeVelocity = collision.relativeVelocity;
        entry.info = msg.ToString();
        entry.otherName = src.name;
        UAV otherUAV = src.gameObject.GetComponent<UAV>();
        if (otherUAV != null)
            entry.otherCmd = otherUAV.lastFetchedCmd;
        eventLogger.logs.Add(entry);
    }
    public void OnDestroy()
    {
       //  outputStream.Flush();
       // outputStream.Close();
    }
}
