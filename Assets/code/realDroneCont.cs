using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class realDroneCont : UAV {

    // Use this for initialization
    GameObject fr,fl,br,bl,gyro;
    public float upBaiss;
    public PID pitchPID, rollPID, yawPID,altPID;
    Rigidbody rb;
    public float maxMotorForce;
	void Start () {
       // (this as UAV).Start();
        fr = transform.FindChild("fr").gameObject;
        fl = transform.FindChild("fl").gameObject;
        br = transform.FindChild("br").gameObject;
        bl = transform.FindChild("bl").gameObject;
        gyro = transform.FindChild("gyro").gameObject;
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

   public float pitchSetPoint = 0;
    public float yawSetPoint = 180;
    public float rollSetPoint = 0;
    public float driveFr, driveFl, driveBr, driveBl;
    public float altSetPoint=10;
    private void FixedUpdate()
    {

        if (altSetPoint - gyro.transform.position.y > 10)
            upBaiss = 20;
        else upBaiss = altPID.Update(altSetPoint, gyro.transform.position.y, Time.deltaTime);
       
        motorsLoop();

      //  Debug.Log(rb.velocity);
    }
    public void motorsLoop()
    {
        float p = getPitch();
        float r = getRoll();
        float y = getYaw();
        float pitchDrive = -pitchPID.Update(pitchSetPoint, p, Time.deltaTime);
        float rollDrive = rollPID.Update(rollSetPoint, r, Time.deltaTime);


        float yawErr = y-fixAngle(yawSetPoint);
      
        yawErr = fixAngle(yawErr);
        float yawDrive = yawPID.Update(0,yawErr, Time.deltaTime);
        driveFr = upBaiss + pitchDrive + rollDrive;
        driveFl = upBaiss + pitchDrive - rollDrive;
        driveBr = upBaiss - pitchDrive + rollDrive;
        driveBl = upBaiss - pitchDrive - rollDrive;
        addForceToPreppler(fr, driveFr);
        addForceToPreppler(br, driveBr);
        addForceToPreppler(fl, driveFl);
        addForceToPreppler(bl, driveBl);

        //Debug.Log(new Vector3(p, r, y));
        rb.AddTorque(gyro.transform.up * yawDrive);
    }
    public void addForceToPreppler(GameObject pos,float value)
    {
        value=  Mathf.Clamp(value, 0, maxMotorForce);
        rb.AddForceAtPosition(gyro.transform.up * value, pos.transform.position);
         
    }
    float getPitch()
    {

        float pitch = gyro.transform.rotation.eulerAngles.x;
   
       return fixAngle(pitch);   
    }
    float getRoll()
    {

        float roll = gyro.transform.rotation.eulerAngles.z;
    
        return fixAngle(roll);
    }
    float getYaw()
    {
        float yaw = gyro.transform.rotation.eulerAngles.y;
        return fixAngle(yaw);
    }
    float fixAngle(float inputAngle)
    {
        if (inputAngle < -360f) return fixAngle(inputAngle + 360f);
        if (inputAngle > 360f) return fixAngle(inputAngle - 360f);
        if (inputAngle > 180)
            inputAngle = (inputAngle - 360f);
        return inputAngle;
    }
}
