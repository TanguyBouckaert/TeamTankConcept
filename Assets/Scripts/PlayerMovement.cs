using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameObject Tank;
    [SerializeField] float WalkSpeed, TurnSpeed;
    [SerializeField] Slider Healthbar;

    //New Input System
    [SerializeField] public InputAction Turn;
    [SerializeField] public InputAction ForwardBackward;
    [SerializeField] public InputAction InteractWithTank;
    [SerializeField] public InputAction RepairTank;

    [SerializeField] public Image[] Inventory;

    private int _resourceAmount;
    private bool _nearTank;

    private void Awake()
    {
        InteractWithTank.started += InteractWithTank_started;
        RepairTank.started += RepairTank_started;
    }

    private void InteractWithTank_started(InputAction.CallbackContext obj)
    {
        if (_nearTank)
            EnterTank();
    }

    private void RepairTank_started(InputAction.CallbackContext obj)
    {
        if (_nearTank)
        {
            if (_resourceAmount > 0)
            {
                Inventory[_resourceAmount - 1].enabled = false;
                _resourceAmount--;
                Repair();
            }
            else
                Debug.Log("No Resources!");
        }

    }

    private void OnEnable()
    {
        Debug.Log("Switched to PLAYER Controlls!");

        Turn.Enable();
        ForwardBackward.Enable();

        InteractWithTank.Enable();
        RepairTank.Enable();    
    }

    private void OnDisable()
    {
        Turn.Disable();
        ForwardBackward.Disable();

        InteractWithTank.Disable();
        RepairTank.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Controlles();
        CheckHealth();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tank.tag)
            _nearTank = true;

        if (other.CompareTag("Resource"))
        {
            foreach (var item in Inventory)
            {
                if (!item.isActiveAndEnabled)
                {
                    item.gameObject.SetActive(true);
                    item.gameObject.GetComponent<Image>().enabled = true;
                    _resourceAmount++;
                    Destroy(other.gameObject);
                    break;
                }
                else
                    continue;
            }
        }

        if (other.CompareTag("Bullet"))
        {
            Healthbar.value = Healthbar.value - 10f;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Tank.tag)
            _nearTank = false;
    }

    private void EnterTank()
    {
        //Disable Player Component
        this.transform.gameObject.SetActive(false); 
        this.gameObject.transform.Find("PlayerCamera").gameObject.SetActive(false); //Camera

        //Enable Tank Component
        Tank.gameObject.GetComponent<TankMovement>().enabled = true;
        Tank.gameObject.transform.Find("Turret").Find("TankCamera").gameObject.SetActive(true);
    }

    private void Controlles()
    {
        transform.rotation = this.transform.rotation * Quaternion.Euler(0f, Turn.ReadValue<float>() * TurnSpeed, 0f);

        transform.Translate(Vector3.forward * ForwardBackward.ReadValue<float>() * WalkSpeed * Time.deltaTime);
    }

    private void Repair()
    {
        float tankHealth = Tank.gameObject.transform.Find("TankCanvas").Find("TankHealthBar").GetComponent<Slider>().value;
        Tank.gameObject.transform.Find("TankCanvas").Find("TankHealthBar").GetComponent<Slider>().value = tankHealth + 20f;
    }

    private void CheckHealth()
    {
        if (Healthbar.value < 1f)
        {
            Application.Quit();
        }
    }
}
