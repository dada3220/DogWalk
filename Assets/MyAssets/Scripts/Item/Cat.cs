using UnityEngine;

public class Cat : FieldItem
{
    protected override void OnPlayerCollect()
    {
        Debug.Log("�v���C���[���L�������");
    }


    protected override void OnDogCollect()
    {
        Debug.Log("�����L�������");

    }
}
