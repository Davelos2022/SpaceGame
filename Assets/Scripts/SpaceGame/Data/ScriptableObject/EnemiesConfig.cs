using UnityEngine;
using System;

namespace SpaceGame.Data
{
    [CreateAssetMenu(fileName = "Enemies Config", menuName = "Enemies")]
    public class EnemiesConfig : ScriptableObject
    {
        public EnemyData[] Enemies;
    }

    [Serializable]
    public class EnemyData
    {
        [Header("Space Ship config")]
        public Sprite SpriteEnemy;
        [Range(10f, 200f)] public float Health;
        [Range(70f, 150f)] public float Speed;

        [Header("Attack config")]
        public Sprite BulletSprite;
        [Range(5f, 30f)] public float Damage;
        [Range(.2f, 2f)] public float FireRate;

        [Header("Count points after destroy")]
        [Range(10, 500)] public int Bonus;
    }
}
