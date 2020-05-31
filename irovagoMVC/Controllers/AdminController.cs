using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using irovagoMVC.Models;

namespace irovagoMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult HomePage()
        {
            IEnumerable<LogMVCModel> logList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Logs").Result;
            logList = response.Content.ReadAsAsync<IEnumerable<LogMVCModel>>().Result;
            return View(logList);
        }

        public ActionResult LogOut()
        {   
           
            return RedirectToAction("Index","Home");
        }


        public ActionResult AdminManageHotel()
        {
            IEnumerable<HotelMVCModel> hotelList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Hotels").Result;
            hotelList = response.Content.ReadAsAsync<IEnumerable<HotelMVCModel>>().Result;
            return View(hotelList);
        }


        public ActionResult ManageAgency()
        {

            IEnumerable<AgencyMVCModel> agencyList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Agencies").Result;
            agencyList = response.Content.ReadAsAsync<IEnumerable<AgencyMVCModel>>().Result;
            return View(agencyList);
        }

        public ActionResult DeleteAgency(int agencyId)
        {
            IEnumerable<OfferMVCModel> offerList;
            HttpResponseMessage responseOffer = GlobalVariables.webApiClient.GetAsync("Offers").Result;
            offerList = responseOffer.Content.ReadAsAsync<IEnumerable<OfferMVCModel>>().Result;
            var offerListToDelete = offerList.Where(x => x.agencyID == agencyId).ToList();

            IEnumerable<FavoriteOffersMVCModel> favoriteofferList;
            HttpResponseMessage responseFavoriteOffer = GlobalVariables.webApiClient.GetAsync("FavoriteOffers").Result;
            favoriteofferList = responseFavoriteOffer.Content.ReadAsAsync<IEnumerable<FavoriteOffersMVCModel>>().Result;


            IEnumerable<LoginMVCModel> loginList;
            HttpResponseMessage responseLogin = GlobalVariables.webApiClient.GetAsync("Logins").Result;
            loginList = responseLogin.Content.ReadAsAsync<IEnumerable<LoginMVCModel>>().Result;
            var loginToDelete = loginList.Where(x => x.agencyID == agencyId).ToList();

            foreach (LoginMVCModel login in loginToDelete)
            {
                HttpResponseMessage responseDeleteLogin = GlobalVariables.webApiClient.DeleteAsync("Logins/" + login.loginID).Result;
            }
           


            var favOfferListToDelete = new List<FavoriteOffersMVCModel>();


            foreach (OfferMVCModel offer in offerListToDelete)
            {
                 favOfferListToDelete = favoriteofferList.Where(x => x.offerID == offer.offerID).ToList();
                foreach (FavoriteOffersMVCModel favoriteOffer in favOfferListToDelete)
                {
                    HttpResponseMessage responseFavoriteOffersDelete = GlobalVariables.webApiClient.DeleteAsync("FavoriteOffers/" + favoriteOffer.favoriteOfferID).Result;

                }

                HttpResponseMessage responseDeleteOffer = GlobalVariables.webApiClient.DeleteAsync("Offers/" + offer.offerID).Result;
            }

            HttpResponseMessage response = GlobalVariables.webApiClient.DeleteAsync("Agencies/"+agencyId).Result;
            return RedirectToAction("ManageAgency", "Admin");
        }

        public ActionResult AddAgency(int id = 0)
        {

            return View(new LoginMVCModel());
        }

        [HttpPost]
        public ActionResult AddAgency(LoginMVCModel log)
        {
            log.email = log.Agency.email;
            HttpResponseMessage response = GlobalVariables.webApiClient.PostAsJsonAsync("Logins", log).Result;
            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TempData["SuccessMessage"] = "Your information added successfully"; 
            }
            else
            {
                TempData["SuccessMessage"] = "Operation failed";
            }
            return View();
        }

        public ActionResult AddHotel(int? id)
        {
            ManageHotelMVCModel manageHotelMVC = new ManageHotelMVCModel();
            if (id == -1)
            {
                manageHotelMVC = (ManageHotelMVCModel) Session["AddOrEditHotel"];
                return View(manageHotelMVC);
            }
            IEnumerable<RoomTypeMVCModel> roomList;
            HttpResponseMessage responseRoomList = GlobalVariables.webApiClient.GetAsync("RoomTypes").Result;
            roomList = responseRoomList.Content.ReadAsAsync<IEnumerable<RoomTypeMVCModel>>().Result;
            roomList = setRoomTypesDisplayName(roomList);


            HotelMVCModel hotel = new HotelMVCModel();
           
            
            manageHotelMVC.hotelRoomList = new List<RoomTypeMVCModel>();
            if (id == null) //new hotel
            {
                manageHotelMVC.hotel = new HotelMVCModel();
                manageHotelMVC.roomList = roomList.ToList();
            }
            else //update 
            {
                HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Hotels/"+id).Result;
                hotel = response.Content.ReadAsAsync<HotelMVCModel>().Result;

                IEnumerable<HotelRoomTypeMVCModel> allHotelRoomList;
                HttpResponseMessage responseHotelRoomList = GlobalVariables.webApiClient.GetAsync("HotelRoomTypes").Result;
                allHotelRoomList = responseHotelRoomList.Content.ReadAsAsync<IEnumerable<HotelRoomTypeMVCModel>>().Result;
                var hotelRoomList = allHotelRoomList.Where(x => x.hotelID == id).ToList();
                manageHotelMVC.hotelRoomList = getRoomTypeList(hotelRoomList);
                manageHotelMVC.roomList = getDropDownRoomList(hotelRoomList, hotel.hotelID, roomList.ToList());
             
            }
            manageHotelMVC.hotel = hotel;
            manageHotelMVC.roomType = new RoomTypeMVCModel();
            Session["AddOrEditHotel"] = manageHotelMVC;
            return View(manageHotelMVC);
        }


        public ActionResult SubmitHotel(ManageHotelMVCModel manageHotelMVCModel)
        {
            ManageHotelMVCModel temp = (ManageHotelMVCModel)Session["AddOrEditHotel"];
            manageHotelMVCModel.hotel.imgHotel = temp.hotel.imgHotel;

            if (manageHotelMVCModel.hotel.hotelID != 0) //update
            {
                HttpResponseMessage responseUpdate = GlobalVariables.webApiClient.PutAsJsonAsync("Hotels/" + manageHotelMVCModel.hotel.hotelID, manageHotelMVCModel.hotel).Result;
                controlResult(responseUpdate);
            }
            else //add
            {
                HttpResponseMessage responseUpdate = GlobalVariables.webApiClient.PostAsJsonAsync("Hotels", manageHotelMVCModel.hotel).Result;
                controlResult(responseUpdate);
            }

            saveNewRoomTypesToDB();
            return RedirectToAction("AdminManageHotel","Admin");
        }

        public void controlResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TempData["SuccessMessage"] = "Your information updated successfully";

            }
            else
            {
                TempData["SuccessMessage"] = "Operation failed";
            }
        }

        public void saveNewRoomTypesToDB()
        {
            IEnumerable<HotelRoomTypeMVCModel> allHotelRoomList;
            HttpResponseMessage responseHotelRoomList = GlobalVariables.webApiClient.GetAsync("HotelRoomTypes").Result;
            allHotelRoomList = responseHotelRoomList.Content.ReadAsAsync<IEnumerable<HotelRoomTypeMVCModel>>().Result;

            IEnumerable<HotelMVCModel> hotelList;
            HttpResponseMessage response = GlobalVariables.webApiClient.GetAsync("Hotels").Result;
            hotelList = response.Content.ReadAsAsync<IEnumerable<HotelMVCModel>>().Result;

        

            ManageHotelMVCModel manageHotelMVC = new ManageHotelMVCModel();
            manageHotelMVC = (ManageHotelMVCModel) Session["AddOrEditHotel"];
            if (manageHotelMVC.hotel.hotelID == 0) 
            {
                 manageHotelMVC.hotel.hotelID =  hotelList.Last().hotelID;
            } 
           
          
            var hotelRoomList = allHotelRoomList.Where(x => x.hotelID == manageHotelMVC.hotel.hotelID).ToList();
       
            //Delete Control 
            foreach(HotelRoomTypeMVCModel hotelRoomTypeMVC in hotelRoomList)
            {
                bool delete = true;
                foreach (RoomTypeMVCModel room in manageHotelMVC.hotelRoomList)
                {
                    if (room.roomTypeID == hotelRoomTypeMVC.roomTypeID)
                    {                       
                        delete = false;
                    }
                }

                if (delete)
                {
                    HttpResponseMessage responseDeleteRoom = GlobalVariables.webApiClient.DeleteAsync("HotelRoomTypes/" + hotelRoomTypeMVC.hotelRoomTypeID).Result;
                }
            }

            //Add Control 
           

            foreach (RoomTypeMVCModel room in manageHotelMVC.hotelRoomList)
            {
                bool add = true;
                foreach (HotelRoomTypeMVCModel hotelRoomTypeMVC in hotelRoomList)
                {
                    if (room.roomTypeID == hotelRoomTypeMVC.roomTypeID)
                    {
                        add = false;
                    }
                }

                if (add)
                {
                    HotelRoomTypeMVCModel newRoom = new HotelRoomTypeMVCModel();
                    newRoom.hotelID = manageHotelMVC.hotel.hotelID;
                    newRoom.roomTypeID = room.roomTypeID;
                    HttpResponseMessage responseDeleteRoom = GlobalVariables.webApiClient.PostAsJsonAsync("HotelRoomTypes", newRoom).Result;
                }
            }

        }

       
        public ActionResult ArrangeRoomOfHotel(int roomTypeId, bool isAdded, HotelMVCModel hotel)
        {
            ManageHotelMVCModel manageHotelMVC = new ManageHotelMVCModel();
            RoomTypeMVCModel room;
            HttpResponseMessage responseRoom = GlobalVariables.webApiClient.GetAsync("RoomTypes/" + roomTypeId).Result;
            room = responseRoom.Content.ReadAsAsync<RoomTypeMVCModel>().Result;
            room.displayName = room.name + " " + room.type;

            if (Session["AddOrEditHotel"] != null )
            {
                manageHotelMVC = (ManageHotelMVCModel)Session["AddOrEditHotel"];
                hotel.imgHotel = manageHotelMVC.hotel.imgHotel;
                manageHotelMVC.hotel = hotel;

                if (isAdded && roomTypeId != 0) //add
                {
                    manageHotelMVC.hotelRoomList.Add(room);
                    manageHotelMVC.roomList = manageHotelMVC.roomList.Where(x => x.roomTypeID != roomTypeId).ToList();
                }
                else //delete
                {
                    manageHotelMVC.hotelRoomList = manageHotelMVC.hotelRoomList.Where(x => x.roomTypeID != roomTypeId).ToList();
                    manageHotelMVC.roomList.Add(room);
                }

            }

            Session["AddOrEditHotel"] = manageHotelMVC;
            ModelState.Clear();
            return View("AddHotel",manageHotelMVC);
        }

        public ActionResult UploadImage(HttpPostedFileBase file, int hotelId, string name, string address, string url, string explanation)
        {
            ManageHotelMVCModel manageHotelMVC = (ManageHotelMVCModel)Session["AddOrEditHotel"];
            manageHotelMVC.hotel.hotelID = hotelId;
            manageHotelMVC.hotel.name = name;
            manageHotelMVC.hotel.url = url;
            manageHotelMVC.hotel.address = address;
            manageHotelMVC.hotel.explanation = explanation;

            if (file != null)
            {
                string theFileName = Path.GetFileName(file.FileName);
                byte[] thePictureAsBytes = new byte[file.ContentLength];
                using (BinaryReader theReader = new BinaryReader(file.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(file.ContentLength);
                    string base64 = "data:image/jpeg;base64,";
                    manageHotelMVC.hotel.imgHotel = base64 + Convert.ToBase64String(thePictureAsBytes);
                }

            }
            Session["AddOrEditHotel"] = manageHotelMVC;
            // after successfully uploading redirect the user
            return View("AddHotel", manageHotelMVC);
        }




        public List<RoomTypeMVCModel> getDropDownRoomList(List<HotelRoomTypeMVCModel> hotelRoomList, int hotelId, List<RoomTypeMVCModel> roomList )
        {
           
            foreach (HotelRoomTypeMVCModel hotelRoomType in hotelRoomList)
            {
                    roomList = roomList.Where(x => x.roomTypeID != hotelRoomType.roomTypeID).ToList(); 
            } 

            return roomList;

        }

      

        public List<RoomTypeMVCModel> getRoomTypeList(List<HotelRoomTypeMVCModel> hotelRoomList)
        {
            List<RoomTypeMVCModel> roomList = new List<RoomTypeMVCModel>();

            foreach (HotelRoomTypeMVCModel hotelRooomType in hotelRoomList)
            {
                RoomTypeMVCModel roomType;
                HttpResponseMessage responseRoom = GlobalVariables.webApiClient.GetAsync("RoomTypes/" + hotelRooomType.roomTypeID).Result;
                roomType = responseRoom.Content.ReadAsAsync<RoomTypeMVCModel>().Result;
                roomType.displayName = roomType.name + " " + roomType.type;
                roomList.Add(roomType);
            }

            return roomList;

        }

        public IEnumerable<RoomTypeMVCModel> setRoomTypesDisplayName(IEnumerable<RoomTypeMVCModel> roomList)
        {
            foreach(RoomTypeMVCModel room in roomList)
            {
                room.displayName = room.name + " " + room.type;
            }
            return roomList;
        }

    }
}