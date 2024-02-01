using System;
using System.Collections.Generic;
using UnityEngine;

namespace XH
{
    /// <summary>
    /// 管理游戏对象
    /// 1 创建游戏对象
    /// </summary>
    class GameObjectManager : CSharpSingleton<GameObjectManager>
    {
        private struct GOProps : IComparer<GOProps>
        {
            public bool isActive;
            public string goName;
            public GOProps(string name, bool active)
            {
                isActive = active;
                goName = name;
            }

            public int Compare(GOProps x, GOProps y)
            {
                if(x.isActive != y.isActive)
                    return !x.isActive ? -1 : 1;
                else
                    return x.goName.CompareTo(y.goName);
            }
        }

        private Dictionary<string, SortedList<GOProps, GameObject>> objectPool = new Dictionary<string, SortedList<GOProps, GameObject>>();

        public void Init(Action action)
        {
            if(action != null)
                action();
        }

        public GameObject GetGO(string key, Vector3 position=default, Quaternion quaternion=default)
        { 
            GameObject go = null;
            int count = 0;
            if (objectPool.ContainsKey(key))
            {
                SortedList<GOProps, GameObject> list = objectPool[key];
                count = list.Count;
                if(list.Count > 0 && list.GetEnumerator().Current.Key.isActive == false)
                { 
                    go = list.GetEnumerator().Current.Value;
                    go.name = list.GetEnumerator().Current.Key.goName;
                    objectPool[key].Remove(list.GetEnumerator().Current.Key);
                }
            }
            
            if(go == null)
            { 
                go = ResourceManager.Instance.LoadPrefab<GameObject>(key);
                go.name = key + "_" + (count + 1).ToString();
            }
            go.transform.position = position;
            go.transform.rotation = quaternion;
            go.SetActive(true);

            return go;
        }

        public void CollectGO(GameObject go)
        {
            go.SetActive(false);
            int count = objectPool[go.name].Count;
            GOProps props = new GOProps(go.name + "_" + (count + 1).ToString(),  false);
            objectPool[go.name].Add(props, go);
        }
    }
}
