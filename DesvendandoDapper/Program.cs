using System.Data.SqlClient;
using Dapper;

var connectionString = "Server=localhost; Database=DotLive08_Ecommerce; User Id=sa; Password=1q2w3e4r@#$;trustServerCertificate=true;MultipleActiveResultSets=True";


using (var connection = new SqlConnection(connectionString))
{
    connection.Open();

    //Retorna a quantidade de linhas afetadas
    var sqlInsert = "INSERT INTO Orders(OrderId, CustomersFk,OrderDetails) VALUES(@OrderId, @CustomersFk, @OrderDetails)";
    connection.Execute(sqlInsert, new { OrderId = 1, CustomersFk = 1, OrderDetails = "Detalhes" });

    //Retorna o Id INSERIDO
    var sqlInsert2 = "INSERT INTO Orders OUTPUT INSERTED.OrderId VALUES(@OrderId, @CustomersFk, @OrderDetails)";
    var id = connection.ExecuteScalar<int>(sqlInsert2, new { OrderId = 2, CustomersFk = 1, OrderDetails = "Detalhes" });

    
}

Console.Read();
public class Order
{
    public int OrderId { get; set; }
    public int CustomersFk { get; set; }
    public string OrderDetails { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
}


public class Customer
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
}

public class OrderItem
{
    public int OrderItemId { get; set; }
    public string ProductName { get; set; }
    public int ProductQuantity { get; set; }
    public int OrderFk { get; set; }
}