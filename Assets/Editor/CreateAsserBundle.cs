using System.IO;
using UnityEditor;

public class CreateAssetBundles
{
    //使用一个静态方法进行打包，将这个方法添加到菜单栏
    [MenuItem("XH/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        //因为打包时不会自动创建路径，所以检验打包路径是否存在，不存在需要先创建打包路径
        string dir = "AssetBundles";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        //打包
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
