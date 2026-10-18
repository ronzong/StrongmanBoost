using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public float sizeIncreaseVal;
    [HideInInspector] public int healthIncreaseVal;
    public float smoothTime;
    private Vector3 currentVelocity;
    private Vector3 targetScale;

    public Animator anim;
    public Transform scaleTransform;
    public HealthAmountManager healthAmountManager;
    public ItemButtonsManager ItemButtonsManager;

    private const string scaleKeyX = "PlayerScaleX";
    private const string scaleKeyY = "PlayerScaleY";
    private const string scaleKeyZ = "PlayerScaleZ";

    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Transform playerHandPose;
    private Vector3 handPosInitialValue;

    private void Start()
    {
        ItemBtn itemBtn = ItemButtonsManager.itemButtons[0];

        sizeIncreaseVal = itemBtn.sizeIncreaseVal;
        healthIncreaseVal = itemBtn.healthIncreaseAmount;

        LoadPlayerScale();

        float skinSize = PlayerPrefs.GetFloat("SkinSize", skinnedMeshRenderer.GetBlendShapeWeight(0));
        SetPlayerSkinSize(skinSize);

        handPosInitialValue = playerHandPose.localPosition;

        float handPosX = PlayerPrefs.GetFloat("HandPosX", playerHandPose.localPosition.x);
        float handPosZ = PlayerPrefs.GetFloat("HandPosZ", playerHandPose.localPosition.z);

        Vector3 handPosition = playerHandPose.localPosition;

        handPosition.x = handPosX;
        handPosition.z = handPosZ;

        playerHandPose.localPosition = handPosition;
    }

    private void Update()
    {
        Vector3 rot = transform.localEulerAngles;
        rot.y = 0;
        transform.localEulerAngles = rot;

        transform.localPosition = Vector3.zero;
    }

    public void PlayIncreaseHealthAnim()
    {
        anim.SetTrigger("Bicep");
    }

    public void IncreaseHealth()
    {
        print("Increase Health");

        if(skinnedMeshRenderer.GetBlendShapeWeight(0) > 0)
            SetPlayerSkinSize(skinnedMeshRenderer.GetBlendShapeWeight(0) - 1);

        ChangeHandPose();

        SetPlayerSize(sizeIncreaseVal);

        SavePlayerScale();

        healthAmountManager.GiveHealth(healthIncreaseVal);
    }

    private void ChangeHandPose()
    {
        Vector3 handPosition = playerHandPose.localPosition;
        handPosition.x += .01f;
        handPosition.z -= .01f;
        playerHandPose.localPosition = handPosition;

        PlayerPrefs.SetFloat("HandPosX", handPosition.x);
        PlayerPrefs.SetFloat("HandPosZ", handPosition.z);
    }

    public void ResetHandPose()
    {
        playerHandPose.localPosition = handPosInitialValue;
        Vector3 handPosition = playerHandPose.localPosition;

        PlayerPrefs.SetFloat("HandPos", handPosition.x);
    }

    public void SetPlayerSkinSize(float val)
    {
        skinnedMeshRenderer.SetBlendShapeWeight(0, val);
        PlayerPrefs.SetFloat("SkinSize", skinnedMeshRenderer.GetBlendShapeWeight(0));
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

    public void SetPlayerToInitialSize()
    {
        targetScale = scaleTransform.localScale;

        targetScale.x = 1;
        targetScale.y = 1;
        targetScale.z = 1;

        StartCoroutine(SmoothScaleCoroutine());
    }

    private void SavePlayerScale()
    {
        PlayerPrefs.SetFloat(scaleKeyX, scaleTransform.localScale.x);
        PlayerPrefs.SetFloat(scaleKeyY, scaleTransform.localScale.y);
        PlayerPrefs.SetFloat(scaleKeyZ, scaleTransform.localScale.z);
    }

    private void LoadPlayerScale()
    {
        if (PlayerPrefs.HasKey(scaleKeyX) && PlayerPrefs.HasKey(scaleKeyY) && PlayerPrefs.HasKey(scaleKeyZ))
        {
            float scaleX = PlayerPrefs.GetFloat(scaleKeyX);
            float scaleY = PlayerPrefs.GetFloat(scaleKeyY);
            float scaleZ = PlayerPrefs.GetFloat(scaleKeyZ);

            Vector3 loadedScale = new Vector3(scaleX, scaleY, scaleZ);
            scaleTransform.localScale = loadedScale;
        }
    }
}
