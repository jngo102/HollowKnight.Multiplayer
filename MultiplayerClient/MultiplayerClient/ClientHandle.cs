using System;
using System.Net;
using ModCommon;
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
            Log("Reading SpawnPlayer packet");
            int id = packet.ReadInt();
            string username = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Vector3 scale = packet.ReadVector3();
            
            Log("Got Position: " + position);
            Log("Got Scale: " + scale);

            Log($"Spawning instance of Player {id} at {position}.");
            GameManager.Instance.SpawnPlayer(id, username, position, scale);
        }

        public static void DestroyPlayer(Packet packet)
        {
            int clientToDestroy = packet.ReadInt();

            Log("Destroying Player " + clientToDestroy);
            
            GameManager.Instance.Destroy(clientToDestroy);
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
            Log("Reading animation packet...");
            int id = packet.ReadInt();
            string animation = packet.ReadString();
            
            Log($"Playing animation {animation} on Player {id}");
            if (GameManager.Players[id] != null)
            {
                GameManager.Players[id].gameObject.GetComponent<tk2dSpriteAnimator>().Play(animation);
            }
            else
            {
                Log($"Could not play animation {animation} on Player {id}, Player null! ");
            }    
        }

        public static void PlayerDisconnected(Packet packet)
        {
            int id = packet.ReadInt();

            if (Client.Instance.myId == id)
            {
                Log("You have disconnected from the server.");
                Client.Instance.Disconnect();
            }
            else
            {
                Log($"Player {id} has disconnected from the server.");    
            }
            
            Log("Destroying GameManager Player of ID " + id);
            GameManager.Instance.Destroy(id);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client Handle] " + message);
    }
}