using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hana.LYJ;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Hana.Common
{
    public class UIManager : Singleton<UIManager>
    {

        public AudioSource audioSource; // 버튼 클릭 사운드를 재생할 AudioSource
        public AudioClip clickSound;    // 버튼 클릭 소리 클립

        [SerializeField] private TextMeshProUGUI waveText; // 웨이브 텍스트 UI
        [SerializeField] private Slider healthSlider; // 플레이어 체력 UI
        [SerializeField] private Slider levelUpSlider; // 레벨업 게이지 UI
        [SerializeField] private GameObject skillPanel; // 스킬 선택 패널 UI
        [SerializeField] private Button[] skillButtons; // 스킬 버튼 배열
        [SerializeField] private Joystick joystick; // 플레이어 이동을 위한 조이스틱
        [SerializeField] private Button attackButton; // 공격 버튼
        [SerializeField] private Button optionButton; //옵션 버튼

        [SerializeField] private GameObject startScreen; // 시작 화면 UI
        [SerializeField] private GameObject gameOverScreen; // 게임 오버 화면 UI
        [SerializeField] private GameObject endingScreen; // 엔딩 화면 UI
        [SerializeField] private GameObject optionScreen; //옵션 화면 UI
        private Button replayButton; // 리플레이 버튼 참조

        private void Start()
        {
            UpdateUIState();

            // 리플레이 버튼 이벤트 등록
            if (replayButton != null)
            {
                replayButton.onClick.AddListener(OnReplayButtonClick);
            }
        }

        public void UpdateUIState()
        {
            if (startScreen != null)
            {
                startScreen.SetActive(GameManager.Instance.CurrentState == GameState.Start);
                Debug.Log("테스트 스타트스크린");
            }
            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(GameManager.Instance.CurrentState == GameState.GameOver);
                Debug.Log("테스트 게임오버스크린");
            }
            if (endingScreen != null)
            {
                endingScreen.SetActive(GameManager.Instance.CurrentState == GameState.Ending);
                Debug.Log("테스트 엔딩스크린");
            }
            if (optionScreen != null)
            {
                optionScreen.SetActive(GameManager.Instance.CurrentState == GameState.Pause);
                Debug.Log("테스트 옵션스크린");
            }
        }

        public Joystick GetJoystick()
        {
            // 현재 조이스틱 객체를 반환
            return joystick;
        }

        public void SetupAttackButton(System.Action onAttack)
        {
            // 공격 버튼을 클릭하면 onAttack 이벤트 실행
            attackButton.onClick.AddListener(() => onAttack?.Invoke());
        }

        public void UpdateWaveText(int waveNumber)
        {
            waveText.text = $"Wave {waveNumber}";
        }

        public void UpdateBossText()
        {
            waveText.text = "Boss";
        }


        public void UpdateHealthSlider(float currentHealth, float maxHealth)
        {
            healthSlider.value = Mathf.Clamp(currentHealth / maxHealth, 0f, 1f);
        }

        public void UpdateLevelUpSlider(float currentValue, float maxValue)
        {
            // 레벨업 게이지 UI 업데이트
            levelUpSlider.value = currentValue / maxValue;
        }

        public void ShowLevelUpUI()
        {
            // 레벨업 UI 활성화
            skillPanel.SetActive(true);
        }

        public void HideLevelUpUI()
        {
            // 레벨업 UI 비활성화
            skillPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void ShowGameOverScreen()
        {
            // 게임 오버 화면 표시
            gameOverScreen.SetActive(true);
        }

        public void OnReplayButtonClick()
        {
            // 게임 상태 초기화
            GameManager.Instance.ReplayGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ShowStartScreen()
        {
            // 시작 화면 표시
            startScreen.SetActive(true);
        }

        public void SetupSkillButtons(System.Action<string> onSkillSelected)
        {
            for (int i = 0; i < skillButtons.Length; i++)
            {
                int index = i; // 버튼별로 개별 인덱스를 유지
                skillButtons[i].onClick.RemoveAllListeners(); // 기존 이벤트 제거
                skillButtons[i].onClick.AddListener(() => onSkillSelected(skillButtons[index].name)); // 클릭 시 이벤트 추가
            }
        }

        public void ShowOptionScreen()
        {
            // 게임 오버 화면 표시
            optionScreen.SetActive(true);
        }

        public void OnContinueButtonClicked()
        {
            optionScreen.SetActive(false);
            GameManager.Instance.PlayGame();
        }

        public void OnQuitButtonClicked()
        {
            GameManager.Instance.QuitGame();
        }

        public void PlayClickSound()
        {
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound); // 클릭 소리 재생
            }
        }
    }
}