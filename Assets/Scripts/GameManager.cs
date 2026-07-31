using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;             
using System.Threading;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float timeLimit = 60f;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI resultText;

    private float timeLeft;
    private bool isGameActive = true;

    void Start()
    {
        timeLeft = timeLimit;

        var ct = this.GetCancellationTokenOnDestroy();

        GameLoopAsync(ct).Forget();
    }

    // ゲーム全体の流れをUniTaskで管理
    private async UniTaskVoid GameLoopAsync(CancellationToken ct)
    {
        // 1. カウントダウンループ
        while (timeLeft > 0 && isGameActive)
        {
            timeLeft -= Time.deltaTime;

            timeText.text = "Time: " + Mathf.CeilToInt(Mathf.Max(0, timeLeft)).ToString();

            if (timeLeft <= 10f && timeText.transform.localScale == Vector3.one)
            {
                timeText.color = Color.red;
                timeText.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f)
                         .SetLoops(-1, LoopType.Yoyo);
            }

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        TimeUp();
    }

    void TimeUp()
    {
        isGameActive = false;
        timeText.text = "Time: 0";

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = "TIME UP!";
            resultText.transform.localScale = Vector3.zero;
            resultText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        Debug.Log("ゲームオーバー！");
    }
    //外から呼び出し関数
    public void AddTime(float amount)
    {
        timeLeft += amount;

        timeText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f);
    }
}