using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ChimpLab.PhilosophicalMonkey;

namespace Temptress
{
    public abstract class TemplateBase
    {
        protected const string Pattern = @"{{([^{}]+)}}";
        protected const string ForeachControlSectionPattern = @"(<<foreach:([^{}]+)\b[^>]*>>(.*?)<</foreach:([^{}]+)>>)";//<<foreach:([^{}]+)\b[^>]*>>(.*?)<</foreach:([^{}]+)>>
        protected readonly Regex regex;
        protected readonly Regex controlSectionRegex;
        protected PropertyInfo[] properties;

        public string TemplateContent { get; private set; }

        protected TemplateBase(string template)
        {
            regex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            controlSectionRegex = new Regex(ForeachControlSectionPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            TemplateContent = template;
        }

        public IEnumerable<TemplateMergeItem> TemplateMergeItems
        {
            get
            {
                if(templateItems == null || !templateItems.IsValueCreated)
                    GetTemplateMergeItems();

                return templateItems.Value;
            }
        }

        public bool HasControlSections
        {
            get
            {
                return ControlSections.Any();
            }
        }

        public IEnumerable<ControlSection> ControlSections
        {
            get
            {
                return GetControlSections();
            }
        }

        public bool IsValid()
        {
            //Lazy<bool> isValid = new Lazy<bool>(() =>
            //{
                foreach (var mergeValue in this.TemplateMergeItems)
                {
                    if (!this.MergeNames().Any(x => x.Equals(mergeValue.Value)))
                        return false;
                }
                return true;
            //});
            //return isValid.Value;
        }

        #region Working with Control Sections

        protected Lazy<IEnumerable<ControlSection>> controlSections;

        protected IEnumerable<ControlSection> GetControlSections()
        {
            this.controlSections = new Lazy<IEnumerable<ControlSection>>(
                () =>
                {
                    IEnumerable<Capture> captures = RegexTextReplacementValues(this.controlSectionRegex, this.TemplateContent);
                    var captureArr = captures.ToArray();

                    if (captureArr.Length == 0)
                        return new List<ControlSection>();

                    IEnumerable<Temptress.ControlSection> result = new List<ControlSection>
                    {
                        new ControlSection
                        {
                            Index = captureArr[0].Index,
                            Length = captureArr[0].Length,
                            Value = captureArr[0].Value,
                            PropertyName = captureArr[1].Value,
                            TemplateText = captureArr[2].Value
                        }
                    };

                    return result;
                });

            return controlSections.Value;
        }

        #endregion

        #region Working with TemplateContent

        protected Lazy<IEnumerable<TemplateMergeItem>> templateItems;

        protected IEnumerable<TemplateMergeItem> GetTemplateMergeItems()
        {
            this.templateItems = new Lazy<IEnumerable<TemplateMergeItem>>(() =>
            {
                IEnumerable<Capture> captures = RegexTextReplacementValues(this.regex, this.TemplateContent);
                return captures
                               .Select(
                    c => new TemplateMergeItem { Value = c.Value, Index = c.Index, Length = c.Length });
            });

            return templateItems.Value;
        }

        protected IEnumerable<Capture> RegexTextReplacementValues(Regex curRegex, string template)
        {
            Match m = curRegex.Match(template);
            int matchCount = 0;

            while (m.Success)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Group g = m.Groups[i];
                    CaptureCollection cc = g.Captures;
                    for (int j = 0; j < cc.Count; j++)
                    {
                        Capture c = cc[j];
                        if (!string.IsNullOrEmpty(c.Value))
                        {
                            yield return c;
                        }
                    }
                }
                m = m.NextMatch();
            }
        }

        #endregion

        #region Working with T

        public IEnumerable<string> MergeNames()
        {
            IEnumerable<string> result = MergeOptions().Select(x => x.ReplaceableText);
            return result;
        }

        public IEnumerable<string> MergeValues()
        {
            return MergeOptions().Select(x => "{{" + x.ReplaceableText + "}}");
        }

        public IEnumerable<string> FriendlyMergeLabels()
        {
            return MergeOptions().Select(x => x.DisplayName);
        }

        
        private IEnumerable<MergeOption> mergeOptions;

        public IEnumerable<MergeOption> MergeOptions()
        {
            if (mergeOptions == null)
            {
                mergeOptions = TypeOptionNamesFromProperties(this.properties);
            }
            return mergeOptions;
        }

        protected IEnumerable<MergeOption> TypeOptionNamesFromProperties(PropertyInfo[] props, string path = "")
        {

            List<MergeOption> result = new List<MergeOption>();
                
            foreach (var prop in props)
            {
                Type type = GetUnderlyingType(prop);
                    
                if (Reflect.OnTypes.IsSimple(type))
                {
                    var option = new MergeOption();
                    var attributes = GetPropertyAttributes(prop);
                    object displayName = null;
                    option.ReplaceableText = BuildPropertyName(prop, path);
                    if (attributes.TryGetValue("DisplayName", out displayName))
                    {
                        option.DisplayName = displayName.ToString();
                    }
                    else
                    {
                        option.DisplayName = option.ReplaceableText;
                    }
                            
                    result.Add(option);
                }
                else
                {
                    path = BuildPath(prop.Name, path);
                    var option = BuildComplexPropertyName(type, path);
                    result.AddRange(option);
                    path = "";
                }
            }
            return result;
        }

        public Dictionary<string, object> GetPropertyAttributes(PropertyInfo property)
        {
            Dictionary<string, object> attribs = new Dictionary<string, object>();
            // look for attributes that takes one constructor argument
            foreach (CustomAttributeData attribData in Reflect.OnAttributes.GetCustomAttributesData(property))
            {

                if (attribData.ConstructorArguments.Count == 1)
                {
                    //string typeName = attribData.Constructor.DeclaringType.Name;
                    string typeName = attribData.AttributeType.Name;
                    if (typeName.EndsWith("Attribute")) typeName = typeName.Substring(0, typeName.Length - 9);
                    attribs[typeName] = attribData.ConstructorArguments[0].Value;
                }

            }
            return attribs;
        }


        protected Type GetUnderlyingType(PropertyInfo prop)
        {
            Type type = prop.PropertyType;
            if (Reflect.OnTypes.IsGenericType(type) && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        protected IEnumerable<MergeOption> BuildComplexPropertyName(Type type, string path)
        {
            var curProps = type.GetProperties();
            IEnumerable<MergeOption> currentPropNames = TypeOptionNamesFromProperties(curProps, path);
            return currentPropNames;
        }

        protected string BuildPath(string name, string path)
        {
            string breadCrumb = name;
            if (string.IsNullOrEmpty(path))
                path = breadCrumb;
            else
                path = string.Format("{0}.{1}", path, breadCrumb);
            return path;
        }

        protected string BuildPropertyName(PropertyInfo prop, string path)
        {
            string propName = prop.Name;
            if (!string.IsNullOrEmpty(path))
                propName = string.Format("{0}.{1}", path, propName);
            return propName;
        }

        #endregion
    }
}