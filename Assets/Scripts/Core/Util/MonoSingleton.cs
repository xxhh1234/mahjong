using UnityEngine;

namespace XH
{
    /// <summary>
    /// ˫��ʵ�ֵ���ģʽ���κμ̳и�����඼�ǵ�����
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }
                }
                return instance;
            }
        }


        private void Awake()
        {
            if (instance == null) instance = this as T;
        }

        private void OnApplicationQuit()
        {
            instance = null;
        }
    }
}

