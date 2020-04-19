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