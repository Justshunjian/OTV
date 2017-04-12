using Otv.Models;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DanMu.Controllers
{
    public class ManagerController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            T_User user = null;
            try
            {
                ViewBag.LoginUser = Session["User"];

                user = db.T_Users.Find(ViewBag.LoginUser);

                if (user == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Message");
            }

            return View(user);
        }

        [HttpPost]
        public JsonResult Update(string User,string Email,string Desc)
        {
            JsonResult ret = new JsonResult();

            do
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(User)
                                || string.IsNullOrWhiteSpace(Email)
                                || string.IsNullOrWhiteSpace(Desc))
                    {
                        ret.Data = "参数非法";
                        break;
                    }

                    Regex regex = new Regex(@"^[a-z0-9A-Z\._%+-]+@[a-z0-9A-Z\._%+-]+\.([a-zA-Z]){2,4}$");
                    if (!regex.Match(Email).Success)
                    {
                        ret.Data = "邮件格式非法";
                        break;
                    }

                    T_User user = db.T_Users.Find(User);
                    if (user == null)
                    {
                        ret.Data = "账户名不存在";
                        break;
                    }

                    user.Email = Email;
                    user.Desc = Desc;
                    db.Entry<T_User>(user).State = EntityState.Modified;
                    db.SaveChanges();
                    ret.Data = "ok";
                }
                catch (Exception)
                {
                    ret.Data = "发送错误";
                }

            } while (false);

            return ret;
        }

        [HttpPost]
        public JsonResult ResetPwd(string User, string OldPwd, string NewPwd, string PreNewPwd)
        {
            JsonResult ret = new JsonResult();

            do
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(User)
                                || string.IsNullOrWhiteSpace(OldPwd)
                                || string.IsNullOrWhiteSpace(NewPwd)
                                || string.IsNullOrWhiteSpace(PreNewPwd))
                    {
                        ret.Data = "参数非法";
                        break;
                    }

                    Regex regex = new Regex(@"^[a-z0-9A-Z_-]{6,12}$");
                    if (!regex.Match(NewPwd).Success)
                    {
                        ret.Data = "密码必须是6-12位大小写字母、数字及下划线";
                        break;
                    }

                    if (!NewPwd.Equals(PreNewPwd))
                    {
                        ret.Data = "两次密码不一致";
                        break;
                    }

                    T_User user = db.T_Users.Find(User);
                    if (user == null)
                    {
                        ret.Data = "账户名不存在";
                        break;
                    }

                    string pwd = MD5Utils.encode(OldPwd);

                    if (!user.Pwd.Equals(pwd))
                    {
                        ret.Data = "原密码错误";
                        break;
                    }

                    user.Pwd = MD5Utils.encode(NewPwd);
                    db.Entry<T_User>(user).State = EntityState.Modified;
                    //记录密码修改
                    T_PwdUpdate pwdUpdate = new T_PwdUpdate { 
                        User = user.User,
                        CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Pwd = user.Pwd,
                        Email = user.Email,
                        Desc = "修改密码成功"
                    };
                    db.T_PwdUpdates.Add(pwdUpdate);

                    db.SaveChanges();
                    ret.Data = "ok";
                }
                catch (Exception)
                {
                    ret.Data = "发送错误";
                }

            } while (false);

            return ret;
        }
    }
}
