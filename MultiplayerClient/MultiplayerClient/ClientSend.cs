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
            Log("Client.Instance.udp.SendData(packet)");
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
                packet.Write(Client.Instance.username);
                packet.Write(heroTransform.position);
                packet.Write(heroTransform.localScale);

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
                
                Log("PlayerAnimation.SendUDPData(packet)");
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

        public static void SpawnPlayer(int sourceId, int targetId, string username, Vector3 position, Vector3 scale)
        {
            using (Packet packet = new Packet((int) ClientPackets.SpawnPlayer))
            {
                packet.Write(sourceId);
                packet.Write(targetId);
                packet.Write(username);
                packet.Write(position);
                packet.Write(scale);

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