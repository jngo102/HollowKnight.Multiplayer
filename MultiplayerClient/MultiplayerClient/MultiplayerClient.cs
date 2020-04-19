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

            playerPrefab.SetActive(false);

            GameObject gameManager = new GameObject("Game Manager");
            GameManager gm = gameManager.AddComponent<GameManager>();
            gm.playerPrefab = playerPrefab;
            
            Object.DontDestroyOnLoad(clientManager);
            Object.DontDestroyOnLoad(playerPrefab);
            Object.DontDestroyOnLoad(gameManager);

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        }
        
        private void OnSceneChanged(Scene prevScene, Scene nextScene)
        {
            Log("isConnected: " + Client.Instance.isConnected);
            if (Client.Instance.isConnected)
            {
                Log("Is Connected and Changing Scene.");
                ClientSend.SceneChanged(nextScene.name);
            }
        }
    }
}