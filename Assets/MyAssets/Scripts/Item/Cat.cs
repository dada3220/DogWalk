using UnityEngine;

public class Cat : FieldItem
{
    protected override void OnPlayerCollect()
    {
        Debug.Log("プレイヤーが猫を取った");
    }


    protected override void OnDogCollect()
    {
        Debug.Log("犬が猫を取った");

    }
}
