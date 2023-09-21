using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    
 [Header("Movement")]   
 private Vector3 _input;
 private Vector3 _forward, _right;
 [SerializeField] private Rigidbody _rb;
 [SerializeField] private float _walkSpeed = 5;
 [SerializeField] private float _speed;
 [SerializeField] private float _rotationSpeed =720;
 private Vector3 _heading;
 private bool _gatherInput = true;
 private bool _dashing = false;
 private bool _canDash = true;
 [SerializeField] private float _dashSpeed =30f;
 [SerializeField] private float _dashDuration =0.5f;
 [SerializeField] private float _dashCooldown =1f;

 [Header("Attack")]
 [SerializeField] private float _attackCooldown =1;
 [SerializeField] private GameObject _attackTrigger;
 private bool canAttack = true;
 


 private void Start()
 {
     _forward = Camera.main.transform.forward;
     _forward.y = 0;
     _forward.Normalize();
     _right = Quaternion.Euler(new Vector3(0,90, 0)) * _forward;
     _speed = _walkSpeed;


 }

 private void Update()
 {
   GatherInput();
   //the attack should probably be done from another script.
   CheckAttack();
   CheckDash();

 }

 private void FixedUpdate()
 {
     Move();
     Look();
 }

 void GatherInput()
 {
     if (_gatherInput)
     {
         Debug.Log(_gatherInput);
         _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
         _input.Normalize();
     }
 }

 void CheckAttack()
 {
     if (Input.GetButtonDown("Fire1") && canAttack)
     {
         _attackTrigger.SetActive(true);
         canAttack = false;
         StartCoroutine(ResetAttackCooldown());

     }
 }
 void Look()
 {
     if (_input != Vector3.zero)
     {

         var localTarget = transform.InverseTransformPoint(transform.position + _heading);
    
         var angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

         var eulerAngleVelocity  = new Vector3 (0, angle, 0) * _rotationSpeed;
         var deltaRotation  = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime );
         _rb.MoveRotation(_rb.rotation * deltaRotation);
     }
 }
 void Move()
 {
     if (_input.magnitude > 0.1f || _dashing)
     {
     Vector3 rightMovement = _right * _speed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
     Vector3 upMovement = _forward * _speed * Time.deltaTime * Input.GetAxisRaw("Vertical");
          
     _heading = Vector3.Normalize(rightMovement+upMovement);
     
     _rb.MovePosition(transform.position + _heading * _speed * Time.deltaTime);
     }
     
    
 }

 void CheckDash()
 {
     if (Input.GetButtonDown("Fire2") && _canDash)
     {
         _gatherInput = false;
         if (_input.magnitude < 0.1f)
         {
             _input = transform.forward;
         }

         StartCoroutine("ResetDashCooldown");
         
     }
 }

 IEnumerator ResetDashCooldown()
 {
     _canDash = false;
     _speed = _dashSpeed;
     _dashing = true;
     yield return new WaitForSeconds(_dashDuration);
     _dashing = false;
     _gatherInput = true;
     _speed = _walkSpeed;
     yield return new WaitForSeconds(_dashCooldown - _dashDuration);
     _canDash = true;

 }
 IEnumerator ResetAttackCooldown()
 {
     
     yield return new WaitForSeconds(0.1f);
     _attackTrigger.SetActive(false);
     yield return new WaitForSeconds(_attackCooldown-0.1f);
     canAttack = true;
 }
}
