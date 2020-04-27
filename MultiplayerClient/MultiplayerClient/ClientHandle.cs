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
            int myId = packet.ReadInt();
            string msg = packet.ReadString();

            Log($"Message from server: {msg}");
            Client.Instance.myId = myId;

            ClientSend.WelcomeReceived();
            
            Client.Instance.udp.Connect(((IPEndPoint) Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void SpawnPlayer(Packet packet)
        {
            if (Client.Instance.isConnected)
            {
                int id = packet.ReadInt();
                string username = packet.ReadString();
                Vector3 position = packet.ReadVector3();
                Vector3 scale = packet.ReadVector3();
                string animation = packet.ReadString();
                List<bool> charmsData = new List<bool>();
                for (int charmNum = 1; charmNum <= 40; charmNum++)
                {
                    charmsData.Add(packet.ReadBool());
                }
                
                GameManager.Instance.SpawnPlayer(id, username, position, scale, animation, charmsData);
            }
        }

        public static void DestroyPlayer(Packet packet)
        {
            int clientToDestroy = packet.ReadInt();

            GameManager.Instance.DestroyPlayer(clientToDestroy);
        }

        public static void PvPEnabled(Packet packet)
        {
            bool enablePvP = packet.ReadBool();

            Log("Enabling PvP on Client Side");

            GameManager.Instance.EnablePvP(enablePvP);
        }
            
        public static void PlayerPosition(Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            if (GameManager.Instance.Players.ContainsKey(id))
            {
                GameManager.Instance.Players[id].gameObject.transform.position = position;
            }
        }

        public static void PlayerScale(Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 scale = packet.ReadVector3();

            if (GameManager.Instance.Players.ContainsKey(id))
            {
                GameManager.Instance.Players[id].gameObject.transform.localScale = scale;
            }
        }
        
        public static void PlayerAnimation(Packet packet)
        {
            int id = packet.ReadInt();
            string animation = packet.ReadString();

            if (GameManager.Instance.Players.ContainsKey(id))
            {
                var anim = GameManager.Instance.Players[id].gameObject.GetComponent<tk2dSpriteAnimator>();
                anim.Stop();
                anim.Play(animation);

                GameManager.Instance.StartCoroutine(MPClient.Instance.PlayAnimation(id, animation));
            }
        }

        public static void CharmsUpdated(Packet packet)
        {
            int fromClient = packet.ReadInt();
            for (int charmNum = 1; charmNum <= 40; charmNum++)
            {
                bool equippedCharm = packet.ReadBool();
                GameManager.Instance.Players[fromClient].SetAttr("equippedCharm_" + charmNum, equippedCharm);
            }
            Log("Finished Modifying equippedCharm bools");
        }
        
        public static void PlayerDisconnected(Packet packet)
        {
            int id = packet.ReadInt();
            Log($"Player {id} has disconnected from the server.");
    
            GameManager.Instance.DestroyPlayer(id);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client Handle] " + message);
    }
}