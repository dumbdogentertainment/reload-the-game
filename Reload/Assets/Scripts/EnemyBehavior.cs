namespace DumbDogEntertainment
{
    using DumbDogEntertainment.ScriptableObjects;
    using UnityEngine;

    public class EnemyBehavior : MonoBehaviour
    {
        public ScriptableObjects.Enemy my;
        public Vector3 target;

        void Start()
        {
            TowerBehavior[] towers = GameObject.FindObjectsOfType<TowerBehavior>();

            int randomTowerIndex = Random.Range(0, towers.Length - 1);
            this.my.targetPosition = towers[randomTowerIndex].transform.position;
            this.my.targetPosition.y = this.transform.position.y;
        }

        void Update()
        {
            Vector3 direction = this.my.targetPosition - this.transform.localPosition;
            this.transform.Translate(direction.normalized * 0.025f, Space.World);
        }
    }
}