using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ScoreBehavior : MonoBehaviour
{

    [SerializeField] Image bonusBarImage;
    [SerializeField] TextMeshProUGUI bonusScoreText;
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] float bonusBarTimer;
    [SerializeField] int currentScore = 0;
    [SerializeField] int currentBonus = 0;
    [SerializeField] Color hotComboColor;
    [SerializeField] Color coldComboColor;

    private readonly string BST = "+";
    private readonly string CST = "Current Score: ";
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
        bonusScoreText.text = BST + currentBonus;
        timeLeft = 0;

        resetBonus += ResetBonus;
        LooseTrash.SendScore += IncreaseScore;
        CollectableTrash.SendScore += IncreaseScore;
        StainTrash.SendScore += IncreaseScore;
        BaseEnemy.SendScore += IncreaseScore;
        TrashPile.SendScore += IncreaseScore;
        TrashBall.SendScore += IncreaseScore;

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
        

    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        float fillAmount = Math.Clamp(timeLeft / (bonusBarTimer - 1), 0, 1);
        bonusBarImage.fillAmount = fillAmount;
        bonusBarImage.color = Color.Lerp(coldComboColor, hotComboColor, fillAmount);

        if (timeLeft <= 0)
        {
            currentScore += currentBonus;
            currentBonus = 0;
            thisBonusScore = 0;
            UpdateUI();
        }

    }

    private void ResetBonus()
    {
        timeLeft = bonusBarTimer;
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
        bonusScoreText.text = BST + currentBonus;
    }

    private void CheckBonus(int score)
    {
        if (bonusTable != null)
            foreach (DataRow row in bonusTable.Rows)
            {
                if (score < (int)row["Threshold"]){
                    break;
                }
                else
                {
                    currentBonus = (int)row["Bonus Amount"];
                }
            }
    }
}
