using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using UnityEngine;

namespace MultiplayerClient
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;
        
        public byte id;
        public string username;

        public static string activeScene;
        
        public bool equippedCharm_1;
        public bool equippedCharm_2;
        public bool equippedCharm_3;
        public bool equippedCharm_4;
        public bool equippedCharm_5;
        public bool equippedCharm_6;
        public bool equippedCharm_7;
        public bool equippedCharm_8;
        public bool equippedCharm_9;
        public bool equippedCharm_10;
        public bool equippedCharm_11;
        public bool equippedCharm_12;
        public bool equippedCharm_13;
        public bool equippedCharm_14;
        public bool equippedCharm_15;
        public bool equippedCharm_16;
        public bool equippedCharm_17;
        public bool equippedCharm_18;
        public bool equippedCharm_19;
        public bool equippedCharm_20;
        public bool equippedCharm_21;
        public bool equippedCharm_22;
        public bool equippedCharm_23;
        public bool equippedCharm_24;
        public bool equippedCharm_25;
        public bool equippedCharm_26;
        public bool equippedCharm_27;
        public bool equippedCharm_28;
        public bool equippedCharm_29;
        public bool equippedCharm_30;
        public bool equippedCharm_31;
        public bool equippedCharm_32;
        public bool equippedCharm_33;
        public bool equippedCharm_34;
        public bool equippedCharm_35;
        public bool equippedCharm_36;
        public bool equippedCharm_37;
        public bool equippedCharm_38;
        public bool equippedCharm_39;
        public bool equippedCharm_40;

        public int health;
        public int maxHealth;
        public int healthBlue;

        private void Awake()
        {
            Instance = this;
        }
        
        private void Log(object message) => Modding.Logger.Log("[Player Manager] " + message);
    }
}