using DanMu.Controllers;
using Otv.Models;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Otv.Controllers
{
    public class ChannelController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.LoginUser = Session["User"];
            try
            {
                T_Setting setting = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_HOUR));
                ViewBag.Hour = Int32.Parse(setting.Value);
                setting = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_MINUTE));
                ViewBag.Minute = Int32.Parse(setting.Value);
                setting = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_COUNT));
                ViewBag.Count = Int32.Parse(setting.Value);
            }
            catch (Exception)
            {
                ViewBag.Hour = 9;
                ViewBag.Minute = 0;
            }
            return View();
        }

        #region 获取节目列表 +getChnList
        /// <summary>
        /// 获取节目列表
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
        public JsonResult getChnList(int page, int rows, string sidx, string sord, bool _search, string searchField, string searchString, string searchOper)
        {
            JsonResult ret = new JsonResult();

            int take = 2 * 10000;
            var res = new JsonResult();
            try
            {
                var chnList = db.T_HotChns.ToList();

                //搜索过滤
                if (_search)
                {
                    if ("eq".Equals(searchOper))//等于
                    {
                        if ("ChnInfo".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.ChnInfo == searchString).ToList();
                        }
                        else if ("Cuid".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.Cuid == searchString).ToList();
                        }
                        else if ("Area".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.Area == searchString).ToList();
                        }
                    }
                    else if ("bw".Equals(searchOper))//开始于
                    {
                        if ("ChnInfo".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.ChnInfo.StartsWith(searchString)).ToList();
                        }
                        else if ("Cuid".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.Cuid.StartsWith(searchString)).ToList();
                        }
                        else if ("Area".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.Area == searchString).ToList();
                        }
                    }
                    else if ("cn".Equals(searchOper))//包含
                    {
                        if ("ChnInfo".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.ChnInfo.Contains(searchString)).ToList();
                        }
                        else if ("Cuid".Equals(searchField))
                        {
                            chnList = chnList.Where(t => t.Cuid.Contains(searchString)).ToList();
                        }
                    }
                    else if ("nc".Equals(searchOper))//不包含
                    {
                        if ("ChnInfo".Equals(searchField))
                        {
                            chnList = chnList.Where(t => !t.ChnInfo.Contains(searchString)).ToList();
                        }
                        else if ("Cuid".Equals(searchField))
                        {
                            chnList = chnList.Where(t => !t.Cuid.Contains(searchString)).ToList();
                        }
                    }
                    else if ("le".Equals(searchOper))//小于等于
                    {
                        if ("HeatValue".Equals(searchField))
                        {
                            long value = long.Parse(searchString);
                            chnList = chnList.Where(t => t.HeatValue <= value).ToList();
                        }

                    }
                    else if ("ge".Equals(searchOper))//大于等于
                    {
                        if ("HeatValue".Equals(searchField))
                        {
                            long value = long.Parse(searchString);
                            chnList = chnList.Where(t => t.HeatValue >= value).ToList();
                        }
                    }
                }

                //排序
                if ("asc".Equals(sord))
                {
                    if ("ChnInfo".Equals(sidx))
                    {
                        chnList = chnList.OrderBy(t => t.ChnInfo).Take(take).ToList();
                    }
                    else if ("Cuid".Equals(sidx))
                    {
                        chnList = chnList.OrderBy(t => t.Cuid).Take(take).ToList();
                    }
                    else if ("HeatValue".Equals(sidx))
                    {
                        chnList = chnList.OrderBy(t => t.HeatValue).Take(take).ToList();
                    }
                    else if ("Area".Equals(sidx))
                    {
                        chnList = chnList.OrderBy(t => t.Area).Take(take).ToList();
                    }
                }
                else if ("desc".Equals(sord))
                {
                    if ("ChnInfo".Equals(sidx))
                    {
                        chnList = chnList.OrderByDescending(t => t.ChnInfo).Take(take).ToList();
                    }
                    else if ("Cuid".Equals(sidx))
                    {
                        chnList = chnList.OrderByDescending(t => t.Cuid).Take(take).ToList();
                    }
                    else if ("HeatValue".Equals(sidx))
                    {
                        chnList = chnList.OrderByDescending(t => t.HeatValue).Take(take).ToList();
                    }
                    else if ("Area".Equals(sidx))
                    {
                        chnList = chnList.OrderByDescending(t => t.Area).Take(take).ToList();
                    }
                };
                //封装数据
                int records = chnList.Count;
                int total = (int)Math.Ceiling(((double)records) / rows);
                int skip = (rows * (page - 1));
                if (skip >= records)
                {
                    page = total;
                    skip = (rows * (page - 1));
                }
                chnList = chnList.Skip(skip).Take(rows).ToList();

                res.Data = new { records = records, page = page, rows = chnList, total = total };

            }
            catch (Exception)
            {
                res.Data = "error";
            }

            return res;
        } 
        #endregion

        #region 节目热度定时清零 +HotChnResetQuartz
        /// <summary>
        /// 节目热度定时清零
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult HotChnResetQuartz(string hour, string minute)
        {
            JsonResult ret = new JsonResult();

            do
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(hour)
                                || string.IsNullOrWhiteSpace(minute))
                    {
                        ret.Data = "参数不能为空";
                        break;
                    }

                    T_Setting settingHour = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_HOUR));
                    settingHour.Value = hour;
                    db.Entry<T_Setting>(settingHour).State = EntityState.Modified;

                    T_Setting settingMinute = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_MINUTE));
                    settingMinute.Value = minute;
                    db.Entry<T_Setting>(settingMinute).State = EntityState.Modified;

                    if (QuartzUtils.init(0, "", int.Parse(hour), int.Parse(minute)))
                    {
                        db.SaveChanges();
                        ret.Data = "ok";
                    }
                    else
                    {
                        ret.Data = "定时设置失败";
                    }
                }
                catch (Exception)
                {
                    ret.Data = "发送错误";
                }

            } while (false);


            return ret;
        } 
        #endregion

        #region 设置获取节目热度列表个数  +Setting
        /// <summary>
        /// 设置获取节目热度列表个数
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
                    if (count < 5 || count > 10000)
                    {
                        ret.Data = "设置的值不在5-10000之间";
                        break;
                    }

                    T_Setting setting = db.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_COUNT));
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
