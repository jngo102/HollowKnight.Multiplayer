using Modding;
using MultiplayerServer.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MultiplayerServer
{
    public class MultiplayerServer : Mod
    {
        public static Dictionary<byte[], string> textureCache = new Dictionary<byte[], string>(new ByteArrayComparer());

        public override string GetVersion()
        {
            return "0.0.1";
        } 

        public override void Initialize()
        {
            // Initialize texture cache
            // This will allow us to easily send textures to the server when asked to.
            Log("Listing saved textures :");
            string cacheDir = Path.Combine(Application.dataPath, "SkinCache");
            Directory.CreateDirectory(cacheDir);
            string[] files = Directory.GetFiles(cacheDir);
            foreach (string filePath in files)
            {
                string filename = Path.GetFileName(filePath);
                Log(filename);

                byte[] hash = new byte[20];
                for (int i = 0; i < 40; i += 2)
                {
                    hash[i / 2] = Convert.ToByte(filename.Substring(i, 2), 16);
                }

                textureCache[hash] = filePath;
            }

            GameManager.instance.gameObject.AddComponent<MPServer>();
            GUIController.Instance.BuildMenus();
        }
    }
}