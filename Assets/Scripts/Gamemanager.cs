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

    private float width = 4;
    private float height = 4;

    private int blockCount = 0;
    public int score = 0;
    public int arrNum = 4;

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
        blockArr = new GameObject[arrNum, arrNum];  // 4x4 배열
        Camera cam = Camera.main;
        cam.transform.position = new Vector3((arrNum / 2f) - 0.5f, (-arrNum / 2f) + 0.5f, -10f);
        BlockSpawn();
        BlockSpawn();
    }


    // Update is called once per frame
    void Update()
    {
        //Test Code
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int y = 0; y <= arrNum - 1; y++)
            {
                for (int x = arrNum - 2; x >= 0; x--)
                {
                    for (int i = 1; i <= 1 + x; i++)
                    {
                        //MoveR(3 - i, y);
                        Move(arrNum - 1 - i, y, arrNum - 1 - i + 1, y);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for (int y = 0; y <= arrNum -1; y++)
            {
                for (int x = 1; x <= arrNum - 1; x++)
                {
                    for (int i = 1; i <= arrNum - x; i++)
                    {
                        //MoveL(i, y);
                        Move(i, y, i-1, y);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            for(int x=0; x <= arrNum - 1; x++)
            {
                for(int y=1; y<= arrNum - 1; y++)
                {
                    for(int i=1; i<= arrNum - y; i++)
                    {
                        Move(x, i, x, i - 1);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("test");
            for (int x = 0; x <= arrNum - 1; x++)
            {
                for (int y = arrNum - 2; y >= 0; y--)
                {
                    for (int i = 1; i <= 1 + y; i++)
                    {
                        Move(x, arrNum - 1 - i, x, arrNum - 1 - i + 1);
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
            Restart();
        }

        text.text = "Score : " + score;
    }

    public void BlockSpawn()
    {
        if (blockCount <= 15)
        {
            while (true)
            {
                int x = Random.Range(0, arrNum);
                int y = Random.Range(0, arrNum);
                if (blockArr[x, y] == null)
                {
                    int randomNum = Random.Range(0, 2);
                  
                    GameObject newBlock = Instantiate(prefab, new Vector3(x, -y, 0), Quaternion.identity);

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
        Block curBlock = blockArr[curX, curY].GetComponent<Block>();
        Block nextBlock = blockArr[nextX, nextY].GetComponent<Block>();

        GameObject curObj = blockArr[curX, curY];
        GameObject nextObj = blockArr[nextX, nextY];

        if (curObj != null && nextObj == null)
        {
            curBlock.Move(new Vector3(nextX, -nextY, 0));

            blockArr[nextX, nextY] = blockArr[curX, curY];
            blockArr[curX, curY] = null;
            isMove = true;
        }
        // 같은 숫자일 때 결합
        else if (curObj != null && nextObj != null && !nextBlock.isCombine && !curBlock.isCombine && (curBlock.number == nextBlock.number))
        {
            Destroy(blockArr[nextX, nextY]);

            curBlock.Move(new Vector3(nextX, -nextY, 0));
            curBlock.Combine(blockSprites[curBlock.number + 1]);
            
            blockArr[nextX, nextY] = blockArr[curX, curY];
            blockArr[curX, curY] = null;

            blockCount--;
            isMove = true;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Defeat()
    {

    }
}
