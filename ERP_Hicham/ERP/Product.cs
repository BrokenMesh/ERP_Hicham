using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Hicham.ERP
{
    public class Product
    {
        public uint ID { get; private set; }
        public string Name { get; private set; }
        public float Cost { get; private set; }

        public Product(uint _ID, string _name, float _cost) {
            ID = _ID;
            Name = _name;
            Cost = _cost;
        }

    }
}
