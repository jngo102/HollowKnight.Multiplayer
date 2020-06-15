using System;
using System.Collections;
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
        public bool SpectatorMode;

        public Dictionary<byte, PlayerManager> Players = new Dictionary<byte, PlayerManager>();

        // Loaded texture list, indexed by their hash. A texture can be shared by multiple players.
        public Dictionary<byte[], Texture2D> loadedTextures = new Dictionary<byte[], Texture2D>(new ByteArrayComparer());

        public byte MaxPlayers = 50;
        
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

            Log("Done Spawning Player " + id);
        }

        public void ReloadPlayerTextures(PlayerManager player)
        {
            foreach(var row in player.texHashes) {
                var hash = row.Key;
                var tt = row.Value;

                if(loadedTextures.ContainsKey(hash))
                {
                    // Texture already loaded : ezpz
                    player.textures[tt] = loadedTextures[hash];
                }
                else
                {
                    if(MultiplayerClient.textureCache.ContainsKey(hash))
                    {
                        // Texture not loaded but on disk : also ezpz
                        byte[] texBytes = File.ReadAllBytes(MultiplayerClient.textureCache[hash]);
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(texBytes);

                        loadedTextures[hash] = texture;
                        player.textures[tt] = texture;
                    }
                    else
                    {
                        // Ask the server for the texture and load it later...
                        ClientSend.RequestTexture(hash);
                    }
                }
            }

            if (player.textures.ContainsKey(TextureType.Knight))
            {
                Log("Knight tex length: " + player.textures[TextureType.Knight].EncodeToPNG().Length);
                var materialPropertyBlock = new MaterialPropertyBlock();
                player.GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetTexture("_MainTex", player.textures[TextureType.Knight]);
                player.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock); ;
            }
        }
        
        public void EnablePvP(bool enable)
        {
            Instance.PvPEnabled = enable;
            foreach (PlayerManager player in Players.Values)
            {
                player.gameObject.layer = enable ? 11 : 9;
                player.gameObject.GetComponent<DamageHero>().enabled = enable;
            }
        }

        public void DestroyPlayer(byte playerId)
        {
            if(Players.ContainsKey(playerId))
            {
                Log("Destroying Player " + playerId);
                Destroy(Players[playerId].gameObject);
                Players.Remove(playerId);
            }
            else
            {
                Log("Was asked to destroy player " + playerId + " even though we don't have it. Ignoring.");
            }
        }

        public void DestroyAllPlayers()
        {
            List<byte> playerIds = new List<byte>(Players.Keys);
            foreach (byte playerId in playerIds)
            {
                DestroyPlayer(playerId);
            }
        }

        private static void Log(object message) => Modding.Logger.Log("[Session Manager] " + message);
    }
}
