using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using bookstore.Models;
using System.Runtime.Serialization.Json;
using System.IO;


namespace bookstore.Controllers
{
    public class HomeController : Controller
    {
        BookStoreEntities context = new BookStoreEntities();
        // GET: Home
        public ActionResult Index()
        {



            return View();
        }

        public ActionResult Shipping()
        {
            var user = getCurrentUser();
            ViewBag.Address = user.Address1;



            return View();
        }

        public new ActionResult Profile()
        {
            var user = getCurrentUser();

            try
            {
                ViewBag.Name = user.Name;
                ViewBag.Email = user.Email;
                ViewBag.Phone = user.Phone;
                ViewBag.Family = user.Family;
                ViewBag.Address = user.Address1;
                ViewBag.Password = user.Password;
                return View();
            }
            catch
            {
                return View();
            }



        }


        public ActionResult Login()
        {
            return View();
        }

        public ActionResult getPro()
        {
            var res = context.Table_Products.Select(x => new { x.pkID, x.Name, x.Price, x.Image }).ToList();

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getBasket(string basket)
        {
            string[] items = basket.Split(',');

            int[] pid = Array.ConvertAll(items, s => int.Parse(s));
            //int[] items = int.Parse(basket.Split(','));

            var res = context.Table_Products.Where(x => pid.Contains(x.pkID)).Select(x => new { x.pkID, x.Name, x.Image, x.Price }).ToList();

            Dictionary<int, int> results = new Dictionary<int, int>();
            for (int i = 0; i < pid.Length; i++)
            {
                int currentNumber = pid[i];
                if (results.ContainsKey(currentNumber))
                {
                    results[currentNumber]++;
                }
                else
                {
                    results[currentNumber] = 1;
                }
            }
            string json = JsonConvert.SerializeObject(results);

            return Json(new { res = res, results = json }, JsonRequestBehavior.AllowGet);


            //var res = context.Table_Products.Where(x => pid.Contains(x.pkID)).Select(x => new { x.pkID, x.Name, x.Image, x.Price }).ToList();
            //return Json(res, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult check_login(string phone, string psw)
        {
            //Session.Timeout = 30;

            int state = 0;
            var user = context.Table_Users.Where(x => x.Phone == phone).SingleOrDefault();
            if (user == null)
            {
                Table_Users nu = new Table_Users();
                nu.Phone = phone;
                nu.Password = psw;
                context.Table_Users.Add(nu);
                context.SaveChanges();

                var user2 = context.Table_Users.Where(x => x.Phone == phone).Single();

                var cookieText = Encoding.UTF8.GetBytes(user.pkID.ToString());
                var encryptedValue = Convert.ToBase64String(MachineKey.Protect(cookieText));

                //Session["hashtt@try"] = user2;
                Response.Cookies["hashtt@try"].Value = encryptedValue;
                Response.Cookies["hashtt@try"].Expires = DateTime.Now.AddDays(300);


                state = 1; //user registerd

            }
            else
            {


                if (user.Password == psw)
                {
                    var cookieText = Encoding.UTF8.GetBytes(user.pkID.ToString());
                    var encryptedValue = Convert.ToBase64String(MachineKey.Protect(cookieText));

                    Session["hashtt@try"] = user.pkID;
                    Response.Cookies["hashtt@try"].Value = encryptedValue;
                    Response.Cookies["hashtt@try"].Expires = DateTime.Now.AddDays(700);

                    state = 2;
                }

                else
                {
                    state = 3;
                }
            }

            return Json(state, JsonRequestBehavior.AllowGet);


        }



        public void logout()
        {
            Response.Cookies["hashtt@try"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect("~/Home/index");
            Session.Abandon();
        }

        //[ValidateAntiForgeryToken]
        public dynamic getCurrentUser()
        {

            if (Request.Cookies["hashtt@try"] == null)
            {
                Response.Redirect("~/Home/Login");
                return null;
            }
            else
            {
                if (Request.Cookies["hashtt@try"].Value == null)
                {
                    Response.Redirect("~/Home/Login");
                }
                var bytes = Convert.FromBase64String(Request.Cookies["hashtt@try"].Value);
                var output = MachineKey.Unprotect(bytes);
                string result = Encoding.UTF8.GetString(output);

                int userid = int.Parse(result);

                var user = context.Table_Users.Where(x => x.pkID == userid).SingleOrDefault();



                return user;

                //}

            }
        }
            //[ValidateAntiForgeryToken]
            public void setName()
            {
                if (Request.Cookies["hashtt@try"] == null)
                {
                    Session["userName"] = "خوش آمدید";
                }
                else
                {
                    if (Request.Cookies["hashtt@try"].Value != null)
                    {
                        var bytes = Convert.FromBase64String(Request.Cookies["hashtt@try"].Value);
                        var output = MachineKey.Unprotect(bytes);
                        string result = Encoding.UTF8.GetString(output);

                        int userid = int.Parse(result);

                        var user = context.Table_Users.Where(x => x.pkID == userid).SingleOrDefault();

                        Session["userName"] = user.Name + " " + user.Family;
                    }
                    else
                    {
                        Session["userName"] = "خوش آمدید";

                    }
                }
            }

            
            [ValidateAntiForgeryToken]
            public ActionResult updateUser(string fname, string lname, string phone, string email, string password, string address)
            {
                int state = 0;
                var user = context.Table_Users.Where(x => x.Phone == phone).SingleOrDefault();
                if (user != null)
                {
                    //Table_Users nu = new Table_Users();
                    user.Name = fname;
                    user.Family = lname;
                    user.Phone = phone;
                    user.Password = password;
                    user.Email = email;
                    user.Address1 = address;

                    //context.Table_Users.Add(nu);
                    context.SaveChanges();



                    state = 1; // user updated
                }
                else
                {
                    state = 2;
                }


                //var res = context.Table_Users.Where(x => phone == x.Phone).Select(x => new { x.pkID, x.Name, x.Family, x.Phone, x.Email, x.Password, x.Address1 }).ToList();

                return Json(state, JsonRequestBehavior.AllowGet);

            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult invoice(string basket)
            {
                if (Request.Cookies["hashtt@try"] == null) { return Json(false, JsonRequestBehavior.AllowGet); }
                if (Request.Cookies["hashtt@try"].ToString() == "") { return Json(false, JsonRequestBehavior.AllowGet); }


                int state = 0;
                int factornumber = context.Table_Invoice.Max(x => x.factorNumber);
                factornumber++;
                int price = 0;


                var bytes = Convert.FromBase64String(Request.Cookies["hashtt@try"].Value);
                var output = MachineKey.Unprotect(bytes); //ask mazrouei
                string result = Encoding.UTF8.GetString(output);

                int userid = int.Parse(result);

                string[] basket2 = basket.Split(',');

                int[] basket3 = Array.ConvertAll(basket2, s => int.Parse(s));

                var check = context.Table_Invoice.Where(x => x.fkUserID == userid && x.status == false).FirstOrDefault();
                if (check != null)
                {
                    factornumber = check.factorNumber;
                    goto label;
                }


                List<Table_Invoice> records = new List<Table_Invoice>();


                foreach (int item in basket3)
                {
                    Table_Invoice ni = new Table_Invoice
                    {
                        fkUserID = userid,
                        fkProductID = item,
                        status = false,
                        transID = "0",
                        trackingID = "0",
                        factorNumber = factornumber
                    };
                    records.Add(ni);

                }

                context.Table_Invoice.AddRange(records);
                context.SaveChanges();
            label:
                price = context.View_Invoice.Where(x => x.factorNumber == factornumber).Sum(x => x.Price);

                state = 1;

                return Json(new { state = state, fn = factornumber, price = price }, JsonRequestBehavior.AllowGet);

            }
        }
    }
