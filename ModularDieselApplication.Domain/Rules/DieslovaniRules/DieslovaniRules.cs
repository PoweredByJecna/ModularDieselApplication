using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Domain.Rules
{
    public class DieslovaniRules
    {
        // ----------------------------------------
        // Check if diesel is required for a location.
        // ----------------------------------------
        public async Task<HandleResult> IsDieselRequired(string klasifikace, DateTime Od, DateTime Do, int baterie, Odstavka newOdstavka, HandleResult result)
        {
            if (newOdstavka?.Lokality?.DA == true)
            {
                result.Success = false;
                result.Duvod = "Na lokalitě není potřeba dieslovat, nachází se tam stacionární generátor.";
                return result;
            }

            if (newOdstavka?.Lokality?.Zasuvka == false)
            {
                result.Success = false;
                result.Duvod = "Na lokalitě se nedá dieslovat, protože tam není zásuvka.";
                return result;
            }

            var casVypadku = await Task.Run(() => klasifikace.ZiskejCasVypadku());
            var rozdil = (Do - Od).TotalMinutes;

            if (casVypadku * 60 > rozdil)
            {
                result.Success = false;
                result.Duvod = "Lokalita může být vypnuta delší dobu než je délka výpadku.";
                return result;
            }
            else
            {
                if (await BatteryAsync(Od, Do, baterie))
                {
                    result.Success = false;
                    result.Duvod = "Lokalita vydrží na baterie.";
                    return result;
                }
                else
                {
                    result.Success = true;
                    return result;
                }
            }
        }

        // ----------------------------------------
        // Check if the location can run on batteries for the specified time.
        // ----------------------------------------
        private static async Task<bool> BatteryAsync(DateTime od, DateTime do_, int baterie)
        {
            return await Task.Run(() =>
            {
                var rozdil = (do_ - od).TotalMinutes;
                if (!int.TryParse(baterie.ToString(), out var baterieMinuty))
                    baterieMinuty = 0;

                return rozdil <= baterieMinuty;
            });
        }
    }
}