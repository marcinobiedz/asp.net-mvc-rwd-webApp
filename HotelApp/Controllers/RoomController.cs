using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelApp.Controllers
{
    public class RoomController : Controller
    {
        //
        // GET: /Room/

        public ActionResult Index()
        {
            using (var db = new HotelDBEntities())
            {
                List<room> rooms = new List<room>();
                rooms = db.rooms.ToList();
                if (rooms.Count() == 0)
                    return View();
                else
                    return View(rooms);
            }
        }

    }
}
