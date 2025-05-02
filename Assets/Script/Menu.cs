using UnityEngine;
public class Menu : MonoBehaviour
{
    public static Menu Instance { get; private set; }

    public GameObject Menu_obj;
    public GameObject player_;
    public float startSpeed;
    bool on_place = false;

    GameObject player_instance;

    Camera mainCamera;
    public float Camera_speed;
    public GameObject bg_menu;
    public GameObject HUD;
    public GameObject bg_1_obj;
    public GameObject bg_2_obj;

    public GameObject life_bg;
    public GameObject life;

    Vector3 camera_start;
    Vector3 bg_1;
    Vector3 bg_2;

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

    void Start()
    {
        BGSTOP();

        mainCamera = Camera.main;

        camera_start = mainCamera.transform.position;

        bg_1 = bg_1_obj.transform.position;
        bg_2 = bg_2_obj.transform.position;
    }


    void Update()
    {
        StartGame();
    }

    public void StartButton()
    {
        Menu_obj.SetActive(false);
        GameObject player_instance1 = Instantiate(player_, new Vector2(-27.8f, -2.83f), Quaternion.identity);
        player_instance1.GetComponent<Rigidbody2D>().gravityScale = 0;
        player_instance1.GetComponent<PlayerScript>().enabled = false;

        player_instance = player_instance1;
    }

    void RealStart()
    {
        player_instance.GetComponent<Rigidbody2D>().gravityScale = 1;
        player_instance.GetComponent<PlayerScript>().enabled = true;
        GameObject[] bg = GameObject.FindGameObjectsWithTag("bg");
        foreach (GameObject bg_ in bg)
        {
            bg_.GetComponent<BackgroundScroller>().enabled = true;
        }
        HUDManager.Instance.On_place = true;
        HUD.SetActive(true);
        MapManager.Instance.StartCoroutine("startGame");
        HUDManager.Instance.StartCoroutine("second");

        for (int i = 0; i < 4; i++)
        {
            GameObject newLife = Instantiate(life, life_bg.transform);
            HUDManager.Instance.lifes_.Add(newLife);
        }


    }

    void StartGame()
    {
        if (player_instance != null)
        {
            if (player_instance.transform.position.x < -5.1f && on_place == false)
            {
                player_instance.transform.position = Vector2.MoveTowards(player_instance.transform.position, new Vector2(-5, -2.83f), startSpeed * Time.deltaTime);
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, new Vector3(0, 0, -10), Camera_speed * Time.deltaTime);
            }
            else if (player_instance.transform.position.x > -5.1f && on_place == false)
            {
                on_place = true;
                bg_menu.SetActive(false);
                RealStart();
            }
        }
    }

    public void GoMenu()
    {
        Menu_obj.SetActive(true);
        GameObject[] attk = GameObject.FindGameObjectsWithTag("attk");

        foreach (GameObject attk_ in attk)
        {
            Destroy(attk_);
        }

        HUDManager.Instance.GameOverScene.SetActive(false);

        Time.timeScale = 1;

        HUD.SetActive(false);

        bg_1_obj.transform.position = bg_1;
        bg_2_obj.transform.position = bg_2;
        HUDManager.Instance.On_place = false;
        on_place = false;
        HUDManager.Instance.point = 0;
        HUDManager.Instance.point_tx.text = HUDManager.Instance.point.ToString();
        HUDManager.Instance.GameOver = false;
        mainCamera.transform.position = camera_start;
        bg_menu.SetActive(true);
    }

    public void BGSTOP()
    {
        GameObject[] bg = GameObject.FindGameObjectsWithTag("bg");
        foreach (GameObject bg_ in bg)
        {
            bg_.GetComponent<BackgroundScroller>().enabled = false;
        }
    }

    public void PlayAgain()
    {
        GameObject[] attk = GameObject.FindGameObjectsWithTag("attk");

        foreach (GameObject attk_ in attk)
        {
            Destroy(attk_);
        }

        HUDManager.Instance.GameOverScene.SetActive(false);

        Time.timeScale = 1;

        HUD.SetActive(false);

        bg_1_obj.transform.position = bg_1;
        bg_2_obj.transform.position = bg_2;
        HUDManager.Instance.On_place = false;
        on_place = false;
        HUDManager.Instance.point = 0;
        HUDManager.Instance.point_tx.text = HUDManager.Instance.point.ToString();
        HUDManager.Instance.GameOver = false;
        mainCamera.transform.position = camera_start;
        bg_menu.SetActive(true);

        GameObject player_instance_new = Instantiate(player_, new Vector2(-27.8f, -2.83f), Quaternion.identity);
        player_instance_new.GetComponent<Rigidbody2D>().gravityScale = 0;
        player_instance_new.GetComponent<PlayerScript>().enabled = false;

        player_instance = player_instance_new;


    }

    public void Quit()
    {
        Application.Quit();
    }
}
