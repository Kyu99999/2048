using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mapSizeText;

    private int mapSize;

    // Start is called before the first frame update
    void Start()
    {
        mapSize = 3;
        mapSizeText.text = mapSize + " X " + mapSize;
    }


    public void GameStart()
    {
        PlayerPrefs.SetInt("MapSize", mapSize);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Increase()
    {
        if (mapSize < 10)
        {
            mapSize++;
            mapSizeText.text = mapSize + " X " + mapSize;
        }
    }

    public void Decrease()
    {
        if (mapSize > 3)
        {
            mapSize--;
            mapSizeText.text = mapSize + " X " + mapSize;
        }
    }
}
