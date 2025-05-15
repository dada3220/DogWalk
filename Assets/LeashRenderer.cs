using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LeashRenderer : MonoBehaviour
{
    public Transform player;
    public Transform dog;

    public float maxSlack = 1f;     // �������߂��Ƃ��̂���ݗ�
    public float slackFalloff = 3f; // ����݁��s���̕ω����x

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 20; // ���_���i�����قǋȐ������炩�j
    }

    void Update()
    {
        if (player == null || dog == null) return;

        Vector3 start = player.position;
        Vector3 end = dog.position;

        float distance = Vector3.Distance(start, end);
        float slack = Mathf.Clamp(maxSlack - distance / slackFalloff, 0f, maxSlack);

        // ���������炩�ȃA�[�`�i�p���{���Ȑ����j�ɂ���
        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)(line.positionCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // �p���{�����ۂ�Y���ɂ���ݒǉ�
            float arch = Mathf.Sin(Mathf.PI * t) * slack;
            point.y -= arch;

            line.SetPosition(i, point);
        }
    }
}
