using System;
using System.Collections.Generic;
using System.Net;
using ModCommon;
using ModCommon.Util;
using UnityEngine;

namespace MultiplayerClient
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet packet)
        {
            byte myId = packet.ReadByte();
            string msg = packet.ReadString();

            Log($"Message from server: {msg}");
            Client.Instance.myId = myId;

            ClientSend.WelcomeReceived();
            ClientSend.KnightTexture();
            
            Client.Instance.udp.Connect(((IPEndPoint) Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void SpawnPlayer(Packet packet)
        {
            if (Client.Instance.isConnected)
            {
                byte id = packet.ReadByte();
                string username = packet.ReadString();
                Vector3 position = packet.ReadVector3();
                Vector3 scale = packet.ReadVector3();
                string animation = packet.ReadString();
                List<bool> charmsData = new List<bool>();
                for (int charmNum = 1; charmNum <= 40; charmNum++)
                {
                    charmsData.Add(packet.ReadBool());
                }

                bool pvpEnabled = packet.ReadBool();
                SessionManager.Instance.EnablePvP(pvpEnabled);
                
                SessionManager.Instance.SpawnPlayer(id, username, position, scale, animation, charmsData);
            }
        }

        public static void KnightTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int order = packet.ReadInt();
            byte[] knightTexBytes = packet.ReadBytes(4093);
            Log("Received Tex from Server: " + knightTexBytes.Length);
            
            SessionManager.Instance.Players[client].texIndexedByteDict[order] = knightTexBytes;
        }

        public static void FinishedSendingTexBytes(Packet packet)
        {
            byte client = packet.ReadByte();
            bool finishedSending = packet.ReadBool();
            Log("Received Finished from Server: " + finishedSending);
            
            if (finishedSending)
            {
                SessionManager.Instance.CompileByteFragments(client);
            }
        }
        
        public static void DestroyPlayer(Packet packet)
        {
            byte clientToDestroy = packet.ReadByte();

            SessionManager.Instance.DestroyPlayer(clientToDestroy);
        }

        public static void PvPEnabled(Packet packet)
        {
            bool enablePvP = packet.ReadBool();

            SessionManager.Instance.EnablePvP(enablePvP);
        }
            
        public static void PlayerPosition(Packet packet)
        {
            byte id = packet.ReadByte();
            Vector3 position = packet.ReadVector3();

            if (SessionManager.Instance.Players.ContainsKey(id))
            {
                SessionManager.Instance.Players[id].gameObject.transform.position = position;
            }
        }

        public static void PlayerScale(Packet packet)
        {
            byte id = packet.ReadByte();
            Vector3 scale = packet.ReadVector3();

            if (SessionManager.Instance.Players.ContainsKey(id))
            {
                SessionManager.Instance.Players[id].gameObject.transform.localScale = scale;
            }
        }
        
        public static void PlayerAnimation(Packet packet)
        {
            byte id = packet.ReadByte();
            string animation = packet.ReadString();

            if (SessionManager.Instance.Players.ContainsKey(id))
            {
                var anim = SessionManager.Instance.Players[id].gameObject.GetComponent<tk2dSpriteAnimator>();
                anim.Stop();
                anim.Play(animation);

                SessionManager.Instance.StartCoroutine(MPClient.Instance.PlayAnimation(id, animation));
            }
        }

        public static void HealthUpdated(Packet packet)
        {
            byte fromClient = packet.ReadByte();
            int health = packet.ReadInt();
            int maxHealth = packet.ReadInt();
            int healthBlue = packet.ReadInt();

            Log("Health Data from Server: " + health + " " + maxHealth + " " + healthBlue);
            
            SessionManager.Instance.Players[fromClient].health = health;
            SessionManager.Instance.Players[fromClient].maxHealth = maxHealth;
            SessionManager.Instance.Players[fromClient].healthBlue = healthBlue;
        }
        
        public static void CharmsUpdated(Packet packet)
        {
            byte fromClient = packet.ReadByte();
            for (int charmNum = 1; charmNum <= 40; charmNum++)
            {
                bool equippedCharm = packet.ReadBool();
                SessionManager.Instance.Players[fromClient].SetAttr("equippedCharm_" + charmNum, equippedCharm);
            }
            Log("Finished Modifying equippedCharm bools");
        }
        
        public static void PlayerDisconnected(Packet packet)
        {
            byte id = packet.ReadByte();
            Log($"Player {id} has disconnected from the server.");
    
            SessionManager.Instance.DestroyPlayer(id);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client Handle] " + message);
    }
}