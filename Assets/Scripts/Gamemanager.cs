using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class Gamemanager : MonoBehaviour
{
    private GameObject[,] blockArr;

    public GameObject prefab;

    void Start()
    {
        blockArr = new GameObject[4, 4];  // 4x4 배열

        BlockSpawn();
        BlockSpawn();
        BlockSpawn();
        BlockSpawn();
        BlockSpawn();

    }


    // Update is called once per frame
    void Update()
    {
        //Test Code
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (blockArr[i, j] != null)
                    {
                        Debug.Log(i + " " + j + " 1");
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int i = 2; i >= 0; i--)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (blockArr[i + 1, j] == null)
                    {
                            
                    }
                        
                }
            }
        }
        //
    }

    public void BlockSpawn()
    {
        while (true)
        {
            int x = Random.Range(0, 4);
            int y = Random.Range(0, 4);
            if (blockArr[x,y] == null)
            {
                // Test Code      // 숫자 2 또는 4
                GameObject newObj = new GameObject();
                blockArr[x, y] = newObj;
                break;
            }
        }
    }

    public void Move()
    {
        
    }
}
