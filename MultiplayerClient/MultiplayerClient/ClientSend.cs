using System;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using MultiplayerClient.Canvas;
using UnityEngine;

namespace MultiplayerClient
{
    public class ClientSend : MonoBehaviour
    {
        /// <summary>Sends a packet to the server via TCP.</summary>
        /// <param name="packet">The packet to send to the sever.</param>
        private static void SendTCPData(Packet packet)
        {
            packet.WriteLength();
            Client.Instance.tcp.SendData(packet);
        }

        /// <summary>Sends a packet to the server via UDP.</summary>
        /// <param name="packet">The packet to send to the sever.</param>
        private static void SendUDPData(Packet packet)
        {
            packet.WriteLength();
            Client.Instance.udp.SendData(packet);
        }
        
        #region Packets

        /// <summary>Lets the server know that the welcome message was received.</summary>
        public static void WelcomeReceived()
        {
            using (Packet packet = new Packet((int) ClientPackets.WelcomeReceived))
            {
                Transform heroTransform = HeroController.instance.gameObject.transform;
                
                packet.Write(Client.Instance.myId);
                packet.Write(MultiplayerClient.settings.username);
                packet.Write(HeroController.instance.GetComponent<tk2dSpriteAnimator>().CurrentClip.name);
                packet.Write(PlayerManager.activeScene);
                packet.Write(heroTransform.position);
                packet.Write(heroTransform.localScale);
                packet.Write(PlayerData.instance.health);
                packet.Write(PlayerData.instance.maxHealth);
                packet.Write(PlayerData.instance.healthBlue);

                for (int charmNum = 1; charmNum <= 40; charmNum++)
                {
                    packet.Write(PlayerData.instance.GetAttr<PlayerData, bool>("equippedCharm_" + charmNum));
                }
                
                Log("Welcome Received Packet Length: " + packet.Length());

                SendTCPData(packet);
            }
        }

        #region CustomKnight Integration

        private static void FragmentAndSendTexture(Texture2D tex, int clientPacketId, string texName)
        {
            byte[] texBytes = tex.DuplicateTexture().EncodeToPNG();
            int length = 16378;
            byte[] fragment = new byte[length];
            short order = 0;
            for (int i = 0; i < texBytes.Length; i += length)
            {
                if (texBytes.Length - i < length)
                {
                    length = texBytes.Length - i;
                }
                
                Array.Copy(texBytes, i, fragment, 0,  length);
                using (Packet packet = new Packet(clientPacketId))
                {
                    packet.Write(order);
                    packet.Write(fragment);

                    order++;

                    SendTCPData(packet);
                }
            }
            
            Log("Sending Finish Sending Texture Bytes");
            FinishedSendingTexBytes(texName);
        }
        
        private static void FinishedSendingTexBytes(string texName, bool finishedSending = true)
        {
            using (Packet packet = new Packet((int) ClientPackets.FinishedSendingTexBytes))
            {
                packet.Write(texName);
                packet.Write(finishedSending);

                SendTCPData(packet);
            }
        }
        
        public static void BaldurTexture()
        {
            GameObject charmEffects = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects");
            GameObject baldur = charmEffects.FindGameObjectInChildren("Blocker Shield").FindGameObjectInChildren("Shell Anim");
            Texture2D tex = baldur.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            tex = tex.DuplicateTexture();

            FragmentAndSendTexture(tex, (int) ClientPackets.BaldurTexture, "Baldur");
        }
        
        public static void FlukeTexture()
        {
            GameObject charmEffects = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects");
            PlayMakerFSM poolFlukes = charmEffects.LocateMyFSM("Pool Flukes");
            GameObject fluke = poolFlukes.GetAction<CreateGameObjectPool>("Pool Normal", 0).prefab.Value;
            Texture2D tex = fluke.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            tex = tex.DuplicateTexture();
            
            FragmentAndSendTexture(tex, (int) ClientPackets.FlukeTexture, "Fluke");
        }
        
        public static void GrimmTexture()
        {
            GameObject charmEffects = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects");
            PlayMakerFSM spawnGrimmchild = charmEffects.LocateMyFSM("Spawn Grimmchild");
            GameObject grimm = spawnGrimmchild.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            Texture2D tex = grimm.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            tex = tex.DuplicateTexture();
           
            FragmentAndSendTexture(tex, (int) ClientPackets.GrimmTexture, "Grimm");
        }
        
        public static void HatchlingTexture()
        {
            GameObject charmEffects = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects");
            PlayMakerFSM hatchlingSpawn = charmEffects.LocateMyFSM("Hatchling Spawn");
            GameObject hatchling = hatchlingSpawn.GetAction<SpawnObjectFromGlobalPool>("Hatch", 2).gameObject.Value;
            Texture2D tex = hatchling.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            tex = tex.DuplicateTexture();
            
            FragmentAndSendTexture(tex, (int) ClientPackets.HatchlingTexture, "Hatchling");
        }
        
        public static void KnightTexture()
        {
            var anim = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
            Texture2D tex = anim.GetClipByName("Idle").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            
            FragmentAndSendTexture(tex, (int) ClientPackets.KnightTexture, "Knight");
        }

