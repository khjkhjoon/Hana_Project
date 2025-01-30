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
        #region 기본 스텟 관련
        public float maxHealth = 10f;
        public float currentHealth;
        public float Damage;
        #endregion

        #region 이동 관련 변수
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public Joystick joystick; // 조이스틱 스크립트 참조

        private float hAxis;
        private float vAxis;

        private Vector3 moveDirection;
        private Vector3 lastmoveDirection;

        //private Rigidbody rb; // 3D 물리 처리를 위한 Rigidbody
        private Animator animator; // 애니메이터 추가
        #endregion

        #region 사격 관련 변수
        public float bulletSpeed = 20f;
        public float fireRate = 0.5f;
        public GameObject bulletPrefab;
        public GameObject crosshairUI; // 크로스헤어 UI 오브젝트
        private RectTransform crosshairRectTransform; // UI RectTransform

        private float nextFireTime = 1f;
        #endregion

        #region 체력바 관련 변수
        public Slider healthBar;
        #endregion

        #region 사운드 관련 변수
        public AudioSource audioSource; // 오디오 소스 컴포넌트
        public AudioClip[] damageSounds; // 데미지 사운드 배열
        public AudioClip[] deathSounds; // 죽을 때 사운드 배열
        #endregion

        private GameObject nearestEnemy; // 가장 가까운 적 저장

        public GameManager gameManager;      // 게임 매니저 참조
        public SkillManager skillManager;    // 스킬 매니저 참조

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

        #region 유니티 기본 함수
        void Start()
        {
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;
            Damage = 5f;

            if (crosshairUI != null)
            {
                crosshairRectTransform = crosshairUI.GetComponent<RectTransform>();
                crosshairUI.SetActive(false); // 처음에는 비활성화
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

        #region 이동관련 함수

        void MovePlayer()
        {
            Vector2 input = joystick.GetInput();

            // 3D 이동 벡터 계산
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

                // Y축 회전만 적용
                transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, currentRotation.eulerAngles.z);
            }
        }
        #endregion

        #region 플레이어 사격 관련 함수
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
             //총알 생성
            GameObject bullet = Instantiate(bulletPrefab, transform.position + lastmoveDirection, Quaternion.identity);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            Bullet bull = bullet.GetComponent<Bullet>();
            if (bull != null)
                bull.SetBulletProperties(Damage,this.tag);

            // 적을 향한 방향 계산
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
                UpdateCrosshairUI(nearestEnemy.transform.position); // 적의 위치에 UI를 업데이트
                crosshairUI.SetActive(true);
            }
            else
            {
                crosshairUI.SetActive(false); // 적이 없으면 UI 비활성화
            }
        }

        void UpdateCrosshairUI(Vector3 enemyWorldPosition)
        {
            if (crosshairRectTransform != null)
            {
                // 월드 좌표를 스크린 좌표로 변환
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemyWorldPosition);

                // 크로스헤어 RectTransform의 위치 업데이트
                crosshairRectTransform.position = screenPosition;
            }
        }
        #endregion

        #region 데미지 및 사망처리
        public void TakeDamage(float damage)
        {
            // 데미지 받기
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            PlayRandomSound(damageSounds); // 데미지 사운드 재생

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

        #region 체력바 관련 함수
        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
                Debug.Log($"currentHealth = {currentHealth}");
            }
        }

        #endregion

        #region 사운드 관련 함수
        private void PlayRandomSound(AudioClip[] soundArray)
        {
            if (soundArray != null && soundArray.Length > 0 && audioSource != null)
            {
                int randomIndex = Random.Range(0, soundArray.Length); // 랜덤 인덱스 선택
                audioSource.PlayOneShot(soundArray[randomIndex]); // 사운드 재생
            }
        }
        #endregion
    }
}