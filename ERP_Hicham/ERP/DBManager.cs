using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ERP_Hicham.Dialog.Confirm;
using ERP_Hicham.ERP.Billing;
using MySql.Data.MySqlClient;

namespace ERP_Hicham.ERP
{
    public class DBManager
    {
        MySqlConnection DB;

        public DBManager(string _server, string _userid, string _password, string _database) {
            string conStr = $"server={_server};userid={_userid};password={_password};database={_database};";

            DB = new MySqlConnection(conStr);
            DB.Open();

            Console.WriteLine($"MySql version: {DB.ServerVersion}");
        }

        public string QuerySQLVersion() {
            string _query = "SELECT VERSION()";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);
            return _cmd.ExecuteScalar().ToString();
        }


        public Customer? QueryCustomer(uint _id) {
            string _query = $"SELECT * FROM customer WHERE idCustomer = {_id}";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);

            MySqlDataReader _reader = _cmd.ExecuteReader();

            Customer? _customer = null;

            while (_reader.Read()) {
                _customer = new Customer(
                    (uint)_reader.GetInt32("idCustomer"),
                    _reader.GetString("cooperation"),
                    _reader.GetString("firstName"),
                    _reader.GetString("lastName"),
                    _reader.GetString("address"),
                    _reader.GetString("city"),
                    _reader.GetString("state"));
            }

            _reader.Close();

