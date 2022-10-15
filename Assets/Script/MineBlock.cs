using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MineBlock : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public uint BlockRow { get { return m_BlockRow; } }
    public uint BlockColumn { get { return m_BlockColumn; } }
    public bool isBomb = false;
    public bool isUnPacked = false;
    public bool isFlaged = false;
    private Text _btnText;
    /// <summary>
    /// ��
    /// </summary>
    uint m_BlockRow = 0;
    /// <summary>
    /// ��
    /// </summary>
    uint m_BlockColumn = 0;
    private RectTransform m_RectTransform;
    public void SetIndex(uint bRow, uint bColumn)
    {
        m_BlockColumn = bColumn;
        m_BlockRow = bRow;
        if (m_RectTransform == null)
            m_RectTransform = GetComponent<RectTransform>();
        if (m_RectTransform != null)
        {
            //Debug.Log($"�У�{m_BlockRow} | �У�{m_BlockColumn}");
            m_RectTransform.anchoredPosition3D = new Vector3(bColumn, -bRow, 0) * 32;
        }
    }
    public void SetBtnClick()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnOnClick);
    }
    public void BtnOnClick()
    {
        Debug.Log($"�У�{m_BlockRow} | �У�{m_BlockColumn}");
        if (isBomb)
        {
            isUnPacked = true;
            // you lose...
            Debug.Log("you lose...");
            SetText("��");
        }
        else
        {
            StartCoroutine(OpenBlockArea()) ;
        }
    }
    private void SetUnPackedSprite()
    {
        Image img = GetComponent<Image>();
        img.sprite = BlockManager.Ins.BtnStatus[1];
        Button btn = GetComponent<Button>();
        var cB = btn.colors;
        cB.normalColor = new Color(0.9f, 0.9f, 0.9f);
        btn.colors = cB;
    }

    public void SetSprite(Sprite spr)
    {
        Image img = GetComponent<Image>();
        img.sprite = spr;
    }

    public IEnumerator OpenBlockArea()
    {
        isUnPacked = true;
        SetUnPackedSprite();
        uint mineCount = GetMineCountArround();
        SetText(mineCount.ToString());
        Debug.Log($"����������{mineCount}");
        if (mineCount == 0)
        {
            var bArround = GetBlockArround();
            foreach (var b in bArround)
            {
                yield return 0;
                if (b != null && !b.isBomb && !b.isUnPacked)
                    StartCoroutine(b.OpenBlockArea()) ;
            }
        }
        yield break;
    }
    private uint GetMineCountArround()
    {
        var bArround = GetBlockArround();
        Debug.Log(bArround.Count);
        uint count = 0;
        foreach (var b in bArround)
        {
            if (b == null) continue;
            if (b.isBomb) count++;
        }
        return count;
    }
    private List<MineBlock> GetBlockArround()
    {
        List<MineBlock> blocks = new List<MineBlock>();
        if (BlockRow > 0) // ��1�����
        {
            if (BlockColumn > 0)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow - 1, BlockColumn - 1));   // ��1�е�1��
            blocks.Add(BlockManager.Ins.GetBlock(BlockRow - 1, BlockColumn));           // ��1�е�2��
            if (BlockColumn < BlockManager.Ins.ColCount - 1)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow - 1, BlockColumn + 1));   // ��1�е�3��
        }
        // �������
        {
            if (BlockColumn > 0)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow, BlockColumn - 1));       // ��1��
            if (BlockColumn < BlockManager.Ins.ColCount - 1)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow, BlockColumn + 1));       // ��3��
        }
        if (BlockRow < BlockManager.Ins.RowCount - 1) // ��1�����
        {
            if (BlockColumn > 0)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow + 1, BlockColumn - 1));   // ��1�е�1��
            blocks.Add(BlockManager.Ins.GetBlock(BlockRow + 1, BlockColumn));           // ��1�е�2��
            if (BlockColumn < BlockManager.Ins.ColCount - 1)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow + 1, BlockColumn + 1));   // ��1�е�3��
        }
        return blocks;
    }
    public void SetText(string str)
    {
        if (_btnText == null)
            _btnText = GetComponentInChildren<Text>();
        if (str != "0")
            _btnText.text = str;
    }
    public string GetText(string str)
    {
        if (_btnText == null)
            _btnText = GetComponentInChildren<Text>();
        return _btnText.text;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BlockManager.Ins.CurrBlock = null;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        BlockManager.Ins.CurrBlock = this;
    }
    // Start is called before the first frame update
    
}
