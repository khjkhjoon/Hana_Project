using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.LYJ
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private int currentWave;           // ���� ���̺�
        [SerializeField] private float waveInterval;        // ���̺� ����
        [SerializeField] private GameObject[] enemies;      // �� ������ �迭
        [SerializeField] private Transform[] spawnPoints;   // �� ���� ��ġ �迭

        public void StartNewWave(int wave)
        {
            // ���ο� ���̺� ����
        }
        public void SpawnEnemies(int enemyCount)
        {
            // �� ����
        }
        public void EndWave()
        {
            // ���̺� ���� ó��
        }
        public void ResetWaves()
        {
            // ���̺� ���� �ʱ�ȭ
        }
    }
}