using HotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelApp.Controllers
{
    public class ReservationController : Controller
    {
        //
        // GET: /Reservation/
        public ActionResult Index(int id = 0, int type = 0, string text = null)
        {
            Message info = new Message();
            info.id = id;
            info.type = type;
            info.text = text;
            info.idStr = Convert.ToString(id);
            return View(info);
        }

        public ActionResult User(Message info)
        {
            if ((user)Session["user"] != null)
            {
                if (info.text == null)
                    return View();
                else
                    return View(info);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Delete(int id)
        {
            Message news = new Message();
            if ((user)Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                using (var db = new HotelDBEntities())
                {
                    try
                    {
                        reservation toDel = db.reservations.Find(id);
                        db.reservations.Remove(toDel);
                        db.SaveChanges();
                        news.type = 1;
                        news.text = "Your reservation has been deleted";
                        return View("User", news);
                    }
                    catch (Exception e)
                    {
                        news.type = 0;
                        news.text = "Unexpected database problem.";
                        return View("User", news);
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Book(string bookStart, string bookEnd, int bookRoom = 0)
        {
            var user = (user)Session["user"];
            Message info = new Message();
            if (user == null)
            {
                return RedirectToAction("Index", "Reservation");
            }
            if (String.IsNullOrEmpty(bookStart) || String.IsNullOrEmpty(bookEnd) || bookRoom == 0)
            {
                info.text = "You have to choose room number and both dates.";
                return RedirectToAction("Index", "Reservation", new { info.text });
            }
            DateTime startDate = Convert.ToDateTime(bookStart);
            DateTime endDate = Convert.ToDateTime(bookEnd);
            if (startDate >= endDate)
            {
                info.text = "Start day must be earlier ther end day.";
                return RedirectToAction("Index", "Reservation", new { info.text });
            }
            List<DateTime> busyDates = new List<DateTime>();
            using (var db = new HotelDBEntities())
            {
                var reservs = db.reservations.ToList();
                if (reservs.Count() == 0)
                {
                    reservation res = new reservation();
                    res.start_date = startDate;
                    res.end_date = endDate;
                    res.room_id = bookRoom;
                    res.user_id = user.id;
                    res.days = Convert.ToInt32((endDate - startDate).TotalDays);
                    db.reservations.Add(res);
                    db.SaveChanges();
                    //----------------------------------------------------------------------------
                    info.type = 1;
                    info.text = "Reservations has been added.";
                    return RedirectToAction("Index", "Reservation", new { info.type, info.text });
                }
                else
                {
                    foreach (reservation item in reservs)
                    {
                        busyDates.Add(item.start_date);
                        DateTime a = item.start_date;
                        DateTime b = item.end_date;
                        while (a.AddDays(1) != b)
                        {
                            busyDates.Add(a.AddDays(1));
                            a = a.AddDays(1);
                        }
                    }
                    if (busyDates.Contains(startDate) || busyDates.Contains(endDate.AddDays(-1)))
                    {
                        info.type = 0;
                        info.text = "Room is reserved in those days.";
                        return RedirectToAction("Index", "Reservation", new { info.type, info.text });
                    }
                    else
                    {
                        reservation res = new reservation();
                        res.start_date = startDate;
                        res.end_date = endDate;
                        res.room_id = bookRoom;
                        res.user_id = user.id;
                        res.days = Convert.ToInt32((endDate - startDate).TotalDays);
                        db.reservations.Add(res);
                        db.SaveChanges();
                        //----------------------------------------------------------------------------
                        info.type = 1;
                        info.text = "Reservations has been added.";
                        return RedirectToAction("Index", "Reservation", new { info.type, info.text });
                    }
                }
            }
        }
    }
}
