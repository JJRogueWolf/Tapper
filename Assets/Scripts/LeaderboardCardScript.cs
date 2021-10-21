using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardCardScript : MonoBehaviour
{
    [SerializeField]
    private Sprite filledFrame;

    [SerializeField]
    private Image backgroundImage;

    public void setSelectedImage()
    {
        backgroundImage.sprite = filledFrame;
    }
}
