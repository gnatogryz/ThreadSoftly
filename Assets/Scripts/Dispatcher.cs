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

		void FixedUpdate() {
			if (dispatch.Count > 0) {
				lock (dispatch) {
					while (dispatch.Count > 0) {
						dispatch.Dequeue()();
					}
				}
			}

			threads.RemoveAll(th => th.IsAlive == false);
		}

		void OnDisable() {
			KillAllThreads();
		}

		void OnDestroy() {
			KillAllThreads();
		}


		void KillAllThreads() {
			killAllThreads = true;
			int i = 0;
			foreach (var th in threads) {
				if (th.IsAlive) {
					th.Interrupt();
					th.Join(100);
					i++;
				}
			}
			if (i > 0) {
				Debug.Log("[THREADING] Aborting " + i + " threads.\n");
			}
		}


		public static System.Threading.Thread Run(System.Threading.ThreadStart what) {
			var th = new System.Threading.Thread(what);
			instance.threads.Add(th);
			th.Start();
			return th;
		}


		public static void Dispatch(System.Action what) {
			lock (instance.dispatch) {
				instance.dispatch.Enqueue(what);
			}
		}
	}
}
