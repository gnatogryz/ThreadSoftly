using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAAA;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Thread.Run(() => {
			Debug.Log("KURWA 1");
			float i = -555;
			while (i < 555 && !Thread.killAll) {
				i += 0.00006f;
			}
			Thread.Dispatch(() => {
				Debug.Log("MAĆ 1");
			});
		});

		Thread.Run(() => {
			Debug.Log("KURWA 2");
			float i = -555;
			while (i < 556 && !Thread.killAll) {
				i += 0.00006f;
			}
			Thread.Dispatch(() => {
				Debug.Log("MAĆ 2");
			});
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
