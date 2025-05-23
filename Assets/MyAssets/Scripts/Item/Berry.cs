using UnityEngine;

public class Berry : FieldItem
{
    protected override void OnPlayerCollect()
    {
        Debug.Log("ƒvƒŒƒCƒ„[‚ªÀ‚ğæ‚Á‚½");
    }
       

    protected override void OnDogCollect()
    {
        Debug.Log("Œ¢‚ªÀ‚ğæ‚Á‚½");
       
    }
}
