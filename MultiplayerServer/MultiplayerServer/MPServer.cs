using UnityEngine;

namespace MultiplayerServer
{
    public class MPServer : MonoBehaviour
    {
        private GameObject _networkManager;
        
        private void Start()
        {
            _networkManager = new GameObject("NetworkManager");
            _networkManager.AddComponent<NetworkManager>();
            _networkManager.AddComponent<ThreadManager>();

            GameObject playerPrefab = new GameObject(
                "PlayerPrefab",
                /*typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(NonBouncer),
                typeof(Player),
                typeof(SpriteFlash),
                typeof(tk2dSprite),
                typeof(tk2dSpriteAnimator)*/
                typeof(SpriteRenderer),
                typeof(Player)
            )
            {
                layer = 9,
            };
            
            NetworkManager.Instance.playerPrefab = playerPrefab;
            
            DontDestroyOnLoad(playerPrefab);
            DontDestroyOnLoad(_networkManager);
        }
    }
}