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

public enum EState
{
    PLAYING,
    PAUSE,
    GAMEOVER,
}


public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    public EState state { get; set; }

    public Block[,] blockArr;
    public EDir dir;
    public Obstacle[,] horObstacleArr;
    public Obstacle[,] verOvstacleArr;

    public Sprite[] blockSprites;

    public GameObject blockPrefab;
    public GameObject nodePrefab;
    public GameObject obstaclePrefab;

    public GameObject gameOverUI;
    public GameObject clearUI;

    public TextMeshProUGUI text;
    public TextMeshProUGUI bestScoreText;

    public int blockCount { get; set; } = 0;
    private int score = 0;
    public int arrSize = 4;
    private int bestScore = 0;
    public int clearScore = 0;

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
        Init();
    }

    void Update()
    {
        switch (state)
        {
            case EState.PLAYING:
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
                break;
            case EState.PAUSE:
                break;
            case EState.GAMEOVER:
                break;
            default:
                break;
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

        bool obstacle = CheckObstacle(curX, curY, dir);

        // 이동
        if (curBlock.score != 0 && nextBlock.score == 0 && !obstacle)
        {
            curBlock.Move(new Vector3((curX + nextX), -(curY + nextY), 0));
            nextBlock.transform.position = new Vector3(curX, -curY, 0);
            blockArr[curX + nextX, curY + nextY] = curBlock;
            blockArr[curX, curY] = nextBlock;
           
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
        else if (curBlock.score != 0 && curBlock.score == nextBlock.score && !curBlock.isCombine && !nextBlock.isCombine && !obstacle)
        {
            nextBlock.SetBlock(nextBlock.score * 2, nextBlock.spriteNumber + 1, blockSprites[nextBlock.spriteNumber + 1]);
            nextBlock.Combine();
            curBlock.SetNode(new Vector3(curX, -curY, 0));
            if (score == clearScore)
            {
                Clear();
            }
            blockCount--;
            isMove = true;
        }
    }

    public void Restart()
    {
        PlayerPrefs.Save();
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

    public void Init()
    {
        bestScore = PlayerPrefs.GetInt("BestScore");
        bestScoreText.text = bestScore.ToString();

        blockArr = new Block[arrSize, arrSize];  // 4x4 배열
        verOvstacleArr = new Obstacle[arrSize - 1, arrSize];
        horObstacleArr = new Obstacle[arrSize, arrSize - 1];

        // 노드 및 블럭
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

        // 장애물
        for (int i = 0; i < arrSize - 1; i++)
        {
            for (int j = 0; j < arrSize; j++)
            {
                GameObject obs = Instantiate(obstaclePrefab, new Vector3(i + 1 / 2f, -j, 0), Quaternion.identity);
                verOvstacleArr[i, j] = obs.GetComponent<Obstacle>();
                verOvstacleArr[i, j].SetBlock(BLOCKTYPE.VERTICAL);
                verOvstacleArr[i, j].SetActive(false);
            }
        }
        for (int i = 0; i < arrSize; i++)
        {
            for (int j = 0; j < arrSize - 1; j++)
            {
                GameObject obs = Instantiate(obstaclePrefab, new Vector3(i , -j - 1 / 2f, 0), Quaternion.identity);
                horObstacleArr[i, j] = obs.GetComponent<Obstacle>();
                horObstacleArr[i, j].SetBlock(BLOCKTYPE.HORIZONTAL);
                horObstacleArr[i, j].SetActive(false);
            }
        }

        Camera cam = Camera.main;
        cam.transform.position = new Vector3((arrSize / 2f) - 0.5f, (-arrSize / 2f) + 0.5f, -10f);
        cam.orthographicSize = 0.4f + 0.8f * (float)arrSize;

        int x = Random.Range(0, arrSize);
        int y = Random.Range(0, arrSize);
        blockArr[x, y].Init(1, blockSprites[1]);
        blockCount++;

        BlockSpawn();

        state = EState.PLAYING;
    }

    public void GameOver()
    {
        SetState(EState.GAMEOVER);
        gameOverUI.SetActive(true);
    }

    public void Clear()
    {
        SetState(EState.PAUSE);
        clearUI.SetActive(true);
    }

    public void SetState(EState state)
    {
        this.state = state;
    }

    public void SetScore(int score)
    {
        this.score += score;
        text.text = this.score.ToString();
        if (this.score >= bestScore)
        {
            bestScore = this.score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            bestScoreText.text = bestScore.ToString();
        }
    }

    public void Continue()
    {
        SetState(EState.PLAYING);
        gameOverUI.SetActive(false);
        clearUI.SetActive(false);
    }


    public bool CheckObstacle(int curX, int curY, EDir dir)
    {
        bool obstacle = false;

        switch (dir)
        {
            case EDir.LEFT:
                if (verOvstacleArr[curX - 1, curY].isAlive)
                {
                    obstacle = true;
                }
                break;
            case EDir.RIGHT:
                if (verOvstacleArr[curX, curY].isAlive)
                {
                    obstacle = true;
                }
                break;
            case EDir.DOWN:
                if (horObstacleArr[curX, curY].isAlive)
                {
                    obstacle = true;
                }
                break;
            case EDir.UP:
                if (horObstacleArr[curX, curY-1].isAlive)
                {
                    obstacle = true;
                }
                break;
            default:
                break;
        }

        return obstacle;
    }
}
