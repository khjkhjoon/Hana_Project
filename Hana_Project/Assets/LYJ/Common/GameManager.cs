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