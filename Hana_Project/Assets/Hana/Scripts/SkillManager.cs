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
                Debug.LogError("PlayerController가 씬에 없습니다! 스킬 적용 불가.");
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
                    Debug.LogWarning("알 수 없는 스킬 타입: " + skillType);
                    break;
            }

            // 스킬 적용 후 UI 비활성화 및 레벨업 게이지 초기화
            UIManager.Instance.HideLevelUpUI();
            GameManager.Instance.ResetLevelUpProgress();
        }
    }
}
