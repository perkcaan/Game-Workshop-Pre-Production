using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class ScoreBehavior : MonoBehaviour
{

    [SerializeField] Image bonusBarImage;
    [SerializeField] TextMeshProUGUI bonusScoreText;
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] float bonusBarTimer;
    [SerializeField] int currentScore = 0;
    [SerializeField] int currentBonus = 0;

    //Defaults
                            //Bonus Start Text
    private readonly string BST = "+ ";
                            //Current Score Start Text
    private readonly string CST = "Current Score: ";
    private readonly float DEFAULT_BONUS_BAR_TIMER = 15f;

    private float timeLeft;

    private delegate void ResetBonusEvent();
    private event ResetBonusEvent resetBonus;

    // Start is called before the first frame update
    void Start()
    {
        if (bonusBarTimer == 0)
        {
            bonusBarTimer = DEFAULT_BONUS_BAR_TIMER;
        }
        currentScoreText.text = CST + currentScore;
        bonusScoreText.text = BST + currentBonus;
        timeLeft = bonusBarTimer;

        resetBonus += ResetBonus;
        LooseTrash.SendScore += IncreaseScore;
        CollectableTrash.SendScore += IncreaseScore;
        BaseEnemy.SendScore += IncreaseScore;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        bonusBarImage.fillAmount = timeLeft / bonusBarTimer;
    }

    private void ResetBonus()
    {
        timeLeft = bonusBarTimer;
    }
    
    private void IncreaseScore(int score)
    {
        currentScore += score;
        resetBonus?.Invoke();
        UpdateUI();
    }

    private void UpdateUI()
    {
        currentScoreText.text = CST + currentScore;
        bonusScoreText.text = BST + currentBonus;
    }

}
