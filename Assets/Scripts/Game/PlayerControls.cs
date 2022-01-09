using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour
{
    private const float GRAVITY = 9.81f;

    public bool InputEnabled { 
        get => _inputEnabled;
        set {
            _inputEnabled = value;
            if (!value)
            {
                _movement = Vector2.zero;
                _lookLocal = Vector2.zero;
                _lookWorld = Vector2.zero;
            }
        }
    }

    public float PlayerSpeed { get => _playerSpeed; }

    [Range(0.5f, 2.5f)]
    [SerializeField]
    private float _playerSpeed = 1.0f;
    [SerializeField]
    private Vector2 _cameraSensivity = 50.0f * Vector2.one;
    
    private Camera _camera;
    private CharacterController _controller;

    private Vector3 _velocity = Vector3.zero;
    private Vector2 _movement = Vector3.zero;
    private Vector3 _lookWorld = Vector3.zero;
    private Vector3 _lookLocal = Vector3.zero;

    private bool _inputEnabled = true;
    private float _cameraAngleX = 0.0f;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Player movement
        Vector3 gravity = Vector3.down * GRAVITY;
        Vector3 acceleration = gravity;
        _velocity += acceleration * Time.deltaTime;

        Vector3 movementInput = _movement.x * transform.right + _movement.y * transform.forward;
        Vector3 movement = _velocity + movementInput;

        _controller.Move(movement * Time.deltaTime);

        if (_controller.isGrounded)
            _velocity.y = 0.0f;

        // Camera orientation
        transform.Rotate(_lookWorld * Time.deltaTime);

        _cameraAngleX += _lookLocal.x * Time.deltaTime;
        _cameraAngleX = Mathf.Clamp(_cameraAngleX, -90.0f, 90.0f);
        _camera.transform.localRotation = Quaternion.Euler(_cameraAngleX, 0.0f, 0.0f);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!InputEnabled) return;
        Vector2 input = context.ReadValue<Vector2>();
        _movement = _playerSpeed * input.normalized;
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (!InputEnabled) return;
        Vector2 input = context.ReadValue<Vector2>();
        _lookWorld = new Vector3(0.0f, input.x * _cameraSensivity.y, 0.0f);
        _lookLocal = new Vector3(-input.y * _cameraSensivity.x, 0.0f, 0.0f);
    }
}
