/**
Vincent Casamayou
RIES GROUP
SMAP Animation System
29/05/2020
**/


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace Display
{


    public class AnimateObject : MonoBehaviour
    {

        public bool isRectTransform = false;
        CloudData data;
        public float indexkey = 0.0f;

        public float keyframeTimestep = 5.0f; // Time the object takes to move from one frame to the next

        public float timestep = 5f;

        public float animationTime;
        public float AnimationDuration = 30; //in seconds


        RectTransform UItransform;

        Animation anim;

        public Keyframe keyRotationW = new Keyframe();
        public Keyframe keyRotationX = new Keyframe();
        public Keyframe keyRotationY = new Keyframe();
        public Keyframe keyRotationZ = new Keyframe();

        public Keyframe keyScaleX = new Keyframe();
        public Keyframe keyScaleY = new Keyframe();
        public Keyframe keyScaleZ = new Keyframe();

        public Keyframe keyPositionX = new Keyframe();
        public Keyframe keyPositionY = new Keyframe();
        public Keyframe keyPositionZ = new Keyframe();

        //Only for RectTransform (ClippingPlane)
        public Keyframe keyHeight = new Keyframe();
        public Keyframe keyWidth = new Keyframe();

        public AnimationClip clip;

        AnimationCurve curveRotationW;
        AnimationCurve curveRotationX;
        AnimationCurve curveRotationY;
        AnimationCurve curveRotationZ;

        AnimationCurve curveScaleX;
        AnimationCurve curveScaleY;
        AnimationCurve curveScaleZ;

        public AnimationCurve curvePositionX;
        public AnimationCurve curvePositionY;
        public AnimationCurve curvePositionZ;

        //Only for RectTransform (ClippingPlane)
        AnimationCurve curveHeight;
        AnimationCurve curveWidth;

        //CloudStatus Variables
        private Dictionary<int, VR_Interaction.CloudStateInTime> CloudStatesDict;

        void Awake()
        {
            CloudStatesDict = new Dictionary<int, VR_Interaction.CloudStateInTime>();
            anim = gameObject.AddComponent(typeof(Animation)) as Animation;
            anim.playAutomatically = false;
            clip = new AnimationClip();
            clip.legacy = true;

            curveRotationW = new AnimationCurve(keyRotationW);
            curveRotationX = new AnimationCurve(keyRotationX);
            curveRotationY = new AnimationCurve(keyRotationY);
            curveRotationZ = new AnimationCurve(keyRotationZ);

            curveScaleX = new AnimationCurve(keyScaleX);
            curveScaleY = new AnimationCurve(keyScaleY);
            curveScaleZ = new AnimationCurve(keyScaleZ);

            curvePositionX = new AnimationCurve(keyPositionX);
            curvePositionY = new AnimationCurve(keyPositionY);
            curvePositionZ = new AnimationCurve(keyPositionZ);


            //ClearAnimation();
            /**
            if (isRectTransform)
            {
                UItransform = this.gameObject.GetComponent<RectTransform>();
                curveHeight = new AnimationCurve(keyHeight);
                curveWidth = new AnimationCurve(keyWidth);

                //Initialize UI Size
                curveHeight.MoveKey(0, new Keyframe(0, UItransform.rect.height));
                curveWidth.MoveKey(0, new Keyframe(0, UItransform.rect.width));

            }
            else
            {

                //Initialize Object Position
                curveRotationW.MoveKey(0, new Keyframe(0, this.gameObject.transform.localRotation.w));
                curveRotationX.MoveKey(0, new Keyframe(0, this.gameObject.transform.localRotation.x));
                curveRotationY.MoveKey(0, new Keyframe(0, this.gameObject.transform.localRotation.y));
                curveRotationZ.MoveKey(0, new Keyframe(0, this.gameObject.transform.localRotation.z));

                curveScaleX.MoveKey(0, new Keyframe(0, this.gameObject.transform.localScale.x));
                curveScaleY.MoveKey(0, new Keyframe(0, this.gameObject.transform.localScale.y));
                curveScaleZ.MoveKey(0, new Keyframe(0, this.gameObject.transform.localScale.z));

                curvePositionX.MoveKey(0, new Keyframe(0, this.gameObject.transform.localPosition.x));
                curvePositionY.MoveKey(0, new Keyframe(0, this.gameObject.transform.localPosition.y));
                curvePositionZ.MoveKey(0, new Keyframe(0, this.gameObject.transform.localPosition.z));

            }

            UpdateAnimation();
            **/
        }

        public  void ClearAnimation()
        {
            clip.ClearCurves();

            curveRotationW = new AnimationCurve(keyRotationW);
            curveRotationX = new AnimationCurve(keyRotationX);
            curveRotationY = new AnimationCurve(keyRotationY);
            curveRotationZ = new AnimationCurve(keyRotationZ);

            curveScaleX = new AnimationCurve(keyScaleX);
            curveScaleY = new AnimationCurve(keyScaleY);
            curveScaleZ = new AnimationCurve(keyScaleZ);

            curvePositionX = new AnimationCurve(keyPositionX);
            curvePositionY = new AnimationCurve(keyPositionY);
            curvePositionZ = new AnimationCurve(keyPositionZ);

        }
        //DONE
        public void InitializeFirstKeyframe(Transform TransformToAdd)
        {
            //Initialize Object Position
            curveRotationW.MoveKey(0, new Keyframe(0, TransformToAdd.localRotation.w));
            curveRotationX.MoveKey(0, new Keyframe(0, TransformToAdd.localRotation.x));
            curveRotationY.MoveKey(0, new Keyframe(0, TransformToAdd.localRotation.y));
            curveRotationZ.MoveKey(0, new Keyframe(0, TransformToAdd.localRotation.z));

            curveScaleX.MoveKey(0, new Keyframe(0, TransformToAdd.localScale.x));
            curveScaleY.MoveKey(0, new Keyframe(0, TransformToAdd.localScale.y));
            curveScaleZ.MoveKey(0, new Keyframe(0, TransformToAdd.localScale.z));

            curvePositionX.MoveKey(0, new Keyframe(0, TransformToAdd.localPosition.x));
            curvePositionY.MoveKey(0, new Keyframe(0, TransformToAdd.localPosition.y));
            curvePositionZ.MoveKey(0, new Keyframe(0, TransformToAdd.localPosition.z));


        }
        //DONE
        public int AddKeyframe(Transform TransformToAdd, float time)
        {
            indexkey++;

            int indexKeyReturn = 0;
            //animationTime = keyframeTimestep * indexkey;
            Debug.Log(TransformToAdd);
            Debug.Log(curveRotationW);
            //FOR UI ELEMENTS
            if (isRectTransform)
            {


                indexKeyReturn = curveWidth.AddKey(time, UItransform.rect.width);
                curveHeight.AddKey(time, UItransform.rect.height);

                curvePositionX.AddKey(time, UItransform.anchoredPosition.x);
                curvePositionY.AddKey(time, UItransform.anchoredPosition.y);

            }
            //FOR GAMEOBJECTS
            else
            {

                curveRotationW.AddKey(time, TransformToAdd.localRotation.w);
                curveRotationX.AddKey(time, TransformToAdd.localRotation.x);
                curveRotationY.AddKey(time, TransformToAdd.localRotation.y);
                curveRotationZ.AddKey(time, TransformToAdd.localRotation.z);

                curveScaleX.AddKey(time, TransformToAdd.localScale.x);
                curveScaleY.AddKey(time, TransformToAdd.localScale.y);
                curveScaleZ.AddKey(time, TransformToAdd.localScale.z);

                curvePositionX.AddKey(time, TransformToAdd.localPosition.x);
                curvePositionY.AddKey(time, TransformToAdd.localPosition.y);
                indexKeyReturn = curvePositionZ.AddKey(time, TransformToAdd.localPosition.z);
                
            }


            //indexkey++;

            UpdateAnimation();
            return indexKeyReturn;
        }

        public Vector3 GetPositionatTime(float Time)
        {
            Vector3 result = Vector3.zero;
            result.x = curvePositionX.Evaluate(Time);
            result.y = curvePositionY.Evaluate(Time);
            result.z = curvePositionZ.Evaluate(Time);
            return result;
        }

        //Used to trigger specific event, for now only change colormap 
        public void AddAnimationEvent(VR_Interaction.CloudStateInTime state)
        {
            if (!CloudStatesDict.ContainsKey(state.ID))
            {
                CloudStatesDict.Add(state.ID, state);
            }
            else
            {
                Debug.Log("Tried to add event at id " + state.ID + " but an event already had this id !");
            }
            AnimationEvent evt = new AnimationEvent();
            evt.time = state.Time;

            evt.intParameter = state.ID;
            evt.functionName = "UpdateCloudStatus";
            clip.AddEvent(evt);

        }

        public void ClearEvents()
        {
            AnimationEvent[] allEvents = clip.events;
            if (allEvents.Length != 0)
            {
                var eventsList = allEvents.ToList();
                for (int j = 0; j < eventsList.Count; j++)
                {
                    eventsList.RemoveAt(j);
                }
                clip.events = eventsList.ToArray();
            }
            CloudStatesDict.Clear();
            UpdateAnimation();
        }

        public void UpdateColorMap(string ColorMapName)
        {
            CloudUpdater.instance.ChangeCurrentColorMap(ColorMapName);
        }

        public void UpdateCloudStatus(int CloudStateID)
        {
            //Call cloudupdater update mesh
            CloudUpdater.instance.OverrideBoxScale(CloudStatesDict[CloudStateID].BoxScale);
            CloudUpdater.instance.OverrideMesh(CloudStatesDict[CloudStateID].Mesh);
            CloudUpdater.instance.ChangePointSize(CloudStatesDict[CloudStateID].PointSize);
            CloudUpdater.instance.ChangeCloudScale(CloudStatesDict[CloudStateID].Scale);
            UpdateColorMap(CloudStatesDict[CloudStateID].ColorMap);
        }

        //DONE
        public int UpdateKeyframe(int indexKey, int newTime, Transform objectTransform)
        {
            //animationTime = keyframeTimestep * (float)index;
            int indexKeyReturn = 0;

            //For UI ELEMENTS
            if (isRectTransform)
            {

                Keyframe TMPkeyHeight = new Keyframe(newTime, UItransform.rect.height);
                curveHeight.MoveKey(indexKey, TMPkeyHeight);
                Keyframe TMPkeyWidth = new Keyframe(newTime, UItransform.rect.width);
                curveWidth.MoveKey(indexKey, TMPkeyWidth);

                Keyframe TMPkeyPositionX = new Keyframe(newTime, UItransform.anchoredPosition.x);
                curvePositionX.MoveKey(indexKey, TMPkeyPositionX);
                Keyframe TMPkeyPositionY = new Keyframe(newTime, UItransform.anchoredPosition.y);
                indexKeyReturn = curvePositionY.MoveKey(indexKey, TMPkeyPositionY);
            }

            //FOR GAMEOBJECTS
            else
            {
                Keyframe TMPkeyRotationW = new Keyframe(newTime, objectTransform.localRotation.w);
                curveRotationW.MoveKey(indexKey, TMPkeyRotationW);
                Keyframe TMPkeyRotationX = new Keyframe(newTime, objectTransform.localRotation.x);
                curveRotationX.MoveKey(indexKey, TMPkeyRotationX);
                Keyframe TMPkeyRotationY = new Keyframe(newTime, objectTransform.localRotation.y);
                curveRotationY.MoveKey(indexKey, TMPkeyRotationY);
                Keyframe TMPkeyRotationZ = new Keyframe(newTime, objectTransform.localRotation.z);
                curveRotationZ.MoveKey(indexKey, TMPkeyRotationZ);

                Keyframe TMPkeyScaleX = new Keyframe(newTime, objectTransform.localScale.x);
                curveScaleX.MoveKey(indexKey, TMPkeyScaleX);
                Keyframe TMPkeyScaleY = new Keyframe(newTime, objectTransform.localScale.y);
                curveScaleY.MoveKey(indexKey, TMPkeyScaleY);
                Keyframe TMPkeyScaleZ = new Keyframe(newTime, objectTransform.localScale.z);
                curveScaleZ.MoveKey(indexKey, TMPkeyScaleZ);

                Keyframe TMPkeyPositionX = new Keyframe(newTime, objectTransform.localPosition.x);
                curvePositionX.MoveKey(indexKey, TMPkeyPositionX);
                Keyframe TMPkeyPositionY = new Keyframe(newTime, objectTransform.localPosition.y);
                curvePositionY.MoveKey(indexKey, TMPkeyPositionY);
                Keyframe TMPkeyPositionZ = new Keyframe(newTime, objectTransform.localPosition.z);
                indexKeyReturn = curvePositionZ.MoveKey(indexKey, TMPkeyPositionZ);
            }


            UpdateAnimation();
            return indexKeyReturn;
        }



        public void UpdateAnimation()
        {
            //FOR UI ELEMENTS
            if (isRectTransform)
            {
                clip.SetCurve("", typeof(RectTransform), "m_SizeDelta.x", curveWidth);
                clip.SetCurve("", typeof(RectTransform), "m_SizeDelta.y", curveHeight);

                clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curvePositionX);
                clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.y", curvePositionY);
            }
            //FOR GAMEOBJECTS
            else
            {
                clip.SetCurve("", typeof(Transform), "localRotation.w", curveRotationW);
                clip.SetCurve("", typeof(Transform), "localRotation.x", curveRotationX);
                clip.SetCurve("", typeof(Transform), "localRotation.y", curveRotationY);
                clip.SetCurve("", typeof(Transform), "localRotation.z", curveRotationZ);

                clip.SetCurve("", typeof(Transform), "localScale.x", curveScaleX);
                clip.SetCurve("", typeof(Transform), "localScale.y", curveScaleY);
                clip.SetCurve("", typeof(Transform), "localScale.z", curveScaleZ);

                clip.SetCurve("", typeof(Transform), "localPosition.x", curvePositionX);
                clip.SetCurve("", typeof(Transform), "localPosition.y", curvePositionY);
                clip.SetCurve("", typeof(Transform), "localPosition.z", curvePositionZ);

            }


            anim.AddClip(clip, clip.name);
            anim.clip = clip;

        }


        public void PlayAnimation()
        {
            Debug.Log("PlayAnimation Called");
            if (!anim.isPlaying)
            {
                Debug.Log("Animation playing");
                anim.Play();

            }
            else
            {
                Debug.Log("Animation stop");
                anim.Stop();
            }
        }

        public void SetAnimationSpeed(float animSpeed)
        {
            keyframeTimestep = timestep / animSpeed;
            Debug.Log("Timestep is " + keyframeTimestep);
        }


        //HERE IF YOU WANT TO REMOVE NEW VARIABLES
        public void RemoveCurves(int i)
        {
            //FOR UI ELEMENTS
            if (isRectTransform)
            {
                curveHeight.RemoveKey(i);
                curveWidth.RemoveKey(i);
                curvePositionX.RemoveKey(i);
                curvePositionY.RemoveKey(i);
            }
            //FOR GAMEOBJECTS
            else
            {
                curveRotationW.RemoveKey(i);
                curveRotationX.RemoveKey(i);
                curveRotationY.RemoveKey(i);
                curveRotationZ.RemoveKey(i);

                curveScaleX.RemoveKey(i);
                curveScaleY.RemoveKey(i);
                curveScaleZ.RemoveKey(i);

                curvePositionX.RemoveKey(i);
                curvePositionY.RemoveKey(i);
                curvePositionZ.RemoveKey(i);
            }

        }


        public void ShiftCurves(int index, int shift = 0)
        {
            int key_to_shift = index - shift;

            if (key_to_shift >= curveRotationW.keys.Length)
            {
                key_to_shift = curveRotationW.keys.Length - 1;
            }
            else if (key_to_shift <= 0)
            {
                key_to_shift = 1;
            }

            //FOR UI ELEMENTS
            if (isRectTransform)
            {
                curveHeight.MoveKey(index, curveHeight.keys[key_to_shift]);
                curveWidth.MoveKey(index, curveWidth.keys[key_to_shift]);

                curvePositionX.MoveKey(index, curvePositionX.keys[key_to_shift]);
                curvePositionY.MoveKey(index, curvePositionY.keys[key_to_shift]);
            }
            //FOR GAMEOBJECTS
            else
            {
                curveRotationW.MoveKey(index, curveRotationW.keys[key_to_shift]);
                curveRotationX.MoveKey(index, curveRotationX.keys[key_to_shift]);
                curveRotationY.MoveKey(index, curveRotationY.keys[key_to_shift]);
                curveRotationZ.MoveKey(index, curveRotationZ.keys[key_to_shift]);

                curveScaleX.MoveKey(index, curveScaleX.keys[key_to_shift]);
                curveScaleY.MoveKey(index, curveScaleY.keys[key_to_shift]);
                curveScaleZ.MoveKey(index, curveScaleZ.keys[key_to_shift]);

                curvePositionX.MoveKey(index, curvePositionX.keys[key_to_shift]);
                curvePositionY.MoveKey(index, curvePositionY.keys[key_to_shift]);
                curvePositionZ.MoveKey(index, curvePositionZ.keys[key_to_shift]);

            }

        }


        public void RemoveAnimation()
        {
            for (int i = 0; i < curvePositionX.keys.Length; i++)
            {
                RemoveCurves(i);
            }


            indexkey = 0;

            clip.ClearCurves();

            anim.RemoveClip(clip);
        }


        public void RemoveKeyframe(int index)
        {
            float timestamp = curvePositionX.keys[index].time;

            animationTime = animationTime - keyframeTimestep;

            for (int i = index; i < curvePositionX.keys.Length; i++)
            {
                Debug.Log("Shift Keyframe " + i);
                if (i == curvePositionX.keys.Length - 1)
                {
                    Debug.Log("Last Curve removed");
                    RemoveCurves(i);
                    break;
                }
                else
                {
                    ShiftCurves(i, -1);
                }
            }



            //Remove Event
            AnimationEvent[] allEvents = clip.events;
            if (allEvents.Length != 0)
            {
                var eventsList = allEvents.ToList();
                for (int j = 0; j < eventsList.Count; j++)
                {
                    if (eventsList[j].time == timestamp)
                    {
                        eventsList.RemoveAt(j);
                    }
                }
                clip.events = eventsList.ToArray();
            }

            UpdateAnimation();

        }


    }
}