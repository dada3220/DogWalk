using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // 追いかける対象（プレイヤー）
    public Vector2 followOffset = new Vector2(2f, 1f); // プレイヤーからのオフセット
    public float followSpeed = 2f;   // 追従スピード

    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (target == null) return;

        // プレイヤー位置にオフセットを加えた位置をターゲットに
        targetPosition = new Vector3(
            target.position.x + followOffset.x,
            target.position.y + followOffset.y,
            transform.position.z // カメラのZ位置は固定
        );

        // 現在の位置からターゲット位置へスムーズに移動
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
