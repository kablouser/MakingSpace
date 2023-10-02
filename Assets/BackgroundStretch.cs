#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class BackgroundStretch : MonoBehaviour
{
    public float imageSize = 2048f / 64f;
    public Camera cameraMain;

    public float lastSize = 0f;

    public void OnValidate()
    {
        if (cameraMain == null)
        {
            Undo.RecordObject(this, "BackgroundStretch.OnValidate()");
            cameraMain = Camera.main;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        float height = cameraMain.orthographicSize * 2.0f;
        float aspect = cameraMain.aspect;
        float width = height * aspect;

        float maxSize = Mathf.Max(width, height);

        if (maxSize != lastSize)
        {
            lastSize = maxSize;
            transform.localScale = Vector3.one * lastSize / imageSize;
        }        
    }
}
#endif