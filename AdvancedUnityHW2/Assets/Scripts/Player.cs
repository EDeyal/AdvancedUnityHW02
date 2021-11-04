using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Physics and movement
    private Rigidbody _rigidbody;
    private Camera _camera;

    //movement
    public float mouseSensitivity = 200;
    [SerializeField]
    private float _speed = 1;
    [SerializeField]
    private int _jumpForce = 5;
    private float _horizontalInput;
    private float _verticalInput;
    float _camRotX;
    [SerializeField]
    float _camMaxHightAngle = 60;
    [SerializeField]
    float _camMinHightAngle = -60;
    Vector3 _verticalRotation;
    Vector3 _rotation;


    //grounded
    [SerializeField]
    private float _rayLength = 0.7f;
    [SerializeField]
    private bool _isGrounded = false;
    private LayerMask _groundLayer = 1 << 9;
    private RaycastHit _hitGround;

    #endregion

    #region Gun Stuff
    private ParticleSystem _gun;
    private float _timeSinceLastShot = 0;
    [SerializeField]
    private float _gunCD = 1;
    #endregion

    #region Health
    private int _maxHP = 100;
    [SerializeField]
    private int _currentHP;
    [SerializeField]
    private bool _isHit = false;
    private float _hitTimer;
    private float _takeDamage = 0;

    #endregion

    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _camRotX = _camera.transform.localEulerAngles.x;
        _rigidbody = GetComponent<Rigidbody>();
        _gun = GetComponentInChildren<ParticleSystem>();
        transform.position = new Vector3(22.5f, 2, -22.5f);
        _currentHP = _maxHP;
    }

    void Update()
    {
        Movement();
        PlayerLook();
        timer();
        if (Input.GetMouseButtonDown(0))
        {
            ShootGun();
        }
        _takeDamage += Time.deltaTime;
        if (_takeDamage > 1)
        {
            _currentHP--;
            float HPRatio = (float)_currentHP / _maxHP;
            //_HPBarImage.fillAmount = HPRatio;
            _takeDamage = 0;
        }
        //take damage per second
    }
    private void Movement()
    {
        Debug.DrawRay(transform.position, Vector3.down * _rayLength);
        if (Physics.BoxCast(transform.position, transform.lossyScale / 2, Vector3.down, transform.rotation, _rayLength, _groundLayer))
        {
            //Debug.Log("Hit: " + _hitGround.collider.name);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded == true)
        {
            _rigidbody.AddForce(transform.up * _jumpForce);
            _isGrounded = false;
        }
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.right * _horizontalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.forward * _verticalInput * _speed * Time.deltaTime);
    }
    public void MovePlayer(Vector3 MovementDelta)
    {
        transform.position += MovementDelta;
    }
    private void PlayerLook()
    {
        //_rotation = new Vector3(0, Input.GetAxisRaw("Mouse X"), 0) * mouseSensitivity * Time.deltaTime;
        //_verticalRotation = new Vector3(Input.GetAxisRaw("Mouse Y"), 0,0) * mouseSensitivity * Time.deltaTime;
        //_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(_rotation));
        //_camera.transform.Rotate(-_verticalRotation);

        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        _camRotX -= mouseY;
        _camRotX = Mathf.Clamp(_camRotX, _camMinHightAngle, _camMaxHightAngle);
        _camera.transform.localRotation = Quaternion.Euler(_camRotX, _camera.transform.localEulerAngles.y, 0);
        float rotY = transform.eulerAngles.y;
        rotY += mouseX;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotY, transform.localEulerAngles.z);

        //float verticalLookDirection = 0;
        //float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        //float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        //transform.Rotate(Vector3.up, mouseX);

        //verticalLookDirection -= mouseY;
        //verticalLookDirection = Mathf.Clamp(verticalLookDirection, -90, 90);
        //_camera.transform.
        //localRotation = Quaternion.Euler(verticalLookDirection, 0, 0);
    }
    private void timer()
    {
        if (_timeSinceLastShot < _gunCD)
        {
            _timeSinceLastShot += Time.deltaTime;
        }
        if (_isHit == true)
        {
            //Debug.Log("recive lazer damage");
            TakeDamage(10);
            _isHit = false;
            //_gameManager.totaltime -= 10;
        }
        else
        {
            _hitTimer += Time.deltaTime;
            if (_hitTimer > 5)
            {
                _hitTimer = 2;
            }
        }

    }
    private void ShootGun()
    {
        if (_gun == null)
        {
            Debug.Log("Gun is Null");
            return;
        }
        else if (_timeSinceLastShot >= _gunCD)
        {
            _gun.Play();
            _timeSinceLastShot = 0;
        }
    }
    public void TakeDamage(int DamageAmount)
    {
        _currentHP -= DamageAmount;
        float HPRatio = (float)_currentHP / _maxHP;
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Flag")
        //{
        //    if (_gameManager != null)
        //    {
        //        _gameManager.finishLevel = true;

        //    }
        //    else
        //    {
        //        Debug.Log("Game Manager is NULL");
        //    }
        //}
        if (other.tag == "Spawner")
        {
            //transform player to starting point and reduce 10 seconds from the timer
            transform.position = new Vector3(22.5f, 2, -22.5f);
            TakeDamage(10);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Lazer")
        {

            if (_hitTimer < 1)
            {
                //Debug.Log("not suppose to recive damage");
                _isHit = false;
            }
            else if (_hitTimer >= 1)
            {
                //Debug.Log("SupposetoreciveDamage");
                _isHit = true;
                _hitTimer = 0;
            }
        }
    }
}