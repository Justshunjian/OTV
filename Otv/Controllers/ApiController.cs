using DanMu.Controllers;
using Otv.Models;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace Otv.Controllers
{
    [AllowAnonymous]
    public class ApiController : BaseController
    {

        /// <summary>
        /// 根据节目信息获取节目唯一cuid
        /// </summary>
        /// <param name="cuid">节目唯一id</param>
        /// <param name="sat">卫星信息</param>
        /// <param name="cinfo">节目信息</param>
        /// <param name="t">tv/radio</param>
        /// <param name="f">频率</param>
        /// <param name="p">极性</param>
        /// <param name="a">区域</param>
        /// <returns></returns>
        public JsonResult getCuid(string cuid,string sat,string cinfo,int t,int f,int p,string a)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                ChnModel model = new ChnModel
                {
                    Cuid = cuid,
                    Sat = sat,
                    CInfo = cinfo,
                    TvRadio = t,
                    Frequency = f,
                    Polar = p,
                    Area = a
                };
                string uid = CuidUtils.getCuid(model);
                ret.Data = new { ret = "ok", cuid = uid, type = 0 };
            }catch{
                ret.Data = new { ret = "error", type = 1, desc = "error happen" };
            }

            return ret;
        }

        /// <summary>
        /// 判断CUID是否存在
        /// </summary>
        /// <param name="cuid"></param>
        /// <returns></returns>
        public JsonResult CheckCuid(string cuid)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                if (string.IsNullOrWhiteSpace(cuid))
                {
                    ret.Data = new { ret = "error", type = 2, desc = "cuid can not null" };
                }else if (!CuidUtils.cuidExists(cuid)) //判断当前cuid是否合法
                {
                    ret.Data = new { ret = "error", type = 3, decs = "cuid is invalid" };
                }
                else
                {
                    ret.Data = new { ret = "ok", type = 0 };
                }

            }catch
            {
                ret.Data = new { ret = "error", type = 1, desc = "error happen" };
            }
            return ret;
        }

        /// <summary>
        /// 动态生成二维码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ActionResult createQR(string content)
        {
            FileContentResult file = null;
            try
            {
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 2;
                qrCodeEncoder.QRCodeVersion = 7;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                Bitmap img = qrCodeEncoder.Encode(content);

                Bitmap newImg = new Bitmap(img.Width + 20, img.Height + 20);     //新建位图newImg
                Graphics gp = Graphics.FromImage(newImg);  //创建newImg的Graphics
                gp.FillRectangle(Brushes.White, new Rectangle(0, 0, newImg.Width, newImg.Height));   //把newImg涂成白色
                gp.DrawImage(img, 10, 10, img.Width, img.Height);
                gp.Dispose();

                MemoryStream ms = new MemoryStream();
                newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                newImg.Dispose();
                file = File(ms.ToArray(), "image/jpeg");
                ms.Dispose();
            }
            catch (Exception)
            {
                
            }
            return file;
        }

        /// <summary>
        /// 获取机器码列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public JsonResult getUids(int p=0)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (p == 0)
            {
                ret.Data = new { type="stb",list=CuidUtils.stbUids};
            }
            else if (p == 1)
            {
                ret.Data = new { type = "phone", list = CuidUtils.phoneUids };
            }
            return ret;
        }

        /// <summary>
        /// 获取客户端机顶盒MAC标识
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public JsonResult uploadStbUid(string uid)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                if (!CuidUtils.checkStbUid(uid)) //判断当前uid是否合法,如果不存在就生成
                {
                    CuidUtils.AddStbUid(uid);
                    ret.Data = new { ret = "ok", type = 2, decs = "add new stb mac"};
                }
                else
                {
                    ret.Data = new { ret = "ok", type = 0 };
                }
            }
            catch
            {
                ret.Data = new { ret = "error", type = 1, desc = "error happen" };
            }
            return ret;
        }

        /// <summary>
        /// 获取手机唯一标识
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public JsonResult getPhoneUid(string uid)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                if (!CuidUtils.checkPhoneUid(uid)) //判断当前uid是否合法,如果不存在就生成
                {
                    string createUid = CuidUtils.getPhoneUid();
                    ret.Data = new { ret = "ok", type = 2, decs = "create new phone uid", uid = createUid };
                }
                else
                {
                    ret.Data = new { ret = "ok", type = 0 };
                }

            }
            catch
            {
                ret.Data = new { ret = "error", type = 1, desc = "error happen" };
            }
            return ret;
        }

        #region 客户端向服务器提交弹幕数据 +Put
        /// <summary>
        /// 客户端向服务器提交弹幕数据
        /// </summary>
        /// <param name="cuid">节目唯一号</param>
        /// <param name="content">内容</param>
        /// <param name="uid">客户端唯一标识</param>
        /// <param name="p">0:stb,1:phone</param>
        /// <returns></returns>
        public JsonResult Put(string cuid, string content,string uid,int p=0)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                //判断当前cuid是否合法
                if (!CuidUtils.cuidExists(cuid))
                {
                    ret.Data = new { ret = "error", type=2,decs = "cuid is invalid" };
                }
                else if (!QueueManager.filter(content))
                {
                    ret.Data = new { ret = "error", type = 3, desc = "content is invalid" };
                }
                else
                {
                    int type = 0;
                    if (p == 0 && !CuidUtils.checkStbUid(uid))
                    {
                        type = 5;
                        ret.Data = new { ret = "error", type = type, desc = "please upload stb mac info at first" };
                        return ret;
                    }
                    else if (p == 1 && !CuidUtils.checkPhoneUid(uid))
                    {
                        type = 4;
                        uid = CuidUtils.getPhoneUid();
                    }

                    DanmuBean bean = new DanmuBean()
                    {
                        Cuid = cuid,
                        Content = content,
                        Uid = uid,
                        DateTime = DateTime.Now
                    };
                    //添加到队列
                    QueueManager.Instance.Put(bean);

                    //写数据库
                    T_Record danmu = new T_Record()
                    {
                        Cuid = bean.Cuid,
                        Content = bean.Content,
                        Uid = uid,
                        Date = DateTime.Now,
                        IP = getClientIP()
                    };
                    db.T_Records.Add(danmu);
                    db.SaveChanges();
                    ret.Data = new { ret = "ok", type = type, uid = uid };
                }
            }
            catch
            {
                ret.Data = new { ret = "error", type = 1, desc = "error happen" };
            }

            return ret;
        } 
        #endregion

        #region 客户端向服务器获取节目唯一编号的弹幕数据 +Get
        /// <summary>
        /// 客户端向服务器获取节目唯一编号的弹幕数据
        /// </summary>
        /// <param name="cuid">节目唯一编号</param>
        /// <returns>JSON格式的弹幕数据</returns>
        [HttpGet]
        public JsonResult Get(string cuid, string uid,int p=0)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            do
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(cuid))
                    {
                        ret.Data = new { ret = "error", type = 2, desc = "cuid can not null" };
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(uid))
                    {
                        ret.Data = new { ret = "error", type = 3, desc = "uid can not null" };
                        break;
                    }
                    if (p == 0 && !CuidUtils.checkStbUid(uid))
                    {
                        ret.Data = new { ret = "error", type = 4, desc = "stb uid is invalid" };
                        break;
                    }
                    else if (p == 1 && !CuidUtils.checkPhoneUid(uid))
                    {
                        ret.Data = new { ret = "error", type = 5, desc = "phone uid is invalid" };
                        break;
                    }
                    //获取弹幕数据
                    List<DanmuBean> danmus = QueueManager.Instance.Get(cuid,uid);

                    if (danmus == null)
                    {
                        ret.Data = new { ret = "error", type =6, desc = "data is null" };
                    }
                    else
                    {
                        //var list = danmus.Select(t => new { Type = t.Type, Content = t.Content, Live = t.Live,Time = t.DateTime.ToString() });
                        var list = danmus.Select(t => new { Content = t.Content,Time = t.DateTime.ToString() });
                        ret.Data = new { ret = "ok", cuid = cuid, data = list, type = 0 };
                    }
                }
                catch
                {
                    ret.Data = new { ret = "error", type = 1, desc = "error happen" };
                } 
            } while (false);

            return ret;
        } 
        #endregion

        #region 统计节目热度 +PutHonChn
        /// <summary>
        /// 统计节目热度
        /// </summary>
        /// <param name="preCuid">前一播放节目唯一编号</param>
        /// <param name="curCuid">当前播放节目唯一标号</param>
        /// <returns>返回统计状态</returns>
        public JsonResult PutHonChn(string preCuid, string curCuid)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                do
                {
                    //判断当前播放节目唯一编号是否存在
                    if (string.IsNullOrWhiteSpace(curCuid))
                    {
                        ret.Data = new { ret = "error", type = 2, decs = "curCuid is null" };
                        break;
                    }

                    //操作前一播放节目,如果为null就认为客户端STB是开机时的统计节目热度操作
                    if (!string.IsNullOrWhiteSpace(preCuid))
                    {
                        T_HotChn preChn = db.T_HotChns.Find(preCuid);
                        if (preChn == null)
                        {
                            ret.Data = new { ret = "error", type = 3, decs = "preCuid is not exists" };
                            break;
                        }
                        if (preChn.HeatValue > 0)
                        {
                            preChn.HeatValue--;
                            db.Entry<T_HotChn>(preChn).State = EntityState.Modified;
                        }
                    }

                    //判断当前cuid是否合法
                    if (!CuidUtils.cuidExists(curCuid))
                    {
                        ret.Data = new { ret = "error", type = 4, decs = "curCuid is invalid" };
                        break;
                    }

                    //操作当前播放节目
                    T_HotChn curChn = db.T_HotChns.Where(t => t.Cuid == curCuid).FirstOrDefault();
                    if (curChn == null)//创建T_HotChn
                    {
                        ret.Data = new { ret = "error", type = 5, decs = "curCuid is not exists" };
                        break;
                    }
                    else//对HeatValue++
                    {
                        curChn.HeatValue++;
                        db.Entry<T_HotChn>(curChn).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    //返回统计状态
                    ret.Data = new { ret = "ok", type = 0 };
                } while (false);
            }
            catch
            {
                ret.Data = new { ret = "error", type = 1, decs = "error happen" };
            }

            return ret;
        }
        #endregion

        #region 获取节目热度列表 +GetHonChnList
        /// <summary>
        /// 获取节目热度列表
        /// </summary>]
        /// <param name="area">区域</param>
        /// <returns>返回热度最高的节目列表给请求客户端</returns>
        [HttpGet]
        public JsonResult GetHonChnList(string area)
        {
            JsonResult ret = new JsonResult();
            ret.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                //获取客户端可以抓取列列长度
                int cnt = SettingUtils.GetHotChnNum();

                //操作表T_HotChn
                var hotChnList = db.T_HotChns.Where(t => t.Area == area).OrderByDescending(t => t.HeatValue).Take(cnt).ToList()
                    .Select(t => new { cuid=t.Cuid,chn=t.ChnInfo,v=t.HeatValue});

                ret.Data = new { ret = "ok", data = hotChnList, type = 0 };
            }
            catch
            {
                ret.Data = new { ret = "error", type = 1 };
            }

            return ret;
        }
        #endregion

        #region 获取反向代理时的客户端的IP地址 getClientIP
        /// <summary>
        /// 获取反向代理时的客户端的IP地址
        /// </summary>
        /// <returns>返回客户端真实IP</returns>
        private string getClientIP()
        {
            HttpRequestBase request = HttpContext.Request;

            string ip = request.Headers.Get("x-forwarded-for");

            if (ip == null || ip.Length == 0 || string.Equals("unknown", ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Headers.Get("Proxy-Client-IP");
            }
            if (ip == null || ip.Length == 0 || string.Equals("unknown", ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.Headers.Get("WL-Proxy-Client-IP");

            }
            if (ip == null || ip.Length == 0 || string.Equals("unknown", ip, StringComparison.OrdinalIgnoreCase))
            {
                ip = request.UserHostAddress;
            }
            return ip;
        }
        #endregion
    }
}
