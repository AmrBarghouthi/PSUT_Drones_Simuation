using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessages : MonoBehaviour {

	public GUIText gt;
	public Dictionary<string, string> Messages = new Dictionary<string,string>();
	public bool Enabled = true;

	void Start () {
		Debug.Log("Broadcasting is starting...");
		gt = GetComponent<GUIText>();
	}

	void Update() {
		if (!Enabled) return;
		foreach (char c in Input.inputString) {
            if (c == '$') {
				Debug.Log("End of input has been reached, broadcasting will start now");
				Enabled = false;
				StartBroadcasting(gt.text);
			}
            else
            {
                gt.text += c;
            }

			Debug.Log("The current text is: " + gt.text);
        }
	}

	private void StartBroadcasting(string text) {
		string[] messages = text.Split(' ');
		for (int i = 0; i < messages.Length; i++) {
			Messages["MessageNumber" + (i+1).ToString()] = messages[i];
		}
		
		BroadcastMessage("RecieveMessages", Messages);
	}
}
