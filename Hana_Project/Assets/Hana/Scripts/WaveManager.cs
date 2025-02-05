using Hana.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class WaveManager : Singleton<WaveManager>
{
    [Header("���̺� ����")]
    private int currentWave = 1; // ���� ���̺� ��ȣ
    private int maxWaves = 10; // �ִ� ���̺� (���� ���� ����)
    private bool finalBossDefeated = false; // ���� ���� óġ ����
    private bool isWaveInProgress = false; // ���̺� ���� ���� Ȯ�� ����

    [Header("�� ���� ����")]
    public GameObject enemyPrefab; // �� ������
    public Transform player; // �÷��̾� ��ġ
    public float spawnRadius = 2f; // �÷��̾�� ������ �ּ� �Ÿ�
    public float spawnInterval = 1f; // �� ��ȯ ���� (��)

    [Header("�� ���� ����")]
    public GameObject mapObject; // �� ������Ʈ ����
    private Bounds mapBounds; // �� ���

    [Header("���� ����Ʈ")]
    public Transform[] spawnPoints; // ���� ����Ʈ �迭

    #region ���� ����
    public AudioSource audiosource;
    public AudioClip deathSound; //Enemy �״� �Ҹ�
    #endregion

    // ���� ���̺��� ��� ���� ������ ����Ʈ
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
                Debug.LogError("�� ������Ʈ�� Collider�� �����ϴ�!");
                QuitGame();
            }
        }
        else
        {
            Debug.LogError("�� ������Ʈ�� �������� �ʾҽ��ϴ�!");
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
                Debug.LogError("Player ������Ʈ�� ã�� �� �����ϴ�!");
                QuitGame();
            }
        }
    }

    public void StartNextWave()
    {
        // ���� ���� �� ���̺� ���� ����
        if (GameManager.Instance.CurrentState == GameState.GameOver) return;

        if (isWaveInProgress) return; // �̹� ���̺갡 ���� ���̸� �������� ����
        isWaveInProgress = true;

        if (currentWave > maxWaves)
        {
            if (!finalBossDefeated)
            {
                SpawnFinalBoss(); // ���� ���� ����
            }
            return;
        }

        Debug.Log("Wave " + currentWave + " ����!");
        UIManager.Instance.UpdateWaveText(currentWave);
        StartCoroutine(SpawnEnemiesForWave(currentWave));
    }

    private IEnumerator SpawnEnemiesForWave(int wave)
    {
        Debug.Log("���̺� " + wave + "�� ���� ��ȯ�մϴ�.");
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

        Debug.Log($"���̺� {wave}�� �� ��ȯ �Ϸ�!");
    }

    private Vector3 GetRandomSpawnPointPosition()
    {
        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        int randomIndex = Random.Range(0, shuffledSpawnPoints.Count);
        return shuffledSpawnPoints[randomIndex].position;
    }

    private void SpawnFinalBoss()
    {
        Debug.Log("���� ���� ����!");
        finalBossDefeated = true;
        // TODO: ���� ���� ���� ���� �߰�
    }

    public void EnemyDefeated(GameObject defeatedEnemy)
    {
        if (!currentEnemies.Contains(defeatedEnemy)) return; // �̹� ó���� ���̸� ����
        audiosource.PlayOneShot(deathSound);
        currentEnemies.Remove(defeatedEnemy);

        if (AllEnemiesDefeated())
        {
            Debug.Log($"Wave {currentWave} �Ϸ�!");
            currentWave++;
            isWaveInProgress = false; // ���� ���̺� �غ� ���� �÷��� �ʱ�ȭ
            StartNextWave();
        }
    }

    private bool AllEnemiesDefeated()
    {
        return currentEnemies.Count == 0;
    }

    private void QuitGame()
    {
        Debug.LogError("�ʼ� ������Ʈ�� �����Ǿ� ������ �����մϴ�!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
