using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }

    //Missile
    public GameObject missile;
    public GameObject missile_alert;
    public float missileSpeed = 1.0f;
    bool Targeting = false;
    float AlertTime = 4;
    public float AlertSpeed;
    public Sprite alert_income;

    //SimpleLaser
    public GameObject laser_r; //Reuso para ComplexLaser
    public GameObject laser_l; //Reuso para ComplexLaser
    public GameObject laser_b; //Reuso para ComplexLaser
    public float laserSpeed;
    public int laser_time;

    //ComplexLaser
    public GameObject center_;
    public float ComplexLaserSpeed;
    public float rotationSpeed;

    //BombExplosion
    public GameObject bomb_;
    public float bomb_speed;

    //VerticalLaser
    public GameObject vertical_laser_up;
    public GameObject vertical_laser_down;
    public GameObject vertical_laser_b;
    public float vertical_laser_speed;


    // Lista de corrotinas para ataques
    private Func<IEnumerator>[] coroutines;

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
        // Criando um array de referências para métodos que retornam IEnumerator
        coroutines = new Func<IEnumerator>[]
        {
            LaserAttk,
            MissileAlert,
            BombAttk,
            VerticalLaserMiddle,
            VerticalLaserApart,
            VerticalLaserDouble,
        };

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Targeting == true)
        {
            MissileAlertTimer();
        }
    }

    //InGameMechanic

    IEnumerator startGame()
    {

        while (HUDManager.Instance.GameOver == false)
        {
            int Random_sec = UnityEngine.Random.Range(3, 5);

            yield return new WaitForSeconds(Random_sec);

            float Random_chance = UnityEngine.Random.value;

            if (Random_chance < 0.7f)
            {
                for (int i = 0; i < 4; i++)
                {
                    float Random_chanc_spawn = UnityEngine.Random.value;

                    if (Random_chanc_spawn < 0.3f)
                    {
                        // Escolhendo aleatoriamente um índice do array
                        int randomIndex = UnityEngine.Random.Range(0, coroutines.Length);

                        StartCoroutine(coroutines[randomIndex]());
                    }
                }
            }
            else
            {
               // Escolhendo aleatoriamente um índice do array
               int randomIndex = UnityEngine.Random.Range(0, coroutines.Length);

               StartCoroutine(coroutines[randomIndex]());
            }
        }
        
    }


    //AttackMechanics

    //SimpleLaserMechanic
    IEnumerator LaserAttk()
    {
        int Random_num = UnityEngine.Random.Range(1, 3);

        for (int i = 0; i < Random_num; i++)
        {
            float Random_chance = UnityEngine.Random.value;

            float Random_y = UnityEngine.Random.Range(-3.4f, 4.8f);

            GameObject laser_l_instance = Instantiate(laser_l, new Vector2(-8.18f, Random_y), Quaternion.identity);
            GameObject laser_r_instance = Instantiate(laser_r, new Vector2(8.2f, Random_y), laser_r.transform.rotation);

            for (int p = 0; p < 5; p++)
            {
                yield return new WaitForSeconds(0.1f);

                laser_r_instance.GetComponent<SpriteRenderer>().color = Color.red;
                laser_l_instance.GetComponent<SpriteRenderer>().color = Color.red;

                yield return new WaitForSeconds(0.1f);

                laser_r_instance.GetComponent<SpriteRenderer>().color = Color.white;
                laser_l_instance.GetComponent<SpriteRenderer>().color = Color.white;
            }

            
                yield return new WaitForSeconds(0.5f);

                GameObject laser_b_instance = Instantiate(laser_b, new Vector2(0, Random_y), laser_b.transform.rotation);

                while (laser_b_instance.transform.localScale.y < 35)
                {
                    yield return new WaitForFixedUpdate();

                    Vector3 scale = laser_b_instance.transform.localScale;
                    scale.y = Mathf.Lerp(scale.y, 35.1f, laserSpeed * Time.fixedDeltaTime);
                    laser_b_instance.transform.localScale = scale;
                }

                Destroy(laser_b_instance, laser_time);  
                Destroy(laser_l_instance, laser_time);
                Destroy(laser_r_instance, laser_time);

        }
    }

    //MissileMechanic
    public void MissileAlertTimer()
    {
        AlertTime -= Time.fixedDeltaTime;

        if (AlertTime <= 0)
        {
            Targeting = false;
            AlertTime = 4;
        }
    }

    IEnumerator MissileAlert()
    {
        Targeting = true;

        float Random_place = UnityEngine.Random.Range(-3.4f, 4.8f);

        GameObject missile_alert_instance = Instantiate(missile_alert, new Vector2(8.275f, 0), Quaternion.identity);

        while (Targeting)
        {
            yield return new WaitForFixedUpdate();

            missile_alert_instance.transform.position = Vector2.MoveTowards(missile_alert_instance.transform.position, new Vector2(missile_alert_instance.transform.position.x, Random_place), AlertSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(missile_alert_instance.transform.position.y - Random_place) < 0.1f)
            {
                Random_place = UnityEngine.Random.Range(-3.4f, 4.8f);
            }
        }

        missile_alert_instance.GetComponent<SpriteRenderer>().sprite = alert_income;
        missile_alert_instance.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        yield return new WaitForSeconds(0.7f);

        SpawnMissile(new Vector2(10, missile_alert_instance.transform.position.y));

        Destroy(missile_alert_instance);

        
    }

    public void SpawnMissile(Vector2 position)
    {
        GameObject missile_instance = Instantiate(missile, position, Quaternion.identity);
        missile_instance.GetComponent<Rigidbody2D>().AddForce(new Vector2(-missileSpeed * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
    }

    //ComplexLaserMechanic
    /**IEnumerator ComplexLaserAttk()
    {
        float Random_y = UnityEngine.Random.Range(-3.4f, 3f);
        float Random_x = UnityEngine.Random.Range(1, 4f);

        GameObject center_instance = Instantiate(center_, new Vector2(12.5f, Random_y), Quaternion.identity);
        GameObject laser_l_instance = Instantiate(laser_l, new Vector2(12.5f - Random_x, Random_y), Quaternion.identity);
        GameObject laser_r_instance = Instantiate(laser_r, new Vector2(12.5f + Random_x, Random_y), laser_r.transform.rotation);

        float Dist_l_r = Vector2.Distance(laser_l_instance.transform.position, laser_r_instance.transform.position);

        GameObject laser_b_instance = Instantiate(laser_b, new Vector2(12.5f, Random_y), laser_b.transform.rotation);
        laser_b_instance.transform.localScale = new Vector3(2, Dist_l_r * 1.5f, 1);

        while (center_instance.transform.position.x > -17)
        {

            center_instance.transform.position = Vector2.MoveTowards(center_instance.transform.position, new Vector2(-17.1f, Random_y), ComplexLaserSpeed * Time.deltaTime);

            if (Dist_l_r < laser_b_instance.transform.localScale.y)
            {
                laser_b_instance.transform.localScale = new Vector3(2, Dist_l_r, 1);
            }

            Vector3 position = center_instance.transform.position;

            laser_l_instance.transform.RotateAround(position, Vector3.forward, rotationSpeed * Time.deltaTime);
            laser_r_instance.transform.RotateAround(position, Vector3.forward, rotationSpeed * Time.deltaTime);
            laser_b_instance.transform.RotateAround(position, Vector3.forward, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(center_instance);
        Destroy(laser_l_instance);
        Destroy(laser_r_instance);
        Destroy(laser_b_instance);
    }**/ // Mecanica não agradou. Principalmente a parte de rotação.

    //BombExplosionMechanic
    public void SpawnBomb(Vector2 position)
    {
        GameObject bomb_instance = Instantiate(bomb_, position, Quaternion.identity);
        bomb_instance.GetComponent<Rigidbody2D>().AddForce(Vector2.left * bomb_speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    IEnumerator BombAttk()
    {
        yield return null;

        float Random_doble = UnityEngine.Random.value;

        if (Random_doble < 0.75f)
        {
            int Random_max = UnityEngine.Random.Range(1, 4);

            for (int i = 0; i < Random_max; i++)
            {
                float Random_y = UnityEngine.Random.Range(-3.4f, 4.7f);
                float Random_time = UnityEngine.Random.Range(0.5f, 1.5f);

                yield return new WaitForSeconds(Random_time);
                SpawnBomb(new Vector2(10, Random_y));
            }
        }
        else
        {
            float Random_y = UnityEngine.Random.Range(-3.4f, 4.8f);
            SpawnBomb(new Vector2(10, Random_y));
        }
    }

    //VerticalLaserMechanic

    IEnumerator VerticalLaserMiddle()
    {
        float Random_sec = UnityEngine.Random.Range(0, 4);

        yield return new WaitForSeconds(Random_sec);

        float Random_y = UnityEngine.Random.Range(-2.2f, -1);

        GameObject vertical_laser_up_instance = Instantiate(vertical_laser_up, new Vector2(10, -Random_y), vertical_laser_up.transform.rotation);
        GameObject vertical_laser_down_instance = Instantiate(vertical_laser_down, new Vector2(10, Random_y), vertical_laser_down.transform.rotation);
        GameObject vertical_laser_b_instance = Instantiate(vertical_laser_b, new Vector2(10, 0), vertical_laser_b.transform.rotation);

        float Dist_up_down = Vector2.Distance(vertical_laser_up_instance.transform.position, vertical_laser_down_instance.transform.position);

        vertical_laser_b_instance.transform.localScale = new Vector3(Dist_up_down * 4, 3, 1);

        while (vertical_laser_up.transform.position.x > -16.9)
        {
            yield return new WaitForFixedUpdate();

            vertical_laser_up_instance.transform.position = Vector2.MoveTowards(vertical_laser_up_instance.transform.position, new Vector2(-17.1f, vertical_laser_up_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            vertical_laser_down_instance.transform.position = Vector2.MoveTowards(vertical_laser_down_instance.transform.position, new Vector2(-17.1f, vertical_laser_down_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            vertical_laser_b_instance.transform.position = Vector2.MoveTowards(vertical_laser_b_instance.transform.position, new Vector2(-17.1f, vertical_laser_b_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
        }

        Destroy(vertical_laser_up_instance);
        Destroy(vertical_laser_down_instance);
        Destroy(vertical_laser_b_instance);


    }

    IEnumerator VerticalLaserApart()
    {
        float Random_sec = UnityEngine.Random.Range(1, 5);

        yield return new WaitForSeconds(Random_sec);

        float random_side = UnityEngine.Random.value;

        if (random_side < 0.5f)
        {
            GameObject vertical_laser_up_instance = Instantiate(vertical_laser_up, new Vector2(10, 4.54f), vertical_laser_up.transform.rotation);

            float random_chance = UnityEngine.Random.value;
            float y_ = 0;

            if (random_chance < 0.5f)
            {
                y_ = 1.14f;
            }
            else
            {
                y_ = -0.93f;
            }

            GameObject vertical_laser_down_instance = Instantiate(vertical_laser_down, new Vector2(10, y_), vertical_laser_down.transform.rotation);

            float Dist_up_down = Vector2.Distance(vertical_laser_up_instance.transform.position, vertical_laser_down_instance.transform.position);

            // 🟢 **Ajuste: Calcular a posição do meio corretamente**
            float middleY = (vertical_laser_up_instance.transform.position.y + vertical_laser_down_instance.transform.position.y) / 2;

            GameObject vertical_laser_b_instance = Instantiate(vertical_laser_b, new Vector2(10, middleY), vertical_laser_b.transform.rotation);
            vertical_laser_b_instance.transform.localScale = new Vector3(Dist_up_down * 4, 3, 1);

            while (vertical_laser_up_instance.transform.position.x > -17)
            {
                yield return new WaitForFixedUpdate();

                vertical_laser_up_instance.transform.position = Vector2.MoveTowards(vertical_laser_up_instance.transform.position, new Vector2(-17.1f, vertical_laser_up_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
                vertical_laser_down_instance.transform.position = Vector2.MoveTowards(vertical_laser_down_instance.transform.position, new Vector2(-17.1f, vertical_laser_down_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
                vertical_laser_b_instance.transform.position = Vector2.MoveTowards(vertical_laser_b_instance.transform.position, new Vector2(-17.1f, vertical_laser_b_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            }

            Destroy(vertical_laser_up_instance, 0.1f);
            Destroy(vertical_laser_down_instance, 0.1f);
            Destroy(vertical_laser_b_instance, 0.1f);

        }
        else
        {
            GameObject vertical_laser_down_instance = Instantiate(vertical_laser_down, new Vector2(10, -3.759486f), vertical_laser_down.transform.rotation);

            float random_chance = UnityEngine.Random.value;
            float y_ = 0;

            if (random_chance < 0.5f)
            {
                y_ = 2.12f;
            }
            else
            {
                y_ = 0;
            }

            GameObject vertical_laser_up_instance = Instantiate(vertical_laser_up, new Vector2(10, y_), vertical_laser_up.transform.rotation);

            float Dist_up_down = Vector2.Distance(vertical_laser_up_instance.transform.position, vertical_laser_down_instance.transform.position);

            // 🟢 **Ajuste: Calcular a posição do meio corretamente**
            float middleY = (vertical_laser_up_instance.transform.position.y + vertical_laser_down_instance.transform.position.y) / 2;

            GameObject vertical_laser_b_instance = Instantiate(vertical_laser_b, new Vector2(10, middleY), vertical_laser_b.transform.rotation);
            vertical_laser_b_instance.transform.localScale = new Vector3(Dist_up_down * 4, 3, 1);

            while (vertical_laser_up_instance.transform.position.x > -17)
            {
                yield return new WaitForFixedUpdate();
                vertical_laser_up_instance.transform.position = Vector2.MoveTowards(vertical_laser_up_instance.transform.position, new Vector2(-17.1f, vertical_laser_up_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
                vertical_laser_down_instance.transform.position = Vector2.MoveTowards(vertical_laser_down_instance.transform.position, new Vector2(-17.1f, vertical_laser_down_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
                vertical_laser_b_instance.transform.position = Vector2.MoveTowards(vertical_laser_b_instance.transform.position, new Vector2(-17.1f, vertical_laser_b_instance.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            }

            Destroy(vertical_laser_up_instance, 0.1f);
            Destroy(vertical_laser_down_instance, 0.1f);
            Destroy(vertical_laser_b_instance, 0.1f);
        }
    }

    IEnumerator VerticalLaserDouble()
    {
        float Random_sec = UnityEngine.Random.Range(1, 5);

        yield return new WaitForSeconds(Random_sec);

        //Parte inferior
        GameObject vertical_laser_up_instance1 = Instantiate(vertical_laser_up, new Vector2(10, -1.012641f), vertical_laser_up.transform.rotation);
        GameObject vertical_laser_down_instance1 = Instantiate(vertical_laser_down, new Vector2(10, -3.8f), vertical_laser_down.transform.rotation);

        float Dist_up_down1 = Vector2.Distance(vertical_laser_up_instance1.transform.position, vertical_laser_down_instance1.transform.position);

        // 🟢 **Ajuste: Calcular a posição do meio corretamente**
        float middleY = (vertical_laser_up_instance1.transform.position.y + vertical_laser_down_instance1.transform.position.y) / 2;

        GameObject vertical_laser_b_instance1 = Instantiate(vertical_laser_b, new Vector2(10, middleY), vertical_laser_b.transform.rotation);
        vertical_laser_b_instance1.transform.localScale = new Vector3(Dist_up_down1 * 4, 3, 1);

        //Parte superior
        GameObject vertical_laser_up_instance2 = Instantiate(vertical_laser_up, new Vector2(10, 4.593272f), vertical_laser_up.transform.rotation);
        GameObject vertical_laser_down_instance2 = Instantiate(vertical_laser_down, new Vector2(10, 1.86039f), vertical_laser_down.transform.rotation);

        float Dist_up_down2 = Vector2.Distance(vertical_laser_up_instance2.transform.position, vertical_laser_down_instance2.transform.position);

        // 🟢 **Ajuste: Calcular a posição do meio corretamente**
        middleY = (vertical_laser_up_instance2.transform.position.y + vertical_laser_down_instance2.transform.position.y) / 2;

        GameObject vertical_laser_b_instance2 = Instantiate(vertical_laser_b, new Vector2(10, middleY), vertical_laser_b.transform.rotation);
        vertical_laser_b_instance2.transform.localScale = new Vector3(Dist_up_down2 * 4, 3, 1);

        while (vertical_laser_up_instance1.transform.position.x > -17)
        {
            yield return new WaitForFixedUpdate();
            vertical_laser_up_instance1.transform.position = Vector2.MoveTowards(vertical_laser_up_instance1.transform.position, new Vector2(-17.1f, vertical_laser_up_instance1.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            vertical_laser_down_instance1.transform.position = Vector2.MoveTowards(vertical_laser_down_instance1.transform.position, new Vector2(-17.1f, vertical_laser_down_instance1.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            vertical_laser_b_instance1.transform.position = Vector2.MoveTowards(vertical_laser_b_instance1.transform.position, new Vector2(-17.1f, vertical_laser_b_instance1.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            vertical_laser_up_instance2.transform.position = Vector2.MoveTowards(vertical_laser_up_instance2.transform.position, new Vector2(-17.1f, vertical_laser_up_instance2.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            vertical_laser_down_instance2.transform.position = Vector2.MoveTowards(vertical_laser_down_instance2.transform.position, new Vector2(-17.1f, vertical_laser_down_instance2.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
            vertical_laser_b_instance2.transform.position = Vector2.MoveTowards(vertical_laser_b_instance2.transform.position, new Vector2(-17.1f, vertical_laser_b_instance2.transform.position.y), vertical_laser_speed * Time.fixedDeltaTime);
        }

        Destroy(vertical_laser_up_instance1, 0.1f);
        Destroy(vertical_laser_down_instance1, 0.1f);
        Destroy(vertical_laser_b_instance1, 0.1f);
        Destroy(vertical_laser_up_instance2, 0.1f);
        Destroy(vertical_laser_down_instance2, 0.1f);
        Destroy(vertical_laser_b_instance2, 0.1f);
    }

}
