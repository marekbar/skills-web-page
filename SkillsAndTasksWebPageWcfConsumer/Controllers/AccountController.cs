using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SkillsAndTasksWebPageWcfConsumer.Models;

namespace SkillsAndTasksWebPageWcfConsumer.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult LogOn()
        {
            return View();
        }

        
        public ActionResult Error()
        {
            ViewBag.Error = Session["Errors"] == null ? "" : (String)Session["Errors"];
            Session["Errors"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var service = WCF.Service.Create();
                    if (service == null) throw new Exception("Nie można połączyć się z usługą.");
                    Service.Response resp = null;
                    try
                    {
                        resp = service.login(model.UserName, model.Password);
                    }
                    catch { throw new Exception("Usługa jest czasowo niedostępna."); }

                    if (resp != null && resp.Result == true && resp.HasError == false)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        return RedirectToAction("Index", "Data");
                    }
                    else
                    {
                        throw new Exception("Podana nazwa użytkownika lub hasło są niepoprawne.");
                    }

                }
            }
            catch (Exception ex)
            {
                Session["Errors"] = ex.Message;
                return RedirectToAction("Error", "Account");
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Activate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Activate(AccountActivateModel model)
        {
            try
            {
                var service = WCF.Service.Create();
                if (service == null) throw new Exception("Nie moża nawiązać połączenia z usługą.");
                var resp = service.activateByCode(model.Activation);
                if (!(resp.Result == true && resp.HasError == false))
                {
                    throw new Exception("Użytkownik nie został aktywowany, ponieważ wpisany kod jest nieprawidłowy.");
                }

                return RedirectToAction("Index", "Data");
            }
            catch (Exception ex)
            {
                Session["Errors"] = ex.Message;
                return RedirectToAction("Error", "Account");
            }
        }

        [HttpPost]
        public ActionResult Register(RegisterUser model)
        {
            if (ModelState.IsValid)
            {
                bool isCreated = true;
                string error = "";
                try
                {
                    var service = WCF.Service.Create();
                    Service.User user = new Service.User();
                    user.Login = model.Login;
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.Town = model.Town;
                    user.Mail = model.Mail;
                    user.Phone = model.Password;
                    user.Password = model.Password;
                    
                    var resp = service.createAccount(user);
                    if (resp.Result == true && resp.HasError == false)
                    {
                        isCreated = true;
                    }
                    else
                    {
                        isCreated = false;
                    }
                }
                catch (Exception ex)
                {
                    isCreated = false;
                    error = ex.Message;
                }
                if (isCreated)
                {
                    FormsAuthentication.SetAuthCookie(model.Login, false);
                    return RedirectToAction("Activate", "Account");
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Bieżące hasło jest niepoprawne lub nowe hasło jest nieprawidłowe.");
                }
            }

            // Dotarcie do tego miejsca wskazuje, że wystąpił błąd, wyświetl ponownie formularz
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Na stronie http://go.microsoft.com/fwlink/?LinkID=177550 znajduje się
            // pełna lista kodów stanu.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Nazwa użytkownika już istnieje. Wprowadź inną nazwę użytkownika.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Nazwa użytkownika dla tego adresu e-mail już istnieje. Wprowadź inny adres e-mail.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Podane hasło jest nieprawidłowe. Wprowadź prawidłową wartość hasła.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Podany adres e-mail jest nieprawidłowy. Sprawdź wartość i spróbuj ponownie.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Podana odpowiedź dla funkcji odzyskiwania hasła jest nieprawidłowa. Sprawdź wartość i spróbuj ponownie.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Podane pytanie dla funkcji odzyskiwania hasła jest nieprawidłowe. Sprawdź wartość i spróbuj ponownie.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Podana nazwa użytkownika jest nieprawidłowa. Sprawdź wartość i spróbuj ponownie.";

                case MembershipCreateStatus.ProviderError:
                    return "Dostawca uwierzytelniania zwrócił błąd. Sprawdź wpis i spróbuj ponownie. Jeśli problem nie zniknie, skontaktuj się z administratorem systemu.";

                case MembershipCreateStatus.UserRejected:
                    return "Żądanie utworzenia użytkownika zostało anulowane. Sprawdź wpis i spróbuj ponownie. Jeśli problem nie zniknie, skontaktuj się z administratorem systemu.";

                default:
                    return "Wystąpił nieznany błąd. Sprawdź wpis i spróbuj ponownie. Jeśli problem nie zniknie, skontaktuj się z administratorem systemu.";
            }
        }
        #endregion
    }
}
