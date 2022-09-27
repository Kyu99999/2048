using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mapSizeText;

    private int startMapSize;

    // Start is called before the first frame update
    void Start()
    {
        startMapSize = 3;
        mapSizeText.text = startMapSize + " X " + startMapSize;
    }


    public void GameStart()
    {
        PlayerPrefs.SetInt("MapSize", startMapSize);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Increase()
    {
        if (startMapSize < 10)
        {
            startMapSize++;
            mapSizeText.text = startMapSize + " X " + startMapSize;
        }
    }

    public void Decrease()
    {
        if (startMapSize > 3)
        {
            startMapSize--;
            mapSizeText.text = startMapSize + " X " + startMapSize;
        }
    }
}
