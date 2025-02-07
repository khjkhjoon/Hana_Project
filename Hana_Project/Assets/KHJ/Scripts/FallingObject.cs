using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hana.KHJ
{
    public class FallingObject : MonoBehaviour
    {
        [SerializeField] 
        private float Damage = 4f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();
                player.TakeDamage(Damage);
            }
            if (other.CompareTag("Ground") || transform.position.y < -1f)
            {
                Destroy(gameObject);
            }
        }
    }
}