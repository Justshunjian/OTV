using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;

namespace Otv.Utils
{
    public class Email
    {
        public string To { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Repeat { get; set; }
    }
    /// <summary>
    /// 邮件发送管理类
    /// </summary>
    public class EmailHandler
    {
        private HttpServerUtilityBase Server;
        #region EmailHandler单例设计模式
        /// <summary>
        /// 邮件发送消息实体对象链表
        /// </summary>
        private readonly List<Email> m_list;
        /// <summary>
        /// 邮件发送线程对象
        /// </summary>
        private readonly Thread m_thread;
        /// <summary>
        /// 线程信号量
        /// </summary>
        private readonly ManualResetEvent m_threadEvent;
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly Object m_lockObj;

        private static readonly EmailHandler instance = new EmailHandler();
        
        private EmailHandler()
        {
            this.m_lockObj = new object();
            this.m_threadEvent = new ManualResetEvent(false);
            this.m_list = new List<Email>();

            this.m_thread = new Thread(this.execute);
            this.m_thread.Start();//启动线程
        } 
        #endregion

        #region 发送邮件 SendEmail
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sentModel">邮件发送方参数</param>
        /// <param name="recvModel">邮件接收方参数</param>
        /// <returns>返回发送邮箱的结果</returns>
        private bool SendEmail(Email email)
        {
            bool ret = true;
            MailMessage mailMessage = new MailMessage(); // 发送人和收件人
            try
            {
                // 设置发送方的邮件信息,例如使用网易的smtp
                string smtpServer = "smtp.163.com"; //SMTP服务器
                string mailFrom = "App2public@163.com"; //登陆用户名
                string userPassword = "qwertgfdsa";//登陆密码
                // 邮件服务设置
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
                smtpClient.Host = smtpServer; //指定SMTP服务器
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);//用户名和密码

                // 发送邮件设置
                mailMessage.From = new MailAddress(mailFrom,"Otv 管理员");

                mailMessage.To.Add(email.To);//添加收件方邮件地址

                mailMessage.Subject = email.Title;//主题
                mailMessage.Body = email.Content;//内容
                mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
                mailMessage.IsBodyHtml = true;//设置为HTML格式
                mailMessage.Priority = MailPriority.Normal;//优先级

                smtpClient.Send(mailMessage); // 发送邮件

              }
            catch (Exception)
            {
                ret = false;
            }
            finally
            {
                mailMessage.Dispose();
            }

            return ret;
        }
        #endregion

        #region 发送邮件线程执行方法 execute
        /// <summary>
        /// 发送邮件线程执行方法
        /// </summary>
        private void execute()
        {
            do
            {
                bool ret = false;

                //线程挂起，等待接收数据
                this.m_threadEvent.WaitOne();
                //从链表中取出邮件发送方的数据

                try
                {
                    Email eamil = this.m_list[0];
                    if (eamil != null)
                    {
                        //发送邮件
                        ret = SendEmail(eamil);
                        if (ret)
                        {
                            System.Diagnostics.EventLog.WriteEntry(eamil.Title + " 发送成功", "内容:\n" + eamil.Content);
                        }
                        else
                        {
                            System.Diagnostics.EventLog.WriteEntry(eamil.Title + " 发送失败", "内容:\n" + eamil.Content);
                        }

                        lock (this.m_lockObj)
                        {
                            //移除
                            this.m_list.Remove(eamil);

                            //如果发送失败，进行再次发送
                            if (!ret)
                            {
                                //重复发送3次，超过3次就被当做发送失败，停止发送
                                if (eamil.Repeat < 3)
                                {
                                    //追加数据
                                    //插入到List最后
                                    this.m_list.Add(eamil);

                                    try
                                    {
                                        Thread.Sleep(1000);//延时1秒发送下一封邮件
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                                else
                                {
                                    eamil.Repeat++;
                                }

                            }
                        }

                        //终止线程
                        if (this.m_list.Count == 0)
                        {
                            this.m_threadEvent.Reset();
                        }
                    }
                    
                }
                catch (Exception)
                {
                    
                }
            } while (true);
        } 
        #endregion

        #region 获取EmailHandler实例 +Instance
        /// <summary>
        /// 获取实例
        /// </summary>
        public static EmailHandler Instance { get { return instance; } } 
	    #endregion

        #region 添加待发送邮件信息 +AppendMail
        /// <summary>
        /// 添加待发送邮件信息
        /// </summary>
        /// <param name="mail">接收方信息</param>
        public void AppendMail(Email mail, HttpServerUtilityBase Server)
        {
            this.Server = Server;
            lock (this.m_lockObj)
            {
                this.m_list.Add(mail);
                if (this.m_list.Count != 0)
                {
                    this.m_threadEvent.Set();
                }
            }
        } 
        #endregion
        
    }
}