using System;
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
        public static void WelcomeReceived()
        {
            using (Packet packet = new Packet((int) ClientPackets.WelcomeReceived))
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
                
                Log("Welcome Received Packet Length: " + packet.Length());

                SendTCPData(packet);
            }
        }

        public static void KnightTexture()
        {
            var sprite = HeroController.instance.GetComponent<tk2dSprite>();
            var anim = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
            Texture2D knightTex = sprite.GetCurrentSpriteDef().material.mainTexture as Texture2D;
            byte[] knightTexBytes = knightTex.DuplicateTexture().EncodeToPNG();
            int length = 4093;
            byte[] fragment = new byte[length];
            short order = 0;
            for (int i = 0; i < knightTexBytes.Length; i += length)
            {
                if (knightTexBytes.Length - i < length)
                {
                    length = knightTexBytes.Length - i;
                }
                
                Array.Copy(knightTexBytes, i, fragment, 0,  length);
                using (Packet packet = new Packet((int) ClientPackets.KnightTexture))
                {
                    packet.Write(order);
                    packet.Write(fragment);

                    order++;
                    
                    Log("Sending Tex Data from Client to Server: " + packet.Length());

                    SendTCPData(packet);
                }
            }
            
            Log("Sending Finish Sending Texture Bytes");
            FinishedSendingTexBytes();
        }

        public static void FinishedSendingTexBytes(bool finishedSending = true)
        {
            using (Packet packet = new Packet((int) ClientPackets.FinishedSendingTexBytes))
            {
                packet.Write(finishedSending);

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

                Log("Sending Health Data to Server");
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

                Log("Packet Length: " + packet.Length());
                Log("Sending CharmsUpdated Packet from Client");
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