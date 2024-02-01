using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace XH
{
    class ResourceManager : CSharpSingleton<ResourceManager>
    {
        private static readonly int resourceCount = 1000;
        private readonly LRUCache<string, Object> resourcePool = new LRUCache<string, Object>(resourceCount);
        

        public void Init()
        {

        }

        public T LoadPrefab<T>(string prefabName, string path=null, bool isSync=true) where T : UnityEngine.Object
        {
            T res = resourcePool.Get(prefabName) as T;
            if(res == null)
            {
                XHLogger.XH_ASSERT(path != null, string.Format("资源池中没有资源{0}", prefabName));
                Cut(path, out string prefixName, out string assetName);
                #if DEBUG
                    if(isSync)
                        res = Resources.Load<T>(path);
                    else
                        res = Resources.LoadAsync<T>(path).asset as T;
                #else
                    if(isSync)
                        res = AssetBundle.LoadFromFile(prefixName).LoadAsset<T>(assetName);
                    else
                        res = AssetBundle.LoadFromFileAsync(prefixName).assetBundle.LoadAsset<T>(assetName);  
                #endif
            }

            return res;
        }

        public Sprite LoadSprite(string atlasName, string spriteName)
        {
            SpriteAtlas atlas = LoadPrefab<SpriteAtlas>(atlasName);
            return atlas.GetSprite(spriteName);
        }

        private void Cut(string path, out string prefix, out string assetName)
        {
            int i = path.Length - 1;
            for (; i >= 0; --i)
            {
                if (path[i] == '/') break;
            }
            prefix = path.Substring(0, i);
            assetName = path.Substring(i + 1);
        }
    }
}
