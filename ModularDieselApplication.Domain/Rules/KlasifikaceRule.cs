namespace ModularDieselApplication.Domain.Rules
{
    public static class KlasifikaceRule
    {
        // ----------------------------------------
        // Get the weight of a classification.
        // ----------------------------------------
        public static int ZiskejVahu(this string Klasifikace)
        {
            var klasifikaceVaha = new Dictionary<string, int>
            {
                {"A1", 6},
                {"A2", 5},
                {"B1", 4},
                {"B2", 3},
                {"B", 2},
                {"C", 2},
                {"D1", 1}
            };

            // If the classification is not found, return the default value of 1.
            return klasifikaceVaha.TryGetValue(Klasifikace, out int vaha) ? vaha : 1;
        }

        // ----------------------------------------
        // Get the outage duration for a classification.
        // ----------------------------------------
        public static int ZiskejCasVypadku(this string Klasifikace)
        {
            var klasifikaceVaha = new Dictionary<string, int>
            {
                {"A1", 1},
                {"A2", 1},
                {"B1", 1},
                {"B2", 1},
                {"B", 12},
                {"C", 12},
                {"D1", 12}
            };

            // If the classification is not found, return the default value of 1.
            return klasifikaceVaha.TryGetValue(Klasifikace, out int vaha) ? vaha : 1;
        }
    }
}