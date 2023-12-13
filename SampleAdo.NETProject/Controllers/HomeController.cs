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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            InsertProduct(product);
            return RedirectToAction("Index","Home");
        }

        private void InsertProduct(Product product)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                string sql = "Insert into Products(ProductName,UnitPrice) Values(@ProductName,@UnitPrice)";
                using (SqlCommand sqlCommand = new SqlCommand(sql,sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@ProductName",product.ProductName);
                    sqlCommand.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public IActionResult Edit(int id)
        {
            Product product = GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        private Product GetProductById(int id) //public her yerden çağrılabilir, yani her controller'dan public metodlarını çağırabiliriz. private metodlar her yerden çağrılamaz. Bulunduğu controller içerisinden erişilebilir.
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                string sql = "select ProductID,ProductName,UnitPrice from Products where ProductID = @ProductID";
                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@ProductID",id);

                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        if (sqlDataReader.Read())
                        {
                            Product product = new Product()
                            {
                                ProductID = Convert.ToInt32(sqlDataReader["ProductID"]),
                                ProductName = Convert.ToString(sqlDataReader["ProductName"]),
                                UnitPrice = Convert.ToDecimal(sqlDataReader["UnitPrice"])
                            };

                            return product;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            UpdateProduct(product);
            return RedirectToAction("Index","Home");
        }

        private void UpdateProduct(Product product)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                string sql = "Update Products Set ProductName = @ProductName, UnitPrice = @UnitPrice where ProductID = @ProductID";
                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@ProductID", product.ProductID);
                    sqlCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                    sqlCommand.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
