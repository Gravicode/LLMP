using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMP.Desktop.Helpers
{
    public class HtmlHelper
    {

        public static string ConvertToHtmlUsingConcatenation(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            // Replace newlines with <br> tags
            string html = plainText.Replace(Environment.NewLine, "<br>");

            // Wrap the whole text in <p> tags
            html = $"<p>{html}</p>";

            return html;
        }

    }
}
