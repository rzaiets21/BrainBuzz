using UnityEngine;
using UnityEngine.UI;

public class RewardPowerupObject : RewardObject
{
    [SerializeField] private Image[] images;

    public RewardPowerupObject SetRewardType(int rewardType)
    {
        foreach (var image in images)
        {
            image.enabled = false;
        }

        images[rewardType].enabled = true;
        return this;
    }
}