using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG : MonoBehaviour
{
    public float bouncebackSpeed = 10f;

    Vector3 clickStartMousepos = Vector3.zero;
    Vector3 clickStartpos;
    Vector3 startPos;

    float bounce_margin = 15f; // 스크롤 금지 구역

    Vector3 bgSize;

    void Start()
    {
        startPos = transform.position;
        clickStartpos = transform.position;
        bgSize = GetComponent<Renderer>().bounds.size;
    }

    //TODO: 바운스 백 로직
    // TODO: 경계에서만 바운스 백 하도록.

    bool isGrabbed = false;

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            clickStartMousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickStartpos = transform.position;
            isGrabbed = true;
        }
        else if (Input.GetMouseButton(0))
        {
            var cur = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var ydiff = cur.y - clickStartMousepos.y;

            var pos = clickStartpos + new Vector3(0, ydiff, 0);
            float y = Mathf.Clamp(pos.y, -startPos.y - bounce_margin, startPos.y + bounce_margin);
            pos.y = y;
            transform.position = pos;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isGrabbed = false;
        }

        if (isGrabbed == false)
        {
            if (transform.position.y > startPos.y)
            {
                MoveToWantYPosUpdate(startPos.y);
            }
            else if (transform.position.y < -startPos.y)
            {
                MoveToWantYPosUpdate(-startPos.y);
            }
        }
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }



    void MoveToWantYPosUpdate(float want)
    {
        var y = Mathf.Lerp(transform.position.y, want, bouncebackSpeed * Time.deltaTime);
        if (Mathf.Abs(y - want) < 0.01f)
            y = want;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
