using Online_Store.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Online_Store.Controllers
{
    public class HomeController : Controller
    {
        private ShopDbEntities db = new ShopDbEntities();

        public ActionResult Index(string search, int? ProductTypeID)
        {
            if (TempData["cart"] != null)
            {
                List<Cart> cartList2 = TempData["cart"] as List<Cart>;

                int x = 0;

                foreach(var item in cartList2)
                {
                    x += item.ProductTotal;
                }
                TempData["total"] = x;
            }
            TempData.Keep();

            IEnumerable<Product> products;

            if (ProductTypeID == null)
            {
                products = db.Products.Include(p => p.ProductType);
                products = db.Products.Where(p => p.ProductName.Contains(search) || search == null);
            }
            else
            {
                products = db.Products.Include(p => p.ProductType);
                products = db.Products.Where(p => (p.ProductTypeID.Equals((int)ProductTypeID)) && (p.ProductName.Contains(search) || search == null));
            }
            
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "ProductTypeName");
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            List<Product> similarProducts = db.Products.Where(p => p.ProductTypeID == product.ProductTypeID && p.ProductID != product.ProductID).Take(4).ToList();
            ViewBag.similarProduct = similarProducts;

            return View(product);
        }

        public ActionResult AddCart(int? id)
        {
            if (Session["UserId"] != null)
            {
                var products = db.Products.Where(p => p.ProductID == id);
                return View(products.SingleOrDefault());
            }
            else
                return RedirectToAction("Login", "Users");
        }

        List<Cart> cartList = new List<Cart>();

        [HttpPost]
        public ActionResult AddCart(Product pd, string qty, int id)
        {
            Product products = db.Products.Where(p => p.ProductID == id).SingleOrDefault();

            Cart c = new Cart();

            c.ProductId = products.ProductID;
            c.ProductName = products.ProductName;
            c.ProductPrice = (int)products.ProductPrice;
            c.ProductQuantity = Convert.ToInt32(qty);
            c.ProductTotal = c.ProductPrice * c.ProductQuantity;

            if (TempData["cart"] == null)
            {
                cartList.Add(c);
                TempData["cart"] = cartList;
            }
            else
            {
                List<Cart> cartList2 = TempData["cart"] as List<Cart>;
                int flag = 0;
                foreach (var item in cartList2)
                {
                    if (item.ProductId == c.ProductId)
                    {
                        item.ProductQuantity += c.ProductQuantity;
                        item.ProductTotal += c.ProductTotal;
                        flag = 1;
                    }
                }

                if(flag == 0)
                {
                    cartList2.Add(c);
                }

                TempData["cart"] = cartList2;
            }

            TempData.Keep();

            return RedirectToAction("Index");
        }

        public ActionResult Remove(int? id)
        {
            List<Cart> cartList2 = TempData["cart"] as List<Cart>;
            Cart c = cartList2.Where(a => a.ProductId == id).SingleOrDefault();
            cartList2.Remove(c);

            int h = 0;

            foreach (var item in cartList2)
            {
                h += item.ProductTotal;
            }
            TempData["total"] = h;

            return RedirectToAction("Checkout");
        }

        public ActionResult Checkout()
        {
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(Order o)
        {
            List<Cart> cartList = TempData["cart"] as List<Cart>;
            Invoice inv = new Invoice();
            inv.UserID = Convert.ToInt32(Session["UserId"].ToString());
            inv.InvoiceTotal = Convert.ToInt16(TempData["total"]);
            inv.InvoiceDateTime = DateTime.Now;
            db.Invoices.Add(inv);
            db.SaveChanges();

            foreach (var item in cartList)
            { 
                Order od = new Order();
                od.UserID = Convert.ToInt32(Session["UserId"].ToString());
                od.InvoiceID = inv.InvoiceID;
                od.ProductID = item.ProductId;
                od.OrderQuantity = Convert.ToInt16(item.ProductQuantity);
                od.OrderUnitPrice = Convert.ToInt16(item.ProductPrice);
                od.OrderTotal = Convert.ToInt16(item.ProductTotal);
                od.OrderDateTime = DateTime.Now;
                db.Orders.Add(od);
                db.SaveChanges();

                Random rnd = new Random();
                Review rev = new Review();
                rev.OrderID = od.OrderID;
                rev.Rating = rnd.Next(1, 5);
                db.Reviews.Add(rev);
                db.SaveChanges();
            }


            TempData.Remove("total");
            TempData.Remove("cart");
            TempData["message"] = "Order Complete";

            return RedirectToAction("Index");
        }


        public ActionResult UserInvoice()
        {
            if (Session["UserId"] != null)
            {
                var uid = (int)Session["UserId"];
                var invoice = db.Invoices.Where(p => p.UserID == uid);
                return View(invoice.ToList());
            }
            else
                return RedirectToAction("Login", "Users");
        }


        public ActionResult UserOrder(int? id)
        {
            if (Session["UserId"] != null || Session["AdminId"] != null)
            {
                var order = db.Orders.Where(o => o.InvoiceID == id);
                return View(order.ToList());
            }
            else
                return RedirectToAction("Login", "Users");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Online Store description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Online Store contact page.";

            return View();
        }
    }
}