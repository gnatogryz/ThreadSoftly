using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAAA;

public class Tester : MonoBehaviour {

	Worker worker;
	int finishedThreads = 0;
	public int busyThreads;

	// Use this for initialization
	void Start () {


	worker = new Worker(8);

	worker.Run(() => {
		Debug.Log("Kurwa");
		worker.Dispatch(() => Debug.Log("Mać"));
	});

		int i = -1;
		while (++i < 500) {
			int idx = i;
			worker.Run(() => {
				int ź = -10;
				while (++ź < 20) {
					var a = Mathf.Sqrt(Mathf.Sin(1119) % 4 % 3.5f) % 9;
					var b = Mathf.Acos(Mathf.Sin(a));
					Debug.LogWarning(b);
				}

				//Debug.Log("Started " + idx);

				if (i==100) {
					Debug.Log(GetComponent<Rigidbody>().freezeRotation);
				}

				worker.Dispatch(() => {
					//Debug.Log("Finished " + idx);
					finishedThreads++;
				});
			});
		}
	}


	void Update() {
		busyThreads = worker.busyThreadCount;
	}
}
