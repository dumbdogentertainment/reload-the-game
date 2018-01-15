namespace DumbDogEntertainment
{
    using System;
    using System.Collections;
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

        Transform muzzleTransform;

        void Awake()
        {
            this.currentEnergy = this.maxEnergy;
            this.energyRechargeCooldown = this.energyRechargeRate;

            this.muzzleTransform = transform.Find("barrel");
            this.fireCooldown = this.fireRate;
        }

        void Update()
        {
            #region Recharge

            if (this.energyRechargeCooldown <= 0)
            {
                this.currentEnergy += this.energyRechargeRate;
                this.currentEnergy = Mathf.Clamp(this.currentEnergy, 0f, this.maxEnergy);
                this.energyRechargeCooldown = this.energyRechargeRate;
            }

            this.energyRechargeCooldown -= Time.deltaTime;

            #endregion

            if (this.fireCooldown <= 0)
            {
                ////GameObject shellGameObject = (GameObject)Instantiate(
                ////    this.shellPrefab,
                ////    muzzleTransform.position,
                ////    muzzleTransform.rotation);

                Instantiate(
                    this.shellPrefab,
                    muzzleTransform.position,
                    muzzleTransform.rotation);

                this.fireCooldown = this.fireRate;
            }

            this.fireCooldown -= Time.deltaTime;
        }
    }
}