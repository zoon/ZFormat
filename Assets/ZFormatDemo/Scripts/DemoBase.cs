#region Usings

using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

#endregion

public abstract class DemoBase : MonoBehaviour
{
    #region Editor

    [UsedImplicitly] public Text CustomFormatText;
    [UsedImplicitly] public Text TimeToFormat;
    [UsedImplicitly] public Text[] Prices;
    [UsedImplicitly] public Text IntegerText;
    [UsedImplicitly] public Text FloatText;
    [UsedImplicitly] public Text ScientificText;

    #endregion

    private readonly Stopwatch _sw = new Stopwatch();
    protected long _count;
    protected long _ticks;

    #region Unity Callbacks

    [UsedImplicitly]
    protected abstract void Awake();

    [UsedImplicitly]
    protected IEnumerator Start()
    {
        // warm-up
        for (var i = 0; i < 2; i++)
        {
            FormatAll();
        }
        while (true)
        {
            _sw.Start();

            Profiler.BeginSample("[FormatAll]");
            {
                FormatAll();
            }
            Profiler.EndSample();

            ++_count;
            _ticks += _sw.ElapsedTicks;
            _sw.Reset();
            yield return null;
        }
        // ReSharper disable once FunctionNeverReturns
    }

    protected abstract void FormatAll();

    #endregion

    [UsedImplicitly]
    public abstract void OnCLick();
}
