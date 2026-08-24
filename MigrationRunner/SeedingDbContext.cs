using Printpress.Domain;
using Printpress.Infrastructure;

namespace Printpress.MigrationRunner;

internal sealed class SeedingDbContext
{

    private readonly ApplicationDbContext _dbContext;

    public SeedingDbContext(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static Guid G(int n) => Guid.Parse($"00000000-0000-0000-0000-{n:D12}");

    #region Seeding mock data
    public void SeedingMockData()
    {
        SeedingOrdersMockData();

        SeedingClientsMockData();

        SeedingServiceMockData();

        SeedingGroupMockData();

        SeedItemsForOrderGroups();
    }

    private void SeedingOrdersMockData()
    {
        List<Order> orders =
        [
            new Order { Id = G(1), Name = "طلبيه  1 ", ClientId = G(1), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(2), Name = "طلبيه  2 ", ClientId = G(2), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(3), Name = "طلبيه  3 ", ClientId = G(3), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(4), Name = "طلبيه  4 ", ClientId = G(4), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(5), Name = "طلبيه  5 ", ClientId = G(5), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(6), Name = "طلبيه  6 ", ClientId = G(6), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(7), Name = "طلبيه  7 ", ClientId = G(7), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(8), Name = "طلبيه 10 ", ClientId = G(8), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(9), Name = "طلبيه 11 ", ClientId = G(9), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(10), Name = "طلبيه 21 ", ClientId = G(10), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(11), Name = "طلبيه 31 ", ClientId = G(11), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(12), Name = "طلبيه 41 ", ClientId = G(12), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(13), Name = "طلبيه 51 ", ClientId = G(13), TotalPrice = 500, TotalPaid = 50 },
            new Order { Id = G(14), Name = "طلبيه 61 ", ClientId = G(14), TotalPrice = 500, TotalPaid = 50 },
        ];


        foreach (var order in orders)
        {
            // Check if the order already exists in the database
            if (!_dbContext.Order.Any(o => o.Id == order.Id))
            {
                _dbContext.Order.Add(order);
                Console.WriteLine(string.Format("new order add with id {0}", order.Id));
            }
        }
    }

    private void SeedingGroupMockData()
    {
        List<OrderGroup> orderGroup =
        [
        new OrderGroup { Id = G(1), Name = "مجموعة 1", OrderId = G(1) },
        new OrderGroup { Id = G(2), Name = "مجموعة 2", OrderId = G(1) },
        new OrderGroup { Id = G(3), Name = "مجموعة 3", OrderId = G(1) },
        new OrderGroup { Id = G(4), Name = "مجموعة 4", OrderId = G(1) },

        new OrderGroup { Id = G(5), Name = "مجموعة 1", OrderId = G(2) },
        new OrderGroup { Id = G(6), Name = "مجموعة 2", OrderId = G(2) },
        new OrderGroup { Id = G(7), Name = "مجموعة 3", OrderId = G(2) },
        new OrderGroup { Id = G(8), Name = "مجموعة 4", OrderId = G(2) },

        new OrderGroup { Id = G(9), Name = "مجموعة  1", OrderId = G(3) },
        new OrderGroup { Id = G(10), Name = "مجموعة 2", OrderId = G(3) },
        new OrderGroup { Id = G(11), Name = "مجموعة 3", OrderId = G(3) },
        new OrderGroup { Id = G(12), Name = "مجموعة 4", OrderId = G(3) },

        new OrderGroup { Id = G(13), Name = "مجموعة 1", OrderId = G(4) },
        new OrderGroup { Id = G(14), Name = "مجموعة 2", OrderId = G(4) },
        new OrderGroup { Id = G(15), Name = "مجموعة 3", OrderId = G(4) },
        new OrderGroup { Id = G(16), Name = "مجموعة 4", OrderId = G(4) },

        new OrderGroup { Id = G(17), Name = "مجموعة 1", OrderId = G(5) },
        new OrderGroup { Id = G(18), Name = "مجموعة 2", OrderId = G(5) },
        new OrderGroup { Id = G(19), Name = "مجموعة 3", OrderId = G(5) },
        new OrderGroup { Id = G(20), Name = "مجموعة 4", OrderId = G(5) },

        new OrderGroup { Id = G(21), Name = "مجموعة 1", OrderId = G(6) },
        new OrderGroup { Id = G(22), Name = "مجموعة 2", OrderId = G(6) },
        new OrderGroup { Id = G(23), Name = "مجموعة 3", OrderId = G(6) },
        new OrderGroup { Id = G(24), Name = "مجموعة 4", OrderId = G(6) },

        new OrderGroup { Id = G(25), Name = "مجموعة 1", OrderId = G(7) },
        new OrderGroup { Id = G(26), Name = "مجموعة 2", OrderId = G(7) },
        new OrderGroup { Id = G(27), Name = "مجموعة 3", OrderId = G(7) },
        new OrderGroup { Id = G(28), Name = "مجموعة 4", OrderId = G(7) },

        new OrderGroup { Id = G(29), Name = "مجموعة 1", OrderId = G(8) },
        new OrderGroup { Id = G(30), Name = "مجموعة 2", OrderId = G(8) },
        new OrderGroup { Id = G(31), Name = "مجموعة 3", OrderId = G(8) },
        new OrderGroup { Id = G(32), Name = "مجموعة 4", OrderId = G(8) },

        new OrderGroup { Id = G(33), Name = "مجموعة 1", OrderId = G(9) },
        new OrderGroup { Id = G(34), Name = "مجموعة 2", OrderId = G(10) },
        new OrderGroup { Id = G(35), Name = "مجموعة 3", OrderId = G(11) },
        new OrderGroup { Id = G(36), Name = "مجموعة 4", OrderId = G(12) },
        new OrderGroup { Id = G(37), Name = "مجموعة 4", OrderId = G(13) },
        new OrderGroup { Id = G(38), Name = "مجموعة 4", OrderId = G(14) },



    ];

        foreach (var group in orderGroup)
        {
            // Check if the order already exists in the database
            if (!_dbContext.OrderGroup.Any(o => o.Id == group.Id))
            {
                _dbContext.OrderGroup.Add(group);
                Console.WriteLine(string.Format("order group add {0} for order {1}", group.Name, group.OrderId));
            }
        }
    }

    private void SeedItemsForOrderGroups()
    {
        // Retrieve all existing OrderGroups from the database
        var orderGroups = _dbContext.OrderGroup.ToList();

        // Create a list to hold new items
        List<OrderItem> itemsToAdd = new();

        var itemId = 1;

        foreach (var group in orderGroups)
        {
            // Define Arabic item names
            var arabicItemNames = new[]
            {
            "كناب 1",
            "كناب 2",
            "كناب 3",
            "كناب 4",
            "كناب 5"
        };
            // Create five items for each order group
            for (int i = 0; i < arabicItemNames.Length; i++)
            {
                var newItem = new OrderItem
                {
                    Id = G(itemId++),
                    Name = arabicItemNames[i],
                    OrderGroupId = group.Id,
                    Quantity = (i + 1) * 2, // Example: 2, 4, 6, 8, 10
                    Price = (i + 1) * 10.5m // Example: 10.5, 21, 31.5, etc.
                };

                itemsToAdd.Add(newItem);
            }
        }

        foreach (var item in itemsToAdd)
        {
            // Check if the client already exists in the database
            if (!_dbContext.Item.Any(c => c.Id == item.Id))
            {
                _dbContext.Item.Add(item);
                Console.WriteLine(string.Format("new item add with id {0}", item.Id));

            }
        }


    }

    private void SeedingClientsMockData()
    {
        List<Client> clients =
        [
            new Client { Id = G(1), Name = "عميل 1", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(2), Name = "عميل 2", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(3), Name = "عميل 3", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(4), Name = "عميل 4", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(5), Name = "عميل 5", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(6), Name = "عميل 6", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(7), Name = "عميل 7", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(8), Name = "عميل 8", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(9), Name = "عميل 9", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(10), Name = "عميل 10", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(11), Name = "عميل 11", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(12), Name = "عميل 12", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(13), Name = "عميل 13", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(14), Name = "عميل 14", Mobile = "0111284858", Address = "first streat 488" },
            new Client { Id = G(15), Name = "عميل 15", Mobile = "0111284858", Address = "first streat 488" },
        ];


        foreach (var client in clients)
        {
            // Check if the client already exists in the database
            if (!_dbContext.Client.Any(c => c.Id == client.Id))
            {
                _dbContext.Client.Add(client);
                Console.WriteLine(string.Format("new client add with id {0}", client.Id));

            }
        }


    }


    private void SeedingServiceMockData()
    {
        List<Service> services =
        [
           new Service { Id = G(1), Name = "طباعة ورق 70 جم الوان", Price = 0.5m, ServiceCategoryId = G(101) },
           new Service { Id = G(2), Name = "طباعة ورق 80 جم الوان", Price = 0.7m, ServiceCategoryId = G(101) },
           new Service { Id = G(3), Name = "قص", Price = 0.5m, ServiceCategoryId = G(104) },
           new Service { Id = G(4), Name = "لصق", Price = 0.5m, ServiceCategoryId = G(103) },
           new Service { Id = G(5), Name = "تدبيس", Price = 0.5m, ServiceCategoryId = G(102) },
           new Service { Id = G(6), Name = "بيع", Price = 0.5m, ServiceCategoryId = G(105) },
        ];


        foreach (var service in services)
        {
            // Check if the order already exists in the database
            if (!_dbContext.Service.Any(o => o.Id == service.Id))
            {
                _dbContext.Service.Add(service);
                Console.WriteLine(string.Format("new service add with id {0}", service.Id));
            }
        }
    }

    #endregion


    public void SeedingData()
    {
        SeedingItemDetailsKeyLKP();
        SeedingServiceCategories();
        SeedingInventoryItemCategoryLKP();
        SeedingCashAccounts();
    }

    private void SeedingItemDetailsKeyLKP()
    {
        foreach (var itemDetailsKey in Enum.GetValues(typeof(ItemDetailsKeyEnum)))
        {
            if (!_dbContext.ItemDetailsKey_LKP.Any(i => i.Id == (int)itemDetailsKey))
            {
                _dbContext.ItemDetailsKey_LKP.Add(new ItemDetailsKey_LKP { Id = (int)itemDetailsKey, Name = itemDetailsKey.ToString() });

            }
        }
    }

    private void SeedingServiceCategories()
    {
        var categories = new[]
        {
            new ServiceCategory { Id = G(101), Code = "Printing",  Name = "طباعة",  RequireInventoryItem = false },
            new ServiceCategory { Id = G(102), Code = "Stapling",  Name = "تدبيس",  RequireInventoryItem = false },
            new ServiceCategory { Id = G(103), Code = "Clueing",   Name = "بشر",    RequireInventoryItem = false },
            new ServiceCategory { Id = G(104), Code = "Cutting",   Name = "قص",     RequireInventoryItem = false },
            new ServiceCategory { Id = G(105), Code = "Selling",   Name = "بيع",    RequireInventoryItem = false },
        };

        foreach (var category in categories)
        {
            if (!_dbContext.ServiceCategory.Any(c => c.Id == category.Id))
            {
                _dbContext.ServiceCategory.Add(category);
            }
        }
    }

    private void SeedingInventoryItemCategoryLKP()
    {
        foreach (var inventoryItemCategory in Enum.GetValues(typeof(InventoryItemCategoryEnum)))
        {
            if (!_dbContext.InventoryItemCategory_LKP.Any(i => i.Id == (int)inventoryItemCategory))
            {
                _dbContext.InventoryItemCategory_LKP.Add(new InventoryItemCategory_LKP { Id = (int)inventoryItemCategory, Name = inventoryItemCategory.ToString() });

            }
        }
    }

    private void SeedingCashAccounts()
    {
        var cashAccounts = new[]
        {
            new CashAccount { Id = G(1), Name = "الخزنة الرئيسية",    Balance = 0, Type = CashAccountType.Main, ObjectState = TrackingState.Added },
            new CashAccount { Id = G(2), Name = "خزنة قطع الغيار", Balance = 0, Type = CashAccountType.SpareParts, ObjectState = TrackingState.Added },
        };

        foreach (var cashAccount in cashAccounts)
        {
            if (!_dbContext.CashAccount.Any(c => c.Id == cashAccount.Id))
            {
                _dbContext.CashAccount.Add(cashAccount);
                Console.WriteLine(string.Format("new cash account add with name {0}", cashAccount.Name));
            }
        }
    }
}
