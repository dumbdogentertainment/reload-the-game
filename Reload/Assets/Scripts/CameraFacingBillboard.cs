namespace DumbDogEntertainment
{
    using UnityEngine;

    public class CameraFacingBillboard : MonoBehaviour
    {
        void Update()
        {
            this.transform.LookAt(
                this.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}