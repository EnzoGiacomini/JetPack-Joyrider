using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    public static PlayerScript Instance { get; private set; }

    public float JetForce;
    bool is_grounded = true;
    public GameObject fire_effect;

    public bool on_fall = false;

    //PlayerStats
    public int health = 4;

    Rigidbody2D rb;
    Animator player_anim;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        player_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) && on_fall == false)
        {
            Move();
            fire_effect.SetActive(true);
        }
        else
        {
            fire_effect.SetActive(false);
        }

        if (is_grounded == false)
        {
            player_anim.SetBool("Air", true);
        }
        else
        {
            player_anim.SetBool("Air", false);
        }

    }

    void Move()
    {
        rb.AddForce(new Vector2(0, JetForce), ForceMode2D.Impulse);
        if (is_grounded == true)
        {
            player_anim.SetTrigger("JumpForceGround");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && is_grounded == false)
        {
            is_grounded = true;
            player_anim.SetTrigger("Fall");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            is_grounded = false;
        }
    }

    public void DMG()
    {
        health--;

        GameObject life = HUDManager.Instance.lifes_[0];

        HUDManager.Instance.lifes_.RemoveAt(0);

        Destroy(life);

        if (health == 0)
        {
            HUDManager.Instance.GameOverScene.SetActive(true);

            if (HUDManager.Instance.point > SaveManager.Instance.record_points)
            {
                SaveManager.Instance.record_points = HUDManager.Instance.point;
            }

            HUDManager.Instance.points_end.text = "This run points: " + HUDManager.Instance.point.ToString() + "\n Record: " + SaveManager.Instance.record_points.ToString();

            SaveManager.Instance.Save();

            HUDManager.Instance.StopAllCoroutines();
            MapManager.Instance.StopAllCoroutines();
            HUDManager.Instance.GameOver = true;
            Menu.Instance.BGSTOP();
            health = 4;

            Destroy(this.gameObject);

            Time.timeScale = 0;
        }
    }
}
