namespace DumbDogEntertainment
{
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
        public float fireRate = 0.5f;
        public float fireCooldown;
        public float fireCost = 1.5f;

        Transform turretTransform;
        Transform muzzleTransform;

        public Enemy[] currentTargets;

        void Awake()
        {
            this.currentEnergy = this.maxEnergy;
            this.energyRechargeCooldown = this.energyRechargeRate;

            this.turretTransform = transform.Find("turret");
            this.muzzleTransform = this.turretTransform.Find("barrel").Find("muzzle");
            this.fireCooldown = this.fireRate;
        }

        void Update()
        {
            Debug.DrawRay(
            this.turretTransform.position,
            this.turretTransform.TransformDirection(Vector3.back) * 5,
            Color.red);

            this.currentTargets = GameObject.FindObjectsOfType<Enemy>();

            if(this.currentTargets.Any())
            {
                Vector3 lookDirection = this.currentTargets.FirstOrDefault().transform.position - this.turretTransform.position;
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection * -1);

                this.turretTransform.rotation = Quaternion.Euler(
                    lookRotation.eulerAngles.x,
                    lookRotation.eulerAngles.y,
                    this.turretTransform.rotation.eulerAngles.z);
            }

            #region Recharge

            if (this.energyRechargeCooldown <= 0)
            {
                this.currentEnergy += this.energyRechargeRate;
                this.currentEnergy = Mathf.Clamp(this.currentEnergy, 0f, this.maxEnergy);
                this.energyRechargeCooldown = this.energyRechargeRate;
            }

            this.energyRechargeCooldown -= Time.deltaTime;

            #endregion

            if (this.fireCooldown <= 0 && this.currentEnergy >= this.fireCost)
            {
                ////GameObject shellGameObject = (GameObject)Instantiate(
                ////    this.shellPrefab,
                ////    muzzleTransform.position,
                ////    muzzleTransform.rotation);

                Instantiate(
                    this.shellPrefab,
                    muzzleTransform.position,
                    muzzleTransform.rotation);

                this.currentEnergy -= this.fireCost;
                this.fireCooldown = this.fireRate;
            }

            this.fireCooldown -= Time.deltaTime;
        }
    }
}