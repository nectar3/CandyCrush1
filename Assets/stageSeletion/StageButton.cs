using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageButton : MonoBehaviour
{

    public int stage = 1;

    void Start()
    {

    }


    private void OnMouseUpAsButton()
    {
        Debug.Log("Stage " + stage);

    }


    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
    //    {
    //        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

    //        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
    //        if (hit.collider != null)
    //        {
    //            Debug.Log(hit.collider.gameObject.name);
    //        }
    //    }
    //}



}
