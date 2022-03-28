using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAAA;

public class Tester : MonoBehaviour {

	int finishedThreads = 0;

	// Use this for initialization
	void Start() {

		Threader.Run(() => {
			Debug.Log("a");
			Threader.Dispatch(() => Debug.Log("b"));
		});

		int i = -1;
		while (++i < 500) {
			int idx = i;
			Threader.Run(() => {
				int ź = -10;
				while (++ź < 20) {
					var a = Mathf.Sqrt(Mathf.Sin(1119) % 4 % 3.5f) % 9;
					var b = Mathf.Acos(Mathf.Sin(a));
					Debug.LogWarning(b);
				}

				Debug.Log("Started " + idx);

				if (i == 100) {
					Debug.Log(GetComponent<Rigidbody>().freezeRotation);
				}

				Threader.Dispatch(() => {
					Debug.Log("Finished " + idx);
					finishedThreads++;
				});
			});
		}
	}


	void Update() {
		//busyThreads = worker.busyThreadCount;
	}
}
