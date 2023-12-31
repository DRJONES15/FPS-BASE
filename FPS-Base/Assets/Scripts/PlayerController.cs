﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed, gravityModifier, jumpPower, runSpeed = 12f;
    public CharacterController charCon;
    public Transform camTrans;
    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    private Vector3 moveInput;
    private bool canJump, canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    public Animator anim;

    public GameObject bullet;
    public Transform firePoint;




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //walking movment
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        //store y velocity
        float yStore = moveInput.y;

        Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

        moveInput = horiMove + vertMove;
        moveInput.Normalize();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput = moveInput * runSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }

        moveInput.y = yStore;

        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (charCon.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;

        if (canJump)
        {
            canDoubleJump = false;
        }

        //Handle Jumping

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            moveInput.y = jumpPower;

            canDoubleJump = true;

        }
        else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = jumpPower;

            canDoubleJump = false;

        }

        charCon.Move(moveInput * Time.deltaTime);

        //camra movment
        //left and right
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        //up and down
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f , 0f));
        //inver camera
        if(invertX)
        {
          mouseInput.x = -mouseInput.x;
        }
        if(invertY)
        {
          mouseInput.y = -mouseInput.y;
        }

        //Handle Shooting
        if(Input.GetMouseButtonDown(0))
        {
          Debug.DrawRay(camTrans.position, camTrans.forward * 100, Color.red, 4, false);
          RaycastHit hit;
          if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, Mathf.Infinity))
          {
              Debug.Log(hit.point);
              if (Vector3.Distance(camTrans.position, hit.point) > 2f)
              {
                  firePoint.LookAt(hit.point);
              }
          }
          else
          {
              firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
          }



          Instantiate(bullet, firePoint.position, firePoint.rotation);
        }

        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround", canJump);

    }
}
