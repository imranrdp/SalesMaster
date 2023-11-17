using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesMaster.Models;
using SalesMaster.Models.ViewModel;
using System.Reflection;

namespace SalesMaster.Controllers
{
    public class SalesMasterController : Controller
    {
        private readonly AppDbContext _db;

        public SalesMasterController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string search)
        {
            var salesQuery = _db.SalesMasters.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                salesQuery = salesQuery.Where(s =>
                    s.CustomerName.Contains(search) || s.InvoiceNumber.Contains(search) ||
                    s.MobileNo.Contains(search) 
                  
                );
            }

            salesQuery = salesQuery.Include(i => i.SalesDetails);

            var SalesList = salesQuery.ToList();
            return View(SalesList);
        }

        public IActionResult Create()
        {
            SalesmasterViewModel sMaster = new SalesmasterViewModel();
            sMaster.SalesDetails.Add(new SalesDetails() { Id = 1 });

            return View(sMaster);
        }
        [HttpPost]

        public IActionResult Create(SalesmasterViewModel sMaster)
        {
            SalesMasterTable obj = new SalesMasterTable();

            obj.InvoiceNumber = sMaster.InvoiceNumber;
            obj.CustomerName = sMaster.CustomerName;
            obj.MobileNo = sMaster.MobileNo;
            obj.Address = sMaster.Address;
            _db.Add(obj);
            _db.SaveChanges();
            var user = _db.SalesMasters.FirstOrDefault(x => x.MobileNo == sMaster.MobileNo);
            if (user != null)
            {
                if (sMaster.SalesDetails.Count > 0)
                {
                    foreach (var item in sMaster.SalesDetails)
                    {
                        SalesDetails objM = new SalesDetails();
                        objM.SalesMasterId = user.Id;
                        objM.ProductCode = item.ProductCode;
                        objM.ProductName = item.ProductName;
                        objM.Price= item.Price;
                        objM.Quantity = item.Quantity;
                        _db.Add(objM);
                    }
                }
            }
            _db.SaveChanges();
            return RedirectToAction("index");
        }


        public ActionResult Delete(int? id)
        {
            var app = _db.SalesMasters.Find(id);
            var existsDetails = _db.SalesDetails.Where(e => e.SalesMasterId == id).ToList();
            foreach (var exp in existsDetails)
            {
                _db.SalesDetails.Remove(exp);
            }
            _db.Entry(app).State = EntityState.Deleted;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            SalesMasterTable app = _db.SalesMasters.Include(a => a.SalesDetails).FirstOrDefault(x => x.Id == id);

            if (app != null)
            {
                SalesmasterViewModel aps = new SalesmasterViewModel()
                {
                    SalesMasterId = app.Id,
                    InvoiceNumber = app.InvoiceNumber,
                    MobileNo = app.MobileNo,
                    CustomerName = app.CustomerName,
                    Address = app.Address,
                    SalesDetails = app.SalesDetails.ToList()
                };

                return View("Edit", aps);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SalesmasterViewModel sMaster)
        {
            try
            {
                SalesMasterTable existingSales = _db.SalesMasters
                    .Include(a => a.SalesDetails)
                    .FirstOrDefault(x => x.Id == sMaster.SalesMasterId);

                if (existingSales != null)
                {
                    existingSales.InvoiceNumber = sMaster.InvoiceNumber;
                    existingSales.CustomerName = sMaster.CustomerName;
                    existingSales.MobileNo = sMaster.MobileNo;
                    existingSales.Address = sMaster.Address;

                    existingSales.SalesDetails.Clear();
                    foreach (var item in sMaster.SalesDetails)
                    {
                        var newDetails = new SalesDetails
                        {
                            SalesMasterId = existingSales.Id,
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            Price= item.Price,
                            Quantity= item.Quantity,
                        };

                        existingSales.SalesDetails.Add(newDetails);
                    }

                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return NotFound();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                var entry = ex.Entries.FirstOrDefault();
                if (entry != null)
                {
                    var databaseValues = entry.GetDatabaseValues();
                    if (databaseValues != null)
                    {
                        var databaseSalesMaster = (SalesMasterTable)databaseValues.ToObject();

                        ModelState.AddModelError("", "The entity you are trying to edit has been modified by another user. Please refresh the page and try again.");


                        entry.OriginalValues.SetValues(databaseValues);


                        sMaster.SalesDetails = databaseSalesMaster.SalesDetails.ToList();

                        return View("Edit", sMaster);
                    }
                }

                ModelState.AddModelError("", "The entity you are trying to edit has been deleted by another user.");

            }

            return RedirectToAction("Index");
        }


    }
}
