using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using bookstore.Models;

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

        public ActionResult getPro()
        {
            var res = context.Table_Products.Select(x => new { x.pkID, x.Name, x.Price, x.Image }).ToList();

            return Json(res, JsonRequestBehavior.AllowGet);
        }


    }
}