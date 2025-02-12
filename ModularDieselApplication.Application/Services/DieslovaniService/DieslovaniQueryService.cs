


          /* ----------------------------------------
           GetTableDataRunningTableAsync
           ---------------------------------------- */
           
       //public async Task<(int totalRecords, List<object> data)> GetTableDataAllTableAsync(User? currentUser, bool isEngineer)
        /*
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery();
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        */

        /* ----------------------------------------
           GetTableDataRunningTableAsync
           ---------------------------------------- */

        /*
        public async Task<(int totalRecords, List<object> data)> GetTableDataRunningTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup !=null);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        */

        /* ----------------------------------------
           GetTableDataUpcomingTableAsync
           ---------------------------------------- */

        /*   
        public async Task<(int totalRecords, List<object> data)> GetTableDataUpcomingTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup !=null && i.Odstavka.Od.Date==DateTime.Today);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        */

        /* ----------------------------------------
           GetTableDataEndTableAsync
           ---------------------------------------- */

        /*
        public async Task<(int totalRecords, List<object> data)> GetTableDataEndTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.Odchod !=null && o.Vstup == null);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        */


        /* ----------------------------------------
           GetTableDatathrashTableAsync
           ---------------------------------------- */

        /*   
        public async Task<(int totalRecords, List<object> data)> GetTableDatathrashTableAsync(User? currentUser, bool isEngineer)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser), "Current user cannot be null.");
            }
            var technik = await _technikService.GetTechnikByUserIdAsync(currentUser.Id);

            if (technik == null || technik.Firma == null)
            {
                throw new InvalidOperationException("Technik or Firma is null.");
            }
            var firmaId = technik.Firma.ID;

            var validRegions = await _regionyService.GetRegionByIdFirmy(firmaId);

            var query = _dieslovaniRepository.GetDieslovaniQuery();

            if (isEngineer && validRegions.Any())
            {
                query = query.Where(d => validRegions.Contains(d.Odstavka.Lokality.Region.ID));
            }

            int totalRecords =  query.Count(); // Správné asynchronní počítání

            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        */

        
        /* ----------------------------------------
           GetTableDataOdDetailOdstavkyAsync
           ---------------------------------------- */

        /*  
        public async Task<List<object>> GetTableDataOdDetailOdstavkyAsync(int idodstavky)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.ID == idodstavky);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }
        */







