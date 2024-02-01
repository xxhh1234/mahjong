using System;
using System.Collections.Generic;
using UnityEngine;

namespace XH
{
    class ThreadManager : MonoBehaviour
    {
        private static bool NoUpdate =true;
        private static List<Action> UpdateQueue = new List<Action>();
        private static List<Action> UpdateRunQueue = new List<Action>();

        public static void ExecuteUpdate(Action action)
        {
            lock (UpdateQueue)
            {
                UpdateQueue.Add(action);
                NoUpdate = false;
            }
        }
      
        private void FixedUpdate()
        {
            lock (UpdateQueue)
            {
                if (NoUpdate) return;
                UpdateRunQueue.AddRange(UpdateQueue);
                UpdateQueue.Clear();
                NoUpdate = true;
                for (var i = 0; i < UpdateRunQueue.Count; i++)
                {
                    var action = UpdateRunQueue[i];
                    if (action == null) continue;
                    action();
                }
                UpdateRunQueue.Clear();
            }
        }
    }
}
