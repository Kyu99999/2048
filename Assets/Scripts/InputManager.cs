using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

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
        if (Input.GetMouseButtonDown(1))
        {
            DeleteBlock();
            Gamemanager.instance.Continue();
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


}
