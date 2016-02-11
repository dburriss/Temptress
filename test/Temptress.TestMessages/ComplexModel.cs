using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temptress.TestMessages
{
    public class ComplexModel
    {
        public int IntProp { get; set; }
        public int? NullIntProp { get; set; }
        public string SomeText { get; set; }
        public Address Address { get; set; }
        public Address Place { get; set; }

        public IEnumerable<string> BunchOfStrings { get; set; }
        public IEnumerable<Address> BunchOfAddresses { get; set; }
    }
}
