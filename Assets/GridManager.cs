using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public delegate void NormalCallBack();


public class GridManager : MonoBehaviour
{
    public bool allMoveDone = true;

    public static GridManager I { get; private set; }
    void Awake() { I = this; }

    public List<Sprite> Sprites = new List<Sprite>();
    public GameObject CandyPrefab;
    public Transform CandyParent;
    public GameObject DoneEffectPref;

    public TextMeshProUGUI text_score;

    public int dimension = 9;
    public float Distance = 1.0f;

    public GameObject[,] Grid;

    private Vector3 posOffset = new Vector3(0.5f, 0.5f, 0);

    private int score = 0;

    void Start()
    {

        Grid = new GameObject[dimension, dimension];

        InitGrid();

        text_score.SetText("score: " + score);

    }
    public Vector2Int PosToGridIndex(Vector3 pos)
    {
        return new Vector2Int((int)(pos.x - posOffset.x), (int)(pos.y - posOffset.y));
    }

    public Vector3 GridIndexToPos(int row, int col)
    {
        return new Vector3(col + posOffset.x, row + posOffset.y, 0);

    }


    void InitGrid()
    {

        for (int column = 0; column < dimension; column++)
        {
            for (int row = 0; row < dimension; row++)
            {
                var candy = Instantiate(CandyPrefab, new Vector3(column, row, 0) + posOffset, Quaternion.identity);
                candy.transform.SetParent(CandyParent);
                Candy c = candy.GetComponent<Candy>();
                c.SetRowColumn(row, column);
                Grid[row, column] = candy;
            }
        }
        StartCoroutine(WaitAndCheck());
    }

    IEnumerator WaitAndCheck()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(CheckAndRemoveAndFill());
    }
    public bool CheckCandyGridConnection()
    {
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                if (Grid[row, col].GetComponent<Candy>().row != row ||
                    Grid[row, col].GetComponent<Candy>().column != col)
                    return false;
            }
        }
        return true;
    }
    public bool CheckAllCandyMoveDone()
    {
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                if (Grid[row, col].GetComponent<Candy>().moveDone == false)
                    return false;
            }
        }
        return true;
    }


    public HashSet<GameObject> CheckAllBoardMatch()
    {
        var matched = new HashSet<GameObject>();
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                var cur = Grid[row, col];
                var spIndex = cur.GetComponent<Candy>().spriteIndex;
                var mat = GetHorizontalMatch(row, col, spIndex);
                if (mat.Count >= 2)
                {
                    matched.UnionWith(mat);
                    matched.Add(cur);
                }
                var vert = GetVerticalMatch(row, col, spIndex);
                if (vert.Count >= 2)
                {
                    matched.UnionWith(vert);
                    matched.Add(cur);
                }
            }
        }
        return matched;
    }

    // 코루틴을 외부에서 실행하면 그 object 파괴시 코루틴도 멈추기때문에 꼭 매니저에서 실행
    public void RunCheckAndRemoveAndFill()
    {
        StartCoroutine(CheckAndRemoveAndFill());
    }

    //TODO:  매칭확인 -> 삭제 -> 내리기 -> 매칭확인
    //TODO: 버그 
    IEnumerator CheckAndRemoveAndFill()
    {
        while (true)
        {
            var matched = CheckAllBoardMatch();
            Debug.Log("matched: " + matched.Count);
            if (matched.Count == 0)
                break;
            DestroyCandyGO(matched);
            FillBlank();

            yield return new WaitUntil(() => allMoveDone == true);
        }
    }


    public void FillBlank()
    {
        allMoveDone = false;

        for (int col = 0; col < dimension; col++)
        {
            var candies = GetColumnGO(col);
            var newCandy = MakeNewCandy(col, dimension - candies.Count);
            foreach (var item in newCandy)
                candies.Enqueue(item);

            for (int row = 0; row < dimension; row++)
            {
                var cand = candies.Dequeue();
                var candy = cand.GetComponent<Candy>();
                if (candy.row != row)
                    candy.MoveToBlank(row, col, FillBlankMoveCB);
            }
        }
    }

    void FillBlankMoveCB()
    {
        allMoveDone = isAllMoveDone();
    }


    bool isAllMoveDone()
    {
        for (int row = 0; row < dimension; row++)
        {
            for (int col = 0; col < dimension; col++)
            {
                if (Grid[row, col].GetComponent<Candy>().moveDone == false)
                    return false;
            }
        }
        return true;
    }



    Queue<GameObject> MakeNewCandy(int col, int num)
    {
        Queue<GameObject> res = new Queue<GameObject>();
        for (int i = 0; i < num; i++)
        {
            var candy = Instantiate(CandyPrefab, new Vector3(col, dimension + i, 0) + posOffset, Quaternion.identity);
            candy.transform.SetParent(CandyParent);
            res.Enqueue(candy);
        }
        return res;
    }


    Queue<GameObject> GetColumnGO(int col)
    {
        Queue<GameObject> objs = new Queue<GameObject>();
        for (int row = 0; row < dimension; row++)
        {
            if (Grid[row, col] != null)
                objs.Enqueue(Grid[row, col]);
        }
        return objs;
    }

    public void DestroyCandyGO(HashSet<GameObject> gos)
    {
        var count = gos.Count;
        foreach (var go in gos)
        {
            var eff = Instantiate(DoneEffectPref, go.transform.position, Quaternion.identity);
            eff.GetComponent<doneEffectParent>().spr = go.GetComponent<SpriteRenderer>().sprite;
            Grid[go.GetComponent<Candy>().row, go.GetComponent<Candy>().column] = null;
            Destroy(go);
        }
        score += count * 1000;
        text_score.SetText("score: " + score);
    }


    List<GameObject> GetHorizontalMatch(int row, int col, int spriteIndex)
    {
        List<GameObject> matched = new List<GameObject>();
        for (int i = col + 1; i < dimension; i++)
        {
            if (Grid[row, i].GetComponent<Candy>().spriteIndex == spriteIndex)
                matched.Add(Grid[row, i]);
            else
                break;
        }
        return matched;
    }

    List<GameObject> GetVerticalMatch(int row, int col, int spriteIndex)
    {
        List<GameObject> matched = new List<GameObject>();
        for (int i = row + 1; i < dimension; i++)
        {
            if (Grid[i, col].GetComponent<Candy>().spriteIndex == spriteIndex)
                matched.Add(Grid[i, col]);
            else
                break;
        }
        return matched;
    }



}
