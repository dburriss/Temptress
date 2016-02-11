using System.Collections.Generic;

namespace Temptress
{
    public interface IHaveControlSections
    {
        IEnumerable<ControlSection> ControlSections { get; }
    }
}