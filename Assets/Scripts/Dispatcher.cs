using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AAAA {

	public static class Threader {
		public static System.Threading.Thread Run(System.Threading.ThreadStart job) {
			return Internal.Dispatcher.Run(job);
		}

		public static void Dispatch(System.Action job) {
			Internal.Dispatcher.Dispatch(job);
		}
	}

	public class Worker {
		Queue<System.Action> jobQueue;
		object mutex;
		System.Threading.Thread[] loopThreads;
		int threadCount;

		public bool killed { get; private set; }

		bool[] busy;

		public int busyThreadCount {
			get {
				return busy.Where(b => b).Count();
			}
		}

		public Worker(int numberOfThreads) {
			jobQueue = new Queue<System.Action>();
			mutex = new object();
			Internal.Dispatcher.onKillThreads += Kill;

			threadCount = numberOfThreads;
			loopThreads = new System.Threading.Thread[threadCount];

			busy = new bool[threadCount];

			for (int i = 0; i < threadCount; i++) {
				int idx = i;
				busy[i] = false;
				loopThreads[i] = new System.Threading.Thread(() => InternalLoop(idx));
				loopThreads[i].Start();
			}
		}

		public void Kill() {
			Internal.Dispatcher.onKillThreads -= Kill;
			killed = true;		
		}

		public void Run(System.Action job) {
			lock (mutex) {
				jobQueue.Enqueue(job);
			}
		}

		public void Dispatch(System.Action job) {
			Internal.Dispatcher.Dispatch(job);
		}


		void InternalLoop(int idx) {
			int i = idx;
			System.Threading.Thread.Sleep(i);

			int jobCount;

			while (!killed) {
				lock (mutex) {
					jobCount = jobQueue.Count;
				}

				if (jobCount > 0) {
					busy[i] = true;
					System.Action job;
					lock (mutex) {
						job = jobQueue.Dequeue();
					}
					try {
						job.Invoke();
					} catch (System.Exception e) {
						Debug.LogError(e.Message + "\n" + e.StackTrace);
					}
				} else {
					busy[i] = false;
				}

				System.Threading.Thread.Sleep(0);
			}
			Debug.Log("[THREADING] Stopping worker thread " + (i + 1));
		}
	}


	namespace Internal {
		public sealed class Dispatcher : MonoBehaviour {

			static Dispatcher instance;

			bool killAllThreads = false;
			public static bool killAll { get { return instance.killAllThreads; } }

			volatile List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
			volatile Queue<System.Action> dispatch = new Queue<System.Action>();

			static object mutex;

			[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
			static void Initialize() {
				mutex = new object();
				instance = new GameObject("Threading").AddComponent<Dispatcher>();
				instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
				DontDestroyOnLoad(instance.gameObject);
			}

			public static event System.Action onKillThreads;

			void FixedUpdate() {
				if (dispatch.Count > 0) {
					lock (mutex) {
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
				if (onKillThreads != null)
					onKillThreads();
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
				lock (mutex) {
					instance.dispatch.Enqueue(what);
				}
			}
		}
	}
}
