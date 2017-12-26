using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System;

[XmlInclude(typeof(objectState))]
public class visibleObjectState : objectState {

    public string prefabName { set; get; }
    public visibleObjectState()
    {
        
    }
    public visibleObjectState(string _name,Transform t,string _prefabName)
    {
        base.name = _name;
        base.position = t.position;
        base.rotation = t.rotation.eulerAngles;
        base.scale = t.lossyScale;
        prefabName = _prefabName;
       
    }
    public visibleObjectState(string _name, string _prefabName)
    {
        base.name = _name;
        base.position = Vector3.zero;
        base.rotation = Vector3.zero;
        base.scale = Vector3.one;
        prefabName = _prefabName;
       

    }
    public visibleObjectState(GameObject src):base(src)
    {
     //   this.prefabName = src.GetComponent<visableObjectData>().prefabName;

    }
    public override GameObject createGameObject()
    {
        GameObject temp; 
        temp = GameObject.Instantiate( Resources.Load("prefab/" + prefabName) as GameObject);
        temp.name = name;
        temp.transform.position = position;
         
        temp.transform.Rotate( rotation);
      //  Debug.Log(temp.transform.position);
        temp.transform.localScale = scale;
        return temp;

    }
    public override GameObject createGameObject(Transform parent)
    {
        GameObject temp = this.createGameObject();
        temp.transform.SetParent(parent);
        return temp;
    }

}
