using System;
using System.Collections;
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
        private struct GOProps : IComparer
        {
            public bool isActive;
            public string goName;
            public GOProps(string name, bool active)
            {
                isActive = active;
                goName = name;
            }

            public int Compare(object x, object y)
            {
                GOProps goProps1 = (GOProps)x;
                GOProps goProps2 = (GOProps)y;
                if (goProps1.isActive != goProps2.isActive)
                    return !goProps1.isActive ? -1 : 1;
                else
                    return goProps1.goName.CompareTo(goProps2.goName);
            }
        }
        

        private Dictionary<string, SortedList> objectPool = new Dictionary<string, SortedList>();

        public GameObject GOManager;

        public void Init()
        {
            GOManager = new GameObject("GameObjectManager");
        }

        public void UnInit()
        {
            GameObject.DestroyImmediate(GOManager);
        }

        public void AddGo(string key, GameObject go=null)
        {
            int count = 0;
            if (objectPool.ContainsKey(key))
                count = objectPool[key].Count;
            else
                objectPool.Add(key, new SortedList(new GOProps()));
       
            if(go == null)
            {
                ResourceManager.Instance.Load(out GameObject prefab, key, null, false);
                go = GameObject.Instantiate(prefab);
                go.SetActive(false);
            }
            go.name = key + "_" + (count + 1).ToString();
            go.transform.SetParent(GOManager.transform, false);
            go.SetActive(false);
            objectPool[key].Add(new GOProps(go.name, false), go);
        }

        public GameObject GetGO(string key, Vector3 position=default, Quaternion quaternion=default)
        { 
            GameObject go = null;
            int count = 0;
            if (objectPool.ContainsKey(key))
            {
                SortedList list = objectPool[key]; 
                GOProps goProps = (GOProps)list.GetKey(0);
                if (list.Count > 0 && goProps.isActive == false)
                {
                    go = (GameObject)list.GetByIndex(0);
                    go.name = goProps.goName;
                    objectPool[key].Remove(goProps);
                }
                count = list.Count;
            }
            else objectPool.Add(key, new SortedList(new GOProps()));
            
            if(go == null)
            { 
                ResourceManager.Instance.Load(out GameObject prefab, key, null, false);
                go = GameObject.Instantiate(prefab, position, quaternion);
                go.name = key + "_" + (count + 1).ToString();
            }
            
            go.transform.position = position;
            go.transform.rotation = quaternion;
            go.SetActive(true);
            objectPool[key].Add(new GOProps(go.name, true), go);

            return go;
        }

        public void CollectGO(GameObject go)
        {
            string key = go.name.Split('_')[0];
            go.transform.SetParent(GOManager.transform, false);
            go.SetActive(false);
            objectPool[key].Remove(new GOProps(go.name, true));
            if (!objectPool[key].ContainsKey(new GOProps(go.name, false)))
                objectPool[key].Add(new GOProps(go.name, false), go);
        }
    }
}
