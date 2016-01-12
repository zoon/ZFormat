#region Usings

using System;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

#endregion

public class StringFormatDemo : DemoBase
{
    private readonly NumberFormatInfo[] _formats = {
        new CultureInfo("en-US").NumberFormat,
        new CultureInfo("en-GB").NumberFormat,
        new CultureInfo("ja-JP").NumberFormat,
        new CultureInfo("es-ES").NumberFormat
    };

    [UsedImplicitly]
    protected override void Awake()
    {
        // fix yen:
        _formats[2].CurrencySymbol = "¥";
    }

    protected override void FormatAll()
    {
        // Custom:
        CustomFormatText.text = string.Format("Custom: {0:(###) ###-####}", Time.frameCount*100 + 5555551234);

        // Currency:
        var amount = Time.realtimeSinceStartup;
        for (var i = 0; i < Prices.Length; i++)
        {
            Prices[i].text = string.Format(_formats[i], "Currency: {0:C}", amount);
        }

        // Integer:
        IntegerText.text = string.Format("Integer: {0:D6}", Time.frameCount);

        // Float;
        FloatText.text = string.Format("Float: {0:F4}", Mathf.Sin(Time.frameCount/1000f));

        // Scientific:
        ScientificText.text = string.Format("Scientific: {0:E4}", Mathf.Cos(Time.frameCount/1000f));

        // Time to Format:
        if (_count > 0)
        {
            TimeToFormat.text = string.Format("Time to Format: {0:d3} μs",
                _ticks/(_count*TimeSpan.TicksPerMillisecond/1000));
        }
    }

    [UsedImplicitly]
    public override void OnCLick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StringBuilderDemo");
    }
}
