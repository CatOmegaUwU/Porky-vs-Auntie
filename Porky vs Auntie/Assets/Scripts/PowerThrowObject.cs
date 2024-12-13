using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerThrowObject : MonoBehaviour
{
    void Update()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Hit");
            Destroy(gameObject);
        }
        else if (collision.tag == "Player2")
        {
            Debug.Log("Hit2");
            Destroy(gameObject);
        }
        else if (collision.tag == "Wall")
        {
            Debug.Log("HitWall");
            Destroy(gameObject);
        }
    }

}
