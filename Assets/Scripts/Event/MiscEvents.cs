using System;

public class MiscEvents
{
    public event Action onItemCollected;
    public void ItemCollected()
    {
        if (onItemCollected != null)
        {
            onItemCollected();
        }
    }

    public event Action onItemInteracted;
    public void ItemInteracted()
    {
        if(onItemInteracted != null)
        {
            onItemInteracted();
        }
    }
}