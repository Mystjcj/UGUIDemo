using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RotationDiagram2D : MonoBehaviour
{
    public Vector2 ItemSize;
    public Sprite[] ItemSprites;
    public float Offset;
    public float ScaleTimesMin;
    public float ScaleTimesMax;
    public List<RotationDiagramItem> _item;
    [SerializeField]
    public List<ItemPosData> _posData;
    List<ItemPosData> tempPosData;//用于对Item重新排序
    private void Start()
    {
        _item = new List<RotationDiagramItem>();
        _posData = new List<ItemPosData>();
        CreatItem();
        CalulateData();
        SetItemData();
    }
    /// <summary>
    /// 创建预制体模板
    /// </summary>
    /// <returns></returns>
    private GameObject CreateTemplate()
    {
        GameObject item = new GameObject("Template");
        item.AddComponent<RectTransform>().sizeDelta = ItemSize;
        item.AddComponent<Image>();
        item.AddComponent<RotationDiagramItem>();
        return item;
    }
    private void CreatItem()
    {
        GameObject template = CreateTemplate();
        RotationDiagramItem itemTemp = null;
        foreach (Sprite sprite in ItemSprites)
        {
            itemTemp = Instantiate(template).GetComponent<RotationDiagramItem>();
            itemTemp.SetParent(transform);
            itemTemp.SetSprite(sprite);
            itemTemp.AddMoveListener(Move);
            _item.Add(itemTemp);
        }
        Destroy(template);
    }
    private void Move(float offsetX)
    {
        int symbol = offsetX > 0 ? 1 : -1;
        Move(symbol);
    }
    private void Move(int symbol)
    {
        foreach (RotationDiagramItem item in _item)
        {
            item.ChangeId(symbol, _item.Count);
        }
        for (int i = 0; i < _posData.Count; i++)
        {
            _item[i].SetPosData(_posData[_item[i].PosId]);
        }
    }
    private void CalulateData()
    {
        float length = (ItemSize.x + Offset) * _item.Count;
        float radioOffset = 1 / (float)_item.Count;
        float radio = 0;
        for (int i = 0; i < _item.Count; i++)
        {
            _item[i].PosId = i;
            ItemPosData data = new ItemPosData
            {
                X = GetX(radio, length),
                ScaleTimes = GetScaleTimes(radio, ScaleTimesMax, ScaleTimesMin)
            };
            radio += radioOffset;
            _posData.Add(data);
        }
        //根据ScaleTimes对_posData排序
        tempPosData = _posData.OrderBy(p => p.ScaleTimes).ToList();
        for (int i = 0; i < tempPosData.Count; i++)
        {
            tempPosData[i].Order = i;
        }
    }
    private void SetItemData()
    {
        for (int i = 0; i < _posData.Count; i++)
        {
            _item[i].SetPosData(_posData[i]);
        }
    }
    /// <summary>
    /// 得到X的值
    /// </summary>
    /// <param name="radio">环形Y轴负方向为起点，逆时针旋转，占周长的比例</param>
    /// <param name="length">环形的周长</param>
    /// <returns></returns>
    private float GetX(float radio, float length)
    {
        if (radio > 1 || radio < 0)
        {
            Debug.LogError("radio必须在0-1之间");
            return 0;
        }
        if (radio >= 0 && radio < 0.25f)
        {
            return length * radio;
        }
        else if (radio >= 0.25f && radio < 0.75f)
        {
            return length * (0.5f - radio);
        }
        else
        {
            return length * (radio - 1);
        }
    }
    public float GetScaleTimes(float radio, float max, float min)
    {
        if (radio > 1 || radio < 0)
        {
            Debug.LogError("radio必须在0-1之间");
            return 0;
        }
        float scaleOffset = (max - min) / 0.5f;
        if (radio < 0.5f)
        {
            return max - scaleOffset * radio;
        }
        else
        {
            return max - scaleOffset * (1 - radio);
        }
    }
}
