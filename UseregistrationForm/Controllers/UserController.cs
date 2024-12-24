using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UseregistrationForm.DAL;
using UseregistrationForm.Models;

namespace UseregistrationForm.Controllers
{
    public class UserController : Controller
    {
        // GET: User
         UserRepository repository = new UserRepository();
        private const int PageSize = 4;
        // GET: User
        public ActionResult Index(int pageNumber = 1)
        {
            var users = repository.GetAllUsers(pageNumber, PageSize);
            var totalUsers = repository.GetTotalUsersCount();
            var totalPages = (int)Math.Ceiling((double)totalUsers / PageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                
                repository.AddUser(user);
                TempData["Success"] = "You have Successfully Registered";
                return RedirectToAction("Index");

            }
           
            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            User user = repository.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                repository.UpdateUser(user);
                TempData["Success"] = "User Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            User user = repository.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            repository.DeleteUser(id);
            TempData["Success"] = "User Deleted Successfully";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            User user = repository.GetUserById(id); 
            if (user == null)
            {
                return HttpNotFound(); 
            }
            return View(user); 
        }

        public ActionResult Download()
        {
            var users = repository.GetAllUsers(1, int.MaxValue); // Get all users for download
            var csv = new StringBuilder();
            csv.AppendLine("Id,Username,Email");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.Id},{user.Username},{user.Email}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var stream = new MemoryStream(bytes);

            return File(stream, "text/csv", "users.csv");
        }

        private string GenerateCsv(IEnumerable<User> users)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Username,Email,Password"); // CSV headers

            foreach (var user in users)
            {
                csvBuilder.AppendLine($"{user.Username},{user.Email},{user.Password}"); // CSV data rows
            }

            return csvBuilder.ToString();
        }
    }
}