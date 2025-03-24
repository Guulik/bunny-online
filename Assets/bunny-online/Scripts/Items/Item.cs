using UnityEngine;

namespace Items
{
    [CreateAssetMenu (fileName = "ScriptableItem", menuName = "ScriptableObjects/Item")]
    public class Item : ScriptableObject
    {
        public int ID;
        public string itemName;
        public Sprite sprite;
        public GameObject prefab; 
        public int count = 0;

        public Item() {}
        public Item(string name, Sprite sprite)
        {
            this.sprite = sprite;
            itemName = name;
        }
        //for debug
        public virtual string GetProperties()
        {
            return itemName+": ";
        }

        public Pass ToPass()
        { return (Pass)this; }
    }
    
    
    
    [System.Serializable]
    public struct ItemData
    {
        public string itemName; // Название предмета
        public string spriteName; // Имя спрайта (или путь к ресурсу)
        public string prefabName; // Имя префаба (или путь к ресурсу)
        public int count; // Количество предметов
    }
}
