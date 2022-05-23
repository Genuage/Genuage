/**
Written by Gist Github user DashW : https://gist.github.com/DashW/74d726293c0d3aeb53f4
**/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using DesktopInterface;

namespace IO
{


	class BitmapEncoder
	{
		public static void WriteBitmap(Stream stream, int width, int height, byte[] imageData)
		{
			using (BinaryWriter bw = new BinaryWriter(stream))
			{

				// define the bitmap file header
				bw.Write((UInt16)0x4D42);                               // bfType;
				bw.Write((UInt32)(14 + 40 + (width * height * 4)));     // bfSize;
				bw.Write((UInt16)0);                                    // bfReserved1;
				bw.Write((UInt16)0);                                    // bfReserved2;
				bw.Write((UInt32)14 + 40);                              // bfOffBits;

				// define the bitmap information header
				bw.Write((UInt32)40);                               // biSize;
				bw.Write((Int32)width);                                 // biWidth;
				bw.Write((Int32)height);                                // biHeight;
				bw.Write((UInt16)1);                                    // biPlanes;
				bw.Write((UInt16)32);                                   // biBitCount;
				bw.Write((UInt32)0);                                    // biCompression;
				bw.Write((UInt32)(width * height * 4));                 // biSizeImage;
				bw.Write((Int32)0);                                     // biXPelsPerMeter;
				bw.Write((Int32)0);                                     // biYPelsPerMeter;
				bw.Write((UInt32)0);                                    // biClrUsed;
				bw.Write((UInt32)0);                                    // biClrImportant;

				// switch the image data from RGB to BGR
				for (int imageIdx = 0; imageIdx < imageData.Length; imageIdx += 3)
				{
					bw.Write(imageData[imageIdx + 2]);
					bw.Write(imageData[imageIdx + 1]);
					bw.Write(imageData[imageIdx + 0]);
					bw.Write((byte)255);
				}

			}
		}

	}

	/// <summary>
	/// Captures frames from a Unity camera in real time
	/// and writes them to disk using a background thread.
	/// </summary>
	/// 
	/// <description>
	/// Maximises speed and quality by reading-back raw
	/// texture data with no conversion and writing 
	/// frames in uncompressed BMP format.
	/// Created by Richard Copperwaite.
	/// </description>
	/// 
	//[RequireComponent(typeof(Camera))] 
	public class ScreenRecorder : MonoBehaviour
	{
		// Public Properties
		public int maxFrames = 12000; // maximum number of frames you want to record in one video
		public int frameRate = 30; // number of frames to capture per second

		public string outputname;

		public Text updateText;
		public MeshRenderer box;

		// The Encoder Thread
		private Thread encoderThread;

		// Texture Readback Objects
		private RenderTexture tempRenderTexture;
		private Texture2D tempTexture2D;

		// Timing Data
		private float captureFrameTime;
		private float lastFrameTime;
		private int frameNumber;
		private int savingFrameNumber;

		// Encoder Thread Shared Resources
		private Queue<byte[]> frameQueue;
		public string persistentDataPath;
		public string ffmpath;
		private int screenWidth;
		private int screenHeight;
		private bool recordingthreadIsProcessing;
		private bool framesavingprocessfinished;
		public bool isVideoReady = false;
		public string DisplayText;

		private bool ScriptRunning = false;
		void OnEnable()
		{
			//updateText = UIManager.instance.ChangeStatusText();
			//box = GameObject.Find("Box").GetComponent<MeshRenderer>();


			// Set target frame rate (optional)
			Application.targetFrameRate = frameRate;

			// Prepare the data directory
			persistentDataPath = Application.dataPath + "/Records";
			ffmpath = Application.dataPath + @"/FFMPEG/bin\ffmpeg.exe";
			UnityEngine.Debug.Log(ffmpath);

			UnityEngine.Debug.Log("Capturing to: " + persistentDataPath + "/");

			if (!System.IO.Directory.Exists(persistentDataPath))
			{
				System.IO.Directory.CreateDirectory(persistentDataPath);
			}

			// Prepare textures and initial values
			screenWidth = GetComponent<Camera>().pixelWidth;
			screenHeight = GetComponent<Camera>().pixelHeight;
			//screenWidth = Screen.width;
			//screenHeight = Screen.height;

			tempRenderTexture = new RenderTexture(screenWidth, screenHeight, 0);
			tempTexture2D = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
			frameQueue = new Queue<byte[]>();

			frameNumber = 0;
			savingFrameNumber = 0;

			captureFrameTime = 1.0f / (float)frameRate;
			lastFrameTime = Time.time;

			// Kill the encoder thread if running from a previous execution
			if (encoderThread != null && (recordingthreadIsProcessing || encoderThread.IsAlive))
			{
				recordingthreadIsProcessing = false;
				encoderThread.Join();
			}
			isVideoReady = false;
			framesavingprocessfinished = false;
			recordingthreadIsProcessing = false;
			// Start a new encoder thread
			//threadIsProcessing = true;
			//encoderThread = new Thread(EncodeAndSave);
			//encoderThread.Start();
			//updateText.text = "Movie Ready";
		}

