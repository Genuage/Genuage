/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement Caporal
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the Institut Curie, Insitut Pasteur and CNRS.
4. Neither the name of the Institut Curie, Insitut Pasteur and CNRS nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using Display;
using IO;


namespace DesktopInterface
{


    public class PROTOTRajectoriesButton : IButtonScript
    {
        public bool trajectorymodeON = false;
        public Mesh pointcloudMesh;

        MeshRenderer TrajectoriesRenderer;
        MeshRenderer PointsRenderer;

        List<float> TimeList;
        bool playAnimation = false;

        //other UI elements
        public Text TimeDisplayText;
        public Text FrameDisplayText;


        public Button frameforwardButton;
        public Button framebackButton;
        public Button fastforwardButton;
        public Button fastbackButton;
        public Button restartButton;
        public Button pointOverlayButton;

        public Button playButton;
        public Button ShowAllTimePointsButton;

        public InputField DragonTailInput;

        public Dropdown animationSpeedDropdown;

        public Slider frameSlider;

        public Texture2D texture;

        //Animation variables
        public int dragonTail = 2;
        public float startTime = 0f;
        public float endTime;
        public float currentTimeIndex = 0f;
        public float incrementSpeed = 1f;


        //UI to control animation
        public GameObject TrajectoryUIContainer;

        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
            framebackButton.onClick.AddListener(BackFrame);
            frameforwardButton.onClick.AddListener(ForwardFrame);

            fastforwardButton.onClick.AddListener(FastForward);
            fastbackButton.onClick.AddListener(FastBack);

            restartButton.onClick.AddListener(Restart);

            playButton.onClick.AddListener(Play);
            pointOverlayButton.onClick.AddListener(ChangePointOverlayStatus);
            ShowAllTimePointsButton.onClick.AddListener(ShowAllTimePoints);
            DragonTailInput.text = dragonTail.ToString();
            DragonTailInput.onEndEdit.AddListener(ChangeDragonTail);

            animationSpeedDropdown.onValueChanged.AddListener(ChangeSpeed);

            frameSlider.onValueChanged.AddListener(SliderFrame);

            TimeDisplayText.text = "Current Time : 0";
            FrameDisplayText.text = "Current Frame : 0";

            CloudSelector.instance.OnSelectionChange += onSelectionChange;

            texture = ColorMapManager.instance.GetColorMap("jet").texture;
        }

        public void onSelectionChange(int id)
        {
            if (trajectorymodeON)
            {
                Execute();
            }
            
        }


        public override void Execute()
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            if (trajectorymodeON == false)
            {
                
                //UIManager.instance.DeactivateSelectionButtons();
                TrajectoryUIContainer.SetActive(true);

                CloudUpdater.instance.DisplayTrajectories();                 

                TrajectoriesRenderer = data.trajectoryObject.GetComponent<MeshRenderer>();

                PointsRenderer = data.GetComponent<MeshRenderer>();
                TimeList = data.globalMetaData.timeList;

                currentTimeIndex = 0;
                frameSlider.minValue = 0f;
                frameSlider.maxValue = TimeList.Count - 1;
                frameSlider.value = 0f;

                CloudUpdater.instance.SetShaderFrame(0f, 1f);
                trajectorymodeON = true;

            }
            else
            {
                TrajectoryUIContainer.SetActive(false);

                //UIManager.instance.ActivateSelectionButtons();

                CloudUpdater.instance.HideTrajectories();
                playAnimation = false;
                trajectorymodeON = false;
    
            }
        }

        public void ChangeDragonTail(string s)
        {
            if (Int32.TryParse(s, out int j))
            {
                if (j >= 0)
                {
                    dragonTail = j;
                }
            }
            else
            {
                Debug.Log("String could not be parsed.");
                DragonTailInput.text = dragonTail.ToString();
            }
            // Output: -105
        }

        public void Restart()
        {
            currentTimeIndex = 1;
            SetShaderTimeLimit();
            ActualizeSlider();

        }

        public void SliderFrame(float value)
        {
            currentTimeIndex = value;
            SetShaderTimeLimit();
            ActualizeText();
            ActualizeSlider();

        }

        public void ForwardFrame()
        {
            if(!(currentTimeIndex +1 > TimeList.Count - 1))
            {
                ChangeFrame(1);
            }
        }

        public void BackFrame()
        {
            if ((currentTimeIndex - 1 > 0))
            {
                ChangeFrame(-1);
            }
        }

        public void FastForward()
        {
            if (!(currentTimeIndex + 10 > TimeList.Count - 1))
            {
                ChangeFrame(10);
            }
            else
            {

                currentTimeIndex = TimeList.Count - 1;
                SetShaderTimeLimit();
                ActualizeText();
                ActualizeSlider();

            }
        }

        public void FastBack()
        {
            if ((currentTimeIndex - 10 > 0))
            {
                ChangeFrame(-10);
            }
            else
            {
                currentTimeIndex = 1;
                SetShaderTimeLimit();
                ActualizeText();
                ActualizeSlider();

            }
        }

        public void ChangeFrame(int increment)
        {
            if (!playAnimation)
            {
                currentTimeIndex += increment;
                SetShaderTimeLimit();
                ActualizeText();
                ActualizeSlider();
            }

        }

        public void ShowAllTimePoints()
        {
            if (!playAnimation)
            {
                CloudUpdater.instance.SetShaderFrame(0, TimeList.Count);


            }
        }

        public void ActualizeText()
        {
            TimeDisplayText.text = "Current Time : " + TimeList[(int)currentTimeIndex];
            FrameDisplayText.text = "Current Frame : " + (int)currentTimeIndex;
        }

        public void ActualizeSlider()
        {
            frameSlider.value = currentTimeIndex;
        }

        public void ChangeSpeed(int id)
        {
            if (Single.TryParse(animationSpeedDropdown.options[id].text, out float j))
            {
                incrementSpeed = j;
            }
            else
            {
                Console.WriteLine("String could not be parsed.");
            }
        }

        public void SetShaderTimeLimit()
        {
            CloudUpdater.instance.SetShaderFrame(currentTimeIndex - dragonTail, currentTimeIndex);

        }

        public void Play()
        {
            playAnimation = !playAnimation;
        }


        public void ChangePointOverlayStatus()
        {
            PointsRenderer.enabled = !PointsRenderer.enabled;
        }

        private void Update()
        {
            if (playAnimation)
            {
                currentTimeIndex += incrementSpeed*Time.deltaTime;
                SetShaderTimeLimit();
                if (currentTimeIndex > TimeList.Count - 1)
                {
                    currentTimeIndex = startTime;
                }
                ActualizeText();
                ActualizeSlider();
            }
        }
    }
}