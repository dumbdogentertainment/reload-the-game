namespace DumbDogEntertainment
{
    using DumbDogEntertainment.ScriptableObjects;
    using UnityEngine;

    public class ShellBehavior : MonoBehaviour
    {
        public float TimeUntilDestroy = 2f;

        public Projectile projectile;

        public Vector3 target;

        void Start()
        {
            this.target = this.projectile.targetPosition;
        }

        void Update()
        {
            Vector3 direction = this.target - this.transform.localPosition;

            //Debug.DrawRay(
            //this.transform.position,
            //direction,
            //Color.blue);

            float distanceThisFrame = this.projectile.speed * Time.deltaTime;

            TryHit();

            if (direction.magnitude <= distanceThisFrame)
            {
                Destroy(this.gameObject);
            }
            else
            {
                this.transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            }

            this.TimeUntilDestroy -= Time.deltaTime;
            if (this.TimeUntilDestroy <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, this.projectile.damageRadius);
        }

        private void TryHit()
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.projectile.damageRadius);
            Debug.Log(this.name + " damaging " + colliders.Length + " enemies.");

            foreach (Collider collider in colliders)
            {
                EnemyBehavior enemy = collider.GetComponent<EnemyBehavior>();
                if (null != enemy)
                {
                    enemy.DamageMe(this.projectile.damage);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}