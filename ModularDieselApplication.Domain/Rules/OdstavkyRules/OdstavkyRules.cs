using ModularDieselApplication.Domain.Objects;


namespace ModularDieselApplication.Domain.Rules
{
    public class OdstavkyRules
    {
        public static async Task<HandleResult> OdstavkyCheck(DateTime od, DateTime do_, HandleResult result, bool ExistingOdstavka)
        {
            if (!ExistingOdstavka)
            {
                result.Success = false;
                result.Message = "Již existuje jiná odstávka.";
                return result;
            }

            if (! await IsValidDateRange(od, do_))
            {
                result.Success = false;
                result.Message = "Špatně zadané datum.";
                return result;
            }
            else
            {
                result.Success = true;
                return result;
            }
        }
        private static Task<bool> IsValidDateRange(DateTime od, DateTime Do)
        {
            return Task.FromResult(od.Date >= DateTime.Today && od < Do);
        }

    }
}