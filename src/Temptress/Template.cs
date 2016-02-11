using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Temptress
{
    public class Template
    {
        public static Template<T> Create<T>(T type, string template) where T : class
        {
            return new Template<T>(template);
        }
    }

    public class Template<T> : TemplateBase, ITemplate, IHaveMergeItems, IHaveControlSections where T : class
    {
        public Template(string template) : base(template)
        {
            this.properties = typeof(T).GetProperties();
        }
 
        public Type GetDataType()
        {
            return typeof(T);
        }

    }

}
