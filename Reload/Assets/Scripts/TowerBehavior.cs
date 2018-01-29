namespace DumbDogEntertainment
{
    using System.Collections;
    using System.Linq;

    using DumbDogEntertainment.ScriptableObjects;
    using UnityEngine;
    using UnityEngine.UI;

    public class TowerBehavior : MonoBehaviour
    {
        public float maxEnergy = 100f;
        public float currentEnergy;
        public float energyRechargeRate = 1f;
        public float energyRechargeAmount = 2f;
        public float energyRechargeCooldown;

        public float maxHealth = 100f;
        public float currentHealth;

        public GameObject shellPrefab;
        private Projectile towerProjectile;
        public float fireCooldown;
        public float maxRange = 12.5f;

        [SerializeField]
        Transform turretTransform;

        [SerializeField]
        Transform muzzleTransform;

        [SerializeField]
        private Transform energyTransform;

        [SerializeField]
        private Transform healthTransform;

        private Image energyImage;

        private Image healthImage;

        void Awake()
        {
            this.currentHealth = this.maxHealth;

            this.currentEnergy = this.maxEnergy;
            this.energyRechargeCooldown = this.energyRechargeRate;

            this.towerProjectile = this.shellPrefab.GetComponent<ShellBehavior>().projectile;
            this.fireCooldown = this.towerProjectile.rateOfFire;

            this.energyImage = this.energyTransform.GetComponent<Image>();
            this.healthImage = this.healthTransform.GetComponent<Image>();
        }

        void Update()
        {
            //Debug.DrawRay(
            //this.turretTransform.position,
            //this.turretTransform.TransformDirection(Vector3.back) * 5,
            //Color.red);

            if(this.currentHealth <= 0)
            {
                this.gameObject.SetActive(false);
            }

            FireAtTargets(GameObject.FindObjectsOfType<EnemyBehavior>());
            RechargeEnergy();
            UpdateCanvas();
        }

        private void OnTriggerStay(Collider other)
        {
            if(false == other.tag.Equals("Player"))
            {
                return;
            }

            Player.Instance.connectedTower = this.gameObject.GetComponent<TowerBehavior>();

            Debug.Log("Player entered repair zone ..");
        }

        private void UpdateCanvas()
        {
            this.healthImage.fillAmount = this.currentHealth / this.maxHealth;
            this.energyImage.fillAmount = this.currentEnergy / this.maxEnergy;
        }

        private void RechargeEnergy()
        {
            this.energyRechargeCooldown -= Time.deltaTime;

            if (this.energyRechargeCooldown > 0)
            {
                return;
            }

            this.currentEnergy += this.energyRechargeAmount;
            this.currentEnergy = Mathf.Clamp(this.currentEnergy, 0f, this.maxEnergy);
            this.energyRechargeCooldown = this.energyRechargeRate;
        }

        private void FireAtTargets(EnemyBehavior[] enemies)
        {
            this.fireCooldown -= Time.deltaTime;

            EnemyBehavior[] enemiesInRange = enemies
                .Where(enemy => Vector3.Distance(this.muzzleTransform.position, enemy.transform.position).IsBetweenInclusive(3.5f, this.maxRange))
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
                this.turretTransform.localRotation.eulerAngles.z);

            // fire at current target
            if (this.fireCooldown <= 0 && this.currentEnergy >= this.towerProjectile.costToFire)
            {
                GameObject shellGameObject = (GameObject)Instantiate(
                    this.shellPrefab,
                    this.muzzleTransform.position,
                    this.muzzleTransform.rotation);

                Projectile firedProjectile = shellGameObject.GetComponent<ShellBehavior>().projectile;
                firedProjectile.targetPosition = currentTarget.transform.position;

                this.currentEnergy -= this.towerProjectile.costToFire;
                this.fireCooldown = this.towerProjectile.rateOfFire;
            }
        }

        public void ModifyEnergy(float value)
        {
            this.currentEnergy = Mathf.Clamp(this.currentEnergy + value, 0, this.maxEnergy);
        }

        public void ModifyHealth(float value)
        {
            this.currentHealth = Mathf.Clamp(this.currentHealth + value, 0, this.maxHealth);
        }

        public IEnumerator ModifyHealthOverTime(float value, float time)
        {
            float amountAdded = 0f;

            float increment = value / (time * 100f);

            while (amountAdded < value)
            {
                this.currentHealth = Mathf.Clamp(this.currentHealth + increment, 0, this.maxHealth);
                amountAdded += increment;
                yield return null;
            }
        }
    }
}