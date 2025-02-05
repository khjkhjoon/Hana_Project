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
        #region ����
        public Player player;
        #endregion
        private float levelUpProgress = 0f; // ������ ���൵
        private float levelUpMax = 100f; // ������ ������ �ִ밪

        #region ���� ����
        public AudioSource audioSource; //����� �ҽ�
        public AudioClip levelupSound; //������ ����
        public AudioClip gameoverSound; //���ӿ��� ����
        public AudioClip clearSound; //����Ŭ���� ����
        #endregion

        public GameState CurrentState { get; private set; } = GameState.Start;

        [SerializeField] private GameObject startScreen; // ���� ȭ�� UI
        [SerializeField] private GameObject gameOverScreen; // ���� ���� UI
        [SerializeField] private GameObject endingScreen; // ���� ȭ�� UI
        [SerializeField] private GameObject replayButton; // ���÷��� ��ư

        private void Start()
        {
            // ���� ���� �� �ʱ� ü�� ���� �� UI ������Ʈ
            SetState(GameState.Start);
            UIManager.Instance.UpdateUIState();
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return; // �ߺ� ���� ����

            // ���� ���� ���� �� UI ������Ʈ
            CurrentState = newState;
            UIManager.Instance.UpdateUIState();
        }

        public void StartGame()
        {
            // ���� ���� - �÷��� ���·� ��ȯ
            SetState(GameState.Playing);

            WaveManager.Instance.StartNextWave();
        }

        public void AddLevelUpProgress(float amount)
        {
            // ����ġ�� ȹ���Ͽ� ������ ���൵�� ����
            if (CurrentState != GameState.Playing) return;

            levelUpProgress += amount;
            UIManager.Instance.UpdateLevelUpSlider(levelUpProgress, levelUpMax);

            if (levelUpProgress >= levelUpMax)
            {
                LevelUp(); // ������ ���� ���� �� ������ ����
            }
        }

        private void LevelUp()
        {
            audioSource.PlayOneShot(levelupSound);
            Time.timeScale = 0f; // ���� ���߱�

            // ������ UI Ȱ��ȭ �� ���൵ �ʱ�ȭ
            UIManager.Instance.ShowLevelUpUI();

            // ��ų ���� ��ư�� �����Ͽ� ��ų ���� �� ����ǵ��� ����
            UIManager.Instance.SetupSkillButtons(ApplySelectedSkill);
        }

        public void ApplySelectedSkill(string skillType)
        {
            // ���õ� ��ų�� �÷��̾�� ����
            SkillManager.Instance.ApplySkill(skillType);

            // ��ų �г��� ��Ȱ��ȭ
            UIManager.Instance.HideLevelUpUI();

            // ������ ������ �ʱ�ȭ
            levelUpProgress = 0;
            UIManager.Instance.UpdateLevelUpSlider(levelUpProgress, levelUpMax);

            Time.timeScale = 1f; // ���� �ٽ� ����
        }

        public void ResetLevelUpProgress()
        {
            levelUpProgress = 0;
            UIManager.Instance.UpdateLevelUpSlider(levelUpProgress, levelUpMax);
        }

        public void GameOver()
        {
            if (CurrentState == GameState.GameOver) return; // �ߺ� ȣ�� ����

            audioSource.PlayOneShot(gameoverSound);

            // ���� ���� ���·� ����
            SetState(GameState.GameOver);

            Time.timeScale = 0f; // ���� ����
        }

        public void GameEnding()
        {
            audioSource.PlayOneShot(clearSound);
            // ���� ���·� ���� (���� ������ óġ���� �� ȣ��)
            SetState(GameState.Ending);
        }

        public void ReplayGame()
        {
            // ������ ������ϴ� ���
            SetState(GameState.Playing);
            Time.timeScale = 1f; // ���� �ٽ� ����
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
