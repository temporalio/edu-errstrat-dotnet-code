// This file is designated to run the Workflow
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Temporalio.Client;
using TemporalioHandlingErrors.Practice.Workflow;
using TemporalioHandlingErrors.Practice.Workflow.Models;

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(new("localhost:7233")
{
    LoggerFactory = LoggerFactory.Create(builder =>
        builder.
            AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] ").
            SetMinimumLevel(LogLevel.Information)),
});

var order = CreatePizzaOrder();

// Run workflow
var result = await client.ExecuteWorkflowAsync(
    (PizzaWorkflow wf) => wf.RunAsync(order),
    new WorkflowOptions
    {
        Id = $"pizza-workflow-order-{order.OrderNumber}",
        TaskQueue = WorkflowConstants.TaskQueueName,
    });

Console.WriteLine($"""
Workflow result:
  Order Number: {result.OrderNumber}
  Status: {result.Status}
  Confirmation Number: {result.ConfirmationNumber}
  Billing Timestamp: {result.BillingTimestamp}
  Amount: {result.Amount}
""");

PizzaOrder CreatePizzaOrder()
{
    var customer = new Customer(
        CustomerId: 12983,
        Name: "María García",
        Phone: "415-555-7418",
        Email: "maria1985@example.com",
        CreditCardNumber: "1234567890123456");

    var address = new Address(
        Line1: "701 Mission Street",
        City: "San Francisco",
        State: "CA",
        PostalCode: "94103",
        Line2: "Apartment 9C");

    var p1 = new Pizza(
        Description: "Large, with mushrooms and onions",
        Price: 1500);
    var p2 = new Pizza(
        Description: "Small, with pepperoni",
        Price: 1200);
    var p3 = new Pizza(
        Description: "Medium, with extra cheese",
        Price: 1300);

    var pizzaList = new List<Pizza> { p1, p2, p3 };
    var pizzas = new Collection<Pizza>(pizzaList);

    return new PizzaOrder(
        OrderNumber: "Z1238",
        Customer: customer,
        Items: pizzas,
        Address: address,
        IsDelivery: true);
}