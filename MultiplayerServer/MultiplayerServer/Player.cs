using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using UnityEngine;

namespace MultiplayerServer
{
    public class Player : MonoBehaviour
    {
        public byte id;
        public string username;
        public string animation;
        public string activeScene;
        public Vector3 position;
        public Vector3 scale;

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

        public Dictionary<string, byte[]> texBytes = new Dictionary<string, byte[]>();
        public Dictionary<string, string> texHashes = new Dictionary<string, string>
        {
            { "Baldur", "" },
            { "Fluke", "" },
            { "Grimm", "" },
            { "Hatchling", "" },
            { "Knight", "" },
            { "Shield", "" },
            { "Sprint", "" },
            { "Unn", "" },
            { "Void", "" },
            { "VS", "" },
            { "Weaver", "" },
            { "Wraiths", "" },
        };

        public string baldurHash;
        public string flukeHash;
        public string grimmHash;
        public string hatchlingHash;
        public string knightHash;
        public string shieldHash;
        public string sprintHash;
        public string unnHash;
        public string voidHash;
        public string vsHash;
        public string weaverHash;
        public string wraithsHash;
        
        public void Initialize(byte id, string username, string animation, int health, int maxHealth, int healthBlue)
        {
            this.id = id;
            this.username = username;
            this.animation = animation;
            this.health = health;
            this.maxHealth = maxHealth;
            this.healthBlue = healthBlue;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;

            ServerSend.PlayerPosition(this);
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;

            ServerSend.PlayerScale(this);
        }
        
        public void SetAnimation(string animation)
        {
            this.animation = animation;

            ServerSend.PlayerAnimation(this);
        }

        private void Log(object message) => Modding.Logger.Log("[Player] " + message);
    }
}