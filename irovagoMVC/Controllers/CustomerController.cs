using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using irovagoMVC.Models;
using System.Net.Http;

namespace irovagoMVC.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer


        public ActionResult CheckTravelRisks()
        {
            return View();
        }
        public ActionResult CustomerHomePage()
        {
            IEnumerable<OfferMVCModel> offerList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Offers").Result;
            offerList = response.Content.ReadAsAsync <IEnumerable<OfferMVCModel>>().Result;
            var customerId = (int)Session["CustomerId"];
            createLog("Customer", customerId, "GET", "Offers", 0);
            IEnumerable<FavoriteOffersMVCModel> favoritedOffers;
            HttpResponseMessage responseFavorited = GlobalVariables.webApiClient.GetAsync("FavoriteOffers").Result;
            favoritedOffers = responseFavorited.Content.ReadAsAsync<IEnumerable<FavoriteOffersMVCModel>>().Result;
            createLog("Customer", customerId, "GET", "FavoriteOffers", 0);
            var customerFavorites = favoritedOffers.Where(x => x.customerID == customerId).ToList();
            customerFavorites = getOffers(customerFavorites);
            offerList = getHotelsAndAgency(offerList, customerFavorites);
            return View(offerList);
        }

        public void createLog(string actor, int actorId, string operation, string relatedTable, int? relatedRecordId)
        {
            LogMVCModel log = new LogMVCModel();
            log.actor = actor;
            log.actorID = actorId;
            log.operation = operation;
            log.relatedTable = relatedTable;
            log.relatedRecordID = relatedRecordId;
            HttpResponseMessage responseLog = GlobalVariables.webApiClient.PostAsJsonAsync("Logs", log).Result;
        }

        [HttpPost]
        public ActionResult CustomerHomePage(int? id)
        {
            IEnumerable<FavoriteOffersMVCModel> favoritedOffers;
            HttpResponseMessage responseFavorited = GlobalVariables.webApiClient.GetAsync("FavoriteOffers").Result;
            favoritedOffers = responseFavorited.Content.ReadAsAsync<IEnumerable<FavoriteOffersMVCModel>>().Result;
            var customerId = (int)Session["CustomerId"];
            createLog("Customer", customerId, "GET", "FavoriteOffers", 0);
            var customerFavorites = favoritedOffers.Where(x => x.customerID == customerId).ToList();

            var favoriteOfferId = getFavoriteOfferId(id, customerFavorites);
                if (favoriteOfferId > 0) { //unfav

                    HttpResponseMessage responseDelete = GlobalVariables.webApiClient.DeleteAsync("FavoriteOffers/" + favoriteOfferId).Result;
                    createLog("Customer", customerId, "DELETE", "FavoriteOffers", favoriteOfferId);
            }
                else //newfav
                {
                    FavoriteOffersMVCModel newFav = new FavoriteOffersMVCModel();
                    newFav.offerID = id;
                    newFav.customerID = customerId;
                    HttpResponseMessage responseAdd = GlobalVariables.webApiClient.PostAsJsonAsync("FavoriteOffers",newFav).Result;
                    HttpResponseMessage responseFavoritedNew = GlobalVariables.webApiClient.GetAsync("FavoriteOffers").Result;
                    favoritedOffers = responseFavoritedNew.Content.ReadAsAsync<IEnumerable<FavoriteOffersMVCModel>>().Result;
                    FavoriteOffersMVCModel insertedFav = favoritedOffers.Last();

                    createLog("Customer", customerId, "INSERT", "FavoriteOffers", insertedFav.favoriteOfferID);
            }
            return RedirectToAction("CustomerHomePage", "Customer");
        }

        public int getFavoriteOfferId(int? offerId, List<FavoriteOffersMVCModel> favoriteOffers)
        {
            foreach (FavoriteOffersMVCModel favorite in favoriteOffers)
            {
                if (favorite.offerID == offerId)
                {
                    return favorite.favoriteOfferID;
                }
            }
            return 0;
        }

        public bool setFavoritedOffers (int? offerId, List<FavoriteOffersMVCModel> favoriteOffers)
        {
            foreach (FavoriteOffersMVCModel favorite in favoriteOffers)
            {
                if (favorite.offerID == offerId)
                {
                    return true;
                }
            }
            return false;
        }

        public int getFavoriteCount(int customerId)
        {
            IEnumerable<FavoriteOffersMVCModel> favoriteOffers;
            HttpResponseMessage responseFavOffers = GlobalVariables.webApiClient.GetAsync("FavoriteOffers").Result;
            favoriteOffers = responseFavOffers.Content.ReadAsAsync<IEnumerable<FavoriteOffersMVCModel>>().Result;
            createLog("Customer", customerId, "GET", "FavoriteOffers", 0);
            int count = 0;

            foreach (FavoriteOffersMVCModel favorite in favoriteOffers)
            {
                if (favorite.customerID == customerId)
                {
                    count++;
                }
            }
            return count;
        }

        public IEnumerable<OfferMVCModel> getHotelsAndAgency(IEnumerable<OfferMVCModel> offers, List<FavoriteOffersMVCModel> favoriteOffers)
        {
            int customerId = (int)Session["CustomerId"];


            foreach (var offer in offers)
            {
                HotelMVCModel hotel;
                HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Hotels/" + offer.hotelID).Result;
                hotel = response.Content.ReadAsAsync<HotelMVCModel>().Result;
                
                createLog("Customer", customerId, "GET", "Hotels", 0);
                offer.Hotel = hotel;
                AgencyMVCModel agency;
                HttpResponseMessage responseAgency = GlobalVariables.webApiClient.GetAsync("Agencies/" + offer.agencyID).Result;
                agency = responseAgency.Content.ReadAsAsync<AgencyMVCModel>().Result;
                createLog("Customer", customerId, "GET", "Agency", 0);
                offer.Agency = agency;
                offer.isFavorited = setFavoritedOffers(offer.offerID, favoriteOffers);
            }

            return offers;
        }

        public ActionResult CustomerProfile()
        {
            CustomerMVCModel customer;
            int customerID = (int)Session["CustomerId"];
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Customers/" + customerID).Result;
            createLog("Customer", customerID, "GET", "Customer", 0);
            customer = response.Content.ReadAsAsync<CustomerMVCModel>().Result;
            customer.favoritedOfferCount = getFavoriteCount(customerID);
            return View(customer); 
        }

        [HttpPost]
        public ActionResult CustomerProfile(CustomerMVCModel customer)
        {
            int customerID = (int)Session["CustomerId"];
            customer.customerID = customerID;
            HttpResponseMessage response = GlobalVariables.webApiClient.PutAsJsonAsync("Customers/" + customerID, customer).Result;
            customer.favoritedOfferCount = getFavoriteCount(customerID);
            createLog("Customer", customerID, "UPDATE", "Customer", customerID);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Your information updated successfully";
            }
            else
            {
                TempData["SuccessMessage"] = "Information update operation failed";
            }
            return View(customer);
        }

        public ActionResult FavoritedOffers()
        {
            int customerID = (int)Session["CustomerId"];
            IEnumerable<FavoriteOffersMVCModel> favoritedOffers;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("FavoriteOffers").Result;
            favoritedOffers = response.Content.ReadAsAsync<IEnumerable<FavoriteOffersMVCModel>>().Result;
            createLog("Customer", customerID, "GET", "FavoriteOffers", 0);
            var customerId = (int)Session["CustomerId"];
            var customerFavorites = favoritedOffers.Where(x => x.customerID == customerId).ToList();
            customerFavorites = getOffers(customerFavorites);
            return View(customerFavorites);
        }

        [HttpPost]
        public ActionResult FavoritedOffers(int? id)
        {
            int customerID = (int)Session["CustomerId"];
            HttpResponseMessage responseDelete = GlobalVariables.webApiClient.DeleteAsync("FavoriteOffers/"+ id).Result;
            createLog("Customer", customerID, "DELETE", "FavoriteOffers", id);
            return RedirectToAction("FavoritedOffers","Customer");
        }

        public List<FavoriteOffersMVCModel> getOffers(List<FavoriteOffersMVCModel> customerFavorites)
        {
            int customerID = (int)Session["CustomerId"];

            foreach (FavoriteOffersMVCModel favorite in customerFavorites)
            {
                HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Offers/"+favorite.offerID).Result;
                favorite.Offer = response.Content.ReadAsAsync<OfferMVCModel>().Result;
                createLog("Customer", customerID, "GET", "Offers", 0);
                HttpResponseMessage responseHotel = GlobalVariables.webApiClient.GetAsync("Hotels/" + favorite.Offer.hotelID).Result;
                favorite.Offer.Hotel = responseHotel.Content.ReadAsAsync<HotelMVCModel>().Result;
                createLog("Customer", customerID, "GET", "Hotels", 0);
                HttpResponseMessage responseAgency = GlobalVariables.webApiClient.GetAsync("Agencies/" + favorite.Offer.agencyID).Result;
                favorite.Offer.Agency = responseAgency.Content.ReadAsAsync<AgencyMVCModel>().Result;
                createLog("Customer", customerID, "GET", "Agenices", 0);
            }
            return customerFavorites;

        }
    }
}