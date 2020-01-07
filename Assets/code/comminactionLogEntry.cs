using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class comminactionLogEntry
    {
        public string msg, from, to;
        public List<reviverData> recivedby;
        public List<reviverData> faildToReciveBy;
        public float time; 
        public comminactionLogEntry()
        {
                recivedby = new List<reviverData>();
                faildToReciveBy = new List<reviverData>();
        }
    }
[Serializable]
public class reviverData
    {
       public string name;
       public float distance;
       public reviverData(string _name,float dist)
       {
        name = _name;
        distance = dist;
       }
        public reviverData()
        {

        }
    }


