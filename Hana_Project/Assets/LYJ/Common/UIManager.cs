using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hana.Common
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Text waveText;        // ���� ���̺긦 ǥ���ϴ� �ؽ�Ʈ UI
        [SerializeField] private Slider healthSlider;  // �÷��̾� ü���� ǥ���ϴ� �����̴� UI
        [SerializeField] private Slider levelUpSlider; // ������ �������� ǥ���ϴ� �����̴� UI
        [SerializeField] private GameObject startScreen;      // ���� ȭ�� UI
        [SerializeField] private GameObject gameOverScreen;   // ���� ���� ȭ�� UI
        [SerializeField] private GameObject endingScreen;     // ���� ȭ�� UI
        [SerializeField] private GameObject skillPanel;       // ��ų ���� �г� UI
        [SerializeField] private Button[] skillButtons;       // ��ų ���� ��ư��

        public void UpdateWaveText(int wave)
        {
            // ���̺� �ؽ�Ʈ ������Ʈ
        }
        public void UpdateHealthSlider(float currentHealth, float maxHealth)
        {
            // ü�� �����̴� ������Ʈ
        }
        public void UpdateLevelUpSlider(float currentValue, float maxValue)
        {
            // ������ �����̴� ������Ʈ
        }
        public void ShowStartScreen()
        {
            // ���� ȭ�� ǥ��
        }
        public void ShowGameOverScreen()
        {
            // ���� ���� ȭ�� ǥ��
        }
        public void ShowEndingScreen()
        {
            // ���� ȭ�� ǥ��
        }
        public void ShowSkillPanel(string[] skillNames)
        {
            // ��ų ���� ȭ�� ǥ��
        }
        public void HideSkillPanel()
        {
            // ��ų ���� ȭ�� �����
        }
    }
}