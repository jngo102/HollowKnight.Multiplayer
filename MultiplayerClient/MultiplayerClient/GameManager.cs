using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerClient
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        public static Dictionary<int, PlayerManager> Players = new Dictionary<int,PlayerManager>();

        public GameObject localPlayerPrefab;
        public GameObject playerPrefab;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                Log("Instance already exists, destroying object.");
                Destroy(this);
            }
        }

        /// <summary>Spawns a player.</summary>
        /// <param name="id">The player's ID.</param>
        /// <param name="username">The player's username.</param>
        /// <param name="position">The player's starting position.</param>
        /// <param name="scale">The player's starting scale.</param>
        public void SpawnPlayer(int id, string username, Vector3 position, Vector3 scale)
        {
            GameObject player;
            if (id == Client.Instance.myId)
            {
                Log("Instantiating localPlayerPrefab");
                player = Instantiate(localPlayerPrefab);
            }
            else
            {
                Log("Instantiating playerPrefab");
                player = Instantiate(playerPrefab);
            }

            player.SetActive(true);
            // This component needs to be enabled to run past Awake for whatever reason
            player.GetComponent<PlayerController>().enabled = true;

            player.transform.SetPosition2D(position);
            player.transform.localScale = scale;
            
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            playerManager.id = id;
            playerManager.username = username;
            Players.Add(id, playerManager);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Game Manager] " + message);
    }
}