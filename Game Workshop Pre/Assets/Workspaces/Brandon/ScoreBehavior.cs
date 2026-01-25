using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening;

public class ScoreBehavior : MonoBehaviour
{

    [SerializeField] Image bonusBarImage;
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] TextMeshProUGUI bonusScoreText;
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] float bonusBarTimer;

    [SerializeField] int currentScore = 0;
    [SerializeField] int currentBonus = 0;
    [Header("Customize Combo Bar")]
    [SerializeField] Color deadComboColor;
    [SerializeField] Color hotComboColor;
    [SerializeField] Color coldComboColor;
    [SerializeField] float smoothBarSlide;
    [SerializeField] float textSizeChange;
    [SerializeField] float delayBeforeTickingDown;

    private readonly string BST = "+";
    private readonly string CST = "Score: ";
    private readonly float DEFAULT_BONUS_BAR_TIMER = 15f;

    private float timeLeft;

    private delegate void ResetBonusEvent();
    private event ResetBonusEvent resetBonus;
    DataTable bonusTable = new DataTable();
    int thisBonusScore;

    // Start is called before the first frame update
    void Start()
    {
        if (bonusBarTimer == 0)
        {
            bonusBarTimer = DEFAULT_BONUS_BAR_TIMER;
        }
        currentScoreText.text = CST + currentScore;
        timeLeft = 0;

        resetBonus += ResetBonus;
        Trash.SendScore += IncreaseScore;
        BaseEnemy.SendScore += IncreaseScore;
        TrashBall.SendScore += IncreaseScore;
        PlayerMovementController.playerDeath = LoseBonusScore;

        bonusTable.Columns.Add("Threshold", typeof(int));
        bonusTable.Columns.Add("Bonus Amount", typeof(int));
        thisBonusScore = 0;

        object[,] data =
        {
            {0, 0 },
            {10, 5 },
            {20, 10 },
            {30, 15 },
            {50, 25 },
            {75, 35 },
            {100, 50 },
            {125, 60 },
            {150, 75 }
        };
        
        for (int i = 0; i < data.GetLength(0); i++)
        {
            bonusTable.Rows.Add(data[i, 0], data[i, 1]);
        }
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        float fillAmount = Math.Clamp(timeLeft / (bonusBarTimer - delayBeforeTickingDown), 0, 1);
        bonusBarImage.fillAmount = Mathf.Lerp(bonusBarImage.fillAmount, fillAmount, Time.deltaTime * smoothBarSlide);
        comboText.characterSpacing = bonusBarImage.fillAmount * 16;
        bonusBarImage.color = Color.Lerp(coldComboColor, hotComboColor, bonusBarImage.fillAmount);
        comboText.color = Color.Lerp(deadComboColor, Color.white, fillAmount * 8);
        
        if (timeLeft <= 0)
        {
            if (currentBonus > 0)
            {
                currentScore += currentBonus;
                currentScoreText.characterSpacing += textSizeChange;
                DOTween.To(() => currentScoreText.characterSpacing, x => currentScoreText.characterSpacing = x, 0, 0.6f);
            }
            currentBonus = 0;
            thisBonusScore = 0;
            UpdateUI();
        }
    }

    private void ResetBonus()
    {
        timeLeft = bonusBarTimer;
    }
    
    private void LoseBonusScore(bool x)
    {
        timeLeft = 0;
        currentBonus = 0;
    }

    private void IncreaseScore(int score)
    {
        currentScore += score;
        thisBonusScore += score;
        resetBonus?.Invoke();
        CheckBonus(thisBonusScore);
        UpdateUI();
    }

    private void UpdateUI()
    {
        currentScoreText.text = CST + currentScore;
        if (currentBonus == 0) bonusScoreText.text = "";
        else if (bonusScoreText.text != BST + currentBonus)
        {
            bonusScoreText.text = BST + currentBonus;
            bonusScoreText.characterSpacing += textSizeChange * 3;
            DOTween.To(() => bonusScoreText.characterSpacing, x => bonusScoreText.characterSpacing = x, 0, 0.6f);
        }
    }

    private void CheckBonus(int score)
    {
        if (bonusTable != null)
        {
            foreach (DataRow row in bonusTable.Rows)
            {
                if (score < (int)row["Threshold"])
                {
                    break;
                }
                else
                {
                    currentBonus = (int)row["Bonus Amount"];
                }
            }
        }

    }
    
    
}
