using BookHub.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookHub.Models;
using BookHub.DataAccess.Data;

namespace BookHub.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product product) 
        {
            
            var objFromDb = _db.Products.FirstOrDefault(u=>u.Id==product.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = objFromDb.Title;
                objFromDb.ISBN = objFromDb.ISBN;
                objFromDb.Price = objFromDb.Price;
                objFromDb.Price50 = objFromDb.Price50;
                objFromDb.ListPrice = objFromDb.ListPrice;
                objFromDb.Price100 = objFromDb.Price100;
                objFromDb.Description = objFromDb.Description;
                objFromDb.CategoryId = objFromDb.CategoryId;
                objFromDb.Author = objFromDb.Author;
                if (product.ImageUrl != null)
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
