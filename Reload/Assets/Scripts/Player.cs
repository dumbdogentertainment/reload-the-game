namespace DumbDogEntertainment
{
    using UnityEngine;

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

        public float playerSpeed = 5f;
        private Rigidbody playerRigidbody;

        private float horizontalMovement;

        #endregion

        void Start()
        {
            this.playerRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            var debugRayStartPosition = new Vector3(
                this.transform.position.x,
                this.transform.position.y,
                this.transform.position.z);

            Debug.DrawRay(
                debugRayStartPosition,
                this.transform.TransformDirection(Vector3.forward) * 5,
                Color.red);
        }

        void FixedUpdate()
        {
            this.horizontalMovement = InputManager.Instance.GetHorizontalAxisInput();
        }

        void LateUpdate()
        {
            Vector3 desiredMovement = this.playerSpeed * new Vector3(
                this.horizontalMovement,
                0,
                0);

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
            Debug.Log("Energizing nearest tower ...");
        }

        public void RepairNearestTower()
        {
            Debug.Log("Repairing nearest tower ...");
        }

        public void EnergizeSelf()
        {
            Debug.Log("Giving myself a quick energy boost ...");
        }
    }
}