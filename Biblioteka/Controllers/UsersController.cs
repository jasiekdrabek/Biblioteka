using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Biblioteka.Models;

namespace Biblioteka.Controllers
{
    public class UsersController : Controller
    {
        private DbLibrary _db = new DbLibrary();

        public ActionResult Index()
        {
            if (Session["idUser"] != null)
            {
                if (Session["Message"] != null)
                {
                    ViewBag.Message = "Hasło zostało zmienione";
                    Session["Message"] = null;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //GET: Register

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User _user)
        {
            if (ValidPassword(_user.Password)) { 
            _user.Role = "User";
            var errors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    var check = _db.User.FirstOrDefault(s => s.Name == _user.Name);
                    if (check == null)
                    {
                        int i = Guid.NewGuid().GetHashCode();

                        _user.Id = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
                        _db.Configuration.ValidateOnSaveEnabled = false;
                        _db.User.Add(_user);
                        _db.SaveChanges();
                        ViewBag.Message = "Pomyślnie zarejestrowano nowego użytkownika";
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "Taki uzytkownik juz istnieje";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Hasło musi zawierać Wielką litere, małą literą oraz cyfrę";
                }
            }
            return View();


        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string name, string password)
        {
            if (ModelState.IsValid)
            {
                var data = _db.User.Where(s => s.Name.Equals(name) && s.Password.Equals(password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["Name"] = data.FirstOrDefault().Name;
                    Session["idUser"] = data.FirstOrDefault().Id;
                    Session["Role"] = data.FirstOrDefault().Role;
                    Session["Message"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Wpisałeś złe dane";
                    return View();
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult ChangePassword(string password,string oldpassword)
        {
            if (ValidPassword(password))
            {
                string name = (Session["Name"]).ToString();
                var user = _db.User.Where(s => s.Name.Equals(name) && s.Password.Equals(password)).FirstOrDefault();
                if (user != null)
                {
                    user.Password = oldpassword;
                    user.OldPassword = password;
                    _db.SaveChanges();
                    Session["Message"] = "1";
                }
            }
            else
            {
                ViewBag.Message = "Nowe hasło musi zawierać Wielką litere, małą literą oraz cyfrę";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult DeleteAccount()
        {
            string name = (Session["Name"]).ToString();
            var user = _db.User.Where(s => s.Name.Equals(name)).FirstOrDefault();
            _db.User.Remove(user);
            _db.SaveChanges();
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }
        private static bool ValidPassword(string haslo)
        {
            Regex smallletters = new Regex("[a-z]");
            Regex bigletters = new Regex("[A-Z]");
            Regex digits = new Regex("[0-9]");
            var condicion1 = (smallletters.Matches(haslo)).Count;
            var condicion3 = (bigletters.Matches(haslo)).Count;
            var condicion2 = (digits.Matches(haslo)).Count;
            if (condicion1 >= 1 && condicion2 >= 1 && condicion3 >= 1)
            {
                return true;
            }
            return false;
        }
    }
}