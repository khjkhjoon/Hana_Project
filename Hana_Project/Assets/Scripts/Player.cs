using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region �̵� ���� ����
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private float hAxis;
    private float vAxis;

    private Vector3 moveDirection;
    private Vector3 lastmoveDirection;
    #endregion

    #region ��� ���� ����
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;
    public GameObject bulletPrefab;

    private float nextFireTime = 1f;
    #endregion

    private GameObject nearestEnemy; // ���� ����� �� ����

    void Start()
    {
    }

    void Update()
    {
        MovePlayer();
        CheckForEnemies();
        Shooting();
    }

    void MovePlayer()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");

        moveDirection = new Vector3(hAxis, 0, vAxis);

        if (moveDirection.magnitude > 0.1f)
            lastmoveDirection = moveDirection.normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        if (lastmoveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastmoveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Shooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        if (bulletPrefab != null && nearestEnemy != null)
        {
            // �Ѿ� ����
            GameObject bullet = Instantiate(bulletPrefab, transform.position + lastmoveDirection, Quaternion.identity);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            // ���� ���� ���� ���
            Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
            bulletRb.velocity = directionToEnemy * bulletSpeed;

            Debug.Log($"Firing at {nearestEnemy.name}!");
        }
        else if (nearestEnemy == null)
        {
            Debug.Log("No enemy detected to fire at.");
        }
    }

    void CheckForEnemies()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 100f);
        nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) // �±װ� "Enemy"���� Ȯ��
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }
        }

        if (nearestEnemy != null)
        {
            Debug.Log($"Nearest Enemy: {nearestEnemy.name}");
            Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
            lastmoveDirection = directionToEnemy;
        }
    }
}
