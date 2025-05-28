using PitStore.Models;
using System.Collections.Generic;

namespace PitStore.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();
        public IEnumerable<Product> LowStockProducts { get; set; } = new List<Product>();
    }
} 