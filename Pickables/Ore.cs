public class Ore : PickableItem
{
    public override void ObjectPlaced()
    {
        Destroy(gameObject);
        LevelManager.Instance.PlayerInteractions.ObjectPlaced();
    }
}
