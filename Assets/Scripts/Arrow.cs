using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float arrowSpeed = 20f;
    [SerializeField] float penetrateDepth = 0.1f; // Độ lún sâu vào tường (bạn tự chỉnh cho vừa mắt)

    Rigidbody2D myRigidbody;
    BoxCollider2D myCollider;
    PlayerMovement player;
    float xSpeed;

    bool isStuck = false; // Biến trạng thái cực kỳ quan trọng

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        
        player = FindAnyObjectByType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * arrowSpeed;
    }

    void Update()
    {
        // Nếu đã cắm vào tường thì dừng mọi hoạt động bay và lật hình
        if (isStuck) return; 

        myRigidbody.velocity = new Vector2(xSpeed, 0f);
        FlipSprite();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isStuck) return; // Tránh việc va chạm chạy nhiều lần
        isStuck = true;      // Đánh dấu là đã cắm vào tường

        myRigidbody.velocity = Vector2.zero;
        myRigidbody.bodyType = RigidbodyType2D.Static;
        myCollider.enabled = false;

        // Đẩy mũi tên lún tới trước. Mathf.Sign(xSpeed) giúp biết là đang bắn qua trái hay phải
        transform.position += new Vector3(Mathf.Sign(xSpeed) * penetrateDepth, 0f, 0f);

        Destroy(gameObject, 3f);
    }

    void FlipSprite()
    {
        bool arrowHasHorizontalSpeed = Mathf.Abs(xSpeed) > Mathf.Epsilon;
        if (arrowHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
}