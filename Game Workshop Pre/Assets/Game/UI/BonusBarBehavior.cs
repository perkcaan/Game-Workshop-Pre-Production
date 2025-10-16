<<<<<<< Updated upstream
using System;
=======
>>>>>>> Stashed changes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;




public class BonusBarBehavior : MonoBehaviour
{

    [Tooltip("Max Time Limit for Bonus Bar (In Seconds)")]
    [SerializeField] float bonusTimeLimit;

    [SerializeField] Image visualBonusBar;
    [SerializeField] TextMeshProUGUI bonusText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] int score;
    [SerializeField] int currentBonus;

    //Default bonus time limit in case time limit is set to 0
    private readonly float DEFAULT_TIME = 10f;
    private bool barDepreciating;
    private readonly string BONUS_PRETEXT = "Bonus: +";
    private readonly string CURRENT_SCORE_PRETEXT = "Score: ";

<<<<<<< Updated upstream
    private event Action ResetBonusTimeEvent;
    private event Action ResetBonusEvent;
=======
    public delegate void IncreaseScoreEvent(int score);
>>>>>>> Stashed changes
    

    // Start is called before the first frame update
    void Start()
    {
        barDepreciating = true;
        if (bonusTimeLimit <= 0)
            bonusTimeLimit = DEFAULT_TIME;

        visualBonusBar.fillAmount = 1f;
<<<<<<< Updated upstream

        BaseEnemy.ScoreEvent += IncreaseScore;
        ResetBonusTimeEvent += ResetTimer;
        ResetBonusEvent += ResetBonus;

=======
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        if (barDepreciating)
        {
            visualBonusBar.fillAmount -= ( Time.deltaTime / bonusTimeLimit);
<<<<<<< Updated upstream
            Debug.Log(visualBonusBar.fillAmount);
        }

        if (visualBonusBar.fillAmount <= 0f)
        {
            Debug.Log("Hit");
            ResetBonusEvent.Invoke();
=======
        }
        else if (visualBonusBar.fillAmount <= 0f)
        {
>>>>>>> Stashed changes
            visualBonusBar.fillAmount = 0f;
            barDepreciating = false;
        }

        bonusText.text = BONUS_PRETEXT + currentBonus;
        scoreText.text = CURRENT_SCORE_PRETEXT + score; 

    }

    private void IncreaseScore(int score)
    {
        this.score += score;
<<<<<<< Updated upstream
        ResetBonusTimeEvent?.Invoke();
    }

    private void ResetTimer()
    {
        visualBonusBar.fillAmount = 1f;
=======
        ResetBonus();
>>>>>>> Stashed changes
    }

    private void ResetBonus()
    {
<<<<<<< Updated upstream
        this.currentBonus = 0;
=======
        visualBonusBar.fillAmount = 1f;
>>>>>>> Stashed changes
    }

}
