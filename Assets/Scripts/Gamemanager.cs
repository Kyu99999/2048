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

    public EState State { get; set; }

    private Block[,] blockArr;
    private Obstacle[,] horObstacleArr;
    private Obstacle[,] verObstacleArr;
   
    [field: SerializeField]
    public Sprite[] BlockSprites { get; private set; }

    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private GameObject nodePrefab;
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private GameObject clearUI;
    [SerializeField]
    private TextMeshProUGUI curScoreText;
    [SerializeField]
    private TextMeshProUGUI bestScoreText;

    public int BlockCount { get; set; } = 0;
    private int curScore = 0;
    private int bestScore = 0;

    private int mapSize = 4;

    [Tooltip("CLEAR 조건")]
    public int clearScore = 2048;

    [SerializeField]
    [Range(1, 10)]
    [Tooltip("2블록이 나올 확률")]
    private int blockSpawnProbability = 7;

    private bool isBlockMove = false;
    
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

        mapSize = PlayerPrefs.GetInt("MapSize");
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        switch (State)
        {
            case EState.PLAYING:
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    for (int y = 0; y < mapSize; y++)
                    {
                        for (int x = mapSize - 2; x >= 0; x--)
                        {
                            MoveOrCombine(x, y, 1, 0, EDir.RIGHT);
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    for (int y = 0; y < mapSize; y++)
                    {
                        for (int x = 1; x < mapSize; x++)
                        {
                            MoveOrCombine(x, y, -1, 0, EDir.LEFT);
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    for (int x = 0; x < mapSize; x++)
                    {
                        for (int y = 1; y < mapSize; y++)
                        {
                            MoveOrCombine(x, y, 0, -1, EDir.UP);
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    for (int x = 0; x <= mapSize - 1; x++)
                    {
                        for (int y = mapSize - 2; y >= 0; y--)
                        {
                            MoveOrCombine(x, y, 0, 1, EDir.DOWN);
                        }
                    }
                }

                if (isBlockMove)
                {
                    BlockSpawn();
                    isBlockMove = false;

                    if (BlockCount >= mapSize * mapSize)
                    {
                        if (CheckIsDead())
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
        if (BlockCount < (mapSize * mapSize))
        {
            while (true)
            {
                int x = Random.Range(0, mapSize);
                int y = Random.Range(0, mapSize);
                if (blockArr[x, y].Score == 0)
                {
                    int randomNum = Random.Range(1, 11);

                    //70% 확률로 2
                    if (randomNum <= blockSpawnProbability)
                    {
                        blockArr[x, y].Init(1, BlockSprites[1]);
                    }
                    else
                    {
                        blockArr[x, y].Init(2, BlockSprites[2]);
                    }

                    BlockCount++;
                    break;
                }
            }
        }
    }

    public void MoveOrCombine(int curX, int curY, int nextX, int nextY, EDir dir) //재귀
    {
        Block curBlock = blockArr[curX, curY];
        Block nextBlock = blockArr[curX + nextX, curY + nextY];

        bool obstacle = CheckObstacle(curX, curY, dir);

        // 이동
        if (curBlock.Score != 0 && nextBlock.Score == 0 && !obstacle)
        {
            curBlock.Move(new Vector3((curX + nextX), -(curY + nextY), 0));
            nextBlock.transform.position = new Vector3(curX, -curY, 0);
            blockArr[curX + nextX, curY + nextY] = curBlock;
            blockArr[curX, curY] = nextBlock;
           
            isBlockMove = true;

            switch (dir)
            {
                case EDir.LEFT:
                    if (curX != 1)
                    {
                        MoveOrCombine(curX + nextX, curY, nextX, nextY, dir);
                    }
                    break;
                case EDir.RIGHT:
                    if (curX != mapSize - 2)
                    {
                        MoveOrCombine(curX + nextX, curY, nextX, nextY, dir);
                    }
                    break;
                case EDir.DOWN:
                    if (curY != mapSize - 2)
                    {
                        MoveOrCombine(curX, curY + nextY, nextX, nextY, dir);
                    }
                    break;
                case EDir.UP:
                    if (curY != 1)
                    {
                        MoveOrCombine(curX, curY + nextY, nextX, nextY, dir);
                    }
                    break;
                default:
                    break;
            }
        }
        // 결합
        else if (curBlock.Score != 0 && curBlock.Score == nextBlock.Score && !curBlock.IsCombine && !nextBlock.IsCombine && !obstacle)
        {
            nextBlock.SetBlock(nextBlock.Score * 2, nextBlock.SpriteNumber + 1, BlockSprites[nextBlock.SpriteNumber + 1]);
            nextBlock.Combine();
            curBlock.SetNode(new Vector3(curX, -curY, 0));

            SetScore(nextBlock.Score);

            if (nextBlock.Score == clearScore)
            {
                Clear();
            }
            BlockCount--;
            isBlockMove = true;
        }
    }

    public void Restart()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }

    public bool CheckIsDead()
    {
        bool isDead = true;

        // 가로 검사
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize - 1; x++)
            {
                if (blockArr[x, y].Score == blockArr[x + 1, y].Score)
                {
                    isDead = false;
                }
            }
        }

        // 세로 검사
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize - 1; y++)
            {
                if (blockArr[x, y].Score == blockArr[x, y + 1].Score)
                {
                    isDead = false;
                }
            }
        }

        return isDead;
    }

    public void Init()
    {
        bestScore = PlayerPrefs.GetInt("BestScore" + mapSize.ToString());
        bestScoreText.text = bestScore.ToString();

        blockArr = new Block[mapSize, mapSize];  // 4x4 배열
        verObstacleArr = new Obstacle[mapSize - 1, mapSize];
        horObstacleArr = new Obstacle[mapSize, mapSize - 1];

        // 노드 및 블럭 생성
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Instantiate(nodePrefab, new Vector3(i, -j, 0), Quaternion.identity);

                GameObject block = Instantiate(blockPrefab);

                blockArr[i, j] = block.GetComponent<Block>();
                blockArr[i, j].SetBlock(0, 0, BlockSprites[0]);
                blockArr[i, j].transform.position = new Vector3(i, -j, 0);
            }
        }

        // 장애물 생성
        for (int i = 0; i < mapSize - 1; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                GameObject obs = Instantiate(obstaclePrefab, new Vector3(i + 1 / 2f, -j, 0), Quaternion.identity);
                verObstacleArr[i, j] = obs.GetComponent<Obstacle>();
                verObstacleArr[i, j].SetBlock(BLOCKTYPE.VERTICAL);
                verObstacleArr[i, j].SetActive(false);
            }
        }
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize - 1; j++)
            {
                GameObject obs = Instantiate(obstaclePrefab, new Vector3(i , -j - 1 / 2f, 0), Quaternion.identity);
                horObstacleArr[i, j] = obs.GetComponent<Obstacle>();
                horObstacleArr[i, j].SetBlock(BLOCKTYPE.HORIZONTAL);
                horObstacleArr[i, j].SetActive(false);
            }
        }

        // 카메라 이동
        Camera cam = Camera.main;
        cam.transform.position = new Vector3((mapSize / 2f) - 0.5f, (-mapSize / 2f) + 0.5f, -10f);
        cam.orthographicSize = 0.4f + 0.8f * (float)mapSize;

        // 블럭(2) 생성
        int x = Random.Range(0, mapSize);
        int y = Random.Range(0, mapSize);
        blockArr[x, y].Init(1, BlockSprites[1]);
        BlockCount++;

        // 블럭 랜덤 생성
        BlockSpawn();

        State = EState.PLAYING;
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
        this.State = state;
    }

    public void SetScore(int score)
    {
        this.curScore += score;
        curScoreText.text = this.curScore.ToString();
        if (this.curScore >= bestScore)
        {
            bestScore = this.curScore;
            PlayerPrefs.SetInt("BestScore" + mapSize.ToString(), bestScore);
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
                if (verObstacleArr[curX - 1, curY].IsAlive)
                {
                    obstacle = true;
                }
                break;
            case EDir.RIGHT:
                if (verObstacleArr[curX, curY].IsAlive)
                {
                    obstacle = true;
                }
                break;
            case EDir.DOWN:
                if (horObstacleArr[curX, curY].IsAlive)
                {
                    obstacle = true;
                }
                break;
            case EDir.UP:
                if (horObstacleArr[curX, curY-1].IsAlive)
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
