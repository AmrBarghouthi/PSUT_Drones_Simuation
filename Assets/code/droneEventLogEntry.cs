using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine; 
    public class droneEventLogEntry
    {
        public string entryType;
        public Vector3 position;
        public Vector3 rotation;
        public float time;
        public string cmd;
        public Vector3 velocity;
        public Vector3 relativeVelocity;
        public string otherName;
        public string otherCmd;
        public string info;
        public droneEventLogEntry()
        { }
    }
 