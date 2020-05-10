using System;
using System.Collections;
using System.Collections.Generic;
using IL.HutongGames.PlayMaker.Actions;
using ModCommon;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MultiplayerClient.Canvas
{
    public class ConnectionPanel
    {
        public static CanvasPanel Panel;

        private static CanvasInput _ipInput;
        private static CanvasInput _portInput;
        private static CanvasInput _usernameInput;

        public static void BuildMenu(GameObject canvas)
        {
            Texture2D buttonImg = GUIController.Instance.images["Button_BG"];
            Texture2D inputImg = GUIController.Instance.images["Input_BG"];
            Texture2D panelImg = GUIController.Instance.images["Panel_BG"];
            
            float x = Screen.width / 2.0f - inputImg.width / 2.0f - 30.0f;
            float y = 30.0f;

            EventSystem eventSystem = null;
            if (!GameObject.Find("EventSystem"))
            {
                GameObject eventSystemObj = new GameObject("EventSystem");

                eventSystem = eventSystemObj.AddComponent<EventSystem>();
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
                "Connection Text",
                "Connection",
                new Vector2(x, y),
                new Vector2(buttonImg.width, buttonImg.height), 
                GUIController.Instance.trajanNormal,
                24,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
            y += buttonImg.height + 10;

            _ipInput = Panel.AddInput(
                "IP Input",
                inputImg,
                new Vector2(x, y),
                Vector2.zero,
                new Rect(0, y, inputImg.width, inputImg.height),
                GUIController.Instance.trajanNormal,
                "",
                "Address",
                16
            );
            y += inputImg.height + 5;

            _portInput = Panel.AddInput(
                "Port Input",
                inputImg,
                new Vector2(x, y),
                Vector2.zero,
                new Rect(0, y, inputImg.width, inputImg.height),
                GUIController.Instance.trajanNormal,
                "",
                "Port",
                16
            );
            y += inputImg.height + 5;

            _usernameInput = Panel.AddInput(
                "Username Input",
                inputImg,
                new Vector2(x, y),
                Vector2.zero,
                new Rect(0, y, inputImg.width, inputImg.height),
                GUIController.Instance.trajanNormal,
                "",
                "Username",
                16
            );
            y += inputImg.height + 5;
            
            Panel.AddButton(
                "Connect Button",
                buttonImg,
                new Vector2(x, y),
                Vector2.zero,
                ConnectToServer,
                new Rect(0, y, buttonImg.width, buttonImg.height),
                GUIController.Instance.trajanNormal,
                "Connect",
                16
            );
            y += buttonImg.height;

            Panel.AddButton(
                "Disconnect Button",
                buttonImg,
                new Vector2(x, y),
                Vector2.zero,
                DisconnectFromServer,
                new Rect(0, y, buttonImg.width, buttonImg.height),
                GUIController.Instance.trajanNormal,
                "Disconnect",
                16
            );
            y += buttonImg.height;

            if (eventSystem != null)
            {
                eventSystem.firstSelectedGameObject = _ipInput.InputObject;
            }
            
            Panel.SetActive(false, true);
            
            On.GameManager.PauseGameToggle += OnGamePause;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
        }

        private static IEnumerator OnGamePause(On.GameManager.orig_PauseGameToggle orig, global::GameManager self)
        { 
            global::GameManager.instance.StartCoroutine(orig(self));

            bool paused = global::GameManager.instance.IsGamePaused();
            Panel.SetActive(paused, !paused);
            
            yield return null;
        }

        private static void OnSceneChange(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "Menu_Title")
            {
                Panel.SetActive(false, true);
            }
        }
        
        private static Coroutine _connectRoutine;
        private static void ConnectToServer(string buttonName)
        {
            if (!Client.Instance.isConnected)
            {
                Log("Connecting to Server...");
                
                if (_ipInput.GetText() != "") Client.Instance.host = _ipInput.GetText();
                if (_portInput.GetText() != "") Client.Instance.port = int.Parse(_portInput.GetText());
                if (_usernameInput.GetText() != "") Client.Instance.username = _usernameInput.GetText();

                PlayerManager.activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                _connectRoutine = Client.Instance.StartCoroutine(Connect());

                Log("Connected to Server!");
            }
            else
            {
                Log("Already connected to the server!");
            }
        }

        private static IEnumerator Connect()
        {
            int waitTime = 2000;
            int time = DateTime.Now.Millisecond;
            // 5 connection attempts before giving up 
            for (int attempt = 1; attempt <= 5; attempt++)
            {
                Log("Connection Attempt: " + attempt);
                if (!Client.Instance.isConnected)
                {
                    Client.Instance.ConnectToServer();
                }
                else
                {
                    Log("Connected to Server!");
                    HeroController.instance.GetComponent<SpriteFlash>().flashFocusHeal();
                    break;
                }

                yield return new WaitWhile(() => Client.Instance.isConnected && DateTime.Now.Millisecond - time <= waitTime);
                time = DateTime.Now.Millisecond;
            }
        }

        private static void DisconnectFromServer(string buttonName)
        {
            Log("Disconnecting from Server...");
            Client.Instance.StopCoroutine(_connectRoutine);
            ClientSend.PlayerDisconnected(Client.Instance.myId);
            Client.Instance.Disconnect();

            Log("You have disconnected from the server.");
        }

        private static void Log(object message) => Modding.Logger.Log("[Connection Panel] " + message);
    }
}