using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{   
    private UIManager m_uiManager => UIManager.singleton;
    
    public float moveSpeed;
    public float jumpForce;

    private PlayerCollision m_playerCollision;
    private Rigidbody m_rb;
    private Vector2 m_moveValue;    

    void Start()
    {
        m_playerCollision = GetComponent<PlayerCollision>();
        m_rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameManager.IsGamePaused) return;

        transform.Translate(new Vector3(m_moveValue.x, 0, m_moveValue.y) * moveSpeed * Time.deltaTime);
    }

    void OnMove(InputValue value)
    {
        m_moveValue = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (GameManager.IsGamePaused) return;

        if (m_playerCollision.IsGrounded) 
        {
            m_rb.AddForce(transform.up * jumpForce);
            m_uiManager.notificationManager.SendNotif(" is jumping");
        }
    }

    void OnShoot(InputValue value)
    {
        if (GameManager.IsGamePaused) return;

        m_uiManager.notificationManager.SendNotif(" shoots !");
    }
}