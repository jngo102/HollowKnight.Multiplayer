using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
            short order = packet.ReadShort();
            byte[] texBytes = packet.ReadBytes(16378);

            if (SessionManager.Instance.Players.ContainsKey(client))
            {
                SessionManager.Instance.Players[client].TexBytes[texName].Add(order, texBytes);
            }
        }
        
        public static void FinishedSendingTexBytes(Packet packet)
        {
            byte client = packet.ReadByte();
            string texName = packet.ReadString();
            bool finishedSending = packet.ReadBool();
            
            if (finishedSending)
            {
                SessionManager.Instance.CompileByteFragments(client, texName);
            }
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