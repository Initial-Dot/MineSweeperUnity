using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MineBlock : MonoBehaviour
{
    public uint BlockRow { get { return m_BlockRow; } }
    public uint BlockColumn { get { return m_BlockColumn; } }
    public bool isBomb = false;
    public bool isUnPacked = false;

    private Text _btnText;
    /// <summary>
    /// 行
    /// </summary>
    uint m_BlockRow = 0;
    /// <summary>
    /// 列
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
            //Debug.Log($"行：{m_BlockRow} | 列：{m_BlockColumn}");
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
        Debug.Log($"行：{m_BlockRow} | 列：{m_BlockColumn}");
        if (isBomb)
        {
            isUnPacked = true;
            // you lose...
            Debug.Log("you lose...");
            SetText("雷");
        }
        else
        {
            OpenBlockArea();
        }
    }
    private void SetUnPackedSprite()
    {
        Image img = GetComponent<Image>();
        img.sprite = BlockManager.Ins.BtnStatus[1];
    }
    public void OpenBlockArea()
    {
        isUnPacked = true;
        SetUnPackedSprite();
        uint mineCount = GetMineCountArround();
        SetText(mineCount.ToString());
        Debug.Log($"地雷数量：{mineCount}");
        if (mineCount == 0)
        {
            var bArround = GetBlockArround();
            foreach (var b in bArround)
            {
                if (b != null && !b.isBomb && !b.isUnPacked)
                    b.OpenBlockArea();
            }
        }
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
        if (BlockRow > 0) // 上1行情况
        {
            if (BlockColumn > 0)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow - 1, BlockColumn - 1));   // 上1行第1个
            blocks.Add(BlockManager.Ins.GetBlock(BlockRow - 1, BlockColumn));           // 上1行第2个
            if (BlockColumn < BlockManager.Ins.ColCount - 1)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow - 1, BlockColumn + 1));   // 上1行第3个
        }
        // 本行情况
        {
            if (BlockColumn > 0)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow, BlockColumn - 1));       // 第1个
            if (BlockColumn < BlockManager.Ins.ColCount - 1)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow, BlockColumn + 1));       // 第3个
        }
        if (BlockRow < BlockManager.Ins.RowCount - 1) // 下1行情况
        {
            if (BlockColumn > 0)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow + 1, BlockColumn - 1));   // 下1行第1个
            blocks.Add(BlockManager.Ins.GetBlock(BlockRow + 1, BlockColumn));           // 下1行第2个
            if (BlockColumn < BlockManager.Ins.ColCount - 1)
                blocks.Add(BlockManager.Ins.GetBlock(BlockRow + 1, BlockColumn + 1));   // 下1行第3个
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
