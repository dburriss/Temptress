using System.Collections.Generic;

namespace Temptress
{
    public interface IHaveMergeItems
    {
        IEnumerable<TemplateMergeItem> TemplateMergeItems { get; }
    }
}