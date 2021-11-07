using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class Candy : MonoBehaviour
{

    public List<Sprite> sprites;

    public int spriteIndex;
    public float moveSpeed = 0.2f;

    private static Candy selected;

    private SpriteRenderer render;

    public int row = -1;
    public int column = -1;
    public bool moveDone = true;

    bool touchBlock = false;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        spriteIndex = Random.Range(0, sprites.Count);
        render.sprite = sprites[spriteIndex];
    }

    public void SetRowColumn(int row, int col)
    {
        this.row = row;
        this.column = col;
    }


    public void Select()
    {
        render.color = Color.grey;
    }


    public void UnSelect()
    {
        render.color = Color.white;
    }


    private void OnMouseDown()
    {
        if (selected != null)
        {
            selected.UnSelect();
            if (Vector3.Distance(selected.transform.position, transform.position) == 1)
            {
                SwapAndCheckMatch(selected, this, SwapFirstMOveCompleted);
                selected = null;
                return;
            }
        }
        selected = this;
        Select();
    }

    Candy swap_a;
    Candy swap_b;
    int swapMoveDone = 0;
    void SwapFirstMOveCompleted()
    {
        var matched = GridManager.I.CheckAllBoardMatch();
        if (matched.Count == 0)
        {
            SwapAndCheckMatch(swap_a, swap_b, null);
        }
        else
        {
            GridManager.I.DestroyCandyGO(matched);
            GridManager.I.FillBlank();
        }
    }

    // TODO: ������ �˰�����
    public void MoveToBlank(int row, int col)
    {
        moveDone = false;
        transform.DOMove(GridManager.I.GridIndexToPos(row, col), moveSpeed)
            .OnComplete(MoveToBlankDone);

        this.row = row;
        this.column = col;

        if (GridManager.I.Grid[row, column] == this.gameObject)
            GridManager.I.Grid[row, column] = null;
        GridManager.I.Grid[row, column] = this.gameObject;
    }

    void MoveToBlankDone()
    {
        moveDone = true;
    }

    void SwapAndCheckMatch(Candy a, Candy b, TweenCallback cb)
    {
        swap_a = a;
        swap_b = b;

        Sequence seq = DOTween.Sequence();
        seq.Append(a.transform.DOMove(b.transform.position, moveSpeed))
            .Join(b.transform.DOMove(a.transform.position, moveSpeed))
            .OnComplete(cb);

        int t_row = a.row;
        int t_col = a.column;
        a.GetComponent<Candy>().SetRowColumn(b.row, b.column);
        b.GetComponent<Candy>().SetRowColumn(t_row, t_col);
        GridManager.I.Grid[a.row, a.column] = a.gameObject;
        GridManager.I.Grid[b.row, b.column] = b.gameObject;
    }


}