namespace DumbDogEntertainment.ScriptableObjects
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
    public class Enemy : ScriptableObject
    {
        public EnemyType enemyType;
        public Sprite image;

        public float health;
        public int damage;
        public float speed;
        public float rateOfFire;
        public int scoreValue;
        public float damageRadius;

        public Vector3 targetPosition;
    }
}