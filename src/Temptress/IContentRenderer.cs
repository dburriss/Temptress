namespace Temptress
{
    public interface IContentRenderer
    {
        string Render(object data);
    }

    public class TemplateContent : TemplateContentBase, IContentRenderer
    {
        public TemplateContent(ITemplate template) : base(template as ITemplate)
        { }

        public string Render(object data)
        {
            string template = Template.TemplateContent;

            return RegexMatchReplace(template, data);
        }
    }
}
