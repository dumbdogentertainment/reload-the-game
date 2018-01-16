namespace DumbDogEntertainment.ScriptableObjects
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Projectile", menuName = "Projectile")]
    public class Projectile : ScriptableObject
    {
        public ProjectileType projectileType;
        public Sprite image;

        public float damage;
        public float damageRadius;
        public float speed;
        public float rateOfFire;
        public float costToFire;

        public Vector3 targetPosition;
    }
}