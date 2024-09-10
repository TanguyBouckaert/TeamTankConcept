using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public float MovementSpeed, DetectionRange, StopRange, ShootingRange, BulletSpeed;
    [SerializeField] Slider Healthbar;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] Transform BarrelEnd;

    private GameObject _target, _bullet;
    private float _timer, _tankHealth;
    private bool _roundOut;

    private void Start()
    {
        _target = GameObject.Find("Tank");

        if( _target == null)
        {
            Debug.Log("Tank NOT Found!");
        }

    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        _tankHealth = _target.transform.Find("TankCanvas").Find("TankHealthBar").GetComponent<Slider>().value;

        if( distanceToPlayer <= DetectionRange && distanceToPlayer > StopRange )
        {
            Vector3 direction = (_target.transform.position - transform.position).normalized;
            transform.forward = direction;
            transform.position += direction * MovementSpeed * Time.deltaTime;
        }

        if (distanceToPlayer <= ShootingRange)
            Fire();

        if (_bullet == null)
            _roundOut = false;

        CheckHealth();
    }

    private void Fire()
    {
        _timer += Time.deltaTime;

        if ((_timer > 5f || !_roundOut) && _tankHealth > 1.0f)
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
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Healthbar.value = Healthbar.value - 20f;
            Destroy(other.gameObject);
        }
    }
}
