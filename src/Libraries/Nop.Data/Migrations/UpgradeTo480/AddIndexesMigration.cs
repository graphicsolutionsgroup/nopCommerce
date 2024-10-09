﻿using FluentMigrator;
using LinqToDB.DataProvider;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopSchemaMigration("2024-06-21 19:53:03", "AddIndexesMigration for 4.80.0")]
public class AddIndexesMigration : ForwardOnlyMigration
{
    private readonly INopDataProvider _dataProvider;

    public AddIndexesMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!Schema.Table(nameof(Customer)).Index("IX_Customer_Deleted").Exists())
            Create.Index("IX_Customer_Deleted")
                .OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.Deleted)).Ascending()
                .WithOptions().NonClustered();

        if (!Schema.Table(nameof(Order)).Constraint("AK_Order_OrderGuid").Exists())
        {
            var orders = _dataProvider.GetTable<Order>().GroupBy(p => p.OrderGuid, p => p)
                .Where(p => p.Count() > 1)
                .SelectMany(p => p)
                .ToList();

            if (orders.Any())
            {
                foreach (var order in orders)
                    order.OrderGuid = Guid.NewGuid();

                _dataProvider.UpdateEntities(orders);
            }

            Create.UniqueConstraint("AK_Order_OrderGuid")
                .OnTable(nameof(Order))
                .Column(nameof(Order.OrderGuid));
        }
    }
}