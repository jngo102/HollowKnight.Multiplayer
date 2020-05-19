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

            SessionManager.Instance.PlayerTextures[client]["Baldur"] = tex;
        }
        
        public static void FlukeTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasFluke";
            
            SessionManager.Instance.PlayerTextures[client]["Fluke"] = tex;
        }
        
        public static void GrimmTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasGrimm";
            
            SessionManager.Instance.PlayerTextures[client]["Grimm"] = tex;
        }
        
        public static void HatchlingTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasHatchling";
            
            SessionManager.Instance.PlayerTextures[client]["Hatchling"] = tex;
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

            SessionManager.Instance.PlayerTextures[client]["Knight"] = tex;
        }
        
        public static void ShieldTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasShield";
            
            SessionManager.Instance.PlayerTextures[client]["Shield"] = tex;
        }
        
        public static void SprintTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasSprint";
            
            SessionManager.Instance.PlayerTextures[client]["Sprint"] = tex;
        }
        
        public static void UnnTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasUnn";
            
            SessionManager.Instance.PlayerTextures[client]["Unn"] = tex;
        }
        
        public static void VoidTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasVoid";
            
            SessionManager.Instance.PlayerTextures[client]["Void"] = tex;
        }

        public static void VSTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasVS";
            
            SessionManager.Instance.PlayerTextures[client]["VS"] = tex;
        }
        
        public static void WeaverTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasWeaver";
            
            SessionManager.Instance.PlayerTextures[client]["Weaver"] = tex;
        }
        
        public static void WraithsTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            int byteLength = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(byteLength);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(texBytes);
            tex.name = "atlasWraiths";
            
            SessionManager.Instance.PlayerTextures[client]["Wraiths"] = tex;
        }
        
        public static void RequestTextures(Packet packet)
        {
            GameObject hc = HeroController.instance.gameObject;
            
            int receivedKnightTexHash = packet.ReadInt();
            int receivedSprintTexHash = packet.ReadInt();
            int receivedUnnTexHash = packet.ReadInt();
            int receivedVoidTexHash = packet.ReadInt();
            int receivedVSTexHash = packet.ReadInt();
            int receivedWraithsTexHash = packet.ReadInt();
            
            Texture2D knightTex = hc.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            var anim = hc.GetComponent<tk2dSpriteAnimator>();
            Texture2D sprintTex = anim.GetClipByName("Sprint").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D unnTex = anim.GetClipByName("Slug Up").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            int knightTexHash = knightTex.GetHashCode();
            int sprintTexHash = sprintTex.GetHashCode();
            int unnTexHash = unnTex.GetHashCode();
            int voidTexHash = 0;
            int vsTexHash = 0;
            int wraithsTexHash = 0;
            foreach (Transform child in hc.transform)
            {
                if (child.name == "Spells")
                {
                    foreach (Transform spellsChild in child)
                    {
                        if (spellsChild.name == "Scr Heads")
                        {
                            Texture2D wraithsTex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            wraithsTexHash = wraithsTex.GetHashCode();
                        }
                        else if (spellsChild.name == "Scr Heads 2")
                        {
                            Texture2D voidTex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            voidTexHash = voidTex.GetHashCode();
                        }
                    }
                }
                else if (child.name == "Focus Effects")
                {
                    foreach (Transform focusChild in child)
                    {
                        if (focusChild.name == "Heal Anim")
                        {
                            Texture2D vsTex = focusChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            vsTexHash = vsTex.GetHashCode();
                            break;
                        }
                    }
                }
            }
            
            Log("Received Knight Tex Hash: " + receivedKnightTexHash);
            Log("Knight Tex Hash: " + knightTexHash);
            
            Log("Received Void Tex Hash: " + receivedVoidTexHash);
            Log("Void Tex Hash: " + voidTexHash);
            
            if (knightTexHash != receivedKnightTexHash)
            {
                Log("Sending updated Knight Texture");
                ClientSend.KnightTexture();
            }
            
            if (sprintTexHash != receivedSprintTexHash)
            {
                Log("Sending updated Sprint Texture");
                ClientSend.SprintTexture();
            }
            
            if (unnTexHash != receivedUnnTexHash)
            {
                Log("Sending updated Unn Texture");
                ClientSend.UnnTexture();
            }
            
            if (voidTexHash != receivedVoidTexHash)
            {
                Log("Sending updated Void Texture");
                ClientSend.VoidTexture();
            }
            
            if (vsTexHash != receivedVSTexHash)
            {
                Log("Sending updated VS Texture");
                ClientSend.VSTexture();
            }
            
            if (wraithsTexHash != receivedWraithsTexHash)
            {
                Log("Sending updated Wraiths Texture");
                ClientSend.WraithsTexture();
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