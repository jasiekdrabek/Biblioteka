using Biblioteka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class BooksController : Controller
    {
        private DbLibrary _db = new DbLibrary();
        // GET: Books
        public ActionResult Index()
        {
            if (Session["idUser"] != null)
            {
                if (Session["Message"] != null)
            {
                if (Session["Message"].ToString() == "1")
                {
                    ViewBag.Message = "Dodano nową książkę";
                }
                if (Session["Message"].ToString() == "2")
                {
                    ViewBag.Message = "Edytowanie zakończone pomyślnie";
                }
                if(Session["Message"].ToString() == "3")
                {
                    ViewBag.Message = "Wypożyczyłeś nową książkę";
                }
                if(Session["Message"].ToString() == "4")
                {
                    ViewBag.Message = "Już wypożyczyłeś tą książkę";
                }
                if(Session["Message"].ToString() == "5")
                {
                    ViewBag.Message = "Nie możesz wypożyczyć więcej niż 5 książek";
                }
                Session["Message"] = null;
            }
            var books = _db.Book.ToList();
            return View(books);
            }
            else
            {
                return RedirectToAction("Login","Users");
            }
        }
        public ActionResult Add()
        {
            if (Session["idUser"] != null)
            {
                if (Session["Role"].ToString() == "Admin                                   ")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Users");
                }
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }
        [HttpPost]
        public ActionResult Add(Book book)
        {
            if (Session["idUser"] != null)
            {
                if (ModelState.IsValid)
                {
                    var check = _db.Book.FirstOrDefault(s => s.Name == book.Name && s.Author == book.Author);
                    if (check == null)
                    {
                        int i = Guid.NewGuid().GetHashCode();

                        book.Id = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
                        _db.Configuration.ValidateOnSaveEnabled = false;
                        _db.Book.Add(book);
                        _db.SaveChanges();
                        Session["Message"] = "1";
                        return RedirectToAction("Index");
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }
        public ActionResult Edit(int id)
        {
            if (Session["idUser"] != null)
            {
                if (Session["Role"].ToString() == "Admin                                   ")
                {
                    var book = _db.Book.Where(s => s.Id == id).FirstOrDefault();
                    return View(book);
                }
                else
                {
                    return RedirectToAction("Login", "Users");
                }
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }
        [HttpPost]
        public ActionResult Edit(Book book)
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (ModelState.IsValid)
            {
                var b = _db.Book.Where(s => s.Name.Equals(book.Name) && s.Author.Equals(book.Author)).FirstOrDefault();
                b.Name = book.Name;
                b.NuberOfPages = book.NuberOfPages;
                b.Author = book.Author;
                b.Quantity = book.Quantity;
                _db.SaveChanges();
                Session["Message"] = "2";
                return RedirectToAction("Index");
            }
                return View(book);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            var book = _db.Book.Where(s => s.Id == id).FirstOrDefault();
            _db.Book.Remove(book);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}