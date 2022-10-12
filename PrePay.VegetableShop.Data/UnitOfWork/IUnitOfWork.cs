using PrePay.VegetableShop.Data.Repository;
using System;

namespace PrePay.VegetableShop.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        int Complete();
    }
}
