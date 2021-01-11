using System;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.UI;

public class CircleImage : Image
{
    /// <summary>
    /// 圆形由多少块三角形拼成
    /// </summary>
    [SerializeField]
    private int segements = 100;
    [SerializeField]
    private float showPercent = 1;
    private readonly Color32 GRAY_COLOR = new Color32(60, 60, 60, 255);
    private List<Vector3> _vertexList;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        _vertexList = new List<Vector3>();
        AddVertex(vh, segements);
        AddTriangle(vh, segements);
    }
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out Vector2 localPoint);
        return IsValid(_vertexList, localPoint);
    }
    /// <summary>
    /// 判断一点是否在包围盒里
    /// </summary>
    /// <param name="vertexList"></param>
    /// <param name="localPoint"></param>
    /// <returns></returns>
    private bool IsValid(List<Vector3> vertexList, Vector2 localPoint)
    {
        return GetCorssPointNum(vertexList, localPoint) % 2 == 1;
    }

    private int GetCorssPointNum(List<Vector3> vertexList, Vector2 localPoint)
    {
        int count = 0;
        int vertCount = vertexList.Count;
        for (int i = 0; i < vertCount; i++)
        {
            Vector3 vert1 = vertexList[i];
            Vector3 vert2 = vertexList[(i + 1) % vertCount];
            if (IsYInRang(localPoint, vert1, vert2))
            {
                if (localPoint.x < GetX(vert1, vert2, localPoint.y))
                {
                    count++;
                }
            }
        }
        Debug.Log("count:" + count);
        return count;
    }
    /// <summary>
    /// VertexHelper添加顶点信息
    /// </summary>
    /// <param name="vh"></param>
    /// <param name="segements">三角面的数量</param>
    private void AddVertex(VertexHelper vh, int segements)
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        int realSegments = (int)(segements * showPercent);

        Vector2[] uvs = overrideSprite != null ? SpriteUtility.GetSpriteUVs(overrideSprite, false) : new Vector2[2];

        float minX = 10000, minY = 10000, maxX = -10000, maxY = -10000;
        for (int i = 0; i < uvs.Length; i++)
        {
            Vector2 uv = uvs[i];
            minX = Mathf.Min(minX, uv.x);
            maxX = Mathf.Max(maxX, uv.x);
            minY = Mathf.Min(minY, uv.y);
            maxY = Mathf.Max(maxY, uv.y);
        }
        //利用当前UV坐标计算出UVCenter的坐标
        float uvCenterX = (minX + maxX) * 0.5f;
        float uvCenterY = (minY + maxY) * 0.5f;
        Vector2 uvCenter = new Vector2(uvCenterX, uvCenterY);
        //利用当前Image的宽高和UV宽高计算出 X、Y轴上的UV转换系数（uvScaleX，uvScaleY）（每1单位的position所占的uv值）
        float uvScaleX = (maxX - minX) / width;
        float uvScaleY = (maxY - minY) / height;
        Vector2 uvScale = new Vector2(uvScaleX, uvCenterY);
        //求每一段区域所占圆的弧度（radian）与绘制的圆的半径（radius）
        float radian = 2 * Mathf.PI / segements;
        float radius = width * 0.5f;
        Debug.Log("pivot: " + rectTransform.pivot);
        Vector2 centerVertexPos = new Vector2((0.5f - rectTransform.pivot.x) * width, (0.5f - rectTransform.pivot.y) * height);
        Color32 colorTemp = GetOriginColor();

        UIVertex cneterVertex = GetUIVertex(centerVertexPos, uvCenter, colorTemp);
        vh.AddVert(cneterVertex);

        int vertexCount = realSegments + 1;
        float curRadian = 0;
        Vector2 posTemp = Vector2.zero;
        for (int i = 0; i < segements + 1; i++)
        {
            float x = Mathf.Cos(curRadian) * radius;
            float y = Mathf.Sin(curRadian) * radius;
            curRadian += radian;

            if (i < vertexCount)
            {
                colorTemp = color;
            }
            else
            {
                colorTemp = GRAY_COLOR;
            }
            posTemp = new Vector2(x, y);
            Vector2 vertexPos = posTemp + centerVertexPos;
            Vector2 uv = new Vector2(uvCenterX + x * uvScaleX, uvCenterY + y * uvScaleY);
            UIVertex vertexTemp = GetUIVertex(vertexPos, uv, colorTemp);
            vh.AddVert(vertexTemp);
            _vertexList.Add(vertexPos);
        }
    }
    /// <summary>
    /// vertexHelper添加3角面信息，在渲染流程中，以3个顶点顺时针添加的面渲染，3个顶点逆时针添加的面不渲染
    /// </summary>
    /// <param name="vh"></param>
    /// <param name="segements"></param>
    private void AddTriangle(VertexHelper vh, int segements)
    {
        for (int i = 0; i < segements; i++)
        {
            vh.AddTriangle(i + 1, 0, i + 2);
        }
    }
    /// <summary>
    /// 得到UIVertex
    /// </summary>
    /// <param name="vertexPos"></param>
    /// <param name="uv0"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private UIVertex GetUIVertex(Vector2 vertexPos, Vector2 uv0, Color32 col)
    {
        UIVertex vertex = new UIVertex();
        vertex.position = vertexPos;
        vertex.color = col;
        vertex.uv0 = uv0;
        return vertex;
    }

    private Color32 GetOriginColor()
    {
        Color32 colorTemp = (Color.white - GRAY_COLOR) * showPercent;
        return new Color32(
            (byte)(GRAY_COLOR.r + colorTemp.r),
            (byte)(GRAY_COLOR.g + colorTemp.g),
            (byte)(GRAY_COLOR.b + colorTemp.b),
            255
            );
    }
    /// <summary>
    /// 校验点是否在两点之间（Y）
    /// </summary>
    /// <param name="localPoint"></param>
    /// <param name="vert1"></param>
    /// <param name="vert2"></param>
    /// <returns></returns>
    private bool IsYInRang(Vector2 localPoint, Vector3 vert1, Vector3 vert2)
    {
        if (vert1.y > vert2.y)
        {
            return localPoint.y < vert1.y && localPoint.y > vert2.y;
        }
        else
        {
            return localPoint.y < vert2.y && localPoint.y > vert1.y;
        }
    }
    /// <summary>
    /// 得到直线方程上Y=y时，X的值
    /// </summary>
    /// <param name="vert1">直线方程的已知点</param>
    /// <param name="vert2">直线方程的已知点</param>
    /// <param name="y">点Y=y</param>
    /// <returns></returns>
    private float GetX(Vector3 vert1, Vector3 vert2, float y)
    {
        float k = (vert1.y - vert2.y) / (vert1.x - vert2.x);
        //x-vert1=(y-vert1.y)/k;
        return vert1.x + (y - vert1.y) / k;
    }
}
