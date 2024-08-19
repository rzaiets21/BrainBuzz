using TMPro;
using UnityEngine;

public class RewardObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;

    public void SetCountText(int count)
    {
        countText.SetText(count.ToString());
    }
}