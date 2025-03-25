using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Domain.Rules
{
    public class OdstavkyRules
    {
        // ----------------------------------------
        // Validate an odstávka and check for conflicts.
        // ----------------------------------------
        public static async Task<HandleResult> OdstavkyCheck(DateTime od, DateTime do_, HandleResult result, bool ExistingOdstavka)
        {
            if (!ExistingOdstavka)
            {
                result.Success = false;
                result.Message = "Již existuje jiná odstávka.";
                return result;
            }

            if (!await IsValidDateRange(od, do_))
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

        // ----------------------------------------
        // Check if the provided date range is valid.
        // ----------------------------------------
        private static Task<bool> IsValidDateRange(DateTime od, DateTime do_)
        {
            return Task.FromResult(od.Date >= DateTime.Today && od < do_);
        }
    }
}