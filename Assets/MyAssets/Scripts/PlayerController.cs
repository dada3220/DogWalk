using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed = 4f; // 通常のプレイヤー移動速度
    private float currentSpeed; // 実際の移動速度（引っ張り時に変更される）

    private Animator animator; // アニメーション制御用
    private Vector2 lastMoveDir = Vector2.down; // 最後の移動方向

    public DogController dog; // 犬のスクリプト参照（インスペクターで割り当て）

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        // 犬が引っ張られている場合、プレイヤーの速度を1fに制限
        if (dog != null && dog.IsPulled())
        {
            currentSpeed = 1f;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        // 入力取得
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // 移動
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        if (animator != null)
        {
            if (movement.magnitude > 0.01f)
            {
                lastMoveDir = movement;

                if (movement.x > 0)
                    animator.Play("Player_r");
                else if (movement.x < 0)
                    animator.Play("Player_l");
                else if (movement.y < 0)
                    animator.Play("Player_s");
                else if (movement.y > 0)
                    animator.Play("Player_b");
            }
            else
            {
                if (lastMoveDir.x > 0)
                    animator.Play("Player_rF");
                else if (lastMoveDir.x < 0)
                    animator.Play("Player_lF");
                else if (lastMoveDir.y < 0)
                    animator.Play("Player_sF");
                else if (lastMoveDir.y > 0)
                    animator.Play("Player_bF");
            }

            // アニメーター更新
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDir.x);
            animator.SetFloat("LastMoveY", lastMoveDir.y);
        }
    }
}
