using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TankMovement : MonoBehaviour
{
    [SerializeField] GameObject Player01, Player02, BulletPrefab;
    [SerializeField] Transform Player01Spawn, Player02Spawn, BarrelEnd;
    [SerializeField] float TurnSpeed, DriveSpeed, BulletSpeed;
    [SerializeField] Slider Healthbar;

    //Controlles
    [SerializeField] InputAction WheelTurning;
    [SerializeField] InputAction Throttle;

    [SerializeField] InputAction TurretTurning;
    [SerializeField] InputAction FireAction;

    [SerializeField] InputAction ExitPlayer01;
    [SerializeField] InputAction ExitPlayer02;

    private Transform _turretTrans;
    private GameObject _bullet;
    private float _timer;
    private bool _roundOut, _player01Entered, _player02Entered;

    private void Awake()
    {
        Player01.GetComponent<PlayerMovement>().InteractWithTank.started += Player01_InteractWithTank_started;
        Player02.GetComponent<PlayerMovement>().InteractWithTank.started += Player02_InteractWithTank_started;

        ExitPlayer01.started += ExitPlayer01_started;
        ExitPlayer02.started += ExitPlayer02_started;
    }

    private void ExitPlayer02_started(InputAction.CallbackContext context)
    {
        _player02Entered = false;
        transform.Find("Turret").Find("TankCamera").GetComponent<Camera>().rect = new Rect(0.5f, 0, 1, 1);
        ExitTank(Player02);
    }

    private void ExitPlayer01_started(InputAction.CallbackContext obj)
    {
        _player01Entered = false;
        transform.Find("Turret").Find("TankCamera").GetComponent<Camera>().rect = new Rect(0,0,0.5f,1);
        ExitTank(Player01);
    }

    private void Player02_InteractWithTank_started(InputAction.CallbackContext context)
    {
        _player02Entered = true;
        if(_player01Entered)
            transform.Find("Turret").Find("TankCamera").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        else
            transform.Find("Turret").Find("TankCamera").GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
    }

    private void Player01_InteractWithTank_started(InputAction.CallbackContext obj)
    {
        _player01Entered = true;
        if (_player02Entered)
            transform.Find("Turret").Find("TankCamera").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        else
            transform.Find("Turret").Find("TankCamera").GetComponent<Camera>().rect = new Rect(0.5f, 0, 1, 1);//
    }

    private void Start()
    {
        _roundOut = false;
        _turretTrans = transform.Find("Turret").gameObject.transform;
    }

    private void OnEnable()
    {
        Debug.Log("Switched to TANK Controlls!");

        WheelTurning.Enable();
        Throttle.Enable();
        TurretTurning.Enable();
        FireAction.Enable();
        ExitPlayer01.Enable();
        ExitPlayer02.Enable();
    }

    private void OnDisable()
    {
        WheelTurning.Disable();
        Throttle.Disable();
        TurretTurning.Disable();
        FireAction.Disable();
        ExitPlayer01.Disable();
        ExitPlayer02.Disable();
    }

    private void Update()
    {
        Controlles();

        CheckHealth();

    }

    private void ExitTank(GameObject player)
    {
        //Disable Tank components
        this.gameObject.transform.Find("Turret").Find("TankCamera").gameObject.SetActive(false); //Camera

        //Enable Player01 Components
        if (player == Player01)
            player.transform.position = Player01Spawn.position;
        if (player == Player02)
            player.transform.position = Player02Spawn.position;


        player.SetActive(true);
        player.gameObject.GetComponent<PlayerMovement>().enabled = true; //Script 
        player.gameObject.transform.Find("PlayerCamera").gameObject.SetActive(true); //Camera 

    }

    private void Controlles()
    {
        if (_player01Entered) 
        {
            //Wheels
            transform.rotation = this.transform.rotation * Quaternion.Euler(0f, WheelTurning.ReadValue<float>() * TurnSpeed * Time.deltaTime, 0f);

            transform.Translate(Vector3.forward * Throttle.ReadValue<float>() * DriveSpeed * Time.deltaTime);
        }
        
        if (_player02Entered)
        {
            //Turret
            _turretTrans.rotation = _turretTrans.rotation * Quaternion.Euler(0f, TurretTurning.ReadValue<float>() * TurnSpeed * Time.deltaTime, 0f);

            if (FireAction.ReadValue<float>() == 1f && !_roundOut)
            {
                Debug.Log("Firing!!");
                Fire();
            }
        }

        //Turret
        _turretTrans.rotation = _turretTrans.rotation * Quaternion.Euler(0f, TurretTurning.ReadValue<float>() * TurnSpeed * Time.deltaTime, 0f);

        if (FireAction.ReadValue<float>() == 1f && !_roundOut)
        {
            Debug.Log("Firing!!");
            Fire();
        } 

        if(_bullet == null)
            _roundOut = false;
    }

    private void Fire()
    {
        _timer += Time.deltaTime;

        if(_timer > 5f || !_roundOut)
        {
            _bullet = Instantiate(BulletPrefab, BarrelEnd);
            _roundOut = true;
            _bullet.transform.parent = null;
            _bullet.transform.localScale = Vector3.one;
            Rigidbody rb = _bullet.GetComponent<Rigidbody>();
            rb.velocity = BarrelEnd.forward * BulletSpeed;

            Destroy(_bullet, 5f);
            _timer = 0;
        }
        
    }

    private void CheckHealth()
    {
        if (Healthbar.value < 1f)
        {
            ExitTank(Player01);
            ExitTank(Player02);

            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Healthbar.value = Healthbar.value - 5f;
            Destroy(other.gameObject);
        }
    }
}
