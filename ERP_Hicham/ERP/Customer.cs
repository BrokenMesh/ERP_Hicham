using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Hicham.ERP
{
    public class Customer
    {
        public uint ID { get; private set; }
        public string Cooperation { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }

        public Customer(uint _ID, string _cooperation, string _firstName, string _lastName, string _address, string _city, string _state ){
            ID = _ID; 
            Cooperation = _cooperation;
            FirstName = _firstName;
            LastName = _lastName;
            Address = _address;
            City = _city;
            State = _state;
        }

    }
}
