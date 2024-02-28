using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Linq;
using UnityEngine;
using System;

namespace XH
{
    public class XHAtlasPacker
    {
        /// <summary>
        /// 图集图片的路径
        /// </summary>
        public static string AtlasPath = "Assets/SpriteSheet/";
        /// <summary>
        /// 打包后预制体存放路径
        /// </summary>
        public static string AtlasSavePath = "Assets/Resources/Atlas/";

        /// <summary>
        /// 一个文件夹一张图集
        /// </summary>
        [MenuItem("XH/PackAtlas")]
        public static void PackAtlas()
        {
            string[] patterns = new string[] { "*.png" };
            Dictionary<string, List<string>> allFilePaths = new Dictionary<string, List<string>>();

            List<string> dirs = new List<string>(Directory.EnumerateDirectories(AtlasPath));
            foreach (var dir in dirs)
            {
                string atlasName = dir.Substring(dir.LastIndexOf("/"));
                List<string> atlasPath = new List<string>();
                foreach (var pattern in patterns)
                {
                    string[] pathInPattern = Directory.GetFiles(dir, pattern, SearchOption.AllDirectories);
                    atlasPath.AddRange(pathInPattern);
                }
                CreateAtlasPrefab("atl_" + atlasName, atlasPath.ToArray());
            }
        }

        //创建图集预制体
        public static void CreateAtlasPrefab(string atlasName, string[] atlasPath)
        {
            List<Sprite> atlasList = new List<Sprite>();
            foreach (string p in atlasPath)
                atlasList.AddRange(AssetDatabase.LoadAllAssetsAtPath(p).OfType<Sprite>().ToArray());

            Logger.XH_ASSERT(atlasList == null, "图集列表为空");

            GameObject go = new GameObject();
            go.name = atlasName;
            SpriteData spriteData = go.AddComponent<SpriteData>();
            spriteData.SetSprites = atlasList.ToArray();
            string path = AtlasSavePath + atlasName + ".prefab";
            GameObject temp = PrefabUtility.SaveAsPrefabAsset(go, path);
            /*
                #region 添加ab标记
                //此处自动添加ab标记
                //如果加载方式是Resources.load()等不需要ab标记的可以把此处注释掉
                AssetImporter importer = AssetImporter.GetAtPath(path1);
                if (importer == null || temp == null)
                {
                    Debug.LogError("error: " + path1);
                    return;
                }
                importer.assetBundleName = "ui-share.unity3d";

                #endregion
                */
            GameObject.DestroyImmediate(go);
            EditorUtility.SetDirty(temp);
            AssetDatabase.SaveAssets();

            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }
    }
}

