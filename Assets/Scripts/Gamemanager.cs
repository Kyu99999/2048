using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;

public enum EDir
{
    LEFT,
    RIGHT,
    DOWN,
    UP,
}

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;
    public TextMeshProUGUI text;

    public Block[,] blockArr;
    public EDir dir;

    public Sprite[] blockSprites;

    public GameObject blockPrefab;
    public GameObject nodePrefab;

    public RectTransform rectTransform;
    public GameObject gameOverUI;

    public int blockCount = 0;
    public int score = 0;
    public int arrSize = 4;

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
        blockArr = new Block[arrSize, arrSize];  // 4x4 배열

        for (int i = 0; i < arrSize; i++)
        {
            for (int j = 0; j < arrSize; j++)
            {
                Instantiate(nodePrefab, new Vector3(i, -j, 0), Quaternion.identity);

                GameObject block = Instantiate(blockPrefab);
                blockArr[i, j] = block.GetComponent<Block>();
                blockArr[i, j].SetBlock(0, 0, blockSprites[0]);
                blockArr[i, j].transform.position = new Vector3(i, -j, 0);
            }
        }

        Camera cam = Camera.main;
        cam.transform.position = new Vector3((arrSize / 2f) - 0.5f, (-arrSize / 2f) + 0.5f, -10f);

        int x = Random.Range(0, arrSize);
        int y = Random.Range(0, arrSize);
        blockArr[x, y].Init(1, blockSprites[1]);
        blockCount++;

        BlockSpawn();
    }


    // Update is called once per frame
    void Update()
    {
        //Test Code

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            for (int y = 0; y < arrSize; y++)
            {
                for (int x = arrSize - 2; x >= 0; x--)
                {
                    Move(x, y, 1, 0, EDir.RIGHT);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            for (int y = 0; y < arrSize; y++)
            {
                for (int x = 1; x < arrSize; x++)
                {
                    Move(x, y, -1, 0, EDir.LEFT);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int x = 0; x < arrSize; x++)
            {
                for (int y = 1; y < arrSize; y++)
                {
                    Move(x, y, 0, -1, EDir.UP);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            for (int x = 0; x <= arrSize - 1; x++)
            {
                for (int y = arrSize - 2; y >= 0; y--)
                {
                    Move(x, y, 0, 1, EDir.DOWN);
                }
            }
        }

        if (isMove)
        {
            BlockSpawn();
            isMove = false;

            if (blockCount >= arrSize * arrSize)
            {
                if (IsDead())
                {
                    GameOver();
                }
            }
        }

       
    }

    public void BlockSpawn()
    {
        if (blockCount < (arrSize * arrSize))
        {
            while (true)
            {
                int x = Random.Range(0, arrSize);
                int y = Random.Range(0, arrSize);
                if (blockArr[x, y].score == 0)
                {
                    int randomNum = Random.Range(1, 11);

                    //70% 확률로 2
                    if (randomNum >= 4)
                    {
                        blockArr[x, y].Init(1, blockSprites[1]);
                    }
                    else
                    {
                        blockArr[x, y].Init(2, blockSprites[2]);
                    }

                    blockCount++;
                    break;
                }
            }
        }
    }

    public void Move(int curX, int curY, int nextX, int nextY, EDir dir) //재귀
    {
        Block curBlock = blockArr[curX, curY];
        Block nextBlock = blockArr[curX + nextX, curY + nextY];

        // 이동
        if (curBlock.score != 0 && nextBlock.score == 0)
        {
            // Test Code
            curBlock.Move(new Vector3(curX + nextX, -(curY + nextY), 0));
            nextBlock.transform.position = new Vector3(curX, -curY, 0);
            blockArr[curX + nextX, curY + nextY] = curBlock;
            blockArr[curX, curY] = nextBlock;
            //

            //nextBlock.SetBlock(curBlock.score, curBlock.spriteNumber, curBlock.spriteRenderer.sprite);
            //curBlock.SetNode();
            isMove = true;

            switch (dir)
            {
                case EDir.LEFT:
                    if (curX != 1)
                    {
                        Move(curX + nextX, curY, nextX, nextY, dir);
                    }
                    break;
                case EDir.RIGHT:
                    if (curX != arrSize - 2)
                    {
                        Move(curX + nextX, curY, nextX, nextY, dir);
                    }
                    break;
                case EDir.DOWN:
                    if (curY != arrSize - 2)
                    {
                        Move(curX, curY + nextY, nextX, nextY, dir);
                    }
                    break;
                case EDir.UP:
                    if (curY != 1)
                    {
                        Move(curX, curY + nextY, nextX, nextY, dir);
                    }
                    break;
                default:
                    break;
            }
        }
        // 결합
        else if (curBlock.score != 0 && curBlock.score == nextBlock.score && !curBlock.isCombine && !nextBlock.isCombine)
        {
            nextBlock.SetBlock(nextBlock.score * 2, nextBlock.spriteNumber + 1, blockSprites[nextBlock.spriteNumber + 1]);
            nextBlock.Combine();

            score += nextBlock.score;
            text.text = score.ToString();

            curBlock.SetNode();
            blockCount--;
            isMove = true;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsDead()
    {
        bool dead = true;

        // 가로 검사
        for (int y = 0; y < arrSize; y++)
        {
            for (int x = 0; x < arrSize - 1; x++)
            {
                if (blockArr[x, y].score == blockArr[x + 1, y].score)
                {
                    dead = false;
                }
            }
        }

        // 세로 검사
        for (int x = 0; x < arrSize; x++)
        {
            for (int y = 0; y < arrSize - 1; y++)
            {
                if (blockArr[x, y].score == blockArr[x, y + 1].score)
                {
                    dead = false;
                }
            }
        }

        return dead;
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void Continue()
    {
        gameOverUI.SetActive(false);
    }

}