		public void ActivateRecording()
		{
			//ScriptRunning = true;
			EncodeAndSave();
			//threadIsProcessing = true;
			//encoderThread = new Thread(EncodeAndSave);
			//encoderThread.Start();

		}

		void OnDisable()
		{
			ScriptRunning = false;
			DisableRecording();
		}


		public void DisableRecording()
		{
			// Reset target frame rate
			Application.targetFrameRate = -1;

			// Inform thread to terminate when finished processing frames
			framesavingprocessfinished = true;

		}


		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (ScriptRunning == true)
			{
				if (frameNumber <= maxFrames)
				{
					//Check if render target size has changed, if so, terminate
					if (source.width != screenWidth || source.height != screenHeight)
					{
						recordingthreadIsProcessing = false;
						this.enabled = false;
						throw new UnityException("ScreenRecorder render target size has changed!");
					}

					// Calculate number of video frames to produce from this game frame
					// Generate 'padding' frames if desired framerate is higher than actual framerate
					float thisFrameTime = Time.time;
					int framesToCapture = ((int)(thisFrameTime / captureFrameTime)) - ((int)(lastFrameTime / captureFrameTime));

					// Capture the frame
					if (framesToCapture > 0)
					{
						Graphics.Blit(source, tempRenderTexture);
						//Graphics.CopyTexture(tempRenderTexture, tempTexture2D);
						RenderTexture.active = tempRenderTexture;
						tempTexture2D.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
						RenderTexture.active = null;
					}

					// Add the required number of copies to the queue
					for (int i = 0; i < framesToCapture && frameNumber <= maxFrames; ++i)
					{
						frameQueue.Enqueue(tempTexture2D.GetRawTextureData());

						frameNumber++;

						if (frameNumber % frameRate == 0)
						{
							//UnityEngine.Debug.Log("Frame " + frameNumber);
						}
					}

					lastFrameTime = thisFrameTime;

				}
				else //keep making screenshots until it reaches the max frame amount
				{

					//box.enabled = true;
					// Inform thread to terminate when finished processing frames
					framesavingprocessfinished = true;
					// Disable script
					//this.enabled = false;
				}
			}
			// Passthrough
			Graphics.Blit(source, destination);
		}
		private void Update()
		{
			if (recordingthreadIsProcessing)
			{

				UIManager.instance.ChangeStatusText(DisplayText);

			}
			else if (isVideoReady == true)
            {
				encoderThread.Join();
				recordingthreadIsProcessing = false;
				SendRecordingEndEvent();
			}
		}
		public void EncodeAndSave()
		{
			print("SCREENRECORDER IO THREAD STARTED");
			ScriptRunning = true;
			recordingthreadIsProcessing = true;
			encoderThread = new Thread(ThreadProcess);
			encoderThread.Start();
			
		}

		private void ThreadProcess()
        {
			while (recordingthreadIsProcessing)
			{
				if (frameQueue.Count > 0)
				{
					// Generate file path
					string path = persistentDataPath + "/frame" + savingFrameNumber + ".bmp";

					// Dequeue the frame, encode it as a bitmap, and write it to the file
					using (FileStream fileStream = new FileStream(path, FileMode.Create))
					{
						byte[] BytesArray = frameQueue.Dequeue();
						BitmapEncoder.WriteBitmap(fileStream, screenWidth, screenHeight, BytesArray);
						BytesArray = null;
						fileStream.Close();
					}

					// Done

					savingFrameNumber++;
					DisplayText = "Saved " + savingFrameNumber + " frames. " + frameQueue.Count + " frames remaining.";
					//UnityEngine.Debug.Log("Saved " + savingFrameNumber + " frames. " + frameQueue.Count + " frames remaining.");
				}
				else
				{
					if (framesavingprocessfinished)
					{
						break;
					}

					Thread.Sleep(1);
				}
			}


			framesavingprocessfinished = false;
			recordingthreadIsProcessing = false;
			isVideoReady = false;

			UnityEngine.Debug.Log("SCREENRECORDER IO THREAD FINISHED");
			DateTime now = DateTime.Now;
			outputname = now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + now.Hour.ToString() + now.Minute.ToString();
			string commandline = "-s 1920x1080 -i frame%d.bmp -crf 25 " + outputname + ".mp4";

			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = ffmpath,
					Arguments = commandline,
					UseShellExecute = true,
					RedirectStandardOutput = false,
					CreateNoWindow = false,
					WorkingDirectory = persistentDataPath
				}
			};

			proc.Start();
			proc.WaitForExit();


			string[] tmpframes = Directory.GetFiles(persistentDataPath, "*.bmp");
			foreach (string frame in tmpframes)
			{
				File.Delete(frame);
			}
			isVideoReady = true;
		}

		public delegate void OnRecordingEndEvent();
		public event OnRecordingEndEvent OnRecordingEnd;

		private void SendRecordingEndEvent()
		{
			if (OnRecordingEnd != null)
			{
				OnRecordingEnd();
			}
		}

	}
}