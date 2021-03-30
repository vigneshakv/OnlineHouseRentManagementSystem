using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineHouseRentManagementSystem.EmailSettings;
using OnlineHouseRentManagementSystem.Models;

namespace OnlineHouseRentManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private HouseRentManagementEntities db = new HouseRentManagementEntities();

        // GET: Users
        
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,user_email,user_role,user_password")] user user)
        {
            if (ModelState.IsValid)
            {
                var count = db.users.Count(u => u.user_email == user.user_email);
                if (count == 0)
                {
                    db.users.Add(user);
                    if (user.user_role == "seller")
                    {
                        seller seller = new seller
                        {
                            seller_email = (string)Session["email"],
                            seller_name = (string)Session["name"],
                            seller_phone = (string)Session["phone"],
                            seller_state = (string)Session["state"]
                        };
                        db.sellers.Add(seller);
                    }
                    else
                    {
                        customer customer = new customer
                        {
                            customer_email = (string)Session["email"],
                            customer_name = (string)Session["name"],
                            customer_phone = (string)Session["phone"],
                            customer_state = (string)Session["state"]
                        };
                        db.customers.Add(customer);
                        string body = "Welcome To Online HouseRentManagementSystem";
                        string sendemail = ConfigurationManager.AppSettings["SendEmail"];
                        if (sendemail.ToLower() == "true")
                        {
                            SendEmail.Send(customer.customer_email, body);
                        } 
                    }
                    db.SaveChanges();
                    Session["message"] = "";
                    return RedirectToAction("SignupLogin", "Home");
                }
                else
                {
                    Session["message"] = "User email already exist..!";
                    return RedirectToAction("SignupLogin", "Home");
                }
               
            }

            return View(user);
        }

        // GET: Users/Edit/abs@gmail.com
        public ActionResult Edit(string user_email = null)
        {
            if (user_email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                user user = db.users.FirstOrDefault(p => p.user_email == user_email);
                if (user == null)
                {
                    Session["message"] = "Email not found try to sign up..!";
                    return RedirectToAction("SignupLogin", "Home");
                }
                return View(user);
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,user_password")] user user)
        {
            try
            {
                var Tempuser = db.users.Single(p => p.user_id == user.user_id);
                Tempuser.user_password = user.user_password;
                db.SaveChanges();
                return RedirectToAction("SignupLogin", "Home");
            }
            catch(Exception e)
            {
                var msg = e.Message;
                return RedirectToAction("SignupLogin", "Home");
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UserValidate()
        {
            return HttpNotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserValidate(string useremail,string userpassword,FormCollection collection)
        {
            try { 
                var user = db.users.FirstOrDefault(data => data.user_email == useremail && data.user_password == userpassword);
                if (user != null && user.user_email != null)
                {
                    if (collection["rememberme"] != null)
                    {
                        Response.Cookies.Clear();
                        Response.Cookies["userid"].Value = useremail;
                        Response.Cookies["userpassword"].Value = userpassword;
                        Response.Cookies["userid"].Expires = DateTime.Now.AddDays(10);
                        Response.Cookies["userpassword"].Expires = DateTime.Now.AddDays(10);
                    }
                    else
                    {
                        Response.Cookies["userid"].Value = "";
                        Response.Cookies["userpassword"].Value = "";
                    }
                    Session["user_role"] = user.user_role;
                    
                    if (user.user_role == "seller")
                    {
                        var seller = db.sellers.FirstOrDefault(item => item.seller_email == user.user_email);
                        Session["seller_id"] = seller.seller_id;
                        Session["seller_name"] = seller.seller_name;
                        Session["name"] = seller.seller_name;
                        return RedirectToAction("Index", "Prop");
                    }
                    else
                    {
                        var customer = db.customers.FirstOrDefault(item => item.customer_email == user.user_email);
                        Session["customer_id"] = customer.customer_id;
                        Session["cutomer_name"] = customer.customer_name;
                        Session["name"] = customer.customer_name;
                        return RedirectToAction("CustomerIndex", "Prop",new { s = customer.customer_state});
                    }
                }
                else
                {
                    ViewData.Clear();
                    ViewData["error"] = "Invalid User Name or Password";
                    return View("~/Views/Home/SignupLogin.cshtml");
                }
            }
            catch(Exception e)
            {
                return Content(e.Message);
                return RedirectToAction("Index","Home");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
