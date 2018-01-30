using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Static instance
    public static GameManager Instance = null;

    void Awake()
    {
        Instance = this;
        this.scoreText = this.ScoreTextTransform.GetComponent<Text>();
        this.score = 0;
    }

    void OnApplicationQuit()
    {
        Instance = null;
    }
    #endregion

    private int score;

    [SerializeField]
    private Transform ScoreTextTransform;

    private Text scoreText;

    private void Update()
    {
        this.scoreText.text = "Score: " + this.score.ToString().PadLeft(5, '0');
    }

    public void ModifyScore(int value)
    {
        this.score += value;
    }
}