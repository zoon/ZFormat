#region Usings

using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using ZFormat;

#endregion

public class ZFormatDemo : DemoBase
{
    private readonly ZNumberFormatInfo[] _formats = {
        new ZNumberFormatInfo("en-US"),
        new ZNumberFormatInfo("en-GB"),
        new ZNumberFormatInfo("ja-JP"),
        new ZNumberFormatInfo("es-ES")
    };

    private readonly ZString _custom = new ZString(32);
    private readonly ZString _timeToFormat = new ZString(32);

    private readonly ZString[] _prices = {
        new ZString(32),
        new ZString(32),
        new ZString(32),
        new ZString(32),
    };

    private readonly ZString _integer = new ZString(32);
    private readonly ZString _float = new ZString(32);
    private readonly ZString _science = new ZString(32);

    [UsedImplicitly]
    protected override void Awake()
    {
        // fix yen:
        _formats[2].CurrencySymbol = "¥";
    }

    protected override void FormatAll()
    {
        var formatter = ZNumberFormatter.Instance;

        // Custom:
        formatter.NumberToChars("(###) ###-####", Time.frameCount*100 + 5555551234);
        _custom.Reset("Custom: ").Append(formatter);
        CustomFormatText.SetZString(_custom);

        // Currency:
        var amount = Time.realtimeSinceStartup;
        for (var i = 0; i < Prices.Length; i++)
        {
            formatter.NumberToChars("C", amount, _formats[i]);
            _prices[i].Reset("Currency: ").Append(formatter);
            Prices[i].SetZString(_prices[i]);
        }

        // Integer:
        formatter.NumberToChars("D6", Time.frameCount);
        _integer.Reset("Integer:").Append(formatter);
        IntegerText.SetZString(_integer);

        // Float:
        formatter.NumberToChars("F4", Mathf.Sin(Time.frameCount/1000f));
        _float.Reset("Float: ").Append(formatter);
        FloatText.SetZString(_float);

        // Scientific:
        formatter.NumberToChars("E4", Mathf.Cos(Time.frameCount/1000f));
        _science.Reset("Scientific: ").Append(formatter);
        ScientificText.SetZString(_science);

        // Time to Format:
        if (_count > 0)
        {
            formatter.NumberToChars("d3", _ticks/(_count*TimeSpan.TicksPerMillisecond/1000));
            _timeToFormat.Reset("Time to Format: ").Append(formatter).Append(" μs");
            TimeToFormat.SetZString(_timeToFormat);
        }
    }

    [UsedImplicitly]
    public override void OnCLick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StringFormatDemo");
    }
}

public static class TextEx
{
    /// <summary>
    /// API shim for using <see cref="Text"/> with mutable string
    /// </summary>
    public static void SetZString(this Text text, ZString zString)
    {
        text.text = zString.ToString();
        text.cachedTextGenerator.Invalidate();
        text.SetLayoutDirty();
        text.SetVerticesDirty();
    }

    /// <summary>
    /// Appends the string representation of a <see cref="ZNumberFormatter"/> instance to this instance.
    /// </summary>
    /// <returns>this</returns>
    public static ZString Append(this ZString zString, ZNumberFormatter formatter)
    {
        return zString.Append(formatter.Chars, formatter.Count);
    }
}
