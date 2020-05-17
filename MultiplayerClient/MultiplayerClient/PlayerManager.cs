using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using UnityEngine;

namespace MultiplayerClient
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;
        
        public byte id;
        public string username;

        public static string activeScene;
        
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

        public Texture2D baldurTexture;
        public Texture2D flukeTexture;
        public Texture2D grimmTexture;
        public Texture2D hatchlingTexture;
        public Texture2D knightTexture;
        public Texture2D shieldTexture;
        public Texture2D sprintTexture;
        public Texture2D unnTexture;
        public Texture2D voidTexture;
        public Texture2D vsTexture;
        public Texture2D weaverTexture;
        public Texture2D wraithsTexture;
        
        private void Awake()
        {
            Instance = this;

            AssignDefaultTextures();
        }

        private void AssignDefaultTextures()
        {
            GameObject hc = HeroController.instance.gameObject;
            GameObject charmEffects = hc.FindGameObjectInChildren("Charm Effects");
            
            GameObject baldur = charmEffects.FindGameObjectInChildren("Blocker Shield").FindGameObjectInChildren("Shell Anim");
            baldurTexture = baldur.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            
            PlayMakerFSM poolFlukes = charmEffects.LocateMyFSM("Pool Flukes");
            GameObject fluke = poolFlukes.GetAction<CreateGameObjectPool>("Pool Normal", 0).prefab.Value;
            flukeTexture = fluke.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            
            PlayMakerFSM spawnGrimmchild = charmEffects.LocateMyFSM("Spawn Grimmchild");
            GameObject grimm = spawnGrimmchild.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            grimmTexture = grimm.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            
            PlayMakerFSM hatchlingSpawn = charmEffects.LocateMyFSM("Hatchling Spawn");
            GameObject hatchling = hatchlingSpawn.GetAction<SpawnObjectFromGlobalPool>("Hatch", 2).gameObject.Value;
            hatchlingTexture = hatchling.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            
            PlayMakerFSM spawnOrbitShield = charmEffects.LocateMyFSM("Spawn Orbit Shield");
            GameObject orbitShield = spawnOrbitShield.GetAction<SpawnObjectFromGlobalPool>("Spawn", 2).gameObject.Value;
            GameObject shield = orbitShield.FindGameObjectInChildren("Shield");
            shieldTexture = shield.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            PlayMakerFSM weaverlingControl = charmEffects.LocateMyFSM("Weaverling Control");
            GameObject weaver = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn", 0).gameObject.Value;
            weaverTexture = weaver.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;

            knightTexture = hc.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
            
            tk2dSpriteAnimator anim = hc.GetComponent<tk2dSpriteAnimator>();
            sprintTexture = anim.GetClipByName("Sprint").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            unnTexture = anim.GetClipByName("Slug Up").frames[0].spriteCollection.spriteDefinitions[0].material.mainTexture as Texture2D;
            
            foreach (Transform child in hc.transform)
            {
                if (child.name == "Spells")
                {
                    foreach (Transform spellsChild in child)
                    {
                        if (spellsChild.name == "Scr Heads")
                        {
                            wraithsTexture = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                        }
                        else if (spellsChild.name == "Scr Heads 2")
                        {
                            voidTexture = spellsChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                        }
                    }
                }
                else if (child.name == "Focus Effects")
                {
                    foreach (Transform focusChild in child)
                    {
                        if (focusChild.name == "Heal Anim")
                        {
                            vsTexture = focusChild.gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture as Texture2D;
                            break;
                        }
                    }
                }
            }
        }
        
        private void Log(object message) => Modding.Logger.Log("[Player Manager] " + message);
    }
}