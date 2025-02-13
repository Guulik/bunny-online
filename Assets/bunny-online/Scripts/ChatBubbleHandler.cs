using System;
using UnityEngine;

public class ChatBubbleHandler : MonoBehaviour
{
    
    public static ChatBubbleHandler BubbleInstance;

    private void Awake()
    {
        BubbleInstance = this;        
        ShowUp += ChatBubble.CreateBubbleInstance;
    }

    public event EventHandler<OnShowUpEventArgs> ShowUp;
    public class OnShowUpEventArgs : EventArgs
    {
        public Transform parent;
        public string text;
    }
    
    public void OnShowUp(Transform parent, string text)
    {
        ShowUp?.Invoke(this, new OnShowUpEventArgs{parent = parent, text = text});
    }
}