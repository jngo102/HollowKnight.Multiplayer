using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace MultiplayerServer
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

        public static string Hash(this byte[] texBytes)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] hash = sha1.ComputeHash(texBytes);
                
                var stringBuilder = new StringBuilder(hash.Length * 2);

                foreach (byte @byte in hash)
                {
                    stringBuilder.Append(@byte.ToString("x2"));
                }
       
                return stringBuilder.ToString();
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

        public static List<GameObject> FindChildEnemies(this GameObject gameObject)
        {
            List<GameObject> enemies = new List<GameObject>();

            if (gameObject.layer == 11 || gameObject.layer == 17)
            {
                enemies.Add(gameObject);
            }
            
            foreach (Transform childTransform in gameObject.transform)
            {
                GameObject child = childTransform.gameObject;
                if (child.layer == 11 || gameObject.layer == 17)
                {
                    Modding.Logger.Log("Enemy Name: " + child.name);
                    enemies.Add(child);
                }

                List<GameObject> childEnemies = FindChildEnemies(child);

                foreach (GameObject descendant in childEnemies)
                {
                    enemies.Add(descendant);
                }
            }
            
            return enemies;
        }
    }
}