using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using irovagoMVC.Models;
using System.Net.Http;
using System.Web.Routing;
using System.Threading;

namespace irovagoMVC.Controllers
{
    public class AgencyController : Controller
    {
        // GET: Agency
        public ActionResult AgencyHomePage()
        {
            int agencyId = (int)Session["AgencyId"];
            IEnumerable<OfferMVCModel> offerList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Offers").Result;
            offerList = response.Content.ReadAsAsync<IEnumerable<OfferMVCModel>>().Result;
            offerList = getHotelsAndAgency(offerList);
            createLog("Agency", agencyId, "GET", "Offers", 0);
            return View(offerList);
        }

        public ActionResult LogOut()
        {
            Session["AgencyId"] = null;
            return RedirectToAction("Index", "Home");
        }

        public void createLog(string actor, int? actorId, string operation, string relatedTable, int? relatedRecordId)
        {
            LogMVCModel log = new LogMVCModel();
            log.actor = actor;
            log.actorID = actorId;
            log.operation = operation;
            log.relatedTable = relatedTable;
            log.relatedRecordID = relatedRecordId;
            HttpResponseMessage responseLog = GlobalVariables.webApiClient.PostAsJsonAsync("Logs", log).Result;
        }

        public IEnumerable<OfferMVCModel> getHotelsAndAgency(IEnumerable<OfferMVCModel> offers)
        {
            int agencyId = (int)Session["AgencyId"];
            foreach (var offer in offers)
            {
                HotelMVCModel hotel;
                HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Hotels/" + offer.hotelID).Result;
                hotel = response.Content.ReadAsAsync<HotelMVCModel>().Result;
                createLog("Agency", agencyId, "GET", "Hotels", 0);
                offer.Hotel = hotel;

                RoomTypeMVCModel room;
                HttpResponseMessage responseRoom = GlobalVariables.webApiClient.GetAsync("RoomTypes/" + offer.roomTypeID).Result;
                room = responseRoom.Content.ReadAsAsync<RoomTypeMVCModel>().Result;
                offer.RoomType = room;
                offer.RoomType.displayName = room.name + " " + room.type;

                AgencyMVCModel agency;
                HttpResponseMessage responseAgency = GlobalVariables.webApiClient.GetAsync("Agencies/" + offer.agencyID).Result;
                agency = responseAgency.Content.ReadAsAsync<AgencyMVCModel>().Result;
                createLog("Agency", agencyId, "GET", "Agencies", 0);
                offer.Agency = agency;

            }

            return offers;
        }

        public ActionResult AgencyProfile()
        {
            AgencyMVCModel agency;
            int agencyId = (int)Session["AgencyId"];

            
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Agencies/"+agencyId).Result;
            agency = response.Content.ReadAsAsync<AgencyMVCModel>().Result;
            createLog("Agency", agencyId, "GET", "Agencies", 0);
            agency.offerCount = getAgencyOfferCount(agencyId);
            return View(agency);
        }

        public int getAgencyOfferCount(int agencyId)
        {
            IEnumerable<OfferMVCModel> offerList;
            HttpResponseMessage responseOffer = GlobalVariables.webApiClient.GetAsync("Offers").Result;
            offerList = responseOffer.Content.ReadAsAsync<IEnumerable<OfferMVCModel>>().Result;
            createLog("Agency", agencyId, "GET", "Offers", 0);
            var agencyOfferList = offerList.Where(x => x.agencyID == agencyId).ToList();

            return agencyOfferList.Count();
        }

        [HttpPost]
        public ActionResult AgencyProfile(AgencyMVCModel agency)
        {
           
            int agencyId = (int)Session["AgencyId"];
            agency.agencyID = agencyId;
            agency.offerCount = getAgencyOfferCount(agencyId);
            HttpResponseMessage response = GlobalVariables.webApiClient.PutAsJsonAsync("Agencies/" + agencyId, agency).Result;
            createLog("Agency", agencyId, "UPDATE", "Agencies", agencyId);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Your information updated successfully";
            } else
            {
                TempData["SuccessMessage"] = "Information update operation failed";
            }
            return View(agency);
        }

