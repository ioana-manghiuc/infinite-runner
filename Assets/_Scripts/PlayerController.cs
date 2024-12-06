using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController),typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _initialPlayerSpeed = 4f;
    [SerializeField]
    private float _maxiumPlayerSpeed = 30f;
    [SerializeField]
    private float _playerSpeedIncreaseRate = .1f;
    [SerializeField]
    private float _jumpHeight = 1f;
    [SerializeField]
    private float _initialGravityValue = -9.8f;
    [SerializeField]
    private LayerMask _groundLayer;

    private float _playerSpeed;
    private float _gravity;
    private Vector3 _movementDirection = Vector3.forward;

    private PlayerInput _playerInput;
    private InputAction _turnAction;
    private InputAction _jumpAction;
    private InputAction _slideAction;

    private CharacterController _controller;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _turnAction = _playerInput.actions["Turn"];
        _jumpAction = _playerInput.actions["Jump"];
        _slideAction = _playerInput.actions["Slide"];
    }
    private void Start()
    {
        _playerSpeed = _initialPlayerSpeed;
        _gravity = _initialGravityValue;
    }

    private void OnEnable()
    {
        _turnAction.performed += PlayerTurn;
        _jumpAction.performed += PlayerJump;
        _slideAction.performed += PlayerSlide;
    }
    private void OnDisable()
    {
        _turnAction.performed -= PlayerTurn;
        _jumpAction.performed -= PlayerJump;
        _slideAction.performed -= PlayerSlide;
    }

    private void PlayerTurn(InputAction.CallbackContext context)
    {
    }
    private void PlayerSlide(InputAction.CallbackContext context)
    {
    }
    private void PlayerJump(InputAction.CallbackContext context)
    {
    }

    private void Update()
    {
        _controller.Move(transform.forward * _playerSpeed * Time.deltaTime);
    }

    private bool IsGrounded(float length)
    {
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= _controller.height / 2f;
        raycastOriginFirst.y += 0.1f;

        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst -= transform.forward * .2f;
        raycastOriginSecond += transform.forward * .2f;

        // Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green, 2f);
        // Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red, 2f);

        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, _groundLayer)
           || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, _groundLayer))
            return true;

        return false;


    }


}
