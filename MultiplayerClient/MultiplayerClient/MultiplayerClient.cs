using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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

        public static MultiplayerClient Instance;
        
        public static Dictionary<byte[], string> textureCache = new Dictionary<byte[], string>(new ByteArrayComparer());

        public static string CustomKnightDir;
        
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
            
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.MacOSX:
                    CustomKnightDir = Path.GetFullPath(Application.dataPath + "/Resources/Data/Managed/Mods/CustomKnight");
                    break;
                default:
                    CustomKnightDir = Path.GetFullPath(Application.dataPath + "/Managed/Mods/CustomKnight");
                    break;
            }

            GameObjects.Add("Glob", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Globs/Hive Knight Glob"]);
            GameObjects.Add("Slash", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Hive Knight/Slash 1"]);

            Instance = this;
            
            GUIController.Instance.BuildMenus();

            GameManager.instance.gameObject.AddComponent<MPClient>();

            Unload();
            
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

        private void OnCharmUpdate(PlayerData pd, HeroController hc)
        {
            if (Client.Instance != null && Client.Instance.isConnected && pd != null && hc != null)
            {
                ClientSend.CharmsUpdated(pd);
            }
        }

        private void Unload()
        {
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

                if (Client.Instance.isHost && SessionManager.Instance.Players.Count > 0)
                {
                    GameObject[] enemies = UnityEngine.Object.FindObjectsOfType<GameObject>().Where(go => go.layer == 11 || go.layer == 17) as GameObject[];
                    if (enemies != null)
                    {
                        foreach (GameObject enemy in enemies)
                        {
                            enemy.AddComponent<EnemyTracker>();
                        }
                    }
                }
            }
            
            SessionManager.Instance.DestroyAllPlayers();
        }
    }
}