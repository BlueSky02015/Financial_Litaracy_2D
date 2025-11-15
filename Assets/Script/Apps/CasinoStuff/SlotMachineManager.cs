using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SlotMachineManager : MonoBehaviour
{
    [Header("UI - 3x3 Grid")]
    public TMP_Text[,] reelTexts = new TMP_Text[3, 3];

    [Header("Win UI")]
    public TMP_Text resultText;
    public TMP_Text betText;

    [Header("Game Settings")]
    public int minBet = 10;
    public int maxBet = 100;
    public float spinDuration = 1.5f;
    public float interval = 0.1f;

    private int currentBet = 10;
    private bool isSpinning = false;


    public TMP_Text col0Row0, col0Row1, col0Row2;
    public TMP_Text col1Row0, col1Row1, col1Row2;
    public TMP_Text col2Row0, col2Row1, col2Row2;

    void Start()
    {
        // Map inspector references to 2D array
        reelTexts[0, 0] = col0Row0;
        reelTexts[0, 1] = col0Row1;
        reelTexts[0, 2] = col0Row2;
        reelTexts[1, 0] = col1Row0;
        reelTexts[1, 1] = col1Row1;
        reelTexts[1, 2] = col1Row2;
        reelTexts[2, 0] = col2Row0;
        reelTexts[2, 1] = col2Row1;
        reelTexts[2, 2] = col2Row2;

        currentBet = minBet;
        UpdateBetText();
        ResetReels();
    }

    public void SpinReels()
    {
        if (isSpinning) return;
        if (PlayerStats.instance.GetStat(StatType.Money) < currentBet)
        {
            resultText.text = "<color=red>Not enough money!</color>";
            return;
        }

        PlayerStats.instance.ModifyStat(StatType.Money, -currentBet);
        isSpinning = true;
        resultText.text = "Spinning...";

        // Spin each column with slight delay
        StartCoroutine(SpinColumn(0));
        StartCoroutine(SpinColumn(1, 0.2f));
        StartCoroutine(SpinColumn(2, 0.4f));
    }

    IEnumerator SpinColumn(int col, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        int steps = Mathf.FloorToInt(spinDuration / interval);
        int finalNumber = 0;

        // Animate: show random numbers
        for (int i = 0; i < steps; i++)
        {
            int randomNumber = Random.Range(1, 8);
            PushDownColumn(col, randomNumber);
            yield return new WaitForSeconds(interval);
        }

        // Final number (for win check)
        finalNumber = Random.Range(1, 8);
        PushDownColumn(col, finalNumber);

        // When all columns done, check win
        if (col == 2) // last column
        {
            yield return new WaitForSeconds(0.5f);
            CheckWin();
            isSpinning = false;
        }
    }

    // Push numbers down like a waterfall
    void PushDownColumn(int col, int newNumber)
    {
        // Move Row1 → Row2, Row0 → Row1, new → Row0
        reelTexts[col, 2].text = reelTexts[col, 1].text;
        reelTexts[col, 1].text = reelTexts[col, 0].text;
        reelTexts[col, 0].text = newNumber.ToString();
    }

    public List<WinCondition> winConditions = new List<WinCondition>
    {
        new WinCondition { name = "Jackpot", requiredCount = 3, targetNumber = 7, payoutMultiplier = 50, resultMessage = "JACKPOT!" },
        new WinCondition { name = "Triple", requiredCount = 3, targetNumber = -1, payoutMultiplier = 10, resultMessage = "Triple!" },
        new WinCondition { name = "Two 7s", requiredCount = 2, targetNumber = 7, payoutMultiplier = 5, resultMessage = "Two 7s!" },
        new WinCondition { name = "Pair", requiredCount = 2, targetNumber = -1, payoutMultiplier = 2, resultMessage = "Pair!" }
    };

    void CheckWin()
    {
        // Only use MIDDLE ROW for win detection
        int[] middleRow = {
            int.Parse(reelTexts[0, 1].text),
            int.Parse(reelTexts[1, 1].text),
            int.Parse(reelTexts[2, 1].text)
        };

        foreach (var condition in winConditions)
        {
            if (condition.IsMatch(middleRow))
            {
                int winnings = currentBet * condition.payoutMultiplier;
                PlayerStats.instance.ModifyStat(StatType.Money, winnings);

                string colorHex = GetColorForMultiplier(condition.payoutMultiplier);
                resultText.text = $"<color=#{colorHex}>{condition.resultMessage} +${winnings}</color>";
                UpdateBetText();
                return;
            }
        }

        // No win
        resultText.text =  "<color=#F44336>No win. Try again!</color>";
        UpdateBetText();
    }

    void ResetReels()
    {
        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                reelTexts[col, row].text = "0";
            }
        }
    }

    string GetColorForMultiplier(int mult)
    {
        if (mult >= 50) return "FFE500";      // Gold/Yellow
        if (mult >= 10) return "4CAF50";      // Green
        if (mult >= 5) return "2196F3";      // Blue
        return "FFFFFF";                      // White
    }

    // UI Controls
    public void IncreaseBet()
    {
        if (isSpinning) return;
        currentBet = Mathf.Min(currentBet + 10, maxBet);
        UpdateBetText();
    }

    public void DecreaseBet()
    {
        if (isSpinning) return;
        currentBet = Mathf.Max(currentBet - 10, minBet);
        UpdateBetText();
    }

    void UpdateBetText()
    {
        float cash = PlayerStats.instance.GetStat(StatType.Money);
        betText.text = $"{currentBet}";
    }
}