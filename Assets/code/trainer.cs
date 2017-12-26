using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class trainer : MonoBehaviour {

    Stream outputStream;
    StreamWriter outputStreamWriter;
    public int numberOfGenerations;
    public int poolSize;
    public float mutaionRate;
    public float Pc;
    public float testTime;
    public GameObject uavTemplet;
    List<float> [] dnaPool;
    GameObject currentUAV;
    float[] fitness;
    float[] reprodcionProb;
    int currentGereartion;
    int curentUavId;
    Vector3 startPos;
    Vector3 []idealPos;
    bool newDrone = true;
    int currentFrame;
    int numberOfFrames;
    string[] cmdList;
	void Start () {
        outputStream = File.Open("runData/genData.txt", FileMode.CreateNew);
        outputStreamWriter = new StreamWriter(outputStream);
        dnaPool = new List<float>[poolSize];
        fitness = new float[poolSize];
        reprodcionProb = new float[poolSize];
        numberOfFrames = (int)(testTime / Time.fixedDeltaTime);
        idealPos = new Vector3[numberOfFrames];

       
        Vector3 dir = new Vector3(1, 0, 0);
        for(int i=0;i<idealPos.Length;++i)
        {
            if (i * Time.fixedDeltaTime < testTime / 2)
                idealPos[i] = Vector3.zero;
         
        }
       

        currentGereartion = 0;
        curentUavId = -1;
        randPool();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void FixedUpdate()
    {


      
        if (newDrone)
        { 
            if (curentUavId == poolSize-1)
            {
                newPool();
                curentUavId = 0;
                currentGereartion++;
                if (currentGereartion == numberOfGenerations)
                    DestroyImmediate(this);
            }
            else curentUavId++;
            if (currentUAV != null)
            {
                Destroy(currentUAV); 
         
            }

            
            currentUAV = Instantiate(uavTemplet, startPos, Quaternion.identity);
            currentUAV.name = "gen_" + currentGereartion.ToString() + "_id_" + curentUavId.ToString();
            realDroneCont c = currentUAV.GetComponent<realDroneCont>();
      
            c.pitchPID= new PID(dnaPool[curentUavId][0], dnaPool[curentUavId][1], dnaPool[curentUavId][2]);
            c.rollPID = new PID(dnaPool[curentUavId][3], dnaPool[curentUavId][4], dnaPool[curentUavId][5]);
                //  c.speedPID = new PID(dnaPool[curentUavId][9], dnaPool[curentUavId][10], dnaPool[curentUavId][11]);
            c.upBaiss =9.81f;
            
            
             
            fitness[curentUavId] = 0;
            currentFrame = 0;
            newDrone = false;

       
        }
        
        fitness[curentUavId] = -(currentUAV.transform.position - idealPos[currentFrame]).magnitude;
        currentFrame++;
        if (numberOfFrames == currentFrame)
        {
            outputStreamWriter.Write(currentGereartion.ToString() + "," + curentUavId.ToString() + "," +fitness[curentUavId].ToString() );
            for (int i = 0; i < 7; i++)
                outputStreamWriter.Write(","+dnaPool[curentUavId][i]);
            outputStreamWriter.WriteLine("");
            outputStream.Flush();
            newDrone = true;
            Debug.Log(currentUAV.name + " score " + fitness[curentUavId]);
        }
    }

    private void randPool()
    {
        for(int i=0;i<poolSize;i++)
        {
            dnaPool[i] = new List<float>();
            for (int j = 0; j < 7; j++)
                dnaPool[i].Add( UnityEngine.Random.Range(-10, 10));
           
        }
    }
    private void newPool()
    {

       
        for (int i=0;i<poolSize;i++)
        {
            int bestIndex = 0;
            for(int j=0;j<poolSize;j++)
            {
                if (fitness[j] > fitness[bestIndex])
                    bestIndex = j;
            }
            fitness[bestIndex] = float.NegativeInfinity;
            reprodcionProb[bestIndex] = Pc * Mathf.Pow(1 - Pc, i);
            
        }

      
        List<float> []newDanPool = new List<float>[poolSize];
        for (int i=1;i<poolSize;i++)
            reprodcionProb[i] += reprodcionProb[i - 1];
        for(int i=0;i<poolSize;i++)
        {
            float p1 = UnityEngine.Random.Range(0, 1);
            float p2 = UnityEngine.Random.Range(0, 1);
            int index1=0, index2=0;
            for(int j=0;j<poolSize;j++)
            {
                if (p1 > reprodcionProb[j])
                    index1 = j;
                if (p2 > reprodcionProb[j])
                    index2 = j;
            }
            newDanPool[i] = mearg(dnaPool[index1], dnaPool[index2]);
            float m = UnityEngine.Random.Range(0, 1);
            if(m<mutaionRate)
            {
                newDanPool[i] =   mutate(newDanPool[i]);
            }
        }

      
        dnaPool = newDanPool;
    }

    private List<float>  mutate(List<float> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            float a = Mathf.Abs(list[i]);
            float r = 0.1f * a;
            list[i] = UnityEngine.Random.Range(list[i]-r,list[i]+r);

        }
        return list;
    }

    private List<float> mearg(List<float> dna1, List<float> dna2)
    {
        List<float> ret = new List<float>(15);
        for(int i=0;i<dna1.Count; i++)
            ret.Add( UnityEngine.Random.Range(Mathf.Min(dna1[i], dna2[i]), Mathf.Max(dna1[i], dna2[i])));
        

        return ret;
    }
}

