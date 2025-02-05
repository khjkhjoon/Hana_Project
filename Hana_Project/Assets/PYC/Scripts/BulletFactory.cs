using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.PYC
{
    public class BulletFactory : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform spwanPointTransform;
        public float bulletLiftime = 5f;

        public float timer = 0.0f;
        // public float waitingTime = 10000f;

        void Update()
        {
            float waitingTime = 5f;
            timer += Time.deltaTime;

            if (timer > waitingTime)
            {
                GameObject bullet = Instantiate(bulletPrefab
                      //    ,spwanPointTransform.position, spwanPointTransform.rotation);
                      , spwanPointTransform.position, Quaternion.identity);
                Destroy(bullet, bulletLiftime);
                timer = 0.0f;
            }
        }
    }
}
