using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;


public class Block : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public int SpriteNumber { get; set; }
    public int Score { get; set; }
    public bool IsCombine { get; set; } = false;

    [Range(0f,100f)]
    public float speed = 50f;

    private bool IsMoving { get; set; } = false;
    private Vector3 NextPos { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("Spawn");
        Score = 0;
        SpriteNumber = 0;
    }

    private void Update()
    {
        if(IsMoving)
        {
            transform.position = Vector3.Lerp(transform.position, NextPos, speed * Time.deltaTime);
            if (transform.position == NextPos)
            {
                IsMoving = false;
            }
        }
    }

    private void LateUpdate()
    {
        IsCombine = false;
    }

    public void Init(int number, Sprite sprite)
    {
        animator.SetTrigger("Spawn");
        Score = 2;
        this.SpriteNumber = number;
        spriteRenderer.sprite = sprite;

        for(int i = 1; i < number; i++ )
        {
            Score *= 2;
        }
    }

    public void Move(Vector3 pos)
    {
        IsMoving = true;
        NextPos = pos;
    }

    public void Combine()
    {
        animator.SetTrigger("Combine");
        IsCombine = true;
    }

    public void SetNode()
    {
        Score = 0;
        SpriteNumber = 0;
        spriteRenderer.sprite = Gamemanager.instance.BlockSprites[0];
        IsMoving = false;
    }

    public void SetNode(Vector3 pos)
    {
        transform.position = pos;
        Score = 0;
        SpriteNumber = 0;
        spriteRenderer.sprite = Gamemanager.instance.BlockSprites[0];
        IsMoving = false;
    }

    public void SetBlock(int score, int spriteNumber, Sprite sprite)
    {
        this.Score = score;
        this.SpriteNumber = spriteNumber;
        spriteRenderer.sprite = sprite;
    }
}