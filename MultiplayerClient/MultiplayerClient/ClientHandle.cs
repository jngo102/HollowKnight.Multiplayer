using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
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
            //var hashes = GetTextureHashes();
            
            ClientSend.WelcomeReceived(/*hashes, */Client.Instance.isHost);

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
                    
                var player = SessionManager.Instance.Players[id];
                /*foreach (TextureType tt in Enum.GetValues(typeof(TextureType)))
                {
                    var hash = packet.ReadBytes(20);
                    Log("Hash null? " + (hash == null));
                    player.texHashes.Add(hash, tt);
                }*/

                SessionManager.Instance.ReloadPlayerTextures(player);
            }
        }

        #region CustomKnight Integration

        public static void LoadTexture(Packet packet)
        {
            byte[] hash = packet.ReadBytes(20);
            if (SessionManager.Instance.loadedTextures.ContainsKey(hash)) return;

            int texLen = packet.ReadInt();
            byte[] texBytes = packet.ReadBytes(texLen);

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(texBytes);

            // Save reference to texture for easy reuse
            SessionManager.Instance.loadedTextures[hash] = texture;

            foreach (var player in SessionManager.Instance.Players.Values)
            {
                if(player.texHashes.ContainsKey(hash))
                {
                    TextureType tt = player.texHashes[hash];
                    player.textures[tt] = texture;
                }
            }
        }

        public static void HandleTextureRequest(Packet packet)
        {
            byte[] hash = packet.ReadBytes(20);

            Log("Received request for hash " + BitConverter.ToString(hash));
            if (MultiplayerClient.textureCache.ContainsKey(hash))
            {
                byte[] texture = File.ReadAllBytes(MultiplayerClient.textureCache[hash]);
                Log("Sending texture for hash " + BitConverter.ToString(hash));
                ClientSend.SendTexture(texture);
            }
        }

        /// Hash the textures and store them in the cache if needed.
        /// This method is slow - don't use it in the middle of gameplay.
        public static List<byte[]> GetTextureHashes()
        {
            var cacheDir = Path.Combine(Application.dataPath, "SkinCache");
            Directory.CreateDirectory(cacheDir);
            
            GameObject hc = HeroController.instance.gameObject;

            // SHA-1 hashes are 20 bytes, or 160 bits long.
            const int HASH_LENGTH = 20;
            byte[] baldurHash = new byte[HASH_LENGTH];
            byte[] flukeHash = new byte[HASH_LENGTH];
            byte[] grimmHash = new byte[HASH_LENGTH];
            byte[] hatchlingHash = new byte[HASH_LENGTH];
            byte[] knightHash = new byte[HASH_LENGTH];    
            byte[] shieldHash = new byte[HASH_LENGTH];
            byte[] sprintHash = new byte[HASH_LENGTH];
            byte[] unnHash = new byte[HASH_LENGTH];
            byte[] voidHash = new byte[HASH_LENGTH];
            byte[] vsHash = new byte[HASH_LENGTH];
            byte[] weaverHash = new byte[HASH_LENGTH];
            byte[] wraithsHash = new byte[HASH_LENGTH];

            var anim = hc.GetComponent<tk2dSpriteAnimator>();
            Texture2D knightTex = anim.GetClipByName("Idle").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D sprintTex = anim.GetClipByName("Sprint").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D unnTex = anim.GetClipByName("Slug Up").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;

            knightHash = knightTex.Hash();
            sprintHash = sprintTex.Hash();
            unnHash = unnTex.Hash();
            
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
                else if (child.name == "Charm Effects")
                {
                    foreach (Transform charmChild in child)
                    {
                        if (charmChild.name == "Blocker Shield")
                        {
                            GameObject shellAnim = charmChild.GetChild(0).gameObject;
                            Texture2D baldurTex = shellAnim.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            baldurHash = baldurTex.Hash();
                            break;            
                        }
                    }
                    
                    PlayMakerFSM poolFlukes = child.gameObject.LocateMyFSM("Pool Flukes");
                    GameObject fluke = poolFlukes.GetAction<CreateGameObjectPool>("Pool Normal", 0).prefab.Value;
                    Texture2D flukeTex = fluke.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                    flukeHash = flukeTex.Hash();

                    PlayMakerFSM spawnGrimmchild = child.gameObject.LocateMyFSM("Spawn Grimmchild");
                    GameObject grimm = spawnGrimmchild.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
                    Texture2D grimmTex = grimm.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                    grimmHash = grimmTex.Hash();

                    PlayMakerFSM hatchlingSpawn = child.gameObject.LocateMyFSM("Hatchling Spawn");
                    GameObject hatchling = hatchlingSpawn.GetAction<SpawnObjectFromGlobalPool>("Hatch", 2).gameObject.Value;
                    Texture2D hatchlingTex = hatchling.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                    hatchlingHash = hatchlingTex.Hash();

                    PlayMakerFSM spawnOrbitShield = child.gameObject.LocateMyFSM("Spawn Orbit Shield");
                    GameObject orbitShield = spawnOrbitShield.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
                    GameObject shield = orbitShield.FindGameObjectInChildren("Shield");
                    Texture2D shieldTex = shield.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                    shieldHash = shieldTex.Hash();

                    PlayMakerFSM weaverlingControl = child.gameObject.LocateMyFSM("Weaverling Control");
                    GameObject weaver = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn", 0).gameObject.Value;
                    Texture2D weaverTex = weaver.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                    weaverHash = weaverTex.Hash();
                }
            }

            // Ordered according to the TextureType enum
            var hashes = new List<byte[]>();
            hashes.Add(baldurHash);
            hashes.Add(flukeHash);
            hashes.Add(grimmHash);
            hashes.Add(hatchlingHash);
            hashes.Add(knightHash);
            hashes.Add(shieldHash);
            hashes.Add(sprintHash);
            hashes.Add(unnHash);
            hashes.Add(voidHash);
            hashes.Add(vsHash);
            hashes.Add(weaverHash);
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

        public static void DisconnectSelf(Packet packet)
        {
            Log("Disconnecting Self");
            Client.Instance.Disconnect();
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client Handle] " + message);
    }
}
