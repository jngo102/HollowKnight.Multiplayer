using System;

namespace MultiplayerServer
{
    public class ServerSend
    {
        private static void SendTCPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        private static void SendUDPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].udp.SendData(packet);
        }
        
        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].player != null)
                {
                    Server.clients[i].tcp.SendData(packet);   
                }
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    if (Server.clients[i].player != null)
                    {
                        Server.clients[i].tcp.SendData(packet);   
                    }
                }
            }
        }
        
        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].player != null)
                {
                    Server.clients[i].udp.SendData(packet);
                }
            }
        }

        private static void SendUDPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    if (Server.clients[i].player != null)
                    {
                        Server.clients[i].udp.SendData(packet);   
                    }
                }
            }
        }
        
        public static void Welcome(int toClient, string msg)
        {
            using (Packet packet = new Packet((int) ServerPackets.Welcome))
            {
                packet.Write(toClient);
                packet.Write(msg);

                SendTCPData(toClient, packet);
            }
        }

        public static void SpawnPlayer(int toClient, Player player)
        {
            using (Packet packet = new Packet((int) ServerPackets.SpawnPlayer))
            {
                packet.Write(player.id);
                packet.Write(player.username);
                packet.Write(player.position);
                packet.Write(player.scale);
                
                SendTCPData(toClient, packet);
            }
        }

        public static void DestroyPlayer(int toClient, int clientToDestroy)
        {
            using (Packet packet = new Packet((int) ServerPackets.DestroyPlayer))
            {
                packet.Write(clientToDestroy);

                SendTCPData(toClient, packet);
            }
        }
        
        public static void PlayerPosition(Player player)
        {
            using (Packet packet = new Packet((int) ServerPackets.PlayerPosition))
            {
                packet.Write(player.id);
                packet.Write(player.position);

                SendUDPDataToAll(player.id, packet);
            }
        }

        public static void PlayerScale(Player player)
        {
            using (Packet packet = new Packet((int) ServerPackets.PlayerScale))
            {
                packet.Write(player.id);
                packet.Write(player.scale);

                SendUDPDataToAll(player.id, packet);
            }
        }

        public static void PlayerAnimation(Player player)
        {
            using (Packet packet = new Packet((int) ServerPackets.PlayerAnimation))
            {
                packet.Write(player.id);
                packet.Write(player.animation);
                
                SendUDPDataToAll(player.id, packet);
            }
        }

        public static void PlayerDisconnected(int playerId)
        {
            using (Packet packet = new Packet((int) ServerPackets.PlayerDisconnected))
            {
                packet.Write(playerId);

                SendTCPDataToAll(packet);    
            }
        }

        private static void Log(object message) => Modding.Logger.Log("[Server Send] " + message);
    }
}