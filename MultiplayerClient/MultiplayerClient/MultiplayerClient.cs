using System;
using System.Collections;
using System.Collections.Generic;
using ModCommon;
using Modding;
using MultiplayerClient.Canvas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerClient
{
    public class MultiplayerClient : Mod
    {
        public static readonly Dictionary<string, GameObject> GameObjects = new Dictionary<string, GameObject>();
        
        public override string GetVersion()
        {
            return "0.0.2";
        }
        
        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Hive_Knight", "Battle Scene/Globs/Hive Knight Glob"),
                ("GG_Hive_Knight", "Battle Scene/Hive Knight/Slash 1"),
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            GameObjects.Add("Glob", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Globs/Hive Knight Glob"]);
            GameObjects.Add("Slash", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Hive Knight/Slash 1"]);

            GUIController.Instance.BuildMenus();

            global::GameManager.instance.gameObject.AddComponent<MPClient>();

            Unload();
            
            ModHooks.Instance.BeforeSavegameSaveHook += RespawnFix;
            ModHooks.Instance.CharmUpdateHook += OnCharmUpdate;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
            
        }
        
        private void RespawnFix(SaveGameData data)
        {
            PlayerData playerData = data.playerData;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name.Contains("RestBench") || 
                    go.name.Contains("WhiteBench") ||
                    go.name.Contains("Death Respawn"))
                {
                    playerData.respawnScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    playerData.respawnMarkerName = go.name;

                    break;
                }
            }
        }
        
        private void OnCharmUpdate(PlayerData pd, HeroController hc)
        {
            if (Client.Instance != null && Client.Instance.isConnected && pd != null && hc != null)
            {
                ClientSend.CharmsUpdated(pd);
            }
        }

        private void Unload()
        {
            ModHooks.Instance.BeforeSavegameSaveHook -= RespawnFix;
            ModHooks.Instance.CharmUpdateHook -= OnCharmUpdate;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        public static List<string> NonGameplayScenes = new List<string>
        {
            "BetaEnd",
            "Cinematic_Stag_travel",
            "Cinematic_Ending_A",
            "Cinematic_Ending_B",
            "Cinematic_Ending_C",
            "Cinematic_Ending_D",
            "Cinematic_Ending_E",
            "Cinematic_MrMushroom",
            "Cutscene_Boss_Door",
            "End_Credits",
            "End_Game_Completion",
            "GG_Boss_Door_Entrance",
            "GG_End_Sequence",
            "GG_Entrance_Cutscene",
            "GG_Unlock",
            "Intro_Cutscene",
            "Intro_Cutscene_Prologue",
            "Knight Pickup",
            "Menu_Title",
            "Menu_Credits",
            "Opening_Sequence",
            "PermaDeath_Unlock",
            "Pre_Menu_Intro",
            "PermaDeath",
            "Prologue_Excerpt",
        };
        private void OnSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (Client.Instance.isConnected)
            {
                if (!NonGameplayScenes.Contains(nextScene.name))
                {
                    ClientSend.SceneChanged(nextScene.name);
                }
            }
            
            GameManager.Instance.DestroyAllPlayers();
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Multiplayer Client] " + message);
    }
}