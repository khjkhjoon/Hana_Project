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
            // 게임 시작 로직
        }

        public void GameOver()
        {
            // 게임 오버 처리
        }

        public void UpdateWave(int wave)
        {
            // 웨이브 업데이트
        }

        public void UpdatePlayerHealth(int health)
        {
            // 체력 업데이트
        }

        public void UpdateSnackCount(int count)
        {
            // 간식 개수 업데이트
        }

        public void UpdateLevelUpProgress(float value)
        {
            // 레벨업 게이지 업데이트
        }

        private void ResetGame()
        {
            // 게임 초기화
        }

        private void TriggerLevelUp()
        {
            // 레벨업 처리
        }
    }
}