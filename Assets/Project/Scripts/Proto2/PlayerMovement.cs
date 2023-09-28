using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _playerInput;
    private Vector3 _velocity, _desiredVelocityX, _desiredVelocityY, _desiredVelocity;
    [SerializeField] private float speed;
    [SerializeField] private float maxAcceleration, maxAirAcceleration;
    [SerializeField, Range(0f, 90f)] float maxGroundAngle;
    private Rigidbody _body;
    private bool _onGround;
    float _minGroundDotProduct;
    [SerializeField] private float _rotationSpeed;
    private Vector3 _forward;
    private Vector3 _right;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        
        _forward = Camera.main.transform.forward;
        _forward.y = 0;
        _forward.Normalize();
        _right = Quaternion.Euler(new Vector3(0,90, 0)) * _forward;
        OnValidate();
    }

    private void Update()
    {
        _playerInput.x = Input.GetAxis("Horizontal");
        _playerInput.y = Input.GetAxis("Vertical");
            
        _playerInput = Vector2.ClampMagnitude(_playerInput, 1f);
        var acceleration = new Vector3(_playerInput.x, 0f, _playerInput.y) * speed ;
        _desiredVelocityX = _playerInput.x *speed  * _right;
        _desiredVelocityY = _playerInput.y * speed * _forward;
        _desiredVelocity = _desiredVelocityX + _desiredVelocityY;

        Look();

    }

    private void FixedUpdate()
    {
        _velocity = _body.velocity;
        float acceleration = _onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z, maxSpeedChange);
        _body.velocity = _velocity;
        _onGround = false;
    }
    
    void OnCollisionEnter (Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay (Collision collision)
    {
        EvaluateCollision(collision);
    }
    void OnValidate () {
        _minGroundDotProduct = Mathf.Cos(maxGroundAngle* Mathf.Deg2Rad);
    }
    void EvaluateCollision (Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal; 
            _onGround |= normal.y >= _minGroundDotProduct;
        }
    }
    
    void Look()
    {
        if (_playerInput != Vector2.zero)
        {

            var localTarget = transform.InverseTransformPoint(transform.position + _desiredVelocity);
    
            var angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

            var eulerAngleVelocity  = new Vector3 (0, angle, 0) * _rotationSpeed;
            var deltaRotation  = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime );
            _body.MoveRotation(_body.rotation * deltaRotation);
            
            Debug.Log("hola");
        }
    }
    
}
