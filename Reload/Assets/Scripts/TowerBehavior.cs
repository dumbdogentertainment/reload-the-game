namespace DumbDogEntertainment
{
    using System;
    using System.Linq;
    using UnityEngine;

    public class TowerBehavior : MonoBehaviour
    {
        public float maxEnergy = 100f;
        public float currentEnergy;
        public float energyRechargeRate = 1f;
        public float energyRechargeAmount = 2f;
        public float energyRechargeCooldown;

        public GameObject shellPrefab;
        private Projectile towerProjectile;
        public float fireCooldown;

        Transform turretTransform;
        Transform muzzleTransform;

        void Awake()
        {
            this.currentEnergy = this.maxEnergy;
            this.energyRechargeCooldown = this.energyRechargeRate;

            this.turretTransform = transform.Find("turret");
            this.muzzleTransform = this.turretTransform.Find("barrel").Find("muzzle");

            this.towerProjectile = this.shellPrefab.GetComponent<ShellBehavior>().projectile;
            this.fireCooldown = this.towerProjectile.rateOfFire;
        }

        void Update()
        {
            //Debug.DrawRay(
            //this.turretTransform.position,
            //this.turretTransform.TransformDirection(Vector3.back) * 5,
            //Color.red);

            FireAtTargets(GameObject.FindObjectsOfType<EnemyBehavior>());
            Recharge();
        }

        private void Recharge()
        {
            this.energyRechargeCooldown -= Time.deltaTime;

            if (this.energyRechargeCooldown > 0)
            {
                return;
            }

            this.currentEnergy += this.energyRechargeRate;
            this.currentEnergy = Mathf.Clamp(this.currentEnergy, 0f, this.maxEnergy);
            this.energyRechargeCooldown = this.energyRechargeRate;
        }

        private void FireAtTargets(EnemyBehavior[] enemies)
        {
            this.fireCooldown -= Time.deltaTime;

            EnemyBehavior[] enemiesInRange = enemies
                .Where(enemy => Vector3.Distance(this.muzzleTransform.position, enemy.transform.position).IsBetweenInclusive(3.5f, 6.5f))
                .OrderBy(enemy => Vector3.Distance(this.muzzleTransform.position, enemy.transform.position))
                .ToArray();

            if (false == enemiesInRange.Any())
            {
                return;
            }

            // current target is closest enemy in range (3.5, 6.5)
            EnemyBehavior currentTarget = enemiesInRange.FirstOrDefault();

            // look at current target
            Vector3 lookDirection = currentTarget.transform.position - this.turretTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection * -1);

            this.turretTransform.rotation = Quaternion.Euler(
                lookRotation.eulerAngles.x,
                lookRotation.eulerAngles.y,
                this.turretTransform.rotation.eulerAngles.z);

            // fire at current target
            if (this.fireCooldown <= 0 && this.currentEnergy >= this.towerProjectile.costToFire)
            {
                GameObject shellGameObject = (GameObject)Instantiate(
                    this.shellPrefab,
                    this.muzzleTransform.position,
                    this.muzzleTransform.rotation);

                Projectile firedProjectile = shellGameObject.GetComponent<ShellBehavior>().projectile;
                firedProjectile.targetPosition = currentTarget.transform.localPosition;

                this.currentEnergy -= this.towerProjectile.costToFire;
                this.fireCooldown = this.towerProjectile.rateOfFire;
            }
        }
    }
}