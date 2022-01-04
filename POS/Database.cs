using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POS
{
    public class Database
    {
        private const string ConnectionString =
            "datasource=localhost; port=3306;username=root;password=;database=pos;SslMode=none";

        public static MySqlConnection Connection;

        public static void CreateConnection()
        {
            Connection = new MySqlConnection(ConnectionString);
        }

        private static string QueryResult(string cmdText)
        {
            string result = default;

            var cmd = new MySqlCommand(cmdText, Connection)
            {
                CommandTimeout = 60
            };

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                    result = !reader.IsDBNull(0) ? reader.GetString(0) : null;

            reader.Close();

            return result;
        }

        public static bool ExecuteQuery(string cmdText)
        {
            var cmd = new MySqlCommand(cmdText, Connection)
            {
                CommandTimeout = 60
            };

            var result = cmd.ExecuteNonQuery();

            return result >= 1;
        }

        public static bool InsertUser(string username, string password, string firstName, string surname, string dob,
            int role)
            => ExecuteQuery(
                $"INSERT INTO users(id, username, password, name, surname, dob, role) VALUES (null, \"{username}\", \"{password}\",\"{firstName}\",\"{surname}\",\"{dob}\",{role})");

        public static int GetNextUserId()
            => Convert.ToInt32(QueryResult("SELECT MAX(id) FROM users;")) + 1;

        public static int GetNextTransactionId() => Convert.ToInt32(QueryResult("SELECT MAX(id) FROM sales;")) + 1;

        public static int GetUserId(string username) =>
            Convert.ToInt32(QueryResult($"SELECT id FROM users WHERE username=\"{username}\";"));

        public static bool ValidCredentials(string username, string password) =>
            password == QueryResult($"SELECT password FROM users WHERE username=\"{username}\";");

        public static List<string[]> GetAllUsers()
        {
            var users = new List<string[]>();

            const string cmdText = "SELECT * FROM users;";

            var cmd = new MySqlCommand(cmdText, Connection);
            cmd.CommandTimeout = 60;

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    var prods = new string[5];

                    prods[0] = reader.GetString(0);
                    prods[1] = reader.GetString(3) + " " + reader.GetString(4);
                    prods[2] = reader.GetString(1);
                    prods[3] = reader.GetString(5);
                    prods[4] = reader.GetString(6);

                    users.Add(prods);
                }

            reader.Close();

            return users;
        }

        public static List<string[]> GetProducts()
        {
            var products = new List<string[]>();

            const string cmdText = "SELECT * FROM products;";

            var cmd = new MySqlCommand(cmdText, Connection)
            {
                CommandTimeout = 60
            };

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    var prods = new string[5];

                    prods[0] = reader.GetString(1);
                    prods[1] = reader.GetString(2);
                    prods[2] = reader.GetString(3);
                    prods[3] = reader.GetString(4);
                    prods[4] = reader.GetString(5);

                    products.Add(prods);
                }

            reader.Close();

            return products;
        }

        public static List<string[]> GetPendingOrders()
        {
            var pendingOrders = new List<string[]>();

            const string cmdText = "SELECT * FROM pending_orders;";

            var cmd = new MySqlCommand(cmdText, Connection);
            cmd.CommandTimeout = 60;

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    var prods = new string[7];

                    prods[0] = reader.GetString(0);
                    prods[1] = reader.GetDateTime(1).ToString();
                    prods[2] = reader.GetString(2);
                    prods[3] = reader.GetString(3);
                    prods[4] = reader.GetString(4);
                    prods[5] = reader.GetString(5);
                    prods[6] = reader.GetString(6);

                    pendingOrders.Add(prods);
                }

            reader.Close();

            return pendingOrders;
        }

        public static List<string[]> GetReceivedOrders()
        {
            var pendingOrders = new List<string[]>();

            const string cmdText = "SELECT * FROM received_orders;";

            var cmd = new MySqlCommand(cmdText, Connection);
            cmd.CommandTimeout = 60;

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    var prods = new string[6];

                    prods[0] = reader.GetString(1);
                    prods[1] = reader.GetString(2);
                    prods[2] = reader.GetString(3);
                    prods[3] = reader.GetString(4);
                    prods[4] = reader.GetString(5);
                    prods[5] = reader.GetString(6);

                    pendingOrders.Add(prods);
                }

            reader.Close();

            return pendingOrders;
        }

        public static List<string> GetSuppliersName()
        {
            var suppliers = new List<string>();

            const string cmdText = "SELECT name FROM suppliers;";

            var cmd = new MySqlCommand(cmdText, Connection);
            cmd.CommandTimeout = 60;

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                    suppliers.Add(reader.GetString(0));

            reader.Close();

            return suppliers;
        }

        public static List<string> GetSupplierInfo(string name)
        {
            var suppliers = new List<string>();

            var cmdText = $"SELECT phone_no, company FROM suppliers WHERE name=\"{name}\";";

            var cmd = new MySqlCommand(cmdText, Connection)
            {
                CommandTimeout = 60
            };

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    suppliers.Add(reader.GetString(0));
                    suppliers.Add(reader.GetString(1));
                }

            reader.Close();

            return suppliers;
        }

        public static List<string[]> GetSuppliers()
        {
            var suppliers = new List<string[]>();

            const string cmdText = "SELECT * FROM suppliers;";

            var cmd = new MySqlCommand(cmdText, Connection);
            cmd.CommandTimeout = 60;

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    var prods = new string[6];

                    prods[0] = reader.GetString(1);
                    prods[1] = reader.GetString(2);
                    prods[2] = reader.GetString(3);
                    prods[3] = reader.GetString(4);
                    prods[4] = reader.GetString(5);
                    prods[5] = reader.GetString(6);

                    suppliers.Add(prods);
                }

            reader.Close();

            return suppliers.Distinct().ToList();
        }

        public static List<string[]> GetSuppliersFullProducts(string supplier)
        {
            var products = new List<string[]>();

            var cmdText = $"SELECT id, product, barcode, price, qty FROM suppliers WHERE name=\"{supplier}\";";

            var cmd = new MySqlCommand(cmdText, Connection);
            cmd.CommandTimeout = 60;

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    var prods = new string[5];

                    prods[0] = reader.GetString(0);
                    prods[1] = reader.GetString(1);
                    prods[2] = reader.GetString(2);
                    prods[3] = reader.GetString(3);
                    prods[4] = reader.GetString(4);

                    products.Add(prods);
                }

            reader.Close();

            return products;
        }

        public static List<string[]> GetSales(int time)
        {
            //SELECT * FROM sales WHERE YEARWEEK(`date`, 1) = YEARWEEK(CURDATE(), 1)

            var cmdText = "";

            switch (time)
            {
                case 0:
                    cmdText = "SELECT * FROM `sales` WHERE date > date_sub(now(), interval 1 week)";
                    break;

                case 1:
                    cmdText = "SELECT * FROM `sales` WHERE date > date_sub(now(), interval 1 month)";
                    break;

                case 2:
                    cmdText = "SELECT * FROM `sales` WHERE date > date_sub(now(), interval 3 month)";
                    break;

                case 3:
                    cmdText = "SELECT * FROM `sales` WHERE date > date_sub(now(), interval 12    month)";
                    break;
            }

            var sales = new List<string[]>();

            var cmd = new MySqlCommand(cmdText, Connection)
            {
                CommandTimeout = 60
            };

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    var sale = new string[7];

                    sale[0] = reader.GetString(0);
                    sale[1] = reader.GetString(1);
                    sale[2] = reader.GetString(2);
                    sale[3] = reader.GetString(3);
                    sale[4] = reader.GetString(4);
                    sale[5] = reader.GetString(5);
                    sale[6] = reader.GetString(6);

                    sales.Add(sale);
                }

            reader.Close();

            return sales;
        }

        public static List<string> GetSupplierProducts(string supplier)
        {
            var products = new List<string>();

            var cmdText = $"SELECT product FROM suppliers WHERE name=\"{supplier}\";";

            var cmd = new MySqlCommand(cmdText, Connection)
            {
                CommandTimeout = 60
            };


            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                    products.Add(reader.GetString(0));

            reader.Close();

            return products;
        }

        public static List<string> GetProductByBarcode(string barcode)
        {
            var product = new List<string>();

            var cmdText = $"SELECT * FROM products WHERE barcode=\"{barcode}\";";

            var cmd = new MySqlCommand(cmdText, Connection)
            {
                CommandTimeout = 60
            };

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                {
                    product.Add(reader.GetString(1));
                    product.Add(reader.GetString(2));
                    product.Add(reader.GetString(4));
                }

            reader.Close();

            return product;
        }

        public static int GetRole(string username) =>
            Convert.ToInt32(QueryResult($"SELECT role FROM users WHERE username=\"{username}\";"));

        public static bool AddSale(string name, string barcode, int qty, decimal price, decimal total) =>
            ExecuteQuery(
                $"INSERT INTO sales(id, date, name, barcode, qty, price, total) VALUES (null, \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",\"{name}\", \"{barcode}\", {qty},\"{price.ToString().Replace(',', '.')}\", \"{total.ToString().Replace(',', '.')}\");");

        public static bool UpdateUser(List<string> userData) =>
            ExecuteQuery(
                $"UPDATE users SET username=\"{userData[2]}\", name=\"{userData[1].Split(' ')[0]}\", surname=\"{userData[1].Split(' ')[1]}\", dob=\"{userData[3]}\", role={userData[4]} WHERE id={userData[0]};");

        public static bool ConfirmOrder(List<string[]> orders)
        {
            bool b = default, b1 = default;

            foreach (var order in orders)
            {
                int id = Convert.ToInt32(order[0]), qty = Convert.ToInt32(order[5]);
                var total = Convert.ToDecimal(order[6]);
                var prodName = order[2];
                var supplier = order[3];
                var barcode = order[4];


                b = ExecuteQuery($"DELETE FROM pending_orders WHERE id={id}");
                b1 = ExecuteQuery(
                    $"INSERT INTO received_orders(id, date, prod_name, supplier, barcode, qty, total) VALUES (null, \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\", \"{prodName}\", \"{supplier}\", \"{barcode}\", {qty}, {total.ToString().Replace(',', '.')})");
            }

            return b && b1;
        }

        public static bool AddSupplier(string name, string phoneNo, string company, List<string[]> products)
            => products.All(product =>
                ExecuteQuery(
                    $"INSERT INTO suppliers(id, name, barcode, phone_no, company, product, price, qty) VALUES (null, \"{name}\", \"{product[1]}\", \"{phoneNo}\", \"{company}\", \"{product[0]}\", {product[2]}, 500);"));

        public static bool AddProduct(string name, string barcode, decimal org_price, decimal markup_price, int qty) =>
            ExecuteQuery(
                $"INSERT INTO products(id, name, barcode, org_price, mrkup_price, qty) VALUES (null, \"{name}\", \"{barcode}\", {org_price.ToString().Replace(',', '.')}, {markup_price.ToString().Replace(',', '.')}, {qty})");

        public static bool DeleteUser(int id) => ExecuteQuery($"DELETE FROM users WHERE id={id};");

        public static bool UpdateSupplier(string name, string phoneNo, string company, List<string> product)
        {
            var txt =
                $"UPDATE suppliers SET name=\"{name}\",barcode=\"{product[2]}\",phone_no=\"{phoneNo}\",company=\"{company}\",product=\"{product[1]}\",price={product[3]},qty={product[4]} WHERE id={product[0]};";

            return ExecuteQuery(txt);
        }

        public static string GetSupplierProductBarcode(string supplier, string prodname) =>
            QueryResult($"SELECT barcode FROM suppliers WHERE name=\"{supplier}\" AND product=\"{prodname}\";");

        public static bool ProductAvailable(string barcode, int qty) =>
            Convert.ToInt32(QueryResult($"SELECT qty FROM products WHERE barcode=\"{barcode}\";")) >= qty;

        public static bool SupplierProductAvailable(string supplier, string prodname, int qty) =>
            Convert.ToInt32(
                QueryResult($"SELECT qty FROM suppliers WHERE name=\"{supplier}\" AND product=\"{prodname}\";")) >= qty;

        public static decimal GetSupplierProductPrice(string name) =>
            Convert.ToDecimal(QueryResult($"SELECT price FROM suppliers WHERE product=\"{name}\";"));

        public static bool PlaceOrder(string supplier, string product, string barcode, int qty, decimal total)
        {
            var cmdText = "INSERT INTO pending_orders(id, date, prod_name, supplier, barcode, qty, total) " +
                          $"VALUES (null, \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\", \"{product}\", \"{supplier}\", \"{barcode}\", {qty}, {total.ToString().Replace(',', '.')})";

            var cmd = new MySqlCommand(cmdText, Connection);
            cmd.CommandTimeout = 60;

            var result = cmd.ExecuteNonQuery();

            if (result != 1) return false;

            ExecuteQuery(
                $"UPDATE suppliers SET qty=qty-{qty} WHERE name=\"{supplier}\" AND product=\"{product}\";");
            return true;
        }
    }
}