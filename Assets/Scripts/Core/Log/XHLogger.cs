// 调试模式下调试代码

using System;
using Unity.VisualScripting;
using UnityEngine;

namespace XH
{
    public class XHLogger
    {
        public static void XH_LOG(string str)
        {
            #if DEBUG
                Debug.Log(str);
            #endif
        }

        public static void XH_ERROR(string str)
        {
            #if DEBUG
            { 
                Debug.LogError(str);
                Environment.Exit(-1);
            }
            #endif
        }

        public static void XH_ASSERT(bool condition, string str)
        {
            #if DEBUG
                if(!condition)
                    XH_ERROR(str);
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
