using System;
using UnityEngine;
using UnityEngine.UI;

public class LetterHolder : LetterHolderBase
{

    [SerializeField] private HolderColor HolderColor;
    [SerializeField] private GameObject revealParticle;
    [SerializeField] private GameObject revealParticleEffect;
    
    [SerializeField] private GameObject letterParticle;
    [SerializeField] private GameObject finalParticle;
    [SerializeField] private GameObject finalLetterParticle;
    
    [SerializeField] private Image maskImage;
    [SerializeField] private UIGradient maskGradient;
    [SerializeField] private Image maskInnerShadowImage;
    [SerializeField] private Image dropShadowImage;
    [SerializeField] private Image innerShadowImage;

    private void OnValidate()
    {
        if(HolderColor == HolderColor.None)
            return;
        SetColor(HolderColor);
    }

    public void SetColor(LetterHolderColor color)
    {
        baseImage.color = color.baseColor;
        if (maskImage == null)
        {
            Debug.LogError("Mask image is NULL", gameObject);
        }
        maskImage.color = color.isGradient ? Color.white : color.maskColor;
        maskGradient.enabled = color.isGradient;
        
        if (color.isGradient)
        {
            maskGradient.m_color1 = color.maskColors[0];
            maskGradient.m_color2 = color.maskColors[1];
            maskGradient.UpdateGradient();
        }

        maskInnerShadowImage.color = color.maskInnerShadowColor;
        dropShadowImage.color = color.dropShadowColor;
        innerShadowImage.color = color.innerShadowColor;
    }

    protected override void OnColorChanged()
    {
        switch (CurrentColor)
        {
            case HolderColor.Default:
            case HolderColor.None:
                SetColor(LetterHolderColor.defaultColor);
                break;
            case HolderColor.LetterPowerup:
            case HolderColor.Selected:
                SetColor(LetterHolderColor.selected);
                break;
            case HolderColor.LinePowerup:
            case HolderColor.KeyboardPowerup:
            case HolderColor.SelectedLine:
            case HolderColor.RevealLettersPowerup:
                SetColor(LetterHolderColor.selectedLine);
                break;
            case HolderColor.InCorrect:
                SetColor(LetterHolderColor.inCorrect);
                break;
            case HolderColor.VerticalLine:
                SetColor(LetterHolderColor.verticalLine);
                break;
        }
    }

    public override void SetComplete(bool showParticle, bool lastLetterEdited = false)
    {
        base.SetComplete(showParticle, lastLetterEdited);
        letterParticle.gameObject.SetActive(showParticle);
    }

    public override void ShowFinalParticle(bool lastLetterEdited = false)
    {
        finalParticle.SetActive(!lastLetterEdited);
        finalLetterParticle.SetActive(lastLetterEdited);
    }

    public override void ShowRevealParticles()
    {
        revealParticle.SetActive(true);
        revealParticleEffect.SetActive(true);
    }
}