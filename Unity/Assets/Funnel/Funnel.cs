﻿//
// Funnel: Minimal Syphon Server Plugin for Unity
//
// Copyright (C) 2013 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Funnel : MonoBehaviour
{
    #region Class constants and variables

    // Render Event ID (0xfa9100 - 0xfa92ff)
    const int PublishEventID = 0xfa9100;
    const int ReleaseEventID = 0xfa9200;

    // Slot index counter.
    static int slotCount = 0;

    #endregion

    #region Public properties

    // Screen settings.
    public int screenWidth = 1280;
    public int screenHeight = 720;

    // Editor settings.
    public bool drawGameView;
    public bool previewOnInspector;

    // Render texture which is to be sent.
    [System.NonSerialized]
    public RenderTexture
        renderTexture;

    #endregion

    #region Private variables

    // Slot index for this server.
    int slotIndex;

    #endregion

    #region Native plugin interface

    [DllImport("Funnel")]
    static extern void FunnelSetFrameTexture (int slotIndex, string frameName, int textureID, int width, int height);

    #endregion

    #region MonoBehaviour functions

    void Start ()
    {
        // Grab a slot.
        slotIndex = slotCount++;

        // Create a render texture.
        renderTexture = new RenderTexture (screenWidth, screenHeight, 24);

        // Assign the render texture to the cameras that has no target.
        foreach (var cam in Camera.allCameras)
        {
            if (cam.targetTexture == null)
            {
                cam.targetTexture = renderTexture;
                cam.ResetAspect ();
            }
        }
    }

    void OnDisable ()
    {
        // Release the slot.
        GL.IssuePluginEvent (ReleaseEventID + slotIndex);
    }

    void Update ()
    {
        // Set the previous frame to the slot.
        FunnelSetFrameTexture (slotIndex, gameObject.name, renderTexture.GetNativeTextureID (), screenWidth, screenHeight);

        // Call GL operations on the GL thread.
        GL.IssuePluginEvent (PublishEventID + slotIndex);
    }

    void OnGUI ()
    {
        // Draw the render texture on the game view.
        if (drawGameView)
            GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), renderTexture, ScaleMode.ScaleToFit, false);
    }

    #endregion
}
