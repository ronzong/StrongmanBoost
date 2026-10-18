using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator anim;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Transform scaleTransform;
    private Vector3 targetScale;
    private float smoothTime = 0.2f;
    private Vector3 currentVelocity;

    void Start()
    {
        InvokeRepeating("PlayRandom", 4, 4);
    }

    private void PlayRandom()
    {
        int num = UnityEngine.Random.Range(2, 5);
        Invoke("PlayIncreaseHealthAnim", num);
    }

    public void PlayIncreaseHealthAnim()
    {
        anim.SetTrigger("Bicep");
        IncreaseHealth();
    }

    public void IncreaseHealth()
    {
        print("Increase Health");

        if (skinnedMeshRenderer.GetBlendShapeWeight(0) > 0)
            SetPlayerSkinSize(skinnedMeshRenderer.GetBlendShapeWeight(0) - 1);


        float num = UnityEngine.Random.Range(0.01f, 0.1f);

        SetPlayerSize(num);
    }

    public void SetPlayerSkinSize(float val)
    {
        skinnedMeshRenderer.SetBlendShapeWeight(0, val);
    }

    private void SetPlayerSize(float val)
    {
        targetScale = scaleTransform.localScale;

        targetScale.x += val;
        targetScale.y += val;
        targetScale.z += val;

        StartCoroutine(SmoothScaleCoroutine());
    }

    private IEnumerator SmoothScaleCoroutine()
    {
        // Smoothly interpolate the current local scale towards the targetScale.
        while (Vector3.Distance(scaleTransform.localScale, targetScale) > 0.01f)
        {
            scaleTransform.localScale = Vector3.SmoothDamp(scaleTransform.localScale, targetScale, ref currentVelocity, smoothTime);
            yield return null;
        }

        // Ensure the scale reaches the exact target value to avoid errors.
        scaleTransform.localScale = targetScale;
    }
}
