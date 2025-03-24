using System.Collections.Generic;

namespace Items
{
    public static class ItemManager
    {
        private static Dictionary<int, Item> _items = new Dictionary<int, Item>();

        public static void RegisterItem(Item item)
        {
            _items.TryAdd(item.ID, item);
        }

        public static Item GetItemByID(int id)
        {
            if (_items.TryGetValue(id, out Item item))
            {
                return item;
            }
            return null;
        }
    }
}