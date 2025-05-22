using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;
    private Vector2 lastMoveDir = Vector2.down;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        transform.Translate(movement * speed * Time.deltaTime);

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

            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDir.x);
            animator.SetFloat("LastMoveY", lastMoveDir.y);
        }
    }
}
