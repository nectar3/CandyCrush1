using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;


public class DoneEffectParticle : MonoBehaviour
{
    public Sprite sp;

    float fadeSpeed;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        fadeSpeed = transform.parent.GetComponent<doneEffectParent>().fadeSpeed;
        sp = transform.parent.GetComponent<doneEffectParent>().spr;
        sr.sprite = sp;

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-60, 60));

        float firstMoveTime = 0.3f;

        Sequence s = DOTween.Sequence();
        var pos = transform.position + (Vector3)Random.insideUnitCircle * Random.Range(0.2f, 0.5f);
        s.Append(transform.DOMove(pos, firstMoveTime));
        s.Join(transform.DORotate(new Vector3(0, 0, Random.Range(-15f, 15f)), firstMoveTime))
            .OnComplete(StartFade);

    }


    void StartFade()
    {
        StartCoroutine(DoFade());
    }

    float alpha = 1.0f;
    IEnumerator DoFade()
    {
        while (true)
        {
            alpha -= fadeSpeed;
            sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b,
                alpha);
            if (alpha <= 0)
                break;
            yield return null;
        }
        Destroy(gameObject);
        if (transform.parent != null)
            Destroy(transform.parent.gameObject);
    }

}
