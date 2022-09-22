using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;


public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;
    public TextMeshProUGUI text;

    private GameObject[,] blockArr;

    public Sprite[] blockSprites;

    public GameObject prefab;
    public RectTransform rectTransform;

    public float width = 1.4f;
    public float height = 1.4f;

    private int blockCount = 0;
    public int score = 0;

    private bool isMove = false;
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
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(rectTransform.transform.position);
        }
        //Test Code
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int y = 0; y <= 3; y++)
            {
                for (int x = 2; x >= 0; x--)
                {
                    for (int i = 1; i <= 1 + x; i++)
                    {
                        //MoveR(3 - i, y);
                        Move(3 - i, y, 3 - i + 1, y);
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
                        //MoveL(i, y);
                        Move(i, y, i-1, y);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            for(int x=0; x <= 3; x++)
            {
                for(int y=1; y<=3; y++)
                {
                    for(int i=1; i<=4-y; i++)
                    {
                        Move(x, i, x, i - 1);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("test");
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 2; y >= 0; y--)
                {
                    for (int i = 1; i <= 1 + y; i++)
                    {
                        Move(x, 3 - i, x, 3 - i + 1);
                    }
                }
            }
        }

        if (isMove)
        {
            BlockSpawn();
            isMove = false;
        }

        //Test Code
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        text.text = "Score : " + score;
    }

    public void BlockSpawn()
    {
        if (blockCount <= 15)
        {
            while (true)
            {
                int x = Random.Range(0, 4);
                int y = Random.Range(0, 4);
                if (blockArr[x, y] == null)
                {
                    int randomNum = Random.Range(0, 2);
                    GameObject newBlock = Instantiate(prefab, new Vector3(width * x, -height * y, 0), Quaternion.identity);
                    newBlock.GetComponent<Block>().Init(randomNum, blockSprites[randomNum]);
                    blockArr[x, y] = newBlock;
                    blockCount++;
                    break;
                }
            }
        }
    }

    public void Move(int curX, int curY, int nextX, int nextY)  //x,y는 현재 위치, nextX, nextY는 다음 위치
    {
        if (blockArr[curX, curY] != null && blockArr[nextX, nextY] == null)
        {
            // 블럭 이동
            Block block = blockArr[curX, curY].GetComponent<Block>();
            block.Move(new Vector3(width * nextX, -height * nextY, 0));

            blockArr[nextX, nextY] = blockArr[curX, curY];
            blockArr[curX, curY] = null;
            isMove = true;
        }
        // 같은 숫자일 때 결합
        else if (blockArr[curX, curY] != null && blockArr[nextX, nextY] != null && !blockArr[nextX, nextY].GetComponent<Block>().isCombine && (blockArr[curX, curY].GetComponent<Block>().number == blockArr[nextX, nextY].GetComponent<Block>().number))
        {
            GameObject newBlock = Instantiate(prefab, new Vector3(width * nextX, -height * nextY, 0), Quaternion.identity);
            int number = blockArr[curX, curY].GetComponent<Block>().number + 1;

            Block block = newBlock.GetComponent<Block>();
            block.Init(number, blockSprites[number]);
            block.Combine();
            
            Destroy(blockArr[curX, curY]);
            Destroy(blockArr[nextX, nextY]);

            blockArr[curX, curY] = null;
            blockArr[nextX, nextY] = newBlock;

            blockCount--;
            isMove = true;
        }
    }
}
