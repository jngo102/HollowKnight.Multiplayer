using System;
using System.Collections;
using System.IO;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MultiplayerClient.Canvas
{
    public class ConnectionPanel
    {
        public static CanvasPanel Panel;
        
        private static float y;

        private static CanvasInput _ipInput;
        private static CanvasInput _portInput;
        private static CanvasInput _usernameInput;

        public static void BuildMenu(GameObject canvas)
        {
            y = 30.0f;

            GameObject eventSystemObj = new GameObject("EventSystem");
            
            EventSystem eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystem.sendNavigationEvents = true;
            eventSystem.pixelDragThreshold = 10;
            
            eventSystemObj.AddComponent<StandaloneInputModule>();

            UnityEngine.Object.DontDestroyOnLoad(eventSystemObj);

            Texture2D buttonImg = GUIController.Instance.images["Button_BG"];
            Texture2D inputImg = GUIController.Instance.images["Input_BG"];
            Texture2D panelImg = GUIController.Instance.images["Panel_BG"];
            
            Panel = new CanvasPanel(
                canvas,
                panelImg,
                new Vector2(0, y), 
                Vector2.zero,
                new Rect(0, 0, panelImg.width, panelImg.height)
            );
            
            Panel.AddText(
                "Connection Text",
                "Connection",
                new Vector2(0, y),
                new Vector2(buttonImg.width, buttonImg.height), 
                GUIController.Instance.trajanNormal,
                24,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
            y += buttonImg.height;

            _ipInput = Panel.AddInput(
                "IP Input",
                inputImg,
                new Vector2(0, y),
                Vector2.zero,
                new Rect(0, y, inputImg.width, inputImg.height),
                GUIController.Instance.trajanNormal,
                "",
                "IP Address",
                16
            );
            y += inputImg.height;

            _portInput = Panel.AddInput(
                "Port Input",
                inputImg,
                new Vector2(0, y),
                Vector2.zero,
                new Rect(0, y, inputImg.width, inputImg.height),
                GUIController.Instance.trajanNormal,
                "",
                "Port",
                16
            );
            y += inputImg.height;

            _usernameInput = Panel.AddInput(
                "Username Input",
                inputImg,
                new Vector2(0, y),
                Vector2.zero,
                new Rect(0, y, inputImg.width, inputImg.height),
                GUIController.Instance.trajanNormal,
                "",
                "Username",
                16
            );
            y += inputImg.height;
            
            Panel.AddButton(
                "Connect Button",
                buttonImg,
                new Vector2(0, y),
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
                new Vector2(0, y),
                Vector2.zero,
                DisconnectFromServer,
                new Rect(0, y, buttonImg.width, buttonImg.height),
                GUIController.Instance.trajanNormal,
                "Disconnect",
                16
            );
            y += buttonImg.height;

            eventSystem.firstSelectedGameObject = _ipInput.InputObject;
        }

        private static void ConnectToServer(string buttonName)
        {
            if (!Client.Instance.isConnected)
            {
                Log("Connecting to Server");
                
                if (_ipInput.GetText() != "") Client.Instance.ip = _ipInput.GetText();
                if (_portInput.GetText() != "") Client.Instance.port = int.Parse(_portInput.GetText());
                if (_usernameInput.GetText() != "") Client.Instance.username = _usernameInput.GetText();
                Client.Instance.ConnectToServer();

                Log("Connected to Server!");
            }
            else
            {
                Log("Already connected to the server!");
            }
        }

        private static void DisconnectFromServer(string buttonName)
        {
            Log("Disconnecting from Server...");
            ClientSend.PlayerDisconnected(Client.Instance.myId);
        }
        
        public static void Update()
        {
            if (Panel == null)
            {
                return;
            }

            if (global::GameManager.instance.IsGamePaused())
            {
                if (!Panel.active)
                {
                    Panel.SetActive(true, false);    
                }
            }
            else
            {
                if (Panel.active)
                {
                    Panel.SetActive(false, true);   
                }
            }
        }

        private static void Log(object message) => Modding.Logger.Log("[Connection Panel] " + message);
    }
}