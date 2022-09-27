using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("UI 연출 시간")]
    public float uiTimer = 1f;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rectTransform.localScale = new Vector3(0.3f, 0.3f, 1);
        StartCoroutine(BiggerUI());
    }

    private IEnumerator BiggerUI()
    {
        while(true)
        {
            float x = rectTransform.localScale.x;
            float y = rectTransform.localScale.y;
            if (x >= 1f)
            {
                yield break;
            }
            rectTransform.localScale = new Vector3(x + Time.deltaTime / uiTimer, y + Time.deltaTime / uiTimer, 1);
            yield return null;
        }
    }
}