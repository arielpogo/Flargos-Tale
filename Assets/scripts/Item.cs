public class Item {
    public static Item[] GameItem = new Item[] {
        new("test item", false, true),
        new("Tyler's Hat", false, true)
    };

    public string Name { get; private set; } = "DEFAULT_ITEM";
    public bool Stackable { get; private set; } = true;
    public bool KeyItem { get; private set; } = false;  //key item aka deleteable?

    public Item(string name, bool stackable, bool keyItem) {
        Name = name;
        Stackable = stackable;
        KeyItem = keyItem;
    }
}
