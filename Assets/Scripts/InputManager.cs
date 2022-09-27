using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public EState state { get; set; }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        state = Gamemanager.instance.state;

        switch (state)
        {
            case EState.PLAYING:
                if (Input.GetMouseButtonDown(1))
                {
                    DeleteBlock();
                    Gamemanager.instance.Continue();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    SetObstacle();
                }
                break;
            case EState.PAUSE:
                break;
            case EState.GAMEOVER:
                if (Input.GetMouseButtonDown(1))
                {
                    DeleteBlock();
                    Gamemanager.instance.Continue();
                }
                break;
            default:
                break;
        }
    }

    private void DeleteBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit && hit.collider.tag == "Block")
        {
            Block block = hit.collider.GetComponent<Block>();
            block.SetNode();
            Gamemanager.instance.blockCount--;
        }
    }

    private void SetObstacle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if(hit && hit.collider.tag == "Obstacle")
        {
            Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
            obstacle.SetActive(!obstacle.isAlive);
        }
    }
}
