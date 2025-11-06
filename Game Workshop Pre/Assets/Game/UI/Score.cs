using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour // Make singleton****
{
    [Header("Score and Grade")]
    public TextMeshProUGUI scoreText; // Ref to Score UI
    public static int score = 0;
    public int activePointLimit; // Maximum possible points for the active level
    private char grade; // Grade for the active level

    private List<Room> rooms;

    private void Start()
    {
        // Find all Rooms in scene and add them to rooms list
        rooms = new List<Room>(FindObjectsOfType<Room>());
    }

    void Update()
    {
        AssignGrade();
        scoreText.text = $"Score: {score}";

        foreach (Room room in rooms)
        {
            if (room.Cleanliness == 1)
            {
                rooms.Remove(room);
                UpdateScore(10);
            }
        }
    }

    public void UpdateScore(int amount)
    {
        score += amount;
    }

    private void AssignGrade()
    {
        if (score >= 1 * activePointLimit) {
            grade = 'S';    
        }
        else if (score >= .80 * activePointLimit)
        {
            grade = 'A';
        }
        else if (score >= .60 * activePointLimit)
        {
            grade = 'B';
        }
        else if (score >= .40 * activePointLimit)
        {
            grade = 'C';
        }
        else if (score >= .20 * activePointLimit)
        {
            grade = 'D';
        } 
        else
        {
            grade = 'F';
        }
    }
}
