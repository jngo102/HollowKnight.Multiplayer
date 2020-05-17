using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ModCommon;
using ModCommon.Util;
using UnityEngine;

namespace MultiplayerClient
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet packet)
        {
            byte myId = packet.ReadByte();
            string msg = packet.ReadString();

            Log($"Message from server: {msg}");
            Client.Instance.myId = myId;

            ClientSend.WelcomeReceived();
            ClientSend.KnightTexture();
            ClientSend.VoidTexture();
            //ClientSend.VSTexture();
            
            Client.Instance.udp.Connect(((IPEndPoint) Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void SpawnPlayer(Packet packet)
        {
            if (Client.Instance.isConnected)
            {
                byte id = packet.ReadByte();
                string username = packet.ReadString();
                Vector3 position = packet.ReadVector3();
                Vector3 scale = packet.ReadVector3();
                string animation = packet.ReadString();
                List<bool> charmsData = new List<bool>();
                for (int charmNum = 1; charmNum <= 40; charmNum++)
                {
                    charmsData.Add(packet.ReadBool());
                }

                bool pvpEnabled = packet.ReadBool();
                SessionManager.Instance.EnablePvP(pvpEnabled);
                
                SessionManager.Instance.SpawnPlayer(id, username, position, scale, animation, charmsData);
            }
        }

        #region CustomKnight Integration
        
        public static void BaldurTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasBaldur";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.baldurTexture = tex;
        }
        
        public static void FlukeTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasFluke";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.flukeTexture = tex;
        }
        
        public static void GrimmTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasGrimm";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.grimmTexture = tex;
        }
        
        public static void HatchlingTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasHatchling";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.hatchlingTexture = tex;
        }
        
        public static void KnightTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasKnight";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];
            GameObject player = playerManager.gameObject;

            var materialPropertyBlock = new MaterialPropertyBlock();
            player.GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetTexture("_MainTex", tex);
            player.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);

            playerManager.knightTexture = tex;
        }
        
        public static void ShieldTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasShield";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.shieldTexture = tex;
        }
        
        public static void SprintTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasSprint";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.sprintTexture = tex;
        }
        
        public static void UnnTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasUnn";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.unnTexture = tex;
        }
        
        public static void VoidTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasVoid";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.voidTexture = tex;
        }

        public static void VSTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasVS";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.vsTexture = tex;
        }
        
        public static void WeaverTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasWeaver";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.weaverTexture = tex;
        }
        
        public static void WraithsTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasWraiths";
            
            PlayerManager playerManager = SessionManager.Instance.Players[client];

            playerManager.wraithsTexture = tex;
        }
        
        #endregion CustomKnight Integration
        
        public static void DestroyPlayer(Packet packet)
        {
            byte clientToDestroy = packet.ReadByte();

            SessionManager.Instance.DestroyPlayer(clientToDestroy);
        }

        public static void PvPEnabled(Packet packet)
        {
            bool enablePvP = packet.ReadBool();

            SessionManager.Instance.EnablePvP(enablePvP);
        }
            
        public static void PlayerPosition(Packet packet)
        {
            byte id = packet.ReadByte();
            Vector3 position = packet.ReadVector3();

            if (SessionManager.Instance.Players.ContainsKey(id))
            {
                SessionManager.Instance.Players[id].gameObject.transform.position = position;
            }
        }

        public static void PlayerScale(Packet packet)
        {
            byte id = packet.ReadByte();
            Vector3 scale = packet.ReadVector3();

            if (SessionManager.Instance.Players.ContainsKey(id))
            {
                SessionManager.Instance.Players[id].gameObject.transform.localScale = scale;
            }
        }
        
        public static void PlayerAnimation(Packet packet)
        {
            byte id = packet.ReadByte();
            string animation = packet.ReadString();

            if (SessionManager.Instance.Players.ContainsKey(id))
            {
                var anim = SessionManager.Instance.Players[id].gameObject.GetComponent<tk2dSpriteAnimator>();
                anim.Stop();
                anim.Play(animation);

                SessionManager.Instance.StartCoroutine(MPClient.Instance.PlayAnimation(id, animation));
            }
        }

        public static void HealthUpdated(Packet packet)
        {
            byte fromClient = packet.ReadByte();
            int health = packet.ReadInt();
            int maxHealth = packet.ReadInt();
            int healthBlue = packet.ReadInt();

            Log("Health Data from Server: " + health + " " + maxHealth + " " + healthBlue);
            
            SessionManager.Instance.Players[fromClient].health = health;
            SessionManager.Instance.Players[fromClient].maxHealth = maxHealth;
            SessionManager.Instance.Players[fromClient].healthBlue = healthBlue;
        }
        
        public static void CharmsUpdated(Packet packet)
        {
            byte fromClient = packet.ReadByte();
            for (int charmNum = 1; charmNum <= 40; charmNum++)
            {
                bool equippedCharm = packet.ReadBool();
                SessionManager.Instance.Players[fromClient].SetAttr("equippedCharm_" + charmNum, equippedCharm);
            }
            Log("Finished Modifying equippedCharm bools");
        }
        
        public static void PlayerDisconnected(Packet packet)
        {
            byte id = packet.ReadByte();
            Log($"Player {id} has disconnected from the server.");
    
            SessionManager.Instance.DestroyPlayer(id);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client Handle] " + message);
    }
}