            return _customer;
        }

        public List<Customer> QueryAllCustomer() {
            string _query = $"SELECT * FROM customer";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);

            MySqlDataReader _reader = _cmd.ExecuteReader();

            List<Customer> _customerList = new List<Customer>();

            while (_reader.Read()) {
                _customerList.Add(new Customer(
                    (uint)_reader.GetInt32("idCustomer"),
                    _reader.GetString("cooperation"),
                    _reader.GetString("firstName"),
                    _reader.GetString("lastName"),
                    _reader.GetString("address"),
                    _reader.GetString("city"),
                    _reader.GetString("state")));
            }

            _reader.Close();

            return _customerList;
        }

        public void AddCustomer(Customer _customer) {
            string _command = $"INSERT INTO customer (cooperation, firstName, lastName, address, city, state) " +
                $"VALUES ('{_customer.Cooperation}','{_customer.FirstName}','{_customer.LastName}','{_customer.Address}','{_customer.City}','{_customer.State}')";
            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();
        }

        public void RemoveCustomer(uint _id) {
            string _command = $"DELETE FROM customer WHERE idCustomer = {_id};";
            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();
        }

        public void UpdateCustomer(uint _id, Customer _customer) {
            string _command = $"UPDATE customer SET " +
                $"cooperation = '{_customer.Cooperation}', " +
                $"firstName = '{_customer.FirstName}', " +
                $"lastName = '{_customer.LastName}', " +
                $"address = '{_customer.Address}', " +
                $"city = '{_customer.City}', " +
                $"state = '{_customer.State}' " +
                $"WHERE idCustomer = {_id};";

            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();
        }

        public List<Bill> QueryAllBills(Customer _customer) {
            string _query = $"SELECT * FROM bill WHERE customerID={_customer.ID}";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);

            MySqlDataReader _reader = _cmd.ExecuteReader();

            List<Bill> _bills = new List<Bill>();

            while (_reader.Read()) {
                _bills.Add(new Bill(
                    (uint)_reader.GetInt32("idBill"),
                    (uint)_reader.GetInt32("customerID"),
                    _reader.GetDateTime("date")));
            }

            _reader.Close();

            foreach (Bill _b in _bills) {
                QueryBillOrders(_b);
            }

            return _bills;
        }

        public Bill? QueryBill(uint _id) {
            string _query = $"SELECT * FROM bill WHERE idBill={_id}";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);

            MySqlDataReader _reader = _cmd.ExecuteReader();

            Bill? _bill = null;

            while (_reader.Read())
            {
                _bill = new Bill(
                    (uint)_reader.GetInt32("idBill"),
                    (uint)_reader.GetInt32("customerID"),
                    _reader.GetDateTime("date"));
            }

            _reader.Close();

            if (_bill == null) return _bill;

            QueryBillOrders(_bill);
            return _bill;
        }

        public Bill QueryBillOrders(Bill _bill) {
            string _query = $"SELECT * FROM orders WHERE billID={_bill.ID}";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);

            MySqlDataReader _reader = _cmd.ExecuteReader();

            while (_reader.Read()) {
                _bill.AddProdukt(
                    (uint)_reader.GetUInt32("idOrder"),
                    (uint)_reader.GetUInt32("productID"),
                    (uint)_reader.GetUInt32("count"));
            }

            _reader.Close();

            _bill.UpdateValues();

            return _bill;
        }

        public Product? QueryProduct(uint _id) {
            string _query = $"SELECT * FROM products WHERE idProducts={_id}";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);

            MySqlDataReader _reader = _cmd.ExecuteReader();

            Product? product = null;

            while (_reader.Read())
            {
                product = new Product(
                    _reader.GetUInt32("idProducts"),
                    _reader.GetString("name"), 
                    _reader.GetFloat("cost"));
            }

            _reader.Close();

            return product;
        }


        public void AddBill(Bill _bill) {
            string _command = $"INSERT INTO bill(date, customerID) " +
                $"VALUES ('{_bill.date.ToString("yyyy-MM-dd")}','{_bill.customerID}')";
            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();

            string _query = $"SELECT idBill FROM bill WHERE idBill=@@IDENTITY";
            _cmd = new MySqlCommand(_query, DB);
            int _id = (int)_cmd.ExecuteScalar();

            foreach (ProduktBill _pb in _bill.GetProduktList()) {
                _command = $"INSERT INTO orders(count, productID, billID) " +
                $"VALUES ('{_pb.count}','{_pb.produktID}','{_id}')";
                _cmd = new MySqlCommand(_command, DB);
                _cmd.ExecuteNonQuery();
            }
        }

        public void RemoveBill(uint _id) {
            string _command = $"DELETE FROM orders WHERE billID = {_id};";
            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();

            _command = $"DELETE FROM bill WHERE idBill = {_id};";
            _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();
        }

        public void UpdateBill(uint _id, Bill _bill) {
            string _command = $"DELETE FROM orders WHERE billID = {_id};";
            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();

            foreach (ProduktBill _pb in _bill.GetProduktList()) {
                _command = $"INSERT INTO orders(count, productID, billID) " +
                $"VALUES ('{_pb.count}','{_pb.produktID}','{_id}')";
                _cmd = new MySqlCommand(_command, DB);
                _cmd.ExecuteNonQuery();
            }
        }
        public List<Product> QueryAllProducts() {
            string _query = $"SELECT * FROM products";
            MySqlCommand _cmd = new MySqlCommand(_query, DB);

            MySqlDataReader _reader = _cmd.ExecuteReader();

            List<Product> products = new List<Product>();

            while (_reader.Read())
            {
                products.Add(new Product(
                    _reader.GetUInt32("idProducts"),
                    _reader.GetString("name"),
                    _reader.GetFloat("cost")));
            }

            _reader.Close();

            return products;
        }

        public void CreateProduct(Product _product) {
            string _command = $"INSERT INTO products(name, cost) " +
                $"VALUES ('{_product.Name}','{_product.Cost}')";
            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();
        }

        public void UpdateProduct(Product _product, uint _id) {
            string _command = $"UPDATE products SET " +
                $"name = '{_product.Name}', " +
                $"cost = '{_product.Cost}' " +
                $"WHERE idProducts = {_id};";

            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();
        }

        public bool RemoveProduct(uint _id, ref string _listOfDep) {
            string _command = $"SELECT billID FROM orders WHERE productID = {_id};";
            MySqlCommand _cmd = new MySqlCommand(_command, DB);
            MySqlDataReader _reader = _cmd.ExecuteReader();

            List<int> _billIDs = new List<int>();

            while (_reader.Read()) {
                _billIDs.Add(_reader.GetInt32("billID"));
            }

            _reader.Close();

            if (_billIDs.Count != 0) {
                _listOfDep = "";
                foreach (int i in _billIDs)  {
                    _listOfDep += i.ToString() + ", ";
                }
                return false;
            }

            _command = $"DELETE FROM products WHERE idProducts = {_id};";
            _cmd = new MySqlCommand(_command, DB);
            _cmd.ExecuteNonQuery();

            return true;
        }
    }
}
