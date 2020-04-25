using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerServer
{
    public class ThreadManager : MonoBehaviour
    {
        private static readonly List<Action> ToExecuteOnMainThread = new List<Action>();
        private static readonly List<Action> ExecuteCopiedOnMainThread = new List<Action>();
        private static bool _actionToExecuteOnMainThread;

        private void FixedUpdate()
        {
            UpdateMain();
        }
        
        /// <summary>Sets an action to be executed on the main thread.</summary>
        /// <param name="action">The action to be executed on the main thread.</param>
        public static void ExecuteOnMainThread(Action action)
        {
            if (action == null)
            {
                Log("No action to execute on main thread!");
                return;
            }

            lock (ToExecuteOnMainThread)
            {
                ToExecuteOnMainThread.Add(action);
                _actionToExecuteOnMainThread = true;
            }
        }

        /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
        public static void UpdateMain()
        {
            if (_actionToExecuteOnMainThread)
            {
                ExecuteCopiedOnMainThread.Clear();
                lock (ToExecuteOnMainThread)
                {
                    ExecuteCopiedOnMainThread.AddRange(ToExecuteOnMainThread);
                    ToExecuteOnMainThread.Clear();
                    _actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < ExecuteCopiedOnMainThread.Count; i++)
                {
                    ExecuteCopiedOnMainThread[i]();
                }
            }
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Thread Manager] " + message);
    }
}