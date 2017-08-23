# ZFormat

## Unity3D .NET library for formatting numbers without memory allocation

----

### TL;DR

The `ZFormat` library is a possibility to do all kinds of number formatting,
including Currency and Custom, and to communicate with APIs expecting
System.String, with zero dynamic memory allocation.

### Demo

[Unity3D WEBGL demo.](http://zoon.github.io/ZFormat/zformat_wgl/)

### Motivation

For a long time I've been bothered by the fact that there is no standard way
to convert a number into a formatted string without memory allocation. Not
only every Format* method in .NET returns a freshly allocated string, but, in
addition, it allocates memory for various inner buffers, clone the
`NumberFormatInfo` class, etc.

Assume that you want to show an FPS counter on the screen, and you write
something like this:

```csharp
FPSCounter.text = string.Format("{0:F2}", fps);
```

This will cause memory allocation, 0.25KB - seems like no big deal, but if
there will be memory allocation, however small, for every frame, sooner or
later GC is  going to reclaim garbage, and there will be a noticeable pause in
the game. Due to the fact that, historically, Unity3D uses conservative
non-generational GC, the  duration of the pause is determined by the total
size of the mono heap, that, even in a mobile game, can reach tens of megabytes
(and the pause, accordingly, tens of milliseconds). Another issue is the
subsequent [memory heap fragmentation][Frag].

Within the past years the community have developed some guidelines for memory
management. But the situation with strings has not changed much. For the
reason that strings in .NET are immutable, you can't use "pool-of-strings";
for the reason that many APIs are expecting strings, you can't use
`pool-of-StringBuilder` or  `pool-of-List<Char>` without eventually
converting them into a string. But even if this issue is resolved, it
will still be impossible to use standard formatting methods (i.e., <nobr>
`xxx.ToString(String, IFormatProvider)` </nobr>), because they, too, allocate
memory.

### Quick Start

`ZFormat`'s API is somewhat bulky but flexible and non-coupled. It comprises
some extension methods that make things sleeker, but they are not shown here
for the sake of clarity:

```csharp
private readonly ZString _price = new ZString();
private readonly _esFormatInfo = new ZNumberFormatInfo("es-ES");
...
...
// there are no memory allocations after this point
var formatter = ZNumberFormatter.Instance;

formatter.NumberToChars("C2", 12.345, _esFormatInfo);

_price.Reset(formatter.Chars,formatter.Count);

// using it with uGUI Text
PriceLabel.text = _price.ToString();

// tell uGUI to redraw this text label (should be an extension method)
PriceLabel.SetVerticesDirty();
PriceLabel.SetLayoutDirty();
PriceLabel.cachedTextGenerator.Invalidate();

// now you get '12,12 €' on screen
```

### Features

`ZFormat` is 100% compatible with all .NET [standard][StandardFS] and
[custom][CustomFS] format specifiers. It's based on mono's `NumberFormatter`
and correctly passes all (hundreds of) tests.

`ZFormat` is fully culture-aware and supports all culture information
available in Unity3D runtime.

`ZFormat` supports either mono runtime or IL2CPP.

`ZString` is a growable mutable string with an interface similar to
`StringBuilder`. Its ToString method returns the reference to its internal
string buffer, and can be used to communicate with API expecting
`System.String`.

```csharp
var zstr = new ZString(8);
zstr.Clear();
zstr.Append('H').Append("ello").Append(new[]{'!','\n'});
Debug.Log(zstr.ToString());
```

### Performance

Performance is about the same with StringBuilder + Format combination. It
may be slightly better than `{0, XX:YY}` `String.Format` methods.

### Limitations

- `ZFormat` extensively uses unsafe code and pointers, and therefore can't be
supported by WebPlayer (now deprecated, though).

- Round-trip formatting still allocate some bytes, because mono's Double.Parse
does it. It's not easily fixable. However, I consider its usages rather rare.

- `ZFormat` does not support (yet?) `{0,XX:YY}` formatting and paddings.

- `ZFormat` is not thread-safe. You should serialize access to the
`ZNumberFormatter.Instance` by yourself ([Unity3D does not support
`ThreadStatic`][ThreadStatic])

### API

#### class ZNumberFormatter

The ZNumberFormatter class implements methods for formatting numeric values
and returns the result of formatting via two instance properties: `Chars` and
`Count`. To format numeric values, applications should use the 'NumberToChars'
methods.

- **Formatting Methods**:
Converts the numeric value into the reference to the character buffer and
characters count using the specified format. Formatting methods are all of
the form:

```csharp
void NumberToChars(XXX n);
void NumberToChars(string format, XXX n);
void NumberToChars(string format, XXX n, ZNumberFormatInfo info)
```

  Where `XXX` is the name of the particular numeric class: `Int32`, `UInt32`,
  `Int64`, `UInt64`, `Single`, `Double`, and `Decimal`.

  Format string supplies format parameters. If the format parameter is `null`
  or empty string, the number is formatted with the "G" (general format)
  specified. [Standard][StandardFS] and [user-defined][CustomFS] format
  strings are the common .NET format strings, their documentation available
  at MSDN.

  The `ZNumberFormatInfo` supplies such information as the characters to use
  for decimal and thousand separators, and the spelling and placement of
  currency symbols in monetary values.

- **Properties**

  - `char[] Chars`
  - `int Count`

    `Chars` and `Count` properties return the result of formatting in the form of buffer+count.

#### class ZString
Mutable, growable string with API that is loosely based on [`System.Text.StringBuilder`][StringBuilder].

- **Constructors**
    - `ZString(int maxCapacity = 16)`
- **Properties**
    - `int Capacity`
    - `int Length`
- **Methods**
    - `void Clear()`

#### class ZNumberFormatInfo
Provides culture-specific information for formatting numeric values.
Simplified, non-allocating version of [NumberFormatInfo][NumberFormatInfo]

- **Constructors**
  - `ZNumberFormatInfo()` returns an invariant instance.
  - `ZNumberFormatInfo(string culture)` returns an instance based on the culture
specified by name.
  - `ZNumberFormatInfo(NumberFormatInfo nfi)` returns an instance based on
[`NumberFormatInfo`][NumberFormatInfo].

- **Properties**
  - `int CurrencyDecimalDigits` Gets or sets the number of decimal places to use
in currency values.
  - `string CurrencyDecimalSeparator`  Gets or sets the string to use as the
 decimal separator in currency values.
  - `string CurrencyGroupSeparator`	Gets or sets the string that separates groups
of digits to the left of the decimal in currency values.
  - `int[] CurrencyGroupSizes` Gets or sets the number of digits in each group to
the left of the decimal in currency values.
  - `int CurrencyNegativePattern` Gets or sets the format pattern for negative
currency values.
  - `int CurrencyPositivePattern` Gets or sets the format pattern for positive
currency values.
  - `string CurrencySymbol` Gets or sets the string to use as the currency
 symbol.
  - `string NaNSymbol` Gets or sets the string that represents the IEEE NaN (not a
number) value.
  - `string NegativeInfinitySymbol` Gets or sets the string that represents
negative infinity.
  - `string NegativeSign` Gets or sets the string that denotes that the associated
number is negative.
  - `int NumberDecimalDigits` Gets or sets the number of decimal places to use in
numeric values.
  - `string NumberDecimalSeparator` Gets or sets the string to use as the decimal
 separator in numeric values.
  - `string NumberGroupSeparator` Gets or sets the string that separates groups of
digits to the left of the decimal in numeric values.
  - `int[] NumberGroupSizes` Gets or sets the number of digits in each group to
the left of the decimal in numeric values.
  - `int NumberNegativePattern` Gets or sets the format pattern for negative
numeric values.
  - `int PercentDecimalDigits` Gets or sets the number of decimal places to use in
percent values.
  - `string PercentDecimalSeparator` Gets or sets the string to use as the decimal
separator in percent values.
  - `string PercentGroupSeparator` Gets or sets the string that separates groups
of digits to the left of the decimal in percent values.
  - `int[] PercentGroupSizes` Gets or sets the number of digits in each group to
the left of the decimal in percent values.
  - `int PercentNegativePattern` Gets or sets the format pattern for negative
percent values.
  - `int PercentPositivePattern` Gets or sets the format pattern for positive
percent values.
  - `string PercentSymbol` Gets or sets the string to use as the percent symbol.
  - `string PerMilleSymbol` Gets or sets the string to use as the per mille
symbol.
  - `string PositiveInfinitySymbol` Gets or sets the string that represents
positive infinity.
  - `string PositiveSign` Gets or sets the string that denotes that the associated
number is positive.


### Formatting Strings Examples

Some examples of standard format strings and their results are shown in the
table below. (The examples all assume a default `ZNumberFormatInfo`.)

```
Value        Format  Result
12345.6789   C       $12,345.68
-12345.6789  C       ($12,345.68)
12345        D       12345
12345        D8      00012345
12345.6789   E       1.234568E+004
12345.6789   E10     1.2345678900E+004
12345.6789   e4      1.2346e+004
12345.6789   F       12345.68
12345.6789   F0      12346
12345.6789   F6      12345.678900
12345.6789   G       12345.6789
12345.6789   G7      12345.68
123456789    G7      1.234568E8
12345.6789   N       12,345.68
123456789    N4      123,456,789.0000
0x2c45e      x       2c45e
0x2c45e      X       2C45E
0x2c45e      X8      0002C45E
```

--------------------

[Frag]: http://stackoverflow.com/questions/3770457/what-is-memory-fragmentation

[Evo]: https://evo.my.com/en/?lang=en_US

[Geldreich]: http://www.gamasutra.com/blogs/RichGeldreich/20150731/250071/

[Reich]: http://www.gamasutra.com/blogs/WendelinReich/20131109/203841/

[UnityAPI]: http://docs.unity3d.com/ScriptReference/Mesh.SetVertices.html

[IL2CPP_GC]:http://blogs.unity3d.com/2015/07/09/il2cpp-internals-garbage-collector-integration/

[StandardFS]: https://msdn.microsoft.com/en-us/library/dwhawy9k

[CustomFS]: https://msdn.microsoft.com/en-us/library/0c899ak8

[ThreadStatic]: http://docs.unity3d.com/Manual/Attributes.html

[NumberFormatInfo]: https://msdn.microsoft.com/library/system.globalization.numberformatinfo

[StringBuilder]: https://msdn.microsoft.com/en-us/library/system.text.stringbuilder

