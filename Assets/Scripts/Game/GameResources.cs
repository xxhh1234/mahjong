using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoSingleton<GameResources>
{
    // Prefab
    #region
    // Prefab/SiChuan
    /// <summary>
    /// 主相机预制体
    /// </summary>
    public GameObject  prefabMainCamera;
    /// <summary>
    /// 登录界面预制体
    /// </summary>
    public GameObject  prefabLoginPanel;
    /// <summary>
    /// 日志界面预制体
    /// </summary>
    public GameObject prefabLogPanel;
    /// <summary>
    /// 大厅界面预制体
    /// </summary>
    public GameObject prefabLobbyPanel;
    /// <summary>
    /// 房间界面预制体
    /// </summary>
    public GameObject prefabRoomPanel;
    // Prefab/RoomPanel
    /// <summary>
    /// 房间界面里的按钮预制体
    /// </summary>
    public GameObject prefabButtons;
    public List<GameObject> prefabButtonList = new List<GameObject>();
    /// <summary>
    /// 房间界面里的吃牌边框
    /// </summary>
    public GameObject prefabChiFrame;
    /// <summary>
    /// 房间界面里的特效预制体
    /// </summary>
    public GameObject prefabEffects;
    public List<GameObject> prefabEffectList = new List<GameObject>();
    /// <summary>
    /// 房间界面里的空物体
    /// </summary>
    public GameObject prefabEmpty;
    /// <summary>
    /// 房间界面里的手牌预制体
    /// </summary>
    public GameObject prefabHandTile;
    public List<GameObject> prefabHandTileList = new List<GameObject>();
    /// <summary>
    /// 房间界面里的抛牌预制体
    /// </summary>
    public GameObject prefabThrowTile;
    public List<GameObject> prefabThrowTileList = new List<GameObject>();
    #endregion

    // Sprite
    #region
    /// <summary>
    /// 玩家头像图片
    /// </summary>
    public Sprite spriteBoy, spriteGirl, spriteDefault;
    /// <summary>
    /// 房间中控图片
    /// </summary>
    public Sprite spriteDNXB, spriteNXBD, spriteXBDN, spriteBDNX;
    public List<Sprite> spriteController = new List<Sprite>();
    #endregion

    // AudioClip
    #region 
    /// <summary>
    /// 背景音乐
    /// </summary>
    public AudioClip lobbyBgm, roomBgm;
    #endregion

    public string LoadPath;
    public bool isSync;
    public LoadMethod loadMethod = LoadMethod.Resources;

    public void LoadAssets()
    {
        // 1.同步加载Prefab/Common文件夹下的资源
        #region
        LoadPath = "Prefab/Common/";
        isSync = true;
        List<string> paths = new List<string>();
        paths.Add(LoadPath + "MainCamera");
        paths.Add(LoadPath + "LoginPanel");
        paths.Add(LoadPath + "LogPanel");
        paths.Add(LoadPath + "LobbyPanel");
        paths.Add(LoadPath + "RoomPanel");
        DynamicLoader.Instance.Load<GameObject>(paths, (List<GameObject> values) =>
        {
            prefabMainCamera = values[0];
            prefabLoginPanel = values[1];
            prefabLogPanel = values[2];
            prefabLobbyPanel = values[3];
            prefabRoomPanel = values[4];

        }, isSync, loadMethod);
        #endregion
        // 2.同步加载Sprite文件夹下的资源
        #region
        paths.Clear();
        LoadPath = "Sprite/";
        isSync = true;
        paths.Add(LoadPath + "spriteBoy");
        paths.Add(LoadPath + "spriteGirl");
        paths.Add(LoadPath + "spriteDefault");
        paths.Add(LoadPath + "spriteDNXB");
        paths.Add(LoadPath + "spriteNXBD");
        paths.Add(LoadPath + "spriteXBDN");
        paths.Add(LoadPath + "spriteBDNX");
        DynamicLoader.Instance.Load<Sprite>(paths, (List<Sprite> values) =>
        {
            spriteBoy = values[0];
            spriteGirl = values[1];
            spriteDefault = values[2];
            spriteDNXB = values[3];
            spriteNXBD = values[4];
            spriteXBDN = values[5];
            spriteBDNX = values[6];

        }, isSync, loadMethod);
        #endregion
        // 3.异步加载Prefab/RoomPanel文件夹下的资源
        #region
        paths.Clear();
        LoadPath = "Prefab/RoomPanel/";
        isSync = false;
        paths.Add(LoadPath + "Buttons");
        paths.Add(LoadPath + "ChiFrame");
        paths.Add(LoadPath + "Effects");
        paths.Add(LoadPath + "Empty");
        paths.Add(LoadPath + "HandTile");
        paths.Add(LoadPath + "ThrowTile");
        DynamicLoader.Instance.Load<GameObject>(paths, (List<GameObject> values) =>
        {
            prefabButtons= values[0];
            prefabChiFrame = values[1];
            prefabEffects = values[2];
            prefabEmpty = values[3];
            prefabHandTile = values[4];
            prefabThrowTile = values[5];
        }, isSync, loadMethod);
        #endregion
        // 4.异步加载AudioClip/bgm/文件夹下的资源
        /*
        paths.Clear();
        LoadPath = "AudioClip/bgm/";
        isSync = false;
        paths.Add(LoadPath + "lobbyBgm");
        paths.Add(LoadPath + "roomBgm");
        DynamicLoader.Instance.Load<AudioClip>(paths, (List<AudioClip> values) =>
        {
            lobbyBgm = values[0];
            roomBgm = values[1];
        }, isSync, loadMethod);
        */
        // 5. 实例化主相机和登录界面(日志界面)
        GameManager.Instance.goMainCamera = ObjectPool.Instance.GetGameObject("MainCamera", prefabMainCamera,
            new Vector3(0, 0, -10), Quaternion.identity);
        GameManager.Instance.goLoginPanel = ObjectPool.Instance.GetGameObject("LoginPanel", prefabLoginPanel);
        GameManager.Instance.goLogPanel = ObjectPool.Instance.GetGameObject("LogPanel", prefabLogPanel);
    }
}
