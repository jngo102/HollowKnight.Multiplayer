using System;
using UnityEngine;

namespace MultiplayerServer
{
    public class Player : MonoBehaviour
    {
        public int id;
        public string username;
        public string animation;
        public string activeScene;
        public Vector3 position;
        public Vector3 scale;

        public void Initialize(int id, string username)
        {
            this.id = id;
            this.username = username;
            animation = "Exit";
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;

            ServerSend.PlayerPosition(this);
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;

            ServerSend.PlayerScale(this);
        }
        
        public void SetAnimation(string animation)
        {
            this.animation = animation;

            ServerSend.PlayerAnimation(this);
        }

        private void Log(object message) => Modding.Logger.Log("[Player] " + message);
    }
}