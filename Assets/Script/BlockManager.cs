using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour
{
    public Canvas _GamePage;
    public Canvas _StartUpPage;
    public Canvas _InfoPage;
    public GraphicRaycaster _GameCaster;
    public GraphicRaycaster _InfoCaster;
    public GraphicRaycaster _StartCaster;
    public Text _InfoPageText;
    public RectTransform _InfoPanelRect;
    public InputField _RestBombCount;

    public GameObject _MinePad;
    public GameObject BlockPrefab;
    public List<Sprite> BtnStatus;
    public static BlockManager Ins { get { return _Ins; } }
    private static BlockManager _Ins;
    public MineBlock[][] MineBlocks;
    public List<Color> TextColors;
    public MineBlock CurrBlock;
    //public List<MineBlock> MineBlockList;
    public uint RowCount = 15;
    public uint ColCount = 20;
    public uint MineCount = 50;
    private int _unFlagedCount; // 可能是错的，直接把雷数-插旗数
    private int _unFlagedBombCount; // 真实剩余雷数，只计算有效插旗
    private Vector2 renderSize;
    private bool isInGame = false;
    private float infoScale = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (_Ins == null)
            _Ins = this;
        //GenerateBlockMap();
        SetPage(0); // startupPage
    }
    public MineBlock GetBlock(uint row, uint col)
    {
        return MineBlocks[row][col];
    }
    public void StartMineSweep(int diff)
    {
        infoScale = 1f;
        if (diff == 0)
        {
            RowCount = 15;
            ColCount = 10;
            MineCount = 15;
            //Screen.SetResolution(190, 335, false);
            infoScale = 300f / 500f;
            SetWidth(300);
        }
        else if (diff == 1)
        {
            RowCount = 30;
            ColCount = 20;
            MineCount = 80;
            SetWidth(500);
        }
        else if (diff == 2)
        {
            RowCount = 30;
            ColCount = 20;
            MineCount = 120;
            SetWidth(500);
        }
        //OnCanvasSizeChanged();
        GenerateBlockMap();
    }
    public void GenerateBlockMap()
    {
        _unFlagedCount = (int)MineCount;
        _unFlagedBombCount = _unFlagedCount;
        _RestBombCount.text = _unFlagedCount.ToString();
        SetPage(1); // gamePage
        if (MineBlocks != null)
            foreach (MineBlock[] blockArr in MineBlocks)
                foreach (MineBlock block in blockArr)
                    Destroy(block.gameObject);
        MineBlocks = new MineBlock[RowCount][];
        //MineBlockList = new List<MineBlock>();
        for (uint i = 0; i < RowCount; i++)
        {
            MineBlocks[i] = new MineBlock[ColCount];
            for (uint j = 0; j < ColCount; j++)
            {
                GameObject tempGo = Instantiate(BlockPrefab);
                tempGo.transform.SetParent(_MinePad.transform);
                tempGo.transform.localScale = Vector3.one;
                MineBlock mB = tempGo.AddComponent<MineBlock>();
                mB.SetIndex(i, j);
                mB.SetBtnClick();
                MineBlocks[i][j] = mB;
                //MineBlockList.Add(mB);
            }
        }
        GenMines();
    }
    public void Retry()
    {
        SetPage(1);
        GenerateBlockMap();
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
        if (isInGame)
        {
            OnBtnRightClick();
            //OnCanvasSizeChanged();
        }
    }
    public void SetPage(int pIdx)
    {
        if (pIdx == 0)
        {
            isInGame = false;
            _StartUpPage.enabled = true;
            _GamePage.enabled = false;
            _InfoPage.enabled = false;
            _StartCaster.enabled = true;
            _GameCaster.enabled = false;
            _InfoCaster.enabled = false;
            Screen.SetResolution(400, 600, false);
        }
        else if (pIdx == 1)
        {
            isInGame = true;
            _StartUpPage.enabled = false;
            _GamePage.enabled = true;
            _InfoPage.enabled = false;
            _StartCaster.enabled = false;
            _GameCaster.enabled = true;
            _InfoCaster.enabled = false;
        }
        else if (pIdx == 2)
        {
            _StartUpPage.enabled = false;
            _GamePage.enabled = true;
            _InfoPage.enabled = true;
            _StartCaster.enabled = false;
            _GameCaster.enabled = false;
            _InfoCaster.enabled = true;
            _InfoPageText.text = "你输了";
            _InfoPanelRect.localScale = Vector3.one * infoScale;
        }
        else if (pIdx == 3)
        {
            _StartUpPage.enabled = false;
            _GamePage.enabled = true;
            _InfoPage.enabled = true;
            _StartCaster.enabled = false;
            _GameCaster.enabled = false;
            _InfoCaster.enabled = true;
            _InfoPageText.text = "你赢了";
            _InfoPanelRect.transform.localScale = Vector3.one * infoScale;
        }
    }
    void SetWidth(int width)
    {
        //renderSize = _GamePage.renderingDisplaySize;
        RectTransform rect = _MinePad.GetComponent<RectTransform>();
        float scale = (float)width / (ColCount * 32);
        Debug.Log(scale);
        rect.localScale = Vector3.one * scale;
        float height = 100 + scale * RowCount * 32;
        Screen.SetResolution(width, (int)height, false);

    }
    void OnBtnRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (CurrBlock == null) return;
            if (CurrBlock.isUnPacked) return;
            if (CurrBlock.isFlaged)
            {
                _unFlagedCount++;
                CurrBlock.SetSprite(BtnStatus[0]);
                if (CurrBlock.isBomb)
                    _unFlagedBombCount++;
            }
            else
            {
                _unFlagedCount--;
                CurrBlock.SetSprite(BtnStatus[2]);
                if (CurrBlock.isBomb)
                    _unFlagedBombCount--;
            }
            Debug.Log(_unFlagedBombCount);
            _RestBombCount.text = _unFlagedCount.ToString();
            CurrBlock.isFlaged = !CurrBlock.isFlaged;
            if (_unFlagedBombCount == 0)
            {
                // you win
                SetPage(3);
            }
        }
    }
}
