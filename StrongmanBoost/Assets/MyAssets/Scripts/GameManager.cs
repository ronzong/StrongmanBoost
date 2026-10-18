#if FB_INSTANT && UNITY_WEBGL
using PlayerPrefs = FacebookPlayerPrefs;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public int cashCount;
    public Text cashAmountText;
    public ParticleSystem buyParticle;

    void Awake()
    {
        Application.targetFrameRate = 60;

        cashCount = PlayerPrefs.GetInt("Cash", 0);
        cashAmountText.text = cashCount.ToString();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        if (Input.GetKeyDown(KeyCode.C))
            AddCash(5000);
#endif
    }

    public void AddCash(int amount)
    {
        print("Added");
        cashCount += amount;

        UpdateCashText();
    }

    public void LessCash(int amount)
    {
        cashCount -= amount;
        UpdateCashText();
    }

    private void UpdateCashText()
    {
        cashAmountText.text = cashCount.ToString();
        PlayerPrefs.SetInt("Cash", cashCount);
    }
  
    public void Reload()
    {
        AudioManager.Instance.Play("Click");

        FindObjectOfType<AdsManager>().ShowInterstitialAd();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SoundToggle()
    {
        AudioManager.Instance.Play("Click");
        AudioManager.Instance.SoundToggle();
    }

    public void ShowRewardVideo()
    {
        AudioManager.Instance.Play("Click");

        FindObjectOfType<AdsManager>().ShowRewardedAd();
    }
}
