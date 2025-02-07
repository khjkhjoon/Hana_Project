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
        public float Damage = 5f;
        #endregion

        #region 이동 관련 변수
        private Rigidbody rb;
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
        public GameObject arrowObject;

        private RectTransform crosshairRectTransform; // UI RectTransform
        private float nextFireTime = 1f;
        #endregion

        #region 체력바 관련 변수
        public Slider healthBar;
        #endregion

        #region 사운드 관련 변수
        public AudioSource audioSource; // 오디오 소스 컴포넌트
        public AudioClip attackSound; // 공격 사운드
        public AudioClip healSound; //체력 회복 사운드
        public AudioClip[] damageSounds; // 데미지 사운드 배열
        public AudioClip[] deathSounds; // 죽을 때 사운드 배열
        #endregion

        private GameObject nearestEnemy; // 가장 가까운 적 저장

        public GameManager gameManager;      // 게임 매니저 참조
        public SkillManager skillManager;    // 스킬 매니저 참조

        #region 유니티 기본 함수
        void Start()
        {
            rb = GetComponent<Rigidbody>(); 
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;

            if (crosshairUI != null)
            {
                crosshairRectTransform = crosshairUI.GetComponent<RectTransform>();
                crosshairUI.SetActive(false); // 처음에는 비활성화
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

        #region 이동관련 함수

        void MovePlayer()
        {
            Vector2 input = joystick.GetInput();

            // 3D 이동 벡터 계산
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
            //공격 사운드 재생
            audioSource.PlayOneShot(attackSound);

            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, transform.position + lastmoveDirection, Quaternion.LookRotation(lastmoveDirection));
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            Bullet bull = bullet.GetComponent<Bullet>();

            if (bull != null)
                bull.SetBulletProperties(Damage, this.tag);

            // 적을 향한 방향 계산
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
            UpdateArrowDirection();  // 화살표 방향 업데이트
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

        void UpdateArrowDirection()
        {
            if (nearestEnemy != null && arrowObject != null)
            {
                // 플레이어의 위치
                Vector3 playerPosition = transform.position;

                // 적의 방향 계산 (플레이어와 적 사이의 벡터)
                Vector3 directionToEnemy = nearestEnemy.transform.position - playerPosition;

                // 방향을 정규화하여 일정 거리만큼만 이동하도록 설정
                Vector3 arrowDirection = directionToEnemy.normalized;

                float maxDistance = 100.0f;
                Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
                Vector3 targetPosition = playerScreenPosition + new Vector3(arrowDirection.x, arrowDirection.y, 0) * maxDistance;
                arrowObject.GetComponent<RectTransform>().position = targetPosition;
                // Y값을 0으로 설정하여 수평 평면에서만 회전하도록 한다.
                directionToEnemy.y = 0;

                // 방향이 0벡터가 아닌 경우에만 회전
                if (directionToEnemy.magnitude > 0.1f)
                {
                    // Z축을 기준으로 회전 (Y 축은 무시)
                    float angle = Mathf.Atan2(directionToEnemy.z, directionToEnemy.x) * Mathf.Rad2Deg;

                    // 화살표의 회전을 Z축을 기준으로 적용
                    arrowObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
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
            GameManager.Instance.GameOver();  // 게임 오버 호출
        }
        #endregion

        #region 체력 관련 함수
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

        #region 스킬 관련 함수
        public void IncreaseAttackPower()
        {
            Damage += 5;
            Debug.Log("공격력 증가! 현재 공격력: " + Damage);
        }

        public void IncreaseAttackSpeed()
        {
            fireRate *= 0.9f; // 공격 속도 증가 (딜레이 감소)
            Debug.Log("공격 속도 증가! 현재 공격 딜레이: " + fireRate);
        }

        public void IncreaseMoveSpeed()
        {
            moveSpeed += 1.5f;
            Debug.Log("이동 속도 증가! 현재 이동 속도: " + moveSpeed);
        }
        #endregion
    }
}