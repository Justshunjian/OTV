using DanMu.Controllers;
using Otv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Otv.Controllers
{
    public class RecordController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.LoginUser = Session["User"];
            return View();
        }

        #region 获取弹幕记录列表 +getDmList
        /// <summary>
        /// 获取弹幕记录列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">每页数据条数</param>
        /// <param name="sidx">排序列名</param>
        /// <param name="sord">排序方式</param>
        /// <param name="_search">是否搜索</param>
        /// <param name="searchField">过滤列名</param>
        /// <param name="searchString">过滤字串</param>
        /// <param name="searchOper">过滤方式</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getDmList(int page, int rows, string sidx, string sord, bool _search, string searchField, string searchString, string searchOper)
        {
            JsonResult ret = new JsonResult();

            int take = 2 * 10000;
            var res = new JsonResult();
            try
            {
                var dmList = db.T_Records.ToList();

                //搜索过滤
                if (_search)
                {
                    if ("eq".Equals(searchOper))//等于
                    {
                        if ("Cuid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Cuid == searchString).ToList();
                        }
                        if ("Uid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Uid == searchString).ToList();
                        }
                    }
                    else if ("bw".Equals(searchOper))//开始于
                    {
                        if ("Cuid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Cuid.StartsWith(searchString)).ToList();
                        }
                        if ("Content".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Content.StartsWith(searchString)).ToList();
                        }
                        if ("Uid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Uid.StartsWith(searchString)).ToList();
                        }

                    }
                    else if ("cn".Equals(searchOper))//包含
                    {
                        if ("Cuid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Cuid.Contains(searchString)).ToList();
                        }
                        if ("Content".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Content.Contains(searchString)).ToList();
                        } 
                        if ("Uid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => t.Uid.Contains(searchString)).ToList();
                        }
                    }
                    else if ("nc".Equals(searchOper))//不包含
                    {
                        if ("Cuid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => !t.Cuid.Contains(searchString)).ToList();
                        } 
                        if ("Content".Equals(searchField))
                        {
                            dmList = dmList.Where(t => !t.Content.Contains(searchString)).ToList();
                        } 
                        if ("Uid".Equals(searchField))
                        {
                            dmList = dmList.Where(t => !t.Uid.Contains(searchString)).ToList();
                        }
                    }
                    else if ("le".Equals(searchOper))//小于等于
                    {
                        if ("Date".Equals(searchField))
                        {
                            DateTime dt;
                            if (DateTime.TryParse(searchString, out dt))
                            {
                                dmList = dmList.Where(t => t.Date <= dt).ToList();
                            }
                        }
                    }
                    else if ("ge".Equals(searchOper))//大于等于
                    {
                        if ("Date".Equals(searchField))
                        {
                            DateTime dt;
                            if (DateTime.TryParse(searchString, out dt))
                            {
                                dmList = dmList.Where(t => t.Date >= dt).ToList();
                            }
                        }
                    }
                }

                //排序
                if ("asc".Equals(sord))
                {
                    if ("Cuid".Equals(sidx))
                    {
                        dmList = dmList.OrderBy(t => t.Cuid).Take(take).ToList();
                    }
                    else if ("Content".Equals(sidx))
                    {
                        dmList = dmList.OrderBy(t => t.Content).Take(take).ToList();
                    }
                    else if ("Date".Equals(sidx))
                    {
                        dmList = dmList.OrderBy(t => t.Date).Take(take).ToList();
                    }
                    else if ("Uid".Equals(sidx))
                    {
                        dmList = dmList.OrderBy(t => t.Uid).Take(take).ToList();
                    }
                    else
                    {
                        dmList = dmList.OrderByDescending(t => t.Date).Take(take).ToList();
                    }
                }
                else if ("desc".Equals(sord))
                {
                    if ("Cuid".Equals(sidx))
                    {
                        dmList = dmList.OrderByDescending(t => t.Cuid).Take(take).ToList();
                    }
                    else if ("Content".Equals(sidx))
                    {
                        dmList = dmList.OrderByDescending(t => t.Content).Take(take).ToList();
                    }
                    else if ("Uid".Equals(sidx))
                    {
                        dmList = dmList.OrderByDescending(t => t.Uid).Take(take).ToList();
                    }
                    else if ("Date".Equals(sidx))
                    {
                        dmList = dmList.OrderByDescending(t => t.Date).Take(take).ToList();
                    }
                };
                //封装数据
                int records = dmList.Count;
                int total = (int)Math.Ceiling(((double)records) / rows);
                int skip = (rows * (page - 1));
                if (skip >= records)
                {
                    page = total;
                    skip = (rows * (page - 1));
                }
                dmList = dmList.Skip(skip).Take(rows).ToList();

                List<DmBean> list = new List<DmBean>();
                dmList.ForEach(t =>
                {
                    DmBean bean = new DmBean();

                    bean.Cuid = t.Cuid;
                    bean.Content = t.Content;
                    bean.IP = t.IP;
                    bean.Uid = t.Uid;
                    bean.Date = t.Date.ToString();
                    list.Add(bean);
                });
                res.Data = new { records = records, page = page, rows = list, total = total };

            }
            catch (Exception)
            {
                res.Data = "error";
            }

            return res;
        }
        #endregion

    }
}
