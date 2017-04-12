using DanMu.Controllers;
using Otv.Models;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Otv.Controllers
{
    public class DanMuController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                ViewBag.LoginUser = Session["User"];
                T_Setting setting = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.OTV_REQ_COUNT));
                ViewBag.Count = Int32.Parse(setting.Value);
            }
            catch (Exception)
            {
                
            }
            return View();
        }

        #region 获取弹幕预置列表 +getDmList
        /// <summary>
        /// 获取预置弹幕列表
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
                var filterList = db.T_DanMus.ToList();

                //搜索过滤
                if (_search)
                {
                    if ("bw".Equals(searchOper))//开始于
                    {
                        if ("Value".Equals(searchField))
                        {
                            filterList = filterList.Where(t => t.Value.StartsWith(searchString)).ToList();
                        }
                    }
                    else if ("cn".Equals(searchOper))//包含
                    {
                        if ("Value".Equals(searchField))
                        {
                            filterList = filterList.Where(t => t.Value.Contains(searchString)).ToList();
                        }
                    }
                    else if ("nc".Equals(searchOper))//不包含
                    {
                        if ("Value".Equals(searchField))
                        {
                            filterList = filterList.Where(t => !t.Value.Contains(searchString)).ToList();
                        }
                    }
                    else if ("le".Equals(searchOper))//小于等于
                    {
                        if ("Date".Equals(searchField))
                        {
                            DateTime dt;
                            if (DateTime.TryParse(searchString, out dt))
                            {
                                filterList = filterList.Where(t => t.Date <= dt).ToList();
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
                                filterList = filterList.Where(t => t.Date >= dt).ToList();
                            }
                        }
                    }
                }

                //排序
                if ("asc".Equals(sord))
                {
                    if ("Value".Equals(sidx))
                    {
                        filterList = filterList.OrderBy(t => t.Value).Take(take).ToList();
                    }
                    else if ("Date".Equals(sidx))
                    {
                        filterList = filterList.OrderBy(t => t.Date).Take(take).ToList();
                    }
                    else
                    {
                        filterList = filterList.OrderByDescending(t => t.Date).Take(take).ToList();
                    }
                }
                else if ("desc".Equals(sord))
                {
                    if ("Value".Equals(sidx))
                    {
                        filterList = filterList.OrderByDescending(t => t.Value).Take(take).ToList();
                    }
                    else if ("Date".Equals(sidx))
                    {
                        filterList = filterList.OrderByDescending(t => t.Date).Take(take).ToList();
                    }
                };
                //封装数据
                int records = filterList.Count;
                int total = (int)Math.Ceiling(((double)records) / rows);
                int skip = (rows * (page - 1));
                if (skip >= records)
                {
                    page = total;
                    skip = (rows * (page - 1));
                }
                filterList = filterList.Skip(skip).Take(rows).ToList();

                List<DMBean> list = new List<DMBean>();
                filterList.ForEach(t =>
                {
                    DMBean bean = new DMBean();
                    bean.id = t.Id;
                    bean.Id = t.Id;
                    bean.Value = t.Value;
                    bean.State = t.State;
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

        #region 预置弹幕操作管理 +Manage
        /// <summary>
        /// 过滤条目操作管理
        /// </summary>
        /// <param name="Value">新增条目名字</param>
        /// <param name="oper">操作类型(add & del)</param>
        /// <param name="id">删除ID</param>
        /// <returns></returns>
        public JsonResult Manage(string Value, string State, string oper, string id)
        {
            JsonResult res = new JsonResult();

            do
            {
                if (string.IsNullOrEmpty(oper))
                {
                    res.Data = "数据操作类型oper为空!";
                    break;
                }

                if ("add".Equals(oper))
                {
                    res = create(Value, State);
                }
                else if ("del".Equals(oper))
                {
                    res = delete(id);
                }
                else if ("edit".Equals(oper))
                {
                    res = edit(id, Value, State);
                }
                else
                {
                    res.Data = "数据操作类型oper非法!";
                }

            } while (false);

            return res;
        }
        #endregion

        #region 添加
        private JsonResult create(string Value, string State)
        {
            JsonResult res = new JsonResult();
            do
            {
                try
                {
                    if (string.IsNullOrEmpty(Value))
                    {
                        res.Data = "过滤词不能为空";
                        break;
                    }

                    if (Value.Length > 100)
                    {
                        res.Data = "内容长度大于100";
                        break;
                    }

                    T_DanMu fr = db.T_DanMus.Where(t => t.Value == Value).FirstOrDefault();
                    if (fr != null)
                    {
                        res.Data = "已存在相同的评论";
                        break;
                    }

                    bool state = false;
                    if ("on".Equals(State))
                    {
                        state = true;
                    }
                    else if ("1".Equals(State))
                    {
                        state = true;
                    }

                    T_DanMu dm = new T_DanMu()
                    {
                        Value = Value,
                        State = state,
                        Date = DateTime.Now
                    };

                    db.T_DanMus.Add(dm);
                    db.SaveChanges();

                    if (!QueueManager.loadPool())
                    {
                        res.Data = "新增成功，预置Pool实时更新失败";
                        break;
                    }

                    res.Data = "ok";
                }
                catch (Exception)
                {
                    res.Data = "服务器异常";
                }

            } while (false);

            return res;
        }
        #endregion

        #region 删除
        private JsonResult delete(string id)
        {
            JsonResult res = new JsonResult();
            do
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        res.Data = "删除条目ID不能为空";
                        break;
                    }
                    string sql = string.Format("delete from [T_DanMus] where [Id] in ({0})", id);
                    db.Database.ExecuteSqlCommand(sql, new SqlParameter[] { });
                    if (!QueueManager.loadPool())
                    {
                        res.Data = "新增成功，预置Pool实时更新失败";
                        break;
                    }
                    res.Data = "ok";
                }
                catch (Exception)
                {
                    res.Data = "服务器异常";
                }

            } while (false);

            return res;
        }
        #endregion

        #region 编辑
        private JsonResult edit(string id, string Value, string State)
        {
            JsonResult res = new JsonResult();
            do
            {
                try
                {
                    if (string.IsNullOrEmpty(Value))
                    {
                        res.Data = "过滤词不能为空";
                        break;
                    }

                    if (Value.Length > 100)
                    {
                        res.Data = "内容长度大于100";
                        break;
                    }
                    long key = long.Parse(id);
                    T_DanMu d = db.T_DanMus.Where(t => t.Id != key && t.Value == Value).FirstOrDefault();
                    if (d != null)
                    {
                        res.Data = "已存在相同的评论";
                        break;
                    }

                    bool state = false;
                    if ("on".Equals(State))
                    {
                        state = true;
                    }
                    else if ("1".Equals(State))
                    {
                        state = true;
                    }

                    T_DanMu dm = db.T_DanMus.Find(key);
                    if (dm == null)
                    {
                        res.Data = "该条目已不存在";
                        break;
                    }

                    dm.Value = Value;
                    dm.State = state;
                    db.Entry<T_DanMu>(dm).State = EntityState.Modified;
                    db.SaveChanges();
                    if (!QueueManager.loadPool())
                    {
                        res.Data = "新增成功，预置Pool实时更新失败";
                        break;
                    }
                    res.Data = "ok";
                }
                catch (Exception)
                {
                    res.Data = "服务器异常";
                }

            } while (false);

            return res;
        }
        #endregion

        #region 设置获取弹幕条数  +Setting
        /// <summary>
        /// 设置获取弹幕条数
        /// </summary>
        /// <param name="Count"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Setting(string Count)
        {
            JsonResult ret = new JsonResult();

            do
            {
                try
                {
                    int count = Int32.Parse(Count);
                    if (count < 10 || count > 40)
                    {
                        ret.Data = "设置的值不在10-40之间";
                        break;
                    }

                    T_Setting setting = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.OTV_REQ_COUNT));
                    setting.Value = Count;
                    db.Entry<T_Setting>(setting).State = EntityState.Modified;
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
        #endregion
    }
}
