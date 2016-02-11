using ChimpLab.PhilosophicalMonkey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Temptress
{
    public abstract class TemplateContentBase
    {
        protected const int BracketsPerSide = 2;
        protected const int TotalBracketsPerMatch = BracketsPerSide * 2;
        public ITemplate Template { get; private set; }
        public RenderSettings RenderSettings { get; set; }
        private TemplateContentBase()
        {
        }

        public TemplateContentBase(ITemplate template, RenderSettings renderSettings = null)
        {
            Template = template;
            if (renderSettings == null)
                RenderSettings = new RenderSettings();
        }

        protected string RegexMatchReplace(string template, object data)
        {
            var templateBuilder = new StringBuilder(template);
            int processCount = 0;
            int lengthDifference = 0;

            ProcessControlSections(data, ref templateBuilder, ref processCount, ref lengthDifference);

            IHaveMergeItems mergeItemContainer = Template as IHaveMergeItems;
            IEnumerable<TemplateMergeItem> mergeItems = mergeItemContainer.TemplateMergeItems;
            ProcessMergeItems(mergeItems, data, ref templateBuilder, ref processCount, ref lengthDifference);

            return templateBuilder.ToString();
        }

        protected void ProcessControlSections(object data, ref StringBuilder templateBuilder, ref int processCount, ref int lengthDifference)
        {
            IHaveControlSections sectionContainer = Template as IHaveControlSections;
            IEnumerable<ControlSection> sections = sectionContainer.ControlSections;
            foreach (var section in sections)
            {
                ProcessSection(section, templateBuilder, data, ref processCount, ref lengthDifference);
            }
        }

        protected void ProcessSection(ControlSection section, StringBuilder templateBuilder, object data, ref int processCount, ref int lengthDifference)
        {
            var sectionTextBuilder = new StringBuilder();
            var enumerable = GetEnumerableByName(data, section.PropertyName);
            //var template = new Template(data.GetType(), section.Value);
            foreach (var item in enumerable)
            {
                int i = 0;
                var itemTextBuilder = new StringBuilder(section.TemplateText);
                //ProcessMergeItems(item, ref itemTextBuilder, ref processCount, ref i);
                sectionTextBuilder.Append(itemTextBuilder);
            }
            var result = sectionTextBuilder.ToString();
            var sectionLengthDiff = (("foreach".Length * 2) + (section.PropertyName.Length * 2) + ("<<>>".Length * 2) + "/".Length);
            lengthDifference = -sectionLengthDiff;

            templateBuilder = templateBuilder.Remove(section.Index, section.Length);
            templateBuilder = templateBuilder.Insert(section.Index, result);
        }

        protected void ProcessMergeItems(IEnumerable<TemplateMergeItem> mergeItems, object data, ref StringBuilder templateBuilder, ref int processCount, ref int lengthDifference)
        {
            foreach (var templateValue in mergeItems)
            {
                templateBuilder = ReplaceWithPropertyValue(templateValue, templateBuilder, data, ref processCount, ref lengthDifference);
            }
        }

        protected StringBuilder ReplaceWithPropertyValue(TemplateMergeItem mergeItem, StringBuilder templateBuilder, object data, ref int processCount, ref int lengthDifference)
        {
            if (string.IsNullOrEmpty(mergeItem.Value))
                return templateBuilder;

            int start = mergeItem.Index - (BracketsPerSide + (TotalBracketsPerMatch * processCount)) - lengthDifference;
            int length = mergeItem.Length + TotalBracketsPerMatch;

            var propValue = GetPropertyForMatch(mergeItem.Value, data, ref lengthDifference);

            templateBuilder = templateBuilder.Remove(start, length);
            templateBuilder = templateBuilder.Insert(start, propValue);
            processCount++;
            return templateBuilder;
        }

        protected string GetPropertyForMatch(string path, object data, ref int lengthDifference)
        {
            var steps = path.Split(new char[] { '.' });
            object currentPropertyValue = data;

            foreach (var propertyName in steps)
            {
                currentPropertyValue = GetPropertyValueByName(currentPropertyValue, propertyName);
            }

            string value = currentPropertyValue.ToString();
            lengthDifference = lengthDifference + (path.Length - value.Length);
            return value;
        }

        protected object GetPropertyValueByName(object x, string propName)
        {
            Type type = x.GetType();
            PropertyInfo p = type.GetProperty(propName);
            object pValue = p.GetValue(x);

            if (pValue == null)
                return string.Empty;

            //TODO: currently type is the overall message type. should this not be pValueType as below?
            if (type != typeof(string) && IsEnumerable(pValue))
            {
                pValue = GetEnumerableValuesAsString(pValue);
                return String.Concat(pValue);
            }

            var pValueType = pValue.GetType();
            if (Reflect.OnTypes.IsSimple(pValueType))
            {
                return GetFormattedValue(pValueType, pValue);
            }

            return pValue;
        }

        private string GetFormattedValue(Type type, object currentPropertyValue)
        {
            if (type == typeof(string) || Reflect.OnTypes.IsPrimitive(type))
                return currentPropertyValue.ToString();

            if (type == typeof(DateTime))
            {
                return ((DateTime)currentPropertyValue).ToString(RenderSettings.DateTimeFormat);
            }

            return currentPropertyValue.ToString();
        }

        protected IEnumerable GetEnumerableByName(object x, string propName)
        {
            Type type = x.GetType();
            PropertyInfo p = type.GetProperty(propName);
            object pValue = p.GetValue(x);

            if (IsEnumerable(pValue))
                return pValue as IEnumerable;

            throw new InvalidOperationException(propName + "must be of type IEnumerable<T>");
        }

        protected string GetEnumerableValuesAsString(object pValue)
        {
            var e = pValue as IEnumerable<object>;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var item in e)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(item);
                i++;
            }

            return sb.ToString();
        }

        protected bool IsEnumerable(object pValue)
        {
            Type type = pValue.GetType();

            if (type == typeof(string))
                return false;

            return type.GetInterfaces()
                       .Any(t => Reflect.OnTypes.IsGenericType(t)
                            && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }
    }
}
