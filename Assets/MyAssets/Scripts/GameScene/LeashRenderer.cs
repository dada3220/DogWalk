using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LeashRenderer : MonoBehaviour
{
    public Transform player;
    public Transform dog;

    public float maxSlack = 3f;     // 距離が近いときのたるみ量
    public float slackFalloff = 2f; // たるみ→ピンの変化速度
    public float maxLeashLength = 6f; //リード最大長（追加）

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 20; // 頂点数（多いほど曲線が滑らか）
    }

    void Update()
    {
        if (player == null || dog == null) return;

        Vector3 start = player.position;
        Vector3 end = dog.position;

        // 最大長を超えていたら制限
        float distance = Vector3.Distance(start, end);
        if (distance > maxLeashLength)
        {
            Vector3 direction = (end - start).normalized;
            end = start + direction * maxLeashLength;
        }

        // パラボラ的にたるませる
        float slack = Mathf.Clamp(maxSlack - distance / slackFalloff, 0f, maxSlack);

        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)(line.positionCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            float arch = Mathf.Sin(Mathf.PI * t) * slack;
            point.y -= arch;
            line.SetPosition(i, point);
        }
    }
}
