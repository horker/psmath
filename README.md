# psmath: PowerShell Math and Statistics Library

This library provides a collection of math and statistics classes and cmdlets for PowerShell. It is built on top of [Accord.NET](http://accord-framework.net/).

This is a work in progress but already usable for many use cases.

## Installation

The library is published in [PowerShell Gallery](https://www.powershellgallery.com/packages/psmath)
```PowerShell
Install-Module psmath
```

## Features

Each cmdlet in the library comes along with a short alias prefixed with `math.` or `st.` and you can use it instead of the full cmdlet name. For example, you can use `math.sin` instead of `Get-Math.Sin`.

### Mathematical and Statistical Functions

```PowerShell
PS C:\> math.sin (3 / 4 * (math.pi))
0.707106781186548

PS C:\> 1..5 | math.sqrt
1
1.4142135623731
1.73205080756888
2
2.236067977499791

PS C:\> 1..99 | math.median
50

PS C:\> 1..99 | math.var -Unbiased
825

PS C:\> 1..99 | math.summary

Name          :
Count         : 99
Unique        : 99
Minimum       : 1
LowerQuantile : 25
Median        : 50
Mean          : 50
UpperQuantile : 75
Maximum       : 99
```

Cmdlets: `math.abs` `math.acos` `math.argmax` `math.argmin` `math.argsort` `math.asin` `math.atan` `math.atan2` `math.binomial` `math.ceiling` `math.center` `math.choose` `math.contraharmonicmean` `math.cos` `math.cosh` `math.cumsum` `math.diff` `math.distinct` `math.distinctCount` `math.divrem` `math.e` `math.entropy` `math.ewmean` `math.ewvar` `math.exp` `math.factorial` `math.floor` `math.geometric` `math.hist` `math.issorted` `math.kalman` `math.kurtosis` `math.log` `math.log10` `math.loggeometric` `math.lowerq` `math.max` `math.mean` `math.median` `math.min` `math.mode` `math.movingmean` `math.movingmmax` `math.movingmmin` `math.movingstdev` `math.movingsum` `math.movingvar` `math.normalize` `math.pi` `math.pow` `math.quantile` `math.randomseed` `math.rank` `math.rem` `math.reverse` `math.round` `math.sample` `math.scale` `math.se` `math.shuffle` `math.sigmoid` `math.sign` `math.sin` `math.sinh` `math.skewness` `math.softmax` `math.sqrt` `math.standardize` `math.stdev` `math.sum` `math.summary` `math.tan` `math.tanh` `math.ties` `math.truncate` `math.truncatedmean` `math.truthtable` `math.upperq` `math.var` `math.zscore`

### Sequence Generators

Sequence generators similar to NumPy's `arange` and `linspace`.

```PowerShell
PS C:\> seq 0 5 1.5
0
1.5
3
4.5

PS C:\> $seq = seq.linspace 1 60
PS C:\> $seq -join "`t"
0       0.0166666666666667      0.0333333333333333      0.05    0.0666666666666667      0.0833333333333333
0.1     0.116666666666667       0.133333333333333       0.15    0.166666666666667       0.183333333333333
0.2     0.216666666666667       0.233333333333333       0.25    0.266666666666667       0.283333333333333
0.3     0.316666666666667       0.333333333333333       0.35    0.366666666666667       0.383333333333333
0.4     0.416666666666667       0.433333333333333       0.45    0.466666666666667       0.483333333333333
0.5     0.516666666666667       0.533333333333333       0.55    0.566666666666667       0.583333333333333
0.6     0.616666666666667       0.633333333333333       0.65    0.666666666666667       0.683333333333333
0.7     0.716666666666667       0.733333333333333       0.75    0.766666666666667       0.783333333333333
0.8     0.816666666666667       0.833333333333333       0.85    0.866666666666667       0.883333333333333
0.9     0.916666666666667       0.933333333333333       0.95    0.966666666666667       0.983333333333333

PS C:\> seq 10 20 2 -func { $x * $x }, { math.sqrt $x }

 x  y0               y1
 -  --               --
10 100 3.16227766016838
12 144 3.46410161513775
14 196 3.74165738677394
16 256                4
18 324 4.24264068711928

```

Cmdlets: `seq` `seq.linspace` `seq.values` `seq.zeros`

### Array Methods

The methods of `System.Linq.Enumerable` and Accord.NET classes are defined in the Array class.

```PowerShell
PS C:\> (0..99).Mean()
49.5

PS C:\> "A quick brown fox jumps over the lazy dog".Split().Aggregate({ $args[1] + " - " + $args[0] })
dog - lazy - the - over - jumps - fox - brown - quick - A
```

Methods: `Aggregate` `All` `Any` `ArgMin` `ArgSort` `AsEnumerable` `Bottom` `Cartesian` `Center` `Clear` `Concat` `Concatenate` `Contains` `ContraHarmonicMean` `Copy` `Cross` `CumulativeSum` `DefaultIfEmpty` `Distinct` `DistinctCount` `DropNa` `DropNaN` `ElementAt` `ElementAtOrDefault` `Entropy` `Except` `Expand` `ExponentialWeightedMean` `ExponentialWeightedVariance` `Find` `First` `FirstOrDefault` `GeometricMean` `Get` `GroupBy` `GroupJoin` `HasInfinity` `HasNaN` `Intersect` `IsEqual` `IsSorted` `Join` `Kronecker` `Kurtosis` `Last` `LastOrDefault` `LogGeometricMean` `LongCount` `LowerQuartile` `Max` `Mean` `Median` `Min` `Mode` `Normalize` `Null` `OrderBy` `OrderByDescending` `Outer` `Product` `Quantile` `Quantiles` `Quartiles` `Rank` `Reverse` `Sample` `Scale` `Select` `SelectMany` `SequenceEqual` `Shuffled` `Single` `SingleOrDefault` `Skewness` `Skip` `SkipWhile` `Sorted` `Split` `Stack` `StandardDeviation` `StandardError` `Standardize` `Stdev` `Sum` `Swap` `Take` `TakeWhile` `Ties` `ToDictionary` `ToList` `ToLookup` `Top` `Trim` `TruncatedMean` `Union` `UpperQuartile` `Var` `Variance` `Where` `Zip`

### Matrix

The `Matrix` class represents a matrix and offers a reasonable output format and useful methods. You can create `Matrix` objects by the `mat` cmdlet (`New-Math.Matrix`) and variants (`mat.identity`, `mat.diagonal` and so on).

```PowerShell
PS C:\> $m = mat 1,2,3,4 3 3
PS C:\> $m

[3 x 3]
 1 4 3
 2 1 4
 3 2 1

PS C:\> $m.Inv

[3 x 3]
 -0.19444  0.05556  0.36111
  0.27778 -0.22222  0.05556
  0.02778  0.27778 -0.19444

PS C:\> $svd = $m.Svd()
PS C:\> $svd | fl

U      : [3 x 3] [ 0.67552 0.56617 -0.47235 ] [ 0.58632 -0.80092 -0.12149 ] [ 0.4471 0.19488 0.873 ]
D      : [3 x 3] [ 7.13366 0 0 ] [ 0 2.31427 0 ] [ 0 0 2.1806 ]
V      : [3 x 3] [ 0.4471 -0.19488 0.873 ] [ 0.58632 0.80092 -0.12149 ] [ 0.67552 -0.56617 -0.47235 ]
Source : Accord.Math.Decompositions.SingularValueDecomposition

PS C:\> $svd.U * $svd.D * $svd.V.T

[3 x 3]
 1 4 3
 2 1 4
 3 2 1
```

Cmdlets: `mat.diagonal` `mat.fromobject` `mat.identity` `mat.magic` `mat.mesh` `mat.meshgrid` `mat.onehot` `mat.truthtable` `mat.zeros`

Methods and properties: `Abs` `Add` `Apply` `ApplyInPlace` `ArgMax` `ArgMin` `ArgSort` `AsString` `Bottom` `Cartesian` `Ceiling` `Cholesky` `Clone` `Combinations` `Concatenate` `Convolve` `Correlation` `Count` `Covariance` `Cross` `CumulativeSum` `Decompose` `Determinant` `Distinct` `DistinctCount` `Divide` `DivideByDiagonal` `Dot` `Eigenvalue` `Entropy` `Equals` `Exp` `Expand` `ExponentialWeightedCovariance` `Find` `First` `FirstOrNull` `Flatten` `Floor` `GetColumn` `GetColumns` `GetHashCode` `GetLowerTriangle` `GetNumberOfElements` `GetRange` `GetRow` `GetRows` `GetSymmetric` `GetTotalLength` `GetType` `GetUpperTriangle` `gramSchmidtOrthogonalizationWrapper` `HasInfinity` `HasNaN` `Inverse` `IsDiagonal` `IsLowerTriangular` `IsPositiveDefinite` `IsSingular` `IsSymmetric` `IsUpperTriangular` `Kronecker` `Last` `Log` `LogDeterminant` `LogPseudoDeterminant` `Lu` `Max` `Mean` `Median` `Merge` `Min` `Mode` `Multiply` `Nmf` `Normalize` `Null` `OneHot` `Outer` `Pad` `Permutations` `Pow` `Product` `PseudoDeterminant` `Qr` `Rank` `Reversed` `Round` `Shuffle` `Sign` `SignedPow` `SignSqrt` `Skewness` `Solve` `Sort` `Split` `Sqrt` `Stack` `StanardDeviation` `StanardError` `Subsets` `Subtract` `Sum` `Svd` `ToFlatArray` `ToJagged` `Top` `ToPSObject` `ToString` `Trace` `Transpose` `Variance` `WeightedCovariance` `Whitening` `ZScores` `Item` `Columns` `Inv` `Rows` `T` `Values`

### Probability Distributions

```PowerShell
PS C:\> $n = st.normal -Mean 10 -StdDev 3
PS C:\> $n

Mean              : 10
Median            : 10
Variance          : 9
StandardDeviation : 3
Mode              : 10
Skewness          : 0
Kurtosis          : 0
Support           : [-∞, ∞]
Entropy           : 2.51755082187278
Quartiles         : [7.97653074941176, 12.0234692505882]

PS C:\> $n.Generate(3)
6.70926907721133
7.16162377748582
15.2954486068024

PS C:\> seq 7 13 .01 -func { $n.pdf($x) }

   x                 y0
   -                 --
   7 0.0806569081730478
7.01 0.0809257635370322
7.02 0.0811946129148634
7.03 0.0814634503023332
7.04 0.0817322696757198
          :
```

Cmdlets: `st.bernoulli` `st.beta` `st.binomial` `st.cauchy` `st.chisquire` `st.empirical` `st.exponential` `st.f` `st.gamma` `st.geometric` `st.hypergeo` `st.logistic` `st.lognormal` `st.negbinomial` `st.normal` `st.poison` `st.t` `st.uniform` `st.uniformd` `st.weibull`

### Hypothesis Testing

```PowerShell
PS C:\> $samples = (st.normal 10 9).generate(20)
PS C:\> $samples | Invoke-TTest -HypothesizedMean 0 -Alternate Different

Analysis              : 0.999782277992546
StandardError         : 1.98120573653148
EstimatedValue        : 11.4688217870095
HypothesizedValue     : 0
Confidence            : [7.3221105237174, 15.6155330503016]
Hypothesis            : ValueIsDifferentFromHypothesis
StatisticDistribution : T(x; df = 19)
PValue                : 1.40972806286221E-05
Statistic             : 5.78880909515642
Tail                  : TwoTail
Size                  : 0.05
Significant           : True
CriticalValue         : 2.09302405440831
```

Cmdlets: `Invoke-BinomialTest` `Invoke-ChiSquareTest` `Invoke-FisherExactTest` `Invoke-FTest` `Invoke-KolmogorovSmirnovTest` `Invoke-LeveneTest` `Invoke-MannWhitneyWilcoxonTest` `Invoke-PairedTTest` `Invoke-ShapiroWilkTest` `Invoke-SignTest` `Invoke-TTest` `Invoke-TwoSampleKolmogorovSmirnovTest` `Invoke-TwoSampleSignTest` `Invoke-TwoSampleTTest` `Invoke-TwoSampleWilcoxonSignedRankTest` `Invoke-WilcoxonSignedRankTest`

### Regressions

(particularly WIP)

```PowerShell
PS C:\> $samples = seq 0 10 1 -func { 2 * $x + (st.normal).generate() + 10 }
PS C:\> $samples

x               y0
-               --
0 12.4668147765386
1 12.3778237651562
2 13.5261567687156
3 16.6537265658641
4 19.4478735361853
5 21.6786215694439
6 20.8376203580729
7 24.4205486696051
8 25.2553007079806
9 26.5474208799279

PS C:\> $r = $samples | Invoke-LinearRegression -OutputName y0
PS C:\> $r.Regression.ToString()
y(x0) = 3.55268307161757*x0

PS C:\> $r.RSquareAdjusted
-0.719608861231189
```

Cmdlets: `Invoke-GeneralizedLinearRegression` `Invoke-LinearRegression` `Invoke-LogisticRegression` `Invoke-StepwiseLogisticRegression`

### Type Converters

Flexible type conversion cmdlets.

```PowerShell
PS C:\> math.todouble '$3.99', '1,000,000', '3e-4'
3.99
1000000
0.0003

PS C:\> math.todatetime 1998/3/7, Aug-7, "Sunday, May 7, 2017"
3/7/1998 12:00:00 AM
8/7/2018 12:00:00 AM
5/7/2017 12:00:00 AM
```

Cmdlets: `math.fromunixtime` `math.todatetime` `math.todatetimeoffset` `math.todouble` `math.tounixtime`

## License

This library is licensed under the GNU Lesser General Public License version 2.1 or later.
