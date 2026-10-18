using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBtn : MonoBehaviour
{
    public string itemName;
    private ItemButtonsManager itemButtonsManager;
    private Player player;
    public float sizeIncreaseVal;
    private float clickBtnTime = 0.3f;
    public int priceAmount, healthIncreaseAmount;
    public Button buyBtn, clickBtn;
    public Text healthIncAmountText, priceText, itemNameText;
    public Image imageToFill;
    public GameObject prefabItem;

    private void Start()
    {
        itemNameText.text = itemName;
        player = FindObjectOfType<Player>();
        itemButtonsManager = FindObjectOfType<ItemButtonsManager>();

        healthIncAmountText.text = healthIncreaseAmount.ToString();
        priceText.text = priceAmount.ToString();
    }

    public void StartAutoClick()
    {
        AudioManager.Instance.Play("Click");

        InvokeRepeating("IncreaseHealthBtnClick", 0, 1);
        clickBtn.enabled = false;
    }

    public void StopAutoClick()
    {
        AudioManager.Instance.Play("Click");

        CancelInvoke("IncreaseHealthBtnClick");
        clickBtn.enabled = true;
    }

    public void IncreaseHealthBtnClick()
    {
        AudioManager.Instance.Play("LiftWeight");

        clickBtn.interactable = false;

        StartCoroutine(FillImageOverTime());
        player.PlayIncreaseHealthAnim();

        Invoke("EnableClickBtn", clickBtnTime);
    }

    private void EnableClickBtn()
    {
        clickBtn.interactable = true;
    }

    private IEnumerator FillImageOverTime()
    {
        float elapsedTime = 0;

        while (elapsedTime < clickBtnTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / clickBtnTime;
            imageToFill.fillAmount = Mathf.Lerp(0, 1, normalizedTime);
            yield return null;
        }

        imageToFill.fillAmount = 0;
    }

    public void Buy()
    {
        FindObjectOfType<AdsManager>().ShowInterstitialAd();

        FindObjectOfType<GameManager>().buyParticle.Play();
        AudioManager.Instance.Play("Buy");

        FindObjectOfType<GameManager>().LessCash(priceAmount);

        Setup();
    }

    private void Setup()
    {
        player.sizeIncreaseVal = sizeIncreaseVal;
        player.healthIncreaseVal = healthIncreaseAmount;

        Destroy(buyBtn.gameObject);
        itemButtonsManager.ArrangeButtonsList();
    }
}
