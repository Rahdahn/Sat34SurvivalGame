using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public int requiredItemsForAttack = 3; // 必要なアイテム数

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private int collectedItems = 0;
    private bool canAttack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // --- 横移動 ---
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if (move > 0) spriteRenderer.flipX = false;
        else if (move < 0) spriteRenderer.flipX = true;

        // --- ジャンプ ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
            isGrounded = false;
        }

        // --- 攻撃 ---
        if (canAttack && Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Attack");
            canAttack = false; // 1回使ったらリセット（1回だけ攻撃できる）
        }

        // --- アニメーション状態 ---
        if (isGrounded)
        {
            animator.SetBool("Run", Mathf.Abs(move) > 0.01f);
            animator.SetBool("Idle", Mathf.Abs(move) < 0.01f);
        }
        else
        {
            animator.SetBool("Run", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Fall", rb.velocity.y < -0.1f);
        }
    }

    // 地面判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.ResetTrigger("Jump");
            animator.SetBool("Fall", false);
        }
    }

    // --- アイテム取得処理 ---
    public void AddItem()
    {
        collectedItems++;
        Debug.Log("Collected Items: " + collectedItems);

        if (collectedItems >= requiredItemsForAttack)
        {
            canAttack = true;
            collectedItems = 0; // 使ったらリセットするならここで0にする
            Debug.Log("Attack Unlocked!");
        }
    }
}
