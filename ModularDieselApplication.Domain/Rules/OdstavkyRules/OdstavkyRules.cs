using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Entities;
using System.Runtime.CompilerServices;

namespace ModularDieselApplication.Domain.Rules
{
    public class OdstavkyRules
    {
        public HandleOdstavkyDieslovaniResult OdstavkyCheck(Lokalita lokalitaSearch, DateTime od, DateTime do_, HandleOdstavkyDieslovaniResult result, bool ExistingOdstavka)
        {
            if (!ExistingOdstavka)
            {
                result.Success = false;
                result.Message = "Již existuje jiná odstávka.";
                return result;
            }

            if (!IsValidDateRange(od, do_))
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
        private static bool IsValidDateRange(DateTime od, DateTime Do)
        {
            return od.Date >= DateTime.Today && od < Do;
        }

    }
}