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
                GameManager.instance.StartCoroutine(SessionManager.Instance.CompileByteFragments(client, texName));
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
            int receivedBaldurHash = packet.ReadInt();
            int receivedFlukeHash = packet.ReadInt();
            int receivedGrimmHash = packet.ReadInt();
            int receivedHatchlingHash = packet.ReadInt();
            int receivedKnightHash = packet.ReadInt();
            int receivedShieldHash = packet.ReadInt();
            int receivedSprintHash = packet.ReadInt();
            int receivedUnnHash = packet.ReadInt();
            int receivedVoidHash = packet.ReadInt();
            int receivedVSHash = packet.ReadInt();
            int receivedWeaverHash = packet.ReadInt();
            int receivedWraithsHash = packet.ReadInt();
            
            GameObject charmEffects = hc.FindGameObjectInChildren("Charm Effects");
            
            GameObject baldur = charmEffects.FindGameObjectInChildren("Blocker Shield").FindGameObjectInChildren("Shell Anim");
            Texture2D baldurTex = baldur.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            int baldurHash = baldurTex.GetHashCode();

            PlayMakerFSM poolFlukes = charmEffects.LocateMyFSM("Pool Flukes");
            GameObject fluke = poolFlukes.GetAction<CreateGameObjectPool>("Pool Normal", 0).prefab.Value;
            Texture2D flukeTex = fluke.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            int flukeHash = flukeTex.GetHashCode();
            
            PlayMakerFSM spawnGrimmchild = charmEffects.LocateMyFSM("Spawn Grimmchild");
            GameObject grimm = spawnGrimmchild.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            Texture2D grimmTex = grimm.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            int grimmHash = grimmTex.GetHashCode();
            
            PlayMakerFSM hatchlingSpawn = charmEffects.LocateMyFSM("Hatchling Spawn");
            GameObject hatchling = hatchlingSpawn.GetAction<SpawnObjectFromGlobalPool>("Hatch", 2).gameObject.Value;
            Texture2D hatchlingTex = hatchling.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            int hatchlingHash = hatchlingTex.GetHashCode();

            PlayMakerFSM spawnOrbitShield = charmEffects.LocateMyFSM("Spawn Orbit Shield");
            GameObject orbitShield = spawnOrbitShield.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            GameObject shield = orbitShield.FindGameObjectInChildren("Shield");
            Texture2D shieldTex = shield.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            int shieldHash = shieldTex.GetHashCode();
            
            PlayMakerFSM weaverlingControl = charmEffects.LocateMyFSM("Weaverling Control");
            GameObject weaver = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn", 0).gameObject.Value;
            Texture2D weaverTex = weaver.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            int weaverHash = weaverTex.GetHashCode();
            
            var anim = hc.GetComponent<tk2dSpriteAnimator>();
            Texture2D knightTex = anim.GetClipByName("Idle").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D sprintTex = anim.GetClipByName("Sprint").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D unnTex = anim.GetClipByName("Slug Up").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            int knightHash = knightTex.GetHashCode();
            int sprintHash = sprintTex.GetHashCode();
            int unnHash = unnTex.GetHashCode();
            int voidHash = 0;
            int vsHash = 0;
            int wraithsHash = 0;
            foreach (Transform child in hc.transform)
            {
                if (child.name == "Spells")
                {
                    foreach (Transform spellsChild in child)
                    {
                        if (spellsChild.name == "Scr Heads")
                        {
                            Texture2D wraithsTex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            wraithsHash = wraithsTex.GetHashCode();
                        }
                        else if (spellsChild.name == "Scr Heads 2")
                        {
                            Texture2D voidTex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            voidHash = voidTex.GetHashCode();
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
                            vsHash = vsTex.GetHashCode();
                            break;
                        }
                    }
                }
            }

            if (baldurHash != receivedBaldurHash)
            {
                Log("Sending updated Baldur Texture");
                ClientSend.BaldurTexture();
            }
            
            if (flukeHash != receivedFlukeHash)
            {
                Log("Sending updated Fluke Texture");
                ClientSend.FlukeTexture();
            }
            
            if (grimmHash != receivedGrimmHash)
            {
                Log("Sending updated Grimm Texture");
                ClientSend.GrimmTexture();
            }
            
            if (hatchlingHash != receivedHatchlingHash)
            {
                Log("Sending updated Hatchling Texture");
                ClientSend.HatchlingTexture();
            }
            
            if (knightHash != receivedKnightHash)
            {
                Log("Sending updated Knight Texture");
                ClientSend.KnightTexture();
            }
            
            if (shieldHash != receivedShieldHash)
            {
                Log("Sending updated Shield Texture");
                ClientSend.ShieldTexture();
            }
            
            if (sprintHash != receivedSprintHash)
            {
                Log("Sending updated Sprint Texture");
                ClientSend.SprintTexture();
            }
            
            if (unnHash != receivedUnnHash)
            {
                Log("Sending updated Unn Texture");
                ClientSend.UnnTexture();
            }
            
            if (voidHash != receivedVoidHash)
            {
                Log("Sending updated Void Texture");
                ClientSend.VoidTexture();
            }
            
            if (vsHash != receivedVSHash)
            {
                Log("Sending updated VS Texture");
                ClientSend.VSTexture();
            }
            
            if (weaverHash != receivedWeaverHash)
            {
                Log("Sending updated Weaver Texture");
                ClientSend.WeaverTexture();
            }
            
            if (wraithsHash != receivedWraithsHash)
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