using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace XH
{
    class ResourceManager : CSharpSingleton<ResourceManager>
    {
        private static readonly int resourceCount = 10000;
        private LRUCache resourcePool = new LRUCache(resourceCount);
        private string resourcePath = Application.dataPath + "\\Resources\\";
        private List<string> patterns = new List<string>()
        { 
            "*.prefab", "*.SpriteAtlas", "*.png", "*.jpg", "*.mp3", "*.wav", "*.ogg"
        };

        public void Init()
        {
            List<string> allPaths = new List<string>();
            string fileName = "RESOURCESPATH";
#if UNITY_EDITOR
            resourcePath = resourcePath.Replace("/", "\\");
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(resourcePath));
            foreach (var dir in dirs)
            {
                foreach (var pattern in patterns)
                {
                    string[] pathInPattern = Directory.GetFiles(dir, pattern, SearchOption.AllDirectories);
                    allPaths.AddRange(pathInPattern);
                }
            }
            string res = "";
            for (int i = 0; i < allPaths.Count; ++i)
            { 
                string path = allPaths[i];
                path = path.Split("Resources\\")[1].Split(".")[0];
                allPaths[i] = path;
                if(i == 0)
                    res += path;
                else
                    res += "\n" + path;
            }
            try
            {

                File.WriteAllText(resourcePath + fileName + ".txt", string.Empty);
                using (StreamWriter writer = new StreamWriter(resourcePath + fileName + ".txt"))
                {
                    writer.WriteLine(res);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
            }
#else
        Load(out TextAsset textFile, fileName, fileName);
        allPaths = textFile.text.Split("\n").ToList();
        allPaths.RemoveAt(allPaths.Count - 1);
        string str = allPaths[allPaths.Count - 1];
        allPaths[allPaths.Count - 1] = str.Trim();
#endif
            foreach (var assetPath in allPaths)
            {
                string assetName = assetPath.Substring(assetPath.LastIndexOf("\\") + 1);
                Load(out object prefab, assetName, assetPath);
                if (assetName.EndsWith("View"))
                {
                    UIManager.Instance.AddViewName(assetName);
                    continue;
                }
                if (prefab is SpriteAtlas)
                {
                    SpriteAtlas atlasPrefab = (SpriteAtlas)prefab;
                    Sprite[] spriteAtlas = new Sprite[100];
                    int size = atlasPrefab.GetSprites(spriteAtlas);
                    for (int i = 0; i < size; ++i)
                    {
                        spriteAtlas[i].name = spriteAtlas[i].name.Split("(")[0];
                        resourcePool.Put(assetName + "_" + spriteAtlas[i].name, spriteAtlas[i]);
                    }
                    continue;
                }
                if (prefab is Texture2D)
                    continue;
                if(prefab is AudioClip)
                    continue;
                GameObjectManager.Instance.AddGo(assetName);
            }
        }
        public void UnInit()
        {
            resourcePool.ClearCache();
        }
        public void Load<T>(out T res, string name, string path=null, bool isSync=true) where T : class
        {
            object obj = resourcePool.Get(name);
            if (obj == null)
            {
                Logger.XH_ASSERT(path != null, string.Format("资源池中没有资源{0}", name));
                Cut(ref path, out string prefixName, out string assetName);
                Logger.XH_ASSERT(name == assetName, string.Format("资源池中没有资源{0}", name));
            // #if DEBUG
                if (isSync)
                        obj = Resources.Load(path);
                else
                        obj = Resources.LoadAsync(path).asset;
            /*
            #else
                if(isSync)
                    obj = AssetBundle.LoadFromFile(prefixName).LoadAsset(assetName);
                else
                    obj = AssetBundle.LoadFromFileAsync(prefixName).assetBundle.LoadAsset(assetName);  
                #endif
            */
                Logger.XH_ASSERT(obj != null, string.Format("资源{0}加载失败", name));
                resourcePool.Put(name, obj);
            }
            Logger.XH_ASSERT(obj != null, string.Format("资源池中没有资源{0}", name));
            res = obj as T;
        }
        public void UnLoad()
        {
            Resources.UnloadUnusedAssets();
        }
        public Sprite LoadSprite(string atlasName, string spriteName)
        {
            string assetName = atlasName + "_" + spriteName;
            Load(out Sprite sprite, assetName);
            return sprite;
        }

        private void Cut(ref string path, out string prefix, out string assetName)
        {
            path = path.Replace("\\", "/");
            path = path.Split(".")[0];
            int index = path.LastIndexOf("/");
            if(index == -1)
            {
                prefix = "";
                assetName = path;
                return;
            }
            prefix = path.Substring(0, index);
            assetName = path.Substring(index + 1);
        }
    }
}
