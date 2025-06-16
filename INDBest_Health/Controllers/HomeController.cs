using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INDBest_Health.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		public ActionResult Blogs()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		public ActionResult Teams()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		public ActionResult Career()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		[HttpPost]
		public JsonResult ApplyNow(FormCollection form, HttpPostedFileBase ResumeFile)
		{
			string firstName = form["FirstName"];
			string lastName = form["LastName"];
			string email = form["Email"];
			string phone = form["Phone"];
			string message = form["Message"];
			string resumePath = "";

			if (string.IsNullOrWhiteSpace(firstName))
				return Json(new { success = false, message = "First Name is required." });

			if (string.IsNullOrWhiteSpace(lastName))
				return Json(new { success = false, message = "Last Name is required." });

			if (string.IsNullOrWhiteSpace(email))
				return Json(new { success = false, message = "Email is required." });

			if (string.IsNullOrWhiteSpace(phone))
				return Json(new { success = false, message = "Phone number is required." });

			if (string.IsNullOrWhiteSpace(message))
				return Json(new { success = false, message = "Message is required." });

			if (ResumeFile == null || ResumeFile.ContentLength == 0)
				return Json(new { success = false, message = "Resume file is required." });

			try
			{
				// Save uploaded file
				if (ResumeFile != null && ResumeFile.ContentLength > 0)
				{
					string uploadsDir = Server.MapPath("~/Uploads");
					if (!Directory.Exists(uploadsDir))
						Directory.CreateDirectory(uploadsDir);

					string uniqueFile = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ResumeFile.FileName);
					string fullPath = Path.Combine(uploadsDir, uniqueFile);
					ResumeFile.SaveAs(fullPath);
					resumePath = "/Uploads/" + uniqueFile;
				}

				// Save to DB
				string connStr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

				using (SqlConnection conn = new SqlConnection(connStr))
				{
					string query = @"INSERT INTO Candidates (FirstName, LastName, Email, Phone, Message, ResumePath, CreatedAt)
									VALUES (@FirstName, @LastName, @Email, @Phone, @Message, @ResumePath, GETDATE())";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@FirstName", firstName);
						cmd.Parameters.AddWithValue("@LastName", lastName);
						cmd.Parameters.AddWithValue("@Email", email);
						cmd.Parameters.AddWithValue("@Phone", phone);
						cmd.Parameters.AddWithValue("@Message", message);
						cmd.Parameters.AddWithValue("@ResumePath", resumePath);

						conn.Open();
						cmd.ExecuteNonQuery();
					}
				}

				return Json(new { success = true, message = "Application submitted successfully!", redirectURL = @Url.Action("Career", "Home", new { area = "" }) });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Error: " + ex.Message });
			}
		}

	}
}