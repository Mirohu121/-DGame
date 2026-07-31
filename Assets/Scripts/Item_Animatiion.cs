using UnityEngine;
using DG.Tweening;


public class Item_Animatiion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Yé≤Ç…âiâìÇ…âÒì] (DOTween)
        transform.DORotate(new Vector3(0, 360, 0), 2.0f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Restart)
                 .SetEase(Ease.Linear);

        // è„â∫Ç…Ç”ÇÌÇ”ÇÌïÇóV (DOTween)
        transform.DOMoveY(transform.position.y + 0.5f, 1.0f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
