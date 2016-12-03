using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Helpers
{
    public static class Configs
    {
        //public static WebTinTucEntities db = new WebTinTucEntities();
        #region ### delete
        //public static Config LoadConfigs()
        //{
        //    //string query = "select top 1 * from Configs";
        //    //var _config = db.Database.SqlQuery<Config>(query);
        //    var _config = from x in db.Configs select x;
        //    return _config.FirstOrDefault();
        //}

        //public static IEnumerable<Menu> LoadMenus()
        //{
        //    //var query = "select * from Menus";
        //    //IEnumerable<Menu> data = null;
        //    //if (db.Database.SqlQuery<Menu>(query) != null)
        //    //{
        //    //    data = db.Database.SqlQuery<Menu>(query);
        //    //}
        //    var data = from x in db.Menus select x;
        //    return data;
        //}

        //public static IEnumerable<Category> LoadCategories()
        //{
        //    //var query = "select * from Categories";
        //    //var data = db.Database.SqlQuery<Category>(query);
        //    var data = from x in db.Categories select x;
        //    return data;
        //}

        //public static IEnumerable<Document> LoadDocuments()
        //{
        //    //var query = "select * from Documents";
        //    //var data = db.Database.SqlQuery<Document>(query);
        //    var data = from x in db.Documents select x;
        //    return data;
        //}

        //public static ConfigAccount LoadConfigAccount()
        //{
        //    //string query = "SELECT top(1) * FROM ConfigAccount";
        //    //var _config = db.Database.SqlQuery<ConfigAccount>(query);
        //    var _config = from x in db.ConfigAccounts select x;
        //    return _config.FirstOrDefault();
        //}

        //public static IEnumerable<Module> LoadModules()
        //{
        //    string query = "select * from Modules";
        //    var _modules = db.Database.SqlQuery<Module>(query);
        //    return _modules;
        //}

        //public static IEnumerable<DocumentDetail> LoadDocumentDetail()
        //{
        //    //string query = "select * from DocumentDetails";
        //    //var _dd = db.Database.SqlQuery<DocumentDetail>(query);
        //    var data = from x in db.DocumentDetails select x;
        //    return data;
        //}
        #endregion

        public static string GetNgay()
        {
            var day = DateTime.Now.DayOfWeek;
            string thu = "";
            switch (day)
            {
                case DayOfWeek.Friday:
                    thu = "Thứ 6";
                    break;
                case DayOfWeek.Monday:
                    thu = "Thứ 2";
                    break;
                case DayOfWeek.Saturday:
                    thu = "Thứ 7";
                    break;
                case DayOfWeek.Sunday:
                    thu = "Chủ nhật";
                    break;
                case DayOfWeek.Thursday:
                    thu = "Thứ 5";
                    break;
                case DayOfWeek.Tuesday:
                    thu = "Thứ 3";
                    break;
                case DayOfWeek.Wednesday:
                    thu = "Thứ 4";
                    break;
                default:
                    break;
            }
            string xxx = string.Format("{0}, Ngày {1}.", thu, DateTime.Now.ToString("dd/MM/yyyy"));
            return xxx;
        }

        public static bool Sendmail(string from, string pass, string to, string mailHost, int mailPort, bool mailEnableSsl, int MailTimeout, string topic, string content)
        {
            try
            {
                var fromAddress = from;
                var toAddress = to;
                //Password of your gmail address
                string fromPassword = pass;
                // Passing the values and make a email formate to display
                string subject = topic;
                string body = content;
                // smtp settings
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromAddress);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = mailHost;//"smtp.gmail.com";
                    smtp.Port = mailPort;// 465;//587;
                    smtp.EnableSsl = mailEnableSsl;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                    smtp.Timeout = MailTimeout;
                }
                // Passing values to smtp object
                smtp.Send(message);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public const int ImageMinimumBytes = 512;
        public static bool IsImage(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static string unicodeToNoMark(string input)
        {
            input = input.ToLowerInvariant().Trim();
            if (input == null) return "";
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++)
            {
                input = input.Replace(a_u[i], a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            input = removeSpecialChar(input);
            input = input.Replace(" ", "-");
            input = input.Replace("--", "-");
            return input;
        }

        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”", "").Replace(".", "").Replace("%", "");
            return input;
        }

        public static string splitString(string input)
        {
            string strNew = unicodeToNoMark(input);
            return strNew.Replace("-", " ");
        }

        public static List<SelectListItem> ListPositionMenu()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            //Add select list item to list of selectlistitems
            newList.Add(new SelectListItem() { Value = "left", Text = "Menu bên trái" });
            newList.Add(new SelectListItem() { Value = "top", Text = "Menu bên trên" });
            newList.Add(new SelectListItem() { Value = "bottom", Text = "Menu bên dưới" });
            return newList;
        }

        public static List<SelectListItem> ListTypePost()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            //Add select list item to list of selectlistitems
            newList.Add(new SelectListItem() { Value = "new", Text = "Tin tức", Selected = true });
            newList.Add(new SelectListItem() { Value = "thongbao", Text = "Thông báo" });            
            newList.Add(new SelectListItem() { Value = "congvan", Text = "Công văn" });
            return newList;
        }

        public static List<SelectListItem> ListTypeModule()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "lienketlink", Text = "Liên kết bằng đường dẫn" });
            newList.Add(new SelectListItem() { Value = "lienkethinhanh", Text = "Liên kết bằng hình ảnh" });
            newList.Add(new SelectListItem() { Value = "quangcao1", Text = "Kiểu quảng cáo 1" });
            newList.Add(new SelectListItem() { Value = "quangcao2", Text = "Kiểu quảng cáo 2" });
            newList.Add(new SelectListItem() { Value = "video", Text = "Kiểu video" });
            newList.Add(new SelectListItem() { Value = "album", Text = "Kiểu album ảnh" });
            newList.Add(new SelectListItem() { Value = "contact", Text = "Kiểu liên hệ" });
            newList.Add(new SelectListItem() { Value = "congvan", Text = "Kiểu Công văn" });
            newList.Add(new SelectListItem() { Value = "thongbao", Text = "Kiểu thông báo" });
            newList.Add(new SelectListItem() { Value = "map", Text = "Kiểu bản đồ" });
            newList.Add(new SelectListItem() { Value = "custom", Text = "Tùy chỉnh" });
            return newList;
        }

        public static List<SelectListItem> ListTypAdv()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "quangcao1", Text = "Kiểu quảng cáo 1" });
            newList.Add(new SelectListItem() { Value = "quangcao2", Text = "Kiểu quảng cáo 2" });
            newList.Add(new SelectListItem() { Value = "quangcao3", Text = "Kiểu quảng cáo 3" });
            newList.Add(new SelectListItem() { Value = "quangcao4", Text = "Kiểu quảng cáo 4" });
            newList.Add(new SelectListItem() { Value = "quangcao5", Text = "Kiểu quảng cáo 5" });
            return newList;
        }

        public static List<SelectListItem> ListTargetLink()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "_target", Text = "Mở cửa sổ trang mới" });
            newList.Add(new SelectListItem() { Value = "_self", Text = "Mở trên trang hiện tại" });
            return newList;
        }

        public static List<SelectListItem> ListPosition()
        {
            List<SelectListItem> newList = new List<SelectListItem>();              
            newList.Add(new SelectListItem() { Value = "left", Text = "Vị trí bên trái" });
            newList.Add(new SelectListItem() { Value = "right", Text = "Vị trí bên phải" });
            return newList;
        }

        public static List<SelectListItem> ListLinhVucHoiDap()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "1", Text = "Văn Hóa – Xã hội" });
            newList.Add(new SelectListItem() { Value = "2", Text = "Kinh tế" });
            newList.Add(new SelectListItem() { Value = "3", Text = "Lao động thương binh – xã hội" });
            newList.Add(new SelectListItem() { Value = "4", Text = "Tài nguyên và môi trường" });
            newList.Add(new SelectListItem() { Value = "5", Text = "Giáo dục và đào tạo" });
            newList.Add(new SelectListItem() { Value = "6", Text = "Công – Nông – Lâm nghiệp" });
            newList.Add(new SelectListItem() { Value = "7", Text = "Tư pháp" });
            newList.Add(new SelectListItem() { Value = "8", Text = "Y tế" });
            newList.Add(new SelectListItem() { Value = "9", Text = "Lĩnh vực khác" });
            return newList;
        }

        public static List<SelectListItem> ListDonViTraLoi()
        {
            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Value = "1", Text = "Ban biên tập cổng thông tin điện tử" });
            newList.Add(new SelectListItem() { Value = "2", Text = "UBND Thị trấn Krông Năng" });
            newList.Add(new SelectListItem() { Value = "3", Text = "UBND xã Phú Xuân huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "4", Text = "UBND xã Phú Lộc huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "5", Text = "UBND xã Dliêya huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "6", Text = "UBND xã Ea Tân huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "7", Text = "UBND xã Ea Tóh huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "8", Text = "UBND xã Ea Tam huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "9", Text = "UBND xã Ea Dah huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "10", Text = "UBND xã Ea Hồ huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "11", Text = "UBND xã Ea Púk huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "12", Text = "UBND xã Cư K lông huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "13", Text = "UBND Xã Tam Giang huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "14", Text = "Văn phòng UBND và HĐND Huyện ( Ban tiếp công dân củ )" });
            newList.Add(new SelectListItem() { Value = "15", Text = "Phòng Tài Nguyên & Môi Trường huyện Krông Năng" });
            newList.Add(new SelectListItem() { Value = "16", Text = "Trung tâm phát triển quỹ đất huyện Krông Năng "});
            newList.Add(new SelectListItem() { Value = "17", Text = "Phòng giáo dục & đào tạo huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "18", Text = "Phòng kinh tế và hạ tầng huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "19", Text = "Phòng nội vụ huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "20", Text = "Phòng LĐTBXH huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "21", Text = "Phòng  y tế huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "22", Text = "Phòng văn hóa và thông tin huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "23", Text = "Phòng dân tộc huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "24", Text = "Phòng Tư Pháp huyện Krông Năng"});
            newList.Add(new SelectListItem() { Value = "25", Text = "Phòng nông nghiệp và phát triển nông thôn huyện Krông Năng"});
            return newList;
        }

        public static int maxRequestLength
        {
            get
            {
                HttpRuntimeSection section =
                   ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;

                if (section != null)
                    return section.MaxRequestLength; // Default Value
                else
                    return 4096 * 1024; // Default Value
            }
        }

        public static string PercentVote(double numberVote, double TotalVote)
        {
            return Math.Round((numberVote / TotalVote) * 100, 2).ToString() + "%";
        }

        public static string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }

        public static String DMYToMDY(String input)
        {
            return Regex.Replace(input,
            @"\b(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{2,4})\b",
            "${month}/${day}/${year}");
        }

        public static DateTime MDYToDMY(String input)
        {
            String x = Regex.Replace(input,
            @"(?<month>\d{1,2})/\b(?<day>\d{1,2})/(?<year>\d{2,4})\b",
            "${day}/${month}/${year}");
            return Convert.ToDateTime(x);
        }

        //public static void SetCookie(string key, string value, TimeSpan expires)
        //{
        //    HttpCookie encodedCookie = HttpSecureCookie.Encode(new HttpCookie(key, value));

        //    if (HttpContext.Current.Request.Cookies[key] != null)
        //    {
        //        var cookieOld = HttpContext.Current.Request.Cookies[key];
        //        cookieOld.Expires = DateTime.Now.Add(expires);
        //        cookieOld.Value = encodedCookie.Value;
        //        HttpContext.Current.Response.Cookies.Add(cookieOld);
        //    }
        //    else
        //    {
        //        encodedCookie.Expires = DateTime.Now.Add(expires);
        //        HttpContext.Current.Response.Cookies.Add(encodedCookie);
        //    }
        //}
        //public static string GetCookie(string key)
        //{
        //    string value = string.Empty;
        //    HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

        //    if (cookie != null)
        //    {
        //        // For security purpose, we need to encrypt the value.
        //        HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);
        //        value = decodedCookie.Value;
        //    }
        //    return value;
        //}

    }

    



    //#region Cookie
    //public static class CookieProtectionHelperWrapper
    //{

    //    private static MethodInfo _encode;
    //    private static MethodInfo _decode;

    //    static CookieProtectionHelperWrapper()
    //    {
    //        // obtaining a reference to System.Providers assembly
    //        Assembly systemWeb = typeof(HttpContext).Assembly;
    //        if (systemWeb == null)
    //        {
    //            throw new InvalidOperationException(
    //                "Unable to load System.Web.");
    //        }
    //        // obtaining a reference to the internal class CookieProtectionHelper
    //        Type cookieProtectionHelper = systemWeb.GetType(
    //                "System.Web.Security.CookieProtectionHelper");
    //        if (cookieProtectionHelper == null)
    //        {
    //            throw new InvalidOperationException(
    //                "Unable to get the internal class CookieProtectionHelper.");
    //        }
    //        // obtaining references to the methods of CookieProtectionHelper class
    //        _encode = cookieProtectionHelper.GetMethod(
    //                "Encode", BindingFlags.NonPublic | BindingFlags.Static);
    //        _decode = cookieProtectionHelper.GetMethod(
    //                "Decode", BindingFlags.NonPublic | BindingFlags.Static);

    //        if (_encode == null || _decode == null)
    //        {
    //            throw new InvalidOperationException(
    //                "Unable to get the methods to invoke.");
    //        }
    //    }

    //    public static string Encode(CookieProtection cookieProtection,
    //                                byte[] buf, int count)
    //    {
    //        return (string)_encode.Invoke(null,
    //                new object[] { cookieProtection, buf, count });
    //    }

    //    public static byte[] Decode(CookieProtection cookieProtection,
    //                                string data)
    //    {
    //        return (byte[])_decode.Invoke(null,
    //                new object[] { cookieProtection, data });
    //    }
    //}

    //public static class MachineKeyCryptography
    //{
    //    public static string Encode(string text, CookieProtection cookieProtection)
    //    {
    //        if (string.IsNullOrEmpty(text) || cookieProtection == CookieProtection.None)
    //        {
    //            return text;
    //        }
    //        byte[] buf = Encoding.UTF8.GetBytes(text);
    //        return CookieProtectionHelperWrapper.Encode(cookieProtection, buf, buf.Length);
    //    }

    //    public static string Decode(string text, CookieProtection cookieProtection)
    //    {
    //        if (string.IsNullOrEmpty(text))
    //        {
    //            return text;
    //        }
    //        byte[] buf;
    //        try
    //        {
    //            buf = CookieProtectionHelperWrapper.Decode(cookieProtection, text);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new System.Security.Cryptography.CryptographicException(
    //                "Unable to decode the text", ex.InnerException);
    //        }
    //        if (buf == null || buf.Length == 0)
    //        {
    //            throw new System.Security.Cryptography.CryptographicException(
    //                "Unable to decode the text");
    //        }
    //        return Encoding.UTF8.GetString(buf, 0, buf.Length);
    //    }
    //}

    //public static class HttpSecureCookie
    //{

    //    public static HttpCookie Encode(HttpCookie cookie)
    //    {
    //        return Encode(cookie, CookieProtection.All);
    //    }

    //    public static HttpCookie Encode(HttpCookie cookie,
    //                  CookieProtection cookieProtection)
    //    {
    //        HttpCookie encodedCookie = CloneCookie(cookie);
    //        encodedCookie.Value =
    //          MachineKeyCryptography.Encode(cookie.Value, cookieProtection);
    //        return encodedCookie;
    //    }

    //    public static HttpCookie Decode(HttpCookie cookie)
    //    {
    //        return Decode(cookie, CookieProtection.All);
    //    }

    //    public static HttpCookie Decode(HttpCookie cookie,
    //                  CookieProtection cookieProtection)
    //    {
    //        HttpCookie decodedCookie = CloneCookie(cookie);
    //        decodedCookie.Value =
    //          MachineKeyCryptography.Decode(cookie.Value, cookieProtection);
    //        return decodedCookie;
    //    }

    //    public static HttpCookie CloneCookie(HttpCookie cookie)
    //    {
    //        HttpCookie clonedCookie = new HttpCookie(cookie.Name, cookie.Value);
    //        clonedCookie.Domain = cookie.Domain;
    //        clonedCookie.Expires = cookie.Expires;
    //        clonedCookie.HttpOnly = cookie.HttpOnly;
    //        clonedCookie.Path = cookie.Path;
    //        clonedCookie.Secure = cookie.Secure;

    //        return clonedCookie;
    //    }
    //}
    //#endregion
}