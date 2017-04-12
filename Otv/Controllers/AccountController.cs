using DanMu.Filter;
using Otv.Models;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace DanMu.Controllers
{
    [InitializeSimpleMembership]
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                do
                {
                    if (string.IsNullOrWhiteSpace(model.User)
                        ||string.IsNullOrWhiteSpace(model.Pwd)
                        ||string.IsNullOrWhiteSpace(model.VerificationCode))
                    {
                        ModelState.AddModelError("", "登陆信息不完整");
                        break;
                    }

                    if (!GetRandomCode().ToLower().Equals(model.VerificationCode.ToLower()))
                    {
                        ModelState.AddModelError(string.Empty, "验证码输入错误");
                        break;
                    }

                    T_User user = db.T_Users.Find(model.User);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "该用户不存在");
                        break;
                    }

                    string pwd = MD5Utils.encode(model.Pwd);

                    if (user.Pwd.Equals(pwd))
                    {
                        Session["User"] = model.User;
                        Session.Timeout = 24*60;//登陆超时30分钟

                        user.LastLoginDate = DateTime.Now;
                        db.Entry<T_User>(user).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Index", "Manager");
                    }

                    ModelState.AddModelError("", "用户密码不正确");

                } while (false);
            }
            catch (Exception)
            {
                
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            //清除登录Session
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reset(ResetModel model)
        {
            try
            {
                do
                {
                    if (string.IsNullOrWhiteSpace(model.User)
                        || string.IsNullOrWhiteSpace(model.Email)
                        || string.IsNullOrWhiteSpace(model.VerificationCode))
                    {
                        ModelState.AddModelError("", "登陆信息不完整");
                        break;
                    }

                    if (!GetRandomCode().ToLower().Equals(model.VerificationCode.ToLower()))
                    {
                        ModelState.AddModelError(string.Empty, "验证码输入错误");
                        break;
                    }

                    T_User user = db.T_Users.Find(model.User);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "该用户不存在");
                        break;
                    }

                    if (!string.Equals(user.Email,model.Email,StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("", "该用户邮件不正确");
                        break;
                    }

                    string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //写数据库
                    T_PwdUpdate pwdUpdate = new T_PwdUpdate
                    {
                        User = model.User,
                        Pwd = "",
                        Email = model.Email,
                        CreateDate = dt,
                        Desc = "忘记密码"
                    };

                    db.T_PwdUpdates.Add(pwdUpdate);
                    db.SaveChanges();

                    //查找数据库

                    //发送邮件
                    string title = string.Format("Otv账户密码修改_{0}", DateTime.Now.ToString());
                    StringBuilder MailContent = new StringBuilder();
                    MailContent.Append("亲爱Otv会员：" + model.User + "<br/>");
                    MailContent.Append("    您好！你于");
                    MailContent.Append(DateTime.Now);
                    MailContent.Append("通过邮件审请找回密码。<br/>");
                    MailContent.Append("    为了安全起见，请点击以下链接重设个人密码,链接有效期30分钟：<br/><br/>");
                    string url = "http://www.linkavaiyun.com/otv/Account/ResetPassword?User=" + model.User + "&Date=" + dt;
                    MailContent.Append("<a href='" + url + "'>" + url + "</a><br/><br/>");
                    MailContent.Append("    (如果无法点击该URL链接地址，请将它复制并粘帖到浏览器的地址输入框，然后单击回车即可。)");
                    Email email = new Email
                    {
                        To = model.Email,
                        Title = title,
                        Content = MailContent.ToString(),
                        Repeat = 0
                    };
                    EmailHandler.Instance.AppendMail(email,HttpContext.Server);

                    ModelState.AddModelError(string.Empty, "邮件已发送到 "+model.Email+" ,请查收并修改密码");

                } while (false);
            }
            catch (Exception)
            {

            }
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string User, string Date)
        {
            ViewBag.User = User;
            ViewBag.Date = Date;

            try
            {
                do
                {
                    //判断该链接是否有效
                    T_PwdUpdate pwdUpdate = db.T_PwdUpdates.Find(new object[] { User, Date });
                    if (pwdUpdate == null || !string.IsNullOrWhiteSpace(pwdUpdate.Pwd))
                    {
                        throw new Exception();
                    }

                    DateTime start = DateTime.Parse(pwdUpdate.CreateDate);
                    DateTime end = DateTime.Now;
                    TimeSpan ts = end - start;
                    if (ts.Minutes > 30)
                    {
                        throw new Exception();
                    }

                } while (false);
            }
            catch (Exception)
            {
                return RedirectToAction("ResetSuc", "Account", new { msg = "该链接已失效" });
            }

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPwdModel model)
        {
            try
            {
                do
                {
                    ViewBag.User = model.User;
                    ViewBag.Date = model.Date;
                    if (string.IsNullOrWhiteSpace(model.User)
                        || string.IsNullOrWhiteSpace(model.Pwd)
                        || string.IsNullOrWhiteSpace(model.RePwd)
                        || string.IsNullOrWhiteSpace(model.VerificationCode))
                    {
                        ModelState.AddModelError("", "登陆信息不完整");
                        break;
                    }

                    if (!model.RePwd.Equals(model.Pwd))
                    {
                        ModelState.AddModelError("", "两次密码不相同");
                        break;
                    }

                    if (!GetRandomCode().ToLower().Equals(model.VerificationCode.ToLower()))
                    {
                        ModelState.AddModelError(string.Empty, "验证码输入错误");
                        break;
                    }

                    T_User user = db.T_Users.Find(model.User);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "该用户不存在");
                        break;
                    }

                    //判断该链接是否有效
                    T_PwdUpdate pwdUpdate = db.T_PwdUpdates.Find(new object[] { model.User, model.Date });
                    if (pwdUpdate == null)
                    {
                        return RedirectToAction("ResetSuc", "Account", new { msg = "该链接已失效" });
                    }

                    DateTime start = DateTime.Parse(pwdUpdate.CreateDate);
                    DateTime end = DateTime.Now;
                    TimeSpan ts = end - start;
                    if (ts.Minutes > 30)
                    {
                        return RedirectToAction("ResetSuc", "Account", new { msg = "该链接已失效" });
                    }

                    string pwd = MD5Utils.encode(model.Pwd);

                    user.Pwd = pwd;
                    db.Entry<T_User>(user).State = EntityState.Modified;

                    pwdUpdate.Pwd = pwd;
                    pwdUpdate.Desc = "修改密码成功";
                    db.Entry<T_PwdUpdate>(pwdUpdate).State = EntityState.Modified;

                    db.SaveChanges();

                    //清除登录Session
                    Session.Clear();

                    return RedirectToAction("ResetSuc", "Account", new { msg = "密码修改成功" });

                } while (false);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "修改密码失败");
            }

            return View();
        }

        public ActionResult ResetSuc(string msg)
        {
            ViewBag.Msg = msg;
            return View();
        }


        #region 验证码
        [OutputCache(Duration = 0)]
        public ActionResult RandomCode()
        {
            string code = ValidateCode.CreateRandomCode(4);
            Session["vcode"] = code;
            return File(ValidateCode.CreateImage(code), @"image/Jpeg");
        }

        public string GetRandomCode()
        {
            string code = "1500";
            try
            {
                code = Session["vcode"].ToString();
            }
            catch (Exception)
            {

                code = "1500";
            }
            return code;
        }

        public JsonResult valCodeCheck()
        {
            JsonResult res = new JsonResult();
            res.Data = GetRandomCode();
            return res;
        }
        #endregion

    }
}
