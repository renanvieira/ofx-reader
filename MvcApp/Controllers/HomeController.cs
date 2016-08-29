using MvcApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewData["Menu"] = "Home";
            return View("Index");
        }

        public ActionResult View()
        {
            try
            {
                ViewData["Menu"] = "View";

                using (DatabaseEntities efModel = new DatabaseEntities())
                {
                    var items = efModel.OFXTransactions.OrderBy(x => x.date).ToList();

                    return View("View", items);                   
                }

            }
            catch (Exception ex)
            {
                ViewData["Exception"] = ex.ToString();
                return null;
            }
        }

    }
}
