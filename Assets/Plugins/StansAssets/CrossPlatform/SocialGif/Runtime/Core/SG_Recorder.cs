using UnityEngine;
using System;
using System.Collections.Generic;

namespace SA.GIF {
	public sealed class SG_Recorder {

		public enum RecorderState
		{
			Recording,
			Paused,
			Stopped,
			PreProcessing
		}

		/// <summary>
		/// Called when the pre-processing step has finished.
		/// </summary>
		public event Action OnPreProcessingDone = delegate{};
		
		/// <summary>
		/// Called once a gif recorder state is changed.
		/// </summary>
		public event Action<RecorderState> OnRecorderStateChanged = delegate {};

		/// <summary>
		/// Called by each worker thread every time a frame is processed during the save process.
		/// The first parameter holds the worker ID and the second one a value in range [0;1] for
		/// the actual progress. This callback is probably not thread-safe, use at your own risks.
		/// </summary>
		public event Action<int, float> OnFileSaveProgress = delegate{};

		/// <summary>
		/// Called once a gif file has been saved. The first parameter will hold the worker ID and
		/// the second one the absolute file path.
		/// </summary>
		public event Action<int, string> OnFileSaved = delegate{};

		private RecorderComponent recorder = null;
		private string _SavedFilePath = string.Empty;

		public SG_Recorder(Camera camera = null) {
			if (camera == null) {
				camera = Camera.main;

				if (camera == null) {
					Debug.LogWarning ("You are trying to create recorder with NO Camera!");
					return;
				}
			}

			recorder = camera.gameObject.GetComponent<RecorderComponent> ();
			if (recorder == null) {
				recorder = camera.gameObject.AddComponent<RecorderComponent> ();
			}

			recorder.OnPreProcessingDone += Recorder_OnPreProcessingDone;
			recorder.OnFileSaveProgress += Recorder_OnFileSaveProgress;
			recorder.OnFileSaved += Recorder_OnFileSaved;
			recorder.OnRecorderStateChanged += (state) => { OnRecorderStateChanged.Invoke(state); };
		}

		/// <summary>
		/// Initializes the component. Use this if you need to change the recorder settings in a script.
		/// This will flush the previously saved frames as settings can't be changed while recording.
		/// </summary>
		/// <param name="autoAspect">Automatically compute height from the current aspect ratio</param>
		/// <param name="width">Width in pixels</param>
		/// <param name="height">Height in pixels</param>
		/// <param name="fps">Recording FPS</param>
		/// <param name="bufferSize">Maximum amount of seconds to record to memory</param>
		/// <param name="repeat">-1: no repeat, 0: infinite, >0: repeat count</param>
		/// <param name="quality">Quality of color quantization (conversion of images to the maximum
		/// 256 colors allowed by the GIF specification). Lower values (minimum = 1) produce better
		/// colors, but slow processing significantly. Higher values will speed up the quantization
		/// pass at the cost of lower image quality (maximum = 100).</param>
		public void Setup(bool autoAspect, int width, int height, int fps, float bufferSize, int repeat, int quality) {
			if (recorder != null)
				recorder.Setup (autoAspect, width, height, fps, bufferSize, repeat, quality);
		}

		/// <summary>
		/// Pauses recording.
		/// </summary>
		public void Pause() {
			if (recorder != null)
				recorder.Pause ();
		}

		/// <summary>
		/// Starts or resumes recording. You can't resume while it's pre-processing data to be saved.
		/// </summary>
		public void Record() {
			if (recorder != null)
				recorder.Record ();
		}

		/// <summary>
		/// Stops the recording. You can't resume the record after it has been stopped.
		/// </summary>
		public void Stop() {
			if (recorder != null)
				recorder.Stop ();
		}

		/// <summary>
		/// Clears all saved frames from memory and starts fresh.
		/// </summary>
		public void FlushMemory() {
			if (recorder != null)
				recorder.FlushMemory ();
		}

		/// <summary>
		/// Saves the stored frames to a gif file. The filename will automatically be generated.
		/// Recording will be paused and won't resume automatically. You can use the 
		/// <code>OnPreProcessingDone</code> callback to be notified when the pre-processing
		/// step has finished.
		/// </summary>
		public void Save() {
			if (recorder != null)
				recorder.Save ();
		}

		/// <summary>
		/// Saves the stored frames to a gif file. If the filename is null or empty, an unique one
		/// will be generated. You don't need to add the .gif extension to the name. Recording will
		/// be paused and won't resume automatically. You can use the <code>OnPreProcessingDone</code>
		/// callback to be notified when the pre-processing step has finished.
		/// </summary>
		/// <param name="filename">File name without extension</param>
		public void Save(string filename) {
			if (recorder != null)
				recorder.Save (filename);
		}

		/// <summary>
		/// Current state of the recorder.
		/// </summary>
		public RecorderState State {
			get {
				if (recorder != null)
					return recorder.State;
				else
					return RecorderState.Paused;
			}
		}

		public RenderTexture[] Frames {
			get { 
				return recorder.Frames.ToArray();
			}
		}

		public int FPS {
			get {
				return recorder.FPS;
			}
		}

		/// <summary>
		/// Saved File Path. Empty if recorded gif was mot yet saved
		/// </summary>
		public string SavedFilePath {
			get {
				return _SavedFilePath;
			}
		}

		private void Recorder_OnFileSaved (int id, string path) {
			_SavedFilePath = path;
			OnFileSaved (id, path);
		}

		private void Recorder_OnFileSaveProgress (int id, float progress) {
			OnFileSaveProgress (id, progress);
		}

		private void Recorder_OnPreProcessingDone () {
			OnPreProcessingDone ();
		}
	}	
}
