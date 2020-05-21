using System.Collections.Generic;
using System.IO;
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

            // Ideally you would do this on skin change.
            // For now we only do it when joining the server.
            var hashes = GetTextureHashes();

            ClientSend.WelcomeReceived(hashes);
            
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

        public static void LoadTexture(Packet packet)
        {
            byte client = packet.ReadByte();
            var texType = (TextureType)packet.ReadByte();
            int texLen = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(texLen);
            GameManager.instance.StartCoroutine(SessionManager.Instance.CompileByteFragments(client, texBytes, texType));
        }

        public static void HandleTextureRequest(Packet packet)
        {
            byte[] hash = packet.ReadBytes(20);

            if(MultiplayerClient.textureCache.ContainsKey(hash))
            {
                byte[] texture = File.ReadAllBytes(MultiplayerClient.textureCache[hash]);
                ClientSend.SendTexture(hash, texture);
            }
        }

        /// Hash the textures and store them in the cache if needed.
        /// This method is slow - don't use it in the middle of gameplay.
        public static List<byte[]> GetTextureHashes()
        {
            var cacheDir = Path.Combine(Application.dataPath, "SkinCache");
            Directory.CreateDirectory(cacheDir);
            
            GameObject hc = HeroController.instance.gameObject;
            GameObject charmEffects = hc.FindGameObjectInChildren("Charm Effects");
            
            GameObject baldur = charmEffects.FindGameObjectInChildren("Blocker Shield").FindGameObjectInChildren("Shell Anim");
            Texture2D baldurTex = baldur.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            PlayMakerFSM poolFlukes = charmEffects.LocateMyFSM("Pool Flukes");
            GameObject fluke = poolFlukes.GetAction<CreateGameObjectPool>("Pool Normal", 0).prefab.Value;
            Texture2D flukeTex = fluke.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            PlayMakerFSM spawnGrimmchild = charmEffects.LocateMyFSM("Spawn Grimmchild");
            GameObject grimm = spawnGrimmchild.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            Texture2D grimmTex = grimm.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            PlayMakerFSM hatchlingSpawn = charmEffects.LocateMyFSM("Hatchling Spawn");
            GameObject hatchling = hatchlingSpawn.GetAction<SpawnObjectFromGlobalPool>("Hatch", 2).gameObject.Value;
            Texture2D hatchlingTex = hatchling.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            var anim = hc.GetComponent<tk2dSpriteAnimator>();
            Texture2D knightTex = anim.GetClipByName("Idle").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D sprintTex = anim.GetClipByName("Sprint").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D unnTex = anim.GetClipByName("Slug Up").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;

            PlayMakerFSM spawnOrbitShield = charmEffects.LocateMyFSM("Spawn Orbit Shield");
            GameObject orbitShield = spawnOrbitShield.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            GameObject shield = orbitShield.FindGameObjectInChildren("Shield");
            Texture2D shieldTex = shield.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            PlayMakerFSM weaverlingControl = charmEffects.LocateMyFSM("Weaverling Control");
            GameObject weaver = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn", 0).gameObject.Value;
            Texture2D weaverTex = weaver.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            // SHA-1 hashes are 20 bytes, or 160 bits long.
            const int HASH_LENGTH = 20;
            byte[] voidHash = new byte[HASH_LENGTH];
            byte[] vsHash = new byte[HASH_LENGTH];
            byte[] wraithsHash = new byte[HASH_LENGTH];

            foreach (Transform child in hc.transform)
            {
                if (child.name == "Spells")
                {
                    foreach (Transform spellsChild in child)
                    {
                        if (spellsChild.name == "Scr Heads")
                        {
                            Texture2D wraithsTex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            wraithsHash = wraithsTex.Hash();
                        }
                        else if (spellsChild.name == "Scr Heads 2")
                        {
                            Texture2D voidTex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            voidHash = voidTex.Hash();
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
                            vsHash = vsTex.Hash();
                            break;
                        }
                    }
                }
            }

            // Ordered according to the TextureType enum
            var hashes = new List<byte[]>();
            hashes.Add(baldurTex.Hash());
            hashes.Add(flukeTex.Hash());
            hashes.Add(grimmTex.Hash());
            hashes.Add(hatchlingTex.Hash());
            hashes.Add(knightTex.Hash());
            hashes.Add(shieldTex.Hash());
            hashes.Add(sprintTex.Hash());
            hashes.Add(unnTex.Hash());
            hashes.Add(voidHash);
            hashes.Add(vsHash);
            hashes.Add(weaverTex.Hash());
            hashes.Add(wraithsHash);
            return hashes;
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