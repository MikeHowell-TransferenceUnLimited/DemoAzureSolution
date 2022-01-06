using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TestDBAccess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Customer cust = new Customer();

            SqlConnectionStringBuilder sconstrbuilder = new SqlConnectionStringBuilder();
            sconstrbuilder.DataSource = "mchsqlii.database.windows.net";
            sconstrbuilder.UserID = "mike";
            sconstrbuilder.Password = "!Strawberry123!";
            sconstrbuilder.InitialCatalog = "northwind";

            using (SqlConnection con = new SqlConnection(sconstrbuilder.ConnectionString))
            {
                con.Open();
                {
                    using (SqlCommand cmd = new SqlCommand(
                           "select customerid, companyname, contactname, contacttitle, address, city, phone from customers where customerid=@cid",
                           con))
                    {
                        cmd.Parameters.Add(new SqlParameter("cid", "alfki"));

                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();

                        cust.CustomerID = dr["CustomerID"].ToString();
                        cust.CompanyName = dr["CompanyName"].ToString();
                        cust.CompanyName = dr["ContactName"].ToString();
                        cust.CompanyName = dr["ContactTitle"].ToString();
                        cust.CompanyName = dr["Address"].ToString();
                        cust.CompanyName = dr["City"].ToString();
                        cust.CompanyName = dr["Phone"].ToString();
                    }
                }
            }
        }
    }

    public class Customer
    {
        public string CustomerID;
        public string CompanyName;
        public string ContactName;
        public string ContactTitle;
        public string Address;
        public string City;
        public string Phone;
    }
}