        public static void ShieldTexture()
        {
            GameObject charmEffects = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects");
            PlayMakerFSM spawnOrbitShield = charmEffects.LocateMyFSM("Spawn Orbit Shield");
            GameObject orbitShield = spawnOrbitShield.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            GameObject shield = orbitShield.FindGameObjectInChildren("Shield");
            Texture2D tex = shield.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            tex = tex.DuplicateTexture();
            
            FragmentAndSendTexture(tex, (int) ClientPackets.ShieldTexture, "Shield");
        }
        
        public static void SprintTexture()
        {
            var anim = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
            Texture2D tex = anim.GetClipByName("Sprint").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            
            FragmentAndSendTexture(tex, (int) ClientPackets.SprintTexture, "Sprint");
        }
        
        public static void UnnTexture()
        {
            var anim = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
            Texture2D tex = anim.GetClipByName("Slug Up").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            tex = tex.DuplicateTexture();
            
            FragmentAndSendTexture(tex, (int) ClientPackets.UnnTexture, "Unn");
        }
        
        public static void VoidTexture()
        {
            GameObject hc = HeroController.instance.gameObject;
            Texture2D tex = null;
            foreach (Transform child in hc.transform)
            {
                if (child.name == "Spells")
                {
                    foreach (Transform spellsChild in child)
                    {
                        if (spellsChild.name == "Scr Heads 2")
                        {
                            tex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                        }
                    }
                }
            }
            
            FragmentAndSendTexture(tex, (int) ClientPackets.VoidTexture, "Void");
        }
        
        public static void VSTexture()
        {
            GameObject hc = HeroController.instance.gameObject;
            Texture2D tex = null;
            foreach (Transform child in hc.transform)
            {
                if (child.name == "Focus Effects")
                {
                    foreach (Transform focusChild in child)
                    {
                        if (focusChild.name == "Heal Anim")
                        {
                            tex = focusChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            break;
                        }
                    }
                }
            }

            FragmentAndSendTexture(tex, (int) ClientPackets.VSTexture, "VS");
        }
        
        public static void WeaverTexture()
        {
            GameObject charmEffects = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects");
            PlayMakerFSM weaverlingControl = charmEffects.LocateMyFSM("Weaverling Control");
            GameObject weaver = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn", 0).gameObject.Value;
            Texture2D tex = weaver.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            tex = tex.DuplicateTexture();
            
            FragmentAndSendTexture(tex, (int) ClientPackets.WeaverTexture, "Weaver");
        }

        public static void WraithsTexture()
        {
            GameObject hc = HeroController.instance.gameObject;
            Texture2D tex = null;
            foreach (Transform child in hc.transform)
            {
                if (child.name == "Spells")
                {
                    foreach (Transform spellsChild in child)
                    {
                        if (spellsChild.name == "Scr Heads")
                        {
                            tex = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                        }
                    }
                }
            }
            
            
            FragmentAndSendTexture(tex, (int) ClientPackets.WraithsTexture, "Wraiths");
        }

        public static void ServerHash(string texName, string hash)
        {
            using (Packet packet = new Packet((int) ClientPackets.ServerHash))
            {
                packet.Write(texName);
                packet.Write(hash);

                SendTCPData(packet);
            }
    }
        
        #endregion CustomKnight Integration
        
        public static void PlayerPosition(Vector3 position)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerPosition))
            {
                packet.Write(position);
                
                SendUDPData(packet);
            }
        }
        
        public static void PlayerScale(Vector3 scale)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerScale))
            {
                packet.Write(scale);
                
                SendUDPData(packet);
            }
        }

        public static void PlayerAnimation(string animation)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerAnimation))
            {
                packet.Write(animation);
                
                SendUDPData(packet);
            }
        }

        public static void SceneChanged(string sceneName)
        {
            using (Packet packet = new Packet((int) ClientPackets.SceneChanged))
            {
                packet.Write(sceneName);
                
                SendTCPData(packet);
            }
        }

        public static void HealthUpdated(int currentHealth, int currentMaxHealth, int currentHealthBlue)
        {
            using (Packet packet = new Packet((int) ClientPackets.HealthUpdated))
            {
                packet.Write(currentHealth);
                packet.Write(currentMaxHealth);
                packet.Write(currentHealthBlue);

                Log("Sending Health Data to Server");
                SendTCPData(packet);
            }
        }
        
        public static void CharmsUpdated(PlayerData pd)
        {
            using (Packet packet = new Packet((int) ClientPackets.CharmsUpdated))
            {
                for (int i = 1; i <= 40; i++)
                {
                    packet.Write(pd.GetBool("equippedCharm_" + i));
                }

                Log("Packet Length: " + packet.Length());
                Log("Sending CharmsUpdated Packet from Client");
                SendTCPData(packet);
            }
        }
        
        public static void PlayerDisconnected(int id)
        {
            using (Packet packet = new Packet((int) ClientPackets.PlayerDisconnected))
            {
                packet.Write(id);

                SendTCPData(packet);
            }
        }
        
        #endregion

        private static void Log(object message) => Modding.Logger.Log("[Client Send] " + message);
    }
}