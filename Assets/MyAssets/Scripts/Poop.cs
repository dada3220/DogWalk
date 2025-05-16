using UnityEngine;

public class Poop : MonoBehaviour
{
    public float lifeTime = 10f;
    private float timer = 0f;
    private bool touchedByPlayer = false;

    void Update()
    {
        timer += Time.deltaTime;

        // 画面外チェック（より確実な方法）
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        bool isOutOfScreen = viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1;

        if (!touchedByPlayer && (isOutOfScreen || timer >= lifeTime))
        {
            DogController dog = FindObjectOfType<DogController>();
            if (dog != null)
            {
                dog.DecreaseAffection(10);
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            touchedByPlayer = true;
            Destroy(gameObject);
        }
    }
}
