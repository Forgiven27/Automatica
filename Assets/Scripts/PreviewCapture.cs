#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class PreviewCapture : EditorWindow
{
    private GameObject targetObject;
    private int textureWidth = 256;
    private int textureHeight = 256;
    private string savePath = "Assets/Previews/preview.png";
    private Color backgroundColor = new Color(0, 0, 0, 0);
    private bool orthographic = true;
    private float orthographicSize = 2f;
    private float distance = 3f;
    private Vector3 rotation = Vector3.zero;
    private Vector3 positionOffset = Vector3.zero;
    private bool autoFit = true;

    [MenuItem("Tools/Preview Capture")]
    public static void ShowWindow()
    {
        GetWindow<PreviewCapture>("Preview Capture");
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target", targetObject, typeof(GameObject), true);

        textureWidth = EditorGUILayout.IntField("Width", textureWidth);
        textureHeight = EditorGUILayout.IntField("Height", textureHeight);

        // Path selection
        EditorGUILayout.BeginHorizontal();
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string dir = Path.GetDirectoryName(savePath);
            string file = Path.GetFileName(savePath);
            string path = EditorUtility.SaveFilePanel("Save Preview", dir, file, "png");
            if (!string.IsNullOrEmpty(path))
            {
                // Convert absolute path to relative asset path if inside project
                if (path.StartsWith(Application.dataPath))
                {
                    savePath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    savePath = path;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        backgroundColor = EditorGUILayout.ColorField("Background", backgroundColor);
        orthographic = EditorGUILayout.Toggle("Orthographic", orthographic);

        autoFit = EditorGUILayout.Toggle("Auto Fit", autoFit);
        if (!autoFit)
        {
            if (orthographic)
                orthographicSize = EditorGUILayout.FloatField("Ortho Size", orthographicSize);
            else
                distance = EditorGUILayout.FloatField("Distance", distance);
        }

        rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
        positionOffset = EditorGUILayout.Vector3Field("Offset", positionOffset);

        EditorGUILayout.Space();

        GUI.enabled = targetObject != null;
        if (GUILayout.Button("Capture Preview", GUILayout.Height(40)))
        {
            Capture();
        }
        GUI.enabled = true;
    }

    private void Capture()
    {
        if (targetObject == null)
        {
            Debug.LogError("No target object assigned.");
            return;
        }

        // Create directory if needed
        string directory = Path.GetDirectoryName(savePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // Root object for rotation/offset
        GameObject root = new GameObject("PreviewRoot");
        root.transform.position = positionOffset;
        root.transform.rotation = Quaternion.Euler(rotation);

        // Instantiate target as child of root
        GameObject instance;
        if (PrefabUtility.IsPartOfAnyPrefab(targetObject))
            instance = (GameObject)PrefabUtility.InstantiatePrefab(targetObject, root.transform);
        else
            instance = Instantiate(targetObject, root.transform);

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        // Calculate world bounds after transformations
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("Target has no Renderer components.");
            DestroyImmediate(root);
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        // Camera setup
        GameObject camGO = new GameObject("PreviewCamera");
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backgroundColor;
        cam.orthographic = orthographic;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 100f;
        cam.enabled = false;

        // Auto-fit camera
        if (autoFit)
        {
            float maxSize = bounds.extents.magnitude * 1.1f;
            if (orthographic)
            {
                orthographicSize = maxSize;
            }
            else
            {
                cam.fieldOfView = 60f;
                float angle = cam.fieldOfView * Mathf.Deg2Rad;
                distance = maxSize / Mathf.Tan(angle * 0.5f);
            }
        }

        // Position camera
        Vector3 camPos = bounds.center - camGO.transform.forward * distance;
        if (orthographic)
        {
            cam.orthographicSize = orthographicSize;
            camPos = bounds.center - camGO.transform.forward * distance;
        }
        camGO.transform.position = camPos;
        camGO.transform.LookAt(bounds.center);

        // Basic lighting
        GameObject lightGO = new GameObject("PreviewLight");
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(45f, 45f, 0);
        light.intensity = 1f;

        // Render texture
        RenderTexture rt = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        cam.Render();

        // Read pixels
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // Save as PNG
        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(savePath, pngData);
        AssetDatabase.Refresh();

        // Cleanup
        cam.targetTexture = null;
        DestroyImmediate(rt);
        DestroyImmediate(lightGO);
        DestroyImmediate(camGO);
        DestroyImmediate(root);

        Debug.Log($"Preview saved to: {savePath}");
    }
}
#endif