using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //カウントダウン
    public Text countdownText;
    public int startCount = 3;//秒数
    //public GameObject plane;

    //public MonoBehaviour playerController;

    void Start()
    {
        

        countdownText.gameObject.SetActive(true);
        //plane.SetActive(true);
        StartCoroutine(CountDownCoroutine());
    }

    IEnumerator CountDownCoroutine()
    {

        //playerController.enabled = false;
        InputBlocker.isBlocked = true;
        int count = startCount;

        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        //playerController.enabled = true;
        InputBlocker.isBlocked = false;

        countdownText.text = "START!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        //plane.SetActive(false);
    }
}
