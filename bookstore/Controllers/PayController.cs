using bookstore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;

namespace bookstore.Controllers
{
    public class PayController : Controller
    {
        // GET: Pay
        BookStoreEntities context = new BookStoreEntities();
        private string GatewaySend = "https://panel.aqayepardakht.ir/api/create";
        private string GatewayResult = "https://panel.aqayepardakht.ir/api/verify";

        private string pin = "aqayepardakht";


        private new string Redirect = "https://localhost:44302/pay/CallBack";


        [HttpPost]
        [ActionName("submit")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> SubmitAsync(string fn, string price)
        {


            try
            {



                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "pin", pin },
                        { "amount", price },
                        { "callback", Redirect },
                        { "invoice_id", fn }



                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(GatewaySend, content);
                    dynamic responseString = await response.Content.ReadAsStringAsync();

                    //Controller account = JsonConvert.DeserializeObject<Controller>(responseString);

                    //Console.WriteLine(account.transid);

                    //responseString = JsonSerializer.Deserialize<responseString>(jsonString);

                    //Console.WriteLine($"Date: {responseString?.status}");
                    //Console.WriteLine($"TemperatureCelsius: {responseString?.transid}");


                    //string status = responseString.status;
                    //string trid = responseString.transid;

                    //dynamic stuff = JObject.Parse("responseString");
                    //string status= stuff["status"];
                    //string transid= stuff["transid"];


                    if (responseString.Length > 3)
                    {
                        int fnn = int.Parse(fn);
                        var items = context.Table_Invoice.Where(x => x.factorNumber == fnn).ToList();

                        foreach (var d in items)
                        {
                            d.transID = responseString;
                        }
                        context.SaveChanges();




                        return Json(new { message = "https://panel.aqayepardakht.ir/startpay/" + responseString, success = true, JsonRequestBehavior.AllowGet });

                    }
                    else
                    {
                        //return View();
                        int error = int.Parse(responseString);
                        return Json(new { error = error, success = false, JsonRequestBehavior.AllowGet });
                    }
                }






            }
            catch (Exception ex)
            {
                //return View();
                return Json(new { message = ex.Message, success = true, JsonRequestBehavior.AllowGet });
            }
            //return View();
        }

        //string Date2Persian(DateTime date)
        //{
        //    PersianCalendar p = new PersianCalendar();
        //    return $"{p.GetYear(date)}/{p.GetMonth(date):D2}/{p.GetDayOfMonth(date):D2} - {date.Hour:D2}:{date.Minute:D2}";
        //}

        //[HttpPost]
        
        [AllowAnonymous]
        [ActionName("CallBack")]
        public async System.Threading.Tasks.Task<ActionResult> CallBackAsync()
        {
            if (!string.IsNullOrEmpty(Request.Form["transid"]))
            {
                string tid = Request.Form["transID"].ToString();

                var items = context.View_Invoice.Where(x => x.transID == tid).ToList();

                int a = items.Sum(x => x.Price);

                string amount = a.ToString();

                try
                {
                    using (var client = new HttpClient())
                    {
                        var values = new Dictionary<string, string>
                        {
                            { "pin", pin },
                            { "amount", amount },

                            { "transid", Request.Form["transid"].ToString() },
                        };











                        var content = new FormUrlEncodedContent(values);

                        var response = await client.PostAsync(GatewayResult, content);
                        var responseString = await response.Content.ReadAsStringAsync();

                        if (responseString == "1")
                        {




                            try
                            {



                                var items2 = context.Table_Invoice.Where(x => x.transID == tid).ToList();

                                foreach (var p in items2)
                                {

                                    p.status = true;
                                    p.trackingID = Request.Form["tracking_number"].ToString();

                                }
                                context.SaveChanges();
                                ViewBag.Success = "1";
                                ViewBag.message = "تراکنش با موفقیت انجام شد || شماره تراکنش: " + Request.Form["tracking_number"].ToString();

                            }
                            catch (Exception ex)
                            {
                                ViewBag.Success = "0"; //aya dorost ast?
                                ViewBag.message = ex.Message;

                            }
                        }
                        else
                        {

                            ViewBag.Success = "0";
                            ViewBag.message = "پرداخت ناموفق";


                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Success = "0";
                    ViewBag.message = ex.Message;


                }
            }
            else
            {
                ViewBag.Success = "0";
                ViewBag.message = new string[] { "پرداخت با موفقیت انجام نشد" };


            }
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error(string a)
        {
            ViewBag.Message = a;
            return View();
        }




        public ActionResult Invoice(string invoiceCode)
        {
            if (string.IsNullOrEmpty(invoiceCode))
                ViewBag.ShowImage = "0";
            else
            {
                ViewBag.ShowImage = "1";
                ViewBag.InvoiceCode = invoiceCode.Trim();
            }
            return View();
        }
        [HttpPost]
        public ActionResult SearchFile(string invoiceCode)
        {
            if (!Request.IsAjaxRequest())
                return Json(false);
            var relativePath = "~/UploadedImages/" + invoiceCode + ".jpg";
            var absolutePath = HttpContext.Server.MapPath(relativePath);
            return Json(System.IO.File.Exists(absolutePath));
        }
    }
}