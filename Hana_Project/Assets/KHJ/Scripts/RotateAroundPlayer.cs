using UnityEngine;

public class RotateAroundPlayer : MonoBehaviour
{
    public Transform player;  // 플레이어 Transform
    public float rotationSpeed = 100f; // 회전 속도
    public float detectionRange = 10f; // 적 탐지 범위
    private Transform nearestEnemy; // 가장 가까운 적

    public float moveDistance = 1f; // 이동할 거리 (1칸)

    void Update()
    {
        if (player != null)
        {
            // 플레이어 주변에서 Y축을 기준으로 회전
            transform.RotateAround(player.position, Vector3.up, rotationSpeed * Time.deltaTime);

            // 가장 가까운 적 찾기
            FindNearestEnemy();

            // 가장 가까운 적의 방향을 가리키도록 회전 및 이동
            if (nearestEnemy != null)
            {
                // 적 방향을 향한 단위 벡터 계산
                Vector3 directionToEnemy = (nearestEnemy.position - transform.position).normalized;

                // Z축 기준으로 회전 (가장 가까운 적의 방향을 향하도록)
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToEnemy.x, 0, directionToEnemy.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 적 방향으로 1칸 이동
                transform.Translate(directionToEnemy * moveDistance, Space.World);
            }
        }
    }

    // 가장 가까운 적 찾기
    void FindNearestEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, detectionRange);  // 탐지 범위 내의 적들
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
                    nearestEnemy = enemy.transform;
                }
            }
        }
    }
}
