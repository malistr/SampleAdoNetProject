using Microsoft.AspNetCore.Mvc;
using SampleAdo.NETProject.Models;
using System.Data.SqlClient;

namespace SampleAdo.NETProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("NorthwindConnection");
        }
        public IActionResult Index()
        {
            List<Product> products = GetProducts();
            return View(products);
        }

        public List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(_connectionString))  //using kullanımı connection,reader,command için önemlidir. Açık olan bağlantının otomatik olarak dispose olmasını sağlar.
            {
                connection.Open();
                string sql = "Select ProductID,ProductName,UnitPrice From Products";
                using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())  //ExecuteReader komutu artık kayıtları okuyor, çalıştırıyor.
                    {
                        while (sqlDataReader.Read())
                        {
                            Product product = new Product
                            {
                                ProductID = Convert.ToInt32(sqlDataReader["ProductID"]),
                                ProductName = Convert.ToString(sqlDataReader["ProductName"]),
                                UnitPrice = Convert.ToDecimal(sqlDataReader["UnitPrice"])
                            };

                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }
    }
}
