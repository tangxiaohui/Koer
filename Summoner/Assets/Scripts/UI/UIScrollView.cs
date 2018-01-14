using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

/// <summary>
/// 虚拟列表，主要是复用item
/// </summary>
[System.Serializable]
public class UIScrollView: ScrollRect {
    #region 枚举
    #endregion
    #region 变量
    #region 公共变量

    public int Col
    {
        get {
            if (horizontal == false && _col == 0)
            {
                _col = 1;
                CalculateColAndRow(1, 0);
            }
            return _col;
        }
        set {
            if (value > 0)
            {
                CalculateColAndRow(value,0);               
            }
        }
    }
    public int Row
    {
        get
        {
            if (horizontal && _row == 0)
            {
                _row = 1;
                CalculateColAndRow(0, 1);
            }
            return _row;
        }
        set {
            CalculateColAndRow(0, value);
        }
    }
    /// <summary>
    ///停止拖动的时候，是否将可视范围
    ///内第一个元素顶部或左边贴至列表
    ///的顶部或左边
    /// </summary>
    public bool IsAutoAligning;

    public int MaxItem
    {
        set {
            _maxVisibleItem = value;
            _forceMaxItem = true;
        }
    }

    [Header("Padding")]
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    public Vector2 Spacing;

    public GameObject Template;

    public System.Action<GameObject> Init;
    public System.Action<int,GameObject> InitializeItem;

    public System.Action<int> AutoAligningCallBack;

    #endregion
    #region  私有变量 

    Stack<GameObject> _objPool = new Stack<GameObject>();

    /// <summary>
    /// 是否强制设置最多的item
    /// </summary>
    bool _forceMaxItem = false;

    /// <summary>
    /// 用于判断可视范围最后一行是否显示完全
    /// </summary>
    bool _isFloor;

    /// <summary>
    /// 最后一排的数量,如果为0表示最后一排没有空位
    /// </summary>
    int _offset;
    /// <summary>
    /// 用于判断是否在最后一排不满的情况下是否达到最后一排
    /// </summary>
    bool _reachLast = false;

    List<string> _itemID;

    /// <summary>
    /// 是否达到需要复用组件的情况
    /// </summary>
    bool _reachMaxLimit;
    /// <summary>
    /// 应该有的item数量
    /// </summary>
    int _count = 0;

    /// <summary>
    /// 当前存在的第一个item的的索引
    /// </summary>
    int _firstIndex;

    /// <summary>
    /// 可视范围内第一个item索引,如果滚动很快，这个计算的不是很准确
    /// </summary>
    int _index = 0;

    /// <summary>
    /// 可视范围内的最大列数
    /// </summary>
    int _col;
    /// <summary>
    /// 可视范围内的最大行数
    /// </summary>
    int _row;

    Deque<GameObject> _items = new Deque<GameObject>();

    float _itemSize;

    /// <summary>
    /// 可视范围内第一个复用组件的索引
    /// </summary>
    int _itemIndex;

    /// <summary>
    /// 同时最多存在的item数量
    /// </summary>
    int _maxLimitItem;

    /// <summary>
    /// 可视范围内第一个item的最大索引
    /// </summary>
    int _maxFirstIndex;

    /// <summary>
    /// 可视范围内的最大item数
    /// </summary>
    int _maxVisibleItem;
    /// <summary>
    /// 用于判断何时生成复用item
    /// </summary>
    float _minValue = 0;
    /// <summary>
    /// 用于判断何时生成复用item
    /// </summary>
    float _maxValue = 0;
    /// <summary>
    /// _minValue和_maxValue的差值 
    /// </summary>
    float _distance;

    /// <summary>
    /// 第一个列表元素的ID
    /// </summary>
    int _firstItemID;
    /// <summary>
    /// 水平状态下最大列数
    /// </summary>
    int _maxCol;
    /// <summary>
    /// 垂直状态下最大行数
    /// </summary>
    int _maxRow;

    bool _isEndDrag = false;

    Dictionary<int, Vector3> _destination = new Dictionary<int, Vector3>();
    #endregion
    #endregion
    #region 函数
    #region 公共函数

    //获取Itema数量
    public int GetMaxCount()
    {
        if(_items != null)
        {
            return _items.Count;
        }
        return 0;
    }

    public void SetTemplate(GameObject pre)
    {
        Template = pre;
        if(Template != null)
        {
            Template.SetActive(false);
        }
    }


