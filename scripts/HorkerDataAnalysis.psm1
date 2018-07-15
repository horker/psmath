Set-StrictMode -Version Latest

$METHOD_LIST = @(
  [PSCustomObject]@{
    ClassInfo = [Horker.DataAnalysis.ArrayMethods.LinqMethods]
    MethodNames = @(
      "Aggregate"
      "All"
      "Any"
      "AsEnumerable"
      "Concat"
      "Contains"
      "Count"
      "DefaultIfEmpty"
      "Distinct"
      "ElementAt"
      "ElementAtOrDefault"
      "Except"
      "First"
      "FirstOrDefault"
      "GroupBy"
      "GroupJoin"
      "Intersect"
      "Join"
      "Last"
      "LastOrDefault"
      "LongCount"
      "Max"
      "Min"
      "OrderBy"
      "OrderByDescending"
      "Reverse"
      "Select"
      "SelectMany"
      "SequenceEqual"
      "Single"
      "SingleOrDefault"
      "Skip"
      "SkipWhile"
      "Sum"
      "Take"
      "TakeWhile"
      "ToDictionary"
      "ToList"
      "ToLookup"
      "Union"
      "Where"
      "Zip"
    )
  }

  [PSCustomObject]@{
    ClassInfo = [Horker.DataAnalysis.ArrayMethods.MeasuresMethods]
    MethodNames = @(
      "ContraHarmonicMean"
      "Entropy"
      "ExponentialWeightedMean"
      "ExponentialWeightedVariance"
      "GeometricMean"
      "Kurtosis"
      "LogGeometricMean"
      "LowerQuartile"
      "Mean"
      "Median"
      "Mode"
      "Quantile"
      "Quantiles"
      "Quartiles"
      "Skewness"
      "StandardDeviation"
      "StandardError"
      "TruncatedMean"
      "UpperQuartile"
      "Variance"
    )
  }

  [PSCustomObject]@{
    ClassInfo = [Horker.DataAnalysis.ArrayMethods.ToolsMethods]
    MethodNames = @(
      "Center"
      "Rank"
      "Standardize"
      "Ties"
    )
  }

  [PSCustomObject]@{
    ClassInfo = [Horker.DataAnalysis.ArrayMethods.VectorMethods]
    MethodNames = @(
      "Sample"
      "Scale"
      "Shuffled"
      "Sorted"
    )
  }

  [PSCustomObject]@{
    ClassInfo = [Horker.DataAnalysis.ArrayMethods.MatrixMethods]
    MethodNames = @(
      "ArgMin"
      "ArgSort"
      "Bottom"
      "Cartesian"
      "Clear"
      "Concatenate"
      "Copy"
      "Cross"
      "CumulativeSum"
      "DistinctCount"
      "Expand"
      "Find"
      "First"
      "Get"
      "HasInfinity"
      "HasNaN"
      "IsEqual"
      "IsSorted"
      "Kronecker"
      "Normalize"
      "Null"
      "Outer"
      "Product"
      "Split"
      "Stack"
      "Swap"
      "Top"
      "Trim"
    )
  }
)

foreach ($l in $METHOD_LIST) {
  $ci = $l.ClassInfo
  foreach ($m in $l.MethodNames) {
    $mi = $ci.GetMethod($m)
    Update-TypeData -TypeName System.Array -MemberName $m -MemberType CodeMethod -Value $mi -Force
  }
}
