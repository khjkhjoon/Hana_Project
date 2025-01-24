using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.KHJ
{
    public class Bullet : MonoBehaviour
    {
        #region ��ġ ���� ����

        private float damage;
        private string shooterTag; // �߻��� �±� (Player, Enemy ��)
        private Vector3 spawnPosition; // �Ѿ��� ������ ��ġ
        private float maxDistance = 20f; // ����� �ִ� �Ÿ�

        #endregion

        private void Start()
        {
            // �Ѿ� ���� �� ��ġ ����
            spawnPosition = transform.position;
        }

        private void Update()
        {
            // �Ѿ��� ���� �Ÿ� �̻� �̵��ߴ��� Ȯ��
            float distance = Vector3.Distance(spawnPosition, transform.position);
            if (distance > maxDistance)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// �Ѿ��� �������� �߻��� �±׸� �����ϴ� �Լ�
        /// </summary>
        /// <param name="amount">������ ��</param>
        /// <param name="tag">�߻��� �±�</param>
        public void SetBulletProperties(float amount, string tag)
        {
            damage = amount;
            shooterTag = tag;
        }

        private void OnTriggerEnter(Collider other)
        {
            // �߻��ڿ� ���� �±׸� ���� ��ü�� ����
            if (other.CompareTag(shooterTag)) return;

            // �� �±׿� ���� ������ ����
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
                    player.TakeDamage(damage); // Player Ŭ������ TakeDamage �޼��� �ʿ�
                }
            }

            // �Ѿ� ����
            Destroy(gameObject);
        }
    }
}
