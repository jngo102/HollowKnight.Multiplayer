using System;
using System.Collections.Generic;
using System.Security.Policy;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using MultiplayerClient.Canvas;
using UnityEngine;

namespace MultiplayerClient
{
    public class ClientSend : MonoBehaviour
    {
        /// <summary>Sends a packet to the server via TCP.</summary>
        /// <param name="packet">The packet to send to the sever.</param>
        private static void SendTCPData(Packet packet)
        {
            packet.WriteLength();
            Client.Instance.tcp.SendData(packet);
        }

        /// <summary>Sends a packet to the server via UDP.</summary>
        /// <param name="packet">The packet to send to the sever.</param>
        private static void SendUDPData(Packet packet)
        {
            packet.WriteLength();
            Client.Instance.udp.SendData(packet);
        }

        #region Packets

        /// <summary>Lets the server know that the welcome message was received.</summary>
        public static void WelcomeReceived(List<byte[]> textureHashes)
        {
            using (Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
            {
                Transform heroTransform = HeroController.instance.gameObject.transform;

                packet.Write(Client.Instance.myId);
                packet.Write(MultiplayerClient.settings.username);
                packet.Write(HeroController.instance.GetComponent<tk2dSpriteAnimator>().CurrentClip.name);
                packet.Write(PlayerManager.activeScene);
                packet.Write(heroTransform.position);
                packet.Write(heroTransform.localScale);
                packet.Write(PlayerData.instance.health);
                packet.Write(PlayerData.instance.maxHealth);
                packet.Write(PlayerData.instance.healthBlue);

                for (int charmNum = 1; charmNum <= 40; charmNum++)
                {
                    packet.Write(PlayerData.instance.GetAttr<PlayerData, bool>("equippedCharm_" + charmNum));
                }

                foreach (var hash in textureHashes)
                {
                    packet.Write(hash);
                }

                Log("Welcome Received Packet Length: " + packet.Length());

                SendTCPData(packet);
            }
        }

        public static void RequestTexture(byte[] hash)
        {
            using (Packet packet = new Packet((int)ClientPackets.TextureRequest))
            {
                packet.Write(hash);
                SendTCPData(packet);
            }
        }

        public static void SendTexture(byte[] texture)
        {
            using (Packet packet = new Packet((int) ClientPackets.TextureFragment))
            {
                // It's really that easy
                packet.Write(texture.Length);
                packet.Write(texture);
                SendTCPData(packet);
            }
        }
        
        public static void PlayerPosition(Vector3 position)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerPosition))
            {
                packet.Write(position);
                
                SendUDPData(packet);
            }
        }
        
        public static void PlayerScale(Vector3 scale)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerScale))
            {
                packet.Write(scale);
                
                SendUDPData(packet);
            }
        }

        public static void PlayerAnimation(string animation)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerAnimation))
            {
                packet.Write(animation);
                
                SendUDPData(packet);
            }
        }

        public static void SceneChanged(string sceneName)
        {
            using (Packet packet = new Packet((int) ClientPackets.SceneChanged))
            {
                packet.Write(sceneName);
                
                SendTCPData(packet);
            }
        }

        public static void HealthUpdated(int currentHealth, int currentMaxHealth, int currentHealthBlue)
        {
            using (Packet packet = new Packet((int) ClientPackets.HealthUpdated))
            {
                packet.Write(currentHealth);
                packet.Write(currentMaxHealth);
                packet.Write(currentHealthBlue);

                SendTCPData(packet);
            }
        }
        
        public static void CharmsUpdated(PlayerData pd)
        {
            using (Packet packet = new Packet((int) ClientPackets.CharmsUpdated))
            {
                for (int i = 1; i <= 40; i++)
                {
                    packet.Write(pd.GetBool("equippedCharm_" + i));
                }

                SendTCPData(packet);
            }
        }
        
        public static void PlayerDisconnected(int id)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerDisconnected))
            {
                packet.Write(id);

                SendTCPData(packet);
            }
        }
        
        #endregion

        private static void Log(object message) => Modding.Logger.Log("[Client Send] " + message);
    }
}
