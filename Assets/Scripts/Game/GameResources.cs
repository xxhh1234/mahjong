using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoSingleton<GameResources>
{
    // Prefab
    #region
    // Prefab/SiChuan
    /// <summary>
    /// �����Ԥ����
    /// </summary>
    public GameObject  prefabMainCamera;
    /// <summary>
    /// ��¼����Ԥ����
    /// </summary>
    public GameObject  prefabLoginPanel;
    /// <summary>
    /// ��־����Ԥ����
    /// </summary>
    public GameObject prefabLogPanel;
    /// <summary>
    /// ��������Ԥ����
    /// </summary>
    public GameObject prefabLobbyPanel;
    /// <summary>
    /// �������Ԥ����
    /// </summary>
    public GameObject prefabRoomPanel;
    // Prefab/RoomPanel
    /// <summary>
    /// ���������İ�ťԤ����
    /// </summary>
    public GameObject prefabButtons;
    public List<GameObject> prefabButtonList = new List<GameObject>();
    /// <summary>
    /// ���������ĳ��Ʊ߿�
    /// </summary>
    public GameObject prefabChiFrame;
    /// <summary>
    /// ������������ЧԤ����
    /// </summary>
    public GameObject prefabEffects;
    public List<GameObject> prefabEffectList = new List<GameObject>();
    /// <summary>
    /// ���������Ŀ�����
    /// </summary>
    public GameObject prefabEmpty;
    /// <summary>
    /// ��������������Ԥ����
    /// </summary>
    public GameObject prefabHandTile;
    public List<GameObject> prefabHandTileList = new List<GameObject>();
    /// <summary>
    /// ��������������Ԥ����
    /// </summary>
    public GameObject prefabThrowTile;
    public List<GameObject> prefabThrowTileList = new List<GameObject>();
    #endregion

    // Sprite
    #region
    /// <summary>
    /// ���ͷ��ͼƬ
    /// </summary>
    public Sprite spriteBoy, spriteGirl, spriteDefault;
    /// <summary>
    /// �����п�ͼƬ
    /// </summary>
    public Sprite spriteDNXB, spriteNXBD, spriteXBDN, spriteBDNX;
    public List<Sprite> spriteController = new List<Sprite>();
    #endregion

    // AudioClip
    #region 
    /// <summary>
    /// ��������
    /// </summary>
    public AudioClip lobbyBgm, roomBgm;
    #endregion

    public string LoadPath;
    public bool isSync;
    public LoadMethod loadMethod = LoadMethod.Resources;

    public void LoadAssets()
    {
        // 1.ͬ������Prefab/Common�ļ����µ���Դ
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
        // 2.ͬ������Sprite�ļ����µ���Դ
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
        // 3.�첽����Prefab/RoomPanel�ļ����µ���Դ
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
        // 4.�첽����AudioClip/bgm/�ļ����µ���Դ
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
        // 5. ʵ����������͵�¼����(��־����)
        GameManager.Instance.goMainCamera = ObjectPool.Instance.GetGameObject("MainCamera", prefabMainCamera,
            new Vector3(0, 0, -10), Quaternion.identity);
        GameManager.Instance.goLoginPanel = ObjectPool.Instance.GetGameObject("LoginPanel", prefabLoginPanel);
        GameManager.Instance.goLogPanel = ObjectPool.Instance.GetGameObject("LogPanel", prefabLogPanel);
    }
}
