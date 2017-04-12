using Otv.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Otv.Utils
{
    public class CuidUtils
    {
        /// <summary>
        /// 节目cuid集合
        /// </summary>
        private static Dictionary<string, ChnModel> sCuidLists = new Dictionary<string, ChnModel>();
        /// <summary>
        /// 机顶盒唯一标识集合
        /// </summary>
        public static List<string> stbUids = new List<string>();
        /// <summary>
        /// 手机客户端唯一标识集合
        /// </summary>
        public static List<string> phoneUids = new List<string>();

        #region  释放内存 release
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void release()
        {
            sCuidLists.Clear();
            stbUids.Clear();
            phoneUids.Clear();
        } 
        #endregion

        /// <summary>
        /// 判断机顶盒MAC标识是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool checkStbUid(string uid)
        {
            return stbUids.Contains(uid);
        }

        /// <summary>
        /// 添加机顶盒MAC标识
        /// </summary>
        /// <returns></returns>
        public static void AddStbUid(string uid)
        {
            stbUids.Add(uid);
        }

        /// <summary>
        /// 判断机顶盒唯一标识是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool checkPhoneUid(string uid)
        {
            return phoneUids.Contains(uid);
        }

        /// <summary>
        /// 生成机顶盒唯一标识
        /// </summary>
        /// <returns></returns>
        public static string getPhoneUid()
        {
            string uid = GuidTo16String();
            phoneUids.Add(uid);
            return uid;
        }

        /// <summary>
        /// 获取节目唯一标识
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string getCuid(ChnModel model )
        {
            //判断cuid存在对应的数据是否合法
            if (!modelExists(model))
            {
                string cuid = foreachCuidList(model);
                if (string.IsNullOrWhiteSpace(cuid))
                {
                    //如果没有找到cuid，自动生成一个cuid
                    cuid = GuidTo16String();

                    //如果是新节目，就添加到节目热度表中，并设置热度为0
                    using (UsersContext db = new UsersContext())
                    {
                        //操作当前播放节目
                        T_HotChn curChn = db.T_HotChns.Find(cuid);
                        if (curChn == null)//创建T_HotChn
                        {
                            curChn = new T_HotChn
                            {
                                Cuid = cuid,
                                ChnInfo = String.Format("[{0}][{1}][{2}][{3}][{4}][{5}]", model.Sat, model.CInfo,model.TvRadio, model.Frequency, model.Polar, model.Area),
                                Area = model.Area,
                                HeatValue = 0
                            };
                            db.T_HotChns.Add(curChn);
                        }
                        db.SaveChanges();
                    }
                    model.Cuid = cuid;
                    sCuidLists.Add(cuid, model);
                }
                return cuid;
            }

            return model.Cuid;
            
        }

        /// <summary>
        /// 判断cuid存在对应的数据是否合法
        /// </summary>
        /// <param name="cuid"></param>
        /// <returns></returns>
        public static bool cuidExists(string cuid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cuid))
                {
                    return false;
                }

                //判断该节目是否在节目cuid集合中
                if (sCuidLists.ContainsKey(cuid))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// 循环判断集合中是否有符合的数据
        /// </summary>
        /// <param name="model">节目信息</param>
        /// <returns>如果找到就返回对应的cuid，否则返回null</returns>
        private static string foreachCuidList(ChnModel model)
        {
            string cuid = null;
            //判断cuid中集合是否存在节目的信息
            foreach (KeyValuePair<String, ChnModel> kvp in sCuidLists)
            {
                ChnModel chnModel = kvp.Value;
                //判断数据是否正确
                if (model.Sat.Equals(chnModel.Sat) && model.CInfo.Equals(chnModel.CInfo) && model.Polar == chnModel.Polar)
                {
                    if (model.Frequency >= chnModel.Frequency - 14 && model.Frequency <= chnModel.Frequency + 14)
                    {
                        cuid = chnModel.Cuid;
                        break;
                    }
                }
            }

            return cuid;
        }

        /// <summary>
        /// 判断cuid存在对应的数据是否合法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static bool modelExists(ChnModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Cuid))
                {
                    return false;
                }

                //判断该节目是否在节目cuid集合中
                if (sCuidLists.ContainsKey(model.Cuid))
                {
                    ChnModel chnModel = null;
                    if (sCuidLists.TryGetValue(model.Cuid, out chnModel))
                    {
                        //判断数据是否正确
                        if (model.Sat.Equals(chnModel.Sat) && model.CInfo.Equals(chnModel.CInfo) && model.Polar == chnModel.Polar)
                        {
                            if (model.Frequency >= chnModel.Frequency - 14 && model.Frequency <= chnModel.Frequency + 14)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                 return false;
            }

            return false;
        }

        /// <summary>
        /// 由连字符分隔的32位数字
        /// </summary>
        /// <returns></returns>
        private static string GetGuid()
        {
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();
            return guid.ToString();
        }
        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <param name=\"guid\"></param>  
        /// <returns></returns>  
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        /// <summary>  
        /// 根据GUID获取19位的唯一数字序列  
        /// </summary>  
        /// <returns></returns>  
        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }  
    }
}