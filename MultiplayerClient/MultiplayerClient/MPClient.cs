using System;
using System.Collections;
using System.Collections.Generic;
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

        public string activeScene;

        private static GameObject _hero;
        private static HeroController _hc;
        private static GameObject _playerPrefab;
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

            ModHooks.Instance.CharmUpdateHook += OnCharmUpdate;
            ModHooks.Instance.SavegameSaveHook += OnSavegameSave;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSavegameSave(int id)
        {
            Log("Saved Game.");
            string respawnScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            GameObject[] sceneGameObjects = FindObjectsOfType<GameObject>();
            string respawnMarkerName = null;
            foreach (GameObject go in sceneGameObjects)
            {
                if (go.name.Contains("RestBench") || go.name.Contains("WhiteBench"))
                {
                    Log("Found bench.");
                    respawnMarkerName = go.name;
                }
            }

            if (respawnMarkerName != null)
            {
                Log("Setting respawnScene and respawnMarkerName");
                PlayerData.instance.respawnScene = respawnScene;
                PlayerData.instance.respawnMarkerName = respawnMarkerName;
            }
        }

        // Animations that apparently play many times a second even though they should only play once
        private List<string> _dumbAnimations = new List<string>
        {
            "Slash",
            "SlashAlt",
            "UpSlash",
            "DownSlash",
            "Wall Slash",
            "Dash",
            "Shadow Dash",
            "Shadow Dash Sharp",
            "Shadow Dash Down Sharp",
            "Double Jump",
            "Walljump",
            "Recoil",
            "HardLand",
        };

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
            
            _hc = HeroController.instance;
            _hero = _hc.gameObject;
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
            
            _playerPrefab = new GameObject(
                "PlayerPrefab",
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
           
            _playerPrefab.PrintSceneHierarchyTree();
            
            GameManager.Instance.playerPrefab = _playerPrefab;
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
        
        private void OnCharmUpdate(PlayerData pd, HeroController hc)
        {
            if (Client.Instance != null && Client.Instance.isConnected && pd != null && hc != null)
            {
                ClientSend.CharmsUpdated(pd);
            }
        }
        
        private void OnSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (Client.Instance.isConnected)
            {
                ClientSend.SceneChanged(nextScene.name);
            }
        }

        public IEnumerator PlayAnimation(int id, string animation)
        {
            GameObject player = GameManager.Players[id].gameObject;
            GameObject attacks = player.FindGameObjectInChildren("Attacks");
            GameObject effects = player.FindGameObjectInChildren("Effects");
            GameObject spells = player.FindGameObjectInChildren("Spells");
            switch (animation)
            {
                case "SD Dash":
                    Log("SD Dash");
                    /*sdEnergy.SetActive(true);
                    sdBurst.SetActive(true);
                    sdTrail.SetActive(true);
                    sdBurstGlow.SetActive(true);
                    sdTrail.GetComponent<MeshRenderer>().enabled = true;
                    trailAnim = sdTrail.GetComponent<tk2dSpriteAnimator>();
                    trailAnim.PlayFromFrame(0);
                    trailAnim.Play("SD Trail");*/
                    break;
                case "Air Cancel":
                    Log("Air Cancel");
                    //trailAnim = sdTrail.GetComponent<tk2dSpriteAnimator>();
                    //trailAnim.Play("SD Trail End");
                    break;
                case "Slash":
                    Log("Slash");
                    GameObject slash = Instantiate(_hc.slashPrefab, attacks.transform);
                    slash.SetActive(true);
                    NailSlash nailSlash = slash.GetComponent<NailSlash>();
                    nailSlash.SetMantis(GameManager.Players[id].equippedCharm_13);
                    if (GameManager.Players[id].equippedCharm_18 && GameManager.Players[id].equippedCharm_13)
                    {
                        nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.4f, nailSlash.scale.y * 1.4f,
                            nailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_13)
                    {
                        nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.25f,
                            nailSlash.scale.y * 1.25f, nailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_18)
                    {
                        nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.15f,
                            nailSlash.scale.y * 1.15f, nailSlash.scale.z);
                    }

                    nailSlash.StartSlash();

                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject slashCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], slash.transform);
                        slashCollider.SetActive(true);
                        Vector2[] points = slash.GetComponent<PolygonCollider2D>().points;
                        slashCollider.GetComponent<PolygonCollider2D>().points = points;
                        var anim = slash.GetComponent<tk2dSpriteAnimator>();
                        float lifetime = anim.DefaultClip.frames.Length / anim.ClipFps;
                        Destroy(slash, lifetime);
                        Destroy(slashCollider, lifetime);
                    }

                    break;
                case "SlashAlt":
                    GameObject altSlash = Instantiate(_hc.slashAltPrefab, attacks.transform);
                    altSlash.SetActive(true);
                    var altNailSlash = altSlash.GetComponent<NailSlash>();
                    altNailSlash.SetMantis(GameManager.Players[id].equippedCharm_13);
                    if (GameManager.Players[id].equippedCharm_18 && GameManager.Players[id].equippedCharm_13)
                    {
                        altNailSlash.transform.localScale = new Vector3(altNailSlash.scale.x * 1.4f,
                            altNailSlash.scale.y * 1.4f, altNailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_13)
                    {
                        altNailSlash.transform.localScale = new Vector3(altNailSlash.scale.x * 1.25f,
                            altNailSlash.scale.y * 1.25f, altNailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_18)
                    {
                        altNailSlash.transform.localScale = new Vector3(altNailSlash.scale.x * 1.15f,
                            altNailSlash.scale.y * 1.15f, altNailSlash.scale.z);
                    }

                    altNailSlash.StartSlash();

                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject altSlashCollider =
                            Instantiate(MultiplayerClient.GameObjects["Slash"], altSlash.transform);
                        altSlashCollider.SetActive(true);
                        altSlashCollider.layer = 22;
                        Vector2[] altPoints = altSlash.GetComponent<PolygonCollider2D>().points;
                        altSlashCollider.GetComponent<PolygonCollider2D>().points = altPoints;
                        var altAnim = altSlash.GetComponent<tk2dSpriteAnimator>();
                        float altLifetime = altAnim.DefaultClip.frames.Length / altAnim.ClipFps;
                        Destroy(altSlash, altLifetime);
                        Destroy(altSlashCollider, altLifetime);
                    }

                    break;
                case "DownSlash":
                    GameObject downSlash = Instantiate(_hc.downSlashPrefab, attacks.transform);
                    downSlash.SetActive(true);
                    var downNailSlash = downSlash.GetComponent<NailSlash>();
                    downNailSlash.SetMantis(GameManager.Players[id].equippedCharm_13);
                    if (GameManager.Players[id].equippedCharm_18 && GameManager.Players[id].equippedCharm_13)
                    {
                        downNailSlash.transform.localScale = new Vector3(downNailSlash.scale.x * 1.4f,
                            downNailSlash.scale.y * 1.4f, downNailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_13)
                    {
                        downNailSlash.transform.localScale = new Vector3(downNailSlash.scale.x * 1.25f,
                            downNailSlash.scale.y * 1.25f, downNailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_18)
                    {
                        downNailSlash.transform.localScale = new Vector3(downNailSlash.scale.x * 1.15f,
                            downNailSlash.scale.y * 1.15f, downNailSlash.scale.z);
                    }

                    downNailSlash.StartSlash();

                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject downSlashCollider =
                            Instantiate(MultiplayerClient.GameObjects["Slash"], downSlash.transform);
                        downSlashCollider.SetActive(true);
                        downSlashCollider.layer = 22;
                        Vector2[] downPoints = downSlash.GetComponent<PolygonCollider2D>().points;
                        downSlashCollider.GetComponent<PolygonCollider2D>().points = downPoints;
                        var downAnim = downSlash.GetComponent<tk2dSpriteAnimator>();
                        float downLifetime = downAnim.DefaultClip.frames.Length / downAnim.ClipFps;
                        Destroy(downSlash, downLifetime);
                        Destroy(downSlashCollider, downLifetime);
                    }

                    break;
                case "UpSlash":
                    GameObject upSlash = Instantiate(_hc.upSlashPrefab, attacks.transform);
                    upSlash.SetActive(true);
                    var upNailSlash = upSlash.GetComponent<NailSlash>();
                    upNailSlash.SetMantis(GameManager.Players[id].equippedCharm_13);
                    if (GameManager.Players[id].equippedCharm_18 && GameManager.Players[id].equippedCharm_13)
                    {
                        upNailSlash.transform.localScale = new Vector3(upNailSlash.scale.x * 1.4f,
                            upNailSlash.scale.y * 1.4f, upNailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_13)
                    {
                        upNailSlash.transform.localScale = new Vector3(upNailSlash.scale.x * 1.25f,
                            upNailSlash.scale.y * 1.25f, upNailSlash.scale.z);
                    }
                    else if (GameManager.Players[id].equippedCharm_18)
                    {
                        upNailSlash.transform.localScale = new Vector3(upNailSlash.scale.x * 1.15f,
                            upNailSlash.scale.y * 1.15f, upNailSlash.scale.z);
                    }

                    upNailSlash.StartSlash();

                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject upSlashCollider =
                            Instantiate(MultiplayerClient.GameObjects["Slash"], upSlash.transform);
                        upSlashCollider.SetActive(true);
                        upSlashCollider.layer = 22;
                        Vector2[] upPoints = upSlash.GetComponent<PolygonCollider2D>().points;
                        upSlashCollider.GetComponent<PolygonCollider2D>().points = upPoints;
                        var upAnim = upSlash.GetComponent<tk2dSpriteAnimator>();
                        float upLifetime = upAnim.DefaultClip.frames.Length / upAnim.ClipFps;
                        Destroy(upSlash, upLifetime);
                        Destroy(upSlashCollider, upLifetime);
                    }

                    break;
                case "Wall Slash":
                    GameObject wallSlash = Instantiate(_hc.wallSlashPrefab, attacks.transform);
                    wallSlash.SetActive(true);
                    var wallNailSlash = wallSlash.GetComponent<NailSlash>();
                    wallNailSlash.SetMantis(GameManager.Players[id].equippedCharm_13);

                    wallNailSlash.StartSlash();

                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject wallSlashCollider =
                            Instantiate(MultiplayerClient.GameObjects["Slash"], wallSlash.transform);
                        wallSlashCollider.SetActive(true);
                        wallSlashCollider.layer = 22;
                        Vector2[] wallPoints = wallSlash.GetComponent<PolygonCollider2D>().points;
                        wallSlashCollider.GetComponent<PolygonCollider2D>().points = wallPoints;
                        var wallAnim = wallSlash.GetComponent<tk2dSpriteAnimator>();
                        float wallLifetime = wallAnim.DefaultClip.frames.Length / wallAnim.ClipFps;
                        Destroy(wallSlash, wallLifetime);
                        Destroy(wallSlashCollider, wallLifetime);
                    }

                    break;
                case "Fireball2 Cast":
                    GameObject fireballParent = _spellControl.GetAction<SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value;
                    PlayMakerFSM fireballCast = fireballParent.LocateMyFSM("Fireball Cast");
                    AudioClip castClip;
                    if (GameManager.Players[id].equippedCharm_11)
                    {
                        castClip = (AudioClip) fireballCast.GetAction<AudioPlayerOneShotSingle>("Fluke R", 0).audioClip
                            .Value;
                        if (GameManager.Players[id].equippedCharm_10)
                        {
                            GameObject dungFlukeObj = fireballCast.GetAction<SpawnObjectFromGlobalPool>("Dung R", 0)
                                .gameObject.Value;
                            GameObject dungFluke = Instantiate(dungFlukeObj, spells.transform.position,
                                Quaternion.identity);
                            dungFluke.SetActive(true);
                            dungFluke.transform.rotation = Quaternion.Euler(0, 0, 26 * -player.transform.localScale.x);
                            dungFluke.layer = 22;
                            PlayMakerFSM dungFlukeControl = dungFluke.LocateMyFSM("Control");
                            var blowClip = (AudioClip) dungFlukeControl.GetAction<AudioPlayerOneShotSingle>("Blow", 4).audioClip.Value;
                            Destroy(dungFluke.LocateMyFSM("Control"));
                            if (GameManager.Instance.pvpEnabled)
                            {
                                dungFluke.AddComponent<DamageHero>();
                            }
                            dungFluke.AddComponent<DungFluke>().blowClip = blowClip;
                            dungFluke.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(5, 15) * -player.transform.localScale.x, Random.Range(0, 20));
                            Destroy(dungFluke.LocateMyFSM("Control"));
                            Destroy(dungFluke.FindGameObjectInChildren("Damager"));
                        }
                        else
                        {
                            GameObject flukeObj = fireballCast.GetAction<FlingObjectsFromGlobalPool>("Flukes", 0).gameObject.Value;
                            for (int i = 0; i <= 15; i++)
                            {
                                GameObject fluke = Instantiate(flukeObj, spells.transform.position, Quaternion.identity);
                                fluke.SetActive(true);
                                fluke.layer = 22;
                                if (GameManager.Instance.pvpEnabled)
                                {
                                    fluke.AddComponent<DamageHero>();
                                }
                                fluke.AddComponent<SpellFluke>();
                                fluke.GetComponent<AudioSource>().Play();
                                fluke.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(5, 15) * -player.transform.localScale.x, Random.Range(0, 20));
                                fluke.GetComponent<SpriteFlash>().flashFocusHeal();
                                Destroy(fluke.GetComponent<SpellFluke>());
                                Destroy(fluke.FindGameObjectInChildren("Damager"));
                            }
                        }
                    }
                    else
                    {
                        castClip = (AudioClip) fireballCast.GetAction<AudioPlayerOneShotSingle>("Cast Right", 3).audioClip.Value;
                        GameObject fireballObj = fireballCast.GetAction<SpawnObjectFromGlobalPool>("Cast Right", 4).gameObject.Value;
                        GameObject fireball = Instantiate(fireballObj, spells.transform.position, Quaternion.identity);
                        fireball.SetActive(true);
                        fireball.layer = 22;
                        // Instantiating fireball and setting properties here is weird, so create a component for it.
                        var fireballComponent = fireball.AddComponent<Fireball>();
                        fireballComponent.playerId = id;
                        fireballComponent.xDir = -player.transform.localScale.x;
                    }

                    GameObject audioPlayer = fireballCast.GetAction<AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(castClip);
                    break;
                case "Quake Land 2":
                    GameObject qSlamObj = _hero.transform.Find("Spells").Find("Q Slam 2").gameObject;
                    GameObject quakeSlam = Instantiate(qSlamObj, spells.transform);
                    quakeSlam.SetActive(true);
                    quakeSlam.layer = 22;
                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject quakeHitL = quakeSlam.FindGameObjectInChildren("Hit L");
                        quakeHitL.AddComponent<DamageHero>();
                        GameObject quakeHitR = quakeSlam.FindGameObjectInChildren("Hit R");
                        quakeHitR.AddComponent<DamageHero>();    
                    }

                    yield return new WaitForSeconds(0.25f);
                    GameObject qPillarObj = _hero.transform.Find("Spells").Find("Q Pillar").gameObject;
                    GameObject quakePillar = Instantiate(qPillarObj, spells.transform);
                    quakePillar.SetActive(true);

                    GameObject qMegaObj = _hero.transform.Find("Spells").Find("Q Mega").gameObject;
                    GameObject qMega = Instantiate(qMegaObj, spells.transform);
                    qMega.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);
                    qMega.SetActive(true);
                    GameObject qMegaHitL = qMega.FindGameObjectInChildren("Hit L");
                    qMegaHitL.layer = 22;
                    Destroy(qMegaHitL.LocateMyFSM("damages_enemy"));
                    if (GameManager.Instance.pvpEnabled)
                    {
                        qMegaHitL.AddComponent<DamageHero>();
                    }
                    GameObject qMegaHitR = qMega.FindGameObjectInChildren("Hit R");
                    qMegaHitR.layer = 22;
                    Destroy(qMegaHitR.LocateMyFSM("damages_enemy"));
                    if (GameManager.Instance.pvpEnabled)
                    {
                        qMegaHitR.AddComponent<DamageHero>();
                    }

                    yield return new WaitForSeconds(1.0f);
                    
                    Destroy(quakeSlam);
                    Destroy(qPillarObj);
                    Destroy(qMegaObj);
                    
                    break;
                case "Scream 2":
                    Log("Scream 2");
                    AudioClip screamClip = (AudioClip) _spellControl.GetAction<AudioPlay>("Scream Antic2", 1).oneShotClip.Value;
                    GameObject scrHeadsObj = _hero.transform.Find("Spells").Find("Scr Heads 2").gameObject;
                    GameObject screamHeads = Instantiate(scrHeadsObj, spells.transform);
                    screamHeads.SetActive(true);
                    screamHeads.name = "Scream Heads Player " + GameManager.Players[id].username;
                    Destroy(screamHeads.LocateMyFSM("Deactivate on Hit"));
                    
                    GameObject screamHitL = screamHeads.FindGameObjectInChildren("Hit L");
                    Destroy(screamHitL.LocateMyFSM("damages_enemy"));
                    
                    GameObject screamHitR = screamHeads.FindGameObjectInChildren("Hit R");
                    Destroy(screamHitR.LocateMyFSM("damages_enemy"));
                    
                    GameObject screamHitU = screamHeads.FindGameObjectInChildren("Hit U");
                    Destroy(screamHitU.LocateMyFSM("damages_enemy"));

                    var screamHitLDamager = Instantiate(new GameObject("Hit L"), screamHitL.transform);
                    screamHitLDamager.layer = 22;
                    var screamHitLDmgPoly = screamHitLDamager.AddComponent<PolygonCollider2D>();
                    screamHitLDmgPoly.isTrigger = true;
                    var screamHitLPoly = screamHitL.GetComponent<PolygonCollider2D>(); 
                    screamHitLDmgPoly.points = screamHitLPoly.points;
                    if (GameManager.Instance.pvpEnabled)
                    {
                        screamHitLDamager.AddComponent<DamageHero>();
                    }

                    var screamHitRDamager = Instantiate(new GameObject("Hit R"), screamHitR.transform);
                    screamHitRDamager.layer = 22;
                    var screamHitRDmgPoly = screamHitRDamager.AddComponent<PolygonCollider2D>();
                    screamHitRDmgPoly.isTrigger = true;
                    var screamHitRPoly = screamHitR.GetComponent<PolygonCollider2D>();
                    screamHitRDmgPoly.points = screamHitRPoly.points;
                    if (GameManager.Instance.pvpEnabled)
                    {
                        screamHitRDamager.AddComponent<DamageHero>();
                    }
                    
                    var screamHitUDamager = Instantiate(new GameObject("Hit U"), screamHitU.transform);
                    screamHitUDamager.layer = 22;
                    var screamHitUDmgPoly = screamHitUDamager.AddComponent<PolygonCollider2D>();
                    screamHitUDmgPoly.isTrigger = true;
                    var screamHitUPoly = screamHitU.GetComponent<PolygonCollider2D>();
                    screamHitUDmgPoly.points = screamHitUPoly.points;
                    if (GameManager.Instance.pvpEnabled)
                    {
                        screamHitUDamager.AddComponent<DamageHero>();
                    }

                    Destroy(screamHitLPoly);
                    Destroy(screamHitRPoly);
                    Destroy(screamHitUPoly);
                    
                    yield return new WaitForSeconds(player.GetComponent<tk2dSpriteAnimator>().GetClipByName("Scream 2 Get").Duration);
                    
                    Destroy(screamHeads);
                    Destroy(screamHitLDamager);
                    Destroy(screamHitRDamager);
                    Destroy(screamHitUDamager);

                    break;
                case "NA Cyclone Start":
                    GameObject cycloneObj = _hc.transform.Find("Attacks").Find("Cyclone Slash").gameObject;
                    GameObject cycloneSlash = Instantiate(cycloneObj, attacks.transform);
                    cycloneSlash.SetActive(true);
                    cycloneSlash.layer = 22;
                    cycloneSlash.name = "Cyclone Slash " + GameManager.Players[id].username;
                    cycloneSlash.LocateMyFSM("Control Collider").SetState("Init");
                    GameObject hitL = cycloneSlash.FindGameObjectInChildren("Hit L");
                    GameObject hitR = cycloneSlash.FindGameObjectInChildren("Hit R");

                    GameObject cycHitLDamager = Instantiate(new GameObject("Cyclone Hit L"), hitL.transform);
                    cycHitLDamager.layer = 11;
                    if (GameManager.Instance.pvpEnabled)
                    {
                        cycHitLDamager.AddComponent<DamageHero>();
                        cycHitLDamager.AddComponent<TinkEffect>();
                        cycHitLDamager.AddComponent<TinkSound>();
                    }

                    var cycHitLDmgPoly = cycHitLDamager.AddComponent<PolygonCollider2D>();
                    cycHitLDmgPoly.isTrigger = true;
                    var hitLPoly = hitL.GetComponent<PolygonCollider2D>();
                    cycHitLDmgPoly.points = hitLPoly.points;

                    GameObject cycHitRDamager = Instantiate(new GameObject("Cyclone Hit R"), hitR.transform);
                    cycHitRDamager.layer = 11;
                    if (GameManager.Instance.pvpEnabled)
                    {
                        cycHitRDamager.AddComponent<DamageHero>();
                        cycHitRDamager.AddComponent<TinkEffect>();
                        cycHitRDamager.AddComponent<TinkSound>();
                    }
                    var cycHitRDmgPoly = cycHitRDamager.AddComponent<PolygonCollider2D>();
                    cycHitRDmgPoly.isTrigger = true;
                    var cycHitRPoly = hitR.GetComponent<PolygonCollider2D>();
                    cycHitRDmgPoly.points = cycHitRPoly.points;

                    Destroy(hitLPoly);
                    Destroy(cycHitRPoly);

                    break;
                case "NA Cyclone":
                    // So that this animation isn't included in default and immediately destroys the Cyclone Slash
                    break;
                case "NA Cyclone End":
                    if (GameObject.Find("Cyclone Slash " + GameManager.Players[id].username))
                    {
                        Destroy(GameObject.Find("Cyclone Slash " + GameManager.Players[id].username));
                    }

                    break;
                case "NA Big Slash":
                    GameObject gsObj = _hc.transform.Find("Attacks").Find("Great Slash").gameObject;
                    GameObject ds = Instantiate(_hc.transform.Find("Attacks").Find("Dash Slash").gameObject, attacks.transform.position, Quaternion.identity);
                    ds.SetActive(true);
                    var greatSlash = Instantiate(gsObj, attacks.transform);
                    greatSlash.SetActive(true);
                    greatSlash.layer = 22;
                    greatSlash.LocateMyFSM("Control Collider").SetState("Init");

                    var gsAnim = greatSlash.GetComponent<tk2dSpriteAnimator>();
                    float gsLifetime = gsAnim.DefaultClip.frames.Length / gsAnim.ClipFps;
                    Destroy(greatSlash, gsLifetime);
                    
                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject gsCollider =
                            Instantiate(MultiplayerClient.GameObjects["Slash"], greatSlash.transform);
                        gsCollider.SetActive(true);
                        gsCollider.layer = 22;
                        Vector2[] gsPoints = greatSlash.GetComponent<PolygonCollider2D>().points;
                        gsCollider.GetComponent<PolygonCollider2D>().points = gsPoints;
                        gsCollider.GetComponent<DamageHero>().damageDealt = 2;
                    }

                    break;
                case "NA Dash Slash":
                    GameObject dsObj = _hc.transform.Find("Attacks").Find("Dash Slash").gameObject;
                    var dashSlash = Instantiate(dsObj, attacks.transform);
                    dashSlash.SetActive(true);
                    dashSlash.layer = 22;
                    dashSlash.LocateMyFSM("Control Collider").SetState("Init");

                    var dsAnim = dashSlash.GetComponent<tk2dSpriteAnimator>();
                    float dsLifetime = dsAnim.DefaultClip.frames.Length / dsAnim.ClipFps;
                    Destroy(dashSlash, dsLifetime);

                    if (GameManager.Instance.pvpEnabled)
                    {
                        GameObject dsCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], dashSlash.transform);
                        dsCollider.SetActive(true);
                        dsCollider.layer = 22;
                        Vector2[] dsPoints = dashSlash.GetComponent<PolygonCollider2D>().points;
                        dsCollider.GetComponent<PolygonCollider2D>().points = dsPoints;
                        dsCollider.GetComponent<DamageHero>().damageDealt = 2;
                    }

                    break;
                default:
                    foreach (Transform childTransform in attacks.transform)
                    {
                        Destroy(childTransform.gameObject);
                    }
                    
                    foreach (Transform childTransform in effects.transform)
                    {
                        Destroy(childTransform.gameObject);
                    }
                    
                    foreach (Transform childTransform in spells.transform)
                    {
                        Destroy(childTransform.gameObject);
                    }

                    break;
            }
        }

        private void OnDestroy()
        {
            ModHooks.Instance.CharmUpdateHook -= OnCharmUpdate;
            ModHooks.Instance.SavegameSaveHook -= OnSavegameSave;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private static void Log(object message) => Modding.Logger.Log("[MP Client] " + message);
    }
}