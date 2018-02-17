using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
public class testSenaroGentrater1 : MonoBehaviour {
	public float AreaWidth;
	float minX,minZ,maxX,maxZ;
	public int numberOfDrones;
	// Use this for initialization
	public float droneRaduis;
	public float Y;
	int maxNumberOfTrys = 1000;
	public int numberOfMoveCmds;
	List<Vector3> currentStartLocations;
	void Start () {
		minX = -AreaWidth / 2;
		maxX = -minX;
		maxZ = maxX;
		minZ = minX;
		currentStartLocations = new List<Vector3> ();
		for (int i = 0; i < numberOfDrones; i++) {
			bool canAdd = false;
			Vector3 point = Vector3.zero;
			int trys = 0;
			while (!canAdd) {
				point = getRandomVector ();
				canAdd = true;
				trys++;
				for (int j = 0; j < currentStartLocations.Count; j++) {
					if ((currentStartLocations [j] - point).magnitude < droneRaduis * 2) {
						canAdd = false;
						break;
					}
				}
				if (trys >= maxNumberOfTrys) {
					Debug.Log ("satruation");
					break;
				}
					

			}
			if (canAdd) {
				currentStartLocations.Add (point);
			//	Debug.Log (point);
			}
		}

		Debug.Log ("points Gnareted");
		List<objectState> stateList = new List<objectState>();
		int count =0;
		foreach (var x in currentStartLocations) {
			
			Dictionary<string, string> uavData = new Dictionary<string, string> ();
			uavData ["mass"] = "4";
			string[] cmdList = new string[numberOfMoveCmds+1];
			cmdList [0] = "setSpeed 30";
			for (int k = 0; k < numberOfMoveCmds; k++) {
				Vector3 endLocation = getRandomVector ();
				cmdList [k+1] = "move  x" + endLocation.x.ToString () + "  y" + endLocation.y.ToString () + " z" + endLocation.z.ToString ();
				
			}

			uavObjectState state = new uavObjectState("drone " + count.ToString(), "basicDrone");
			state.position = x;
			state.uavData = uavObjectState.makeListFromDictionary(uavData) ;
			state.cmdList = cmdList;

			stateList.Add (state);

			count++;
		}
		string path = "coedeGenrtatedCase"+System.DateTime.Now.ToFileTimeUtc().ToString() + ".xml";
		Stream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);

		XmlSerializer serializer =   new XmlSerializer(typeof(List<objectState>),new []{ typeof(visibleObjectState),typeof(uavObjectState),typeof(List<KeyValuePair<string,string>>)});
		var sw = new StreamWriter (fileStream);
		serializer.Serialize (sw,stateList);
		sw.Flush ();
		sw.Close ();
	 
	}


	Vector3 getRandomVector()
	{
		return new Vector3 (UnityEngine.Random.Range(minX,maxX),Y,UnityEngine.Random.Range(minZ,maxZ));
	}
	// Update is called once per frame
	void Update () {
		
	}
}
