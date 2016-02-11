namespace Temptress
{


    public class TemplateRenderer<T> : TemplateContentBase, IContentRenderer where T : class
    {
        
        public TemplateRenderer(Template<T> template) : base(template as ITemplate)
        {}

        public virtual string Render(T data, RenderMode renderMode = RenderMode.Normal)
        {
            return Merge(data);
        }

        public string Render(T data)
        {
            return Render((T)data, RenderMode.Normal);
        }

        public string Render(object data)
        {
            return Render((T)data, RenderMode.Normal);
        }

        private string Merge(T data)
        {
            string template = Template.TemplateContent;

            return RegexMatchReplace(template, data);
        }

    }
}