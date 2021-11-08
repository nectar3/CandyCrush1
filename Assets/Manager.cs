using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public void Test()
    {
        bool dd = GridManager.I.CheckCandyGridConnection();

        Debug.Log("CheckCandyGridConnection: " + dd);
    }

    public void Test2()
    {
        //bool dd = GridManager.I.CheckAllCandyMoveDone();
        //Debug.Log("CheckAllCandyMoveDone: " + dd);

        //Debug.Log(GridManager.I.CheckAndRemoveAndFill == null);
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{

        //    Vector3 mousePos = Input.mousePosition;
        //    var pos = Camera.main.ScreenToWorldPoint(mousePos);
        //    pos.z = 0;
        //    Debug.Log(pos);

        //}


    }
}
