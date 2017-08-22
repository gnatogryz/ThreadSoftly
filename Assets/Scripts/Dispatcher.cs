using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AAAA {

	public static class Threader {
		public static void Run(System.Action job) {
			Internal.Dispatcher.Run(job);
		}

		public static void Dispatch(System.Action job) {
			Internal.Dispatcher.Dispatch(job);
		}

		public static void Sleep(int milliseconds) {
			System.Threading.Thread.Sleep(milliseconds);
		}

		public static bool alive {
			get {
				return !Internal.Dispatcher.killAll;
			}
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
				busy[idx] = false;
				loopThreads[idx] = new System.Threading.Thread(() => InternalLoop(idx));
				loopThreads[idx].Start();
			}
		}

		public void Kill() {
			Internal.Dispatcher.onKillThreads -= Kill;
			killed = true;
			for (int i = 0; i < loopThreads.Length; i++) {
				if (loopThreads[i].IsAlive) {
					loopThreads[i].Interrupt();
					loopThreads[i].Join(100);
				}
			}
			System.Threading.Thread.Sleep(100);
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

			bool killAllThreads = true;

			public static bool killAll { get { return instance.killAllThreads; } }

			volatile List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
			volatile Queue<System.Action> dispatch = new Queue<System.Action>();

			static object mutex;

			static Worker worker;

			[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
			static void Initialize() {
				instance = new GameObject("Threading").AddComponent<Dispatcher>();
				instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
				instance.killAllThreads = false;
				DontDestroyOnLoad(instance.gameObject);
				
				worker = new Worker(8);
				mutex = new object();
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
				//worker.Kill();
				//KillAllThreads();
			}

			void OnDestroy() {
				//worker.Kill();
				//KillAllThreads();
			}


			void OnApplicationQuit() {
				worker.Kill();
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


			public static void Run(System.Action what) {
				/*var th = new System.Threading.Thread(what);
				instance.threads.Add(th);
				th.Start();*/
				worker.Run(what);
				//return th;
			}


			public static void Dispatch(System.Action what) {
				lock (mutex) {
					instance.dispatch.Enqueue(what);
				}
			}
		}
	}
}
