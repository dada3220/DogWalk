using UnityEngine;
using Cysharp.Threading.Tasks;

public class Flower : MonoBehaviour
{
    public float digDuration = 3f;                 // �@�鎞��

    private DogController dog;

    void Start()
    {
        dog = Object.FindFirstObjectByType<DogController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dog"))
        {
            dog = other.GetComponent<DogController>();
            if (dog != null)
            {
                ForceRunAsync().Forget();
            }
        }
        else if (other.CompareTag("Player"))
        {
            // �v���C���[���G�ꂽ�疳��
        }
    }

    private async UniTask ForceRunAsync()
    {
        // ����AI�X�N���v�g
        Animator dogAnimator = dog.GetComponent<Animator>();
        var dogAI = dog.GetComponent<MonoBehaviour>(); 

        // �@��A�j���[�V�����Đ�
        if (dogAnimator != null)
        {
            dogAnimator.Play("dogdig");
        }

        // AI ���ꎞ��~
        if (dogAI != null)
        {
            dogAI.enabled = false;
        }

        float timer = 0f;

        while (timer < digDuration)
        {
            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        // AI �ĊJ
        if (dogAI != null)
        {
            dogAI.enabled = true;
        }

        // �A�j���[�V������߂�
        if (dogAnimator != null)
        {
            dogAnimator.Play("walk_front");
        }

        Destroy(gameObject); // �@��I������������
    }
}
