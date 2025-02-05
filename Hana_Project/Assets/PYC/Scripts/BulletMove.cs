using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.PYC
{
    public class BulletMove : MonoBehaviour
    {
        public float moveSpeed = 5f;

        void Update()
        {
            BossAttack();
        }

        public void BossAttack()
        {
            // this.transform.Translate(1 * moveSpeed * Time.deltaTime, 0, 0);
            this.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        }
    }
}


