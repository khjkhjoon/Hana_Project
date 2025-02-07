using System.Collections;
using Hana.Common;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Hana.KHJ
{
    public class Boss : MonoBehaviour
    {
        #region 참조 관련
        Player player;
        #endregion
        #region 기본 스텟
        public float maxHealth = 300;
        public float currentHealth;
        [SerializeField]
        private float power = 2;
        #endregion
        #region 패턴 관련
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float fireRate = 5f;
        public int bulletCount = 10;
        public float bulletSpeed = 5f;

        public GameObject warningPrefab;
        public GameObject fallingObjectPrefab;
        public float warningDuration = 2f;

        private Animator animator;
        private bool isFiring = false;
        #endregion
        #region 체력바
        [SerializeField] private GameObject healthBarPrefab;
        private GameObject healthBarInstance;
        private RectTransform healthBarTransform;
        #endregion
        #region 사운드
        private AudioSource audiosource;
        public AudioClip hittingSound;
        public AudioClip bulletSound;
        public AudioClip fallingSound;
        #endregion
        void Start()
        {
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();
            audiosource = GetComponent<AudioSource>();

            if (player == null)
            {
                GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
                player = foundPlayer?.GetComponent<Player>();
            }

            // HealthBar 설정
            if (healthBarPrefab != null)
            {
                GameObject canvas = GameObject.Find("Canvas");  // UI를 표시할 캔버스 찾기
                if (canvas != null)
                {
                    healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
                    healthBarTransform = healthBarInstance.GetComponent<RectTransform>();
                }
            }

            StartCoroutine(RandomPatternRoutine());
        }
        void Update()
        {
            UpdateHealthBarPosition();
        }
        IEnumerator RandomPatternRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(3f, 6f));

                int pattern = Random.Range(0, 2);
                if (pattern == 0)
                {
                    StartCoroutine(FirePatternRoutine());
                }
                else
                {
                    TriggerFallingAttack();
                }
            }
        }
        void UpdateHealthBarPosition()
        {
            if (healthBarTransform != null)
            {
                // 보스의 월드 좌표를 화면 좌표로 변환
                Vector3 worldPosition = transform.position + new Vector3(0, -1f, 0);  // Y축 기준으로 발 밑 위치
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

                // 체력바가 화면 안에 있을 때만 위치 업데이트
                if (screenPosition.z > 0)
                {
                    healthBarTransform.position = screenPosition;
                }
                else
                {
                    healthBarTransform.position = new Vector3(-1000, -1000, 0);  // 화면 밖으로 숨기기
                }
            }
        }
        #region 패턴 1
        IEnumerator FirePatternRoutine()
        {
            if (!isFiring)
            {
                isFiring = true;
                animator.SetTrigger("Call");
                yield return new WaitForSeconds(0.8f);

                FireBulletPattern(); // 첫 실행 때도 총알 발사되도록 수정

                yield return new WaitForSeconds(fireRate);
                isFiring = false;
            }
        }

        void FireBulletPattern()
        {
            float angleStep = 360f / bulletCount;
            float angle = Time.time * 100f; // 첫 발사 시에도 랜덤한 회전값 적용

            for (int i = 0; i < bulletCount; i++)
            {
                float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
                float bulletDirZ = Mathf.Sin(angle * Mathf.Deg2Rad);
                Vector3 bulletMoveDirection = new Vector3(bulletDirX, 0, bulletDirZ).normalized;

                audiosource.PlayOneShot(bulletSound);

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();



                Bullet bull = bullet.GetComponent<Bullet>();
                if (bull != null)
                {
                    bull.SetBulletProperties(power, this.tag);
                }
                if (bulletRb != null)
                {
                    bulletRb.velocity = bulletMoveDirection * bulletSpeed;
                }
                else
                {
                    Debug.LogError("Bullet Rigidbody is missing!");
                }

                angle += angleStep;
            }
        }
        #endregion

        #region 패턴 2
        void TriggerFallingAttack()
        {
            if (player == null) return;

            Vector3 playerPosition = player.transform.position;
            Vector3 warningPosition = playerPosition + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));

            GameObject warning = Instantiate(warningPrefab, warningPosition, Quaternion.Euler(90, 0, 0));
            audiosource.PlayOneShot(fallingSound);
            Destroy(warning, warningDuration);

            StartCoroutine(SpawnFallingObjectAfterDelay(warningPosition, warningDuration));
        }

        IEnumerator SpawnFallingObjectAfterDelay(Vector3 position, float delay)
        {
            yield return new WaitForSeconds(delay);

            GameObject fallingObject = Instantiate(fallingObjectPrefab, position + new Vector3(0, 10f, 0), Quaternion.identity);
            Rigidbody fallingRb = fallingObject.GetComponent<Rigidbody>();
            if (fallingRb != null)
            {
                fallingRb.velocity = new Vector3(0, -10f, 0);
            }
            else
            {
                Debug.LogError("Falling object Rigidbody is missing!");
            }

            Debug.Log("낙하 공격 발생!");
        }
        #endregion
        #region 체력 관련 함수

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

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            Debug.Log($"Boss Health: {currentHealth}");
            if (currentHealth <= 0)
            {
                OnDestroy();
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

        private void OnDestroy()
        {
            if (gameObject != null)
            {
                GameManager.Instance.AddLevelUpProgress(10f);
                //audiosource.PlayOneShot(deathSound);
                if (healthBarInstance != null)
                {
                    Destroy(healthBarInstance);
                }
                GameManager.Instance.GameEnding();
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
