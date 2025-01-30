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
        public float Damage;
        #endregion

        #region �̵� ���� ����
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
        private RectTransform crosshairRectTransform; // UI RectTransform

        private float nextFireTime = 1f;
        #endregion

        #region ü�¹� ���� ����
        public Slider healthBar;
        #endregion

        #region ���� ���� ����
        public AudioSource audioSource; // ����� �ҽ� ������Ʈ
        public AudioClip[] damageSounds; // ������ ���� �迭
        public AudioClip[] deathSounds; // ���� �� ���� �迭
        #endregion

        private GameObject nearestEnemy; // ���� ����� �� ����

        public GameManager gameManager;      // ���� �Ŵ��� ����
        public SkillManager skillManager;    // ��ų �Ŵ��� ����

        public Bullet Bullet
        {
            get => default;
            set
            {
            }
        }

        public GameManager GameManager
        {
            get => default;
            set
            {
            }
        }

        public HealthBarController HealthBarController
        {
            get => default;
            set
            {
            }
        }

        public Joystick Joystick
        {
            get => default;
            set
            {
            }
        }

        #region ����Ƽ �⺻ �Լ�
        void Start()
        {
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;
            Damage = 5f;

            if (crosshairUI != null)
            {
                crosshairRectTransform = crosshairUI.GetComponent<RectTransform>();
                crosshairUI.SetActive(false); // ó������ ��Ȱ��ȭ
            }
        }

        void Update()
        {
            //MovePlayer();
            CheckForEnemies();
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
            moveDirection = new Vector3(input.x, 0, input.y) * moveSpeed;

            if (moveDirection.magnitude > 0.01f)
            {
                lastmoveDirection = moveDirection.normalized;
                animator.SetBool("Run", true);
            }
            else
                animator.SetBool("Run", false);

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            if (lastmoveDirection.magnitude > 0)
            {
                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(lastmoveDirection);

                // Y�� ȸ���� ����
                transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, currentRotation.eulerAngles.z);
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
             //�Ѿ� ����
            GameObject bullet = Instantiate(bulletPrefab, transform.position + lastmoveDirection, Quaternion.identity);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            Bullet bull = bullet.GetComponent<Bullet>();
            if (bull != null)
                bull.SetBulletProperties(Damage,this.tag);

            // ���� ���� ���� ���
            Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
            if (bulletRb != null)
                bulletRb.velocity = directionToEnemy * bulletSpeed;

            if (nearestEnemy != null)
            {
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

            if (nearestEnemy != null)
            {
                UpdateCrosshairUI(nearestEnemy.transform.position); // ���� ��ġ�� UI�� ������Ʈ
                crosshairUI.SetActive(true);
            }
            else
            {
                crosshairUI.SetActive(false); // ���� ������ UI ��Ȱ��ȭ
            }
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
        }
        #endregion

        #region ü�¹� ���� �Լ�
        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
                Debug.Log($"currentHealth = {currentHealth}");
            }
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
    }
}