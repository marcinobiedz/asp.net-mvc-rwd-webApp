using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelApp.Models
{
    public class Countries
    {
        public IList<SelectListItem> countryList = new List<SelectListItem>();
        public Countries() {
            SelectListItem africa = new SelectListItem();
            africa.Text = "Africa";
            africa.Value = "Africa";
            SelectListItem europe = new SelectListItem();
            europe.Text = "Europe";
            europe.Value = "Europe";
            SelectListItem northAmerica = new SelectListItem();
            northAmerica.Text = "North America";
            northAmerica.Value = "North America";
            SelectListItem southAmerica = new SelectListItem();
            southAmerica.Text = "South America";
            southAmerica.Value = "South America";
            SelectListItem australia = new SelectListItem();
            australia.Text = "Australia";
            australia.Value = "Australia";
            SelectListItem asia = new SelectListItem();
            asia.Text = "Asia";
            asia.Value = "Asia";
            SelectListItem middleEast = new SelectListItem();
            middleEast.Text = "Middle East";
            middleEast.Value = "Middle East";
            countryList.Add(africa);
            countryList.Add(asia);
            countryList.Add(australia);
            countryList.Add(europe);
            countryList.Add(middleEast);
            countryList.Add(northAmerica);
            countryList.Add(southAmerica);
        }
    }
}