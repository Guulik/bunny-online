using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestingDialogue : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DialogueQuestion.Instance.ShowQuestion("внимание загадка", () => { Debug.Log("Правильно"); },
                () => { Debug.Log("неправильно"); });
        }
    }
}