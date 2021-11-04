using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private GameplayInputActions gameplayInputActions;
        private InputAction movement;
        private InputAction look;
        
        [Header("Movement")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float jumpHeight = 1f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.1f;
        [SerializeField] private LayerMask groundLayerMask;
        private Vector3 velocity;
        private bool isGrounded;
        
        [Header("Look")]
        [SerializeField] private Transform playerCam;
        [SerializeField] private float mouseSensitivity = 1f;
        private float xRotation;
        private void Awake()
        {
            gameplayInputActions = new GameplayInputActions();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            movement = gameplayInputActions.Player.Movement;
            movement.Enable();

            look = gameplayInputActions.Player.Look;
            look.Enable();
            
            gameplayInputActions.Player.Jump.performed += Jump;
            gameplayInputActions.Player.Jump.Enable();

            gameplayInputActions.Player.Attack.performed += Attack;
            gameplayInputActions.Player.Attack.Enable();
            
            gameplayInputActions.Player.Interact.performed += Interact;
            gameplayInputActions.Player.Interact.Enable();
        }

        private void OnDisable()
        {
            movement.Disable();
            gameplayInputActions.Player.Jump.Disable();
            gameplayInputActions.Player.Attack.Disable();
            gameplayInputActions.Player.Interact.Disable();
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayerMask);
            
            Look();
            Movement();
            Gravity();
            
            characterController.Move(velocity * Time.deltaTime);
        }

        private void Movement()
        {
            var m = movement.ReadValue<Vector2>();
            var t = transform;
            var move = t.right * m.x + t.forward * m.y;

            characterController.Move(move * (movementSpeed * Time.deltaTime));
        }

        private void Gravity()
        {
            if (isGrounded)
            {
                if (velocity.y < 0)
                {
                    velocity.y = -0.5f;
                }
                return;
            }
            
            velocity.y += gravity * Time.deltaTime;
        }
        
        private void Look()
        {
            var mouseMovement = look.ReadValue<Vector2>();
            var mouseX = mouseMovement.x * mouseSensitivity * Time.deltaTime;
            var mouseY = mouseMovement.y * mouseSensitivity * Time.deltaTime;
            
            transform.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);
            playerCam.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
        private void Jump(InputAction.CallbackContext ctx)
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void Attack(InputAction.CallbackContext ctx)
        {
            Debug.Log("attack");
        }
        private void Interact(InputAction.CallbackContext ctx)
        {
            Debug.Log("interact");
        }
    }
}
