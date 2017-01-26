using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAAA;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int i = 100;
		while (--i > -1) {
			int idx = i;
			Thread.Run(() => {
				int ź = -9999;
				while (++ź < 9999) { }					
				Debug.Log("Started " + idx);
				Thread.Dispatch(() => {
					Debug.Log("Finished " + idx);
				});
			});
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
