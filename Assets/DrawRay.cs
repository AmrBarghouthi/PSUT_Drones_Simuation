using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawRay : MonoBehaviour {
	GameObject DetectedGameObject;

	float targetAngle;
	

	List<double> pReadingAngles;
	List<double> pDistanceReadings;
	List<double> agoraFactors;


	// Use this for initializing the number of rays to shoot out of the drone.
	private int RaysToShoot = 30;
	void Start () {
		pReadingAngles = new List<double>();
		StartCoroutine(StartAgoraphilic()); // Waiting for 3 seconds just to make sure the object has been detected if there's any.
	}

	IEnumerator StartAgoraphilic()
    {
        yield return new WaitForSeconds(3);
		agoraFactors = GetAgoraFactors();
    }
	

	void Update () {
			float angle = 0;
			for (int i=0; i<RaysToShoot; i++) {
				
			float x = Mathf.Sin (angle);
			float y = Mathf.Cos (angle);
			angle += 2 * Mathf.PI / RaysToShoot;


			Vector3 dir = new Vector3 (transform.position.x + x, transform.position.y - 1, transform.position.z + y);

			RaycastHit hit;

			Debug.DrawRay(transform.position, dir, Color.red);

			if (Physics.Raycast(transform.position, dir, out hit)) {
				angle = (float)(angle * (180f/Math.PI));
				pReadingAngles.Add(angle);
				DetectedGameObject = hit.collider.gameObject; // That was the object that was hit by the ray.
			}
		}
	}

	private void GetAngle() {
		targetAngle = DetectedGameObject.transform.rotation.x;
	}

	private List<double> GetAgoraFactors() {
		GetAngle();
		bool[] flags = new bool[pReadingAngles.Count];
		List<double> agoraFactors = new List<double>();
		for(int i = 0; i < pReadingAngles.Count; i++) {
			flags[i] = true;
		}
		double[] factors = new double[pReadingAngles.Count];
		double subNumber = 1.0 / (pReadingAngles.Count);

		for (int i = 0; i < pReadingAngles.Count; i++) {
			factors[i] = 1 - (i * subNumber);
			agoraFactors.Add(0);
		}

		double angleDiff;
		for (int i = 0; i < pReadingAngles.Count; i++) {
			angleDiff = double.MaxValue;
			int leastForRound = 0;
			for (int j = 0; j < pReadingAngles.Count; j++) {
				double currentDiff = Math.Abs(pReadingAngles[j] - targetAngle);
				currentDiff = currentDiff > 180 ? 360 - currentDiff : currentDiff;
				if (currentDiff < angleDiff && flags[j]) {
					angleDiff = currentDiff;
					leastForRound = j;
				}
			}

			agoraFactors[leastForRound] = factors[i];
			flags[leastForRound] = false;
		}

		return agoraFactors;
	}


}
