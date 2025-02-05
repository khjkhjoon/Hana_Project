using Hana.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class WaveManager : Singleton<WaveManager>
{
    [Header("웨이브 설정")]
    private int currentWave = 1; // 현재 웨이브 번호
    private int maxWaves = 10; // 최대 웨이브 (보스 등장 조건)
    private bool finalBossDefeated = false; // 최종 보스 처치 여부
    private bool isWaveInProgress = false; // 웨이브 진행 여부 확인 변수

    [Header("적 스폰 설정")]
    public GameObject enemyPrefab; // 적 프리팹
    public Transform player; // 플레이어 위치
    public float spawnRadius = 2f; // 플레이어와 떨어진 최소 거리
    public float spawnInterval = 1f; // 적 소환 간격 (초)

    [Header("맵 제한 설정")]
    public GameObject mapObject; // 맵 오브젝트 참조
    private Bounds mapBounds; // 맵 경계

    [Header("스폰 포인트")]
    public Transform[] spawnPoints; // 스폰 포인트 배열

    #region 사운드 설정
    public AudioSource audiosource;
    public AudioClip deathSound; //Enemy 죽는 소리
    #endregion

    // 현재 웨이브의 모든 적을 추적할 리스트
    private List<GameObject> currentEnemies = new List<GameObject>();

    private void Start()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            InitializeMapBounds();
            InitializePlayer();
            StartNextWave();
        }
    }

    private void InitializeMapBounds()
    {
        if (mapObject != null)
        {
            Collider mapCollider = mapObject.GetComponent<Collider>();
            if (mapCollider != null)
            {
                mapBounds = mapCollider.bounds;
            }
            else
            {
                Debug.LogError("맵 오브젝트에 Collider가 없습니다!");
                QuitGame();
            }
        }
        else
        {
            Debug.LogError("맵 오브젝트가 설정되지 않았습니다!");
            QuitGame();
        }
    }

    private void InitializePlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player 오브젝트를 찾을 수 없습니다!");
                QuitGame();
            }
        }
    }

    public void StartNextWave()
    {
        // 게임 오버 시 웨이브 진행 차단
        if (GameManager.Instance.CurrentState == GameState.GameOver) return;

        if (isWaveInProgress) return; // 이미 웨이브가 진행 중이면 실행하지 않음
        isWaveInProgress = true;

        if (currentWave > maxWaves)
        {
            if (!finalBossDefeated)
            {
                SpawnFinalBoss(); // 최종 보스 등장
            }
            return;
        }

        Debug.Log("Wave " + currentWave + " 시작!");
        UIManager.Instance.UpdateWaveText(currentWave);
        StartCoroutine(SpawnEnemiesForWave(currentWave));
    }

    private IEnumerator SpawnEnemiesForWave(int wave)
    {
        Debug.Log("웨이브 " + wave + "에 적을 소환합니다.");
        int enemiesToSpawn = Mathf.RoundToInt(currentWave * 1.5f);
        int spawnedEnemies = 0;

        while (spawnedEnemies < enemiesToSpawn)
        {
            Vector3 spawnPosition = GetRandomSpawnPointPosition();

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemies.Add(enemy);
            spawnedEnemies++;

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log($"웨이브 {wave}의 적 소환 완료!");
    }

    private Vector3 GetRandomSpawnPointPosition()
    {
        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        int randomIndex = Random.Range(0, shuffledSpawnPoints.Count);
        return shuffledSpawnPoints[randomIndex].position;
    }

    private void SpawnFinalBoss()
    {
        Debug.Log("최종 보스 등장!");
        finalBossDefeated = true;
        // TODO: 최종 보스 스폰 로직 추가
    }

    public void EnemyDefeated(GameObject defeatedEnemy)
    {
        if (!currentEnemies.Contains(defeatedEnemy)) return; // 이미 처리된 적이면 무시
        audiosource.PlayOneShot(deathSound);
        currentEnemies.Remove(defeatedEnemy);

        if (AllEnemiesDefeated())
        {
            Debug.Log($"Wave {currentWave} 완료!");
            currentWave++;
            isWaveInProgress = false; // 다음 웨이브 준비를 위해 플래그 초기화
            StartNextWave();
        }
    }

    private bool AllEnemiesDefeated()
    {
        return currentEnemies.Count == 0;
    }

    private void QuitGame()
    {
        Debug.LogError("필수 오브젝트가 누락되어 게임을 종료합니다!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
