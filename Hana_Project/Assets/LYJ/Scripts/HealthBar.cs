using System.Collections;
using System.Collections.Generic;
using Hana.KHJ;
using UnityEngine;
using UnityEngine.UI;

namespace Hana.LYJ
{
    public class HealthBarController : MonoBehaviour
    {
        [Header("Health Bar Settings")]
        public Slider healthBarSlider; // UI �����̴�
        private Player player; // �÷��̾� ��Ʈ�ѷ� ����

        private void Start()
        {
            // PlayerController ã��
            player = GetComponent<Player>();

            if (player == null)
            {
                Debug.LogError("player�� ã�� �� �����ϴ�.");
                return;
            }

            // ü�¹� �ʱ�ȭ
            healthBarSlider.maxValue = player.maxHealth;
            healthBarSlider.value = player.currentHealth;
        }

        private void Update()
        {
            if (player != null)
            {
                // �÷��̾� ü�¿� ���� ü�¹� ������Ʈ
                healthBarSlider.value = player.currentHealth;
            }
        }
    }
}