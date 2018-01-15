namespace DumbDogEntertainment
{
    using UnityEngine;

    public class ShellBehavior : MonoBehaviour
    {
        public float TimeUntilDestroy = 2f;

        void Start()
        {

        }

        void Update()
        {
            if(this.TimeUntilDestroy <= 0)
            {
                Destroy(this.gameObject);
            }

            this.transform.position -= new Vector3(0, 0, 0.1f);
            this.TimeUntilDestroy -= Time.deltaTime;
        }
    }
}