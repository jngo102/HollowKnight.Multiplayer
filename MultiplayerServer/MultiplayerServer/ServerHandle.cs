using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();
            string activeScene = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Vector3 scale = packet.ReadVector3();
            
            Log("Received Position: " + position);
            Log("Received Scale: " + scale);

            Server.clients[fromClient].SendIntoGame(username, position, scale);
            SceneChanged(fromClient, activeScene);
            
            Log($"{username} connected successfully and is now player {fromClient}.");
            if (fromClient != clientIdCheck)
            {
                Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck}.");
            }
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
            Log("Reading Animation Packet...");
            string animation = packet.ReadString();
            
            Server.clients[fromClient].player.SetAnimation(animation);
        }

        public static void SceneChanged(int fromClient, Packet packet)
        {
            string sceneName = packet.ReadString();
            
            Server.clients[fromClient].player.activeScene = sceneName;

            for (int i = 1; i <= Server.MaxPlayers; i++)    
            {    
                if (Server.clients[i].player != null && i != fromClient)
                {
                    
                }
            }
            
            for (int i = 1; i <= Server.MaxPlayers; i++)    
            {    
                if (Server.clients[i].player != null && i != fromClient)
                {
                    if (Server.clients[i].player.activeScene == sceneName)
                    {
                        Log($"Spawning Player {i} on client {fromClient}");
                        ServerSend.SpawnPlayer(fromClient, Server.clients[i].player);
                        Log($"Spawning Player {fromClient} on client {i}");
                        ServerSend.SpawnPlayer(i, Server.clients[fromClient].player);
                    }
                    else
                    {
                        Log($"Destroying Player {i} on client {fromClient}");
                        ServerSend.DestroyPlayer(fromClient, i);
                        Log($"Destroying Player {fromClient} on client {i}");
                        ServerSend.DestroyPlayer(i, fromClient);
                    }
                }
            }
        }
        
        public static void SceneChanged(int fromClient, string sceneName)
        {
            Server.clients[fromClient].player.activeScene = sceneName;

            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].player != null && i != fromClient)
                {
                    if (Server.clients[i].player.activeScene == sceneName)
                    {
                        Log("Same Scene: Spawning Players");
                        ServerSend.SpawnPlayer(fromClient, Server.clients[i].player);
                        
                        ServerSend.SpawnPlayer(Server.clients[i].player.id, Server.clients[fromClient].player);
                    }
                }
            }
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