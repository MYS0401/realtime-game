using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text countdownText;
    public int startCount = 3;
    //public GameObject plane;

    void Start()
    {
        countdownText.gameObject.SetActive(true);
        //plane.SetActive(true);
        StartCoroutine(CountDownCoroutine());
    }

    IEnumerator CountDownCoroutine()
    {
        int count = startCount;

        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.text = "START!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        //plane.SetActive(false);
    }
}
