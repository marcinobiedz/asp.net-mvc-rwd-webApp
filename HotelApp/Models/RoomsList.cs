using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelApp.Models
{
    public class RoomsList
    {
        public List<SelectListItem> rooms = new List<SelectListItem>();
        public RoomsList()
        {
            using (var db = new HotelDBEntities())
            {
                var prerooms = db.rooms.ToList();
                if (prerooms.Count() > 0) 
                {
                    foreach (room item in prerooms) 
                    {
                        SelectListItem selectItem = new SelectListItem();
                        selectItem.Value = Convert.ToString(item.id);
                        selectItem.Text = "Room #"+item.number+", Storey: "+item.floor+", ";
                        selectItem.Text += "Number of places: "+item.places;
                        rooms.Add(selectItem);
                    }
                }
            }
        }
    }
}