using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    public List<GameObject> lifes_;

    public Text point_tx;
    public int point = 0;
    public bool On_place = false;
    public bool GameOver = false;

    public GameObject GameOverScene;
    public Text points_end;

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

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (On_place == true)
        {
            point_tx.text = point.ToString();
        }
    }

    IEnumerator second()
    {
        while (GameOver == false)
        {
            yield return new WaitForSeconds(1);
            point++;
        }   
    }
}
