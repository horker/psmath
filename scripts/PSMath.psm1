Set-StrictMode -Version Latest

$METHOD_LIST = @(
  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.Linq[{0}]"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
    MethodNames = @(
      "Aggregate"
      "All"
      "Any"
      "AsEnumerable"
      "Concat"
      "Contains"
      "DefaultIfEmpty"
      "Distinct"
      "ElementAt"
      "ElementAtOrDefault"
      "Except"
      "First"
      "FirstOrDefault"
      #"GroupBy"
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
    ClassInfo = "Horker.Math.ArrayMethods.Untyped.Additional"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
    MethodNames = @(
      "DropNa"
      "DropNaN"
      "Histogram"
      "Summary"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.Additional[{0}]"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
    MethodNames = @(
      "Split"
      "Shuffle"
      "Slice"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Measures"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
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
      "StdDev"
      "StandardError"
      "TruncatedMean"
      "UpperQuartile"
      "Variance"
      "Var"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Tools"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
    MethodNames = @(
      "Center"
      "Rank"
      "Standardize"
      "Ties"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Vector"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
    MethodNames = @(
      "Sample"
      "Scale"
      "Sort"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.Matrix[{0}]"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
    MethodNames = @(
      "Clear"
      "Concatenate"
      "Copy"
      "DistinctCount"
      "Expand"
      "Find"
      "First"
      "Get"
      "IsEqual"
      "IsSorted"
      "Swap"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Untyped.Matrix"
    TargetClass = "Object", "Double", "Single", "Int64", "Int32", "Int16", "Byte", "SByte"
    MethodNames = @(
      "ArgMin"
      "ArgSort"
      "Bottom"
      "Cartesian"
      "Cross"
      "CumulativeSum"
      "HasInfinity"
      "HasNaN"
      "Kronecker"
      "Normalize"
      "Null"
      "Outer"
      "Product"
      "Stack"
      "Top"
      "Trim"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseObject"
    TargetClass = "Object"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseDouble"
    TargetClass = "Double"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseSingle"
    TargetClass = "Single"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseInt64"
    TargetClass = "Int64"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseInt32"
    TargetClass = "Int32"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseInt16"
    TargetClass = "Int16"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseByte"
    TargetClass = "Byte"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }

  [PSCustomObject]@{
    ClassInfo = "Horker.Math.ArrayMethods.Typed.ElementwiseSByte"
    TargetClass = "SByte"
    MethodNames = @(
      "ElementAdd"
      "ElementSubtract"
      "ElementMultiply"
      "ElementDivide"
      "ElementReminder"
      "ElementNegate"
    )
  }
)

foreach ($l in $METHOD_LIST) {
  $targets = $l.TargetClass
  foreach ($t in $targets) {
    $ci = Invoke-Expression "[$($l.ClassInfo -f $t)]"
    foreach ($m in $l.MethodNames) {
      $mi = $ci.GetMethod($m)
      Update-TypeData -TypeName "$t[]" -MemberName $m -MemberType CodeMethod -Value $mi -Force
    }
  }
}
