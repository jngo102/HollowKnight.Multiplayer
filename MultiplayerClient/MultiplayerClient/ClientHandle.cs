using System;
using System.Collections.Generic;
using System.Net;
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

                bool pvpEnabled = packet.ReadBool();

                Log($"Spawning Player {username} at {position}");
                GameManager.Instance.SpawnPlayer(id, username, position, scale, animation, charmsData, pvpEnabled);
            }
        }

        public static void DestroyPlayer(Packet packet)
        {
            int clientToDestroy = packet.ReadInt();

            GameManager.Instance.Destroy(clientToDestroy);
        }

        public static void PvPEnabled(Packet packet)
        {
            bool enabled = packet.ReadBool();

            Log("Enabling PvP on Client Side");
            
            GameManager.Instance.pvpEnabled = enabled;
            for (int playerNum = 0; playerNum <= GameManager.Players.Count; playerNum++)
            {
                GameObject player = GameManager.Players[playerNum].gameObject;
                player.layer = enabled ? 11 : 9;

                if (enabled)
                {
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
                else
                {
                    if (player.GetComponent<BoxCollider2D>())
                    {
                        Destroy(player.GetComponent<BoxCollider2D>());    
                    }

                    if (player.GetComponent<DamageHero>())
                    {
                        Destroy(player.GetComponent<DamageHero>());
                    }
                }
            }
        }
            
        public static void PlayerPosition(Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            GameManager.Players[id].gameObject.transform.position = position;
        }

        public static void PlayerScale(Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 scale = packet.ReadVector3();

            GameManager.Players[id].gameObject.transform.localScale = scale;
        }
        
        public static void PlayerAnimation(Packet packet)
        {
            int id = packet.ReadInt();
            string animation = packet.ReadString();
            
            var anim = GameManager.Players[id].gameObject.GetComponent<tk2dSpriteAnimator>();
            anim.Stop();
            Log($"Playing {animation} on Player {id}");
            anim.Play(animation);
            
            GameManager.Instance.StartCoroutine(MPClient.Instance.PlayAnimation(id, animation));
        }

        public static void CharmsUpdated(Packet packet)
        {
            int fromClient = packet.ReadInt();
            for (int charmNum = 1; charmNum <= 40; charmNum++)
            {
                bool equippedCharm = packet.ReadBool();

                try
                {
                    Log($"Setting equippedCharm_{charmNum} of client {fromClient} to {equippedCharm}");
                    GameManager.Players[fromClient].SetAttr("equippedCharm_" + charmNum, equippedCharm);
                }
                catch (Exception e)
                {
                    Log(e);
                }
            }
            Log("Finished Modifying equippedCharm bools");
        }
        
        public static void PlayerDisconnected(Packet packet)
        {
            int id = packet.ReadInt();
            Log($"Player {id} has disconnected from the server.");
    
            GameManager.Instance.Destroy(id);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client Handle] " + message);
    }
}