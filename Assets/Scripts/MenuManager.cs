using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField]
    private TextMeshProUGUI mapSizeText;

    private int startMapSize;

    public bool IsOnObstacle { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var obj = FindObjectsOfType<MenuManager>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        startMapSize = 3;
        mapSizeText.text = startMapSize + " X " + startMapSize;
    }

    public void GameStart()
    {
        PlayerPrefs.SetInt("MapSize", startMapSize);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void IncreaseMapSize()
    {
        if (startMapSize < 10)
        {
            startMapSize++;
            mapSizeText.text = startMapSize + " X " + startMapSize;
        }
    }

    public void DecreaseMapSize()
    {
        if (startMapSize > 3)
        {
            startMapSize--;
            mapSizeText.text = startMapSize + " X " + startMapSize;
        }
    }
}
