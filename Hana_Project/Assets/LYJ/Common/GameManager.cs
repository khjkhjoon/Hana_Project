using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.Common
{
    public class GameManager : Singleton<GameManager>
    {
        private int currentWave;
        private int playerHealth;
        private int snackCount;
        private float levelUpProgress;
        private float levelUpMax;

        public UIManager UIManager
        {
            get => default;
            set
            {
            }
        }

        public LYJ.SkillManager SkillManager
        {
            get => default;
            set
            {
            }
        }

        public LYJ.WaveManager WaveManager
        {
            get => default;
            set
            {
            }
        }

        public KHJ.Enemy Enemy
        {
            get => default;
            set
            {
            }
        }

        public KHJ.Player Player
        {
            get => default;
            set
            {
            }
        }

        public void StartGame()
        {
            // ���� ���� ����
        }

        public void GameOver()
        {
            // ���� ���� ó��
        }

        public void UpdateWave(int wave)
        {
            // ���̺� ������Ʈ
        }

        public void UpdatePlayerHealth(int health)
        {
            // ü�� ������Ʈ
        }

        public void UpdateSnackCount(int count)
        {
            // ���� ���� ������Ʈ
        }

        public void UpdateLevelUpProgress(float value)
        {
            // ������ ������ ������Ʈ
        }

        private void ResetGame()
        {
            // ���� �ʱ�ȭ
        }

        private void TriggerLevelUp()
        {
            // ������ ó��
        }
    }
}