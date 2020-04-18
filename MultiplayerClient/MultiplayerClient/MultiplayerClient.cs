using Modding;
using MultiplayerClient.Canvas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerClient
{
    public class MultiplayerClient : Mod
    {
        public override string GetVersion()
        {
            return "0.0.1";
        }
        
        public override void Initialize()
        {
            GUIController.Instance.BuildMenus();
            
            GameObject clientManager = new GameObject("ClientManager");
            clientManager.AddComponent<Client>();
            clientManager.AddComponent<ThreadManager>();
            
            GameObject localPlayerPrefab = new GameObject(
                "LocalPlayerPrefab",
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(NonBouncer),
                typeof(SpriteFlash),
                typeof(tk2dSprite),
                typeof(tk2dSpriteAnimator),
                typeof(PlayerController),
                typeof(PlayerManager)
            )
            {
                layer = 9,
            };
            
            GameObject playerPrefab = new GameObject(
                "PlayerPrefab",
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(NonBouncer),
                typeof(SpriteFlash),
                typeof(tk2dSprite),
                typeof(tk2dSpriteAnimator),
                typeof(PlayerController),
                typeof(PlayerManager)
            )
            {
                layer = 9,
            };

            localPlayerPrefab.SetActive(false);
            playerPrefab.SetActive(false);

            Object.DontDestroyOnLoad(clientManager);
            Object.DontDestroyOnLoad(localPlayerPrefab);
            Object.DontDestroyOnLoad(playerPrefab);

            GameObject gameManager = new GameObject("Game Manager");
            GameManager gm = gameManager.AddComponent<GameManager>();
            gm.playerPrefab = playerPrefab;
            gm.localPlayerPrefab = localPlayerPrefab;
            
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        }
        
        private void OnSceneChanged(Scene prevScene, Scene nextScene)
        {
            Log("Scene Changed");
            ClientSend.SceneChanged(nextScene.name);
        }
    }
}