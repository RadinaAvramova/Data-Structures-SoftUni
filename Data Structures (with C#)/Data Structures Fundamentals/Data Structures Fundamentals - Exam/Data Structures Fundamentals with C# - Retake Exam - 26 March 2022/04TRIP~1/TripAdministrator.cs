using System;
using System.Collections.Generic;
using System.Linq;

namespace TripAdministrations
{
    public class TripAdministrator
    {
        private Dictionary<string, Company> companyByName = new Dictionary<string, Company>();
        private Dictionary<string, Trip> tripById = new Dictionary<string, Trip>();
        private Dictionary<string, List<Trip>> tripsByCompany = new Dictionary<string, List<Trip>>();

        public TripAdministrator()
        {
        }

        public void AddCompany(Company c)
        {
            if (this.companyByName.ContainsKey(c.Name))
            {
                throw new ArgumentException();
            }

            c.CurrentTrips = c.TripOrganizationLimit;
            this.companyByName.Add(c.Name, c);
            this.tripsByCompany.Add(c.Name, new List<Trip>());
        }

        public void AddTrip(Company c, Trip t)
        {
            if (!this.companyByName.ContainsKey(c.Name))
            {
                throw new ArgumentException();
            }

            if (this.tripById.ContainsKey(t.Id))
            {
                throw new ArgumentException();
            }

            if (this.companyByName[c.Name].CurrentTrips == 0)
            {
                throw new ArgumentException();
            }

            c.CurrentTrips--;
            this.tripsByCompany[c.Name].Add(t);
            this.tripById.Add(t.Id, t);
        }

        public bool Exist(Company c)
        {
            return this.companyByName.ContainsKey(c.Name);
        }

        public bool Exist(Trip t)
        {
            return this.tripById.ContainsKey(t.Id);
        }

        public void RemoveCompany(Company c)
        {
            if (!this.companyByName.ContainsKey(c.Name))
            {
                throw new ArgumentException();
            }

            var t = this.tripsByCompany[c.Name];
            foreach (var item in t)
            {
                this.tripById.Remove(item.Id);
            }

            this.companyByName.Remove(c.Name);
            this.tripsByCompany.Remove(c.Name);
        }

        public IEnumerable<Company> GetCompanies()
        {
            return this.companyByName.Values;
        }

        public IEnumerable<Trip> GetTrips()
        {
            return this.tripById.Values;
        }

        public void ExecuteTrip(Company c, Trip t)
        {
            if (!this.companyByName.ContainsKey(c.Name))
            {
                throw new ArgumentException();
            }

            if (!this.tripById.ContainsKey(t.Id))
            {
                throw new ArgumentException();

            }

            if (!this.tripsByCompany[c.Name].Contains(t))
            {
                throw new ArgumentException();
            }

            this.companyByName[c.Name].CurrentTrips--;
            this.tripById.Remove(t.Id);
            this.tripsByCompany[c.Name].Remove(t);
        }

        public IEnumerable<Company> GetCompaniesWithMoreThatNTrips(int n)
        {
            return this.companyByName.Values.Where(x => this.tripsByCompany[x.Name].Count > n);
        }

        public IEnumerable<Trip> GetTripsWithTransportationType(Transportation t)
        {
            return this.tripById.Values.Where(x => x.Transportation == t);
        }

        public IEnumerable<Trip> GetAllTripsInPriceRange(int lo, int hi)
        {
            return this.tripById.Values.Where(x => x.Price >= lo && x.Price <= hi);
        }
    }
}
