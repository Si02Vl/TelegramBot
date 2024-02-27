namespace TelegramBot;

public class ShoppingList
{
    public int Id {get; set;}
    public string Product {get; set;}
    public bool IsBought {get; set;}
    public long ChatId {get; set;}
}