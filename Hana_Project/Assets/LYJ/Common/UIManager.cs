using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hana.Common
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Text waveText;        // 현재 웨이브를 표시하는 텍스트 UI
        [SerializeField] private Slider healthSlider;  // 플레이어 체력을 표시하는 슬라이더 UI
        [SerializeField] private Slider levelUpSlider; // 레벨업 게이지를 표시하는 슬라이더 UI
        [SerializeField] private GameObject startScreen;      // 시작 화면 UI
        [SerializeField] private GameObject gameOverScreen;   // 게임 오버 화면 UI
        [SerializeField] private GameObject endingScreen;     // 엔딩 화면 UI
        [SerializeField] private GameObject skillPanel;       // 스킬 선택 패널 UI
        [SerializeField] private Button[] skillButtons;       // 스킬 선택 버튼들

        public void UpdateWaveText(int wave)
        {
            // 웨이브 텍스트 업데이트
        }
        public void UpdateHealthSlider(float currentHealth, float maxHealth)
        {
            // 체력 슬라이더 업데이트
        }
        public void UpdateLevelUpSlider(float currentValue, float maxValue)
        {
            // 레벨업 슬라이더 업데이트
        }
        public void ShowStartScreen()
        {
            // 시작 화면 표시
        }
        public void ShowGameOverScreen()
        {
            // 게임 오버 화면 표시
        }
        public void ShowEndingScreen()
        {
            // 엔딩 화면 표시
        }
        public void ShowSkillPanel(string[] skillNames)
        {
            // 스킬 선택 화면 표시
        }
        public void HideSkillPanel()
        {
            // 스킬 선택 화면 숨기기
        }
    }
}