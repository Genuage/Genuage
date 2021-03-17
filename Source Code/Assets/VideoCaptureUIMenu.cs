using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VR_Interaction
{


    public class VideoCaptureUIMenu : MonoBehaviour
    {
        public Button CloseButton;

        public Toggle ManualTimingToggle;
        public Toggle AutomaticTimingToggle;
        public Text DurationText;
        //WAYPÖINT TIMING BUTTONS
        public GameObject WaypointsTimingButtonsContainer;
        public GameObject TimingButtonPrefab;
        public Button ApplyTimingChangesButton;
        public Dictionary<int, VideoCaptureTimeButton> WaypointTimingButtonList;

        //CLOUDSTATE TIMING BUTTONS
        public GameObject CloudStatesTimingButtonsContainer;
        public Dictionary<int, VideoCaptureTimeButton> CloudStatesTimingButtonList;
        public Button NewCloudStateButton;

        public Button RecordButton;
        public bool AutomaticModeOn = true;
        public int defaultTransitionTime = 5;
        private void Awake()
        {
            WaypointTimingButtonList = new Dictionary<int, VideoCaptureTimeButton>();
            CloudStatesTimingButtonList = new Dictionary<int, VideoCaptureTimeButton>();
            AutomaticTimingToggle.onValueChanged.AddListener(CheckToggle);
            ApplyTimingChangesButton.onClick.AddListener(TimingEditFinished);
            SetTimingButtonsActive(false);
            ApplyTimingChangesButton.interactable = false;
            RecordButton.onClick.AddListener(LaunchRecordingEvent);
            NewCloudStateButton.onClick.AddListener(LaunchAddCloudStateEvent);

            if (CloseButton)
            {
                CloseButton.onClick.AddListener(DeleteSelf);
            }
            //DEBUG
            /**
            CreateTimeButton(0, 0);
            CreateTimeButton(1, 0);
            CreateTimeButton(2, 0);
            CreateTimeButton(3, 0);
            **/
        }

        public delegate void OnDeleteEvent();
        public event OnDeleteEvent OnDelete;
        private void DeleteSelf()
        {
            if (OnDelete != null)
            {
                OnDelete();
            }

            Destroy(this.gameObject);
        }
        //Toggle Auto and Manual Modes

        public delegate void OnRecordStartEvent();
        public event OnRecordStartEvent OnRecordStart;

        private void LaunchRecordingEvent()
        {
            if(OnRecordStart != null)
            {
                OnRecordStart();
            }
        }
        private void CheckToggle(bool value)
        {
            if(value == true)
            {
                //SWITCH TO AUTOMATIC MODE
                SetTimingButtonsActive(false);
                int index = 0;
                foreach (KeyValuePair<int, VideoCaptureTimeButton> kvp in WaypointTimingButtonList)
                {
                    kvp.Value.SetTime(index * defaultTransitionTime);
                    index++;
                }
                ApplyTimingChangesButton.interactable = false;
                TimingEditFinished();
                AutomaticModeOn = true;
            }
            else
            {
                //SWITCH TO MANUAL MODE
                SetTimingButtonsActive(true);
                ApplyTimingChangesButton.interactable = true;
                AutomaticModeOn = true;
            }
        }

        public void ChangeDurationText(int newDuration)
        {
            DurationText.text = "Duration : " + newDuration + " s";
        }

        //TimingButtons
        public void CreateTimeButton(int id, int Time)
        {
            GameObject go = Instantiate(TimingButtonPrefab) as GameObject;
            go.transform.SetParent(WaypointsTimingButtonsContainer.transform, false);
            VideoCaptureTimeButton t = go.GetComponent<VideoCaptureTimeButton>();
            WaypointTimingButtonList.Add(id,t);
            t.SetID(id);
            t.SetTime(Time);
            t.OnDelete += DeleteWaypointTiming;
            UpdateDuration();
            if (AutomaticModeOn)
            {
                t.DeactivateButtons();
            }
        }

        private void UpdateDuration()
        {
            int durationmax = 0;
            foreach (KeyValuePair <int,VideoCaptureTimeButton> t in WaypointTimingButtonList)
            {
                if(durationmax < t.Value.Time)
                {
                    durationmax = t.Value.Time;
                }
            }
            ChangeDurationText(durationmax);

        }
        public delegate void OnTimingEditEvent();
        public event OnTimingEditEvent OnTimingEdit;

        private void TimingEditFinished()
        {
            UpdateDuration();
            if(OnTimingEdit != null)
            {
                OnTimingEdit();
            }
        }

        private void SetTimingButtonsActive(bool value)
        {
            if (value == true)
            {
                foreach(KeyValuePair<int, VideoCaptureTimeButton> kvp in WaypointTimingButtonList)
                {
                    kvp.Value.ActivateButtons();
                }
                foreach (KeyValuePair<int, VideoCaptureTimeButton> kvp in CloudStatesTimingButtonList)
                {
                    kvp.Value.ActivateButtons();
                }


            }
            else
            {
                foreach (KeyValuePair<int, VideoCaptureTimeButton> kvp in CloudStatesTimingButtonList)
                {
                    kvp.Value.DeactivateButtons();
                }
                foreach (KeyValuePair<int, VideoCaptureTimeButton> kvp in CloudStatesTimingButtonList)
                {
                    kvp.Value.DeactivateButtons();
                }

            }
        }

        public List<CameraWaypoint> GetWaypointTimingInfo()
        {
            List<CameraWaypoint> list = new List<CameraWaypoint>();
            foreach (KeyValuePair<int, VideoCaptureTimeButton> t in WaypointTimingButtonList)
            {
                list.Add(new CameraWaypoint(t.Value.ID, t.Value.Time));
            }

            return list;
        }

        public List<CloudStateInTime> GetCloudStateTimingInfo()
        {
            List<CloudStateInTime> list = new List<CloudStateInTime>();
            foreach (KeyValuePair<int, VideoCaptureTimeButton> t in CloudStatesTimingButtonList)
            {
                list.Add(new CloudStateInTime(t.Value.ID, t.Value.Time, null, 0f, Vector3.zero, Vector3.zero, null));
            }

            return list;

        }

        public delegate void OnAddCloudStateEvent();
        public event OnAddCloudStateEvent OnAddCloudState;

        private void LaunchAddCloudStateEvent()
        {

            if (OnAddCloudState != null)
            {
                OnAddCloudState();
            }
        }

        public void CreateCloudStateTimingButton(int id, int Time)
        {
            GameObject go = Instantiate(TimingButtonPrefab) as GameObject;
            go.transform.SetParent(CloudStatesTimingButtonsContainer.transform, false);
            VideoCaptureTimeButton t = go.GetComponent<VideoCaptureTimeButton>();
            CloudStatesTimingButtonList.Add(id, t);
            t.SetID(id);
            t.SetTime(Time);
            t.OnDelete += DeleteCloudStateTiming;
            UpdateDuration();
            if (AutomaticModeOn)
            {
                t.DeactivateButtons();
            }
        }
        public delegate void OnWaypointDeletedEvent(int id);
        public event OnWaypointDeletedEvent OnWaypointDeleted;
        private void DeleteWaypointTiming(int id)
        {
            //WaypointTimingButtonList[id].OnDelete -= DeleteWaypointTiming;
            Destroy(WaypointTimingButtonList[id].gameObject);
            WaypointTimingButtonList.Remove(id);
            if (OnWaypointDeleted != null)
            {
                OnWaypointDeleted(id);
            }

        }




        public delegate void OnCloudStateDeletedEvent(int id);
        public event OnCloudStateDeletedEvent OnCloudStateDeleted;
        private void DeleteCloudStateTiming(int id)
        {
            CloudStatesTimingButtonList[id].OnDelete -= DeleteCloudStateTiming;
            Destroy(CloudStatesTimingButtonList[id].gameObject);
            CloudStatesTimingButtonList.Remove(id);
            if (OnCloudStateDeleted != null)
            {
                OnCloudStateDeleted(id);
            }

        }

    }
}