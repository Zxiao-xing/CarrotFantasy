using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEffect : MonoBehaviour
{
    [SerializeField] int damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.activeSelf)
            if (collision.tag.Equals("Monster") || collision.tag.Equals("Item"))
                collision.SendMessage("TakeDamage", damage);
    }
}
