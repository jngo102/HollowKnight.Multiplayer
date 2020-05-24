using System;
using System.Collections.Generic;
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
            
            Server.clients[fromClient].SendIntoGame(username, position, scale, currentClip, health, maxHealth, healthBlue, charmsData);
            SceneChanged(fromClient, activeScene);

            Log($"{username} connected successfully and is now player {fromClient}.");
            if (fromClient != clientIdCheck)
            {
                Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck}.");
            }
        }

        #region CustomKnight Integration

        private static void HandleTexture(byte fromClient, Packet packet, int serverPacketId, string texName)
        {
            int texLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(texLength);

            Player player = Server.clients[fromClient].player;

            player.texBytes[texName] = texBytes;
            player.texHashes[texName] = texBytes.Hash();
            
            ServerSend.SendTexture(fromClient, texBytes, serverPacketId);
        }

        private static void HandleTextureUpToDate(byte fromClient, Packet packet, int serverPacketId, string texName)
        {
            byte[] texBytes = Server.clients[fromClient].player.texBytes[texName];
            ServerSend.SendTexture(fromClient, texBytes, serverPacketId);
        }
        
        public static void BaldurTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.BaldurTexture, "Baldur");
        }
        
        public static void FlukeTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.FlukeTexture, "Fluke");
        }
        
        public static void GrimmTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.GrimmTexture, "Grimm");
        }
        
        public static void HatchlingTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.HatchlingTexture, "Hatchling");
        }

        public static void KnightTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.KnightTexture, "Knight");
        }

        public static void ShieldTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.ShieldTexture, "Shield");
        }
        
        public static void SprintTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.SprintTexture, "Sprint");
        }
        
        public static void UnnTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.UnnTexture, "Unn");
        }
        
        public static void VoidTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.VoidTexture, "Void");
        }
        
        public static void VSTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.VSTexture, "VS");
        }
        
        public static void WeaverTexture(byte fromClient, Packet packet)
        { 
            HandleTexture(fromClient, packet, (int) ServerPackets.WeaverTexture, "Weaver");
        }
        
        public static void WraithsTexture(byte fromClient, Packet packet)
        {
            HandleTexture(fromClient, packet, (int) ServerPackets.WraithsTexture, "Wraiths");
        }
        
        public static void BaldurTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.BaldurTexture, "Baldur");
        }
        
        public static void FlukeTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.FlukeTexture, "Fluke");
        }
        
        public static void GrimmTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.GrimmTexture, "Grimm");
        }
        
        public static void HatchlingTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.HatchlingTexture, "Hatchling");
        }

        public static void KnightTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.KnightTexture, "Knight");
        }

        public static void ShieldTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.ShieldTexture, "Shield");
        }
        
        public static void SprintTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.SprintTexture, "Sprint");
        }
        
        public static void UnnTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.UnnTexture, "Unn");
        }
        
        public static void VoidTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.VoidTexture, "Void");
        }
        
        public static void VSTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.VSTexture, "VS");
        }
        
        public static void WeaverTextureUpToDate(byte fromClient, Packet packet)
        { 
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.WeaverTexture, "Weaver");
        }
        
        public static void WraithsTextureUpToDate(byte fromClient, Packet packet)
        {
            HandleTextureUpToDate(fromClient, packet, (int) ServerPackets.WraithsTexture, "Wraiths");
        }
        
        #endregion CustomKnight Integration
        
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
                    }
                    
                    Player iPlayer = Server.clients[i].player;
                    Player fromPlayer = Server.clients[fromClient].player;
                    ServerSend.SpawnPlayer(fromClient, iPlayer);
                    ServerSend.SpawnPlayer(i, fromPlayer);
                    
                    // CustomKnight integration
                    if (ServerSettings.CustomKnightIntegration)
                    {
                        Log("Checking Hashes");
                        ServerSend.CheckHashes(i);
                        ServerSend.CheckHashes(fromClient);
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