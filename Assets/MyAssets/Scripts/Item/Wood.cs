using UnityEngine;

public class Wood : FieldItem
{
    protected override void OnPlayerCollect()
    {
        DogController dog = FindFirstObjectByType<DogController>(); // シーン上のDogControllerを検索

        if (dog != null)
        {
            dog.affection += 6; // 好感度6上げる
        }
    }


    protected override void OnDogCollect()
    {
        Debug.Log("犬が枝を取った");

    }
}
