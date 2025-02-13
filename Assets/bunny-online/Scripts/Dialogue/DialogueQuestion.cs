using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueQuestion : MonoBehaviour
{
    public static DialogueQuestion Instance { get; private set; } 
        
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _correctButton;
    [SerializeField] private Button _inCorrectButton;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void ShowQuestion(string questionText, Action actionCorrect, Action actoinIncorrect)
    {
        gameObject.SetActive(true);
        
        _text.text = questionText;
        _correctButton.onClick.AddListener(() => {
            Hide();
            actionCorrect();
        });
        _inCorrectButton.onClick.AddListener(() =>
        {
            Hide();
            actoinIncorrect();
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
