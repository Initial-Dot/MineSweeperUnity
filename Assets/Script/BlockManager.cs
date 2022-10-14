using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public Canvas _Canvas;
    public GameObject BlockPrefab;
    public static BlockManager Ins { get { return _Ins; } }
    private static BlockManager _Ins;
    public MineBlock[][] MineBlocks;
    //public List<MineBlock> MineBlockList;
    public uint RowCount = 15;
    public uint ColCount = 20;
    public uint MineCount = 50;
    // Start is called before the first frame update
    void Start()
    {
        if (_Ins == null)
            _Ins = this;
        GenerateBlockMap();
    }
    public MineBlock GetBlock(uint row, uint col)
    {
        return MineBlocks[row][col];
    }
    public void GenerateBlockMap()
    {
        MineBlocks = new MineBlock[RowCount][];
        //MineBlockList = new List<MineBlock>();
        for (uint i = 0; i < RowCount; i++)
        {
            MineBlocks[i] = new MineBlock[ColCount];
            for (uint j = 0; j < ColCount; j++)
            {
                GameObject tempGo = Instantiate(BlockPrefab);
                tempGo.transform.SetParent(_Canvas.transform);
                MineBlock mB = tempGo.AddComponent<MineBlock>();
                mB.SetIndex(i, j);
                mB.SetBtnClick();
                MineBlocks[i][j] = mB;
                //MineBlockList.Add(mB);
            }
        }
        GenMines();
    }
    private void GenMines()
    {
        uint c = 0;
        do
        {
            do
            {
                int rowIdx = Random.Range(0, (int)RowCount);
                int colIdx = Random.Range(0, (int)ColCount);
                if (!MineBlocks[rowIdx][colIdx].isBomb)
                {
                    MineBlocks[rowIdx][colIdx].isBomb = true;
                    c++;
                    break;
                }
            } while (true);
        } while (c < MineCount);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
