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

        public float energy;
        public float maximumEnergy = 100f;
        public float boostedMaximumEnergy = 150f;
        private Image energyImage;
        private Image boostedEnergyImage;

        public float speed = 5f;
        public float boostedSpeed = 8f;

        [SerializeField]
        private bool isCurrentlyBoosted = false;
        [SerializeField]
        private bool isCurrentlyEnergizing = false;

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
            this.boostedEnergyImage = this.transform
                .Find("EnergyCanvas")
                .Find("boosted_energy_image")
                .GetComponent<Image>();

            this.energy = Random.Range(50f, this.maximumEnergy);
        }

        void Update()
        {
            // regen energy
            this.energy = Mathf.Clamp(this.energy + 0.005f, 0, this.maximumEnergy);

            // degen boosted energy
            if (this.isCurrentlyBoosted)
            {
                this.energy -= 0.03f;
            }

            UpdateEnergyCanvas();
        }

        void UpdateEnergyCanvas()
        {
            this.energyImage.fillAmount = this.isCurrentlyBoosted ?
                this.energy / this.boostedMaximumEnergy :
                this.energy / this.maximumEnergy;

            this.boostedEnergyImage.enabled = this.isCurrentlyBoosted;
        }

        void FixedUpdate()
        {
            this.horizontalMovement = InputManager.Instance.GetHorizontalAxisInput();
        }

        void LateUpdate()
        {
            Vector3 desiredMovement = (this.isCurrentlyBoosted ? this.boostedSpeed : this.speed) *
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

        public void TakeEnergyFromNearestTower()
        {
            Debug.Log("Taking energy from nearest tower ...");
        }

        public void EnergizeNearestTower()
        {
            if (this.isCurrentlyEnergizing)
            {
                return;
            }

            Debug.Log("Energizing nearest tower ...");
            StartCoroutine(GiveEnergy());
        }

        public void RepairNearestTower()
        {
            Debug.Log("Repairing nearest tower ...");
        }

        public void EnergizeSelf()
        {
            if (this.isCurrentlyBoosted)
            {
                return;
            }

            Debug.Log("Giving myself a quick energy boost ...");
            StartCoroutine(BoostSelf());
        }

        /// <summary>
        /// Boost self .. gives additional energy (up to a certain maximum)
        /// that degrades over time .. also, grants increased speed.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BoostSelf()
        {
            this.isCurrentlyBoosted = true;
            this.energy = Mathf.Clamp(this.energy + 25f, 0, this.boostedMaximumEnergy);
            yield return new WaitForSeconds(4);
            this.isCurrentlyBoosted = false;
        }

        private IEnumerator GiveEnergy()
        {
            this.isCurrentlyEnergizing = true;
            this.energy = Mathf.Clamp(this.energy - 15, 0, this.boostedMaximumEnergy);
            yield return new WaitForSeconds(2);
            this.isCurrentlyEnergizing = false;
        }
    }
}