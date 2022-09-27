using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BLOCKTYPE
{
    VERTICAL,
    HORIZONTAL,
}


public class Obstacle : MonoBehaviour
{
    public bool IsAlive { get; set; } = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void SetBlock(BLOCKTYPE blockType)
    {
        switch (blockType)
        {
            case BLOCKTYPE.VERTICAL:
                transform.localScale = new Vector3(0.08f, 1, 1);
                break;
            case BLOCKTYPE.HORIZONTAL:
                transform.localScale = new Vector3(1, 0.08f, 1);
                break;
            default:
                break;
        }
    }

    public void SetActive(bool isAlive)
    {
        this.IsAlive = isAlive;
        spriteRenderer.enabled = isAlive;
    }
}
