using UnityEngine;

namespace MultiplayerServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Vector3 scale = packet.ReadVector3();

            Log($"{username} connected successfully and is now player {fromClient}.");
            if (fromClient != clientIdCheck)
            {
                Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck}.");
            }

            Server.clients[fromClient].SendIntoGame(username, position, scale);
        }

        public static void SpawnPlayer(int fromClient, Packet packet)
        {
            int sourceId = packet.ReadInt();
            int targetId = packet.ReadInt();
            string username = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Vector3 scale = packet.ReadVector3();

            Player player = NetworkManager.Instance.InstantiatePlayer(position, scale);
            player.Initialize(sourceId, username);

            ServerSend.SpawnPlayer(targetId, player);
        }
        
        public static void PlayerPosition(int fromClient, Packet packet)
        {
            Vector3 position = packet.ReadVector3();
            
            Server.clients[fromClient].player.SetPosition(position);
        }

        public static void PlayerScale(int fromClient, Packet packet)
        {
            Vector3 scale = packet.ReadVector3();

            Server.clients[fromClient].player.SetScale(scale);
        }
        
        public static void PlayerAnimation(int fromClient, Packet packet)
        { 
            string animation = packet.ReadString();
            
            Log($"Server.clients[fromClient].player.SetAnimation({animation})");
            Server.clients[fromClient].player.SetAnimation(animation);
        }

        public static void SceneChanged(int fromClient, Packet packet)
        {
            string sceneName = packet.ReadString();

            ServerSend.CheckSameScene(fromClient, sceneName);
        }
        
        public static void PlayerDisconnected(int fromClient, Packet packet)
        {
            Object.Destroy(Server.clients[fromClient].player.gameObject);
            Server.clients[fromClient].player = null;
            Server.clients[fromClient].tcp.Disconnect();
            Server.clients[fromClient].udp.Disconnect();
            
            ServerSend.PlayerDisconnected(fromClient);
        }

        private static void Log(object message) => Modding.Logger.Log("[Server Handle] " + message);
    }
}