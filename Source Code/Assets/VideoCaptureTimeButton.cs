using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VR_Interaction
{


    public class VideoCaptureTimeButton : MonoBehaviour
    {
        public int Time = 5;
        public int ID = 1;
        public InputField input;
        public Text TextID;
        public Button PlusFiveButton;
        public Button MinusFiveButton;
        public Button PlusOneButton;
        public Button MinusOneButton;
        public Button DeleteButton;

        private void Awake()
        {
            PlusFiveButton.onClick.AddListener(AddFive);
            PlusOneButton.onClick.AddListener(AddOne);
            MinusFiveButton.onClick.AddListener(SubstractFive);
            MinusOneButton.onClick.AddListener(SubstractOne);
            DeleteButton.onClick.AddListener(Delete);

            //Time = 30;
        }
        public void DeactivateButtons()
        {
            PlusFiveButton.interactable = false;
            MinusFiveButton.interactable = false;
            PlusOneButton.interactable = false;
            MinusOneButton.interactable = false;
        }

        public void ActivateButtons()
        {
            PlusFiveButton.interactable = true;
            MinusFiveButton.interactable = true;
            PlusOneButton.interactable = true;
            MinusOneButton.interactable = true;
        }
        public void SetID(int id)
        {
            ID = id;
            TextID.text = id.ToString();
        }

        public void SetTime(int time)
        {
            Time = time;
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            if (Time < 0)
            {
                Time = 0;
            }
            input.text = Time.ToString() + " s";
        }

        private void AddFive()
        {
            Time += 5;
            UpdateOutput();
        }

        private void AddOne()
        {
            Time += 1;
            UpdateOutput();
        }

        private void SubstractFive()
        {
            Time -= 5;
            UpdateOutput();
        }
        private void SubstractOne()
        {
            Time -= 1;
            UpdateOutput();
        }

        public delegate void OnDeleteEvent(int id);
        public event OnDeleteEvent OnDelete;

        private void Delete()
        {
            if (OnDelete != null)
            {
                OnDelete(ID);
            }

        }

    }
}