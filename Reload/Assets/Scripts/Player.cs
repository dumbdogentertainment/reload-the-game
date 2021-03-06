﻿namespace DumbDogEntertainment
{
    using System.Collections;

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        #region Static instance
        public static Player Instance = null;

        void Awake()
        {
            Instance = this;
        }

        void OnApplicationQuit()
        {
            Instance = null;
        }
        #endregion

        #region Properties and variables

        public float regenAmount = 0.05f;
        public float degenAmount = 0.08f;
        public float energizeAmount = 25f;

        public float maximumEnergy = 100f;

        public float speed = 5f;
        public float boostedSpeed = 8f;

        public TowerBehavior connectedTower;

        public float boostAbilityCooldown = 4f;
        public float energizeAbilityCooldown = 1f;
        public float repairAbilityCooldown = 3f;

        private Image energyImage;
        private Image overflowEnergyImage;
        private Rigidbody playerRigidbody;
        private float horizontalMovement;

        [SerializeField]
        private Transform energyBar;

        [SerializeField]
        private Transform overflowEnergyBar;

        //[SerializeField]
        private float energy;

        //[SerializeField]
        private float mainReserveEnergy;

        //[SerializeField]
        private float overflowReserveEnergy;

        //[SerializeField]
        private bool isCurrentlyBoostingSelf = false;

        //[SerializeField]
        private bool isCurrentlyEnergizingTower = false;

        //[SerializeField]
        private bool isCurrentlyRepairingTower = false;

        //[SerializeField]
        private bool isCurrentlyTakingEnergyFromTower = false;

        #endregion

        void Start()
        {
            this.playerRigidbody = GetComponent<Rigidbody>();

            this.energyImage = this.energyBar.GetComponent<Image>();
            this.overflowEnergyImage = this.overflowEnergyBar.GetComponent<Image>();

            this.mainReserveEnergy = this.maximumEnergy;
            this.overflowReserveEnergy = 0f;
        }

        void Update()
        {
            // regen main energy
            this.mainReserveEnergy = Mathf.Clamp(
                this.mainReserveEnergy + this.regenAmount,
                0,
                this.maximumEnergy);

            // degen overflow energy
            this.overflowReserveEnergy = Mathf.Clamp(
                this.overflowReserveEnergy - this.degenAmount,
                0,
                this.maximumEnergy);

            // total energy for use
            this.energy = this.mainReserveEnergy + this.overflowReserveEnergy;

            UpdateEnergyCanvas();
        }

        void FixedUpdate()
        {
            this.horizontalMovement = InputManager.Instance.GetHorizontalAxisInput();
        }

        void LateUpdate()
        {
            Vector3 desiredMovement = (this.isCurrentlyBoostingSelf ? this.boostedSpeed : this.speed) *
                new Vector3(this.horizontalMovement, 0, 0);

            this.playerRigidbody.velocity = desiredMovement;
        }

        void UpdateEnergyCanvas()
        {
            this.energyImage.fillAmount = this.mainReserveEnergy / this.maximumEnergy;
            this.overflowEnergyImage.fillAmount =
                Mathf.Clamp(this.overflowReserveEnergy, 0, this.maximumEnergy) / this.maximumEnergy;
        }

        public void EnergizeSelf()
        {
            if (this.isCurrentlyBoostingSelf)
            {
                return;
            }

            Debug.Log("Giving myself a quick energy boost ...");
            StartCoroutine(BoostSelf());
        }

        public void EnergizeNearestTower()
        {
            if (this.isCurrentlyEnergizingTower || null == this.connectedTower)
            {
                return;
            }

            Debug.Log("Energizing nearest tower ...");
            StartCoroutine(GiveEnergy());
        }

        public void RepairNearestTower()
        {
            if(this.isCurrentlyRepairingTower || null == this.connectedTower)
            {
                return;
            }

            Debug.Log("Repairing nearest tower ...");
            StartCoroutine(RepairTower());
        }

        public void TakeEnergyFromNearestTower()
        {
            if (this.isCurrentlyTakingEnergyFromTower || null == this.connectedTower)
            {
                return;
            }

            Debug.Log("Taking energy from nearest tower ...");
            StartCoroutine(TakeEnergy());
        }

        /// <summary>
        /// Boost self .. gives additional energy (up to a certain maximum)
        /// that degrades over time .. also, grants increased speed.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for Unity's StartCoroutine method.</returns>
        private IEnumerator BoostSelf()
        {
            this.isCurrentlyBoostingSelf = true;

            float reservesAddition = Mathf.Clamp(this.maximumEnergy - this.energy, 0, this.energizeAmount);
            this.mainReserveEnergy += reservesAddition;
            this.overflowReserveEnergy += (this.energizeAmount - reservesAddition);

            yield return new WaitForSeconds(this.boostAbilityCooldown);
            this.isCurrentlyBoostingSelf = false;
        }

        /// <summary>
        /// Transfer energy from player's own reserves if enough is available.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for Unity's StartCoroutine method.</returns>
        private IEnumerator GiveEnergy()
        {
            this.isCurrentlyEnergizingTower = true;

            float takeFromReserves = this.energizeAmount;
            if(this.overflowReserveEnergy > 0f)
            {
                // OF: 35   15
                // $$: 25   25
                // TK: 25   15
                // TR: 00   10
                // $$ = TK + TR
                // TK = OF >= $$ ? $$ : OF
                float takeFromOverflow = this.overflowReserveEnergy >= this.energizeAmount ?
                    this.energizeAmount :
                    this.overflowReserveEnergy;

                takeFromReserves = this.energizeAmount - takeFromOverflow;

                this.overflowReserveEnergy -= takeFromOverflow;
            }

            this.mainReserveEnergy -= takeFromReserves;

            this.connectedTower.ModifyEnergy(this.energizeAmount);
            yield return new WaitForSeconds(this.energizeAbilityCooldown);
            this.isCurrentlyEnergizingTower = false;
        }

        /// <summary>
        /// Repair tower in range
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for Unity's StartCoroutine method.</returns>
        private IEnumerator RepairTower()
        {
            this.isCurrentlyRepairingTower = true;
            float speed = this.speed;
            this.speed = 0f;

            StartCoroutine(this.connectedTower.ModifyHealthOverTime(25f, this.repairAbilityCooldown));
            yield return new WaitForSeconds(this.repairAbilityCooldown);

            this.speed = speed;
            this.isCurrentlyRepairingTower = false;
        }

        /// <summary>
        /// Repair tower in range
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for Unity's StartCoroutine method.</returns>
        private IEnumerator TakeEnergy()
        {
            this.isCurrentlyTakingEnergyFromTower = true;

            float reservesAddition = Mathf.Clamp(this.maximumEnergy - this.energy, 0, this.energizeAmount);
            this.mainReserveEnergy += reservesAddition;
            this.overflowReserveEnergy += (this.energizeAmount - reservesAddition);

            yield return new WaitForSeconds(this.energizeAbilityCooldown);
            this.isCurrentlyTakingEnergyFromTower = false;
        }
    }
}