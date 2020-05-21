using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ModCommon.Util;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(byte fromClient, Packet packet)
        {
            byte clientIdCheck = packet.ReadByte();
            string username = packet.ReadString();
            string currentClip = packet.ReadString();
            string activeScene = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Vector3 scale = packet.ReadVector3();
            int health = packet.ReadInt();
            int maxHealth = packet.ReadInt();
            int healthBlue = packet.ReadInt();

            List<bool> charmsData = new List<bool>();
            for (int charmNum = 1; charmNum <= 40; charmNum++)
            {
                charmsData.Add(packet.ReadBool());
            }

            for (int i = 0; i < Enum.GetNames(typeof(TextureType)).Length; i++)
            {
                byte[] hash = packet.ReadBytes(20);
                if (!Server.textureHashes.Contains(hash))
                {
                    ServerSend.RequestTexture(fromClient, hash);
                }
            }

            Server.clients[fromClient].SendIntoGame(username, position, scale, currentClip, health, maxHealth, healthBlue, charmsData);
            SceneChanged(fromClient, activeScene);

            Log($"{username} connected successfully and is now player {fromClient}.");
            if (fromClient != clientIdCheck)
            {
                Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck}.");
            }
        }

        public static void HandleTextureFragment(byte fromClient, Packet packet)
        {
            if (!ServerSettings.CustomKnightIntegration) return;

            // hash -20, integer -4 = -24
            int fragment_length = Client.dataBufferSize - 24;

            byte[] hash = packet.ReadBytes(20);
            int remaining = packet.ReadInt();
            if (remaining < fragment_length) fragment_length = remaining;

            var client = Server.clients[fromClient];
            byte[] fragment = packet.ReadBytes(fragment_length);
            if (client.textureFragments.ContainsKey(hash))
            {
                int cur_tex_len = client.textureFragments[hash].Length;
                byte[] new_tex = new byte[cur_tex_len + fragment_length];
                Buffer.BlockCopy(client.textureFragments[hash], 0, new_tex, 0, cur_tex_len);
                Buffer.BlockCopy(fragment, 0, new_tex, cur_tex_len, fragment_length);
                client.textureFragments[hash] = new_tex;
            }
            else
            {
                client.textureFragments[hash] = fragment;
            }

            // Check hash is valid and save
            if (remaining - fragment_length == 0)
            {
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    byte[] computed_hash = sha1.ComputeHash(client.textureFragments[hash]);
                    if (hash == computed_hash)
                    {
                        string hashStr = BitConverter.ToString(hash).Replace("-", string.Empty);
                        string cacheDir = Path.Combine(Application.dataPath, "SkinCache");
                        string filePath = Path.Combine(cacheDir, hashStr);
                        File.WriteAllBytes(filePath, client.textureFragments[hash]);
                        client.textureFragments.Remove(hash);
                        MultiplayerServer.textureCache[hash] = filePath;
                    }
                    else
                    {
                        Log("Player " + fromClient + "'s texture does not match SHA-1.");
                    }
                }
            }
        }

        public static void HandleTextureRequest(byte fromClient, Packet packet)
        {
            if (!ServerSettings.CustomKnightIntegration) return;

            byte[] hash = packet.ReadBytes(20);

            if (MultiplayerServer.textureCache.ContainsKey(hash))
            {
                byte[] texture = File.ReadAllBytes(MultiplayerServer.textureCache[hash]);
                ServerSend.SendTexture(fromClient, hash, texture);
            }
        }
        
        public static void PlayerPosition(byte fromClient, Packet packet)
        {
            Vector3 position = packet.ReadVector3();
            
            Server.clients[fromClient].player.SetPosition(position);
        }

        public static void PlayerScale(byte fromClient, Packet packet)
        {
            Vector3 scale = packet.ReadVector3();

            Server.clients[fromClient].player.SetScale(scale);
        }
        
        public static void PlayerAnimation(byte fromClient, Packet packet)
        {
            string animation = packet.ReadString();
            
            Server.clients[fromClient].player.SetAnimation(animation);
        }

        public static void SceneChanged(byte fromClient, Packet packet)
        {
            string sceneName = packet.ReadString();
            
            Server.clients[fromClient].player.activeScene = sceneName;

            for (byte i = 1; i <= Server.MaxPlayers; i++)    
            {    
                if (Server.clients[i].player != null && i != fromClient)
                {
                    if (Server.clients[i].player.activeScene == sceneName)
                    {
                        Log("Same Scene, Spawning Players Subsequent Pass");
                        Player iPlayer = Server.clients[i].player;
                        Player fromPlayer = Server.clients[fromClient].player;
                        ServerSend.SpawnPlayer(fromClient, iPlayer);
                        ServerSend.SpawnPlayer(i, fromPlayer);
                    }
                    else
                    {
                        Log("Different Scene, Destroying Players");
                        ServerSend.DestroyPlayer(i, fromClient);
                        //ServerSend.DestroyPlayer(fromClient, i);
                    }
                }
            }
        }

        /// <summary>Initial scene load when joining the server for the first time.</summary>
        /// <param name="fromClient">The ID of the client who joined the server</param>
        /// <param name="sceneName">The name of the client's active scene when joining the server</param>
        public static void SceneChanged(byte fromClient, string sceneName)
        {
            Server.clients[fromClient].player.activeScene = sceneName;

            for (byte i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].player != null && i != fromClient)
                {
                    if (Server.clients[i].player.activeScene == sceneName)
                    {
                        Log("Same Scene, Spawning Players First Pass");
                        ServerSend.SpawnPlayer(fromClient, Server.clients[i].player);
                        ServerSend.SpawnPlayer(Server.clients[i].player.id, Server.clients[fromClient].player);
                        Player iPlayer = Server.clients[i].player;
                        Player fromPlayer = Server.clients[fromClient].player;
                        ServerSend.SpawnPlayer(fromClient, iPlayer);
                        ServerSend.SpawnPlayer(i, fromPlayer);
                    }
                }
            }
        }

        public static void HealthUpdated(byte fromClient, Packet packet)
        {
            int currentHealth = packet.ReadInt();
            int currentMaxHealth = packet.ReadInt();
            int currentHealthBlue = packet.ReadInt();

            Log("From Client: " + currentHealth + " " + currentMaxHealth + " " + currentHealthBlue);
            
            Server.clients[fromClient].player.health = currentHealth;
            Server.clients[fromClient].player.maxHealth = currentMaxHealth;
            Server.clients[fromClient].player.healthBlue = currentHealthBlue;

            ServerSend.HealthUpdated(fromClient, currentHealth, currentMaxHealth, currentHealthBlue);
        }
        
        public static void CharmsUpdated(byte fromClient, Packet packet)
        {
            for (int charmNum = 1; charmNum <= 40; charmNum++)
            {
                bool equippedCharm = packet.ReadBool();
                Server.clients[fromClient].player.SetAttr("equippedCharm_" + charmNum, equippedCharm);
            }
            
            ServerSend.CharmsUpdated(fromClient, Server.clients[fromClient].player);
        }
        
        public static void PlayerDisconnected(byte fromClient, Packet packet)
        {
            int id = packet.ReadInt();

            Object.Destroy(Server.clients[id].player.gameObject);
            Server.clients[id].player = null;
            Server.clients[id].Disconnect();
        }

        private static void Log(object message) => Modding.Logger.Log("[Server Handle] " + message);
    }
}
