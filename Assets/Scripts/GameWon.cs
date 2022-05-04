using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWon : MonoBehaviour
{
    public GameObject winPopUp;
    public Text clockText;
    private void Start()
    {
        winPopUp.SetActive(false);
        clockText.text = Clock.instance.GetCurrentTimeText().text;
    }
    private void OnBoardCompleted()
    {
        winPopUp.SetActive(true);
        clockText.text = Clock.instance.GetCurrentTimeText().text;
    }
    private void OnEnable()
    {
        GameEvents.OnBoardCompleted += OnBoardCompleted;
    }
    private void OnDisable()
    {
        GameEvents.OnBoardCompleted -= OnBoardCompleted;
    }
}
