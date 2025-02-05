using UnityEngine;

public class RotateAroundPlayer : MonoBehaviour
{
    public Transform player;  // �÷��̾� Transform
    public float rotationSpeed = 100f; // ȸ�� �ӵ�
    public float detectionRange = 10f; // �� Ž�� ����
    private Transform nearestEnemy; // ���� ����� ��

    public float moveDistance = 1f; // �̵��� �Ÿ� (1ĭ)

    void Update()
    {
        if (player != null)
        {
            // �÷��̾� �ֺ����� Y���� �������� ȸ��
            transform.RotateAround(player.position, Vector3.up, rotationSpeed * Time.deltaTime);

            // ���� ����� �� ã��
            FindNearestEnemy();

            // ���� ����� ���� ������ ����Ű���� ȸ�� �� �̵�
            if (nearestEnemy != null)
            {
                // �� ������ ���� ���� ���� ���
                Vector3 directionToEnemy = (nearestEnemy.position - transform.position).normalized;

                // Z�� �������� ȸ�� (���� ����� ���� ������ ���ϵ���)
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToEnemy.x, 0, directionToEnemy.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // �� �������� 1ĭ �̵�
                transform.Translate(directionToEnemy * moveDistance, Space.World);
            }
        }
    }

    // ���� ����� �� ã��
    void FindNearestEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, detectionRange);  // Ž�� ���� ���� ����
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
