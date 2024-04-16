using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Tile : LetterHolderBase
{
    [Serializable]
    private struct LetterHolderSpriteColor
    {
        public HolderColor Color;
        public Sprite Sprite;
    }

    [SerializeField] private HolderColor HolderColor;

    [SerializeField] private LetterHolderSpriteColor[] colors;
    
    [SerializeField] private GameObject letterParticle;
    [SerializeField] private GameObject revealLetterParticle;
    [SerializeField] private TileParticles completeRevealLetterParticle;

    private void OnValidate()
    {
        SetColor(HolderColor);
    }

    protected override void OnColorChanged()
    {
        if(CurrentColor == HolderColor.None)
            return;

        var color = colors.First(x => x.Color == CurrentColor);
        baseImage.sprite = color.Sprite;
    }

    public override void SetComplete(bool showParticle, bool lastLetterEdited = false)
    {
        base.SetComplete(showParticle, lastLetterEdited);
        letterParticle.SetActive(showParticle);
    }

    public override void ShowFinalParticle(bool lastLetterEdited = false)
    {
        TileParticlesController.Instance.ShowParticle(this, lastLetterEdited ? TileParticleType.CompleteParticleBlink : TileParticleType.CompleteParticle);
    }

    public override void ShowCompleteParticle()
    {
        TileParticlesController.Instance.ShowParticle(this, TileParticleType.CompleteLevel);
    }

    public override void ShowRevealParticles()
    {
        completeRevealLetterParticle.gameObject.SetActive(true);
        completeRevealLetterParticle.ShowParticle(TileParticleType.CompleteRevealAnimation,() =>
        {
            completeRevealLetterParticle.gameObject.SetActive(false);
            revealLetterParticle.SetActive(true);
        });
    }
}