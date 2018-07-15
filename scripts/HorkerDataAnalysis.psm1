Set-StrictMode -Version Latest

$METHOD_LIST = @(
  [PSCustomObject]@{
    ClassInfo = [Horker.DataAnalysis.LinqMethods]
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
    ClassInfo = [Horker.DataAnalysis.MeasuresMethods]
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
    ClassInfo = [Horker.DataAnalysis.ToolsMethods]
    MethodNames = @(
      "Center"
      "Rank"
      "Standardize"
      "Ties"
    )
  }

  [PSCustomObject]@{
    ClassInfo = [Horker.DataAnalysis.VectorMethods]
    MethodNames = @(
      "Sample"
      "Scale"
      "Shuffled"
      "Sorted"
    )
  }
)

foreach ($l in $METHOD_LIST) {
  $ci = $l.ClassInfo
  foreach ($m in $l.MethodNames) {
    Write-Host $m
    $mi = $ci.GetMethod($m)
    Update-TypeData -TypeName System.Array -MemberName $m -MemberType CodeMethod -Value $mi -Force
  }
}
