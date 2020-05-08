using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MultiplayerServer.Canvas
{
    public class OptionsPanel
    {
        public static CanvasPanel Panel;

        private static CanvasToggle _pvpToggle;

        public static void BuildMenu(GameObject canvas)
        {
            Texture2D panelImg = GUIController.Instance.images["Panel_BG"];
            float toggleHeight = 30;
            
            float x = Screen.width / 2.0f - panelImg.width / 2.0f - 30.0f;
            float y = 200.0f;

            if (!GameObject.Find("EventSystem"))
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                
                EventSystem eventSystem = eventSystemObj.AddComponent<EventSystem>();
                eventSystem.sendNavigationEvents = true;
                eventSystem.pixelDragThreshold = 10;
                
                eventSystemObj.AddComponent<StandaloneInputModule>();

                Object.DontDestroyOnLoad(eventSystemObj);
            }

            Panel = new CanvasPanel(
                canvas,
                panelImg,
                new Vector2(x, y), 
                Vector2.zero,
                new Rect(0, 0, panelImg.width, panelImg.height)
            );
            
            Panel.AddText(
                "Options Text",
                "Options",
                new Vector2(x, y),
                new Vector2(panelImg.width, 60), 
                GUIController.Instance.trajanNormal,
                24,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
            y += 70;
            
            _pvpToggle = Panel.AddToggle(
                "Toggle PvP",
                GUIController.Instance.images["Toggle_BG"],
                GUIController.Instance.images["Checkmark"],
                new Vector2(x, y),
                new Vector2(panelImg.width, 20),
                new Vector2(-60, 0),
                new Rect(0, 0, 150, 20),
                PvPToggle,
                GUIController.Instance.trajanNormal,
                "Enable PvP",
                16
            );
            y += toggleHeight;

            Panel.SetActive(false, true);
            
            On.GameManager.PauseGameToggle += OnGamePause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
        }

        private static IEnumerator OnGamePause(On.GameManager.orig_PauseGameToggle orig, GameManager self)
        {
            GameManager.instance.StartCoroutine(orig(self));

            bool paused = GameManager.instance.IsGamePaused();

            Panel.SetActive(paused, !paused);
            
            yield return null;
        }

        private static void OnSceneChange(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "Menu_Title")
            {
                Log("Is Menu Title");
                Panel.SetActive(false, true);
            }
        }
        
        private static void PvPToggle(bool toggleValue)
        {
            if (toggleValue)
            {
                Log("PvP Enabled");
                ServerSettings.PvPEnabled = true;
                ServerSend.PvPEnabled();
            }
            else
            {
                Log("PvP Disabled");
                ServerSettings.PvPEnabled = false;
                ServerSend.PvPEnabled();
            }
        }

        private static void Log(object message) => Modding.Logger.Log("[Connection Panel] " + message);
    }
}