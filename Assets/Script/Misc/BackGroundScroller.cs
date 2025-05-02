using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed = 5f; // Velocidade de deslocamento do fundo
    public float resetPositionX; // Posição X onde o fundo será reiniciado
    private Vector3 startPosition; // Posição inicial do fundo

    void Start()
    {
        startPosition = transform.position;
        resetPositionX = startPosition.x - GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Move o fundo para a esquerda
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Reposiciona quando sai completamente da tela
        if (transform.position.x <= resetPositionX)
        {
            transform.position = startPosition;
        }
    }
}
