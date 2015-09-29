using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelApp.Models;

namespace HotelApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(int type = 0, string text = null)
        {
            if ((type == 0 || type == 1) && text != null)
            {
                Message message = new Message();
                message.type = type;
                message.text = text;
                return View(message);
            }
            else
            {
                return View();
            }
        }
    }
}
