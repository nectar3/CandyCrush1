using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridManager : MonoBehaviour
{
    public static GridManager I { get; private set; }
    void Awake() { I = this; }

    public List<Sprite> Sprites = new List<Sprite>();
    public GameObject CandyPrefab;
    public Transform CandyParent;

    public int dimension = 9;
    public float Distance = 1.0f;

    public GameObject[,] Grid;

    private Vector3 posOffset = new Vector3(0.5f, 0.5f, 0);

    void Start()
    {

        Grid = new GameObject[dimension, dimension];

        InitGrid();
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


    public void FillBlank()
    {
        Debug.Log("fillBlank");
        for (int col = 0; col < dimension; col++)
        {
            var candies = GetColumnGO(col);
            Debug.Log(string.Format("col:{0}  objCount:{1}", col, candies.Count));
            var newCandy = MakeNewCandy(col, dimension - candies.Count);
            foreach (var item in newCandy)
                candies.Enqueue(item);

            for (int row = 0; row < dimension; row++)
            {
                var cand = candies.Dequeue();
                var candy = cand.GetComponent<Candy>();
                if (candy.row != row)
                    candy.MoveToBlank(row, col);
            }
        }
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
        Debug.Log("DestroyCandyGO: " + gos.Count);
        foreach (var go in gos)
        {
            Grid[go.GetComponent<Candy>().row, go.GetComponent<Candy>().column] = null;
            Destroy(go);
        }


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
