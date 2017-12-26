using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapObject
{
	public Image icon { get; set; }
	public GameObject owner { get; set; }
}

public class MiniMapController : MonoBehaviour {
 
	public Camera mapCamera;

	public static List<MapObject> mapObjects = new List<MapObject>();

	public static void RegisterMapObject(GameObject o, Image i)
	{
		Image image = Instantiate(i);
		mapObjects.Add(new MapObject(){owner = o, icon = image});
	}

	public static void RemoveMapObject(GameObject o)
	{
	   List<MapObject> newList = new List<MapObject>();
	   for (int i = 0; i < mapObjects.Count; i++)
	   {
	      if (mapObjects[i].owner == o)
	      {
	         Destroy(mapObjects[i].icon);
	         continue;
	      }
	      else
	         newList.Add(mapObjects[i]);
	   }

	   mapObjects.RemoveRange(0, mapObjects.Count);
	   mapObjects.AddRange(newList);
	}

	void DrawMapIcons()
	{
		foreach (MapObject mo in mapObjects)
		{
			Vector2 mop = new Vector2(mo.owner.transform.position.x, mo.owner.transform.position.y);
			Vector2 pp = new Vector2(0  , 0);

			if(Vector2.Distance(mop,pp) > 200)
			{
				mo.icon.enabled = false;
				continue;
			}
			else
				mo.icon.enabled = true;
				
			Vector3 screenPos = mapCamera.WorldToViewportPoint(mo.owner.transform.position);
			mo.icon.transform.SetParent(this.transform);

			RectTransform rt = this.GetComponent<RectTransform>();
			Vector3[] corners = new Vector3[4];
			rt.GetWorldCorners(corners);

			screenPos.x = Mathf.Clamp(screenPos.x * rt.rect.width + corners[0].x, corners[0].x, corners[2].x);
			screenPos.y = Mathf.Clamp(screenPos.y * rt.rect.height + corners[0].y, corners[0].y, corners[1].y);

			screenPos.z = 0;
			mo.icon.transform.position = screenPos;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		DrawMapIcons();
	}
}
