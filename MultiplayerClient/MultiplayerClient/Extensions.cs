using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace MultiplayerClient
{
    public static class Extensions
    {
        public static T CopyComponent<T>(this GameObject destination, T original) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }

        public static byte[] Hash(this Texture2D tex)
        {
            byte[] texBytes = tex.DuplicateTexture().EncodeToPNG();
            
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] hash = sha1.ComputeHash(texBytes);

                // Save texture in cache if not already done
                if (!MultiplayerClient.textureCache.ContainsKey(hash))
                {
                    string hashStr = BitConverter.ToString(hash).Replace("-", string.Empty);
                    string cacheDir = Path.Combine(Application.dataPath, "SkinCache");
                    string filePath = Path.Combine(cacheDir, hashStr);
                    File.WriteAllBytes(filePath, texBytes);
                    MultiplayerClient.textureCache[hash] = filePath;
                }

                return hash;
            }
        }
        
        public static Texture2D DuplicateTexture(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }
}
