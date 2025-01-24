using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hana.KHJ
{
    public class Enemy : MonoBehaviour
    {
        #region 기본 스탯 관련 변수
        public float power = 2f; 
        [SerializeField] private float maxHealth = 10f; // 최대 체력
        private float currentHealth; // 현재 체력
        #endregion

        #region 거리 유지 관련 변수
        public Transform player; // Player의 Transform
        public float minDistance = 3f; // Player와의 최소 거리
        public float maxDistance = 5f; // Player와의 최대 거리
        public float moveSpeed = 2f; // Enemy 이동 속도
        public Vector2 mapBoundsMin = new Vector2(-10f, -10f); // 맵 최소 좌표
        public Vector2 mapBoundsMax = new Vector2(10f, 10f);   // 맵 최대 좌표
        #endregion

        #region 사격 관련 변수
        [Header("사격 설정")]
        [SerializeField] private GameObject bulletPrefab; // 총알 프리팹
        [SerializeField] private float bulletSpeed = 10f; // 총알 속도
        [SerializeField] private float fireRate = 2f;     // 발사 간격 (초 단위)
        private float nextFireTime = 0f;                 // 다음 발사 가능 시간
        private Vector3 lastMoveDirection;               // 최근 이동 방향
        #endregion

        #region Unity 기본 함수
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

        #region 체력 관리 함수
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

            // 원래 색상 저장
            Color originalColor = mr.material.color;

            // 빨간색으로 변경
            Color redColor = new Color(1f, 0f, 0f, 0.5f);
            mr.material.color = redColor;

            // 잠시 대기
            yield return new WaitForSeconds(0.1f);

            // 색 복원
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

        #region 이동 관련 함수
        private void MaintainDistanceFromPlayer()
        {
            if (player == null)
            {
                Debug.LogWarning("Player Transform이 설정되지 않았습니다.");
                return;
            }

            Vector3 directionToPlayer = player.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer < minDistance)
            {
                // Player로부터 멀어짐
                MoveInDirection(-directionToPlayer);
            }
            else if (distanceToPlayer > maxDistance)
            {
                // Player에게 가까워짐
                MoveInDirection(directionToPlayer);
            }

            // minDistance <= distanceToPlayer <= maxDistance 일 때는 가만히 있음
        }

        private void MoveInDirection(Vector3 direction)
        {
            // 이동 방향 설정
            Vector3 moveDirection = direction.normalized;

            // 새 위치 계산
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

            // 맵 경계 내 이동 제한
            newPosition.x = Mathf.Clamp(newPosition.x, mapBoundsMin.x, mapBoundsMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, mapBoundsMin.y, mapBoundsMax.y);

            // 위치 업데이트
            transform.position = newPosition;

            // 최근 이동 방향 저장
            lastMoveDirection = moveDirection;
        }
        #endregion

        #region 사격 관련 함수
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
            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, transform.position + lastMoveDirection, Quaternion.identity);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            Bullet bull = bullet.GetComponent<Bullet>();

            // 총알 속성 설정
            if (bull != null)
            {
                bull.SetBulletProperties(power, this.tag); // 데미지와 태그 전달
            }

            // 플레이어를 향한 방향 계산
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // 총알의 속도 설정
            if (bulletRb != null)
            {
                bulletRb.velocity = directionToPlayer * bulletSpeed;
            }
        }
        #endregion
    }
}
