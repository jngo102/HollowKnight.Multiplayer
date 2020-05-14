using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace MultiplayerClient
{
    internal partial class MPClient : MonoBehaviour
    {
        public static MPClient Instance;

        private static GameObject _hero;
        private static HeroController _hc;
        private static GameObject _playerPrefab;
        private static PlayMakerFSM _nailArts;
        private static PlayMakerFSM _spellControl; 
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                Log("Instance already exists, destroying object.");
                Destroy(this);
            }
        }

        private IEnumerator Start()
        {
            while (HeroController.instance == null) yield return null;

            Log("Creating Client Manager");
            GameObject clientManager = new GameObject("Client Manager");
            clientManager.AddComponent<Client>();
            clientManager.AddComponent<ThreadManager>();
            
            Log("Creating Game Manager");
            GameObject gameManager = new GameObject("Game Manager");
            gameManager.AddComponent<GameManager>();

            DontDestroyOnLoad(clientManager);
            DontDestroyOnLoad(gameManager);

            _playerPrefab = new GameObject(
                "PlayerPrefab",
                typeof(BoxCollider2D),
                typeof(DamageHero),
                typeof(EnemyHitEffectsUninfected),
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(NonBouncer),
                typeof(SpriteFlash),
                typeof(tk2dSprite),
                typeof(tk2dSpriteAnimator),
                typeof(PlayerController),
                typeof(PlayerManager)
            )
            {
                layer = 9,
            };

            GameObject attacks = new GameObject("Attacks")
            {
                layer = 9,
            };
            attacks.transform.SetParent(_playerPrefab.transform);
            
            GameObject effects = new GameObject("Effects")
            {
                layer = 9,
            };
            effects.transform.SetParent(_playerPrefab.transform);
            
            GameObject spells = new GameObject("Spells")
            {
                layer = 9,
            };
            spells.transform.SetParent(_playerPrefab.transform);

            //GetKnightAttacks();
            //GetKnightEffects();
            
            _playerPrefab.SetActive(false);
            
            DontDestroyOnLoad(_playerPrefab);
            
            GameManager.Instance.playerPrefab = _playerPrefab;
            
            HeroController.instance.OnDeath += HeroDeath;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
            On.GameManager.WarpToDreamGate += OnWarpToDG;
            On.HeroController.EnterSceneDreamGate += OnEnterSceneDG;
            On.HeroController.TakeDamage += OnTakeDamage;
            On.HeroController.AddHealth += OnAddHealth;
            On.HeroController.MaxHealth += OnMaxHealth;
        }

        private void OnTakeDamage(On.HeroController.orig_TakeDamage orig, HeroController hc, GameObject go,
            CollisionSide damageSide, int damageAmount, int hazardType)
        {
            int old_health = PlayerData.instance.health;
            orig(hc, go, damageSide, damageAmount, hazardType);

            // OnTakeDamage is called even when the player has iframes.
            // And it is called a LOT, so to avoid spamming the server, we check if the health changed before sending.
            if(PlayerData.instance.health != old_health)
            {
                Log("Took Damage: " + PlayerData.instance.health + " " + PlayerData.instance.maxHealth);
                ClientSend.HealthUpdated(PlayerData.instance.health, PlayerData.instance.maxHealth, PlayerData.instance.healthBlue);
            }
        }

        private void OnAddHealth(On.HeroController.orig_AddHealth orig, HeroController hc, int amount)
        {
            orig(hc, amount);
            
            Log("Added Health: " + PlayerData.instance.health + " " + PlayerData.instance.maxHealth);
            ClientSend.HealthUpdated(PlayerData.instance.health, PlayerData.instance.maxHealth, PlayerData.instance.healthBlue);
        }

        private void OnMaxHealth(On.HeroController.orig_MaxHealth orig, HeroController hc)
        {
            orig(hc);
            
            Log("Maxed Health: " + PlayerData.instance.health + " " + PlayerData.instance.maxHealth);
            ClientSend.HealthUpdated(PlayerData.instance.health, PlayerData.instance.maxHealth, PlayerData.instance.healthBlue);
        }
        
        private void OnWarpToDG(On.GameManager.orig_WarpToDreamGate orig, global::GameManager gm)
        {
            try
            {
                orig(gm);
            }
            catch (Exception e)
            {
                Log("Could not warp to DG: " + e);
            }
        }
        
        private void OnSceneChange(Scene prevScene, Scene nextScene)
        {
            if (MultiplayerClient.NonGameplayScenes.Contains(prevScene.name) &&
                !MultiplayerClient.NonGameplayScenes.Contains(nextScene.name))
            {
                _hc = HeroController.instance;
                _hero = _hc.gameObject;
                _hero.AddComponent<HeroTracker>();

                _nailArts = _hero.LocateMyFSM("Nail Arts");
                _spellControl = _hero.LocateMyFSM("Spell Control");

                var anim = _hero.GetComponent<tk2dSpriteAnimator>();
                foreach (tk2dSpriteAnimationClip clip in anim.Library.clips)
                {
                    if (clip.frames.Length > 0)
                    {
                        tk2dSpriteAnimationFrame frame0 = clip.frames[0];
                        frame0.triggerEvent = true;
                        frame0.eventInfo = clip.name;
                        clip.frames[0] = frame0;
                    }
                }
            
                anim.AnimationEventTriggered = AnimationEventDelegate;
            
                // Q2 Land state resets tk2dSpriteAnimator's AnimationEventTriggered delegate, so insert method in following state to reset it
                _spellControl.InsertMethod("Q2 Pillar", _spellControl.GetState("Q2 Pillar").Actions.Length, () =>
                {
                    anim.AnimationEventTriggered = AnimationEventDelegate;
                });
            }
        }
        
        private void OnEnterSceneDG(On.HeroController.orig_EnterSceneDreamGate orig, HeroController self)
        {
            try
            {
                orig(self);
            }
            catch (Exception e)
            {
                Log(e);
            }
        }
        
        private void HeroDeath()
        {
            
        }

        private string _storedClip;
        private void AnimationEventDelegate(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frameNum)
        {
            if (clip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop && clip.name != _storedClip ||
                clip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection && clip.name != _storedClip ||
                clip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
            {
                _storedClip = clip.name;
                tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);

                string clipName = frame.eventInfo;
                ClientSend.PlayerAnimation(clipName);
            }
        }

        private static void Log(object message) => Modding.Logger.Log("[MP Client] " + message);
    }
}