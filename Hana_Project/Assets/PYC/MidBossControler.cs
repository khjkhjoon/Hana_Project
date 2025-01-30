
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.PYC
{
    public class MidBossControler : MonoBehaviour
    {
        [SerializeField] private AudioClip noramClip;
        [SerializeField] private AudioClip dieClip;
        [SerializeField] private AudioClip attackClip;
        [SerializeField] private GameObject attackEffect;
        [SerializeField] private GameObject dieEffect;

        public LYJ.HealthBarController HealthBarController
        {
            get => default;
            set
            {
            }
        }

        public void MidBossMove()
        {

        }

        public void MidBossAttack()
        {

        }
    }
}