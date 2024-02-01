using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    /// <summary>
    /// 对象池（重复使用的游戏物体）子弹，技能，导弹，敌人
    /// </summary>
    class ObjectPool : MonoSingleton<ObjectPool>
    {
        /// <summary>
        /// 存储游戏对象的容器
        /// </summary>
        public Dictionary<string, Queue<GameObject>> cache = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// 返回对象池中未激活的对象，否则生成一个新对象并返回
        /// </summary>
        /// <param name="key">对象名称</param>
        /// <param name="prefab">对象预制体</param>
        /// <param name="position">对象位置</param>
        /// <param name="quaternion">对象旋转角度</param>
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
        /// 直接回收游戏对象
        /// </summary>
        /// <param name="go"></param>
        public void CollectGameObject(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(transform);
        }

        /// <summary>
        /// 延迟回收游戏对象
        /// </summary>
        /// <param name="go">回收的游戏对象</param>
        /// <param name="delay">延迟回收的时间</param>
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
        /// 释放对象名称所对应的所有游戏对象
        /// </summary>
        /// <param name="key">对象名称</param>
        public void Clear(string key)
        {
            while (cache[key].Count != 0)
            {
                Object.Destroy(cache[key].Dequeue());
            }
            cache.Remove(key);
        }

        /// <summary>
        /// 释放对象池中的所有游戏对象
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
