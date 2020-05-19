using System;
using HutongGames.PlayMaker.Actions;
using ModCommon.Util;

namespace MultiplayerServer
{
    public class ServerSend
    {
        private static void SendTCPData(byte toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        private static void SendUDPData(byte toClient, Packet packet)
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

        private static void SendTCPDataToAll(byte exceptClient, Packet packet)
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

        private static void SendUDPDataToAll(byte exceptClient, Packet packet)
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
        
        public static void Welcome(byte toClient, string msg)
        {
            using (Packet packet = new Packet((int) ServerPackets.Welcome))
            {
                packet.Write(toClient);
                packet.Write(msg);
                
                SendTCPData(toClient, packet);
            }
        }

        public static void SpawnPlayer(byte toClient, Player player)
        {
            using (Packet packet = new Packet((int) ServerPackets.SpawnPlayer))
            {
                packet.Write(player.id);
                packet.Write(player.username);
                packet.Write(player.position);
                packet.Write(player.scale);
                packet.Write(player.animation);
                for (int charmNum = 1; charmNum <= 40; charmNum++)
                {
                    packet.Write(player.GetAttr<Player, bool>("equippedCharm_" + charmNum));
                }
                packet.Write(ServerSettings.PvPEnabled);

                Log($"Spawning Player {player.id} on Client {toClient} with Charms");
                SendTCPData(toClient, packet);
            }
        }

        #region CustomKnight Integration
        
        public static void BaldurTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.BaldurTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void FlukeTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.FlukeTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void GrimmTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.GrimmTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void HatchlingTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.HatchlingTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void KnightTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.KnightTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void ShieldTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.ShieldTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void SprintTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.SprintTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void UnnTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.UnnTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void VoidTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.VoidTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void VSTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.VSTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void WeaverTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.WeaverTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void WraithsTexture(byte fromClient, byte[] texBytes)
        {
            using (Packet packet = new Packet((byte) ServerPackets.WraithsTexture))
            {
                packet.Write(fromClient);
                packet.Write(texBytes.Length);
                packet.Write(texBytes);

                Log("Sending Tex from Server to Clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void RequestTextures(
            byte toClient, 
            int knightTexHash, 
            int sprintTexHash,
            int unnTexHash,
            int voidTexHash,
            int vsTexHash,
            int wraithsTexHash
        )
        {
            using (Packet packet = new Packet((int) ServerPackets.RequestTextures))
            {
                packet.Write(knightTexHash);
                packet.Write(sprintTexHash);
                packet.Write(unnTexHash);
                packet.Write(voidTexHash);
                packet.Write(vsTexHash);
                packet.Write(wraithsTexHash);
                
                SendTCPData(toClient, packet);
            }
        }
        
        #endregion CustomKnight Integration
        
        public static void DestroyPlayer(byte toClient, int clientToDestroy)
        {
            using (Packet packet = new Packet((int) ServerPackets.DestroyPlayer))
            {
                packet.Write(clientToDestroy);

                SendTCPData(toClient, packet);
            }
        }

        public static void PvPEnabled()
        {
            using (Packet packet = new Packet((int) ServerPackets.PvPEnabled))
            {
                packet.Write(ServerSettings.PvPEnabled);
                
                SendTCPDataToAll(packet);
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

        public static void HealthUpdated(byte fromClient, int health, int maxHealth, int healthBlue)
        {
            using (Packet packet = new Packet((int) ServerPackets.HealthUpdated))
            {
                packet.Write(fromClient);
                packet.Write(health);
                packet.Write(maxHealth);
                packet.Write(healthBlue);

                Log("Sending Health Data to all clients except " + fromClient);
                SendTCPDataToAll(fromClient, packet);
            }
        }
        
        public static void CharmsUpdated(byte fromClient, Player player)
        {
            using (Packet packet = new Packet((int) ServerPackets.CharmsUpdated))
            {
                packet.Write(fromClient);
                for (int charmNum = 1; charmNum <= 40; charmNum++)
                {
                    packet.Write(player.GetAttr<Player, bool>("equippedCharm_" + charmNum));
                }
                
                SendTCPDataToAll(fromClient, packet);
            }
        }

        public static void PlayerDisconnected(byte playerId)
        {
            using (Packet packet = new Packet((int) ServerPackets.PlayerDisconnected))
            {
                packet.Write(playerId);

                Log("Sending Disconnect Packet to all clients but " + playerId);
                SendTCPDataToAll(playerId, packet); 
            }
        }

        private static void Log(object message) => Modding.Logger.Log("[Server Send] " + message);
    }
}