using System.Globalization;
using System.Threading;

namespace LegacyPM.Web.Services
{
    public class CultureService
    {
        public void UseBrazilianCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
        }

        public void UseEnglishCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        public string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }
    }
}