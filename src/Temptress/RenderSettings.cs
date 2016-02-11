using System.Globalization;

namespace Temptress
{
    public class RenderSettings
    {
        public string DateTimeFormat { get; set; }

        public RenderSettings()
        {
            DateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }
    }
}
