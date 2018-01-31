using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
public class droneWithPid : UAV {

    protected float weight = 3;//not used yet
                               //	protected float lenght;//not used yet
                               //	protected float width;//not used yet
    protected float max_speed = float.PositiveInfinity;//if target speed is less than max the max is used
                                                       //	protected float min_speed;//not used yet
                                                       //variables for the recharge command
    float chrg;
    float waitTime = 1;
    float updateMsgPeriod = 10;
    float updateMsgTimer = 0;
    //end
    //variables for setSpeed command
    float setspeed;
    //end
    //variables defined by amr
    public float speed { set; get; }
    public Vector3 target;
    //****************************************************
    // public float speed; i defined speed in the UAV class
    //****************************************************
    public PID speedContX, speedContY, speedContZ;
    public float speedLoopRadius;

    public PID posX,posY,posZ;
    float speedSetPoint;
    private string curCmd;
    protected float charge;
    //protected float chargePercentage;
    //end of vriables defined by amr
    // Use this for initialization

    public new void Start()
    {

        //target = new Vector3(100,200,300);

        (this as UAV).Start();
        rb = this.GetComponent<Rigidbody>();
        //cmdList = new string[2];
        //cmdList[1] = "move  x500";
        //cmdList[0] = "setSpeed 10";



    }
    // Update is called once per frame
    void decode()
    {
        curCmd = fetchCmad();
        //Debug.Log("decodeing :" + curCmd);

        if (curCmd.Contains("goToObject"))
        {
            string[] temp = curCmd.Split(' ');
            string targtBaseName = temp[1];
            GameObject go = GameObject.Find(targtBaseName);
            if (go != null)
            {

                target = go.transform.position;

            }
            else
            {
                Debug.Log("the object dose not existes");
            }
            curCmd = "move";

        }else
        if (curCmd.Contains("move"))
        {
            string[] subCmd = curCmd.Split(' ');
            bool isRletive = false;
            bool x=false, y=false, z=false;
            for (int subCmdIndex = 0; subCmdIndex < subCmd.Length; subCmdIndex++)
            {//generlized the synticx of the move cmd ex: "move x1 y2" = "move y2 x1"
                if (subCmd[subCmdIndex].Contains("x")) { x = true; target.x = float.Parse(subCmd[subCmdIndex].Substring(1)); }

                if (subCmd[subCmdIndex].Contains("y")) { y = true; target.y = float.Parse(subCmd[subCmdIndex].Substring(1)); }

                if (subCmd[subCmdIndex].Contains("z")) { z = true; target.z = float.Parse(subCmd[subCmdIndex].Substring(1)); }

                if (subCmd[subCmdIndex].Contains("r")) isRletive = true;
//                Debug.Log(subCmd[subCmdIndex]);

            }
            if (!x)
                target.x = transform.position.x;
            if (!y)
                target.y = transform.position.y;
            if (!z)
                target.z = transform.position.z;
            if (isRletive)
                target += this.transform.position;
            curCmd = "move";
        }
        if (curCmd.Contains("wait"))
        {
            if (curCmd.Contains("s"))
            {
                string temp = curCmd.Substring(curCmd.IndexOf('s') + 1);
                waitTime = float.Parse(temp);
            }
            else
                waitTime = float.PositiveInfinity;

        }
        //change position orders drone to move certain displacement while move command
        //orders drone to go to a new position at x,y,z


        // recharge goes here it wiil be set later when we agree on a certain power conumption scenario
        if (curCmd.Contains("recharge"))
        {
            string[] subCmd = curCmd.Split(' ');
            chrg = float.Parse(subCmd[1]);
            curCmd = "recharge";

            /// code to change the charge persentage goes here also
        }
        // set speed goes here
        if (curCmd.Contains("setSpeed"))
        {

            string[] subCmd = curCmd.Split(' ');
            setspeed = float.Parse(subCmd[1]);
            //     Debug.Log("speed:"+setspeed.ToString());
            curCmd = "setSpeed";
        }



        cmdDone = false;

    }
    Rigidbody rb;
    void FixedUpdate()
    {
      //  Debug.Log(name + " inRageObjects " + inRageObjects.Count);

        //decode and execute commands
        if (cmdDone == true)
            decode();
        //no need for eles strat exexuting the new cmd on the smae time unit it's decoded
        {
            //Debug.Log(name+" exeuting " + cmdList[currentCmdIndex-1]);


            /////change position
            if (curCmd == "move" || curCmd =="wait")
            {
                //Debug.Log ("target "+target);
                Vector3 err = target - transform.position;


                // float remaningTime = dir.magnitude / speed;

                Vector3 dir = err.normalized;
                //Debug.Log ("dir" + dir);
                //dir = new Vector3(1, 0, 0);

              //  Debug.Log(rb.velocity.magnitude);
             //   Debug.Log(target);

                float driveX, driveY, driveZ;
                if (err.magnitude > speedLoopRadius)
                {
                    driveX = speedContX.Update(dir.x * speed, rb.velocity.x, Time.deltaTime);
                    driveY = speedContX.Update(dir.y * speed, rb.velocity.y, Time.deltaTime);
                    driveZ = speedContX.Update(dir.z * speed, rb.velocity.z, Time.deltaTime);
                }
                else
                {
                    driveX = posX.Update(target.x, transform.position.x, Time.deltaTime);
                    driveY = posY.Update(target.y, transform.position.y, Time.deltaTime);
                    driveZ = posZ.Update(target.z, transform.position.z, Time.deltaTime);
                }
                rb.AddForce(new Vector3(driveX, driveY, driveZ));
                //Debug.Log(rb.velocity);
               // this.transform.Translate(dir * Time.deltaTime * speed, Space.World);
                if ((target - transform.position).magnitude < 0.2)
                {
                    cmdDone = true;

                }



            }
            else if (curCmd == "recharge")
            {
                this.charge += chrg;
                cmdDone = true;

            }
            else if (curCmd == "setSpeed")
            {
                if (setspeed >= 0)
                    this.speed = (setspeed < max_speed) ? setspeed : max_speed;
                else
                {
                    Debug.Log("not vallid negtive speed no chagange ouccers");
                }
                cmdDone = true;
            }
            else if (curCmd == "wait")
            {
                Debug.Log("remanig time to wait" + waitTime);
                if (waitTime > 0)
                    waitTime -= Time.deltaTime;
            }
            else
                cmdDone = true;
        }

        if(updateMsgTimer>=updateMsgPeriod)
        {
            sendPeriodicMsgs();
            updateMsgTimer = 0;
        }
        updateMsgTimer += Time.deltaTime;
        outputStreamWriter.WriteLine(Time.time.ToString() + ","
            + transform.position.x + ","
            + transform.position.y + ","
            + transform.position.z + ","
            + transform.rotation.eulerAngles.x + ","
            + transform.rotation.eulerAngles.y + ","
            + transform.rotation.eulerAngles.z);

    }
    void sendPeriodicMsgs()
    {

                sendMessage("update", null);
         
    }
    void OnCollisionEnter(Collision collision)
    {

        //Debug.Log("collision");
        //eventsOutputStreamWriter.WriteLine("collisionData");
        //eventsOutputStreamWriter.WriteLine("time = " +Time.time);
        //eventsOutputStreamWriter.WriteLine("object = "+collision.transform.name);
        //eventsOutputStreamWriter.WriteLine("EndcollisionData\n");
        droneEventLogEntry entry = new droneEventLogEntry();
        entry.cmd = lastFetchedCmd;
        entry.entryType = "collision";
        entry.position = this.transform.position;
        entry.rotation = this.transform.rotation.eulerAngles;
        entry.velocity = rb.velocity;
        entry.time = Time.time;
        entry.relativeVelocity = collision.relativeVelocity;
        entry.otherName = collision.gameObject.name;
        UAV otherUAV = collision.gameObject.GetComponent<UAV>();
        if (otherUAV != null)
           entry.otherCmd = otherUAV.lastFetchedCmd;
        eventLogger.logs.Add(entry);

        cmdList = new string[2];
        cmdList[0] = "move y0 x"+transform.position.x.ToString()+" z"+ transform.position.z.ToString();
        cmdList[1] = "wait";
        currentCmdIndex = 0;
        cmdDone = true;

       // Debug.Log(name + " cs LogSize:" + eventLogger.logs.Count);

    }

