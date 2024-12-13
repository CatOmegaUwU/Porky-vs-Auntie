using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    void Update()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Debug.Log("Hit Porky");
            Destroy(gameObject);
        }
        else if (collision.tag == "Player2")
        {
            //Debug.Log("Hit Auntie");
            Destroy(gameObject);
        }
        else if (collision.tag == "Wall")
        {
            //Debug.Log("Hit Wall");
            Destroy(gameObject);
        }
    }

}
