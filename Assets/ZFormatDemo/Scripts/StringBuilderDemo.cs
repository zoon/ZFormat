#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

#endregion

[SuppressMessage("ReSharper", "FunctionNeverReturns")]
public class StringBuilderDemo : DemoBase
{
    private readonly NumberFormatInfo[] _formats = {
        new CultureInfo("en-US").NumberFormat,
        new CultureInfo("en-GB").NumberFormat,
        new CultureInfo("ja-JP").NumberFormat,
        new CultureInfo("es-ES").NumberFormat
    };

    private readonly StringBuilder _sbuilder = new StringBuilder(80);

    [UsedImplicitly]
    protected override void Awake()
    {
        // fix yen:
        _formats[2].CurrencySymbol = "¥";
    }

    protected override void FormatAll()
    {
        // Custom:
        _sbuilder.Length = 0;
        _sbuilder.Append("Custom: ").Append((Time.frameCount*100 + 5555551234).ToString("(###) ###-####"));
        CustomFormatText.text = _sbuilder.ToString();

        // Currency:
        var amount = Time.realtimeSinceStartup;
        for (var i = 0; i < Prices.Length; i++)
        {
            _sbuilder.Length = 0;
            _sbuilder.Append("Currency: ").Append(amount.ToString("C", _formats[i]));
            Prices[i].text = _sbuilder.ToString();
        }

        // Integer:
        _sbuilder.Length = 0;
        _sbuilder.Append("Integer:").Append(Time.frameCount.ToString("D6"));
        IntegerText.text = _sbuilder.ToString();

        // Float;
        _sbuilder.Length = 0;
        _sbuilder.Append("Float: ").Append(Mathf.Sin(Time.frameCount/1000f).ToString("F4"));
        FloatText.text = _sbuilder.ToString();

        // Scientific:
        _sbuilder.Length = 0;
        _sbuilder.Append("Scientific: ").Append(Mathf.Cos(Time.frameCount/1000f).ToString("E4"));
        ScientificText.text = _sbuilder.ToString();

        // Time to Format:
        if (_count > 0)
        {
            _sbuilder.Length = 0;
            _sbuilder
                .Append("Time to Format: ")
                .Append((_ticks/(_count*TimeSpan.TicksPerMillisecond/1000)).ToString("d3"))
                .Append(" μs");
            TimeToFormat.text = _sbuilder.ToString();
        }
    }

    [UsedImplicitly]
    public override void OnCLick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ZFormatDemo");
    }
}
