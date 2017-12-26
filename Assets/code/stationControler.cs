using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class stationControler : MonoBehaviour {
   // public Image icon;
    // Use this for initialization
    void Start()
    {
      //  MiniMapController.RegisterMapObject(this.gameObject, icon);
    }
	// Update is called once per frame
	void Update () {
		
	}
    void OnDestroy()
    {
       // MiniMapController.RemoveMapObject(this.gameObject);
    }
}
