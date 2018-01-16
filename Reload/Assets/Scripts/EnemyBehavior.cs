namespace DumbDogEntertainment
{
    using DumbDogEntertainment.ScriptableObjects;
    using UnityEngine;

    public class EnemyBehavior : MonoBehaviour
    {
        public Enemy my;
        public Vector3 target;
        public float health;

        void Start()
        {
            this.health = this.my.health;
            TowerBehavior[] towers = GameObject.FindObjectsOfType<TowerBehavior>();

            int randomTowerIndex = Random.Range(0, towers.Length - 1);

            // in future, get this from enemy spawner
            this.target = towers[randomTowerIndex].transform.position;
            this.target.y = this.transform.position.y;
        }

        void Update()
        {
            if(this.health <= 0)
            {
                Destroy(this.gameObject);
                return;
            }

            Vector3 direction = this.target - this.transform.localPosition;
            this.transform.Translate(direction.normalized * this.my.speed, Space.World);
        }

        public void DamageMe(float damage)
        {
            this.health -= damage;
        }
    }
}