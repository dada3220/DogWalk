using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    // 引っ張り関連
    public Transform dog;
    public float pullSpeed = 2f;
    public float leashMaxLength = 5f;

    private DogController dogController;

    void Start()
    {
        if (dog != null)
        {
            dogController = dog.GetComponent<DogController>();
        }
    }

    void Update()
    {
        HandleMovement();
        HandleLeashTension();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveX, moveY);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    void HandleLeashTension()
    {
        if (dog == null || dogController == null) return;

        float distance = Vector2.Distance(transform.position, dog.position);
        Vector2 toPlayer = (Vector2)transform.position - (Vector2)dog.position;
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (distance > leashMaxLength)
        {
            float dot = Vector2.Dot(toPlayer.normalized, movementInput.normalized);

            if (dot > 0.5f)
            {
                // プレイヤーが離れる方向に動いているとき → 犬を引っ張る
                dog.position += (Vector3)(toPlayer.normalized * pullSpeed * Time.deltaTime);
                dogController.SetPulledState(true);
            }
            else
            {
                // プレイヤーが近づいている or 停止 → 踏ん張り解除
                dogController.SetPulledState(false);
            }
        }
        else
        {
            // 距離が近いときは踏ん張り状態を解除
            dogController.SetPulledState(false);
        }
    }
}
