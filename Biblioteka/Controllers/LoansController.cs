using Biblioteka.DTO;
using Biblioteka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteka.Controllers
{
    public class LoansController : Controller
    {
        private DbLibrary _db = new DbLibrary();
        // GET: Loans
        public ActionResult Index()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            var l = _db.Loan.ToList();
            var listofloans = new List<LoanDTO>();
            foreach(var a in l)
            {
                var loan = new LoanDTO();
                var name = _db.Book.Where(x => x.Id.Equals(a.BookId)).FirstOrDefault();
                loan.Id = a.Id;
                loan.BookName = name.Name;
                loan.StartOfLoan = a.StartOfLoan;
                loan.EndOfLoan = a.EndOfLoan;
                if (a.UserId == int.Parse(Session["idUser"].ToString()))
                {
                    if(a.EndOfLoan == null)
                    {
                        loan.Status = "Active";
                    }
                    else
                    {
                        loan.Status = "Ended";
                    }
                    listofloans.Add(loan);
                }
            }
            return View(listofloans);
        }
        public ActionResult Loan(int id)
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            var loan = new Loan();
            loan.BookId = id;
            loan.UserId = int.Parse(Session["idUser"].ToString());
            loan.StartOfLoan = DateTime.Now;
            loan.EndOfLoan = null;
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                int i = Guid.NewGuid().GetHashCode();

                loan.Id = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
                _db.Configuration.ValidateOnSaveEnabled = false;
                var book = _db.Book.FirstOrDefault(x => x.Id.Equals(id));
                book.Quantity -= 1;
                if(book.Quantity < 0)
                {
                    return RedirectToAction("Index", "Books", "");
                }
                var userid = int.Parse(Session["idUser"].ToString());
                if (_db.Loan.Where(x => x.UserId.Equals(userid) && x.EndOfLoan.Equals(null)).Count() < 5)
                {
                    if (_db.Loan.Where(x => x.UserId.Equals(userid) && x.BookId.Equals(book.Id) && x.EndOfLoan.Equals(null)).Count() == 0)
                    {
                        _db.Loan.Add(loan);
                        _db.SaveChanges();
                        Session["Message"] = "3";
                    }
                    else
                    {
                        Session["Message"] = "4";
                    }
                }
                else
                {
                    Session["Message"] = "5";
                }
            }
                return RedirectToAction("Index","Books","");
        }
        [HttpPost]
        public ActionResult EndLoan(int id)
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            var loan = _db.Loan.Where(x => x.Id.Equals(id)).FirstOrDefault();
            loan.EndOfLoan = DateTime.Now;
            var book = _db.Book.Where(x => x.Id.Equals(loan.BookId)).FirstOrDefault();
            book.Quantity += 1;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}