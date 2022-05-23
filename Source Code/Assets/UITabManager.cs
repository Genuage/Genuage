using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopInterface
{
    public class TabButton : IButtonScript
    {
        public int tabID;
        private void Awake()
        {
            button = gameObject.GetComponent<Button>();
            initializeClickEvent();
        }

        public override void Execute()
        {
            if (OnButtonClicked != null)
            {
                OnButtonClicked(this.tabID);
            }
        }

        public delegate void OnButtonClickedEvent(int id);
        public event OnButtonClickedEvent OnButtonClicked;

    }

    public class UITabManager : MonoBehaviour
    {
        //Manages opening and closing multiple tabs.

        public List<Button> ButtonList;
        public List<GameObject> TabGameObjectList;

        private void Awake()
        {
            if(ButtonList.Count != TabGameObjectList.Count)
            {
                Debug.Log("ERROR : mismatch between number of tabs and number of buttons.");
            }

            int id = 0;

            foreach(Button b in ButtonList)
            {
                b.gameObject.AddComponent<TabButton>();
                b.gameObject.GetComponent<TabButton>().tabID = id;
                b.gameObject.GetComponent<TabButton>().OnButtonClicked += ButtonPressed;
                id++;
            }
        }

        private void ButtonPressed(int id)
        {
            ChangeTab(id);
        }

        private void ChangeTab(int TabID)
        {
            DeactivateAllTabs();
            ActivateTab(TabID);
        }

        private void ActivateTab(int id)
        {
            if (id < TabGameObjectList.Count)
            {
                TabGameObjectList[id].SetActive(true);
            }
        }

        private void DeactivateAllTabs()
        {
            foreach(GameObject go in TabGameObjectList)
            {
                go.SetActive(false);
            }
        }

    }
}