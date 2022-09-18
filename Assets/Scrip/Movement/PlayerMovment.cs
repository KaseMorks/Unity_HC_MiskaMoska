using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [Header("�������ʰʧ@")] //���ʱ���B�t��
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("�������ʰʧ@")] //���D����B�j��
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("�ե�")] //�Ե{���ݨϥβե�
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;
    public GameObject characterHolder;

    [Header("���z�S��")] //���z�S��
    [Tooltip("����̰��t��")]
    public float maxSpeed = 7f;
    [Tooltip("�t�״�q")]
    public float linearDrag = 4f;
    [Tooltip("���O")]
    public float gravity = 1f;
    [Tooltip("�Y���[�t��")]
    public float fallMultiplier = 5f;

    [Header("�I��")] //Ĳ�a����
    public bool onGround = false;
    public Vector3 colliderOffset;
    [SerializeField] Vector2 AreaShape;
 
    void Update()
    {
        bool wasOnGround = onGround;
        onGround = Physics2D.BoxCast(transform.position - colliderOffset, new Vector3(AreaShape.x, AreaShape.y,1),0,Vector2.zero,0, groundLayer);

        if (!wasOnGround && onGround)
        {
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }
        animator.SetBool("onGround", onGround);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    void FixedUpdate()
    {
        moveCharacter(direction.x);
        if (jumpTimer > Time.time && onGround)
        {
            Jump();
        }

        modifyPhysics();
    }
    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("jumpSpeed", rb.velocity.y);
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        
    }
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }
    /// <summary>
    /// �i�樤�⭱�¤�V���]�w�B�H�����D�ۦa�p�ܧ�
    /// </summary>
    void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    /// <summary>
    /// ø�s�i���ư����u
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position - colliderOffset, new Vector3(AreaShape.x,AreaShape.y, 1));
    }
}