using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LeashRenderer : MonoBehaviour
{
    public Transform player;
    public Transform dog;

    public float maxSlack = 1f;     // 距離が近いときのたるみ量
    public float slackFalloff = 3f; // たるみ→ピンの変化速度

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

        float distance = Vector3.Distance(start, end);
        float slack = Mathf.Clamp(maxSlack - distance / slackFalloff, 0f, maxSlack);

        // 直線を滑らかなアーチ（パラボラ曲線風）にする
        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)(line.positionCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // パラボラっぽくY軸にたるみ追加
            float arch = Mathf.Sin(Mathf.PI * t) * slack;
            point.y -= arch;

            line.SetPosition(i, point);
        }
    }
}
