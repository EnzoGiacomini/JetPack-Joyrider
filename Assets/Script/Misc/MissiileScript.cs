using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MissiileScript : MonoBehaviour
{
    GameObject player_obj;
    public bool is_bomb;

    public GameObject explosion_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_obj = GameObject.FindGameObjectWithTag("Player");
        Destroy(this.gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (is_bomb == false)
            {
                PlayerScript.Instance.DMG();
                GameObject explosion_instance = Instantiate(explosion_, collision.gameObject.transform.position, Quaternion.identity);
                collision.gameObject.GetComponent<Animator>().SetTrigger("Explosion");
                Destroy(this.gameObject);
                Destroy(explosion_instance, 0.4f);
            }
            else if (is_bomb == true)
            {
                PlayerScript.Instance.DMG();
                GameObject explosion_instance = Instantiate(explosion_, collision.gameObject.transform.position, Quaternion.identity);
                collision.gameObject.GetComponent<Animator>().SetTrigger("Flame");
                Destroy(this.gameObject);
                Destroy(explosion_instance, 0.4f);
            }

        }
    }

}
