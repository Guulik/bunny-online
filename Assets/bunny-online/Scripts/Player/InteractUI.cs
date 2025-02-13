using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject interactUI;
    
    public void Show()
    {
        interactUI.SetActive(true);
    }

    public void Hide()
    {
        interactUI.SetActive(false);
    }
}
