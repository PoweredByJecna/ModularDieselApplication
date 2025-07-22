using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Domain.Rules
{
    public class DieslovaniRules
    {
        // ----------------------------------------
        // Check if diesel is required for a location.
        // ----------------------------------------
        public async Task<IsDieselRequiredEnum> IsDieselRequired(string klasifikace, DateTime Od, DateTime Do, int baterie, Odstavka newOdstavka)
        {
            switch (true)
            {
            case bool _ when newOdstavka.Lokality.DA == true:
                return IsDieselRequiredEnum.Agregat;
            case bool _ when newOdstavka.Lokality.Zasuvka == false:
                return IsDieselRequiredEnum.Zasuvka;
            }
            var casVypadku = await Task.Run(() => klasifikace.ZiskejCasVypadku());
            var rozdil = (Do - Od).TotalMinutes;
            switch (true)
            {
            case bool _ when casVypadku * 60 > rozdil:
                return IsDieselRequiredEnum.Priorita;
            case bool _ when await BatteryAsync(Od, Do, baterie):
                return IsDieselRequiredEnum.Baterie;
            default:
                return IsDieselRequiredEnum.Yes;
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