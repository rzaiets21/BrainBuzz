using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public struct LetterHolderColor
{
    public bool isGradient;

    public Color maskColor => maskColors[0];
    public Color[] maskColors;

    public Color baseColor;
    public Color innerShadowColor;
    public Color dropShadowColor;
    public Color maskInnerShadowColor;

    public static LetterHolderColor selected
    {
        get
        {
            return new LetterHolderColor()
            {
                isGradient = true,
                maskColors = new Color[2]
                {
                    new Color(1, 0.8392157f, 0),
                    new Color(1, 0.7215686f, 0)
                },
                
                baseColor = new Color(1, 0.4196078f, 0),
                maskInnerShadowColor = new Color(1, 0.5411765f, 0),
                dropShadowColor = new Color(1, 0.8235294f, 0),
                innerShadowColor = new Color(1, 0.9294118f, 0.8627451f)
            };
        }
    }

    public static LetterHolderColor selectedLine
    {
        get
        {
            return new LetterHolderColor()
            {
                isGradient = false,
                maskColors = new Color[1]
                {
                    new Color(1, 0.8196079f, 0.5215687f)
                },
                
                baseColor = new Color(0.9764706f, 0.7294118f, 0.3176471f),
                maskInnerShadowColor = new Color(0.8588235f, 0.4627451f, 0.09411765f),
                dropShadowColor = new Color(0.945098f, 0.8039216f, 0.6784314f),
                innerShadowColor = new Color(1, 0.8980392f, 0.7333333f)
            };
        }
    }
    
    public static LetterHolderColor defaultColor
    {
        get
        {
            return new LetterHolderColor()
            {
                isGradient = true,
                maskColors = new Color[2]
                {
                    new Color(1, 0.8980392f, 0.8039216f),
                    new Color(1, 0.8117647f, 0.6705883f)
                },
                
                baseColor = new Color(1, 0.6352941f, 0.3019608f),
                maskInnerShadowColor = new Color(0.7647059f, 0.4470588f, 0.1529412f),
                dropShadowColor = new Color(0.945098f, 0.8039216f, 0.6784314f),
                innerShadowColor = new Color(1, 0.9294118f, 0.8627451f)
            };
        }
    }
    
    public static LetterHolderColor inCorrect
    {
        get
        {
            return new LetterHolderColor()
            {
                isGradient = false,
                maskColors = new Color[1]
                {
                    new Color(1, 0.4666667f, 0.4666667f)
                },
                
                baseColor = new Color(0.8431373f, 0.3529412f, 0.3529412f),
                maskInnerShadowColor = new Color(0.5803922f, 0.1803922f, 0.1803922f),
                dropShadowColor = new Color(0.8352941f, 0.4313726f, 0.4313726f),
                innerShadowColor = new Color(1, 0.6313726f, 0.6313726f)
            };
        }
    }
    
    public static LetterHolderColor verticalLine
    {
        get
        {
            return new LetterHolderColor()
            {
                isGradient = false,
                maskColors = new Color[1]
                {
                    new Color(1, 0.6980392f, 0.6980392f)
                },
                
                baseColor = new Color(0.9921569f, 0.5607843f, 0.5607843f),
                maskInnerShadowColor = new Color(0.5803922f, 0.1803922f, 0.1803922f),
                dropShadowColor = new Color(0.9098039f, 0.5882353f, 0.5882353f),
                innerShadowColor = new Color(1, 0.7686275f, 0.7686275f)
            };
        }
    }
}