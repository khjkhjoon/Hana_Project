using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hana.LYJ;
using Hana.KHJ;

namespace Hana.Common
{
    public class SkillManager : Singleton<SkillManager>
    {
        public void ApplySkill(string skillType)
        {
            Player player = FindFirstObjectByType<Player>();

            if (player == null)
            {
                Debug.LogError("PlayerController�� ���� �����ϴ�! ��ų ���� �Ұ�.");
                return;
            }

            switch (skillType)
            {
                case "AttackPower":
                    player.IncreaseAttackPower();
                    break;
                case "AttackSpeed":
                    player.IncreaseAttackSpeed();
                    break;
                case "MoveSpeed":
                    player.IncreaseMoveSpeed();
                    break;
                default:
                    Debug.LogWarning("�� �� ���� ��ų Ÿ��: " + skillType);
                    break;
            }

            // ��ų ���� �� UI ��Ȱ��ȭ �� ������ ������ �ʱ�ȭ
            UIManager.Instance.HideLevelUpUI();
            GameManager.Instance.ResetLevelUpProgress();
        }
    }
}
