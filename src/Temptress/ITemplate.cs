using System;
using System.Collections.Generic;

namespace Temptress
{
    public interface ITemplate
    {
        bool IsValid();

        IEnumerable<string> MergeNames(); // eg. FirstName

        IEnumerable<string> MergeValues(); // eg. {{FirstName}}

        IEnumerable<string> FriendlyMergeLabels(); // eg. First Name

        IEnumerable<MergeOption> MergeOptions();

        string TemplateContent { get; }

        bool HasControlSections { get; }

        Type GetDataType();
    }
}