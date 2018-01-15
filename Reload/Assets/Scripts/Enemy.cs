namespace DumbDogEntertainment
{
    using UnityEngine;

    public class Enemy : MonoBehaviour
    {
        public Vector3 target;

        void Start()
        {
            TowerBehavior[] towers = GameObject.FindObjectsOfType<TowerBehavior>();

            int randomTowerIndex = Random.Range(0, towers.Length - 1);
            this.target = towers[randomTowerIndex].transform.position;
            this.target.y = this.transform.position.y;
        }

        void Update()
        {
            Vector3 direction = this.target - this.transform.localPosition;
            this.transform.Translate(direction.normalized * 0.025f, Space.World);
        }
    }
}