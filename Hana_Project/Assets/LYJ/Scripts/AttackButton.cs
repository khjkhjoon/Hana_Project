using System.Collections;
using System.Collections.Generic;
using Hana.KHJ;
using UnityEngine;

namespace Hana.LYJ
{
    public class AttackButton : MonoBehaviour
    {
        public Player player; // PlayerAttack을 직접 연결

        private void Start()
        {
            if (player == null)
            {
                Debug.LogError("PlayerAttack is not assigned in the Inspector!");
            }
        }

        public void OnAttackButtonPressed()
        {
            if (player != null)
            {
                player.Shooting();
            }
            else
            {
                Debug.LogWarning("player is not assigned!");
            }
        }
    }
}