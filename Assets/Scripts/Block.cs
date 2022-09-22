using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public float width = 1.4f;
    public float height = 1.4f;

    public int number;
    public int score;
    public bool isCombine = false;

    public bool isMoving = false;
    public Vector3 nextPos;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("Spawn");
        score = 2;
    }

    private void Update()
    {
        if(isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, nextPos, 0.5f);
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
        this.number = number;
        spriteRenderer.sprite = sprite;

        for(int i = 0; i < number; i++ )
        {
            score *= 2;
        }
    }

    public void Move(Vector3 pos)
    {
        isMoving = true;
        nextPos = pos;
    }

    public void Combine(Sprite sprite)
    {
        Gamemanager.instance.score += score * 2;
        
        this.number++;
        score *= 2;
        spriteRenderer.sprite = sprite;

        animator.SetTrigger("Combine");

        isCombine = true;
    }
}


