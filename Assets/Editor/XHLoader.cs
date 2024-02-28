// 动态加载游戏中的资源

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace XH
{
    public enum LoadMethod
    {
        Resources, AssetBundle, AssetDataBase, UnityWebRequest
    }

   class XHLoader : MonoBehaviour
    {
        private readonly Dictionary<string, Object> dict = new Dictionary<string, Object>();

        /// <summary>
        /// 将路径切割为前缀和资源名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="prefix">前缀</param>
        /// <param name="assetName">资源名</param>
        public void Cut(string path, out string prefix, out string assetName)
        {
            int i = path.Length - 1;
            for (; i >= 0; --i)
            {
                if (path[i] == '/') break;
            }
            prefix = path.Substring(0, i);
            assetName = path.Substring(i + 1);
        }


        /// <summary>
        /// 加载多种资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="paths">资源路径列表</param>
        /// <param name="action">委托</param>
        /// <param name="isSync">是否同步</param>
        /// <param name="loadMethod">加载方法</param>
        public void Load<T>(List<string> paths, Action<List<T>> action, bool isSync = true, LoadMethod loadMethod = LoadMethod.Resources) where T : Object
        {
            List<string> prefixs = new List<string>();
            List<string> assetNames = new List<string>();
            for (int i = 0; i < paths.Count; ++i)
            {
                Cut(paths[i], out string prefix, out string assetName);
                prefixs.Add(prefix);
                assetNames.Add(assetName);
            }

            List<T> list = new List<T>();
            switch (loadMethod)
            {
                case LoadMethod.Resources:
                {
                    if (isSync)
                    {
                        for (int i = 0; i < paths.Count; ++i)
                        {
                            Object asset;
                            if (dict.ContainsKey(assetNames[i])) asset = dict[assetNames[i]];
                            else
                            {
                                asset = Resources.Load<T>(paths[i]);
                                if (asset == null)
                                {
                                    Debug.LogError("Resources同步加载失败 " + assetNames[i]);
                                }
                                else
                                {
                                    dict.Add(assetNames[i], asset);
                                }
                            }
                            list.Add(asset as T);
                        }
                        action(list);
                        Debug.Log("Resources同步加载成功");
                    }
                    else
                    {
                        StartCoroutine(ResourcesAsync<T>(paths, assetNames, action));
                    }
                }
                break;
                case LoadMethod.AssetBundle:
                {
                    if (isSync)
                    {
                        for (int i = 0; i < prefixs.Count; ++i)
                        {
                            AssetBundle ab;
                            Object asset;
                            if (dict.ContainsKey(prefixs[i])) ab = dict[prefixs[i]] as AssetBundle;
                            else
                            {
                                ab = AssetBundle.LoadFromFile(prefixs[i]);
                                if (ab == null)
                                {
                                    Debug.LogError("AssetBundle加载ab包失败");
                                }
                                else
                                {
                                    dict.Add(prefixs[i], ab);
                                }
                            }

                            if (dict.ContainsKey(assetNames[i])) asset = dict[assetNames[i]];
                            else
                            {
                                asset = ab.LoadAsset(assetNames[i]);
                                if (asset == null)
                                {
                                    Debug.LogError("AssetBundle同步加载失败");
                                }
                                else
                                {
                                    dict.Add(assetNames[i], asset);
                                }
                            }
                            list.Add(asset as T);

                        }
                        action(list);
                        Debug.Log("AssetBundle同步加载成功");
                    }
                    else
                    {
                        StartCoroutine(AssetBundleAsync<T>(prefixs, assetNames, action));
                    }
                }
                break;
                // 只在编辑器模式下使用，没有异步加载
                case LoadMethod.AssetDataBase:
                {
#if UNITY_EDITOR
                    if (isSync)
                    {
                        for (int i = 0; i < paths.Count; ++i)
                        {
                            Object asset;
                            if (dict.ContainsKey(assetNames[i]))
                            {
                                asset = dict[assetNames[i]];
                            }
                            else
                            {
                                asset = AssetDatabase.LoadAssetAtPath<T>(paths[i] + ".prefab");
                                if (asset == null)
                                {
                                    Debug.LogError("AssetDataBase同步加载失败");
                                }
                                else
                                {
                                    dict.Add(assetNames[i], asset);
                                }
                            }
                            list.Add(asset as T);
                        }
                        action(list);
                        Debug.Log("AssetDataBase同步加载成功");
                    }
                    else
                    {
                        Debug.Log("AssetDataBase没有异步加载");
                    }
#endif
                }
                break;
                // 没有同步加载
                case LoadMethod.UnityWebRequest:
                {
                    if (isSync)
                    {
                        Debug.Log("UnityWebRequest没有同步加载");
                    }
                    else
                    {
                        StartCoroutine(UnityWebRequestAsync(prefixs, assetNames, action));
                    }
                }
                break;
            }


        }

        /// <summary>
        /// 卸载多种资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="paths">资源路径列表</param>
        /// <param name="isSync">是否同步</param>
        /// <param name="loadMethod">加载方法</param>
        public void UnLoad<T>(List<string> paths, LoadMethod loadMethod = LoadMethod.Resources) where T : Object
        {
            for (int i = 0; i < paths.Count; ++i)
            {
                Cut(paths[i], out string prefix, out string assetName);
                if (dict.ContainsKey(prefix)) dict.Remove(prefix);
                if (dict.ContainsKey(assetName)) dict.Remove(assetName);
            }
            switch (loadMethod)
            {
                case LoadMethod.Resources:
                {
                    Resources.UnloadUnusedAssets();
                    Debug.Log("Resources卸载完毕");
                }
                break;
                case LoadMethod.AssetBundle:
                {
                    AssetBundle.UnloadAllAssetBundles(true);
                    Debug.Log("AssetBundle卸载完毕");
                }
                break;
                case LoadMethod.AssetDataBase:
                {
                    Resources.UnloadUnusedAssets();
                    Debug.Log("AssetDataBase卸载完毕");
                }
                break;
                case LoadMethod.UnityWebRequest:
                {
                    AssetBundle.UnloadAllAssetBundles(true);
                    Debug.Log("UnityWebRequest卸载完毕");
                }
                break;
            }

        }

        IEnumerator ResourcesAsync<T>(List<string> paths, List<string> assetNames, Action<List<T>> action) where T : Object
        {
            List<T> list = new List<T>();
            //关键：yield return接收的ResourceRequest。只有当该资源加载完毕后，Unity才会执行后面部分的代码
            for (int i = 0; i < paths.Count; ++i)
            {
                Object asset;
                if (dict.ContainsKey(assetNames[i]))
                {
                    asset = dict[assetNames[i]] as T;
                }
                else
                {
                    ResourceRequest resourceRequest = Resources.LoadAsync<T>(paths[i]);
                    yield return resourceRequest;
                    if (resourceRequest.asset == null)
                    {
                        Debug.LogError("Resources异步加载失败");
                        yield break;
                    }
                    asset = resourceRequest.asset;
                    dict.Add(assetNames[i], asset);
                }
                list.Add(asset as T);
            }
            action(list);
            Debug.Log("Resources异步加载成功");
        }

        IEnumerator AssetBundleAsync<T>(List<string> abPaths, List<string> assetNames, Action<List<T>> action) where T : Object
        {
            List<T> list = new List<T>();
            //关键：先异步加载包，再异步加载资源
            for (int i = 0; i < abPaths.Count; ++i)
            {
                AssetBundle ab;
                Object asset;
                if (dict.ContainsKey(abPaths[i]))
                {
                    ab = dict[abPaths[i]] as AssetBundle;
                }
                else
                {
                    AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(abPaths[i]);
                    yield return assetBundleCreateRequest;
                    ab = assetBundleCreateRequest.assetBundle;
                    if (ab == null)
                    {
                        Debug.LogError("AssetBundle加载ab包失败");
                        yield break;
                    }
                    dict.Add(abPaths[i], ab);

                }

                if (dict.ContainsKey(assetNames[i])) asset = dict[assetNames[i]];
                else
                {
                    AssetBundleRequest assetBundleRequest = ab.LoadAssetAsync(assetNames[i]);
                    yield return assetBundleRequest;
                    asset = assetBundleRequest.asset;
                    if (asset == null)
                    {
                        Debug.LogError("AssetBundle异步加载失败");
                        yield break;
                    }
                    dict.Add(assetNames[i], asset);
                }

                list.Add(asset as T);
            }

            action(list);
            Debug.Log("AssetBundle异步加载成功");
        }

        IEnumerator UnityWebRequestAsync<T>(List<string> abPaths, List<string> assetNames, Action<List<T>> action) where T : Object
        {
            List<T> list = new List<T>();
            for (int i = 0; i < abPaths.Count; ++i)
            {
                AssetBundle ab;
                Object asset;
                if (dict.ContainsKey(abPaths[i])) ab = dict[abPaths[i]] as AssetBundle;
                else
                {
                    UnityWebRequest unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(abPaths[i]);
                    yield return unityWebRequest.SendWebRequest();
                    ab = (unityWebRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                    if (ab == null)
                    {
                        Debug.LogError("UnityWebRequest加载ab包失败");
                        yield break;
                    }
                    dict.Add(abPaths[i], ab);
                }

                if (dict.ContainsKey(assetNames[i])) asset = dict[assetNames[i]] as T;
                else
                {
                    AssetBundleRequest assetBundleRequest = ab.LoadAssetAsync(assetNames[i]);
                    yield return assetBundleRequest;
                    asset = assetBundleRequest.asset;
                    if (asset == null)
                    {
                        Debug.LogError("AssetBundle异步加载失败");
                        yield break;
                    }
                    dict.Add(assetNames[i], asset);
                }
                list.Add(asset as T);
            }

            action(list);
            Debug.Log("UnityWebRequest异步加载成功");
        }
    }
}