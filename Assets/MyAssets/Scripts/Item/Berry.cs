using UnityEngine;

public class Berry : FieldItem
{
    protected override void OnPlayerCollect()
    {
        Debug.Log("�v���C���[�����������");
    }
       

    protected override void OnDogCollect()
    {
        Debug.Log("�������������");
       
    }
}
