using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text countdownText;//始まるまでのカウントダウン
    public int startCount = 3;//秒数
    //public GameObject plane;

    //制限時間
    public Text timelimitText;
    public int timelimit = 10;

    //リトライボタン
    public GameObject retry;

    public Text endText;

    //public MonoBehaviour playerController;

    void Start()
    {
        endText.gameObject.SetActive(false);
        retry.SetActive(false);
        countdownText.gameObject.SetActive(true);
        //plane.SetActive(true);
        StartCoroutine(CountDownCoroutine());//カウントダウン開始(StartCount)
    }

    IEnumerator CountDownCoroutine()
    {

        //playerController.enabled = false;
        InputBlocker.isBlocked = true;//操作を受け付けなくする
       
        while (startCount > 0)
        {
            countdownText.text = startCount.ToString();
            yield return new WaitForSeconds(1f);
            startCount--;
        }

        //playerController.enabled = true;
        InputBlocker.isBlocked = false;

        countdownText.text = "START!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        //plane.SetActive(false);

        //カウントダウン開始(TimeLimit)
        StartCoroutine(TimeLimitCoroutine());
    }

    IEnumerator TimeLimitCoroutine()
    {
        while (timelimit >= 0)
        {
            timelimitText.text = timelimit.ToString();
            yield return new WaitForSeconds(1f);
            timelimit--;
        }

        retry.SetActive(true);
        timelimitText.gameObject.SetActive(false);
        endText.gameObject.SetActive(true);
        InputBlocker.isBlocked = true; //操作を受け付けなくする
    }

    public void RetryButtom()
    {
        //シーンの再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
