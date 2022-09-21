using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    private GameObject[,] blockArr;

    public GameObject[] blocks;

    public GameObject prefab;

    public float width = 1.4f;
    public float height = 1.4f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        blockArr = new GameObject[4, 4];  // 4x4 배열

        BlockSpawn();
        BlockSpawn();
        BlockSpawn();
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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int y = 0; y <= 3; y++)
            {
                for (int x = 2; x >= 0; x--)
                {
                    for (int i = 1; i <= 1 + x; i++)
                    {
                        MoveR(3 - i, y);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for (int y = 0; y <= 3; y++)
            {
                for (int x = 1; x <= 3; x++)
                {
                    for (int i = 1; i <= 4 - x; i++)
                    {
                        MoveL(i, y);
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
            if (blockArr[x, y] == null)
            {
                // Test Code      // 숫자 2 또는 4
                int randomBlockNum = Random.Range(0, 2);
                GameObject newObj = Instantiate(blocks[randomBlockNum], new Vector3(width * x, height * y, 0), Quaternion.identity);
                blockArr[x, y] = newObj;
                break;
            }
        }
    }

    public void MoveR(int x, int y)
    {
        if (blockArr[x, y] != null && blockArr[x + 1, y] == null)
        {
            blockArr[x, y].transform.position = new Vector3(width * (x + 1), height * y, 0);
            blockArr[x + 1, y] = blockArr[x, y];
            blockArr[x, y] = null;
        }
        // 같은 숫자일 때 결합
        else if (blockArr[x, y] != null && blockArr[x + 1, y] != null && (blockArr[x + 1, y].GetComponent<Block>().number == blockArr[x, y].GetComponent<Block>().number))
        {
            GameObject newObj = Instantiate(blocks[blockArr[x + 1, y].GetComponent<Block>().number + 1], new Vector3(width * (x + 1), height * y, 0), Quaternion.identity);

            Destroy(blockArr[x + 1, y]);
            Destroy(blockArr[x, y]);

            blockArr[x, y] = null;
            blockArr[x + 1, y] = newObj;
        }

    }

    public void MoveL(int x, int y)
    {
        if (blockArr[x, y] != null && blockArr[x - 1, y] == null)
        {
            blockArr[x, y].transform.position = new Vector3(width * (x - 1), height * y, 0);
            blockArr[x - 1, y] = blockArr[x, y];
            blockArr[x, y] = null;
        }
        // 같은 숫자일 때 결합
        else if (blockArr[x, y] != null && blockArr[x - 1, y] != null && (blockArr[x - 1, y].GetComponent<Block>().number == blockArr[x, y].GetComponent<Block>().number))
        {
            GameObject newObj = Instantiate(blocks[blockArr[x - 1, y].GetComponent<Block>().number + 1], new Vector3(width * (x - 1), height * y, 0), Quaternion.identity);

            Destroy(blockArr[x - 1, y]);
            Destroy(blockArr[x, y]);

            blockArr[x, y] = null;
            blockArr[x - 1, y] = newObj;
        }
    }
}
