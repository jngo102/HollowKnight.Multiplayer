using System;
using System.Collections.Generic;
using System.Linq;
using ModCommon.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerClient
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance;
        public bool PvPEnabled;

        public Dictionary<int, PlayerManager> Players = new Dictionary<int,PlayerManager>();

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
        /// <param name="animation">The starting animation of the spawned player.</param>
        /// <param name="charmsData">List of bools containing charms equipped.</param>
        public void SpawnPlayer(int id, string username, Vector3 position, Vector3 scale, string animation, List<bool> charmsData)
        {
            // Prevent duplication of same player, leaving one idle
            if (Players.ContainsKey(id))
            {
                DestroyPlayer(id);
            }
            
            GameObject player = Instantiate(playerPrefab);

            player.SetActive(true);
            player.SetActiveChildren(true);
            // This component needs to be enabled to run past Awake for whatever reason
            player.GetComponent<PlayerController>().enabled = true;

            player.transform.SetPosition2D(position);
            player.transform.localScale = scale;

            if (Instance.PvPEnabled)
            {
                Log("Enabling PvP Attributes");

                player.layer = 11;

                player.GetComponent<DamageHero>().enabled = true;
            }
            
            player.GetComponent<tk2dSpriteAnimator>().Play(animation);
            
            GameObject nameObj = Instantiate(new GameObject("Username"), position + Vector3.up * 1.25f,
                Quaternion.identity);
            nameObj.transform.SetParent(player.transform);
            nameObj.transform.SetScaleX(0.25f);
            nameObj.transform.SetScaleY(0.25f);
            TextMeshPro nameText = nameObj.AddComponent<TextMeshPro>();
            nameText.text = username;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.fontSize = 24;
            nameText.outlineColor = Color.black;
            nameText.outlineWidth = 0.1f;
            nameObj.AddComponent<KeepWorldScalePositive>();

            DontDestroyOnLoad(player);

            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            playerManager.id = id;
            playerManager.username = username;
            for (int charmNum = 1; charmNum <= 40; charmNum++)
            {
                playerManager.SetAttr("equippedCharm_" + charmNum, charmsData[charmNum - 1]);
            }

            Players.Add(id, playerManager);
        }

        public void EnablePvP(bool enable)
        {
            Instance.PvPEnabled = enable;
            foreach (KeyValuePair<int, PlayerManager> pair in Players)
            {
                Log($"Getting Player {pair.Key}");
                GameObject player = pair.Value.gameObject;
                Log("Changing Player Layer");
                player.layer = enable ? 11 : 9;
                Log("Enabling DamageHero");
                player.GetComponent<DamageHero>().enabled = enable;
                Log("Done.");
            }
        }

        public void DestroyPlayer(int playerId)
        {
            Log("Destroying Player " + playerId);
            Destroy(Players[playerId].gameObject);
            Players.Remove(playerId);
        }

        public void DestroyAllPlayers()
        {
            List<int> playerIds = new List<int>(Players.Keys);
            foreach (int playerId in playerIds)
            {
                DestroyPlayer(playerId);
            }
        }

        private static void Log(object message) => Modding.Logger.Log("[Game Manager] " + message);
    }
}