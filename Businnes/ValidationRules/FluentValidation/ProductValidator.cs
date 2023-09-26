using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Businnes.ValidationRules.FluentValidation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            //RuleFor(p => p.ProductName).MaximumLength(2);
            RuleFor(p => p.ProductName).NotEmpty();
            RuleFor(p => p.Price).NotEmpty();
            RuleFor(p => p.Price).GreaterThan(0).WithMessage("ürün fiyatı 0 dan büyük olmalı"); //withmessage tarayıcıda göstermek için
        }
    }
}
