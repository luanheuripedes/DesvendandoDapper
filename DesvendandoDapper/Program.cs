using System.Data;
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
    /*
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
    */

    //SELECT com objetos mesclados
    var sqlOrderByIdProjection = @"SELECT
		o.OrderId, o.OrderDetails,
		c.CustomerId, c.CustomerName
	    FROM Orders o
	    INNER JOIN Customers c ON o.CustomersFk = c.CustomerId
	    WHERE o.OrderId = @OrderId;";

    var parameters = new DynamicParameters();
    parameters.Add("@OrderId", 1);


    var orderWithCustomerResult = connection.Query<Order, Customer, Order>(sqlOrderByIdProjection,
        (order, customer) =>
    {
        order.Customer = customer;
        order.CustomersFk = customer.CustomerId;
        return order;
    }, param: parameters, splitOn: "CustomerId")
        .SingleOrDefault();

    //Com procedure
    var orderDetailsWithCustomerFromSp = connection.Query<Order, Customer, Order>("GetOrderDetails",
        (order, customer) =>
        {
            order.Customer = customer;
            order.CustomersFk = customer.CustomerId;

            return order;
        },
        param: parameters,
        splitOn: "CustomerId",
        commandType: CommandType.StoredProcedure).SingleOrDefault();

    var sqlOrderDetailsWithOrderItems = @"SELECT 
            o.OrderId, o.OrderDetails, 
            c.CustomerId, c.CustomerName, 
            od.OrderItemId, od.ProductName, od.ProductQuantity 
          FROM Orders o
          INNER JOIN Customers c ON o.CustomersFk = c.CustomerId
          INNER JOIN OrderItems od ON o.OrderId = od.OrderFk
          WHERE o.OrderId = @OrderId";


    //Preenchendo as variaveis de navegação do Order
    var orderDictionary = new Dictionary<int, Order>();
    var orderDetailsWithOrderItems = connection.Query<Order, Customer, OrderItem, Order>(
        sqlOrderDetailsWithOrderItems,
        (order, customer, orderItem) =>
        {
            Order orderEntry;

            if (!orderDictionary.TryGetValue(order.OrderId, out orderEntry))
            {
                orderEntry = order;
                orderEntry.Items = new List<OrderItem>();
                orderEntry.CustomersFk = customer.CustomerId;

                orderDictionary.Add(orderEntry.OrderId, orderEntry);
            }

            if (orderEntry.Customer == null)
                orderEntry.Customer = customer;

            orderItem.OrderFk = order.OrderId;

            orderEntry.Items.Add(orderItem);

            return orderEntry;
        },
        new { OrderId = 1 },
        splitOn: "CustomerId,OrderItemId"
        )
        .Distinct()
        .SingleOrDefault();

    //DELETE
    var sqlDelete = "DELETE FROM Orders WHERE OrderId = @OrderId";
    connection.Execute(sqlDelete, new { OrderId = 2 });

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