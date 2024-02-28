using UnityEngine;

namespace XH
{
    class GameManager : MonoSingleton<GameManager>
    {
        public Camera MainCamera;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitiaizedGame()
        {
            NetManager.Instance.Init();
            GameObjectManager.Instance.Init();
            ResourceManager.Instance.Init();
            Instance.MainCamera = GameObjectManager.Instance.GetGO("MainCamera").GetComponent<Camera>();
            Instance.MainCamera.transform.SetParent(GameObjectManager.Instance.GOManager.transform, false);
            AudioManager.Instance.Init();
            UIManager.Instance.Init("mahjong.");
            Instance.Init();
        }

        private void OnApplicationQuit()
        {
            UIManager.Instance.UnInit();
            AudioManager.Instance.UnInit();
            ResourceManager.Instance.UnInit();
            GameObjectManager.Instance.UnInit();
            NetManager.Instance.UnInit();
        }


        public void Init()
        {
            Instance.gameObject.name = "GameManager";
            Instance.gameObject.AddComponent<ThreadManager>();

            UIManager.Instance.ShowView("LoginView");
        }
    }
}