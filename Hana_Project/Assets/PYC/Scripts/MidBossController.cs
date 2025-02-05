using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


namespace Hana.PYC
{
    public class MidBossController : MonoBehaviour
    {
        private UnityEngine.AI.NavMeshAgent m_agent;
        public Transform[] waypoints;
        private int m_currentwaypointindex = 0;
        private Animator animator;

        public float moveSpeed = 10f;

        void Start()
        {
            m_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            m_agent.SetDestination(waypoints[m_currentwaypointindex].position);
            animator = transform.GetComponentInChildren<Animator>();
            animator.SetFloat("Walk", 1);
        }

        void Update()
        {
            BossMove();
            // BossAttack();
        }


        public void BossMove()
        {
            if (m_agent.remainingDistance < m_agent.stoppingDistance)
            {
                m_currentwaypointindex = (m_currentwaypointindex + 1) % waypoints.Length;
                m_agent.SetDestination(waypoints[m_currentwaypointindex].position);
            }

        }
        public void BossAttack()
        {
            // this.transform.Translate(1 * moveSpeed * Time.deltaTime, 0, 0);
            this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }
}
