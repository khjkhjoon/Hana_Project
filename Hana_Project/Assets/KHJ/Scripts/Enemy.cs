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
        #region 기본 스탯 관련 변수
        public float power = 2f;
        [SerializeField] private float maxHealth = 10f;
        private float currentHealth;
        #endregion

        #region 거리 유지 관련 변수
        public Transform player;
        public float minDistance = 3f;
        public float maxDistance = 5f;
        public float moveSpeed = 2f;

        [Header("맵 제한 설정")]
        public GameObject mapObject;  // 맵 오브젝트 참조
        private Bounds mapBounds;     // 맵 경계
        #endregion

        #region 사격 관련 변수
        [Header("사격 설정")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float fireRate = 2f;
        private float nextFireTime = 0f;
        private Vector3 lastMoveDirection;
        #endregion

        #region 애니메이션 관련 변수
        private Animator animator;
        private bool isRunning = false;
        private bool isAttacking = false;
        #endregion

        #region 체력바 관련
        [SerializeField] private GameObject healthBarPrefab;
        private GameObject healthBarInstance;
        private RectTransform healthBarTransform;

        #endregion

        #region 사운드 설정
        public AudioSource audiosource;
        public AudioClip hittingSound; //타격음
        #endregion

        #region 드롭 아이템
        [SerializeField] 
        private GameObject healthRecoveryPrefab;
        [SerializeField]
        private float healthRecoveryChance = 0.1f; //드롭 확률
        #endregion


        #region Unity 기본 함수
        private void Start()
        {
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();

            //Player 자동 할당
            if (player == null)
            {
                // 게임 오버 시 적 동작 정지
                if (GameManager.Instance.CurrentState == GameState.GameOver) return;

                GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
                if (foundPlayer != null)
                {
                    player = foundPlayer.transform;
                }
                else
                {
                    Debug.LogError("Player 오브젝트를 찾을 수 없습니다. 'Player' 태그가 설정되어 있는지 확인하세요.");
                    Application.Quit();
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }
            //HealthBar 설정
            if (healthBarPrefab != null)
            {
                GameObject canvas = GameObject.Find("Canvas");  // UI를 표시할 캔버스 찾기
                if (canvas != null)
                {
                    healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
                    healthBarTransform = healthBarInstance.GetComponent<RectTransform>();
                }
            }
            // 맵 경계 설정
            if (mapObject != null)
            {
                if (mapObject == null)
                {
                    mapObject = GameObject.FindGameObjectWithTag("Ground");
                    if (mapObject == null)
                    {
                        Debug.LogError("맵 오브젝트를 찾을 수 없습니다.");
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
                    Debug.LogError("맵 오브젝트에 Collider가 없습니다.");
                    Application.Quit();
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }
            else
            {
                //Debug.LogError("맵 오브젝트가 설정되지 않았습니다.");
            }
        }

        private void Update()
        {
            // 게임 오버 시 적 동작 정지
            if (GameManager.Instance.CurrentState == GameState.GameOver) return;

            bool moving = MaintainDistanceFromPlayer();
            bool attacking = Shooting();

            // 플레이어 바라보기 (Y축 회전만 반영)
            if (player != null)
            {
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;  // Y축 회전 방지
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }

            // 애니메이션 상태 업데이트
            if (animator != null)
            {
                animator.SetBool("isRunning", moving);
                animator.SetBool("isAttacking", attacking);
            }

            UpdateHealthBarPosition();
        }
        #endregion

        #region 체력 관리 함수
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
                Debug.LogError("MeshRenderer가 존재하지 않습니다.");
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
                //웨이브 매니저 적 사망체크
                WaveManager.Instance.EnemyDefeated(gameObject);


                Destroy(gameObject);
            }
        }
        #endregion

        #region 이동 관련 함수
        private bool MaintainDistanceFromPlayer()
        {
            if (player == null)
            {
                Debug.LogWarning("Player Transform이 설정되지 않았습니다.");
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

            newPosition.y = transform.position.y;  // 높이값 유지

            transform.position = newPosition;
            lastMoveDirection = moveDirection;
        }
        #endregion

        #region 사격 관련 함수
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

        #region 체력바 관련 함수
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
