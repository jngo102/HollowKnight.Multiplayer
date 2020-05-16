using System;
using System.Collections.Generic;
using System.IO;
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
        public void SpawnPlayer(byte id, string username, Vector3 position, Vector3 scale, string animation, List<bool> charmsData)
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

        public void CompileByteFragments(int client)
        {
            Log("Compiling Texture for client: " + client);
            PlayerManager playerManager = Players[client];
            Dictionary<int, byte[]> dict = playerManager.texIndexedByteDict;
            Log("Creating texBytes");
            byte[] texBytes = new byte[4093 * dict.Count];
            Log("Loop");
            for (short i = 0; i < dict.Count; i++)
            {
                Array.Copy(dict[i], 0, texBytes, i * 4093, 4093);
            }

            Log("Creating knightTex");
            Texture2D knightTex = new Texture2D(1, 1);
            Log("Loading knightTex");
            knightTex.LoadImage(texBytes);
            knightTex.name = "atlas1";

            Log("New Material");
            var mat = new Material(Shader.Find("Sprites/Default-ColorFlash"));
            Log("Setting tex");
            mat.mainTexture = knightTex;
            Log("knightTex name: " + knightTex.name);

            Log("Getting anim and sprite");
            GameObject player = playerManager.gameObject;
            var anim = player.GetComponent<tk2dSpriteAnimator>();
            var sprite = player.GetComponent<tk2dSprite>();
            Log("Setting Collection material");
            Material newMaterial = new Material(Shader.Find("Sprites/Default-ColorFlash"));
            newMaterial.mainTexture = knightTex;
            newMaterial.name = "atlas1 material";
            sprite.GetCurrentSpriteDef().material.mainTexture = knightTex;
            playerManager.knightTexture = knightTex;

            Log("Writing knightTex to PNG");
            File.WriteAllBytes(Path.Combine(Application.streamingAssetsPath, "Test.png"), texBytes);
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