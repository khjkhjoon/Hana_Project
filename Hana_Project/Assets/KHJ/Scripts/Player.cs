using System.Collections;
using System.Collections.Generic;
using Hana.Common;
using Hana.LYJ;
using UnityEngine;
using UnityEngine.UI;

namespace Hana.KHJ
{
    public class Player : MonoBehaviour
    {
        #region �⺻ ���� ����
        public float maxHealth = 10f;
        public float currentHealth;
        public float Damage = 5f;
        #endregion

        #region �̵� ���� ����
        private Rigidbody rb;
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public Joystick joystick; // ���̽�ƽ ��ũ��Ʈ ����

        private float hAxis;
        private float vAxis;

        private Vector3 moveDirection;
        private Vector3 lastmoveDirection;

        //private Rigidbody rb; // 3D ���� ó���� ���� Rigidbody
        private Animator animator; // �ִϸ����� �߰�
        #endregion

        #region ��� ���� ����
        public float bulletSpeed = 20f;
        public float fireRate = 0.5f;
        public GameObject bulletPrefab;
        public GameObject crosshairUI; // ũ�ν���� UI ������Ʈ
        public GameObject arrowObject;

        private RectTransform crosshairRectTransform; // UI RectTransform
        private float nextFireTime = 1f;
        #endregion

        #region ü�¹� ���� ����
        public Slider healthBar;
        #endregion

        #region ���� ���� ����
        public AudioSource audioSource; // ����� �ҽ� ������Ʈ
        public AudioClip attackSound; // ���� ����
        public AudioClip healSound; //ü�� ȸ�� ����
        public AudioClip[] damageSounds; // ������ ���� �迭
        public AudioClip[] deathSounds; // ���� �� ���� �迭
        #endregion

        private GameObject nearestEnemy; // ���� ����� �� ����

        public GameManager gameManager;      // ���� �Ŵ��� ����
        public SkillManager skillManager;    // ��ų �Ŵ��� ����

        #region ����Ƽ �⺻ �Լ�
        void Start()
        {
            rb = GetComponent<Rigidbody>(); 
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;

            if (crosshairUI != null)
            {
                crosshairRectTransform = crosshairUI.GetComponent<RectTransform>();
                crosshairUI.SetActive(false); // ó������ ��Ȱ��ȭ
            }
        }

        void Update()
        {
            CheckForEnemies();
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Shooting();
            }
        }

        void FixedUpdate()
        {
            MovePlayer();
        }
        #endregion

        #region �̵����� �Լ�

        void MovePlayer()
        {
            Vector2 input = joystick.GetInput();

            // 3D �̵� ���� ���
            moveDirection = new Vector3(input.x, 0, input.y);

            if (moveDirection.magnitude > 0.01f)
            {
                lastmoveDirection = moveDirection.normalized;
                animator.SetBool("Run", true);

                Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(targetPosition);
            }
            else
                animator.SetBool("Run", false);


            if (lastmoveDirection.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lastmoveDirection);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
        #endregion

        #region �÷��̾� ��� ���� �Լ�
        public void Shooting()
        {
            if (Time.time >= nextFireTime && bulletPrefab != null && nearestEnemy != null)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
            }
        }

        void Fire()
        {            
            //���� ���� ���
            audioSource.PlayOneShot(attackSound);

            // �Ѿ� ����
            GameObject bullet = Instantiate(bulletPrefab, transform.position + lastmoveDirection, Quaternion.LookRotation(lastmoveDirection));
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            Bullet bull = bullet.GetComponent<Bullet>();

            if (bull != null)
                bull.SetBulletProperties(Damage, this.tag);

            // ���� ���� ���� ���
            if (nearestEnemy != null)
            {
                Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
                if (bulletRb != null)
                    bulletRb.velocity = directionToEnemy * bulletSpeed;

                lastmoveDirection = directionToEnemy;
            }
        }


        void CheckForEnemies()
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 100f);
            nearestEnemy = null;
            float shortestDistance = Mathf.Infinity;

            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy.gameObject;
                    }
                }
            }

            UpdateCrosshairUI(nearestEnemy?.transform.position ?? Vector3.zero);
            crosshairUI.SetActive(nearestEnemy != null);
            UpdateArrowDirection();  // ȭ��ǥ ���� ������Ʈ
        }

        void UpdateCrosshairUI(Vector3 enemyWorldPosition)
        {
            if (crosshairRectTransform != null)
            {
                // ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemyWorldPosition);

                // ũ�ν���� RectTransform�� ��ġ ������Ʈ
                crosshairRectTransform.position = screenPosition;
            }
        }

        void UpdateArrowDirection()
        {
            if (nearestEnemy != null && arrowObject != null)
            {
                // �÷��̾��� ��ġ
                Vector3 playerPosition = transform.position;

                // ���� ���� ��� (�÷��̾�� �� ������ ����)
                Vector3 directionToEnemy = nearestEnemy.transform.position - playerPosition;

                // ������ ����ȭ�Ͽ� ���� �Ÿ���ŭ�� �̵��ϵ��� ����
                Vector3 arrowDirection = directionToEnemy.normalized;

                float maxDistance = 100.0f;
                Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
                Vector3 targetPosition = playerScreenPosition + new Vector3(arrowDirection.x, arrowDirection.y, 0) * maxDistance;
                arrowObject.GetComponent<RectTransform>().position = targetPosition;
                // Y���� 0���� �����Ͽ� ���� ��鿡���� ȸ���ϵ��� �Ѵ�.
                directionToEnemy.y = 0;

                // ������ 0���Ͱ� �ƴ� ��쿡�� ȸ��
                if (directionToEnemy.magnitude > 0.1f)
                {
                    // Z���� �������� ȸ�� (Y ���� ����)
                    float angle = Mathf.Atan2(directionToEnemy.z, directionToEnemy.x) * Mathf.Rad2Deg;

                    // ȭ��ǥ�� ȸ���� Z���� �������� ����
                    arrowObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }

        #endregion

        #region ������ �� ���ó��
        public void TakeDamage(float damage)
        {
            // ������ �ޱ�
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            PlayRandomSound(damageSounds); // ������ ���� ���

            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Player Died");
            PlayRandomSound(deathSounds);
            GameManager.Instance.GameOver();  // ���� ���� ȣ��
        }
        #endregion

        #region ü�� ���� �Լ�
        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
                Debug.Log($"currentHealth = {currentHealth}");
            }
        }

        public void Heal(float amount)
        {
            currentHealth += amount;
            audioSource.PlayOneShot(healSound);
            UpdateHealthBar();
        }
        #endregion

        #region ���� ���� �Լ�
        private void PlayRandomSound(AudioClip[] soundArray)
        {
            if (soundArray != null && soundArray.Length > 0 && audioSource != null)
            {
                int randomIndex = Random.Range(0, soundArray.Length); // ���� �ε��� ����
                audioSource.PlayOneShot(soundArray[randomIndex]); // ���� ���
            }
        }
        #endregion

        #region ��ų ���� �Լ�
        public void IncreaseAttackPower()
        {
            Damage += 5;
            Debug.Log("���ݷ� ����! ���� ���ݷ�: " + Damage);
        }

        public void IncreaseAttackSpeed()
        {
            fireRate *= 0.9f; // ���� �ӵ� ���� (������ ����)
            Debug.Log("���� �ӵ� ����! ���� ���� ������: " + fireRate);
        }

        public void IncreaseMoveSpeed()
        {
            moveSpeed += 1.5f;
            Debug.Log("�̵� �ӵ� ����! ���� �̵� �ӵ�: " + moveSpeed);
        }
        #endregion
    }
}