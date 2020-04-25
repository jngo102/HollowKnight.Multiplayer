using System.Collections.Generic;
using Modding;
using MultiplayerClient.Canvas;
using UnityEngine;

namespace MultiplayerClient
{
    public class MultiplayerClient : Mod
    {
        public static readonly Dictionary<string, GameObject> GameObjects = new Dictionary<string, GameObject>();
        
        public override string GetVersion()
        {
            return "0.0.1";
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
        }
    }
}