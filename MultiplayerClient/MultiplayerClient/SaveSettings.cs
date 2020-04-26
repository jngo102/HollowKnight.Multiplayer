using Modding;
using UnityEngine;

namespace MultiplayerClient
{
    public class SaveSettings : ModSettings, ISerializationCallbackReceiver
    {
        public SaveGameData saveGameData = new SaveGameData(PlayerData.instance, SceneData.instance);    

        public void OnBeforeSerialize()
        {
            throw new System.NotImplementedException();
        }

        public void OnAfterDeserialize()
        {
            throw new System.NotImplementedException();
        }
    }
}