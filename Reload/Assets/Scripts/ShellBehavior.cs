namespace DumbDogEntertainment
{
    using UnityEngine;

    public class ShellBehavior : MonoBehaviour
    {
        public float TimeUntilDestroy = 2f;

        public Projectile projectile;

        public Vector3 target;

        void Start()
        {
            this.target = new Vector3(
                this.projectile.targetPosition.x,
                this.projectile.targetPosition.y,
                this.projectile.targetPosition.z);
        }

        void Update()
        {
            Vector3 direction = this.target - this.transform.localPosition;

            Debug.DrawRay(
            this.transform.position,
            direction,
            Color.blue);

            float distanceThisFrame = this.projectile.speed * Time.deltaTime;

            if(direction.magnitude <= distanceThisFrame)
            {
                Hit();
            }
            else
            {
                this.transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                //Quaternion targetRotation = Quaternion.LookRotation(direction);
                //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, Time.deltaTime * 5.0f);
            }

            this.TimeUntilDestroy -= Time.deltaTime;
            if(this.TimeUntilDestroy <= 0)
            {
                Destroy(this.gameObject);
            }
        }
        
        private void Hit()
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, projectile.damageRadius);

            foreach(Collider collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if(null != enemy)
                {
                    Debug.Log("Hit enemy");
                }
            }
        }
    }
}