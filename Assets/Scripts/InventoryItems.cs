using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItems : MonoBehaviour
{
    public Image bucketImage;
    public TextMeshProUGUI bucketName;
    public Sprite invisibleSprite;

    private BucketData b;

    public void SetBucketPickedUp(BucketData bucket)
    {
        b = bucket;
        if (bucket == null)
        {
            bucketImage.sprite = invisibleSprite;
            bucketName.text = ""; // no bucket
        }
        else
        {
            if (bucket.emptyImage != null)
            {
                bucketImage.sprite = bucket.emptyImage;
            }
            else
            {
                // just use the invisble image I guess?
                bucketImage.sprite = invisibleSprite;
            }
            bucketName.text = bucket.bucketName;
        }
    }

    public void SetFull(bool full)
    {
        if (b == null)
        {
            return;
        }
        Sprite s = full ? b.fullImage : b.emptyImage;
        if (s == null)
        {
            s = b.fullImage;
        }
        if (s == null)
        {
            s = invisibleSprite;
        }
        bucketImage.sprite = s;
    }
}
