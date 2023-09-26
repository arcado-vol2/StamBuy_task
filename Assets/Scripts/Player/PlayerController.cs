using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private PlayerInput playerInput;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 10.0f;
    
    private Rigidbody2D rb;
    private Vector2 input;
    [SerializeField] private SpriteRenderer baseSprite;
    public SpriteRenderer coloredPart;
    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main;

        Vector3 minWorldBounds = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxWorldBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        Vector2 playerSize = baseSprite.bounds.size / 2.0f;

        minBounds = new Vector2(minWorldBounds.x + playerSize.x, minWorldBounds.y + playerSize.y);
        maxBounds = new Vector2(maxWorldBounds.x - playerSize.x, maxWorldBounds.y - playerSize.y);
    }

    void Update()
    {
        input = playerInput.actions["Move"].ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Передвигаем игрока
        transform.position += (Vector3)input * moveSpeed * Time.deltaTime;
        if (input != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        }
    }

    private void LateUpdate()
    {
        Vector2 clampedPosition = new Vector2(Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                                             Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y));
        transform.position = clampedPosition;
    }
}
