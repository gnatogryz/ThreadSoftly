using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAAA;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Thread.Run(() => {
			Debug.Log("Starting 1");
			float i = -55;
			while (i < 55 && !Thread.killAll) {
				i += 0.006f;
			}
			Thread.Dispatch(() => {
				Debug.Log("Finished 1");
			});
		});

		Thread.Run(() => {
			Debug.Log("Starting 2");
			float i = -555;
			while (i < 556 && !Thread.killAll) {
				i += 0.0006f;
			}
			Thread.Dispatch(() => {
				Debug.Log("Finished 2");
			});
		});

		Thread.Run(() => {
			Debug.Log("Starting 3");
			float i = -555;
			while (i < 556 && !Thread.killAll) {
				i += 0.0006f;
			}
			Thread.Dispatch(() => {
				Debug.Log("Finished 3");
			});
		});

		Thread.Run(() => {
			Debug.Log("Starting 4");
			float i = -555;
			while (i < 556 && !Thread.killAll) {
				i += 0.00006f;
			}
			Thread.Dispatch(() => {
				Debug.Log("Finished 4");
			});
		});
	}
	
	// Update is called once per frame
	void Update () {
	}
}
