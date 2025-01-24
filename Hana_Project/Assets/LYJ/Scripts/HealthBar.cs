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
        public Slider healthBarSlider; // UI 슬라이더
        private Player player; // 플레이어 컨트롤러 참조

        private void Start()
        {
            // PlayerController 찾기
            player = GetComponent<Player>();

            if (player == null)
            {
                Debug.LogError("player를 찾을 수 없습니다.");
                return;
            }

            // 체력바 초기화
            healthBarSlider.maxValue = player.maxHealth;
            healthBarSlider.value = player.currentHealth;
        }

        private void Update()
        {
            if (player != null)
            {
                // 플레이어 체력에 따라 체력바 업데이트
                healthBarSlider.value = player.currentHealth;
            }
        }
    }
}