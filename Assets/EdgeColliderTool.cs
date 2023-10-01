using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EdgeColliderTool : MonoBehaviour
{
    [ContextMenu("/MirrorY")]
    public void MirrorY()
    {
        var edgeCol = GetComponent<EdgeCollider2D>();
        Undo.RecordObject(edgeCol, "MirrorY");

        List<Vector2> points = new();
        edgeCol.GetPoints(points);

        int pointsOriginalLen = points.Count;
        for (int i = 0; i < pointsOriginalLen; ++i)
        {
            var o = points[pointsOriginalLen - i - 1];
            o.y *= -1f;
            points.Add(o);
        }

        edgeCol.SetPoints(points);
    }
}
