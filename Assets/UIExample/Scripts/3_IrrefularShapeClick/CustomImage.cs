using UnityEngine;
using UnityEngine.UI;
public class CustomImage : Image
{
    private PolygonCollider2D _polygon;

    public PolygonCollider2D Polygon
    {
        get
        {
            if (_polygon == null)
            {
                _polygon = GetComponent<PolygonCollider2D>();
            }
            return _polygon;
        }
    }
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out Vector3 point);
        return Polygon.OverlapPoint(point);
    }
}