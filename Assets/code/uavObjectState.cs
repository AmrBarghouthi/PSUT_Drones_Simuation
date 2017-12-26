using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uavObjectState : visibleObjectState {
    public string[] cmdList;
    public List<KeyValuePair< string, string> > uavData;
    public uavObjectState()
    {
        uavData = new List<KeyValuePair< string, string> >();
        cmdList = new string[1];
        cmdList[0] = "wait";
         
    }
	public uavObjectState(GameObject src):base(src)
    {
        UAV uav = src.GetComponent<UAV>();
        cmdList = uav.cmdList;
        Dictionary<string, string> Data = uav.makeDictionary();
        uavData = makeListFromDictionary(Data);
        
     }
    public static List<KeyValuePair<string,string> > makeListFromDictionary(Dictionary<string, string> x)
    {
        List<KeyValuePair<string, string> > ret = new List<KeyValuePair<string, string>>();
        var l = x.Keys;
        foreach(var i in l)
            ret.Add(new KeyValuePair<string, string>(i,x[i]));
        return ret;
    }
    public static Dictionary<string, string>  makeDictionaryFromList(List<KeyValuePair<string, string>> x)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        foreach (var i in x)
        {
            ret[i.Key] = i.Value;
        }
        return ret;
    }

    public uavObjectState(string _name,string _prefabName):base(_name,_prefabName)
    {
        
    }
    public override GameObject createGameObject()
    {
        GameObject temp = base.createGameObject();

        temp.GetComponent<UAV>().cmdList = cmdList;
        Dictionary<string, string> data = makeDictionaryFromList(uavData);
        temp.GetComponent<UAV>().setupFromDictionary(data);
        return temp;

    }
}
