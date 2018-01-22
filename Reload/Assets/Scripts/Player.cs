namespace DumbDogEntertainment
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

        [SerializeField]
        private float boostAbilityCooldown = 4f;

        [SerializeField]
        private float energizeAbilityCooldown = 1f;

        [SerializeField]
        private float repairAbilityCooldown = 3f;

        [SerializeField]
        private float energy;

        [SerializeField]
        private float mainReserveEnergy;

        [SerializeField]
        private float overflowReserveEnergy;

        [SerializeField]
        private bool isCurrentlyBoostingSelf = false;

        [SerializeField]
        private bool isCurrentlyEnergizingTower = false;

        [SerializeField]
        private bool isCurrentlyRepairingTower = false;

        [SerializeField]
        private bool isCurrentlyTakingEnergyFromTower = false;

        private Image energyImage;
        private Image overflowEnergyImage;
        private Rigidbody playerRigidbody;
        private float horizontalMovement;

        #endregion

        void Start()
        {
            this.playerRigidbody = GetComponent<Rigidbody>();

            this.energyImage = this.transform
                .Find("EnergyCanvas")
                .Find("energy_image")
                .GetComponent<Image>();
            this.overflowEnergyImage = this.transform
                .Find("EnergyCanvas")
                .Find("overflow_energy_image")
                .GetComponent<Image>();

            this.mainReserveEnergy = Random.Range(50f, this.maximumEnergy);
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

            //Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            //float rayLength;

            //if (groundPlane.Raycast(cameraRay, out rayLength))
            //{
            //    var pointToLook = cameraRay.GetPoint(rayLength);
            //    this.transform.LookAt(new Vector3(pointToLook.x, this.transform.position.y, pointToLook.z));
            //}

            //Camera.main.transform.LookAt(this.transform);
        }

        void UpdateEnergyCanvas()
        {
            this.energyImage.fillAmount = this.energy / this.maximumEnergy;
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
            if (this.isCurrentlyEnergizingTower)
            {
                return;
            }

            Debug.Log("Energizing nearest tower ...");
            StartCoroutine(GiveEnergy());
        }

        public void RepairNearestTower()
        {
            if(this.isCurrentlyRepairingTower)
            {
                return;
            }

            Debug.Log("Repairing nearest tower ...");
            StartCoroutine(RepairTower());
        }

        public void TakeEnergyFromNearestTower()
        {
            if (this.isCurrentlyTakingEnergyFromTower)
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
                ////Debug.Log(string.Format(
                ////    "cost [{0}] = {1} + {2}",
                ////    takeFromOverflow + takeFromReserves,
                ////    takeFromOverflow,
                ////    takeFromReserves));

                this.overflowReserveEnergy -= takeFromOverflow;
            }

            this.mainReserveEnergy -= takeFromReserves;

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

            yield return new WaitForSeconds(this.repairAbilityCooldown);
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