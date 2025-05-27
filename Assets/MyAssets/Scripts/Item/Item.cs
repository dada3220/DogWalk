using UnityEngine;

// �A�C�e���̊�{�N���X�B�v���C���[�⌢�ƐڐG���̓�����`���邽�߂̒��ۃN���X�B

public abstract class Item : MonoBehaviour
{
    protected Transform player;
    protected Transform dog;
    protected Camera mainCamera;

    protected float boundaryMargin = 3f; // ��ʂ�2�g�O�ɏo���玩���I�ɏ��������

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player")?.transform;
        dog = GameObject.FindWithTag("Dog")?.transform;

    }

    protected virtual void Update()
    {
        // ��ʊO�i2�g�ȏ�O�j�ɏo����I�u�W�F�N�g��j��
        if (IsOutOfBounds())
        {
            Destroy(gameObject);
        }
    }

    // �J�����̕\���͈͂��2�g���O���ɏo�����ǂ������`�F�b�N
    protected bool IsOutOfBounds()
    {
        Vector3 camPos = mainCamera.transform.position;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        float minX = camPos.x - width / 2 - boundaryMargin;
        float maxX = camPos.x + width / 2 + boundaryMargin;
        float minY = camPos.y - height / 2 - boundaryMargin;
        float maxY = camPos.y + height / 2 + boundaryMargin;

        Vector3 pos = transform.position;

        return (pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY);
    }

    // �v���C���[�����̃A�C�e�����擾�������̏����i�p���N���X�Ŏ����j
    protected abstract void OnPlayerCollect();

    // �������̃A�C�e�����擾�������̏����i�p���N���X�Ŏ����j
    protected abstract void OnDogCollect();

    // ���̃I�u�W�F�N�g�Ƃ̃g���K�[�ڐG����
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerCollect();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Dog") && other.GetComponent<DogController>() != null)
        {
            OnDogCollect();
            Destroy(gameObject);
        }
    }
}
