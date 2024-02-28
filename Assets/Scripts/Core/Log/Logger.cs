using System;
using UnityEngine;

namespace XH
{
    public class Logger
    {
        public static void XH_LOG(string str)
        {
            #if DEBUG
                Debug.Log(str);
            #endif
        }        
        public static void XH_ASSERT(bool condition, string str)
        {
            #if DEBUG
                Debug.Assert(condition, str);
            #endif
        }
        public static void XH_EXCEPTIION(Exception exception)
        {
            #if DEBUG
                Debug.LogException(exception);
            #endif
        }
    }
}
