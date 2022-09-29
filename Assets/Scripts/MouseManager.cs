using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance;

    [SerializeField]
    private Texture2D mouseCursor;

    public EState State { get; private set; }
    private bool isDelete = false;
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
                if (Input.GetMouseButtonDown(0) && isDelete)
                {
                    DeleteBlock();
                    SetDefaultMouseCursor();
                    isDelete = false;
                }
                    /* Test Code
                    if (Input.GetMouseButtonDown(1))
                    {
                        DeleteBlock();
                        Gamemanager.instance.Continue();
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        SetObstacle();
                    }
                    */
                    break;
            case EState.PAUSE:
                break;
            case EState.GAMEOVER:
                if (Input.GetMouseButtonDown(0) && isDelete)
                {
                    DeleteBlock();
                    SetDefaultMouseCursor();
                    isDelete = false;
                }

                /* Test Code
                if (Input.GetMouseButtonDown(1))
                {
                    DeleteBlock();
                }
                */
                break;
            default:
                break;
        }
    }

    private void DeleteBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        int blockCount = Gamemanager.instance.BlockCount;
        int deleteCount = Gamemanager.instance.DeleteCount;

        if (hit && hit.collider.CompareTag("Block") && deleteCount > 0 && blockCount > 1)
        {
            Block block = hit.collider.GetComponent<Block>();
            block.SetNode();
            Gamemanager.instance.BlockCount--;

            Gamemanager.instance.DeleteCount--;
            Gamemanager.instance.deleteCountText.text = Gamemanager.instance.DeleteCount.ToString();

            Gamemanager.instance.Continue();
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

    public void SetMouseCursor()
    {
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
        isDelete = true;

    }

    public void SetDefaultMouseCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
