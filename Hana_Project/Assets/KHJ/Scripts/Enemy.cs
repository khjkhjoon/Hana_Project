using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hana.KHJ
{
    public class Enemy : MonoBehaviour
    {
        #region �⺻ ���� ���� ����
        public float power = 2f; 
        [SerializeField] private float maxHealth = 10f; // �ִ� ü��
        private float currentHealth; // ���� ü��
        #endregion

        #region �Ÿ� ���� ���� ����
        public Transform player; // Player�� Transform
        public float minDistance = 3f; // Player���� �ּ� �Ÿ�
        public float maxDistance = 5f; // Player���� �ִ� �Ÿ�
        public float moveSpeed = 2f; // Enemy �̵� �ӵ�
        public Vector2 mapBoundsMin = new Vector2(-10f, -10f); // �� �ּ� ��ǥ
        public Vector2 mapBoundsMax = new Vector2(10f, 10f);   // �� �ִ� ��ǥ
        #endregion

        #region ��� ���� ����
        [Header("��� ����")]
        [SerializeField] private GameObject bulletPrefab; // �Ѿ� ������
        [SerializeField] private float bulletSpeed = 10f; // �Ѿ� �ӵ�
        [SerializeField] private float fireRate = 2f;     // �߻� ���� (�� ����)
        private float nextFireTime = 0f;                 // ���� �߻� ���� �ð�
        private Vector3 lastMoveDirection;               // �ֱ� �̵� ����
        #endregion

        #region Unity �⺻ �Լ�
        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            MaintainDistanceFromPlayer();
            Shooting();
        }
        #endregion

        #region ü�� ���� �Լ�
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            Debug.Log($"Enemy Health: {currentHealth}");

            StartCoroutine(FlashRed());

            if (currentHealth <= 0)
            {
                OnDestroy();
            }
        }

        private IEnumerator FlashRed()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();

            if (mr == null)
            {
                Debug.LogError("MeshRenderer == null");
                yield break;
            }

            // ���� ���� ����
            Color originalColor = mr.material.color;

            // ���������� ����
            Color redColor = new Color(1f, 0f, 0f, 0.5f);
            mr.material.color = redColor;

            // ��� ���
            yield return new WaitForSeconds(0.1f);

            // �� ����
            mr.material.color = originalColor;
        }

        private void OnDestroy()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region �̵� ���� �Լ�
        private void MaintainDistanceFromPlayer()
        {
            if (player == null)
            {
                Debug.LogWarning("Player Transform�� �������� �ʾҽ��ϴ�.");
                return;
            }

            Vector3 directionToPlayer = player.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer < minDistance)
            {
                // Player�κ��� �־���
                MoveInDirection(-directionToPlayer);
            }
            else if (distanceToPlayer > maxDistance)
            {
                // Player���� �������
                MoveInDirection(directionToPlayer);
            }

            // minDistance <= distanceToPlayer <= maxDistance �� ���� ������ ����
        }

        private void MoveInDirection(Vector3 direction)
        {
            // �̵� ���� ����
            Vector3 moveDirection = direction.normalized;

            // �� ��ġ ���
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

            // �� ��� �� �̵� ����
            newPosition.x = Mathf.Clamp(newPosition.x, mapBoundsMin.x, mapBoundsMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, mapBoundsMin.y, mapBoundsMax.y);

            // ��ġ ������Ʈ
            transform.position = newPosition;

            // �ֱ� �̵� ���� ����
            lastMoveDirection = moveDirection;
        }
        #endregion

        #region ��� ���� �Լ�
        private void Shooting()
        {
            if (Time.time >= nextFireTime && bulletPrefab != null && player != null)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }

        private void Fire()
        {
            // �Ѿ� ����
            GameObject bullet = Instantiate(bulletPrefab, transform.position + lastMoveDirection, Quaternion.identity);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            Bullet bull = bullet.GetComponent<Bullet>();

            // �Ѿ� �Ӽ� ����
            if (bull != null)
            {
                bull.SetBulletProperties(power, this.tag); // �������� �±� ����
            }

            // �÷��̾ ���� ���� ���
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // �Ѿ��� �ӵ� ����
            if (bulletRb != null)
            {
                bulletRb.velocity = directionToPlayer * bulletSpeed;
            }
        }
        #endregion
    }
}
