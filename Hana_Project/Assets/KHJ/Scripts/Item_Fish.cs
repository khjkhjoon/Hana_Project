using System.Collections;
using System.Collections.Generic;
using Hana.Common;
using UnityEngine;

namespace Hana.KHJ
{
    public class Item_Fish : MonoBehaviour
    {
        public Player player;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("itemTest");
            if (other.CompareTag("Player"))
            {
                player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.Heal(1f);
                    Destroy(gameObject);
                }
            }
        }
    }
}