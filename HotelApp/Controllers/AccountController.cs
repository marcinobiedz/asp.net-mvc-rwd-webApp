using HotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelApp.Controllers
{
    public class AccountController : Controller
    {
        //
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                using (var db = new HotelDBEntities())
                {
                    user userSession = (user)Session["user"];
                    UserPanel modelPanel = new UserPanel();
                    modelPanel.user = db.users.FirstOrDefault(u => u.email == userSession.email);
                    return View(modelPanel);
                }
            }
        }

        [HttpPost]
        public ActionResult Index(user user, string userBirth)
        {
            var userSession = (user)Session["user"];
            user.email = userSession.email;
            DateTime newBirthDate = Convert.ToDateTime(userBirth);
            UserPanel modelPanel = new UserPanel();
            using (var db = new HotelDBEntities())
            {
                user currentUser = db.users.FirstOrDefault(u => u.email == user.email);
                if (String.IsNullOrEmpty(user.name) || String.IsNullOrEmpty(user.surname))
                {
                    modelPanel.user = currentUser;
                    modelPanel.info.type = 0;
                    modelPanel.info.text = "You didn't fill name or surname, please fill those fields.";
                    return View(modelPanel);
                }
                if (!(String.IsNullOrEmpty(user.password)) && (user.password.Length > 10 || user.password.Length < 6))
                {
                    modelPanel.user = currentUser;
                    modelPanel.info.type = 0;
                    modelPanel.info.text = "Password must be between 6 and 10 characters.";
                    return View(modelPanel);
                }
                if (!(String.IsNullOrEmpty(user.password)) && (user.password.Length < 10 || user.password.Length > 6))
                {
                    var crypto = new SimpleCrypto.PBKDF2();
                    var encrPass = crypto.Compute(user.password);
                    currentUser.password = encrPass;
                    currentUser.password_salt = crypto.Salt;
                }
                currentUser.name = user.name;
                currentUser.surname = user.surname;
                currentUser.country = user.country;
                currentUser.birth_date = newBirthDate;
                db.users.Attach(currentUser);
                db.Entry(currentUser).Property(p => p.password).IsModified = true;
                db.Entry(currentUser).Property(p => p.password_salt).IsModified = true;
                db.Entry(currentUser).Property(p => p.name).IsModified = true;
                db.Entry(currentUser).Property(p => p.surname).IsModified = true;
                db.Entry(currentUser).Property(p => p.country).IsModified = true;
                db.Entry(currentUser).Property(p => p.birth_date).IsModified = true;
                db.SaveChanges();
                modelPanel.user = currentUser;
                modelPanel.info.type = 1;
                modelPanel.info.text = "Your data has been changed.";
                return View(modelPanel);
            }
        }

        [HttpGet]
        public ActionResult Forgot()
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Forgot(string forgotEmail)
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Message info = new Message();
                if (String.IsNullOrEmpty(forgotEmail))
                {
                    info.type = 0;
                    info.text = "You didn't fill the e-mail address field.";
                }
                else
                {
                    using (var db = new HotelDBEntities())
                    {
                        var user = db.users.FirstOrDefault(u => u.email == forgotEmail);
                        if (user == null)
                        {
                            info.text = "User with such e-mail address doesn't exist.";
                        }
                        else
                        {
                            string newPass = "test123";
                            var crypto = new SimpleCrypto.PBKDF2();
                            var encrPass = crypto.Compute(newPass);
                            user.password = encrPass;
                            user.password_salt = crypto.Salt;
                            try
                            {
                                db.users.Attach(user);
                                db.Entry(user).Property(p => p.password).IsModified = true;
                                db.Entry(user).Property(p => p.password_salt).IsModified = true;
                                db.SaveChanges();
                                info.type = 1;
                                info.text = "Your password has been changed on: test123.";
                            }
                            catch (Exception e)
                            {
                                info.text = "Unexpected database error.";
                            }
                        }
                    }
                }
                return View(info);
            }
        }

        public ActionResult Logout()
        {
            var sessioUser = (user)Session["user"];
            if (sessioUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Session["user"] = null;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Login(string logEmail, string logPass)
        {
            var sessioUser = (user)Session["user"];
            if (sessioUser != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Message info = new Message();
                List<bool> check = new List<bool>();
                check.Add(String.IsNullOrEmpty(logEmail));
                check.Add(String.IsNullOrEmpty(logPass) || logPass.Length < 6 || logPass.Length > 10);
                if (check.Contains(true))
                {
                    info.text = "You didn't fill correctly all of the fields. ";
                    info.text += "Remember that: password must have between 6-10 marks.";
                    info.type = 0;
                }
                else
                {
                    try
                    {
                        using (var db = new HotelDBEntities())
                        {
                            var checkUser = db.users.FirstOrDefault(u => u.email == logEmail);
                            if (checkUser == null)
                            {
                                info.text = "You have typed wrong e-mail or password.";
                                info.type = 0;
                            }
                            else
                            {
                                var crypto = new SimpleCrypto.PBKDF2();
                                var encrPass = crypto.Compute(logPass, checkUser.password_salt);
                                if (encrPass == checkUser.password)
                                {
                                    Session["user"] = checkUser;
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    info.text = "You have typed wrong e-mail or password.";
                                    info.type = 0;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        info.text = "Unexpected database error. Please contact with administrator.";
                        info.type = 0;
                    }
                }
                return RedirectToAction("Index", "Home", new { info.type, info.text });
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            var userSession = (user)Session["user"];
            if (userSession != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Register(string regEmail, string regPass, string regConfPass, string regName, string regSurname, string regCountry, string regBirth)
        {
            Message info = new Message();
            List<bool> check = new List<bool>();
            check.Add(String.IsNullOrEmpty(regEmail));
            check.Add(String.IsNullOrEmpty(regPass) || regPass.Length < 6 || regPass.Length > 10);
            check.Add(String.IsNullOrEmpty(regConfPass) || regConfPass.Length < 6 || regConfPass.Length > 10);
            check.Add(String.Compare(regConfPass, regPass) != 0);
            check.Add(String.IsNullOrEmpty(regName));
            check.Add(String.IsNullOrEmpty(regSurname));
            check.Add(String.IsNullOrEmpty(regCountry));
            check.Add(String.IsNullOrEmpty(regBirth));
            if (check.Contains(true))
            {
                info.text = "You didn't fill correctly all of the fields. ";
                info.text += "Remember that: password and confirmations must be the same, have between 6-10 marks and birth date of legal age.";
                info.type = 0;
            }
            else
            {
                user newUser = new user();
                var crypto = new SimpleCrypto.PBKDF2();
                var encrPass = crypto.Compute(regPass);
                newUser.password = encrPass;
                newUser.password_salt = crypto.Salt;
                DateTime birthDate = Convert.ToDateTime(regBirth);
                newUser.admin = false;
                newUser.birth_date = birthDate;
                newUser.email = regEmail;
                newUser.name = regName;
                newUser.surname = regSurname;
                newUser.country = regCountry;
                try
                {
                    using (var db = new HotelDBEntities())
                    {
                        var checkUser = db.users.FirstOrDefault(u => u.email == regEmail);
                        if (checkUser == null)
                        {
                            db.users.Add(newUser);
                            db.SaveChanges();
                            info.text = "New user has been added. Now you can log in using your e-mail and password.";
                            info.type = 1;
                        }
                        else
                        {
                            info.text = "User with this e-mail address has been alredy created.";
                            info.type = 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    info.text = "Unexpected database error. Please contact with administrator.";
                    info.type = 0;
                }
            }
            return View(info);
        }
    }
}