    public void ClearContent()
    {
        if (_items != null)
        {
            for (int i = 0; i < _items.Count; ++i)
            {
                //Destroy(_items[i]);
                _items[i].SetActive(false);
                _objPool.Push(_items[i]);
            }

            _items.Clear();
            _count = 0;
        }
        if (_itemID != null)
        {
            _itemID.Clear();
        }
        JumpToTop();
    }

    public void Refresh()
    {
        for (int i = 0, itemCount = _items.Count; i < itemCount; ++i)
        {
            ///_items的第一个位置上始终是0
            InitializeItem(_firstIndex + i, _items[i]);
        }
    }

    public void JumpToIndex(int index)
    {
        if (_count < _maxVisibleItem)
        {
            return;
        }
        if (index < 0 || index > _count)
        {
            return;
        }


        DoJump(index);
    }

    public void JumpToBottom()
    {
        if (_count < _maxVisibleItem)
            return;
        DoJump(_count);
    }

    /// <summary>
    /// 回到第一行/列
    /// </summary>
    public void JumpToTop(System.Action<GameObject> cb = null)
    {
        if (horizontal)
        {
            content.DOLocalMoveX(0, 0.01f).OnComplete(()=> {
                if (cb != null && _items.Count > 0)
                    cb(_items[0]);
            });
        }
        else
        {
            content.DOLocalMoveY(0, 0.01f).OnComplete(() => {
                if (cb != null && _items.Count > 0)
                    cb(_items[0]);
            });
        }
        return;
    }

    /// <summary>
    /// 刷新之后是否回到顶部
    /// </summary>
    /// <param name="jumpToTop"></param>
    public void Refresh(bool jumpToTop)
    {

        if (jumpToTop)
        {
            JumpToTop();
        }
        Refresh();
    }

    /// <summary>
    /// 在总数不变的情况下，添加一个，主要是聊天使用
    /// </summary>
    public void AddItemSpecial()
    {
        if (horizontal)
        {
            HFront2RearSpecial();
        }
        else
        {
            VFront2RearSpecial();
        }
    }

    public void AddItem()
    {
        ++_count;
        _offset = _count % (horizontal ? Row : Col);
        _maxFirstIndex = _count - _maxVisibleItem + _offset;
        if (horizontal)
        {
            _maxCol = Mathf.CeilToInt(_count * 1f / Row);
        }
        else
        {
            _maxRow = Mathf.CeilToInt(_count * 1f / Col);
        }
        if (_items.Count >= _maxLimitItem)
        {       
            _reachMaxLimit = true;
            return;
        }
        else
        {
            _reachMaxLimit = false;
            GameObject go = GetOneGameObject();
            if (_items.Count == 0)
                _firstItemID = go.GetInstanceID();
            go.transform.SetParent(content);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
			go.GetComponent<RectTransform>().SetAnchor(AnchorPresets.TopLeft);
            go.GetComponent<RectTransform>().SetPivot(PivotPresets.TopLeft);
            _items.AddToBack(go);
            if (Init != null)
            {
                Init(go);
            }
            SortLastItem();
            Initialize();
        }
    }

    public void AddItem(string ID)
    {
        if (_itemID == null)
            _itemID = new List<string>();

        
        _itemID.Add(ID);

        AddItem();
    }

    public void RemoveItem(string ID)
    {
        int index = _itemID.FindIndex(x => x.CompareTo(ID) == 0);
        if (index > -1)
        {
            _itemID.RemoveAt(index);
            RemoveItem(index);
        }
    }

    public bool ReachBottom()
    {
        if (_count <= _maxVisibleItem)
            return true;
        return _index == _count - _maxVisibleItem;
    }

