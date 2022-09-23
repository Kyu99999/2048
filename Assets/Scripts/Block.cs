using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public int spriteNumber;
    public int score;
    public bool isCombine = false;
    public float speed = 50f;

    public bool isMoving = false;
    public Vector3 nextPos;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("Spawn");
        score = 0;
        spriteNumber = 0;
    }
    private void Update()
    {
        if(isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, nextPos, speed * Time.deltaTime);
            if (transform.position == nextPos)
            {
                isMoving = false;
            }
        }
    }

    private void LateUpdate()
    {
        isCombine = false;
    }

    public void Init(int number, Sprite sprite)
    {
        animator.SetTrigger("Spawn");
        score = 2;
        this.spriteNumber = number;
        spriteRenderer.sprite = sprite;

        for(int i = 1; i < number; i++ )
        {
            score *= 2;
        }
    }

    public void Move(Vector3 pos)
    {
        isMoving = true;
        nextPos = pos;
    }

    public void Combine()
    {
        animator.SetTrigger("Combine");
        isCombine = true;
    }

    public void SetNode()
    {
        score = 0;
        spriteNumber = 0;
        spriteRenderer.sprite = Gamemanager.instance.blockSprites[0];
    }

    public void SetBlock(int score, int spriteNumber, Sprite sprite)
    {
        this.score = score;
        this.spriteNumber = spriteNumber;
        spriteRenderer.sprite = sprite;
    }
}


