using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Linq.Expressions;


namespace DemoFunctionApp
{
    public static class GetCustomer
    {
        //[FunctionName("GetCustomer")]
        //public static async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        //    ILogger log)

        [FunctionName("GetCustomer")]
        public static ActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                Customer cust = new Customer();

                log.LogInformation("C# HTTP trigger function processed a request.");

                SqlConnectionStringBuilder sconstrbuilder = new SqlConnectionStringBuilder();
                sconstrbuilder.DataSource = "mchsql.database.windows.net";
                sconstrbuilder.UserID = "student";
                sconstrbuilder.Password = "Pa55w.rd";
                sconstrbuilder.InitialCatalog = "northwind";

                using (SqlConnection con = new SqlConnection(sconstrbuilder.ConnectionString))
                {
                    con.Open();
                    {
                        using (SqlCommand cmd = new SqlCommand(
                               "select customerid, companyname, contactname, contacttitle, address, city, phone from customers where customerid=@cid",
                               con))
                        {
                            cmd.Parameters.Add(new SqlParameter("cid", req.Query["customerid"].ToString()));

                            SqlDataReader dr;
                            dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                            dr.Read();

                            cust.CustomerID = dr["CustomerID"].ToString();
                            cust.CompanyName = dr["CompanyName"].ToString();
                            cust.ContactName = dr["ContactName"].ToString();
                            cust.ContactTitle = dr["ContactTitle"].ToString();
                            cust.Address = dr["Address"].ToString();
                            cust.City = dr["City"].ToString();
                            cust.Phone = dr["Phone"].ToString();
                        }
                    }
                }

                //cust.CustomerID = "ALFKI";
                //cust.CompanyName = "Alfreds Futterkiste";
                //cust.ContactName = "Maria Anders";
                //cust.ContactTitle = "Sales Representative";
                //cust.Address = "Obere Str. 57";
                //cust.City = "Berlin";
                //cust.Phone = "030-0074321";


                return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(cust));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString() + req.Query["customerid"].ToString());
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
