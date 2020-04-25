using System;
using System.Collections.Generic;
using ModCommon.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerClient
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public bool pvpEnabled;

        public static Dictionary<int, PlayerManager> Players = new Dictionary<int,PlayerManager>();

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
        /// <param name="charmsData">List of bools containing charms equipped.</param>
        /// <param name="pvp">Whether PvP is enabled.</param>
        /// <param name="animation">The starting animation of the spawned player.</param>
        public void SpawnPlayer(int id, string username, Vector3 position, Vector3 scale, string animation, List<bool> charmsData, bool pvp)
        {
            GameObject player = Instantiate(playerPrefab);

            if (Instance.pvpEnabled)
            {
                Log("Adding PvP Collider");

                player.layer = 11;

                GameObject hero = HeroController.instance.gameObject;

                BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
                var heroCollider = hero.GetComponent<BoxCollider2D>();

                collider.isTrigger = true;
                collider.offset = heroCollider.offset;
                collider.size = heroCollider.size;
                collider.enabled = true;

                Bounds bounds = collider.bounds;
                Bounds heroBounds = heroCollider.bounds;
                bounds.min = heroBounds.min;
                bounds.max = heroBounds.max;

                player.AddComponent<DamageHero>();
            }

            player.SetActive(true);
            player.SetActiveChildren(true);
            // This component needs to be enabled to run past Awake for whatever reason
            player.GetComponent<PlayerController>().enabled = true;

            player.transform.SetPosition2D(position);
            player.transform.localScale = scale;

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
            nameText.outlineWidth = 25.0f;
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
        

        public void Destroy(int playerId)
        {
            Log("Destroying Player " + playerId);
            Destroy(Players[playerId].gameObject);
            Players.Remove(playerId);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Game Manager] " + message);
    }
}