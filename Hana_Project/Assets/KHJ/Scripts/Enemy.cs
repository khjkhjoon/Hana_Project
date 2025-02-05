using System.Collections;
using System.Collections.Generic;
using Hana.Common;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Hana.KHJ
{
    public class Enemy : MonoBehaviour
    {
        #region �⺻ ���� ���� ����
        public float power = 2f;
        [SerializeField] private float maxHealth = 10f;
        private float currentHealth;
        #endregion

        #region �Ÿ� ���� ���� ����
        public Transform player;
        public float minDistance = 3f;
        public float maxDistance = 5f;
        public float moveSpeed = 2f;

        [Header("�� ���� ����")]
        public GameObject mapObject;  // �� ������Ʈ ����
        private Bounds mapBounds;     // �� ���
        #endregion

        #region ��� ���� ����
        [Header("��� ����")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float fireRate = 2f;
        private float nextFireTime = 0f;
        private Vector3 lastMoveDirection;
        #endregion

        #region �ִϸ��̼� ���� ����
        private Animator animator;
        private bool isRunning = false;
        private bool isAttacking = false;
        #endregion

        #region ü�¹� ����
        [SerializeField] private GameObject healthBarPrefab;
        private GameObject healthBarInstance;
        private RectTransform healthBarTransform;

        #endregion

        #region ���� ����
        public AudioSource audiosource;
        public AudioClip hittingSound; //Ÿ����
        #endregion

        #region ��� ������
        [SerializeField] 
        private GameObject healthRecoveryPrefab;
        [SerializeField]
        private float healthRecoveryChance = 0.1f; //��� Ȯ��
        #endregion


        #region Unity �⺻ �Լ�
        private void Start()
        {
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();

            //Player �ڵ� �Ҵ�
            if (player == null)
            {
                // ���� ���� �� �� ���� ����
                if (GameManager.Instance.CurrentState == GameState.GameOver) return;

                GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
                if (foundPlayer != null)
                {
                    player = foundPlayer.transform;
                }
                else
                {
                    Debug.LogError("Player ������Ʈ�� ã�� �� �����ϴ�. 'Player' �±װ� �����Ǿ� �ִ��� Ȯ���ϼ���.");
                    Application.Quit();
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }
            //HealthBar ����
            if (healthBarPrefab != null)
            {
                GameObject canvas = GameObject.Find("Canvas");  // UI�� ǥ���� ĵ���� ã��
                if (canvas != null)
                {
                    healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
                    healthBarTransform = healthBarInstance.GetComponent<RectTransform>();
                }
            }
            // �� ��� ����
            if (mapObject != null)
            {
                if (mapObject == null)
                {
                    mapObject = GameObject.FindGameObjectWithTag("Ground");
                    if (mapObject == null)
                    {
                        Debug.LogError("�� ������Ʈ�� ã�� �� �����ϴ�.");
                        Application.Quit();
                        UnityEditor.EditorApplication.isPlaying = false;
                    }
                }
                Collider mapCollider = mapObject.GetComponent<Collider>();
                if (mapCollider != null)
                {
                    mapBounds = mapCollider.bounds;
                }
                else
                {
                    Debug.LogError("�� ������Ʈ�� Collider�� �����ϴ�.");
                    Application.Quit();
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }
            else
            {
                //Debug.LogError("�� ������Ʈ�� �������� �ʾҽ��ϴ�.");
            }
        }

        private void Update()
        {
            // ���� ���� �� �� ���� ����
            if (GameManager.Instance.CurrentState == GameState.GameOver) return;

            bool moving = MaintainDistanceFromPlayer();
            bool attacking = Shooting();

            // �÷��̾� �ٶ󺸱� (Y�� ȸ���� �ݿ�)
            if (player != null)
            {
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;  // Y�� ȸ�� ����
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }

            // �ִϸ��̼� ���� ������Ʈ
            if (animator != null)
            {
                animator.SetBool("isRunning", moving);
                animator.SetBool("isAttacking", attacking);
            }

            UpdateHealthBarPosition();
        }
        #endregion

        #region ü�� ���� �Լ�
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            Debug.Log($"Enemy Health: {currentHealth}");
            if (currentHealth <= 0)
            {
                Die();
            }
            StartCoroutine(FlashRed());

            if (healthBarInstance != null)
            {
                Slider healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
                if (healthSlider != null)
                {
                    healthSlider.value = currentHealth / maxHealth;
                }
            }
            audiosource.PlayOneShot(hittingSound);
        }

        private IEnumerator FlashRed()
        {
            SkinnedMeshRenderer mr = GetComponentInChildren<SkinnedMeshRenderer>();

            if (mr == null)
            {
                Debug.LogError("MeshRenderer�� �������� �ʽ��ϴ�.");
                yield break;
            }

            Color originalColor = mr.material.color;
            mr.material.color = new Color(1f, 0f, 0f, 0.5f);
            yield return new WaitForSeconds(0.1f);
            mr.material.color = originalColor;
        }

        private void Die()
        {
            if (gameObject != null)
            {
                GameManager.Instance.AddLevelUpProgress(10f);

                if (Random.value <= healthRecoveryChance && healthRecoveryPrefab != null)
                {
                    Vector3 itemPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
                    Instantiate(healthRecoveryPrefab, itemPosition, Quaternion.identity);
                }

                if (healthBarInstance != null)
                {
                    Destroy(healthBarInstance);
                }
                //���̺� �Ŵ��� �� ���üũ
                WaveManager.Instance.EnemyDefeated(gameObject);


                Destroy(gameObject);
            }
        }
        #endregion

        #region �̵� ���� �Լ�
        private bool MaintainDistanceFromPlayer()
        {
            if (player == null)
            {
                Debug.LogWarning("Player Transform�� �������� �ʾҽ��ϴ�.");
                return false;
            }

            Vector3 directionToPlayer = player.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer < minDistance)
            {
                MoveInDirection(-directionToPlayer);
                return true;
            }
            else if (distanceToPlayer > maxDistance)
            {
                MoveInDirection(directionToPlayer);
                return true;
            }
            Debug.Log("Test");
            return false;
        }

        private void MoveInDirection(Vector3 direction)
        {
            Vector3 moveDirection = direction.normalized;
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

            if (mapObject != null)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, mapBounds.min.x, mapBounds.max.x);
                newPosition.z = Mathf.Clamp(newPosition.z, mapBounds.min.z, mapBounds.max.z);
            }

            newPosition.y = transform.position.y;  // ���̰� ����

            transform.position = newPosition;
            lastMoveDirection = moveDirection;
        }
        #endregion

        #region ��� ���� �Լ�
        private bool Shooting()
        {
            if (Time.time >= nextFireTime && bulletPrefab != null && player != null)
            {
                Fire();
                nextFireTime = Time.time + fireRate;
                return true;
            }

            return false;
        }

        private void Fire()
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 bulletPosition= new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            GameObject bullet = Instantiate(bulletPrefab, bulletPosition + lastMoveDirection, Quaternion.LookRotation(directionToPlayer));
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            Bullet bull = bullet.GetComponent<Bullet>();

            if (bull != null)
            {
                bull.SetBulletProperties(power, this.tag);
            }

            

            if (bulletRb != null)
            {
                bulletRb.velocity = directionToPlayer * bulletSpeed;
            }
        }
        #endregion

        #region ü�¹� ���� �Լ�
        private void UpdateHealthBarPosition()
        {
            if (healthBarInstance != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.down * 1.5f);
                healthBarTransform.position = screenPosition;
            }
        }

        #endregion
    }
}
