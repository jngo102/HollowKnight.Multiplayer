using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerServer
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance;

        private const int MaxPlayers = 50;
        private const int Port = 26950;
        
        public GameObject playerPrefab;
        
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                Log("Instance already exists, destroying object.");
                Destroy(Instance);
            }
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;

            Server.Start(MaxPlayers, Port);
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }

        public Player InstantiatePlayer(Vector3 position, Vector3 scale)
        {
            GameObject playerObj = Instantiate(playerPrefab);
            Player playerComponent = playerObj.GetComponent<Player>();
            playerComponent.position = position;
            playerComponent.scale = scale;
            return playerComponent;
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Network Manager] " + message);
    }
}