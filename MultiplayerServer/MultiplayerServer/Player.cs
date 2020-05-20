using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using UnityEngine;

namespace MultiplayerServer
{
    public class Player : MonoBehaviour
    {
        public byte id;
        public string username;
        public string animation;
        public string activeScene;
        public Vector3 position;
        public Vector3 scale;

        public bool equippedCharm_1;
        public bool equippedCharm_2;
        public bool equippedCharm_3;
        public bool equippedCharm_4;
        public bool equippedCharm_5;
        public bool equippedCharm_6;
        public bool equippedCharm_7;
        public bool equippedCharm_8;
        public bool equippedCharm_9;
        public bool equippedCharm_10;
        public bool equippedCharm_11;
        public bool equippedCharm_12;
        public bool equippedCharm_13;
        public bool equippedCharm_14;
        public bool equippedCharm_15;
        public bool equippedCharm_16;
        public bool equippedCharm_17;
        public bool equippedCharm_18;
        public bool equippedCharm_19;
        public bool equippedCharm_20;
        public bool equippedCharm_21;
        public bool equippedCharm_22;
        public bool equippedCharm_23;
        public bool equippedCharm_24;
        public bool equippedCharm_25;
        public bool equippedCharm_26;
        public bool equippedCharm_27;
        public bool equippedCharm_28;
        public bool equippedCharm_29;
        public bool equippedCharm_30;
        public bool equippedCharm_31;
        public bool equippedCharm_32;
        public bool equippedCharm_33;
        public bool equippedCharm_34;
        public bool equippedCharm_35;
        public bool equippedCharm_36;
        public bool equippedCharm_37;
        public bool equippedCharm_38;
        public bool equippedCharm_39;
        public bool equippedCharm_40;

        public int health;
        public int maxHealth;
        public int healthBlue;

        public int baldurHash;
        public int flukeHash;
        public int grimmHash;
        public int hatchlingHash;
        public int knightHash;
        public int shieldHash;
        public int sprintHash;
        public int unnHash;
        public int voidHash;
        public int vsHash;
        public int weaverHash;
        public int wraithsHash;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => HeroController.instance != null);
            
            GameObject hc = HeroController.instance.gameObject;
            GameObject charmEffects = hc.FindGameObjectInChildren("Charm Effects");
            
            GameObject baldur = charmEffects.FindGameObjectInChildren("Blocker Shield").FindGameObjectInChildren("Shell Anim");
            Texture2D baldurTex = baldur.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            baldurHash = baldurTex.GetHashCode();

            PlayMakerFSM poolFlukes = charmEffects.LocateMyFSM("Pool Flukes");
            GameObject fluke = poolFlukes.GetAction<CreateGameObjectPool>("Pool Normal", 0).prefab.Value;
            Texture2D flukeTex = fluke.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            flukeHash = flukeTex.GetHashCode();
            
            PlayMakerFSM spawnGrimmchild = charmEffects.LocateMyFSM("Spawn Grimmchild");
            GameObject grimm = spawnGrimmchild.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            Texture2D grimmTex = grimm.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            grimmHash = grimmTex.GetHashCode();
            
            PlayMakerFSM hatchlingSpawn = charmEffects.LocateMyFSM("Hatchling Spawn");
            GameObject hatchling = hatchlingSpawn.GetAction<SpawnObjectFromGlobalPool>("Hatch", 2).gameObject.Value;
            Texture2D hatchlingTex = hatchling.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            hatchlingHash = hatchlingTex.GetHashCode();

            PlayMakerFSM spawnOrbitShield = charmEffects.LocateMyFSM("Spawn Orbit Shield");
            GameObject orbitShield = spawnOrbitShield.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            GameObject shield = orbitShield.FindGameObjectInChildren("Shield");
            Texture2D shieldTex = shield.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            shieldHash = shieldTex.GetHashCode();
            
            PlayMakerFSM weaverlingControl = charmEffects.LocateMyFSM("Weaverling Control");
            GameObject weaver = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn", 0).gameObject.Value;
            Texture2D weaverTex = weaver.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            weaverHash = weaverTex.GetHashCode();
            
            var anim = hc.GetComponent<tk2dSpriteAnimator>();
            Texture2D knightTex = anim.GetClipByName("Idle").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D sprintTex = anim.GetClipByName("Sprint").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            Texture2D unnTex = anim.GetClipByName("Slug Up").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            knightHash = knightTex.GetHashCode();
            sprintHash = sprintTex.GetHashCode();
            unnHash = unnTex.GetHashCode();
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
        }
        
        public void Initialize(byte id, string username, string animation, int health, int maxHealth, int healthBlue)
        {
            this.id = id;
            this.username = username;
            this.animation = animation;
            this.health = health;
            this.maxHealth = maxHealth;
            this.healthBlue = healthBlue;
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