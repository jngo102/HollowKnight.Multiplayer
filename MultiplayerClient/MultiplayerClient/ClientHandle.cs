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
        }

        public static void SpawnPlayer(Packet packet)
        {
            Log("Reading SpawnPlayer packet");
            int id = packet.ReadInt();
            string username = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Vector3 scale = packet.ReadVector3();

            GameManager.Instance.SpawnPlayer(id, username, position, scale);
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
            
            Log($"Playing animation {animation} on Player {id}...");
            GameManager.Players[id].gameObject.GetComponent<tk2dSpriteAnimator>().Play(animation);
        }

        public static void CheckSameScene(Packet packet)
        {
            int id = packet.ReadInt();
            string sceneName = packet.ReadString();

            Transform heroTransform = HeroController.instance.gameObject.transform;
            
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName)
            {
                ClientSend.SpawnPlayer(Client.Instance.myId, id, Client.Instance.username, heroTransform.position, heroTransform.localScale);
            }
        }
        
        public static void PlayerDisconnected(Packet packet)
        {
            int id = packet.ReadInt();

            if (Client.Instance.myId == id)
            {
                Log("You have disconnected from the server.");
            }
            else
            {
                Log($"Player {id} has disconnected from the server.");    
            }
            
            Log("Destroying GameManager Player of ID " + id);
            Destroy(GameManager.Players[id].gameObject);
            Client.Instance.Disconnect();
            GameManager.Players.Remove(id);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client Handle] " + message);
    }
}