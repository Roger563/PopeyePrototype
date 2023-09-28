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
 [HideInInspector] public bool _canDash = false;
 [SerializeField] private float _dashSpeed =30f;
 [SerializeField] private float _dashCooldown =1f;

 [Header("Attack")]
 [SerializeField] private float _attackCooldown =1;
 [SerializeField] private GameObject _attackTrigger;
 private bool canAttack = true;
 private float _maxDistance;
 [HideInInspector] public bool ChainTensed = false;

 [SerializeField] private Transform anchor;
 private bool dashJustPressed =false;
 private bool hammerAttacking = false;
 private bool canHammerAttack = false;
 private bool performingAttack = false;
 private float offset = 0;

 private LineRenderer _lineRenderer;
 private AnchorController _anchorController;

 private Vector3 axis;
 private void Start()
 {
     _forward = Camera.main.transform.forward;
     _forward.y = 0;
     _forward.Normalize();
     _right = Quaternion.Euler(new Vector3(0,90, 0)) * _forward;
     _speed = _walkSpeed;
     _lineRenderer = GetComponent<LineRenderer>();
     _anchorController = GetComponent<AnchorController>();
     _maxDistance = gameObject.GetComponent<AnchorController>().maxDistance;
 }

 private void Update()
 {
     if(_gatherInput)
        GatherInput();
   //the attack should probably be done from another script.
   CheckAttack();
   CheckDash();
   HammerAttack();
   orbitAnchorMovement();

 }

 private void FixedUpdate()
 {
     Look();
     Move();
 }

 void GatherInput()
 {
     _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
         _input.Normalize();
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
        Vector3 rightMovement = _right * _speed * Time.deltaTime * _input.x;
        Vector3 upMovement = _forward * _speed * Time.deltaTime * _input.z;
        if (!_dashing)
        {
            _heading = Vector3.Normalize(rightMovement + upMovement);
            _rb.MovePosition(transform.position + _heading * _speed * Time.deltaTime);
        }
         else if (_dashing)
        {
            _heading = Vector3.Normalize(_input);
            _rb.MovePosition(transform.position + _input * _speed * Time.deltaTime);
        }
     }

     if (ChainTensed)
     {

         Vector3 directionToTarget = (anchor.position - transform.position).normalized;
         Vector3 newPosition = anchor.position - directionToTarget * _maxDistance;
         transform.position = newPosition;

     }
     
 }


     

 void CheckDash()
 {
     
     if (_canDash && Input.GetButtonDown("Fire2"))
     {
         _speed = _dashSpeed;
         _input = anchor.position - transform.position;
         _input.Normalize();
         _dashing = true;
         _gatherInput = false;
         dashJustPressed = true;
         StartCoroutine("StopDash");
     }
     
 }

 IEnumerator StopDash()
 {
     yield return new WaitForSeconds(0.1f);
     Vector3 directionToTarget = (anchor.position - transform.position).normalized;
     Vector3 newPosition = anchor.position - directionToTarget * _maxDistance;
     transform.position = newPosition;
     _speed = _walkSpeed;
     _dashing = false;
     _gatherInput = true;
     StartCoroutine("HammerAttackPerfectTiming");
 }

 IEnumerator HammerAttackPerfectTiming()
 {
     canHammerAttack = true;
     _lineRenderer.endColor = Color.blue;
     _lineRenderer.startColor = Color.blue;
     yield return new WaitForSeconds(0.1f);
     canHammerAttack = false;
     _lineRenderer.endColor = Color.green;
     _lineRenderer.startColor = Color.green;
     _anchorController.ChainCompletelyTensed = false;

 }
 IEnumerator ResetAttackCooldown()
 {
     
     yield return new WaitForSeconds(0.1f);
     _attackTrigger.SetActive(false);
     yield return new WaitForSeconds(_attackCooldown-0.1f);
     canAttack = true;
 }


 void HammerAttack()
 {
     if (canHammerAttack && Input.GetButtonDown("Fire1"))
     {
         hammerAttacking = true;
         axis = transform.position - anchor.position;
         axis.Normalize();
     }
     if (hammerAttacking)
     {
         float distance = Vector3.Distance(anchor.position, transform.position);
         Vector3 moveDirection = transform.position - anchor.position;
         moveDirection.Normalize();
         if (distance > 0.5f)
         {
             anchor.Translate(moveDirection * 80 * Time.deltaTime);
         }
         else
         {
             hammerAttacking = false;
         }
     }
 }

 void orbitAnchorMovement()
 {
     if (Input.GetButton("Fire3"))
     {
         offset += Time.deltaTime;
         float rotationSpeed = 100;
         float radius = Vector3.Distance(anchor.position, transform.position);
         // Calculate the desired position in a circle around the center object
         Vector3 orbitPosition = transform.position + Quaternion.Euler(anchor.position.x, rotationSpeed * offset, anchor.position.z) * Vector3.forward * radius;
         // Set the position of the orbiting object
         anchor.position = orbitPosition;
     }

     if (Input.GetButtonUp("Fire3"))
     {
        
     }
 }
 
}
