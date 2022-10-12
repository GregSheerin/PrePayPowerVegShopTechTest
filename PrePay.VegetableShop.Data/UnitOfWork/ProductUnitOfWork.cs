using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Data.Repository;

namespace PrePay.VegetableShop.Data.UnitOfWork
{
    public class ProductUnitOfWork : IUnitOfWork
    {
        private readonly ProductContext _context;

        public IProductRepository Products { get; private set; }

        public ProductUnitOfWork(ProductContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
