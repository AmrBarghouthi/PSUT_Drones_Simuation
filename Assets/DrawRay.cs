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
	List<Vector3> avoidingLocation;

	/* Avoiding can happen by either sotring the angle of the target position of the vector3, use whatever function. */
	Vector3 VectorTarget; // TODO: Set the VectorAngle to the target position using the agoraFactors list.
	float AngleTarget; // TODO: Set the AngleTarget to the target position using the agoraFactors list.

	[SerializeField]
	public float speed; // Define it from the inspector

	// Use this for initializing the number of rays to shoot out of the drone.
	private int RaysToShoot = 30;
	void Start () {
		speed = 0.5f;
		pReadingAngles = new List<double>();
		StartCoroutine(StartAgoraphilic()); // Waiting for 3 seconds just to make sure the object has been detected if there's any.
	}

	IEnumerator StartAgoraphilic()
    {
        yield return new WaitForSeconds(3);
		agoraFactors = GetAgoraFactors();
		// Avoid(Angle);
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
				angle = (float)(angle * (180f/Math.PI)); // fix angles calculation.
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
		List<Vector3> avoidingLocation = new List<Vector3>();

		for(int i = 0; i < pReadingAngles.Count; i++) {
			flags[i] = true;
		}
		double[] factors = new double[pReadingAngles.Count];
		double subNumber = 1.0 / (pReadingAngles.Count);

		for (int i = 0; i < pReadingAngles.Count; i++) {
			factors[i] = 1 - (i * subNumber);
			agoraFactors.Add(0);
			avoidingLocation.Add(new Vector3(0f, 0f, 0f));
		}

		double angleDiff;
		for (int i = 0; i < pReadingAngles.Count; i++) {
			angleDiff = double.MaxValue;
			int leastForRound = 0;
			Vector3 avoidingLoc;
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

			// Setting the target angle or target vector should happen around here.
			// and either of the two functions below should be called.
		}

		return agoraFactors;
	}

	// Make the gameobject move by the specified angle.
	private void AvoidByAngle(float target) {
		float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, target, speed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, angle, 0);
	}

	// Make the gameobject move by the specified vector3
	private void AvoidByVector3(Vector3 target) {
		transform.position = Vector3.MoveTowards(transform.position, VectorTarget, speed * Time.deltaTime);
	}


}
