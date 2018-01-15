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
        //public float fireRate = 0.5f;
        public float fireCooldown;
        //public float fireCost = 1.5f;

        Transform turretTransform;
        Transform muzzleTransform;

        public Enemy[] currentTargets;

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
            Debug.DrawRay(
            this.turretTransform.position,
            this.turretTransform.TransformDirection(Vector3.back) * 5,
            Color.red);

            this.currentTargets = GameObject.FindObjectsOfType<Enemy>();

            FireAtTargets(GameObject.FindObjectsOfType<Enemy>());
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

        private void FireAtTargets(Enemy[] enemies)
        {
            this.fireCooldown -= Time.deltaTime;

            if (false == enemies.Any())
            {
                return;
            }

            // current target is closest enemy in range (min, max)
            Enemy currentTarget = enemies.FirstOrDefault();

            // look at current target
            Vector3 lookDirection = this.currentTargets.FirstOrDefault().transform.position - this.turretTransform.position;
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