    public override void setupFromDictionary(Dictionary<string, string> src)
    {
        // read poprties and setupData
        if (src.ContainsKey("weight"))
            weight = float.Parse(src["weight"]);
        else Debug.Log("weight is missing in " + base.name);



        if (src.ContainsKey("speed"))
            speed = float.Parse(src["speed"]);
        else Debug.Log("speed is missing in " + base.name);



        if (src.ContainsKey("max_speed"))
            max_speed = float.Parse(src["max_speed"]);
        else Debug.Log("max_speed is missing in " + base.name);


        if (src.ContainsKey("chrg"))
            chrg = float.Parse(src["chrg"]);
        else Debug.Log("chrg is missing in " + base.name);


    }
    public override Dictionary<string, string> makeDictionary()
    {

        Dictionary<string, string> UAVData = new Dictionary<string, string>();
        UAVData["weight"] = weight.ToString();
        UAVData["speed"] = speed.ToString();
        UAVData["max_speed"] = max_speed.ToString();
        UAVData["chrg"] = chrg.ToString();
        return UAVData;
    }
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name+" is in range");
        //eventsOutputStreamWriter.WriteLine("newObjectInRange");
        //eventsOutputStreamWriter.WriteLine("time = " + Time.time);
        //eventsOutputStreamWriter.WriteLine("object = " + other.name);
        //eventsOutputStreamWriter.WriteLine("endnewObjectInRange\n");
        if (inRageObjects.Contains(other.gameObject))
            return;
        inRageObjects.Add(other.gameObject);
        droneEventLogEntry entry = new droneEventLogEntry();
        entry.cmd = lastFetchedCmd;
        entry.entryType = "object Detected";
        entry.position = this.transform.position;
        entry.rotation = this.transform.rotation.eulerAngles;
        entry.velocity = rb.velocity;
        entry.time = Time.time;
        //  entry.relativeVelocity = collision.relativeVelocity;

        entry.otherName = other.name;
        UAV otherUAV = other.gameObject.GetComponent<UAV>();
        if (otherUAV != null)
            entry.otherCmd = otherUAV.lastFetchedCmd;

        eventLogger.logs.Add(entry);
        if(otherUAV != null)
            sendMessage(lastFetchedCmd, otherUAV);

    }
    private void OnTriggerStay(Collider other)
    {

        if(other.name != "ground")
          Debug.DrawLine(this.transform.position, other.transform.position,Color.black);

    }
    private void OnTriggerExit(Collider other)
    {
		if (inRageObjects.Contains (other.gameObject))
			inRageObjects.Remove (other.gameObject);
		else
			return;
		droneEventLogEntry entry = new droneEventLogEntry();
        entry.cmd = lastFetchedCmd;
        entry.entryType = "object out of range";
        entry.position = this.transform.position;
        entry.rotation = this.transform.rotation.eulerAngles;
        entry.velocity = rb.velocity;
        entry.time = Time.time;
        //  entry.relativeVelocity = collision.relativeVelocity;

        entry.otherName = other.name;
        UAV otherUAV = other.gameObject.GetComponent<UAV>();
        if (otherUAV != null)
            entry.otherCmd = otherUAV.lastFetchedCmd;
        eventLogger.logs.Add(entry);

    }
    void OnApplicationQuit()
    {
        XmlSerializer eventsLogSerializer = new XmlSerializer(typeof(droneEventsLogger));
        eventsLogSerializer.Serialize(eventsOutputStream, eventLogger);
        outputStreamWriter.Flush();
        outputStream.Close();
        eventsOutputStream.Close();

    }

}
