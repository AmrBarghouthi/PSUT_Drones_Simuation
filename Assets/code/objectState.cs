using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[XmlRoot]
public class objectState
{
    [XmlElement("position")]
    public Vector3 position { set; get; }
    [XmlElement("scale")]
    public Vector3 scale { set; get; }
    [XmlElement("rotation")]
    public Vector3 rotation { set; get; }
    [XmlElement("name")]
    public string name { set; get; }
    public string parent;
    public objectState(Transform t)
    {
        name = "new object";
        position = t.position;
        rotation = t.rotation.eulerAngles;
        scale =  t.lossyScale;
        
    }
    public objectState()
    {
       
    }
    public objectState(GameObject obj)
    {
        name = obj.name;
        position = obj.transform.position;
        rotation = obj.transform.rotation.eulerAngles;
        scale = obj.transform.lossyScale;
    }
    public objectState(Transform t, string _name)
    {
        name = _name;
        position = t.position;
        rotation = t.rotation.eulerAngles;
        scale = t.lossyScale;
    }
    public objectState(GameObject obj,string _name)
    {
        name = _name;
        position = obj.transform.position;
        rotation = obj.transform.rotation.eulerAngles;
        scale = obj.transform.lossyScale;

    }
    public virtual GameObject createGameObject()
    {
        GameObject temp = new GameObject(name);
        
        temp.transform.position = position;
        temp.transform.rotation = Quaternion.Euler( rotation);
        temp.transform.localScale = scale;
        return temp;
    }
    public virtual GameObject createGameObject(Transform _parent)
    {

        GameObject temp = this.createGameObject();
        parent = _parent.name;
        temp.transform.SetParent(_parent);
        return temp;
    }
  
}
