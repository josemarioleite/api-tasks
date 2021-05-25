using System.Globalization;
using System.Linq;

namespace remarsemanal.Helpers
{
    public static class StringHelper
    {
        public static string RemoveAcentosECaracteresEspeciais(this string stringASerFormatada)
        {
            stringASerFormatada = stringASerFormatada.Normalize(System.Text.NormalizationForm.FormD);
            var chars = stringASerFormatada.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}