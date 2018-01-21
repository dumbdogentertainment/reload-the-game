namespace DumbDogEntertainment
{
    using System.Collections.Generic;

    using UnityEngine;

    public class InputManager : MonoBehaviour
    {
        #region Static instance
        public static InputManager Instance = null;

        delegate void KeyPressedDelegate();
        Dictionary<KeyCode, KeyPressedDelegate> KeyPressedDelegateDictionary;

        void Awake()
        {
            Instance = this;

            this.KeyPressedDelegateDictionary = new Dictionary<KeyCode, KeyPressedDelegate>
            {
                { KeyCode.Q, Player.Instance.TakeEnergyFromNearestTower },
                { KeyCode.E, Player.Instance.EnergizeNearestTower },
                { KeyCode.S, Player.Instance.RepairNearestTower },
                { KeyCode.W, Player.Instance.EnergizeSelf }
            };
        }

        void OnApplicationQuit()
        {
            Instance = null;
        }
        #endregion

        private void OnGUI()
        {
            KeyCode keyPressed = Event.current.keyCode;
            if (this.KeyPressedDelegateDictionary.ContainsKey(keyPressed))
            {
                this.KeyPressedDelegateDictionary[keyPressed]();
            }

        }

        void Update()
        {

        }

        public float GetHorizontalAxisInput()
        {
            return Input.GetAxisRaw("Horizontal");
        }
    }
}