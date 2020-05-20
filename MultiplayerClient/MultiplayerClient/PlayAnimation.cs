using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MultiplayerClient
{
    internal partial class MPClient
    {
        public IEnumerator PlayAnimation(byte id, string animation)
        {
            PlayerManager playerManager = SessionManager.Instance.Players[id];
            GameObject player = playerManager.gameObject;

            var materialPropertyBlock = new MaterialPropertyBlock();
            MeshRenderer mRend;

            Dictionary<string, Texture2D> texDict = SessionManager.Instance.PlayerTextures[id];
            Texture2D baldurTex = texDict["Baldur"];
            Texture2D flukeTex = texDict["Fluke"];
            Texture2D grimmTex = texDict["Grimm"];
            Texture2D hatchlingTex = texDict["Hatchling"];
            Texture2D knightTex = texDict["Knight"];
            Texture2D shieldTex = texDict["Shield"];
            Texture2D sprintTex = texDict["Sprint"];
            Texture2D unnTex = texDict["Unn"];
            Texture2D voidTex = texDict["Void"];
            Texture2D vsTex = texDict["VS"];
            Texture2D weaverTex = texDict["Weaver"];
            Texture2D wraithsTex = texDict["Wraiths"];
            
            GameObject playerAttacks = player.FindGameObjectInChildren("Attacks");
            GameObject playerEffects = player.FindGameObjectInChildren("Effects");
            GameObject playerSpells = player.FindGameObjectInChildren("Spells");
            
            GameObject heroAttacks = _hero.FindGameObjectInChildren("Attacks");
            GameObject heroEffects = _hero.FindGameObjectInChildren("Effects");
            GameObject heroSpells = _hero.FindGameObjectInChildren("Spells");

            BoxCollider2D collider;
            
            GameObject sdTrail = null;
            GameObject qCharge = null;
            GameObject qTrail2 = null;

            GameObject fireballParent = _spellControl.GetAction<SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value;
            PlayMakerFSM fireballCast = fireballParent.LocateMyFSM("Fireball Cast");
            GameObject audioPlayerObj = fireballCast.GetAction<AudioPlayerOneShotSingle>("Cast Right", 3).audioPlayer.Value;
            GameObject audioPlayer;

            tk2dSpriteAnimator playerAnim = player.GetComponent<tk2dSpriteAnimator>();
            switch (animation)
            {
                case "SD Dash":
                    GameObject sdBurst = Instantiate(heroEffects.FindGameObjectInChildren("SD Burst"), playerEffects.transform);
                    sdBurst.SetActive(true);

                    sdBurst.LocateMyFSM("FSM").InsertMethod("Destroy", 1, () => Destroy(sdBurst));
                    
                    sdTrail = Instantiate(heroEffects.FindGameObjectInChildren("SD Trail"), playerEffects.transform);
                    sdTrail.SetActive(true);

                    sdTrail.name = "SD Trail " + id;
                    PlayMakerFSM sdTrailFsm = sdTrail.LocateMyFSM("FSM");
                    sdTrailFsm.SetState("Idle");
                    sdTrailFsm.InsertMethod("Destroy", 1, () => Destroy(sdTrail));
                    sdTrail.GetComponent<MeshRenderer>().enabled = true;
                    tk2dSpriteAnimator trailAnim = sdTrail.GetComponent<tk2dSpriteAnimator>();
                    trailAnim.PlayFromFrame("SD Trail", 0);

                    GameObject sdBurstGlow = Instantiate(heroEffects.FindGameObjectInChildren("SD Burst Glow"), playerEffects.transform);
                    sdBurstGlow.SetActive(true);
                    
                    if (knightTex != null)
                    {
                        mRend = sdBurst.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    
                        mRend = sdTrail.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                        
                        mRend = sdBurstGlow.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }
                    
                    
                    break;
                case "Air Cancel":
                    GameObject sdBreak = Instantiate(heroEffects.FindGameObjectInChildren("SD Break"), playerEffects.transform);
                    Destroy(sdBreak, 0.54f);

                    if (knightTex != null)
                    {
                        mRend = sdBreak.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    sdTrail.GetComponent<tk2dSpriteAnimator>().Play("SD Trail End");

                    break;
                case "SD Hit Wall":
                    sdTrail.GetComponent<tk2dSpriteAnimator>().Play("SD Trail End");
                    
                    GameObject wallHitEffect = Instantiate(heroEffects.FindGameObjectInChildren("Wall Hit Effect"), playerEffects.transform);

                    if (knightTex != null)
                    {
                        mRend = wallHitEffect.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    wallHitEffect.LocateMyFSM("FSM").InsertMethod("Destroy", 1, () => Destroy(wallHitEffect));
                    
                    break;
                case "Slash":
                    GameObject slash = Instantiate(_hc.slashPrefab, playerAttacks.transform);
                    slash.SetActive(true);

                    if (knightTex != null)
                    {
                        mRend = slash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    AudioSource slashAudioSource = slash.GetComponent<AudioSource>();
                    AudioClip slashClip = slashAudioSource.clip;
                    Destroy(slashAudioSource);
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(slashClip);
                    
                    bool fury = playerManager.equippedCharm_6 && playerManager.health == 1;
                    
                    NailSlash nailSlash = slash.GetComponent<NailSlash>();
                    nailSlash.SetMantis(playerManager.equippedCharm_13);
                    nailSlash.SetFury(playerManager.equippedCharm_6 && playerManager.health == 1);
                    if (playerManager.equippedCharm_18 && playerManager.equippedCharm_13)
                    {
                        nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.4f, nailSlash.scale.y * 1.4f, nailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_13)
                    {
                        nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.25f, nailSlash.scale.y * 1.25f, nailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_18)
                    {
                        nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.15f, nailSlash.scale.y * 1.15f, nailSlash.scale.z);
                    }

                    nailSlash.StartSlash();

                    if (SessionManager.Instance.PvPEnabled)
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

                    if (playerManager.equippedCharm_35)
                    {
                        GameObject elegyBeam = null;
                        int health = playerManager.health;
                        int maxHealth = playerManager.maxHealth;
                        if (player.transform.localScale.x > 0)
                        {
                            if (health == 1 && playerManager.equippedCharm_6)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabL_fury;
                            }
                            else if (health == maxHealth)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabL;
                            }
                        }
                        else if (player.transform.localScale.x < 0)
                        {
                            if (health == 1 && playerManager.equippedCharm_6)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabR_fury;
                            }
                            else if (health == maxHealth)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabR;
                            }
                        }

                        if (elegyBeam != null)
                        {
                            GameObject beam = Instantiate(elegyBeam, player.transform.position, Quaternion.identity);
                            beam.SetActive(true);
                            beam.layer = 22;
                            Destroy(beam.LocateMyFSM("damages_enemy"));
                            if (SessionManager.Instance.PvPEnabled)
                            {
                                beam.AddComponent<DamageHero>().damageDealt = fury && playerManager.equippedCharm_25 ? 2 : 1;
                            }
                        }
                    }

                    break;
                case "SlashAlt":
                    GameObject altSlash = Instantiate(_hc.slashAltPrefab, playerAttacks.transform);
                    altSlash.SetActive(true);

                    if (knightTex != null)
                    {
                        mRend = altSlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    AudioSource altSlashAudioSource = altSlash.GetComponent<AudioSource>();
                    AudioClip altSlashClip = altSlashAudioSource.clip;
                    Destroy(altSlashAudioSource);
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(altSlashClip);
                    
                    bool furyAlt = playerManager.equippedCharm_6 && playerManager.health == 1;
                    
                    var altNailSlash = altSlash.GetComponent<NailSlash>();
                    altNailSlash.SetMantis(playerManager.equippedCharm_13);
                    altNailSlash.SetFury(playerManager.equippedCharm_6 && playerManager.health == 1);
                    if (playerManager.equippedCharm_18 && playerManager.equippedCharm_13)
                    {
                        altNailSlash.transform.localScale = new Vector3(altNailSlash.scale.x * 1.4f, altNailSlash.scale.y * 1.4f, altNailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_13)
                    {
                        altNailSlash.transform.localScale = new Vector3(altNailSlash.scale.x * 1.25f, altNailSlash.scale.y * 1.25f, altNailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_18)
                    {
                        altNailSlash.transform.localScale = new Vector3(altNailSlash.scale.x * 1.15f, altNailSlash.scale.y * 1.15f, altNailSlash.scale.z);
                    }

                    altNailSlash.StartSlash();

                    if (SessionManager.Instance.PvPEnabled)
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

                    if (playerManager.equippedCharm_35)
                    {
                        GameObject elegyBeam = null;
                        int health = playerManager.health;
                        int maxHealth = playerManager.maxHealth;
                        if (player.transform.localScale.x > 0)
                        {
                            if (health == 1 && playerManager.equippedCharm_6)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabL_fury;
                            }
                            else if (health == maxHealth)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabL;
                            }
                        }
                        else if (player.transform.localScale.x < 0)
                        {
                            if (health == 1 && playerManager.equippedCharm_6)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabR_fury;
                            }
                            else if (health == maxHealth)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabR;
                            }
                        }

                        if (elegyBeam != null)
                        {
                            GameObject beam = Instantiate(elegyBeam, player.transform.position, Quaternion.identity);
                            beam.SetActive(true);
                            beam.layer = 22;
                            Destroy(beam.LocateMyFSM("damages_enemy"));
                            if (SessionManager.Instance.PvPEnabled)
                            {
                                beam.AddComponent<DamageHero>().damageDealt = furyAlt && playerManager.equippedCharm_25 ? 2 : 1;
                            }
                        }
                    }
                    
                    break;
                case "DownSlash":
                    GameObject downSlash = Instantiate(_hc.downSlashPrefab, playerAttacks.transform);
                    downSlash.SetActive(true);

                    if (knightTex != null)
                    {
                        mRend = downSlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    AudioSource downSlashAudioSource = downSlash.GetComponent<AudioSource>();
                    AudioClip downSlashClip = downSlashAudioSource.clip;
                    Destroy(downSlashAudioSource);
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(downSlashClip);

                    bool furyDown = playerManager.equippedCharm_6 && playerManager.health == 1;
                    
                    var downNailSlash = downSlash.GetComponent<NailSlash>();
                    downNailSlash.SetMantis(playerManager.equippedCharm_13);
                    downNailSlash.SetFury(playerManager.equippedCharm_6 && playerManager.health == 1);
                    if (playerManager.equippedCharm_18 && playerManager.equippedCharm_13)
                    {
                        downNailSlash.transform.localScale = new Vector3(downNailSlash.scale.x * 1.4f, downNailSlash.scale.y * 1.4f, downNailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_13)
                    {
                        downNailSlash.transform.localScale = new Vector3(downNailSlash.scale.x * 1.25f, downNailSlash.scale.y * 1.25f, downNailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_18)
                    {
                        downNailSlash.transform.localScale = new Vector3(downNailSlash.scale.x * 1.15f, downNailSlash.scale.y * 1.15f, downNailSlash.scale.z);
                    }

                    downNailSlash.StartSlash();

                    if (SessionManager.Instance.PvPEnabled)
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

                    if (playerManager.equippedCharm_35)
                    {
                        GameObject elegyBeam = null;
                        int health = playerManager.health;
                        int maxHealth = playerManager.maxHealth;
                        if (health == 1 && playerManager.equippedCharm_6)
                        {
                            elegyBeam = _hc.grubberFlyBeamPrefabD_fury;
                        }
                        else if (health == maxHealth)
                        {
                            elegyBeam = _hc.grubberFlyBeamPrefabD;
                        }

                        if (elegyBeam != null)
                        {
                            GameObject beam = Instantiate(elegyBeam, player.transform.position, Quaternion.identity);
                            beam.SetActive(true);
                            beam.layer = 22;
                            Vector3 ls = beam.transform.localScale;
                            beam.transform.localScale = new Vector3(player.transform.localScale.x, ls.y, ls.z);
                            beam.transform.rotation = Quaternion.Euler(0, 0, beam.transform.localScale.x > 0 ? 90 : -90);
                            Destroy(beam.LocateMyFSM("damages_enemy"));
                            if (SessionManager.Instance.PvPEnabled)
                            {
                                beam.AddComponent<DamageHero>().damageDealt = furyDown && playerManager.equippedCharm_25 ? 2 : 1;
                            }
                        }
                    }
                    
                    break;
                case "UpSlash":
                    GameObject upSlash = Instantiate(_hc.upSlashPrefab, playerAttacks.transform);
                    upSlash.SetActive(true);

                    if (knightTex != null)
                    {
                        mRend = upSlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    AudioSource upSlashAudioSource = upSlash.GetComponent<AudioSource>();
                    AudioClip upSlashClip = upSlashAudioSource.clip;
                    Destroy(upSlashAudioSource);
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(upSlashClip);

                    bool furyUp = playerManager.equippedCharm_6 && playerManager.health == 1;
                    
                    var upNailSlash = upSlash.GetComponent<NailSlash>();
                    upNailSlash.SetMantis(playerManager.equippedCharm_13);
                    upNailSlash.SetFury(playerManager.equippedCharm_6 && playerManager.health == 1);
                    if (playerManager.equippedCharm_18 && playerManager.equippedCharm_13)
                    {
                        upNailSlash.transform.localScale = new Vector3(upNailSlash.scale.x * 1.4f, upNailSlash.scale.y * 1.4f, upNailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_13)
                    {
                        upNailSlash.transform.localScale = new Vector3(upNailSlash.scale.x * 1.25f, upNailSlash.scale.y * 1.25f, upNailSlash.scale.z);
                    }
                    else if (playerManager.equippedCharm_18)
                    {
                        upNailSlash.transform.localScale = new Vector3(upNailSlash.scale.x * 1.15f, upNailSlash.scale.y * 1.15f, upNailSlash.scale.z);
                    }

                    upNailSlash.StartSlash();

                    if (SessionManager.Instance.PvPEnabled)
                    {
                        GameObject upSlashCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], upSlash.transform);
                        upSlashCollider.SetActive(true);
                        upSlashCollider.layer = 22;
                        upSlashCollider.GetComponent<DamageHero>().damageDealt = playerManager.equippedCharm_6 && playerManager.health == 1 ? 2 : 1;
                        Vector2[] upPoints = upSlash.GetComponent<PolygonCollider2D>().points;
                        upSlashCollider.GetComponent<PolygonCollider2D>().points = upPoints;
                        var upAnim = upSlash.GetComponent<tk2dSpriteAnimator>();
                        float upLifetime = upAnim.DefaultClip.frames.Length / upAnim.ClipFps;
                        Destroy(upSlash, upLifetime);
                        Destroy(upSlashCollider, upLifetime);
                    }

                    if (playerManager.equippedCharm_35)
                    {
                        GameObject elegyBeam = null;
                        int health = playerManager.health;
                        int maxHealth = playerManager.maxHealth;
                        if (health == 1 && playerManager.equippedCharm_6)
                        {
                            elegyBeam = _hc.grubberFlyBeamPrefabU_fury;
                        }
                        else if (health == maxHealth)
                        {
                            elegyBeam = _hc.grubberFlyBeamPrefabU;
                        }
                        
                        if (elegyBeam != null)
                        {
                            GameObject beam = Instantiate(elegyBeam, player.transform.position, Quaternion.identity);
                            beam.SetActive(true);
                            beam.layer = 22;
                            Vector3 ls = beam.transform.localScale;
                            beam.transform.localScale = new Vector3(player.transform.localScale.x, ls.y, ls.z);
                            beam.transform.rotation = Quaternion.Euler(0, 0, beam.transform.localScale.x > 0 ? -90 : 90);
                            Destroy(beam.LocateMyFSM("damages_enemy"));
                            if (SessionManager.Instance.PvPEnabled)
                            {
                                beam.AddComponent<DamageHero>().damageDealt = furyUp && playerManager.equippedCharm_25 ? 2 : 1;
                            }
                        }
                    }
                    
                    break;
                case "Wall Slash":
                    GameObject wallSlash = Instantiate(_hc.wallSlashPrefab, playerAttacks.transform);
                    wallSlash.SetActive(true);

                    if (knightTex != null)
                    {
                        mRend = wallSlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    AudioSource wallSlashAudioSource = wallSlash.GetComponent<AudioSource>();
                    AudioClip wallSlashClip = wallSlashAudioSource.clip;
                    Destroy(wallSlashAudioSource);
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(wallSlashClip);
                    
                    bool furyWall = playerManager.equippedCharm_6 && playerManager.health == 1;
                    
                    var wallNailSlash = wallSlash.GetComponent<NailSlash>();
                    wallNailSlash.SetMantis(playerManager.equippedCharm_13);
                    wallNailSlash.SetFury(playerManager.equippedCharm_6 && playerManager.health == 1);

                    wallNailSlash.StartSlash();

                    if (SessionManager.Instance.PvPEnabled)
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

                    if (playerManager.equippedCharm_35)
                    {
                        GameObject elegyBeam = null;
                        int health = playerManager.health;
                        int maxHealth = playerManager.maxHealth;
                        if (transform.localScale.x > 0)
                        {
                            if (health == 1 && playerManager.equippedCharm_6)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabL_fury;
                            }
                            else if (health == maxHealth)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabL;
                            }
                        }
                        else if (transform.localScale.x < 0)
                        {
                            if (health == 1)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabR_fury;
                            }
                            else if (health == maxHealth)
                            {
                                elegyBeam = _hc.grubberFlyBeamPrefabR;
                            }
                        }

                        if (elegyBeam != null)
                        {
                            GameObject beam = Instantiate(elegyBeam, player.transform.position, Quaternion.identity);
                            beam.SetActive(true);
                            beam.layer = 22;
                            Destroy(beam.LocateMyFSM("damages_enemy"));
                            if (SessionManager.Instance.PvPEnabled)
                            {
                                beam.AddComponent<DamageHero>().damageDealt = furyWall && playerManager.equippedCharm_25 ? 2 : 1;
                            }
                        }
                    }
                    
                    break;
                case "Fireball2 Cast":
                    AudioClip castClip;
                    if (playerManager.equippedCharm_11)
                    {
                        castClip = (AudioClip) fireballCast.GetAction<AudioPlayerOneShotSingle>("Fluke R", 0).audioClip.Value;
                        if (playerManager.equippedCharm_10)
                        {
                            GameObject dungFlukeObj = fireballCast.GetAction<SpawnObjectFromGlobalPool>("Dung R", 0)
                                .gameObject.Value;
                            GameObject dungFluke = Instantiate(dungFlukeObj, playerSpells.transform.position,
                                Quaternion.identity);
                            dungFluke.SetActive(true);
                            dungFluke.transform.rotation = Quaternion.Euler(0, 0, 26 * -player.transform.localScale.x);
                            dungFluke.layer = 22;
                            PlayMakerFSM dungFlukeControl = dungFluke.LocateMyFSM("Control");
                            var blowClip = (AudioClip) dungFlukeControl.GetAction<AudioPlayerOneShotSingle>("Blow", 4).audioClip.Value;
                            Destroy(dungFluke.LocateMyFSM("Control"));
                            if (SessionManager.Instance.PvPEnabled)
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
                            GameObject fluke = Instantiate(flukeObj, playerSpells.transform.position, Quaternion.identity);
                            if (SessionManager.Instance.PvPEnabled)
                            {
                                fluke.AddComponent<DamageHero>();
                            }
                            
                            FlingUtils.Config config = new FlingUtils.Config();
                            config.Prefab = fluke;
                            config.AmountMin = 16;
                            config.AmountMax = 16;
                            config.AngleMin = player.transform.localScale.x < 0 ? 20 : 120;
                            config.AngleMax = player.transform.localScale.x < 0 ? 60 : 160;
                            config.SpeedMin = 15;
                            config.SpeedMax = 21;
                            FlingUtils.SpawnAndFling(config, player.transform, Vector3.zero);
                        }
                    }
                    else
                    {
                        castClip = (AudioClip) fireballCast.GetAction<AudioPlayerOneShotSingle>("Cast Right", 3).audioClip.Value;
                        GameObject fireballObj = fireballCast.GetAction<SpawnObjectFromGlobalPool>("Cast Right", 4).gameObject.Value;
                        GameObject fireball = Instantiate(fireballObj, playerSpells.transform.position  + Vector3.down * 0.5f, Quaternion.identity);
                        fireball.SetActive(true);
                        fireball.layer = 22;

                        if (voidTex != null)
                        {
                            mRend = fireball.GetComponent<MeshRenderer>();
                            mRend.GetPropertyBlock(materialPropertyBlock);
                            materialPropertyBlock.SetTexture("_MainTex", voidTex);
                            mRend.SetPropertyBlock(materialPropertyBlock);
                        }

                        // Instantiating fireball and setting properties here is weird, so create a component for it.
                        var fireballComponent = fireball.AddComponent<Fireball>();
                        fireballComponent.playerId = id;
                        fireballComponent.xDir = -player.transform.localScale.x;
                    }
                    
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(castClip);
                    
                    break;
                case "Quake Antic":
                    AudioClip quakeAnticClip = (AudioClip) _spellControl.GetAction<AudioPlay>("Quake Antic", 0).oneShotClip.Value;
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(quakeAnticClip);
                    
                    qCharge = Instantiate(heroSpells.FindGameObjectInChildren("Q Charge"), playerSpells.transform);
                    qCharge.SetActive(true);
                    qCharge.name = "Q Charge " + id;

                    if (vsTex != null)
                    {
                        mRend = qCharge.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", vsTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    qCharge.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);
                    
                    break;
                case "Quake Fall 2":
                    GameObject qFlashStart = Instantiate(heroSpells.FindGameObjectInChildren("Q Flash Start"), playerSpells.transform);
                    GameObject sdSharpFlash = Instantiate(heroEffects.FindGameObjectInChildren("SD Sharp Flash"), playerEffects.transform);
                    qTrail2 = Instantiate(heroSpells.FindGameObjectInChildren("Q Trail 2"), playerSpells.transform);

                    qFlashStart.SetActive(true);
                    sdSharpFlash.SetActive(true);
                    qTrail2.SetActive(true);

                    if (voidTex != null)
                    {
                        mRend = qFlashStart.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", voidTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);

                        mRend = sdSharpFlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", voidTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);

                        mRend = qTrail2.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", voidTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    Destroy(qFlashStart, 1);
                    Destroy(sdSharpFlash, 1);
                    if (qCharge != null)
                    {
                        Destroy(qCharge);
                    }
                    
                    break;
                case "Quake Land 2":
                    AudioClip q2LandClip = (AudioClip) _spellControl.GetAction<AudioPlay>("Q2 Land", 1).oneShotClip.Value;
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(q2LandClip);
                    
                    if (qTrail2 != null)
                    {
                        Destroy(qTrail2);
                    }
                    
                    GameObject qSlamObj = heroSpells.FindGameObjectInChildren("Q Slam 2");
                    GameObject quakeSlam = Instantiate(qSlamObj, playerSpells.transform);
                    quakeSlam.SetActive(true);
                    quakeSlam.layer = 22;

                    if (voidTex != null)
                    {
                        mRend = quakeSlam.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", voidTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    if (SessionManager.Instance.PvPEnabled)
                    {
                        GameObject quakeHitL = quakeSlam.FindGameObjectInChildren("Hit L");
                        quakeHitL.AddComponent<DamageHero>();
                        GameObject quakeHitR = quakeSlam.FindGameObjectInChildren("Hit R");
                        quakeHitR.AddComponent<DamageHero>();    
                    }

                    yield return new WaitForSeconds(0.25f);
                    GameObject qPillarObj = heroSpells.FindGameObjectInChildren("Q Pillar");
                    GameObject quakePillar = Instantiate(qPillarObj, playerSpells.transform);
                    quakePillar.SetActive(true);

                    GameObject qMegaObj = heroSpells.FindGameObjectInChildren("Q Mega");
                    GameObject qMega = Instantiate(qMegaObj, playerSpells.transform);
                    qMega.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);
                    qMega.SetActive(true);
                    
                    if (voidTex != null)
                    {
                        mRend = quakePillar.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", voidTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                        
                        mRend = qMega.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", voidTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }
                    
                    GameObject qMegaHitL = qMega.FindGameObjectInChildren("Hit L");
                    qMegaHitL.layer = 22;
                    Destroy(qMegaHitL.LocateMyFSM("damages_enemy"));
                    if (SessionManager.Instance.PvPEnabled)
                    {
                        qMegaHitL.AddComponent<DamageHero>();
                    }
                    GameObject qMegaHitR = qMega.FindGameObjectInChildren("Hit R");
                    qMegaHitR.layer = 22;
                    Destroy(qMegaHitR.LocateMyFSM("damages_enemy"));
                    if (SessionManager.Instance.PvPEnabled)
                    {
                        qMegaHitR.AddComponent<DamageHero>();
                    }

                    yield return new WaitForSeconds(1.0f);
                    
                    Destroy(quakeSlam);
                    Destroy(quakePillar);
                    Destroy(qMega);
                    
                    break;
                case "Scream 2":
                    AudioClip screamClip = (AudioClip) _spellControl.GetAction<AudioPlay>("Scream Antic2", 1).oneShotClip.Value;
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(screamClip);

                    GameObject scrHeadsObj = heroSpells.FindGameObjectInChildren("Scr Heads 2");
                    GameObject screamHeads = Instantiate(scrHeadsObj, playerSpells.transform);
                    screamHeads.SetActive(true);
                    screamHeads.name = "Scream Heads Player " + id;

                    if (voidTex != null)
                    {
                        mRend = screamHeads.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", voidTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

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
                    if (SessionManager.Instance.PvPEnabled)
                    {
                        screamHitLDamager.AddComponent<DamageHero>();
                    }

                    var screamHitRDamager = Instantiate(new GameObject("Hit R"), screamHitR.transform);
                    screamHitRDamager.layer = 22;
                    var screamHitRDmgPoly = screamHitRDamager.AddComponent<PolygonCollider2D>();
                    screamHitRDmgPoly.isTrigger = true;
                    var screamHitRPoly = screamHitR.GetComponent<PolygonCollider2D>();
                    screamHitRDmgPoly.points = screamHitRPoly.points;
                    if (SessionManager.Instance.PvPEnabled)
                    {
                        screamHitRDamager.AddComponent<DamageHero>();
                    }
                    
                    var screamHitUDamager = Instantiate(new GameObject("Hit U"), screamHitU.transform);
                    screamHitUDamager.layer = 22;
                    var screamHitUDmgPoly = screamHitUDamager.AddComponent<PolygonCollider2D>();
                    screamHitUDmgPoly.isTrigger = true;
                    var screamHitUPoly = screamHitU.GetComponent<PolygonCollider2D>();
                    screamHitUDmgPoly.points = screamHitUPoly.points;
                    if (SessionManager.Instance.PvPEnabled)
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
                    
                    break;
                case "NA Cyclone":
                    AudioClip cycloneClip = (AudioClip) _nailArts.GetAction<AudioPlayerOneShotSingle>("Play Audio", 0).audioClip.Value;
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(cycloneClip);
                    
                    GameObject cycloneObj = heroAttacks.FindGameObjectInChildren("Cyclone Slash");
                    GameObject cycloneSlash = Instantiate(cycloneObj, playerAttacks.transform);
                    cycloneSlash.SetActive(true);
                    cycloneSlash.layer = 22;
                    cycloneSlash.name = "Cyclone Slash " + id;

                    if (knightTex != null)
                    {
                        mRend = cycloneSlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    cycloneSlash.LocateMyFSM("Control Collider").SetState("Init");
                    GameObject hitL = cycloneSlash.FindGameObjectInChildren("Hit L");
                    GameObject hitR = cycloneSlash.FindGameObjectInChildren("Hit R");

                    GameObject cycHitLDamager = Instantiate(new GameObject("Cyclone Hit L"), hitL.transform);
                    cycHitLDamager.layer = 11;
                    if (SessionManager.Instance.PvPEnabled)
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
                    if (SessionManager.Instance.PvPEnabled)
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
                case "NA Cyclone End":
                    GameObject cSlash = GameObject.Find("Cyclone Slash " + id);
                    if (cSlash != null)
                    {
                        Destroy(cSlash);
                    }

                    break;
                case "NA Big Slash":
                    AudioClip gsClip = (AudioClip) _nailArts.GetAction<AudioPlay>("G Slash", 0).oneShotClip.Value;
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(gsClip);
                    
                    GameObject gsObj = heroAttacks.FindGameObjectInChildren("Great Slash");
                    var greatSlash = Instantiate(gsObj, playerAttacks.transform);
                    greatSlash.SetActive(true);
                    greatSlash.layer = 22;

                    if (knightTex != null)
                    {
                        mRend = greatSlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    greatSlash.LocateMyFSM("Control Collider").SetState("Init");

                    var gsAnim = greatSlash.GetComponent<tk2dSpriteAnimator>();
                    float gsLifetime = gsAnim.DefaultClip.frames.Length / gsAnim.ClipFps;
                    Destroy(greatSlash, gsLifetime);
                    
                    if (SessionManager.Instance.PvPEnabled)
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
                    AudioClip dsClip = (AudioClip) _nailArts.GetAction<AudioPlay>("Dash Slash", 1).oneShotClip.Value;
                    audioPlayer = audioPlayerObj.Spawn(player.transform);
                    audioPlayer.GetComponent<AudioSource>().PlayOneShot(dsClip);
                    
                    GameObject dsObj = heroAttacks.FindGameObjectInChildren("Dash Slash");
                    var dashSlash = Instantiate(dsObj, playerAttacks.transform);
                    dashSlash.SetActive(true);
                    dashSlash.layer = 22;

                    if (knightTex != null)
                    {
                        mRend = dashSlash.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    dashSlash.LocateMyFSM("Control Collider").SetState("Init");

                    var dsAnim = dashSlash.GetComponent<tk2dSpriteAnimator>();
                    float dsLifetime = dsAnim.DefaultClip.frames.Length / dsAnim.ClipFps;
                    Destroy(dashSlash, dsLifetime);

                    if (SessionManager.Instance.PvPEnabled)
                    {
                        GameObject dsCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], dashSlash.transform);
                        dsCollider.SetActive(true);
                        dsCollider.layer = 22;
                        Vector2[] dsPoints = dashSlash.GetComponent<PolygonCollider2D>().points;
                        dsCollider.GetComponent<PolygonCollider2D>().points = dsPoints;
                        dsCollider.GetComponent<DamageHero>().damageDealt = 2;
                    }

                    break;
                case "Shadow Dash":
                    collider = player.GetComponent<BoxCollider2D>();
                    collider.enabled = false;
                    yield return new WaitForSeconds(playerAnim.GetClipByName("Shadow Dash").Duration);
                    collider.enabled = true;
                    
                    break;
                case "Shadow Dash Down":
                    collider = player.GetComponent<BoxCollider2D>();
                    collider.enabled = false;
                    yield return new WaitForSeconds(playerAnim.GetClipByName("Shadow Dash Down").Duration);
                    collider.enabled = true;
                    
                    break;
                case "Recoil":
                    player.GetComponent<SpriteFlash>().flashFocusHeal();
                    
                    GameObject damageEffect = heroEffects.FindGameObjectInChildren("Damage Effect");
                    GameObject hitCrack = Instantiate(damageEffect.FindGameObjectInChildren("Hit Crack"), playerEffects.transform);
                    GameObject hitPt1 = Instantiate(damageEffect.FindGameObjectInChildren("Hit Pt 1"), playerEffects.transform);
                    GameObject hitPt2 = Instantiate(damageEffect.FindGameObjectInChildren("Hit Pt 2"), playerEffects.transform);

                    hitCrack.SetActive(true);
                    hitPt1.SetActive(true);
                    hitPt2.SetActive(true);

                    if (knightTex != null)
                    {
                        mRend = hitCrack.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    hitPt1.GetComponent<ParticleSystem>().Play();
                    hitPt2.GetComponent<ParticleSystem>().Play();

                    Destroy(hitCrack, 1);
                    Destroy(hitPt1, 1);
                    Destroy(hitPt2, 1);

                    break;
                case string slug when animation.StartsWith("Slug"):
                    if (unnTex != null)
                    {
                        mRend = player.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", unnTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    break;
                case "Sprint":
                case string dg when animation.StartsWith("DG"):
                    if (sprintTex != null)
                    {
                        mRend = player.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", sprintTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    break;
                default:
                    foreach (Transform childTransform in playerAttacks.transform)
                    {
                        Destroy(childTransform.gameObject);
                    }
                    
                    foreach (Transform childTransform in playerEffects.transform)
                    {
                        Destroy(childTransform.gameObject);
                    }
                    
                    foreach (Transform childTransform in playerSpells.transform)
                    {
                        Destroy(childTransform.gameObject);
                    }

                    if (knightTex != null)
                    {
                        mRend = player.GetComponent<MeshRenderer>();
                        mRend.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetTexture("_MainTex", knightTex);
                        mRend.SetPropertyBlock(materialPropertyBlock);
                    }

                    GC.Collect();
                    
                    break;
            }
        }
    }
}