using System;
using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    private RectTransform background;
    private TextMeshProUGUI textMeshPro;
    private static readonly Vector2 defaultOffset = new Vector2(1f,1f);

    
    private void Awake()
    {
        background = transform.Find("Background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }
    
    public static void CreateBubbleInstance(object sender, ChatBubbleHandler.OnShowUpEventArgs eventArgs)
    {
        if (eventArgs.parent.GetComponentInChildren<ChatBubble>() != null) return;
        
        Transform chatBubble = Instantiate(GameAssets.asset.ChatBubble, eventArgs.parent);
        chatBubble.localPosition = defaultOffset;
        chatBubble.GetComponent<ChatBubble>().Setup(eventArgs.text);

        Destroy(chatBubble.gameObject, 4f);
    }
    
    private void Setup(string textToSetup)
    {
        textMeshPro.SetText(textToSetup);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 margin = new Vector2(0.5f, 0.5f);
        background.sizeDelta = textSize + margin;
    }

 
    //на всякий случай
    public static void Create(Transform parent, Vector2 localPosition, string text)
    {
        Transform chatBubble = Instantiate(GameAssets.asset.ChatBubble, parent);
        chatBubble.localPosition = localPosition;
        chatBubble.GetComponent<ChatBubble>().Setup(text);
        
        Destroy(chatBubble.gameObject, 4f);
    }

    public static void Create(Transform parent, string text)
    {
        Transform chatBubble = Instantiate(GameAssets.asset.ChatBubble, parent);
        chatBubble.localPosition = defaultOffset;
        chatBubble.GetComponent<ChatBubble>().Setup(text);
        
        Destroy(chatBubble.gameObject, 4f);
    }
}