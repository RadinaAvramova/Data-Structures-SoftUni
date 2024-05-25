using System;
using System.Collections.Generic;
using System.Linq;

namespace BarberShop
{
    public class BarberShop : IBarberShop
    {
        private Dictionary<string, Barber> barbersByName = new Dictionary<string, Barber>();
        private Dictionary<string, Client> clientsByName = new Dictionary<string, Client>();
        private Dictionary<string, List<Client>> clientsByBarber = new Dictionary<string, List<Client>>();
        private Dictionary<string, Client> clientsWithNoBarber = new Dictionary<string, Client>();
        private Dictionary<string, Client> clientsWithBarber = new Dictionary<string, Client>();

        public BarberShop()
        {
        }

        public void AddBarber(Barber b)
        {
            if (this.barbersByName.ContainsKey(b.Name))
            {
                throw new ArgumentException();
            }

            this.barbersByName.Add(b.Name, b);
            this.clientsByBarber.Add(b.Name, new List<Client>());
        }

        public void AddClient(Client c)
        {
            if (this.clientsByName.ContainsKey(c.Name))
            {
                throw new ArgumentException();
            }

            this.clientsByName.Add(c.Name, c);
            this.clientsWithNoBarber.Add(c.Name, c);
        }

        public bool Exist(Barber b)
        {
            return this.barbersByName.ContainsKey(b.Name);
        }

        public bool Exist(Client c)
        {
            return this.clientsByName.ContainsKey(c.Name);
        }

        public IEnumerable<Barber> GetBarbers()
        {
            return this.barbersByName.Values;
        }

        public IEnumerable<Client> GetClients()
        {
            return this.clientsByName.Values;
        }

        public void AssignClient(Barber b, Client c)
        {
            if (!this.barbersByName.ContainsKey(b.Name))
            {
                throw new ArgumentException();
            }

            if (!this.clientsByName.ContainsKey(c.Name))
            {
                throw new ArgumentException();
            }

            c.Barber = b;
            this.clientsByBarber[b.Name].Add(c);
            this.clientsWithNoBarber.Remove(c.Name);
            this.clientsWithBarber.Add(c.Name, c);
        }

        public void DeleteAllClientsFrom(Barber b)
        {
            if (!this.barbersByName.ContainsKey(b.Name))
            {
                throw new ArgumentException();
            }

            var col = this.clientsByBarber[b.Name];
            foreach (var item in col)
            {
                this.clientsWithBarber.Remove(item.Name);
                this.clientsByName.Remove(item.Name);
            }

            this.clientsByBarber[b.Name] = new List<Client>();
        }

        public IEnumerable<Client> GetClientsWithNoBarber()
        {
            return this.clientsWithNoBarber.Values;
        }

        public IEnumerable<Barber> GetAllBarbersSortedWithClientsCountDesc()
        {
            return this.barbersByName.Values.OrderByDescending(x => this.clientsByBarber[x.Name].Count);
        }

        public IEnumerable<Barber> GetAllBarbersSortedWithStarsDecsendingAndHaircutPriceAsc()
        {
            return this.barbersByName.Values.OrderByDescending(x => x.Stars).ThenBy(x => x.HaircutPrice);
        }

        public IEnumerable<Client> GetClientsSortedByAgeDescAndBarbersStarsDesc()
        {
            return this.clientsWithBarber.Values.OrderByDescending(x => x.Age).ThenByDescending(b => b.Barber.Stars);
        }
    }
}
