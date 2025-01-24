using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.KHJ
{
    public class Bullet : MonoBehaviour
    {
        #region 수치 관련 변수

        private float damage;
        private string shooterTag; // 발사자 태그 (Player, Enemy 등)
        private Vector3 spawnPosition; // 총알이 생성된 위치
        private float maxDistance = 20f; // 사라질 최대 거리

        #endregion

        private void Start()
        {
            // 총알 생성 시 위치 저장
            spawnPosition = transform.position;
        }

        private void Update()
        {
            // 총알이 일정 거리 이상 이동했는지 확인
            float distance = Vector3.Distance(spawnPosition, transform.position);
            if (distance > maxDistance)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 총알의 데미지와 발사자 태그를 설정하는 함수
        /// </summary>
        /// <param name="amount">데미지 양</param>
        /// <param name="tag">발사자 태그</param>
        public void SetBulletProperties(float amount, string tag)
        {
            damage = amount;
            shooterTag = tag;
        }

        private void OnTriggerEnter(Collider other)
        {
            // 발사자와 같은 태그를 가진 객체는 무시
            if (other.CompareTag(shooterTag)) return;

            // 적 태그에 따라 데미지 적용
            if (other.CompareTag("Enemy") && shooterTag == "Player")
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            else if (other.CompareTag("Player") && shooterTag == "Enemy")
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(damage); // Player 클래스에 TakeDamage 메서드 필요
                }
            }

            // 총알 제거
            Destroy(gameObject);
        }
    }
}
