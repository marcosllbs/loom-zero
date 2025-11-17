
public class CardModel 
{
    public int Id{ get; set; }
    public bool IsRevealed { get; set; }
    public bool IsMatched{ get; set; }

    public CardModel(int id)
    {
        Id = id;
        IsMatched = false;
        IsRevealed = false;
    }
}
