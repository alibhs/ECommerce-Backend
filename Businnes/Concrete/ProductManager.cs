using Businnes.Abstract;
using Businnes.BusinessAspects.Autofac;
using Businnes.Constants;
using Businnes.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Businnes.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService;


        public ProductManager(IProductDal productDal, ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }

        [ValidationAspect(typeof(ProductValidator))]
        //[CacheRemoveAspect("IProductService.Get")] 
        public IResult Add(Product product)
        {   //business code =
            //validation = nesnenin iş kurallarına dahil olabilmesi için doğru olup olmadığını kontrol eder 

            IResult result = BusinessRules.Run(CheckIfProductContOfCategoryCorrect(product.CategoryId),
                 CheckIfProductNameExists(product.ProductName), CheckIfCategoryLimitExceded());

            if (result != null)
            {
                return result; //error result
            }
            _productDal.Add(product);

            return new SuccessResult(Messages.ProductAdded);

        }

        public IResult Update(Product product)
        {
            IResult result = BusinessRules.Run(CheckIfProductContOfCategoryCorrect(product.CategoryId),
                CheckIfProductNameExists(product.ProductName), CheckIfCategoryLimitExceded());
              
            if(result != null)
            {
                return result;
            }
            _productDal.Update(product);
            return new SuccessResult(Messages.ProductUpdated);

        }

        public IResult Delete(Product product)
        {
          
            _productDal.Delete(product);
            return new SuccessResult(Messages.ProductDeleted);

        }

        public IDataResult<List<Product>> GetAll()
        {

            return new SuccessDataResult<List<Product>>(_productDal.GetAll(), Messages.ProductListed);

        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.CategoryId == id));
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId));
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.Price >= min && p.Price <= max));
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }
        private IResult CheckIfProductContOfCategoryCorrect(int categorId)
        {
            var result = _productDal.GetAll(p => p.CategoryId == categorId).Count;
            if (result >= 15)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }
        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }
        private IResult CheckIfCategoryLimitExceded()
        {
            var result = _categoryService.GetAll();
            if (result.Data.Count > 15)
            {
                return new ErrorResult(Messages.CategoryLimitExceded);

            }
            return new SuccessResult();
        }
    }
}
