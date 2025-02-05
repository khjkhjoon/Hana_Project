using System.Collections;
using System.Collections.Generic;
using Hana.KHJ;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hana.Common
{
    public enum GameState { Start, Playing, GameOver, Ending }

    public class GameManager : Singleton<GameManager>
    {
        #region 참조
        public Player player;
        #endregion
        private float levelUpProgress = 0f; // 레벨업 진행도
        private float levelUpMax = 100f; // 레벨업 게이지 최대값

        #region 사운드 설정
        public AudioSource audioSource; //오디오 소스
        public AudioClip levelupSound; //레벨업 사운드
        public AudioClip gameoverSound; //게임오버 사운드
        public AudioClip clearSound; //게임클리어 사운드
        #endregion

        public GameState CurrentState { get; private set; } = GameState.Start;

        [SerializeField] private GameObject startScreen; // 시작 화면 UI
        [SerializeField] private GameObject gameOverScreen; // 게임 오버 UI
        [SerializeField] private GameObject endingScreen; // 엔딩 화면 UI
        [SerializeField] private GameObject replayButton; // 리플레이 버튼

        private void Start()
        {
            // 게임 시작 시 초기 체력 설정 및 UI 업데이트
            SetState(GameState.Start);
            UIManager.Instance.UpdateUIState();
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return; // 중복 실행 방지

            // 게임 상태 변경 및 UI 업데이트
            CurrentState = newState;
            UIManager.Instance.UpdateUIState();
        }

        public void StartGame()
        {
            // 게임 시작 - 플레이 상태로 전환
            SetState(GameState.Playing);

            WaveManager.Instance.StartNextWave();
        }

        public void AddLevelUpProgress(float amount)
        {
            // 경험치를 획득하여 레벨업 진행도를 증가
            if (CurrentState != GameState.Playing) return;

            levelUpProgress += amount;
            UIManager.Instance.UpdateLevelUpSlider(levelUpProgress, levelUpMax);

            if (levelUpProgress >= levelUpMax)
            {
                LevelUp(); // 레벨업 조건 충족 시 레벨업 실행
            }
        }

        private void LevelUp()
        {
            audioSource.PlayOneShot(levelupSound);
            Time.timeScale = 0f; // 게임 멈추기

            // 레벨업 UI 활성화 및 진행도 초기화
            UIManager.Instance.ShowLevelUpUI();

            // 스킬 선택 버튼을 설정하여 스킬 선택 시 적용되도록 설정
            UIManager.Instance.SetupSkillButtons(ApplySelectedSkill);
        }

        public void ApplySelectedSkill(string skillType)
        {
            // 선택된 스킬을 플레이어에게 적용
            SkillManager.Instance.ApplySkill(skillType);

            // 스킬 패널을 비활성화
            UIManager.Instance.HideLevelUpUI();

            // 레벨업 게이지 초기화
            levelUpProgress = 0;
            UIManager.Instance.UpdateLevelUpSlider(levelUpProgress, levelUpMax);

            Time.timeScale = 1f; // 게임 다시 시작
        }

        public void ResetLevelUpProgress()
        {
            levelUpProgress = 0;
            UIManager.Instance.UpdateLevelUpSlider(levelUpProgress, levelUpMax);
        }

        public void GameOver()
        {
            if (CurrentState == GameState.GameOver) return; // 중복 호출 방지

            audioSource.PlayOneShot(gameoverSound);

            // 게임 오버 상태로 변경
            SetState(GameState.GameOver);

            Time.timeScale = 0f; // 게임 정지
        }

        public void GameEnding()
        {
            audioSource.PlayOneShot(clearSound);
            // 엔딩 상태로 변경 (최종 보스를 처치했을 때 호출)
            SetState(GameState.Ending);
        }

        public void ReplayGame()
        {
            // 게임을 재시작하는 기능
            SetState(GameState.Playing);
            Time.timeScale = 1f; // 게임 다시 시작
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
