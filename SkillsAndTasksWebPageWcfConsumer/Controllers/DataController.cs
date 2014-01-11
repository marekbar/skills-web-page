using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsAndTasksWebPageWcfConsumer.Controllers
{
    /// <summary>
    /// Task to show on page
    /// </summary>
    public class TasksExtended
    {
        public SkillsAndTasksWebPageWcfConsumer.Service.Task UserTask { get; set; }
        public String DateCreate { get; set; }
        public String DateModification { get; set; }
        public String UserCreate { get; set; }
        public String UserAssigned { get; set; }
        public String UserModification { get; set; }
    }



    /// <summary>
    /// User to show on page
    /// </summary>
    public class UserExtended
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String Login { get; set; }
        public String Town { get; set; }
        public String Mail { get; set; }
        public String Phone { get; set; }
    }


    /// <summary>
    /// User skil to show on page
    /// </summary>
    public class UserSkillExtended
    {
        public UserExtended User { get; set; }
        public SkillsAndTasksWebPageWcfConsumer.Service.Skill Skill { get; set; }
    }



    public class DataController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }




        [HttpPost]
        public JsonResult UserSkills(string filter = "")
        {
            bool result = true;
            string error = "";
            dynamic data;
            try
            {
                var service = WCF.Service.Create();
                if (service == null) throw new Exception("Nie można połączyć się z usługą.");
                var resp = service.getDatabase();

                var data1 = from us in resp.UserSkills
                            join u in resp.Users
                            on us.UserId equals u.Id
                            select new UserSkillExtended
                            {
                                User = new UserExtended
                                {
                                    Login = u.Login,
                                    Name = u.Name,
                                    Surname = u.Surname,
                                    Town = u.Town,
                                    Mail = u.Mail,
                                    Phone = u.Phone
                                },
                                Skill = new Service.Skill
                                {
                                    Id = us.SkillId
                                }
                            };
                var data2 = from us in data1
                            join s in resp.Skills
                            on us.Skill.Id equals s.Id
                            select new UserSkillExtended
                            {
                                User = us.User,
                                Skill = s
                            };


                if (filter != "")
                {
                    filter = filter.ToUpper();
                    data = data2.Where(d =>
                        d.Skill.Name.ToUpper().Contains(filter) ||
                        d.Skill.Description.ToUpper().Contains(filter) ||
                        d.User.Login.ToUpper().Contains(filter) ||
                        d.User.Name.ToUpper().Contains(filter) ||
                        d.User.Surname.ToUpper().Contains(filter) ||
                        d.User.Mail.ToUpper().Contains(filter) ||
                        d.User.Town.ToUpper().Contains(filter) ||
                        d.User.Phone.ToUpper().Contains(filter)
                        ).ToList();
                }
                else
                {
                    data = data2;
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                data = null;
                result = false;
                error = ex.Message;
            }
            return Json(new
            {
                result = result,
                error = error,
                data = data
            });
        }

        [HttpPost]
        public JsonResult Users(string filter = "")
        {
            bool result = true;
            string error = "";
            dynamic data;
            try
            {
                var service = WCF.Service.Create();
                if (service == null) throw new Exception("Nie można połączyć się z usługą.");
                var resp = service.getDatabase();
                if (filter != "")
                {
                    filter = filter.ToUpper();
                    data = from u in resp.Users where
                           u.Login.ToUpper().Contains(filter) ||
                           u.Name.ToUpper().Contains(filter) ||
                           u.Surname.ToUpper().Contains(filter) ||
                           u.Phone.ToUpper().Contains(filter) ||
                           u.Town.ToUpper().Contains(filter) ||
                           u.Mail.ToUpper().Contains(filter)
                           select new UserExtended
                           {
                               Login = u.Login,
                               Name = u.Name,
                               Surname = u.Surname,
                               Mail = u.Mail,
                               Town = u.Town,
                               Phone = u.Phone
                           };
                }
                else
                {
                    data = from u in resp.Users 
                           select new UserExtended {
                                 Login = u.Login,
                                 Name = u.Name,
                                 Surname = u.Surname,
                                 Mail = u.Mail,
                                 Town = u.Town,
                                 Phone = u.Phone
                           };
                }
            }
            catch (Exception ex)
            {
                data = null;
                result = false;
                error = ex.Message;
            }
            return Json(new
            {
                result = result,
                error = error,
                data = data
            });
        }

        [HttpPost]
        public JsonResult Skills(string filter = "")
        {
            bool result = true;
            string error = "";
            dynamic data;
            try
            {
                var service = WCF.Service.Create();
                if (service == null) throw new Exception("Nie można połączyć się z usługą.");
                var resp = service.getDatabase();
                if (filter != "")
                {
                    filter = filter.ToUpper();
                    data = resp.Skills.Where(s =>
                        s.Description.ToUpper().Contains(filter) ||
                        s.Name.ToUpper().Contains(filter) ||
                        s.Id.ToString().Contains(filter)).ToList();
                }
                else
                {
                    data = resp.Skills;
                }
            }
            catch (Exception ex)
            {
                data = null;
                result = false;
                error = ex.Message;
            }
            return Json(new {
                result = result,
                error = error,
                data = data
            });
        }

        [HttpPost]
        public JsonResult Tasks(string filter = "")
        {
            bool result = true;
            string error = "";
            dynamic data;
            try
            {
                var service = WCF.Service.Create();
                if (service == null) throw new Exception("Nie można połączyć się z usługą.");
                var resp = service.getDatabase();

                var tasks = from t in resp.Tasks
                            join u in resp.Users
                            on t.CreatedBy equals u.Id
                            select new TasksExtended
                            {
                                UserTask = t,
                                DateCreate = t.CreationDate != null ? t.CreationDate.ToString("dd-MM-yyyy") : "",
                                DateModification = t.ModificationDate != null ? t.ModificationDate.ToString("dd-MM-yyyy") : "",
                                UserCreate = t.CreatedBy != null ? String.Format("{0} {1} ({2})", u.Name, u.Surname, u.Login) : ""
                            };
                var tasks2 = from t in tasks
                            join u in resp.Users
                            on t.UserTask.AssignedTo equals u.Id
                            select new TasksExtended
                            {
                                UserTask = t.UserTask,
                                UserCreate = t.UserCreate,
                                UserAssigned = t.UserTask.AssignedTo != null ? String.Format("{0} {1} ({2})", u.Name, u.Surname, u.Login) : ""
                            };

                var tasks3 = from t in tasks2
                             join u in resp.Users
                             on t.UserTask.ModifiedBy equals u.Id
                             select new TasksExtended
                             {
                                 UserTask = t.UserTask,
                                 UserCreate = t.UserCreate,
                                 UserAssigned = t.UserAssigned,
                                 UserModification = t.UserTask.ModifiedBy != null ? String.Format("{0} {1} ({2})", u.Name, u.Surname, u.Login) : ""
                             };

                               

                if (filter != "")
                {
                    filter = filter.ToUpper();
                    data = tasks3.Where(t =>
                        t.UserAssigned.ToUpper().Contains(filter) ||
                        t.UserCreate.ToUpper().Contains(filter) ||
                        t.UserModification.ToUpper().Contains(filter) ||
                        t.UserTask.Id.ToString().ToUpper().Contains(filter) ||
                        (t.UserTask.IsFinished ? "TAK" : "NIE").ToUpper().Contains(filter) ||
                        t.UserTask.ModificationDate.ToString().ToUpper().Contains(filter) ||
                        t.UserTask.Status.ToUpper().Contains(filter) ||
                        t.UserTask.TaskName.ToUpper().Contains(filter) ||
                        t.UserTask.Description.ToUpper().Contains(filter) ||
                        t.UserTask.CreationDate.ToString().ToUpper().Contains(filter)).ToList();
                }
                else
                {
                    data = tasks.ToList();
                }
                
                GC.Collect();
            }
            catch (Exception ex)
            {
                data = null;
                result = false;
                error = ex.Message;
            }
            return Json(new
            {
                result = result,
                error = error,
                data = data
            });
        }


    }
}
