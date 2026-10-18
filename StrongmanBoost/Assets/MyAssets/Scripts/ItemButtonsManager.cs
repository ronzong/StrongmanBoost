#if FB_INSTANT && UNITY_WEBGL
using PlayerPrefs = FacebookPlayerPrefs;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonsManager : MonoBehaviour
{
    public Transform itemButtonsParent;
    [HideInInspector] public List<ItemBtn> itemButtons;
    private int buttonsCount;
    public GameManager gameManager;
    public Transform playerHandPose;
    private GameObject spawnedItem;

    private void Start()
    {
        for(int i=0; i<itemButtonsParent.childCount;i++)
        {
            ItemBtn btn = itemButtonsParent.GetChild(i).GetComponent<ItemBtn>();
            itemButtons.Add(btn);
        }

        buttonsCount = PlayerPrefs.GetInt("ButtonsCount", 0);

        for (int i = 0; i < buttonsCount; i++)
        {
            Destroy(itemButtons[0].gameObject);
            itemButtons.RemoveAt(0);
        }

        SetBtnColor(0);

        itemButtons[0].clickBtn.interactable = true;
        Destroy(itemButtons[0].buyBtn.gameObject);

        if (itemButtons.Count > 1)
            CheckEnableBuyButton();

        SpawnNewItem();
    }


    public void ArrangeButtonsList()
    {
        Destroy(itemButtons[0].gameObject);
        itemButtons.RemoveAt(0);

        itemButtons[0].clickBtn.interactable = true;
        SetBtnColor(0);

        string val = PlayerPrefs.GetString("IsAutoClickOn", "False");

        if (val == "True")
            itemButtons[0].StartAutoClick();

        if (itemButtons.Count > 1)
            CheckEnableBuyButton();

        if (itemButtons.Count >= 1)
        {
            buttonsCount++;
            PlayerPrefs.SetInt("ButtonsCount", buttonsCount);
        }

        SpawnNewItem();
    }

    private void SpawnNewItem()
    {
        if (spawnedItem != null)
            Destroy(spawnedItem);

        spawnedItem =  Instantiate(itemButtons[0].prefabItem, playerHandPose.position, playerHandPose.rotation, playerHandPose);
    }

    public void CheckEnableBuyButton()
    {
        if(!itemButtons[1].buyBtn.gameObject.activeSelf)
            itemButtons[1].buyBtn.gameObject.SetActive(true);

        if (gameManager.cashCount >= itemButtons[1].priceAmount)
        {
            itemButtons[1].buyBtn.interactable = true;
        }
    }
    private void SetBtnColor(int index)
    {
        itemButtons[index].clickBtn.GetComponent<Image>().color = Color.green;
    }
}
