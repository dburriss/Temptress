namespace Temptress
{
    /// <summary>
    /// I represent the metadata of a replaceable piece of text in the template
    /// </summary>
    public class TemplateMergeItem
    {
        public string Value { get; set; }

        public int Index { get; set; }

        public int Length { get; set; }
    }

    public class ControlSection : TemplateMergeItem
    {
        public string PropertyName { get; set; }
        public string TemplateText { get; set; }
    }
}