        [HttpGet]
        public ActionResult AddOffer(bool refresh = false)
        {
            if (Session["NewOffer"] != null && refresh == true)
            {
                AddOfferMVCModel newOffer = (AddOfferMVCModel)Session["NewOffer"];
                return View(newOffer);
            }
            int agencyId = (int)Session["AgencyId"];
            IEnumerable<HotelMVCModel> hotelList;
            HttpResponseMessage responseHotelList = GlobalVariables.webApiClient.GetAsync("Hotels").Result;
            hotelList = responseHotelList.Content.ReadAsAsync<IEnumerable<HotelMVCModel>>().Result;
            createLog("Agency", agencyId, "GET", "Hotels", 0);
            AddOfferMVCModel model = new AddOfferMVCModel();
            model.offer = new OfferMVCModel();
            model.hotelList = hotelList;
            IEnumerable<HotelRoomTypeMVCModel> allHotelRoomList;
            HttpResponseMessage responseHotelRoomList = GlobalVariables.webApiClient.GetAsync("HotelRoomTypes").Result;
            allHotelRoomList = responseHotelRoomList.Content.ReadAsAsync<IEnumerable<HotelRoomTypeMVCModel>>().Result;
            model.roomList = getRoomTypeList(allHotelRoomList.ToList());
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteOffer(int? id)
        {
            int agencyId = (int)Session["AgencyId"];
            IEnumerable<FavoriteOffersMVCModel> favoriteofferList;
            HttpResponseMessage responseFavoriteOffer = GlobalVariables.webApiClient.GetAsync("FavoriteOffers").Result;
            favoriteofferList = responseFavoriteOffer.Content.ReadAsAsync<IEnumerable<FavoriteOffersMVCModel>>().Result;
            createLog("Agency", agencyId, "GET", "FavoriteOffers", 0);
            var favoriteOfferListToDelete = favoriteofferList.Where(x => x.offerID == id).ToList();

            foreach (FavoriteOffersMVCModel  favorite in favoriteOfferListToDelete)
            {
                HttpResponseMessage responsefavoriteOfferDelete = GlobalVariables.webApiClient.DeleteAsync("FavoriteOffers/" + favorite.favoriteOfferID).Result;
                createLog("Agency", agencyId, "DELETE", "FavoriteOffers", favorite.favoriteOfferID);
            }

            HttpResponseMessage responseDeleteOffer = GlobalVariables.webApiClient.DeleteAsync("Offers/"+id).Result;
            createLog("Agency", agencyId, "DELETE", "Offer", id);

            return RedirectToAction("AgencyOffers", "Agency");
        }


        [HttpPost]
        public ActionResult AddOffer(int? id)
        {
            int agencyId = (int)Session["AgencyId"];
            OfferMVCModel offer;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Offers/"+id).Result;
            offer = response.Content.ReadAsAsync<OfferMVCModel>().Result;
            offer.Agency = getAgency(offer.agencyID);
            offer.RoomType = getRoomType(offer.roomTypeID);
            createLog("Agency", agencyId, "GET", "Offers", 0);

            HotelMVCModel hotel;
            HttpResponseMessage responseHotel = GlobalVariables.webApiClient.GetAsync("Hotels/" + offer.hotelID).Result;
            hotel = responseHotel.Content.ReadAsAsync<HotelMVCModel>().Result;
            createLog("Agency", agencyId, "GET", "Hotels", 0);
            offer.Hotel = hotel;
            IEnumerable<HotelMVCModel> hotelList;
            HttpResponseMessage responseHotelList = GlobalVariables.webApiClient.GetAsync("Hotels").Result;
            hotelList = responseHotelList.Content.ReadAsAsync<IEnumerable<HotelMVCModel>>().Result;
            createLog("Agency", agencyId, "GET", "Hotels", 0);



            IEnumerable<HotelRoomTypeMVCModel> allHotelRoomList;
            HttpResponseMessage responseHotelRoomList = GlobalVariables.webApiClient.GetAsync("HotelRoomTypes").Result;
            allHotelRoomList = responseHotelRoomList.Content.ReadAsAsync<IEnumerable<HotelRoomTypeMVCModel>>().Result;
            var hotelRoomList = allHotelRoomList.Where(x => x.hotelID == offer.hotelID).ToList();
            createLog("Agency", agencyId, "GET", "HotelRoomTypes", 0);

            AddOfferMVCModel model = new AddOfferMVCModel();
            model.offer = offer;
            model.hotelList = hotelList;
            model.roomList = getRoomTypeList(hotelRoomList);
            return View(model);
        }



        public AgencyMVCModel getAgency(int? agencyId)
        {
            AgencyMVCModel agency;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Agencies/" + agencyId).Result;
            agency = response.Content.ReadAsAsync<AgencyMVCModel>().Result;
            createLog("Agency", agencyId, "GET", "Agencies", 0);
            return agency;
        }

        public RoomTypeMVCModel getRoomType(int? roomType)
        {
            int agencyId = (int)Session["AgencyId"];
            RoomTypeMVCModel room;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("RoomTypes/" + roomType).Result;
            room = response.Content.ReadAsAsync<RoomTypeMVCModel>().Result;
            room.displayName = room.name + room.type;
            createLog("Agency", agencyId, "GET", "RoomTypes", 0);
            return room;
        }

        [HttpGet]
        public ActionResult SubmitDropDown(int? offerId, int? price, int? roomTypeId, int? hotelId)
        {
            int agencyId = (int)Session["AgencyId"];
            AddOfferMVCModel addOfferMVCModel = new AddOfferMVCModel();
            if (offerId == null) // yeni offer eklerken açılacak ekran
            {
                IEnumerable<HotelMVCModel> hotelList;
                HttpResponseMessage responseHotelList = GlobalVariables.webApiClient.GetAsync("Hotels").Result;
                hotelList = responseHotelList.Content.ReadAsAsync<IEnumerable<HotelMVCModel>>().Result;
                createLog("Agency", agencyId, "GET", "Hotels", 0);
                addOfferMVCModel.offer = new OfferMVCModel();
                addOfferMVCModel.offer.price = price;
                addOfferMVCModel.offer.roomTypeID = roomTypeId;
                addOfferMVCModel.offer.hotelID = hotelId;
                addOfferMVCModel.roomList = new List<RoomTypeMVCModel>().AsEnumerable();
                addOfferMVCModel.hotelList = hotelList;
            }
            else // drop downdan selected değişince girecek
            {
                if (offerId != 0)
                {
                    OfferMVCModel offer;
                    HttpResponseMessage responseOffer = GlobalVariables.webApiClient.GetAsync("Offers/" + offerId).Result;
                    offer = responseOffer.Content.ReadAsAsync<OfferMVCModel>().Result;

                    addOfferMVCModel.offer = offer;
                    addOfferMVCModel.offer.price = price;
                    addOfferMVCModel.offer.roomTypeID = roomTypeId;
                    addOfferMVCModel.offer.hotelID = hotelId;

                } else
                {
                    addOfferMVCModel.offer = new OfferMVCModel();
                    addOfferMVCModel.offer.price = price;
                    addOfferMVCModel.offer.roomTypeID = roomTypeId;
                    addOfferMVCModel.offer.hotelID = hotelId;
                }

                HotelMVCModel hotel;
                HttpResponseMessage responseHotel = GlobalVariables.webApiClient.GetAsync("Hotels/" + addOfferMVCModel.offer.hotelID).Result;
                hotel = responseHotel.Content.ReadAsAsync<HotelMVCModel>().Result;
                createLog("Agency", agencyId, "GET", "Hotels", 0);


                IEnumerable<HotelMVCModel> hotelList;
                HttpResponseMessage responseHotelList = GlobalVariables.webApiClient.GetAsync("Hotels").Result;
                hotelList = responseHotelList.Content.ReadAsAsync<IEnumerable<HotelMVCModel>>().Result;
                createLog("Agency", agencyId, "GET", "Hotels", 0);



                IEnumerable<HotelRoomTypeMVCModel> allHotelRoomList;
                HttpResponseMessage responseHotelRoomList = GlobalVariables.webApiClient.GetAsync("HotelRoomTypes").Result;
                allHotelRoomList = responseHotelRoomList.Content.ReadAsAsync<IEnumerable<HotelRoomTypeMVCModel>>().Result;
                var hotelRoomList = allHotelRoomList.Where(x => x.hotelID == addOfferMVCModel.offer.hotelID).ToList();
                createLog("Agency", agencyId, "GET", "HotelRoomTypes", 0);


                addOfferMVCModel.hotelList = hotelList;
                addOfferMVCModel.roomList = getRoomTypeList(hotelRoomList);
                addOfferMVCModel.offer.Hotel = hotel;
                ModelState.Clear();
            }
            Session["NewOffer"] = addOfferMVCModel;
            return RedirectToAction("AddOffer","Agency", new { refresh = true});

        }


        [HttpPost]
        public ActionResult Submit(AddOfferMVCModel addOfferMVCModel)
        {
            int agencyId = (int)Session["AgencyId"];
            if (addOfferMVCModel.offer.offerID == 0) //yeni ekleme
            {
                OfferMVCModel offer = new OfferMVCModel();
                offer = addOfferMVCModel.offer;
                offer.agencyID = (int)Session["AgencyId"];

                OfferMVCModel offerToSend = new OfferMVCModel();
                offerToSend.agencyID = offer.agencyID;
                offerToSend.price = offer.price;
                offerToSend.hotelID = offer.hotelID;
                offerToSend.roomTypeID = offer.roomTypeID;
                HttpResponseMessage response = GlobalVariables.webApiClient.PostAsJsonAsync("Offers", offerToSend).Result;


                IEnumerable<OfferMVCModel> offerList;
                HttpResponseMessage responseOffer = GlobalVariables.webApiClient.GetAsync("Offers").Result;
                offerList = responseOffer.Content.ReadAsAsync<IEnumerable<OfferMVCModel>>().Result;
                OfferMVCModel offerNew = offerList.Last();

                createLog("Agency", agencyId, "INSERT", "HotelRoomTypes", offerNew.offerID);
                Thread.Sleep(100);
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.InternalServerError )
                {   
                    TempData["SuccessMessage"] = "Your information added successfully";
                    addOfferMVCModel = getHotelAndRoomList(addOfferMVCModel);
                }
                else
                {
                    TempData["SuccessMessage"] = "Operation failed";
                }
               
            } else //güncelleme
            {
                HttpResponseMessage response = GlobalVariables.webApiClient.PutAsJsonAsync("Offers/" + addOfferMVCModel.offer.offerID, addOfferMVCModel.offer).Result;
                createLog("Agency", agencyId, "UPDATE", "Offers", addOfferMVCModel.offer.offerID);
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    TempData["SuccessMessage"] = "Your information updated successfully";
                    addOfferMVCModel = getHotelAndRoomList(addOfferMVCModel);
                }
                else
                {
                    TempData["SuccessMessage"] = "Information update operation failed";
                }
               
            }
            return View("AddOffer", addOfferMVCModel);
        }

        public AddOfferMVCModel getHotelAndRoomList(AddOfferMVCModel addOfferMVCModel)
        {
            int agencyId = (int)Session["AgencyId"];
            IEnumerable<HotelMVCModel> hotelList;
            HttpResponseMessage responseHotelList = GlobalVariables.webApiClient.GetAsync("Hotels").Result;
            hotelList = responseHotelList.Content.ReadAsAsync<IEnumerable<HotelMVCModel>>().Result;
            createLog("Agency", agencyId, "GET", "Hotels", 0);

            IEnumerable<HotelRoomTypeMVCModel> allHotelRoomList;
            HttpResponseMessage responseHotelRoomList = GlobalVariables.webApiClient.GetAsync("HotelRoomTypes").Result;
            allHotelRoomList = responseHotelRoomList.Content.ReadAsAsync<IEnumerable<HotelRoomTypeMVCModel>>().Result;
            var hotelRoomList = allHotelRoomList.Where(x => x.hotelID == addOfferMVCModel.offer.hotelID).ToList();
            createLog("Agency", agencyId, "GET", "HotelRoomTypes", 0);
            addOfferMVCModel.hotelList = hotelList;
            addOfferMVCModel.roomList = getRoomTypeList(hotelRoomList);
            return addOfferMVCModel;

        }



        public List<RoomTypeMVCModel> getRoomTypeList(List<HotelRoomTypeMVCModel> hotelRoomList)
        {
            int agencyId = (int)Session["AgencyId"];
            List<RoomTypeMVCModel> roomList = new List<RoomTypeMVCModel>();
            
            foreach (HotelRoomTypeMVCModel hotelRooomType in hotelRoomList)
            {
                RoomTypeMVCModel roomType;
                HttpResponseMessage responseRoom = GlobalVariables.webApiClient.GetAsync("RoomTypes/"+hotelRooomType.roomTypeID).Result;
                createLog("Agency", agencyId, "GET", "RoomTypes", 0);
                roomType = responseRoom.Content.ReadAsAsync<RoomTypeMVCModel>().Result;
                roomType.displayName = roomType.name + " " + roomType.type;
                roomList.Add(roomType);
            }

            return roomList;

        }

        public ActionResult AgencyOffers()
        {
            int agencyId = (int)Session["AgencyId"];
            IEnumerable<OfferMVCModel> offerList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Offers").Result;
            offerList = response.Content.ReadAsAsync<IEnumerable<OfferMVCModel>>().Result;
            createLog("Agency", agencyId, "GET", "Offers", 0);
            offerList = getHotelsAndAgency(offerList);
            var agencyOfferList = offerList.Where(x => x.agencyID == agencyId).ToList();
            return View(agencyOfferList);
        }
    }
}