﻿using AutoMapper;
using Paladin.Models;
using Paladin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paladin.Infrastructure;

namespace Paladin.Controllers
{
    [WorkflowFilter(MinRequiredStage = (int)WorkflowValues.VehicleInfo,
        CurrentStage = (int)WorkflowValues.ProductInfo)]
    public class ProductsController : Controller
    {
        private PaladinDbContext _context;

        public ProductsController()
        {
            _context = new PaladinDbContext();
        }
        // GET: Features
        public ActionResult ProductInfo()
        {
            if (Session["Tracker"] == null)
            {
                return RedirectToAction("ApplicantInfo", "Applicant");
            }
            var tracker = (Guid)Session["Tracker"];

            var products = new ProductsVM();
            var existingProducts = _context.Applicants.FirstOrDefault(x => x.ApplicantTracker == tracker).Products.FirstOrDefault();
            if (existingProducts != null)
            {
                products = Mapper.Map<ProductsVM>(existingProducts);
            }
            
            return View(products);
        }

        [HttpPost]
        public ActionResult ProductInfo(ProductsVM vm)
        {
            if (Session["Tracker"] == null)
            {
                return RedirectToAction("ApplicantInfo", "Applicant");
            }
            var tracker = (Guid)Session["Tracker"];

            if (ModelState.IsValid)
            {
                var applicant = _context.Applicants.FirstOrDefault(x => x.ApplicantTracker == tracker);

                var existingProducts = applicant.Products.FirstOrDefault();
                if (existingProducts != null)
                {
                    Mapper.Map(vm, existingProducts);
                    _context.Entry(existingProducts).State = EntityState.Modified;
                }
                else
                {
                    var newProducts = Mapper.Map<Products>(vm);
                    applicant.Products.Add(newProducts);
                }
                _context.SaveChanges();
                return RedirectToAction("Final", "Home");
            }
            return View();
        }
    }
}