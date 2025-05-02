using System.Collections;
using UnityEngine;

public class SimpleLaserScript : MonoBehaviour
{
    GameObject player_obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_obj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerScript.Instance.DMG();
            collision.gameObject.GetComponent<Animator>().SetTrigger("Shock");
            StartCoroutine(fallafterdmg());
        }
    }

    IEnumerator fallafterdmg()
    {
        if (player_obj != null)
        {
            player_obj.GetComponent<PlayerScript>().on_fall = true;
            yield return new WaitForSeconds(0.8f);
            player_obj.GetComponent<PlayerScript>().on_fall = false;
        }
    }
}
