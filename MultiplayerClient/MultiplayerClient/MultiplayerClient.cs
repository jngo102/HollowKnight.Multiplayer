using System;
using System.IO;
using System.Collections.Generic;
using Modding;
using MultiplayerClient.Canvas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerClient
{
    public class MultiplayerClient : Mod<SaveSettings, GlobalSettings>
    {
        public static readonly Dictionary<string, GameObject> GameObjects = new Dictionary<string, GameObject>();
        internal static GlobalSettings settings;

        public static Dictionary<byte[], string> textureCache;

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
            settings = GlobalSettings;

            // Initialize texture cache
            // This will allow us to easily send textures to the server when asked to.
            string cacheDir = Path.Combine(Application.dataPath, "SkinCache");
            Directory.CreateDirectory(cacheDir);
            string[] files = Directory.GetFiles(cacheDir);
            foreach(string filePath in files)
            {
                string filename = Path.GetFileName(filePath);
                byte[] hash = new byte[20];
                for (int i = 0; i < 40; i += 2)
                {
                    hash[i / 2] = Convert.ToByte(filename.Substring(i, 2), 16);
                }

                textureCache[hash] = filePath;
            }

            GameObjects.Add("Glob", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Globs/Hive Knight Glob"]);
            GameObjects.Add("Slash", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Hive Knight/Slash 1"]);

            GUIController.Instance.BuildMenus();

            GameManager.instance.gameObject.AddComponent<MPClient>();

            Unload();
            
            ModHooks.Instance.BeforeSavegameSaveHook += RespawnFix;
            ModHooks.Instance.CharmUpdateHook += OnCharmUpdate;
            ModHooks.Instance.ApplicationQuitHook += OnApplicationQuit;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnApplicationQuit()
        {
            SaveGlobalSettings();

            if (Client.Instance != null)
            {
                Client.Instance.Disconnect();
            }
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
            
            SessionManager.Instance.DestroyAllPlayers();
        }
    }
}