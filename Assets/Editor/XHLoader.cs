// ��̬������Ϸ�е���Դ

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
        /// ��·���и�Ϊǰ׺����Դ��
        /// </summary>
        /// <param name="path">·��</param>
        /// <param name="prefix">ǰ׺</param>
        /// <param name="assetName">��Դ��</param>
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
        /// ���ض�����Դ
        /// </summary>
        /// <typeparam name="T">��Դ����</typeparam>
        /// <param name="paths">��Դ·���б�</param>
        /// <param name="action">ί��</param>
        /// <param name="isSync">�Ƿ�ͬ��</param>
        /// <param name="loadMethod">���ط���</param>
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
                                    Debug.LogError("Resourcesͬ������ʧ�� " + assetNames[i]);
                                }
                                else
                                {
                                    dict.Add(assetNames[i], asset);
                                }
                            }
                            list.Add(asset as T);
                        }
                        action(list);
                        Debug.Log("Resourcesͬ�����سɹ�");
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
                                    Debug.LogError("AssetBundle����ab��ʧ��");
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
                                    Debug.LogError("AssetBundleͬ������ʧ��");
                                }
                                else
                                {
                                    dict.Add(assetNames[i], asset);
                                }
                            }
                            list.Add(asset as T);

                        }
                        action(list);
                        Debug.Log("AssetBundleͬ�����سɹ�");
                    }
                    else
                    {
                        StartCoroutine(AssetBundleAsync<T>(prefixs, assetNames, action));
                    }
                }
                break;
                // ֻ�ڱ༭��ģʽ��ʹ�ã�û���첽����
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
                                    Debug.LogError("AssetDataBaseͬ������ʧ��");
                                }
                                else
                                {
                                    dict.Add(assetNames[i], asset);
                                }
                            }
                            list.Add(asset as T);
                        }
                        action(list);
                        Debug.Log("AssetDataBaseͬ�����سɹ�");
                    }
                    else
                    {
                        Debug.Log("AssetDataBaseû���첽����");
                    }
#endif
                }
                break;
                // û��ͬ������
                case LoadMethod.UnityWebRequest:
                {
                    if (isSync)
                    {
                        Debug.Log("UnityWebRequestû��ͬ������");
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
        /// ж�ض�����Դ
        /// </summary>
        /// <typeparam name="T">��Դ����</typeparam>
        /// <param name="paths">��Դ·���б�</param>
        /// <param name="isSync">�Ƿ�ͬ��</param>
        /// <param name="loadMethod">���ط���</param>
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
                    Debug.Log("Resourcesж�����");
                }
                break;
                case LoadMethod.AssetBundle:
                {
                    AssetBundle.UnloadAllAssetBundles(true);
                    Debug.Log("AssetBundleж�����");
                }
                break;
                case LoadMethod.AssetDataBase:
                {
                    Resources.UnloadUnusedAssets();
                    Debug.Log("AssetDataBaseж�����");
                }
                break;
                case LoadMethod.UnityWebRequest:
                {
                    AssetBundle.UnloadAllAssetBundles(true);
                    Debug.Log("UnityWebRequestж�����");
                }
                break;
            }

        }

        IEnumerator ResourcesAsync<T>(List<string> paths, List<string> assetNames, Action<List<T>> action) where T : Object
        {
            List<T> list = new List<T>();
            //�ؼ���yield return���յ�ResourceRequest��ֻ�е�����Դ������Ϻ�Unity�Ż�ִ�к��沿�ֵĴ���
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
                        Debug.LogError("Resources�첽����ʧ��");
                        yield break;
                    }
                    asset = resourceRequest.asset;
                    dict.Add(assetNames[i], asset);
                }
                list.Add(asset as T);
            }
            action(list);
            Debug.Log("Resources�첽���سɹ�");
        }

        IEnumerator AssetBundleAsync<T>(List<string> abPaths, List<string> assetNames, Action<List<T>> action) where T : Object
        {
            List<T> list = new List<T>();
            //�ؼ������첽���ذ������첽������Դ
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
                        Debug.LogError("AssetBundle����ab��ʧ��");
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
                        Debug.LogError("AssetBundle�첽����ʧ��");
                        yield break;
                    }
                    dict.Add(assetNames[i], asset);
                }

                list.Add(asset as T);
            }

            action(list);
            Debug.Log("AssetBundle�첽���سɹ�");
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
                        Debug.LogError("UnityWebRequest����ab��ʧ��");
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
                        Debug.LogError("AssetBundle�첽����ʧ��");
                        yield break;
                    }
                    dict.Add(assetNames[i], asset);
                }
                list.Add(asset as T);
            }

            action(list);
            Debug.Log("UnityWebRequest�첽���سɹ�");
        }
    }
}