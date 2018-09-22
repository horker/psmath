using System;
using System.Collections.Generic;

namespace Horker.Math
{
    public interface IDataFrameColumn
    {
        int Count { get; }

        DataFrame ToDummyValues(string baseName, CodificationType codificationType = CodificationType.OneHot);
    }
}
