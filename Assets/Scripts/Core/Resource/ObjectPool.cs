using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    /// <summary>
    /// ����أ��ظ�ʹ�õ���Ϸ���壩�ӵ������ܣ�����������
    /// </summary>
    class ObjectPool : MonoSingleton<ObjectPool>
    {
        /// <summary>
        /// �洢��Ϸ���������
        /// </summary>
        public Dictionary<string, Queue<GameObject>> cache = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// ���ض������δ����Ķ��󣬷�������һ���¶��󲢷���
        /// </summary>
        /// <param name="key">��������</param>
        /// <param name="prefab">����Ԥ����</param>
        /// <param name="position">����λ��</param>
        /// <param name="quaternion">������ת�Ƕ�</param>
        /// <returns></returns>
        public GameObject GetGameObject(string key, GameObject prefab=null,
            Vector3 position = default, Quaternion quaternion = default)
        {
            GameObject go = null;
            if (cache.ContainsKey(key))
            {
                Queue<GameObject> Q = cache[key];
                int n = Q.Count;
                GameObject temp = null;
                for (int i = 0; i < n; ++i)
                {
                    temp = Q.Dequeue();
                    Q.Enqueue(temp);
                    temp.TryGetComponent<Image>(out Image img1);
                    prefab.TryGetComponent<Image>(out Image img2);
                    if (!temp.activeSelf && img1 == img2)
                    {
                        go = temp;
                        break;
                    }
                }
            }
            if (go != null)
            {
                go.transform.position = position;
                go.transform.rotation = quaternion;
            }
            else
            {
                go = Object.Instantiate(prefab, position, quaternion);
                if (!cache.ContainsKey(key)) cache.Add(key, new Queue<GameObject>());
                cache[key].Enqueue(go);
            }
            go.name = key;
            go.SetActive(true);
            return go;
        }

        /// <summary>
        /// ֱ�ӻ�����Ϸ����
        /// </summary>
        /// <param name="go"></param>
        public void CollectGameObject(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(transform);
        }

        /// <summary>
        /// �ӳٻ�����Ϸ����
        /// </summary>
        /// <param name="go">���յ���Ϸ����</param>
        /// <param name="delay">�ӳٻ��յ�ʱ��</param>
        public void CollectGameObject(GameObject go, float delay)
        {
            StartCoroutine(Collect(go, delay));
        }

        private IEnumerator Collect(GameObject go, float delay)
        {
            yield return delay;
            go.SetActive(false);
            go.transform.SetParent(transform);
        }

        /// <summary>
        /// �ͷŶ�����������Ӧ��������Ϸ����
        /// </summary>
        /// <param name="key">��������</param>
        public void Clear(string key)
        {
            while (cache[key].Count != 0)
            {
                Object.Destroy(cache[key].Dequeue());
            }
            cache.Remove(key);
        }

        /// <summary>
        /// �ͷŶ�����е�������Ϸ����
        /// </summary>
        public void ClearAll()
        {
            var list = new List<string>(cache.Keys);
            for (int i = 0; i < list.Count; ++i)
            {
                Clear(list[i]);
            }
        }

        ~ObjectPool()
        {
            ClearAll();
        }
    }
}
