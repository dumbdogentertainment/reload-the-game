namespace DumbDogEntertainment
{
    using System.Collections;
    using DumbDogEntertainment.ScriptableObjects;
    using UnityEngine;
    using UnityEngine.UI;

    public class EnemyBehavior : MonoBehaviour
    {
        public Enemy my;
        public Vector3 target;
        public float health;

        [SerializeField]
        Transform healthbar;

        private Image healthImage;

        [SerializeField]
        private bool IsInvincible = false;

        void Start()
        {
            this.health = this.my.health;
            this.healthImage = this.healthbar.GetComponent<Image>();

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

            this.healthImage.fillAmount = this.health / this.my.health;

            Vector3 direction = this.target - this.transform.position;

            if(direction.magnitude <= 0.7f)
            {
                StartCoroutine(SelfDestruct());
            }

            this.transform.Translate(direction.normalized * this.my.speed, Space.World);
        }

        public void SetTarget(GameObject targetGameObject)
        {
            if(null == targetGameObject)
            {
                return;
            }

            this.target = targetGameObject.transform.position;
        }

        public void DamageMe(float damage)
        {
            if(this.IsInvincible)
            {
                return;
            }

            this.health -= damage;

            if(this.health <= 0)
            {
                GameManager.Instance.ModifyScore(this.my.scoreValue);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, this.my.damageRadius);
        }

        private IEnumerator SelfDestruct()
        {
            Debug.Log("Self-destructing ..");
            yield return new WaitForSeconds(0.01f);

            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 5f);

            foreach (Collider collider in colliders)
            {
                Debug.Log(collider.gameObject.name);
                TowerBehavior tower = collider.GetComponentInParent<TowerBehavior>();
                if (null != tower)
                {
                    Debug.Log("Damaging tower.");
                    tower.ModifyHealth(-1 * this.my.damage);
                    Destroy(this.gameObject);
                }
            }

            this.health = 0f;
        }
    }
}