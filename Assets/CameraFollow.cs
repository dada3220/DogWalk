using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // �ǂ�������Ώہi�v���C���[�j
    public Vector2 followOffset = new Vector2(2f, 1f); // �v���C���[����̃I�t�Z�b�g
    public float followSpeed = 2f;   // �Ǐ]�X�s�[�h

    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (target == null) return;

        // �v���C���[�ʒu�ɃI�t�Z�b�g���������ʒu���^�[�Q�b�g��
        targetPosition = new Vector3(
            target.position.x + followOffset.x,
            target.position.y + followOffset.y,
            transform.position.z // �J������Z�ʒu�͌Œ�
        );

        // ���݂̈ʒu����^�[�Q�b�g�ʒu�փX���[�Y�Ɉړ�
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
