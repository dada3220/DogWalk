using UnityEngine;

public class Wood : FieldItem
{
    protected override void OnPlayerCollect()
    {
        DogController dog = FindFirstObjectByType<DogController>(); // �V�[�����DogController������

        if (dog != null)
        {
            dog.affection += 6; // �D���x6�グ��
        }
    }


    protected override void OnDogCollect()
    {
        Debug.Log("�����}�������");

    }
}
