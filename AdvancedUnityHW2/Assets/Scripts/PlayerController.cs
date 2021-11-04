using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Transform cameraTransform;

    float verticalLookDirection;
    [SerializeField]
    CharacterController charController;
    float groundCheckRadius = 0.2f;
    bool isGrounded;
    ParticleSystem gun;
    float timeSinceLastShot;
    public float gunCD = 1;
    [SerializeField] int maxHP = 0;
    [SerializeField] int currentHP = 0;

    //public Image HPBarImage;

    public Transform groundTouchCheck;
    public LayerMask groundMask;
    public float gravity = -9.81f;
    public float mouseSensitivity = 150;
    public float moveSpeed = 10;
    public float jumpPower = 5;

    public Vector3 fallVelocity = Vector3.zero;
    void Start()
    {
        //cameraTransform = Camera.main.transform;
        charController = GetComponent<CharacterController>();
        gun = GetComponentInChildren<ParticleSystem>();
       // transform.position = new Vector3(22.5f, 2, -22.5f);
        TakeDamage(25);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLook();
        PlayerMove();
        //PlayerGravity();
        Timer();
        if (Input.GetMouseButtonDown(0))
        {
            ShootGun();
        }
    }

    void PlayerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        transform.Rotate(Vector3.up, mouseX);

        verticalLookDirection -= mouseY;

        verticalLookDirection = Mathf.Clamp(verticalLookDirection, -90f, 90f);
        //cameraTransform.localRotation = Quaternion.Euler(verticalLookDirection, 0f, 0f);
    }

    void PlayerMove()
    {
        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = (transform.forward * movementZ) + (transform.right * movementX);
        charController.Move(moveDirection * moveSpeed * Time.deltaTime);

        //jumping:
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            fallVelocity.y = jumpPower;
        }
    }

    void PlayerGravity()
    {
        isGrounded = Physics.CheckSphere(groundTouchCheck.position, groundCheckRadius, groundMask);
        if (isGrounded && fallVelocity.y < 0)
        {
            fallVelocity = Vector3.zero;
        }
        else
        {
            fallVelocity.y += gravity * Time.deltaTime;
            charController.Move(fallVelocity * Time.deltaTime);
        }
    }
    void Timer()
    {
        if (timeSinceLastShot < gunCD)
        {
            timeSinceLastShot += Time.deltaTime;
        }
    }

    void ShootGun()
    {
        if (gun == null)
        {
            Debug.Log("Gun is NULL");
            return;
        }
        if (timeSinceLastShot >= gunCD)
        {
            gun.Play();
            timeSinceLastShot = 0;
        }
    }
    void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        float hpRatio = (float)currentHP / maxHP;
        //HPBarImage.fillAmount = hpRatio;
    }
}
