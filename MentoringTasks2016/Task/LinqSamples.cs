﻿// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Linq;
using SampleSupport;
using Task.Data;
using System.Collections.Generic;

// Version Mad01

namespace SampleQueries
{
    public interface ISamples
    {
        List<Customer> GetCustomersWithOrderSumMoreThan(decimal totalOrderSum = 500);
        List<Customer> GetCustomersWithAnyOrderMoreThan(decimal orderTotal = 100);
        List<CustomerStatistic> GetGustomersStartDate();
        IEnumerable<ProductCategoryGroup> GetProductCategories();
    }

    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness, ISamples
    {
        private IDataSource _dataSource;

        public LinqSamples(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        [Category("Linq Demo")]
        [Title("Where Orders Total Sum < x")]
        [Description("")]
        public List<Customer> GetCustomersWithOrderSumMoreThan(decimal totalOrderSum = 500)
        {
            return _dataSource.Customers
                        .Where(customer => customer.Orders.Sum(order => order.Total) > totalOrderSum)
                        .ToList();
        }

        public List<Customer> GetCustomersWithAnyOrderMoreThan(decimal orderTotal = 100)
        {
            return _dataSource.Customers
                        .Where(customer=>customer.Orders.Any(order=> order.Total > orderTotal))
                        .ToList();
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;

            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums)
            {
                Console.WriteLine(x);
            }
        }

        public IEnumerable<ProductCategoryGroup> GetProductCategories()
        {
            var groupedProducts = _dataSource.Products
                .GroupBy(p => p.Category)
                .Select(gp => new ProductCategoryGroup
                              {
                                  Key = gp.Key,
                                  Entities = gp.GroupBy(p => p.UnitsInStock)
                                               .Select(x => new ProductUnitGroup
                                                            {
                                                                Key = x.Key,
                                                                Entities = x.ToList()
                                                            })
                                               .ToList()
                              })
                .ToList();

            foreach(var item in groupedProducts)
            {
                var lastProductGroup = item.Entities.Last();
                lastProductGroup.Entities = lastProductGroup.Entities.OrderBy(x => x.UnitPrice).ToList();
            }

            return groupedProducts;
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2")]
        [Description("This sample return return all presented in market products")]

        public void Linq2()
        {
            var products =
                from p in _dataSource.Products
                where p.UnitsInStock > 0
                select p;

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }

        public List<CustomerStatistic> GetGustomersStartDate()
        {
            var customersWithOrders = _dataSource.Customers.Where(c => c.Orders.Any());
            var customersWithOrderDate =
                customersWithOrders.Select(
                    c =>
                        new CustomerStatistic
                        {
                            Customer = c,
                            StartDateTime = c.Orders.OrderBy(o => o.OrderDate).First().OrderDate
                        });
            return customersWithOrderDate.OrderBy(x => x.StartDateTime)
                    .ThenByDescending(x => x.Customer.Orders.Count())
                    .ThenBy(x => x.Customer.CompanyName)
                    .ToList();
        }
    }
}