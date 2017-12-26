using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class simpleDrone : UAV {

    protected float weight = 3;//not used yet
//	protected float lenght;//not used yet
//	protected float width;//not used yet
	protected float max_speed=float.PositiveInfinity;//if target speed is less than max the max is used 
//	protected float min_speed;//not used yet
    //variables for the recharge command 
    float chrg;
    float waitTime = 1;
    //end 
    //variables for setSpeed command 
    float setspeed;
    //end
    //variables defined by amr
	public float speed { set; get; }
    private Vector3 target;
    //****************************************************
   // public float speed; i defined speed in the UAV class 
    //****************************************************

 
    private string curCmd;
	protected float charge;
	//protected float chargePercentage;
    //end of vriables defined by amr
	// Use this for initialization

	public new void Start () {

        //target = new Vector3(100,200,300);

		(this as UAV).Start();
        /*
            cmdList = new string[2];
            cmdList[1] = "wait";
            cmdList[0] = "setSpeed 30";
        */
      

    }
    
    // Update is called once per frame
   
   
   
    
    void decode()
    {
        if (cmdList.Length <= currentCmdIndex)
        {
            curCmd = "wait";
            Debug.Log("no new commd");
        }
        else curCmd = cmdList[currentCmdIndex];
		Debug.Log ("decodeing :"+curCmd);

        if(curCmd.Contains("goToObject"))
        {
            string[] temp = curCmd.Split(' ');
            string targtBaseName = temp[1];
            GameObject go  = GameObject.Find(targtBaseName);
            if(go!= null)
            {
                target = go.transform.position;
            }
            else
            {
                Debug.Log("the object dose not existes");
            }
            curCmd = "move";
            
        }
        if(curCmd.Contains("move"))
        {
            string []subCmd = curCmd.Split(' ');
			bool isRletive=false;
			for (int subCmdIndex = 0; subCmdIndex < subCmd.Length; subCmdIndex++) {//generlized the synticx of the move cmd ex: "move x1 y2" = "move y2 x1"
				if (subCmd [subCmdIndex].Contains ("x"))target.x = float.Parse (subCmd [subCmdIndex].Substring (1));
				if (subCmd [subCmdIndex].Contains ("y"))target.y = float.Parse (subCmd [subCmdIndex].Substring (1));
                if (subCmd [subCmdIndex].Contains ("z"))target.z = float.Parse (subCmd [subCmdIndex].Substring (1));
				if (subCmd [subCmdIndex].Contains ("r"))isRletive = true; 
			}
			if (isRletive)
				target += this.transform.position;
			curCmd = "move"; 
        }
        if(curCmd.Contains("wait"))
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
            chrg =float.Parse( subCmd[1]);
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
        currentCmdIndex++;
    }
    
    void FixedUpdate () {
        
        //decode and execute commands
        if (cmdDone == true)
			decode();
        //no need for eles strat exexuting the new cmd on the smae time unit it's decoded
        {
            Debug.Log("exeuting " + curCmd);


            /////change position 
            if (curCmd == "move")
            {
                //Debug.Log ("target "+target);
                Vector3 dir = target - transform.position;

               // float remaningTime = dir.magnitude / speed;
                dir.Normalize();
                //Debug.Log ("dir" + dir);

                this.transform.Translate(dir * Time.deltaTime * speed, Space.World);
                if ((target - transform.position).magnitude < 1)
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
                Debug.Log("remanig time to wait"+waitTime);
                if (waitTime > 0)
                    waitTime -= Time.deltaTime;
            }
            else
                cmdDone = true;
        }
       
		outputStreamWriter.WriteLine(Time.time.ToString() + ","
			+ transform.position.x + ","
			+ transform.position.y + ","
			+ transform.position.z + ","
			+ transform.rotation.eulerAngles.x + ","
			+ transform.rotation.eulerAngles.y + ","
			+ transform.rotation.eulerAngles.z);
		
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
		UAVData ["weight"] = weight.ToString();
		UAVData ["speed"] = speed.ToString();
		UAVData ["max_speed"] = max_speed.ToString();
		UAVData ["chrg"] = chrg.ToString();
        return UAVData;
    }
   
   
    void OnApplicationQuit()
    {
        
            outputStream.Close();
      
    }

}
