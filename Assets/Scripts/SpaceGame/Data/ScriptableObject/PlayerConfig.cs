using UnityEngine;
using System;

namespace SpaceGame.Data
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "Player")]
    public class PlayerConfig : ScriptableObject
    {
        public PlayerData[] Players;
    }

    [Serializable]
    public class PlayerData
    {
        [Header("Space Ship config")]
        public Sprite SpritePlayer;
        public float Health;
        [Range(1f, 10f)] public float Speed;

        [Header("Attack config")]
        public Sprite BulletSprite;
        [Range(10f, 50f)] public float Damage;
        [Range(1f, 3.5f)] public float FireRate;
    }
}
