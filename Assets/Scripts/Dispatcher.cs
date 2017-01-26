using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AAAA {
	public sealed class Thread : MonoBehaviour {

		static Thread instance;

		bool killAllThreads = false;
		public static bool killAll { get { return instance.killAllThreads; } }

		volatile List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
		volatile Queue<System.Action> dispatch = new Queue<System.Action>();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Initialize() {
			instance = new GameObject("Threading").AddComponent<Thread>();
			instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
			DontDestroyOnLoad(instance.gameObject);
		}

		void Update() {			
			while (dispatch.Count > 0) {
				dispatch.Dequeue()();
			}

			threads.RemoveAll(th => th.IsAlive == false);
		}

		void OnDisable() {
			KillAllThreads();
		}


		void KillAllThreads() {
			killAllThreads = true;
			int i = 0;
			foreach (var th in threads) {
				if (th.IsAlive) {
					th.Abort();
					i++;
				}
			}
			if (i > 0) {
				Debug.Log("[THREADING] Aborting " + i + " threads.\n");
			}
			if (threads.Any(th => th.IsAlive)) {
				Debug.Log("[THREADING] Some threads are unresponsive and cannot be killed. Use the killAll variable in your loops.\n");
			}
		}


		


		public static void Run(System.Action what) {
			var th = new System.Threading.Thread(() => {
				what();
			});
			instance.threads.Add(th);
			th.Start();
		}


		public static void Dispatch(System.Action what) {
			instance.dispatch.Enqueue(what);
		}
	}
}
