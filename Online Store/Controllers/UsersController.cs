using Online_Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_Store.Controllers
{
    public class UsersController : Controller
    {
        private ShopDbEntities db = new ShopDbEntities();

        // GET: Users
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();

                return Content("Sign Up Successfull");
                //return RedirectToAction("Index");
            }

            return View();
        }


        // GET: Users
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(TempUser tempUser)
        {
            if (ModelState.IsValid)
            {
                var admin = db.Admins.Where(a => a.AdminName.Equals(tempUser.UserName) && a.AdminPassword.Equals(tempUser.UserPassword)).FirstOrDefault();
                var user = db.Users.Where(u => u.UserName.Equals(tempUser.UserName) && u.UserPassword.Equals(tempUser.UserPassword)).FirstOrDefault();

                if (admin != null)
                {
                    Session["AdminId"] = admin.AdminID;
                    return RedirectToAction("AdminHome");
                }
                else if (user != null)
                {
                    Session["UserId"] = user.UserID;
                    return RedirectToAction("Index", "Home");
                }
                else
                    return Content("Sign In Failed");
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Users");
        }

        public ActionResult AdminHome()
        {
            if (Session["AdminId"] != null)
            {
                return View();
            }
            else
                return RedirectToAction("Login", "Users");
        }

        public ActionResult AdminDashboard()
        {
            if (Session["AdminId"] != null)
            {
                var invoice = db.Invoices.ToList();
                if (invoice != null)
                {
                    ViewBag.sum = db.Invoices.Sum(x => x.InvoiceTotal);
                    return View(invoice);
                }
                else
                    return Content("Invoice Empty!");

            }
            else
                return RedirectToAction("Login", "Users");
        }
    }
}