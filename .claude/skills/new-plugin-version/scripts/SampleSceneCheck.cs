// Temporary verification script for the new-plugin-version skill. Copied into
// source/plugin/Assets/Editor/ for one batchmode run, then deleted - never commit it there.
//
// Opens MainScene and checks that every object reference AdsController serializes is assigned
// (an unassigned button throws in Start() and breaks every ad format, not just the new one) and
// that no two direct children of the Canvas overlap. With SAMPLE_SCENE_SCREENSHOT set and a
// graphics device (run without -nographics), it also renders the Main Camera to that PNG path.
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SampleSceneCheck
{
    public static void Run()
    {
        var errors = 0;
        Action<bool, string> check = (ok, msg) =>
        {
            Debug.Log((ok ? "[CHECK OK] " : "[CHECK FAIL] ") + msg);
            if (!ok) errors++;
        };

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity", OpenSceneMode.Single);
        var controller = UnityEngine.Object.FindFirstObjectByType<AdsController>();
        check(controller != null, "AdsController found in MainScene");
        if (controller == null)
        {
            EditorApplication.Exit(1);
            return;
        }

        var so = new SerializedObject(controller);
        var prop = so.GetIterator();
        for (var enter = true; prop.NextVisible(enter); enter = false)
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.name != "m_Script")
            {
                var target = prop.objectReferenceValue;
                check(target != null, prop.name + " -> " + (target != null ? target.name : "UNASSIGNED"));
            }
            else if (prop.propertyType == SerializedPropertyType.String)
            {
                check(!string.IsNullOrEmpty(prop.stringValue), prop.name + " = " + prop.stringValue);
            }
        }

        var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        var kids = new List<RectTransform>();
        foreach (Transform child in canvas.transform)
        {
            if (child is RectTransform rt && child.gameObject.activeSelf) kids.Add(rt);
        }
        for (var i = 0; i < kids.Count; i++)
        {
            for (var j = i + 1; j < kids.Count; j++)
            {
                if (WorldRect(kids[i]).Overlaps(WorldRect(kids[j])))
                {
                    check(false, "overlap: '" + kids[i].name + "' and '" + kids[j].name + "'");
                }
            }
        }
        check(true, "overlap scan over " + kids.Count + " Canvas children");

        var shot = Environment.GetEnvironmentVariable("SAMPLE_SCENE_SCREENSHOT");
        if (!string.IsNullOrEmpty(shot))
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                Debug.Log("[CHECK] no graphics device (-nographics?) - screenshot skipped");
            }
            else
            {
                // Phone-shaped (1080x2340 at half size), the same aspect as a typical device.
                var cam = Camera.main;
                var rt = new RenderTexture(540, 1170, 24);
                cam.aspect = 540f / 1170f;
                cam.targetTexture = rt;
                Canvas.ForceUpdateCanvases();
                cam.Render();
                RenderTexture.active = rt;
                var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex.Apply();
                File.WriteAllBytes(shot, tex.EncodeToPNG());
                cam.targetTexture = null;
                RenderTexture.active = null;
                Debug.Log("[CHECK] screenshot: " + shot);
            }
        }

        Debug.Log("[CHECK] errors: " + errors);
        EditorApplication.Exit(errors == 0 ? 0 : 1);
    }

    static Rect WorldRect(RectTransform rt)
    {
        var c = new Vector3[4];
        rt.GetWorldCorners(c);
        return new Rect(c[0].x, c[0].y, c[2].x - c[0].x, c[2].y - c[0].y);
    }
}
