using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common.Text
{
    public static class TextCaseLangExtensions
    {
        public static string ToSentenceCase(this string text)
        {
            text = text.Clean();
            if (text != null) text = string.Concat(char.ToUpper(text[0]), text.Substring(1).ToLower());
            return text;
        }

        public static string ToLowerFirstLetter(this string text)
        {
            text = text.Clean();
            if (text != null) text = string.Concat(char.ToLower(text[0]), text.Substring(1));
            return text;
        }

        public static string ToTitleCase(this string text)
        {
            if (text.Clean() == null) return null;
            return CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(text);
        }

        public static string ToTextCase(this string text, eTextCase textCase)
        {
            switch (textCase)
            {
                case eTextCase.Uppercase:
                    return text?.ToUpper();
                case eTextCase.Lowerase:
                    return text?.ToLower();
                case eTextCase.SentenceCase:
                    return text?.ToSentenceCase();
                case eTextCase.TitleCase:
                    return text?.ToTitleCase();
                default:
                    return null;
            }
        }

    }
}
