using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

namespace AAAA {

	public static class Threader {
		public static void Run(System.Action job) {
			ThreadPool.QueueUserWorkItem(o => {
				try {
					job();
				} catch (System.Exception e) {
					Debug.LogError(e.Message + "\n" + e.StackTrace);
				}
			});
		}

		public static void Dispatch(System.Action job) {
			Internal.Dispatcher.Dispatch(job);
		}

		public static void Sleep(int milliseconds) {
			System.Threading.Thread.Sleep(milliseconds);
		}
	}


	namespace Internal {
		public sealed class Dispatcher : MonoBehaviour {

			static Dispatcher instance;

			volatile Queue<System.Action> dispatch = new Queue<System.Action>();

			static object mutex;

			[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
			static void Initialize() {
				instance = new GameObject("Threading").AddComponent<Dispatcher>();
				instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
				DontDestroyOnLoad(instance.gameObject);
				mutex = new object();
			}

			void FixedUpdate() {
				lock (mutex) {
					if (dispatch.Count > 0) {
						while (dispatch.Count > 0) {
							dispatch.Dequeue()();
						}
					}
				}
			}

			public static void Dispatch(System.Action what) {
				lock (mutex) {
					instance.dispatch.Enqueue(what);
				}
			}
		}
	}
}
