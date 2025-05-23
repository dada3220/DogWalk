using UnityEngine;

public class Cat : FieldItem
{
    protected override void OnPlayerCollect()
    {
        Debug.Log("ƒvƒŒƒCƒ„[‚ª”L‚ğæ‚Á‚½");
    }


    protected override void OnDogCollect()
    {
        Debug.Log("Œ¢‚ª”L‚ğæ‚Á‚½");

    }
}
