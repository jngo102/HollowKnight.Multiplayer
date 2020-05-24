using System.Collections.Generic;
using System.Net;
using HutongGames.PlayMaker.Actions;
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

        private static void HandleTexture(Packet packet, string texName)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            SessionManager.Instance.PlayerTextures[client][texName] = new Texture2D(2, 2);
            SessionManager.Instance.PlayerTextures[client][texName].LoadImage(texBytes);
        }

        public static void BaldurTexture(Packet packet)
        {
            HandleTexture(packet, "Baldur");
        }

        public static void FlukeTexture(Packet packet)
        {
            HandleTexture(packet, "Fluke");
        }

        public static void GrimmTexture(Packet packet)
        {
            HandleTexture(packet, "Grimm");
        }

        public static void HatchlingTexture(Packet packet)
        {
            HandleTexture(packet, "Hatchling");
        }
        
        
        public static void KnightTexture(Packet packet)
        {
            HandleTexture(packet, "Knight");
        }

        public static void ShieldTexture(Packet packet)
        {
            HandleTexture(packet, "Shield");
        }
        
        public static void SprintTexture(Packet packet)
        {
            HandleTexture(packet, "Sprint");
        }
        
        public static void UnnTexture(Packet packet)
        {
            HandleTexture(packet, "Unn");
        }
        
        public static void VoidTexture(Packet packet)
        {
            HandleTexture(packet, "Void");
        }

        public static void VSTexture(Packet packet)
        {
            HandleTexture(packet, "VS");
        }
        
        public static void WeaverTexture(Packet packet)
        {
            HandleTexture(packet, "Weaver");
        }
        
        public static void WraithsTexture(Packet packet)
        {
            HandleTexture(packet, "Wraiths");
        }

        public static void CheckHashes(Packet packet)
        {
            string receivedBaldurHash = packet.ReadString();
            string receivedFlukeHash = packet.ReadString();
            string receivedGrimmHash = packet.ReadString();
            string receivedHatchlingHash = packet.ReadString();
            string receivedKnightHash = packet.ReadString();
            string receivedShieldHash = packet.ReadString();
            string receivedSprintHash = packet.ReadString();
            string receivedUnnHash = packet.ReadString();
            string receivedVoidHash = packet.ReadString();
            string receivedVSHash = packet.ReadString();
            string receivedWeaverHash = packet.ReadString();
            string receivedWraithsHash = packet.ReadString();

            Dictionary<string, string> texHashes = Client.Instance.texHashes;
            Dictionary<string, byte[]> texBytes = Client.Instance.texBytes;
            
            if (texHashes["Baldur"] != receivedBaldurHash)
            {
                ClientSend.SendTexture(texBytes["Baldur"], (int) ClientPackets.BaldurTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.BaldurTextureUpToDate);
            }
            
            if (texHashes["Fluke"] != receivedFlukeHash)
            {
                ClientSend.SendTexture(texBytes["Fluke"], (int) ClientPackets.FlukeTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.FlukeTextureUpToDate);
            }
            
            if (texHashes["Grimm"] != receivedGrimmHash)
            {
                ClientSend.SendTexture(texBytes["Grimm"], (int) ClientPackets.GrimmTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.GrimmTextureUpToDate);
            }
            
            if (texHashes["Hatchling"] != receivedHatchlingHash)
            {
                ClientSend.SendTexture(texBytes["Hatchling"], (int) ClientPackets.HatchlingTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.HatchlingTextureUpToDate);
            }
            
            if (texHashes["Knight"] != receivedKnightHash)
            {
                ClientSend.SendTexture(texBytes["Knight"], (int) ClientPackets.KnightTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.KnightTextureUpToDate);
            }
            
            if (texHashes["Shield"] != receivedShieldHash)
            {
                ClientSend.SendTexture(texBytes["Shield"], (int) ClientPackets.ShieldTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.ShieldTextureUpToDate);
            }
            
            if (texHashes["Sprint"] != receivedSprintHash)
            {
                ClientSend.SendTexture(texBytes["Sprint"], (int) ClientPackets.SprintTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.SprintTextureUpToDate);
            }
            
            if (texHashes["Unn"] != receivedUnnHash)
            {
                ClientSend.SendTexture(texBytes["Unn"], (int) ClientPackets.UnnTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.UnnTextureUpToDate);
            }
            
            if (texHashes["Void"] != receivedVoidHash)
            {
                ClientSend.SendTexture(texBytes["Void"], (int) ClientPackets.VoidTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.VoidTextureUpToDate);
            }
            
            if (texHashes["VS"] != receivedVSHash)
            {
                ClientSend.SendTexture(texBytes["VS"], (int) ClientPackets.VSTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.VSTextureUpToDate);
            }
            
            if (texHashes["Weaver"] != receivedWeaverHash)
            {
                ClientSend.SendTexture(texBytes["Weaver"], (int) ClientPackets.WeaverTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.WeaverTextureUpToDate);
            }
            
            if (texHashes["Wraiths"] != receivedWraithsHash)
            {
                ClientSend.SendTexture(texBytes["Wraiths"], (int) ClientPackets.WraithsTexture);
            }
            else
            {
                ClientSend.SendTextureUpToDate((int) ClientPackets.WraithsTextureUpToDate);
            }
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