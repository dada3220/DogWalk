using UnityEngine;

public abstract class FieldItem : MonoBehaviour
{
    protected Transform player;
    protected Transform dog;
    protected Camera mainCamera;

    protected float boundaryMargin = 3f; // 2�g�O�ɏo�������
    protected float spawnMargin = 1f; // 1�g�O�ŏo��

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player")?.transform;
        dog = GameObject.FindWithTag("Dog")?.transform;

        // �����_���ʒu�ɃX�|�[��
        transform.position = GetRandomSpawnPosition();
    }

    protected virtual void Update()
    {
        // ��ʊO����F2�g�O�ɏo����j��
        if (IsOutOfBounds())
        {
            Destroy(gameObject);
        }
    }

    /// �����_����1�g�O�ɏo������ʒu��Ԃ�
    protected Vector3 GetRandomSpawnPosition()
    {
        Vector3 camPos = mainCamera.transform.position;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        // �O���̂ǂ�����I��
        int side = Random.Range(0, 4);
        float x = 0f, y = 0f;

        switch (side)
        {
            case 0: // ��
                x = Random.Range(-width / 2, width / 2);
                y = height / 2 + spawnMargin;
                break;
            case 1: // ��
                x = Random.Range(-width / 2, width / 2);
                y = -height / 2 - spawnMargin;
                break;
            case 2: // ��
                x = -width / 2 - spawnMargin;
                y = Random.Range(-height / 2, height / 2);
                break;
            case 3: // �E
                x = width / 2 + spawnMargin;
                y = Random.Range(-height / 2, height / 2);
                break;
        }

        return new Vector3(camPos.x + x, camPos.y + y, 0f);
    }

    // ��ʂ�2�g�O�ɏo�Ă��邩����
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

    // �v���C���[�ƐڐG�����Ƃ��̓���i�p����Œ�`�j
    protected abstract void OnPlayerCollect();

    // ���ƐڐG�����Ƃ��̓���i�p����Œ�`�j
    protected abstract void OnDogCollect();

    // �g���K�[�ڐG����
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerCollect();
            Destroy(gameObject);
        }
        else if (other.GetComponent<DogController>() != null)
        {
            OnDogCollect();
            Destroy(gameObject);
        }
    }
}
