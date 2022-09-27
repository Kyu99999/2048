using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public EState State { get; private set; }
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
        State = Gamemanager.instance.State;

        switch (State)
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

        if (hit && hit.collider.CompareTag("Block"))
        {
            Block block = hit.collider.GetComponent<Block>();
            block.SetNode();
            Gamemanager.instance.BlockCount--;
        }
    }

    private void SetObstacle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if(hit && hit.collider.CompareTag("Obstacle"))
        {
            Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
            obstacle.SetActive(!obstacle.IsAlive);
        }
    }
}
