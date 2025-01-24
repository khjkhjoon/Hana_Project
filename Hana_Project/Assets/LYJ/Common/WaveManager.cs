using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.LYJ
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private int currentWave;           // 현재 웨이브
        [SerializeField] private float waveInterval;        // 웨이브 간격
        [SerializeField] private GameObject[] enemies;      // 적 프리팹 배열
        [SerializeField] private Transform[] spawnPoints;   // 적 스폰 위치 배열

        public void StartNewWave(int wave)
        {
            // 새로운 웨이브 시작
        }
        public void SpawnEnemies(int enemyCount)
        {
            // 적 생성
        }
        public void EndWave()
        {
            // 웨이브 종료 처리
        }
        public void ResetWaves()
        {
            // 웨이브 상태 초기화
        }
    }
}