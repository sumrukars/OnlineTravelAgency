using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using irovagoMVC.Models;
using System.Net.Http;

namespace irovagoMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<OfferMVCModel> offerList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Offers").Result;
            offerList = response.Content.ReadAsAsync<IEnumerable<OfferMVCModel>>().Result;
            offerList = getHotelsAndAgency(offerList);
            return View(offerList);
        }

        public IEnumerable<OfferMVCModel> getHotelsAndAgency(IEnumerable<OfferMVCModel> offers)
        {

            foreach (var offer in offers)
            {
                HotelMVCModel hotel;
                HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Hotels/" + offer.hotelID).Result;
                hotel = response.Content.ReadAsAsync<HotelMVCModel>().Result;
                offer.Hotel = hotel;
                AgencyMVCModel agency;
                HttpResponseMessage responseAgency = GlobalVariables.webApiClient.GetAsync("Agencies/" + offer.agencyID).Result;
                agency = responseAgency.Content.ReadAsAsync<AgencyMVCModel>().Result;
                offer.Agency = agency;

            }

            return offers;
        }

        public ActionResult Login(int id = 0)
        {

            return View(new LoginMVCModel());
        }

        [HttpPost]
        public ActionResult Login(LoginMVCModel log)
        {
        
            IEnumerable<LoginMVCModel> loginTable;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Logins").Result;
            loginTable = response.Content.ReadAsAsync<IEnumerable<LoginMVCModel>>().Result;
      
            foreach (LoginMVCModel login in loginTable)
            {
                if (login.email== log.email && login.password == log.password)
                {
                    if (login.customerID != null)
                    {
                        Session["CustomerId"] = login.customerID;
                        return RedirectToAction("CustomerHomePage", "Customer");
                    }
                    else if (login.agencyID != null)
                    {
                        Session["AgencyId"] = login.agencyID;
                        return RedirectToAction("AgencyHomePage", "Agency");
                       
                    } else if (login.adminID != null)
                    {
                        Session["AdminId"] = login.adminID;
                        return RedirectToAction("AdminHomePage", "Admin");
                    }
                   
                }
            }
            return View();
        }

        public CustomerMVCModel getCustomer(int? customerID)
        {
            IEnumerable<CustomerMVCModel> customerTable;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Customers").Result;
            customerTable = response.Content.ReadAsAsync<IEnumerable<CustomerMVCModel>>().Result;
            foreach (CustomerMVCModel customer in customerTable)
            {
                if (customer.customerID == customerID)
                {
                    return customer;
                }
            }

            return null;
        }
        public ActionResult Register(int id = 0)
        {

            return View(new LoginMVCModel());
        }

        [HttpPost]
        public ActionResult Register(LoginMVCModel reg)
        {
            reg.email = reg.Customer.email;
            HttpResponseMessage response = GlobalVariables.webApiClient.PostAsJsonAsync("Logins", reg).Result;
            return View();
        }

 


    }
}