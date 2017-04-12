using Otv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Otv.Utils
{
    public class SettingUtils
    {

        private static int HOT_NUM = 0;
        public enum SettingKeys
        {
            /// <summary>
            /// request hot chn list number
            /// </summary>
            HOT_CHN_COUNT,
            /// <summary>
            /// request danmu list number
            /// </summary>
            OTV_REQ_COUNT,
            /// <summary>
            /// hot chn reset quartz hour
            /// </summary>
            HOT_CHN_RESET_QUARTZ_HOUR,
            /// <summary>
            /// hot chn reset quartz minute
            /// </summary>
            HOT_CHN_RESET_QUARTZ_MINUTE,

        }

        public static string GetEnumStr(SettingKeys key)
        {
            return Enum.GetName(typeof(SettingKeys), key);
        }

        /// <summary>
        /// 获取节目热度列表设置的个数
        /// </summary>
        /// <returns></returns>
        public static int GetHotChnNum()
        {
            if(HOT_NUM == 0){
                try
                {
                    using(UsersContext context = new UsersContext()){
                        T_Setting setting = context.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_COUNT));
                        HOT_NUM = Int32.Parse(setting.Value);
                    }
                }
                catch (Exception)
                {
                    //默认列表长度为100
                    HOT_NUM = 100;
                }
            }

            return HOT_NUM;
        }
    }
}