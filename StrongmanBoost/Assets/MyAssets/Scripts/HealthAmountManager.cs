#if FB_INSTANT && UNITY_WEBGL
using PlayerPrefs = FacebookPlayerPrefs;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthAmountManager : MonoBehaviour
{
    public int health;
    public Text healthAmountText;
    public GameManager gameManager;
    public Player player;
    public ItemButtonsManager itemButtonsManager;
    public Vector3 cashSpawnPos;
    public List<GameObject> test;

    private void Start()
    {
        health = PlayerPrefs.GetInt("HealthAmount", 0);
        ShowHealthText();
    }

    public void GiveHealth(int val)
    {
        health += val;
        SaveHealth();
        ShowHealthText();
    }

    public void SellHealth()
    {
        FindObjectOfType<AdsManager>().ShowInterstitialAd();

        if (health > 0)
        {
            FindObjectOfType<CoinsManager>().AddCoins(cashSpawnPos, 5);

            AudioManager.Instance.Play("Sell");

            gameManager.AddCash(health);
            health = 0;
            SaveHealth();
            ShowHealthText();
            player.SetPlayerToInitialSize();
            player.SetPlayerSkinSize(100);
            player.ResetHandPose();

            if (itemButtonsManager.itemButtons.Count > 1)
                itemButtonsManager.CheckEnableBuyButton();
        }
    }

    private void SaveHealth()
    {
        PlayerPrefs.SetInt("HealthAmount", health);
    }

    private void ShowHealthText()
    {
        healthAmountText.text = health.ToString();
    }
}
