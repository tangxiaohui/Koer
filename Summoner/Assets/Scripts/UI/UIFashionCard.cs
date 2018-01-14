using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

/// <summary>
/// 计算移动的是按百分比来移动的，100百分比（即1）为移动相邻卡牌的距离，
/// 这么做主要是因为中间卡牌和其相邻的卡牌距离同其他卡牌不一致
/// PS:起始代表最左边卡牌位置，最后卡牌代表最右边卡牌位置
/// </summary>
public class UIFashionCard : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region 枚举
    #endregion
    #region 常量
    const float ANIM_LENGTH = 0.5f;
    /// <summary>
    /// 一次性最大移动单位
    /// </summary>
    const float MAX_LERP = 1f;
    #endregion
    #region 变量
    #region 公共变量
    [Header("中间的卡牌的位置")]
    public Vector3 ChosePos;
    [Header("单边最多显示多少个")]
    public int OneSideCount;
    [Header("最右边卡牌位置")]
    public Vector3 MostRightPos;
    [Header("非中间卡牌的Z轴间隔(正数)")]
    public float IntervalZ;
    [Header("非中间卡牌的X轴间隔(正数)")]
    public float IntervalX;
    [Header("非中间卡牌的旋转角度(正数)")]
    public float RotateAngle;

    public GameObject TemplateItem;

    public System.Action<GameObject> InitChosed;
    public System.Action<GameObject> InitUnChose;

    /// <summary>
    /// Item的初始化函数
    /// </summary>
    public System.Action<int, GameObject> InitializeItem;

    public int Index {
        get {
            return _index;
        }
    }
    #endregion
    #region  私有变量 
    Stack<GameObject> _objPool = new Stack<GameObject>();
    List<GameObject> _items = new List<GameObject>();
    /// <summary>
    /// 当前选中的索引
    /// </summary>
    int _index = 0;
    /// <summary>
    /// 用于计算新的索引
    /// </summary>
    int _newIndex = -1;

    int _allCount = 0;

    Vector3 _rightRotateEular;
    Vector3 _leftRotateEular;
    Vector3 _mostLeftPos;
    Vector3 _mostLeftAvailablePos;
    Vector3 _mostRightAvailablePos;

    Vector2 _lastPosition;
    Vector2 _eachMoveVec;
    /// <summary>
    /// 左边卡牌位置的差值
    /// </summary>
    Vector3 _leftDiffVec;
    /// <summary>
    /// 左边卡牌至中间卡牌位置的差值
    /// </summary>
    Vector3 _leftDiff2MidVec;
    /// <summary>
    /// 中间卡牌至右边卡牌位置的差值
    /// </summary>
    Vector3 _rightDiff2MidVec;
    /// <summary>
    /// 右边卡牌位置的差值
    /// </summary>
    Vector3 _rightDiffVec;

    List<Vector3> _allPos = new List<Vector3>();

    /// <summary>
    /// 滑动的修正值
    /// </summary>
    float _modify = 0.01f;
    /// <summary>
    /// 移动几百分比，相邻卡牌距离为1百分比
    /// </summary>
    float _movement = 0;
    #endregion
    #endregion
    #region 函数
    #region 公共函数
    public void ClearContent()
    {
        if (_items != null)
        {
            for (int i = 0; i < _items.Count; ++i)
            {
                _items[i].SetActive(false);
                _objPool.Push(_items[i]);
            }
            _items.Clear();
        }
        _allCount = 0;
    }

    public void AddItem(int count,int beginIndex = 0)
    {
        _index = beginIndex;
        for (int i = 0; i < count; ++i)
        {
            InitGo();
            if (InitializeItem != null)
            {
                InitializeItem(i, _items[i]);        
            }
        }
        _allCount = count;
        Sorting(true);
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastPosition = eventData.position;
        for (int i = 0; i < _items.Count; ++i)
        {
            _items[i].transform.DOKill();
            InitUnChose(_items[i]);
        }
        ImmediatelySorting();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Sorting(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _eachMoveVec = eventData.position - _lastPosition;
        
        _movement = _eachMoveVec.x * _modify;
        if (Mathf.Abs(_movement) < 0.000001f)
            return;
        //Debug.LogError("_movement:" + _movement);
        if (_movement > 0)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
        _lastPosition = eventData.position;
    }
    #region 计算移动
    #region 向左移动 
    void MoveLeft()
    {
        if (Mathf.Abs(_movement) > MAX_LERP)
            _movement = -MAX_LERP;
        Vector3 pos;
        ///计算右边
        _newIndex = -1;
        bool shouldMoveRight = true;
        ///全部卡牌都集中在最右边的极端情况
        if (_index < _items.Count - 1)
        {
            if ((_items[_index + 1].transform.localPosition.x - _items[_index].transform.localPosition.x) < _rightDiffVec.x)
            {
                shouldMoveRight = false;
            }
        }
        
        if(shouldMoveRight)
        {
            ///相邻卡牌距离是否小于规定的距离
            bool isCloser = false;
            for (int i = _index + 1, count = _items.Count; i < count; ++i)
            {
                pos = _items[i].transform.localPosition;
                ///判断移动前相邻卡牌距离是否小于规定的距离
                if (i < count - 1 && ((_items[i + 1].transform.localPosition.x - _items[i].transform.localPosition.x) < _rightDiffVec.x))
                {
                    isCloser = true;
                }

                ///右边卡牌在中间至右边起始之间
                if (pos.x < _allPos[OneSideCount + 1].x)
                {
                    RightMidTowardsLeft(pos, i, true);
                }
                else
                {
                    RightTowardsLeft(pos, i);
                }

                ///在右边被完全遮住的卡牌就不动了
                ///检查移动后相邻卡牌距离是否小于规定的距离
                if (isCloser)
                {
                    if ((_items[i + 1].transform.localPosition.x - _items[i].transform.localPosition.x) >= _rightDiffVec.x)
                    {

                        _items[i + 1].transform.localPosition = _items[i].transform.localPosition + _rightDiffVec;
                    }
                    break;
                }
            }
        }

        ///计算中间卡牌
        pos = _items[_index].transform.localPosition;
        ///中间卡牌在右边起始右边
        if (pos.x > _allPos[OneSideCount + 1].x)
        {
            RightTowardsLeft(pos, _index);
        }
        ///中间卡牌在中间至右边起始之间
        else if (pos.x > _allPos[OneSideCount].x)
        {
            RightMidTowardsLeft(pos, _index, false);
        }
        else
        {
            ///中间卡牌出现在左边最后的左边
            if (pos.x < _allPos[OneSideCount - 1].x)
            {
                SideMove(pos, _index, false);
            }
            else
            {
                MidTowardsLeft(pos, _index, false);
            }
        }

        ///右边卡牌集中的情况下，中间卡牌移动超过规定距离，才开始移动右边卡牌
        if (shouldMoveRight == false)
        {
            if ((_items[_index + 1].transform.localPosition.x - _items[_index].transform.localPosition.x) >= _rightDiffVec.x)
            {
                _items[_index + 1].transform.localPosition = _items[_index].transform.localPosition + _rightDiffVec;
            }
        }

        /// 计算左边卡牌
        for (int i = _index - 1; i > -1; --i)
        {
            pos = _items[i].transform.localPosition;
            
            ///左边最后的左边
            if (pos.x < _allPos[OneSideCount - 1].x)
            {
                SideMove(pos, i, false);
            }
            else///在左边最后至中间之间
            {
                MidTowardsLeft(pos, i, false);         
            }
          
        }
        if (_newIndex != -1)
        {
            _index = _newIndex;

            SetLayer();
        }
    }

    /// <summary>
    /// 卡牌从右边起始以右向左移动
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    void RightTowardsLeft(Vector3 pos, int index)
    {
        pos += _movement * _rightDiffVec;
        ///移动超过右边起始
        if (pos.x < _allPos[OneSideCount + 1].x)
        {
            pos = _items[index].transform.localPosition;
            ///计算卡牌到右边起始距离几个百分比
            float percent = (pos.x - _allPos[OneSideCount + 1].x) / _rightDiffVec.x;
            _movement += percent;

            pos = _allPos[OneSideCount + 1];

            RightMidTowardsLeft(pos, index, true);
        }
        else
        {
            _items[index].transform.localPosition = pos;
        }
    }

    /// <summary>
    /// 卡牌从右边起始向左移动
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    /// <param name="isRightCard">是否是右边的卡牌</param>
    void RightMidTowardsLeft(Vector3 pos, int index, bool isRightCard)
    {
        pos += _movement * _rightDiff2MidVec;
        ///卡牌移动超过中间位置
        if (pos.x < _allPos[OneSideCount].x)
        {
            pos = _items[index].transform.localPosition;
            float percent;
            if (isRightCard)
            {
                ///右边卡牌则直接是移动了1个单位了
                percent = 1;
            }
            else 
            {
                ///计算卡牌在右边边至中间位置百分比
                percent = (pos.x - _allPos[OneSideCount].x) / _rightDiff2MidVec.x;
            }

            _movement += percent;
            pos = _allPos[OneSideCount];
            MidTowardsLeft(pos, index, isRightCard);
        }
        else
        {
            _items[index].transform.localPosition = pos;
            ///只有右边卡牌可能成为新的索引
            if (CalculateAngle(index, pos, true) < 0.5f && isRightCard)
            {
                _newIndex = index;
            }
        }
    }

    /// <summary>
    /// 卡牌从中间向左移动
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    /// <param name="isLeftCard">是否是右边的卡牌</param>
    void MidTowardsLeft(Vector3 pos, int index, bool isRightCard)
    {
        pos += _movement * _leftDiff2MidVec;

        if (pos.x < _allPos[OneSideCount - 1].x)
        {
            MidMoveToLeft(index, isRightCard);
            if (isRightCard)
            {
                _newIndex = index;
            }
        }
        else
        {
            _items[index].transform.localPosition = pos;
            ///只有右边卡牌可能成为新的索引
            if (CalculateAngle(index, pos, false) < 0.5f && isRightCard)
            {
                _newIndex = index;
            }
        }
    }


    /// <summary>
    /// 卡牌从左边最后的右边移动到左边（起始位置最多至中间）
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isLeftCard">是否是右边的卡牌</param>
    void MidMoveToLeft(int index, bool isRightCard)
    {
        Vector3 pos;
        float percent;
        if (isRightCard)
        {
            percent = 1;
        }
        else
        {
            pos = _items[index].transform.localPosition;
            ///计算中间卡牌在左边最后至中间卡牌的位置百分比
            percent = (pos.x - _allPos[OneSideCount - 1].x) / _leftDiff2MidVec.x;
        }

        _movement += percent;

        pos = _allPos[OneSideCount - 1];
        SideMove(pos, index, false);
    }
    #endregion
    #region 向右移动
    void MoveRight()
    {
        if (_movement > MAX_LERP)
            _movement = MAX_LERP;
        Vector3 pos;
        ///计算右边
        for (int i = _items.Count - 1; i > _index; --i)
        {
            pos = _items[i].transform.localPosition;
            ///在中间至右边起始之间
            if (pos.x < _allPos[OneSideCount + 1].x)
            {
                MidTowardsRight(pos, i, false);
            }
            else
            {
                SideMove(pos, i, true);
            }
        }

        bool shouldMoveLeft = true;
        ///全部卡牌都集中在最左边的极端情况
        if (_index > 0)
        {
            if ((_items[_index].transform.localPosition.x - _items[_index - 1].transform.localPosition.x) < _leftDiffVec.x)
            {
                shouldMoveLeft = false;
            }
        }

        ///计算中间卡牌
        pos = _items[_index].transform.localPosition;
        /// 卡牌在左边最后以左
        if (pos.x < _allPos[OneSideCount - 1].x)
        {
            LeftTowardsRight(pos, _index);
        }
        ///卡牌在左边最后至中间之间
        else if (pos.x < _allPos[OneSideCount].x)
        {
            LeftMidTowardsRight(pos, _index, false);
        }
        else
        {
            ///中间卡牌出现在右边起始右边
            if (pos.x > _allPos[OneSideCount + 1].x)
            {
                SideMove(pos, _index, true);
            }
            else
            {
                MidTowardsRight(pos, _index, false);
            }
        }

        /// 计算左边卡牌
        _newIndex = -1;
        if (shouldMoveLeft == false)
        {
            if ((_items[_index].transform.localPosition.x - _items[_index - 1].transform.localPosition.x) >= _leftDiffVec.x)
            {
                _items[_index - 1].transform.localPosition = _items[_index].transform.localPosition - _leftDiffVec;
                shouldMoveLeft = true;
            }
        }

        if (shouldMoveLeft)
        {
            ///相邻卡牌距离是否小于规定的距离
            bool isCloser = false;
            for (int i = _index - 1; i > -1; --i)
            {
                pos = _items[i].transform.localPosition;
                ///判断移动前相邻卡牌距离是否小于规定的距离
                if (i > 0 && ((_items[i].transform.localPosition.x - _items[i - 1].transform.localPosition.x) < _leftDiffVec.x))
                {
                    isCloser = true;
                }

                ///左边卡牌在左边最后至中间之间
                if (pos.x > _allPos[OneSideCount - 1].x)
                {
                    LeftMidTowardsRight(pos, i, true);
                }
                else
                {
                    LeftTowardsRight(pos, i);
                }

                ///在左边被完全遮住的卡牌就不动了
                ///检查移动后相邻卡牌距离是否小于规定的距离
                if (isCloser)
                {
                    if ((_items[i].transform.localPosition.x - _items[i - 1].transform.localPosition.x) >= _leftDiffVec.x)
                    {
                        _items[i - 1].transform.localPosition = _items[i].transform.localPosition - _leftDiffVec;
                    }
                    break;
                }
            }
        }
        if (_newIndex != -1)
        {
            _index = _newIndex;
            SetLayer();
        }
    }

    /// <summary>
    /// 卡牌从左边最后以左向右移动
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    void LeftTowardsRight(Vector3 pos, int index)
    {
        pos += _movement * _leftDiffVec;
        ///移动超过左边最后
        if (pos.x > _allPos[OneSideCount - 1].x)
        {
            pos = _items[index].transform.localPosition;
            ///计算卡牌到左边最后距离几个百分比
            float percent = (_allPos[OneSideCount - 1].x - pos.x) / _leftDiffVec.x;
            _movement -= percent;
            pos = _allPos[OneSideCount - 1];

            LeftMidTowardsRight(pos, index, true);
        }
        else
        {
            _items[index].transform.localPosition = pos;
        }
    }

    /// <summary>
    /// 卡牌从左边最后向右移动
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    /// <param name="isLeftCard">是否是左边卡牌</param>
    void LeftMidTowardsRight(Vector3 pos,int index, bool isLeftCard)
    {
        pos += _movement * _leftDiff2MidVec;
        ///卡牌移动超过中间位置
        if (pos.x > _allPos[OneSideCount].x)
        {
            pos = _items[index].transform.localPosition;
            float percent;
            if (isLeftCard)
            {
                ///左边卡牌则直接是移动了1个单位了
                percent = 1;
            }
            else
            {
                ///计算中间卡牌在左边至中间位置百分比
                percent = (_allPos[OneSideCount].x - pos.x) / _leftDiff2MidVec.x;
            }
            _movement -= percent;
            pos = _allPos[OneSideCount];
            MidTowardsRight(pos, index, isLeftCard);
        }
        else
        {
            _items[index].transform.localPosition = pos;
            ///只有左边卡牌可能成为新的索引
            if(CalculateAngle(index, pos, false) < 0.5f && isLeftCard)
            {
                _newIndex = index;
            }
        }
    }

    /// <summary>
    /// 卡牌从中间向右移动
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    /// <param name="isLeftCard">是否是左边的卡牌</param>
    void MidTowardsRight(Vector3 pos,int index,bool isLeftCard)
    {
        pos += _movement * _rightDiff2MidVec;
        if (pos.x > _allPos[OneSideCount + 1].x)
        {
            MidMoveToRight(index, isLeftCard);
            if (isLeftCard)
            {
                _newIndex = index;
            }
        }
        else
        {
            _items[index].transform.localPosition = pos;
            ///只有左边卡牌可能成为新的索引
            if (CalculateAngle(index, pos, true) < 0.5f && isLeftCard)
            {
                _newIndex = index;
            }
        }
    }


    /// <summary>
    /// 卡牌从右边起始的左边移动到右边（起始位置最多至中间）
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isLeftCard">是否是左边的卡牌</param>
    void MidMoveToRight(int index,bool isLeftCard)
    {
        Vector3 pos;
        float percent;
        if (isLeftCard)
        {
            percent = 1;
        }
        else
        {
            pos = _items[index].transform.localPosition;
            ///计算卡牌在中间至右边起始的位置百分比
            percent = (_allPos[OneSideCount + 1].x - pos.x) / _rightDiff2MidVec.x;
        }
        _movement -= percent;

        pos = _allPos[OneSideCount + 1];
        SideMove(pos, index, true);
    }
    #endregion
    /// <summary>
    /// 左边最后以左，右边起始以右的移动，主要是判断越界
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    /// <param name="isRight"></param>
    void SideMove(Vector3 pos,int index,bool isRight)
    {
        if (isRight)
        {
            pos += _movement * _rightDiffVec;
            if (pos.x > MostRightPos.x)
            {
                pos = MostRightPos;
            }
            _items[index].transform.localPosition = pos;
            _items[index].transform.localEulerAngles = _rightRotateEular;
        }
        else
        {
            pos += _movement * _leftDiffVec;
            if (pos.x < _mostLeftPos.x)
            {
                pos = _mostLeftPos;
            }
            _items[index].transform.localPosition = pos;
            _items[index].transform.localEulerAngles = _leftRotateEular;
        }
    }

    /// <summary>
    /// 计算位于左边最后和右边起始之间的偏转角度
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <param name="isRight"></param>
    /// <returns></returns>
    float CalculateAngle(int index,Vector3 pos,bool isRight)
    {
        if (isRight)
        {
            ///计算卡牌在中间至右边起始的位置百分比
            float percent = (_allPos[OneSideCount + 1].x - pos.x) / _rightDiff2MidVec.x;
            float angle = RotateAngle * (1- percent);
            _items[index].transform.localEulerAngles = new Vector3(0, angle, 0);
            return 1 - percent;
        }
        else
        {
            ///计算卡牌在左边最后至中间的位置百分比
            float percent = (_allPos[OneSideCount].x - pos.x) / _leftDiff2MidVec.x;
            float angle = -RotateAngle * percent;
            _items[index].transform.localEulerAngles = new Vector3(0, angle, 0);
            return percent;
        }
    }
    #endregion
    #endregion
    #region 私有函数
    GameObject GetOneGameObject()
    {
        if (_objPool.Count == 0)
        {
            GameObject go = Instantiate<GameObject>(TemplateItem);
            return go;
        }
        else
        {
            return _objPool.Pop();
        }
    }

    void InitGo()
    {
        GameObject go = GetOneGameObject();
        go.transform.SetParent(transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = ChosePos;
        go.transform.localEulerAngles = Vector3.zero;
        go.SetActive(true);
        if (InitUnChose != null)
        {
            InitUnChose(go);
        }
        _items.Add(go);
    }

    void Awake()
    {
        if (TemplateItem != null)
        {
            TemplateItem.SetActive(false);
        }

        _rightRotateEular = new Vector3(0,Mathf.Abs(RotateAngle),0);
        _leftRotateEular = new Vector3(0, -_rightRotateEular.y, 0);

        _mostLeftPos = MostRightPos;
        _mostLeftPos.x = -_mostLeftPos.x;

        _mostLeftAvailablePos = _mostLeftPos;
        _mostLeftAvailablePos.x += IntervalX;
        _mostLeftAvailablePos.z -= IntervalZ;

        _mostRightAvailablePos = MostRightPos;
        _mostRightAvailablePos.x -= IntervalX;
        _mostRightAvailablePos.z -= IntervalZ;

        InitAllPos();
    }

    void ImmediatelySorting()
    {
        SetLayer();
        Vector3 targetPos;
        Vector3 beginPos = new Vector3(MostRightPos.x - OneSideCount * IntervalX, MostRightPos.y, MostRightPos.z - OneSideCount * IntervalZ);
        for (int i = _items.Count - 1; i > _index; --i)
        {
            targetPos = beginPos;

            targetPos.z += (i - _index - 1) * IntervalZ;
            targetPos.x += (i - _index - 1) * IntervalX;
            if (targetPos.x > _mostRightAvailablePos.x)
                targetPos = _mostRightAvailablePos;

            _items[i].transform.localPosition = targetPos;
            _items[i].transform.localEulerAngles = _rightRotateEular;
        }

        beginPos = new Vector3(_mostLeftPos.x + OneSideCount * IntervalX, _mostLeftPos.y, _mostLeftPos.z - OneSideCount * IntervalZ);
        for (int i = _index - 1; i > -1; --i)
        {
            targetPos = beginPos;
            targetPos.z += (_index - 1 - i) * IntervalZ;
            targetPos.x += (i - _index + 1) * IntervalX;
            if (targetPos.x < _mostLeftAvailablePos.x)
                targetPos = _mostLeftAvailablePos;
            _items[i].transform.localPosition = targetPos;
            _items[i].transform.localEulerAngles = _leftRotateEular;

        }

        _items[_index].transform.localEulerAngles = Vector3.zero;
        _items[_index].transform.localPosition = ChosePos;
    }

    void SetLayer()
    {
        ///先把右边的卡牌按层次排序
        for (int i = _items.Count - 1; i > _index; --i)
        {
            _items[i].transform.SetAsLastSibling();
        }

        for (int i = 0; i < _index; ++i)
        {
            _items[i].transform.SetAsLastSibling();
        }
        _items[_index].transform.SetAsLastSibling();
    }

    void Sorting(bool isFull)
    {
        float animLength;
        if (isFull)
        {
            animLength = ANIM_LENGTH;
        }
        else
        {
            float posX = _items[_index].transform.localPosition.x;
            if (posX <= _allPos[OneSideCount - 1].x || posX >= _allPos[OneSideCount + 1].x)
            {
                animLength = ANIM_LENGTH;
            }
            else
            {
                animLength = (Mathf.Abs(posX - _allPos[OneSideCount].x) / _leftDiff2MidVec.x) * ANIM_LENGTH;
            }
        }
        SetLayer();

        Vector3 targetPos;
        Vector3 beginPos = new Vector3(MostRightPos.x - OneSideCount * IntervalX,MostRightPos.y,MostRightPos.z - OneSideCount * IntervalZ);
        for (int i = _items.Count - 1; i > _index; --i)
        {
            targetPos = beginPos;

            targetPos.z += (i - _index - 1) * IntervalZ;
            targetPos.x += (i - _index - 1) * IntervalX;
            if (targetPos.x > _mostRightAvailablePos.x)
                targetPos = _mostRightAvailablePos;

            _items[i].transform.DOLocalMove(targetPos, animLength);
            _items[i].transform.DORotate(_rightRotateEular, animLength);

            InitUnChose(_items[i]);
        }

        beginPos = new Vector3(_mostLeftPos.x + OneSideCount * IntervalX, _mostLeftPos.y, _mostLeftPos.z - OneSideCount * IntervalZ);
        for (int i = _index - 1; i > -1; --i)
        {
            targetPos = beginPos;
            targetPos.z += (_index - 1 - i) * IntervalZ;
            targetPos.x += (i - _index + 1) * IntervalX;
            if (targetPos.x < _mostLeftAvailablePos.x)
                targetPos = _mostLeftAvailablePos;
            _items[i].transform.DOLocalMove(targetPos, animLength);
            _items[i].transform.DORotate(_leftRotateEular, animLength);

            InitUnChose(_items[i]);
        }

        _items[_index].transform.DORotate(Vector3.zero, animLength);
        _items[_index].transform.DOLocalMove(ChosePos, animLength).OnComplete(()=> {
            InitChosed(_items[_index]);
        });
    }

    void InitAllPos()
    {
        Vector3 pos;
        ///先计算左边的位置
        for (int i = 0; i < OneSideCount; ++i)
        {
            pos = _mostLeftAvailablePos;
            pos.x += i * IntervalX;
            pos.z -= i * IntervalZ;
            _allPos.Add(pos);
        }
        _allPos.Add(ChosePos);
        ///计算右边的位置
        Vector3 beginPos = new Vector3(MostRightPos.x - OneSideCount * IntervalX, MostRightPos.y, MostRightPos.z - OneSideCount * IntervalZ);
        for (int i = 0; i < OneSideCount; ++i)
        {
            pos = beginPos;
            pos.x += i * IntervalX;
            pos.z += i * IntervalZ;
            _allPos.Add(pos);
        }

        _leftDiffVec = _allPos[1] - _allPos[0];
        _leftDiff2MidVec = _allPos[OneSideCount] - _allPos[OneSideCount - 1];
        _rightDiff2MidVec = _allPos[OneSideCount + 1] - _allPos[OneSideCount];
        _rightDiffVec = _allPos[_allPos.Count - 1] - _allPos[_allPos.Count - 2];
    }
    #endregion
    #endregion

}