    /// <summary>
    /// 删除的时候务必先把List中的数据清除
    /// </summary>
    /// <param name="index"></param>
    public void RemoveItem(int index)
    {
        --_count;
        
        int range = 0;
        if (_reachMaxLimit)
        {
            ///index - _index 是相对可视范围内第一个的偏移量
            ///_index - _firstIndex 是去除第一个item 到可视范围内第一个item的偏移量
            range = _maxLimitItem - (index - _index) - (_index - _firstIndex); 
        }
        else
        {
            range = _items.Count - (index - _firstIndex);
        }
        int itemIndex;
        for (int i = 0; i < range; ++i)
        {
            itemIndex = index - _index + _index - _firstIndex + i;
            itemIndex = itemIndex % _items.Count;
            if (_reachMaxLimit)
            {
                if (index + i < _count)
                {
                    InitializeItem(index + i, _items[itemIndex]);
                }
                else
                {
                    GameObject go = _items.RemoveFromBack();
                    if (horizontal)
                    {
                        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                          _items[itemIndex - _maxLimitItem + _row].GetComponent<RectTransform>().anchoredPosition.x - _itemSize - Spacing.x,
                          _items[itemIndex - _maxLimitItem + _row].GetComponent<RectTransform>().anchoredPosition.y);           
                    }
                    else
                    {
                        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                           _items[itemIndex - _maxLimitItem + _col].GetComponent<RectTransform>().anchoredPosition.x,
                           _items[itemIndex - _maxLimitItem + _col].GetComponent<RectTransform>().anchoredPosition.y + _itemSize + Spacing.y);
                    }
                    InitializeItem(index + i - _maxLimitItem, go);
                    _items.AddToFront(go);
                    --_firstIndex;

                    if (_offset == 1 || (horizontal && _row == 1) || (horizontal == false && _col == 1))
                    {
                        if (horizontal)
                        {
                            content.sizeDelta = new Vector2(content.sizeDelta.x - _itemSize - Spacing.x, content.sizeDelta.y);

                            if (_row == 1)
                            {
                                _minValue += _itemSize + Spacing.x;
                                _maxValue = _minValue + _distance;
                            }
                        }
                        else
                        {
                            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y - _itemSize - Spacing.y);

                            if (_col == 1)
                            {
                                _maxValue -= _itemSize + Spacing.y;
                                _minValue = _maxValue - _distance;
                            }
                        }
                    }
                    else if (_offset == 0)
                    {
                        _reachLast = true;
                    }
                }
            }
            else
            {
                if (index + 1 + i >= _items.Count)
                {
                    //Destroy(_items[itemIndex]);
                    _items[itemIndex].SetActive(false);
                    _objPool.Push(_items[itemIndex]);
                    _items.RemoveAt(itemIndex);

                    if (_offset == 1 || (horizontal && _row == 1) || (horizontal == false && _col == 1))
                    {
                        if (horizontal)
                        {
                            content.sizeDelta = new Vector2(content.sizeDelta.x - _itemSize - Spacing.x, content.sizeDelta.y);
                        }
                        else
                        {
                            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y - _itemSize - Spacing.y);
                        }
                    }
                }
                else
                {
                    InitializeItem(index + i, _items[itemIndex]);
                }
            }
        }
        _offset = _count % (horizontal ? Row : Col);
        if (_count <= _items.Count)
            _reachMaxLimit = false;
        //return true;
    }

    public void Initialize()
    {
        if (Template != null)
        {
            if (horizontal)
            {
                _itemSize = Template.GetComponent<RectTransform>().rect.size.x;

                _distance = content.rect.size.x - viewport.rect.size.x + Left;

                _minValue = -_distance;
                _maxValue = 0;
            }
            else
            {
                _itemSize = Template.GetComponent<RectTransform>().rect.size.y;

                _distance = content.rect.size.y - viewport.rect.size.y - Top;

                _minValue = 0;
                _maxValue = _distance;
            }
            _itemIndex = 0;
            _firstIndex = 0;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        _isEndDrag = false;
        content.DOKill();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        _isEndDrag = true;
    }
    #endregion
    #region 私有函数
    GameObject GetOneGameObject()
    {
        if (_objPool.Count == 0)
        {
            GameObject go = Instantiate<GameObject>(Template);
            return go;
        }
        else
        {
            return _objPool.Pop();
        }
    }

    void DoJump(int index)
    {
        float viewSize;
        float spacing;
        if (horizontal)
        {
            viewSize = viewport.rect.size.x;
            spacing = Spacing.x;
        }
        else
        {
            viewSize = viewport.rect.size.y;
            spacing = Spacing.y;
        }
        float target = index * (spacing + _itemSize) - spacing - viewSize;
        if (horizontal)
        {
            content.DOLocalMoveX(-target, 0.5f);
        }
        else
        {
            content.DOLocalMoveY(target, 0.5f);
        }
    }

    protected override void Awake()
    {

        onValueChanged.AddListener(WarpContent);
        base.Awake();
        if (Template != null)
        {
            Template.SetActive(false);
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (IsAutoAligning && _isEndDrag && velocity == Vector2.zero)
        {
            _isEndDrag = false;
            float posX, posY;
            int index = 0;
            if (horizontal)
            {
                int col = Mathf.RoundToInt(content.localPosition.x / (_itemSize + Spacing.x));

                posX = col * (_itemSize + Spacing.x);
                index = col * _row;
                if (-col == _maxCol - _maxVisibleItem / _row)
                    posX = -(content.sizeDelta.x - GetComponent<RectTransform>().sizeDelta.x);
                posY = content.localPosition.y;
            }
            else
            {
                int row = Mathf.RoundToInt(content.localPosition.y / (_itemSize + Spacing.y));
                index = row * _col;
                posX = content.localPosition.x;
                posY = row * (_itemSize + Spacing.y);
                if (row == _maxRow - _maxVisibleItem / _col)
                    posY = content.sizeDelta.y - GetComponent<RectTransform>().sizeDelta.y;
            }

            content.DOLocalMove(new Vector3(posX, posY, content.localPosition.z), 0.1f).OnComplete(()=> {
                if (AutoAligningCallBack != null)
                    AutoAligningCallBack(Mathf.Abs(index));
            });

        }
    }

    void SortLastItem()
    {
        Vector2 pos = Vector2.zero;


        Vector2 itemSize = Vector2.zero;
        if (_items.Count > 0)
            itemSize = _items[0].GetComponent<RectTransform>().rect.size;

        int col, row;
        int index = _items.Count - 1;
        if (horizontal)
        {
            if (_row == 0)
                Row = 1;
            col = index / _row;
            pos.x = col * (itemSize.x + Spacing.x) + Left;
            row = index % _row;
            pos.y = -row * (itemSize.y + Spacing.y) - Top;
        }
        else
        {
            if (_col == 0)
                Col = 1;
            row = index / _col;
            pos.y = -row * (itemSize.y + Spacing.y) - Top;
            col = index % _col;
            pos.x = col * (itemSize.x + Spacing.x) + Left;
        }
        InitializeItem(index, _items[index]);
        _items[index].GetComponent<RectTransform>().anchoredPosition = pos;

        AdjustContentSize();
    }

    void AdjustContentSize()
    {
        int col, row;

        Vector2 itemSize = Template.GetComponent<RectTransform>().rect.size;

        float offsetX = 0;
        float offsetY = 0;

        if (horizontal)
        {
            col = Mathf.CeilToInt(_items.Count * 1f / Row);
            row = _row;
            offsetX = Left;
        }
        else {
            row = Mathf.CeilToInt(_items.Count * 1f / Col);
            col = _col;
            offsetY = Top;    
        }

        content.sizeDelta = new Vector2((itemSize.x + Spacing.x) * col - Spacing.x + offsetX,
                                        (itemSize.y + Spacing.y) * row - Spacing.y + offsetY);
    }

    /// <summary>
    /// 主要逻辑：通过content的位置来判断是否需要循环使用item
    /// </summary>
    /// <param name="delta"></param>
    void WarpContent(Vector2 delta)
    {
        if (_items.Count == 0)
            return;

        PreCheck(true);

        if (horizontal)
        {
            ///计算目前显示的第一个item
            int value = Mathf.FloorToInt(-content.localPosition.x / (_itemSize + Spacing.x)) * _row;

            if (value >= 0)
            {
                value = value > _maxFirstIndex ? _maxFirstIndex : value;
                _index = value;
                ///itemIndex是循环利用的，所以余_items.Count
                _itemIndex = value % _items.Count;
            }
            ///当前是否显示的最后一列
            if (_index < _count - _maxVisibleItem)
            {
                ///是否需要重新使用未显示的item
                if (content.localPosition.x <= _minValue)
                {
                   
                    int lastIndex;
                    for (int i = 0; i < _row; ++i)
                    {
                        ///是否达到最后一个
                        lastIndex = _firstIndex + _maxLimitItem;
                        if (lastIndex >= _count)
                        {
                            _reachLast = true;
                            break;
                        }
                        HFront2Rear();
                    }
                    ///content扩大1列的大小
                    content.sizeDelta = new Vector2(content.sizeDelta.x + _itemSize + Spacing.x, content.sizeDelta.y);
                    ///移动最大，最小值的位置
                    _minValue -= _itemSize + Spacing.x;
                    _maxValue = _minValue + _distance;
 
                }
                else if (content.localPosition.x >= _maxValue && Mathf.Abs(_maxValue) > 0.000001f && _firstIndex > 0)
                {
                    int condition = _reachLast ? _offset : _row;
                    for (int i = 0; i < condition; ++i)
                    {
                        HRear2Front();
                    }
                    _reachLast = false;
                    ///condition为0表示是最后一行只有1个的情况下删除最后一个，
                    ///因为删除时已经缩小了content,所以这里就不再重复缩小了
                    if (condition != 0)
                        content.sizeDelta = new Vector2(content.sizeDelta.x - _itemSize - Spacing.x, content.sizeDelta.y);

                    _minValue += _itemSize + Spacing.x;
                    _maxValue = _minValue + _distance;
                }
            }
        }
        else
        {
            int value = Mathf.FloorToInt(content.localPosition.y / (_itemSize + Spacing.y)) * _col;
           
            if (value >= 0)
            {
                value = value > _maxFirstIndex ? _maxFirstIndex : value;
                _index = value;
                _itemIndex = value % _items.Count;
            }

            if (_index < _count - _maxVisibleItem)
            {
                if (content.localPosition.y >= _maxValue)
                {
                    for (int i = 0; i < _col; ++i)
                    {
                        if (_firstIndex + _maxLimitItem >= _count)
                        {
                            _reachLast = true;
                            break;
                        }
                        VFront2Rear();
                    }
                    content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + _itemSize + Spacing.y);

                    _maxValue += _itemSize + Spacing.y;
                    _minValue = _maxValue - _distance;
                }
                else if (content.localPosition.y <= _minValue && Mathf.Abs(_minValue) > 0.000001f && _firstIndex > 0)
                {
                    int condition = _reachLast ? _offset : _col;
                    for (int i = 0; i < condition; ++i)
                    {
                        VRear2Front();
                    }

                    _reachLast = false;
                    if(condition != 0)
                        content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y - _itemSize - Spacing.y);

                    _maxValue -= _itemSize + Spacing.y;
                    _minValue = _maxValue - _distance;
                }
            }
        }
    }

    void HFront2Rear()
    {
        GameObject go;
        go = _items.RemoveFromFront();
        ///_items.Count - _row  本来是 _item.Count - 1 - _row + 1
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            _items[_items.Count - _row].GetComponent<RectTransform>().anchoredPosition.x + _itemSize + Spacing.x,
            _items[_items.Count - _row].GetComponent<RectTransform>().anchoredPosition.y);
        int realIndex = _firstIndex + _maxLimitItem;
        if (realIndex > _count - 1)
            realIndex = _count - 1;
        InitializeItem(realIndex, go);
        _items.AddToBack(go);
        ++_firstIndex;
    }

    void HRear2Front()
    {
        GameObject go;
        go = _items.RemoveFromBack();
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            _items[_row - 1].GetComponent<RectTransform>().anchoredPosition.x - _itemSize - Spacing.x,
            _items[_row - 1].GetComponent<RectTransform>().anchoredPosition.y);

        InitializeItem(_firstIndex - 1, go);
        _items.AddToFront(go);
        --_firstIndex;
    }

    void VFront2Rear()
    {
        GameObject go;
        go = _items.RemoveFromFront();
        //Debug.LogError("index:" + (_items.Count - _col).ToString() + "  " + _items[_items.Count - _col]);
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            _items[_items.Count - _col].GetComponent<RectTransform>().anchoredPosition.x,
            _items[_items.Count - _col].GetComponent<RectTransform>().anchoredPosition.y - _itemSize - Spacing.y);

        int realIndex = _firstIndex + _maxLimitItem;
        if (realIndex > _count - 1)
            realIndex = _count - 1;
        InitializeItem(realIndex, go);

        _items.AddToBack(go);
        ++_firstIndex;
    }

    void VRear2Front()
    {
        GameObject go;
        go = _items.RemoveFromBack();
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            _items[_col - 1].GetComponent<RectTransform>().anchoredPosition.x,
            _items[_col - 1].GetComponent<RectTransform>().anchoredPosition.y + _itemSize + Spacing.y);

        InitializeItem(_firstIndex - 1, go);

        _items.AddToFront(go);
        --_firstIndex;
    }

    /// <summary>
    /// firstIndex值不变
    /// </summary>
    void HFront2RearSpecial()
    {
        GameObject go;

        go = _items.RemoveFromFront();
        ///_items.Count - _row  本来是 _item.Count - 1 - _row + 1
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            _items[_items.Count - _row].GetComponent<RectTransform>().anchoredPosition.x + _itemSize + Spacing.x,
            _items[_items.Count - _row].GetComponent<RectTransform>().anchoredPosition.y);
        InitializeItem(_firstIndex + _maxLimitItem - 1, go);
        _items.AddToBack(go);
    }

    /// <summary>
    /// firstIndex值不变
    /// </summary>
    void VFront2RearSpecial()
    {
        if (_items.Count == 0)
            return;
        PreCheck();
        GameObject go;
        go = _items.RemoveFromFront();      
        Vector3 pos = go.GetComponent<RectTransform>().anchoredPosition;
      
        go.transform.DOKill();
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            _items[_items.Count - _col].GetComponent<RectTransform>().anchoredPosition.x,
            _items[_items.Count - _col].GetComponent<RectTransform>().anchoredPosition.y - _itemSize - Spacing.y);

        InitializeItem(_firstIndex + _maxLimitItem - 1, go);

        _items.AddToBack(go);

        int id;
        for (int i = _items.Count - 1; i > 0; --i)
        {
            id = _items[i].transform.GetInstanceID();
            _destination[id] = _items[i - 1].GetComponent<RectTransform>().anchoredPosition;
            _items[i].transform.DOKill();
            DoLocalMove(_items[i].transform, _items[i - 1].GetComponent<RectTransform>().anchoredPosition);
        }

        id = _items[0].transform.GetInstanceID();
        _destination[id] = pos;
        DoLocalMove(_items[0].transform, pos);
    }

    /// <summary>
    /// 主要是检查item是否还在doLocalMove
    /// </summary>
    void PreCheck(bool IsDrag = false)
    {
        int id;
        if (_destination.Count != 0)
        {
            for (int i = 0, count = _items.Count; i < count; ++i)
            {
                _items[i].transform.DOKill();
                id = _items[i].transform.GetInstanceID();
                _items[i].GetComponent<RectTransform>().anchoredPosition = _destination[id];
            }
        }
        if (IsDrag)
        {
            _destination.Clear();
        }
    }

    void DoLocalMove(Transform trans,Vector3 pos)
    {
        trans.DOLocalMove(pos, 0.5f).OnComplete(() => {
            _destination.Clear();
        });
    }

    void CalculateMaxItemNum()
    {
        if (_forceMaxItem == false)
        {
            if (_isFloor)
            {
                if (horizontal)
                    _maxVisibleItem = _row * (_col + 1);
                else
                    _maxVisibleItem = _col * (_row + 1);
            }
            else
            {
                _maxVisibleItem = _col * _row;
            }
        }

        if (horizontal)
        {
            _maxLimitItem = _maxVisibleItem + Row * 2;
        }
        else
        {
            _maxLimitItem = _maxVisibleItem + Col * 2;
        }
    }

    void CalculateColAndRow(int col,int row)
    {
        Vector2 viewportSize = viewport.rect.size;

        Vector2 contentSize = Template.GetComponent<RectTransform>().rect.size;

        ///先去除距离边界的固定大小
        viewportSize.x -= Left;
        viewportSize.x -= Right;
        viewportSize.y -= Top;
        viewportSize.y -= Bottom;

        float colValue = (viewportSize.x + Spacing.x) / (contentSize.x + Spacing.x);
        float rowValue = (viewportSize.y + Spacing.y) / (contentSize.y + Spacing.y);

        int maxCol = Mathf.FloorToInt(colValue);
        int maxRow = Mathf.FloorToInt(rowValue);

        maxCol = maxCol == 0 ? 1 : maxCol;
        maxRow = maxRow == 0 ? 1 : maxRow;

        if (col == 0)
        {
            _col = maxCol;
        }
        else
        {
            _isFloor = Mathf.Abs(rowValue - maxRow) > 0.000001f;
            _col = col > maxCol ? maxCol : col;
        }

     
        if (row == 0)
        {
            _row = maxRow;
        }
        else
        {
            _isFloor = Mathf.Abs(colValue - maxCol) > 0.000001f;
            _row = row > maxRow ? maxRow : row;
        }

        CalculateMaxItemNum();
    }

    #endregion
    #endregion

}
