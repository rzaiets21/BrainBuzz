using UnityEngine;

public class LoadingScreen : Screen
{
    [SerializeField] private ProgressBar progressBar;

    protected override void OnShown()
    {
        UpdateProgress(0f);
    }

    public void UpdateProgress(float progress)
    {
        progressBar.SetValue(progress);
    }
}