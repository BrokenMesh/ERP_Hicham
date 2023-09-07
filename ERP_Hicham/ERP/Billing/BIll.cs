using System;
using System.Collections;
using System.Collections.Generic;

namespace ERP_Hicham.ERP.Billing
{
    public class Bill{
        public uint ID { get; private set; }
        public uint customerID { get; private set; }
        public DateTime date { get; private set; }
        public float cost { get; private set; }
        public string orderNames { get; private set; }

        private List<ProduktBill> produktList = new List<ProduktBill>();

        public Bill(uint _ID, uint _customerID, DateTime _date) {
            ID = _ID;
            customerID = _customerID;
            date = _date;
        }

        public void AddProdukt(uint ID, uint _produktID, uint _count) {
            ProduktBill _pb;
            _pb.ID = ID;
            _pb.count = _count;
            _pb.produktID = _produktID;
            produktList.Add(_pb);
        }

        public void UpdateValues() {
            foreach (ProduktBill _pb in produktList) {
                Product? _product = MainWindow.instance.DB.QueryProduct(_pb.produktID);
                if (_product == null) return;
                cost += _product.Cost * _pb.count;
                orderNames += _product.Name + ", ";
            }
        }

        public List<ProduktBill> GetProduktList() {
            return produktList;
        }
    }

    public struct ProduktBill {
        public uint ID;
        public uint count;
        public uint produktID;
    }
}
