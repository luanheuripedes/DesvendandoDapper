using System.Data.SqlClient;
using Dapper;

var connectionString = "Server=localhost; Database=DotLive08_Ecommerce; User Id=sa; Password=1q2w3e4r@#$;trustServerCertificate=true;MultipleActiveResultSets=True";


using (var connection = new SqlConnection(connectionString))
{
    connection.Open();

    //Retorna a quantidade de linhas afetadas
    /*
    var sqlInsert = "INSERT INTO Orders(OrderId, CustomersFk,OrderDetails) VALUES(@OrderId, @CustomersFk, @OrderDetails)";
    connection.Execute(sqlInsert, new { OrderId = 1, CustomersFk = 1, OrderDetails = "Detalhes" });
    */

    //Retorna o Id INSERIDO
    /*
    var sqlInsert2 = "INSERT INTO Orders OUTPUT INSERTED.OrderId VALUES(@OrderId, @CustomersFk, @OrderDetails)";
    var id = connection.ExecuteScalar<int>(sqlInsert2, new { OrderId = 2, CustomersFk = 1, OrderDetails = "Detalhes" });
    */

    //UPDATE
    /*
    var sqlUpdate = "Update Orders Set OrderDetails = @OrderDetails WHERE OrderId = @OrderId";
    connection.Execute(sqlUpdate, new { OrderDetails = "Update Details", OrderId = 1 });
    */

    //Stored Procedures
    /*
    var parameters = new DynamicParameters();
    parameters.Add("@OrderId", 1);

    var orderDetailsFromSp = connection.Query<OrderDetailResult>("GetOrderDetails", parameters, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
    */

    //Select SIMPLES
    var sqlOrderById = @"SELECT * FROM Orders Where OrderId = @OrderId";
    var sqlOrderByIdResult = connection.QuerySingleOrDefault<Order>(sqlOrderById, new { OrderId = 2 });


    var sqlAllOrders = @"SELECT * FROM Orders";
    var sqlAllOrdersResult = connection.Query<Order>(sqlAllOrders).ToList();

    var sqlOrderByIdProjection = @"SELECT
		o.OrderId, o.OrderDetails,
		c.CustomerId, c.CustomerName
	    FROM Orders o
	    INNER JOIN Customers c ON o.CustomersFk = c.CustomerId
	    WHERE o.OrderId = @OrderId;";

    var sqlOrderByIdProjectionResult = connection.Query<OrderDetailResult>(sqlOrderByIdProjection, new { OrderId = 1}).SingleOrDefault();

    //SELECT com objetos mesclados

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

public class OrderDetailResult
{
    public int OrderId { get; set; }
    public string OrderDetails { get; set; }
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
}