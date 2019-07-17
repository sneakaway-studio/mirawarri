﻿//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    public class FingersImageAutomationScript : MonoBehaviour
    {
        public FingersScript FingersScript;
        public UnityEngine.UI.RawImage Image;
        public ImageGestureRecognizer ImageGesture = new ImageGestureRecognizer();
        public Material LineMaterial;
        public UnityEngine.UI.Text MatchLabel;
        public UnityEngine.UI.InputField ScriptText;

        protected ImageGestureImage LastImage { get; private set; }
        protected Dictionary<ImageGestureImage, string> RecognizableImages;

        private List<List<Vector2>> lineSet = new List<List<Vector2>>();
        private List<Vector2> currentPointList;

        public void DeleteLastScriptLine()
        {
            if (ScriptText != null)
            {
                string[] lines = ScriptText.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    ScriptText.text = string.Join(System.Environment.NewLine, lines, 0, lines.Length - 1).Trim(',');
                }
            }
        }

        protected virtual void Start()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Updated += Tap_Updated;
            FingersScript.AddGesture(tap);

            ImageGesture.Updated += ImageGestureUpdated;
            ImageGesture.MaximumPathCountExceeded += MaximumPathCountExceeded;
            if (RecognizableImages != null)
            {
                ImageGesture.GestureImages = new List<ImageGestureImage>(RecognizableImages.Keys);
            }
            FingersScript.AddGesture(ImageGesture);

            // imageGesture.Simulate(752, 382, 760, 365, 768, 348, 780, 335, 789, 329, 802, 327, 814, 336, 828, 354, 837, 371, 841, 381, 841, 386);
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
                else
                {
                    ImageGesture.Reset();
                    ResetLines();
                    UpdateImage();
                }
            }
        }

        private void UpdateImage()
        {
            Texture2D t = new Texture2D(ImageGestureRecognizer.ImageColumns, ImageGestureRecognizer.ImageRows, TextureFormat.ARGB32, false, false);
            t.filterMode = FilterMode.Point;
            t.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < ImageGestureRecognizer.ImageRows; y++)
            {
                for (int x = 0; x < ImageGestureRecognizer.ImageColumns; x++)
                {
                    // each bit in the row can be checked as well if Pixels is not available
                    // if ((imageGesture.Image.Rows[y] & (ulong)(1 << x)) == 0)
                    if (ImageGesture.Image.Pixels[x + (y * ImageGestureRecognizer.ImageRows)] == 0)
                    {
                        t.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        t.SetPixel(x, y, Color.white);
                    }
                }
            }
            t.Apply();
            Image.texture = t;
            LastImage = ImageGesture.Image.Clone();

            if (ImageGesture.MatchedGestureImage == null)
            {
                MatchLabel.text = "No match";
            }
            else
            {
                MatchLabel.text = "Match: " + RecognizableImages[ImageGesture.MatchedGestureImage];
            }

            MatchLabel.text += " (" + ImageGesture.MatchedGestureCalculationTimeMilliseconds + " ms)";
        }

        private void UpdateScriptText()
        {
            if (ScriptText != null)
            {
                if (ScriptText.text.Length != 0)
                {
                    ScriptText.text += "," + System.Environment.NewLine;
                }
                ScriptText.text += LastImage.GetCodeForRowsInitialize();
            }
        }

        private void AddTouches(ICollection<GestureTouch> touches)
        {
            GestureTouch? t = null;
            foreach (GestureTouch tt in touches)
            {
                t = tt;
                break;
            }
            if (t != null)
            {
                Vector3 v = new Vector3(t.Value.X, t.Value.Y, 0.0f);
                v = Camera.main.ScreenToWorldPoint(v);

                // Debug.LogFormat("STW: {0},{1} = {2},{3}", t.Value.X, t.Value.Y, v.x, v.y);

                currentPointList.Add(v);
            }
        }

        private void ImageGestureUpdated(GestureRecognizer imageGesture, ICollection<GestureTouch> touches)
        {
            if (imageGesture.State == GestureRecognizerState.Ended)
            {
                AddTouches(touches);
                UpdateImage();
                UpdateScriptText();
                // note - if you have received an image you care about, you should reset the image gesture, i.e. imageGesture.Reset()
                // the ImageGestureRecognizer doesn't automaticaly Reset like other gestures when it ends because some images need multiple paths
                // which requires lifting the mouse or finger and drawing again
            }
            else if (imageGesture.State == GestureRecognizerState.Began)
            {
                // began
                currentPointList = new List<Vector2>();
                lineSet.Add(currentPointList);
                AddTouches(touches);
            }
            else if (imageGesture.State == GestureRecognizerState.Executing)
            {
                // moving
                AddTouches(touches);
            }
        }

        private void ResetLines()
        {
            currentPointList = null;
            lineSet.Clear();
            UpdateImage();
        }

        private void MaximumPathCountExceeded(object sender, System.EventArgs e)
        {
            ResetLines();
        }

        private void Tap_Updated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                UnityEngine.Debug.Log("Tap Gesture Ended");
            }
        }

        private void OnRenderObject()
        {
            GL.PushMatrix();
            LineMaterial.SetPass(0);
            GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
            GL.Begin(GL.LINES);
            foreach (List<Vector2> lines in lineSet)
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    GL.Color(Color.white);
                    GL.Vertex(lines[i - 1]);
                    GL.Vertex(lines[i]);
                }
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}
