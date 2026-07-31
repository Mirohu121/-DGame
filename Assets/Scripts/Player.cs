using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Playerアクション")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;

    private bool isGrounded;
    private Vector2 inputVector;
    private Rigidbody rb;
    private Animator anim;

    [SerializeField] private Transform mainCameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 自分自身、または子オブジェクトから Animator を自動取得
        anim = GetComponentInChildren<Animator>();

        if (mainCameraTransform == null && Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        Vector3 moveDir = Vector3.zero;

        // 1. カメラの向きを基準にした移動方向の計算
        if (mainCameraTransform != null)
        {
            Vector3 camForward = mainCameraTransform.forward;
            Vector3 camRight = mainCameraTransform.right;

            // Y軸（上下方向）を無視して水平にする
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            moveDir = camForward * inputVector.y + camRight * inputVector.x;
        }

        bool isMoving = moveDir.magnitude > 0.1f;

        if (anim != null)
        {
            anim.SetBool("IsMoving", isMoving);
        }

        if (isMoving)
        {
            moveDir.Normalize();

            // 2. 移動処理（Rigidbodyの速度を直接書き換える）
            Vector3 targetVelocity = moveDir * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

            // 3. 回転処理（物理回転と干渉しないよう、Transformの回転を直接スムーズ補間）
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 15f);
        }
        else
        {
            // 停止時は水平方向の速度を0にする
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            transform.DOScale(new Vector3(0.8f, 1.2f, 0.8f), 0.15f)
                     .OnComplete(() => transform.DOScale(Vector3.one, 0.15f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded)
        {
            isGrounded = true;
            transform.DOPunchScale(new Vector3(0.2f, -0.2f, 0.2f), 0.2f);
        }
    }
}