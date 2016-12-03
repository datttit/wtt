using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ConfigController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: Config
        public ActionResult Index()
        {
            var data = db.Configs.FirstOrDefault();
            return View(data);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateConfig(Config model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var _config = db.Configs.Find(model.ConfigId);
            if (_config != null)
            {
                _config.PageTitle = model.PageTitle ?? null;
                _config.PageDescription = model.PageDescription ?? null;
                _config.BgColorHeader = model.BgColorHeader ?? null;
                _config.BgImageHeader = model.BgImageHeader ?? null;
                _config.BgColorPanel = model.BgColorPanel ?? null;
                _config.BgImagePanel = model.BgImagePanel ?? null;
                _config.BgColorFooter = model.BgColorFooter ?? null;
                _config.BgImageFooter = model.BgImageFooter ?? null;
                _config.FooterContent = model.FooterContent ?? null;
                db.SaveChanges();
                TempData["Updated"] = "Cập nhật cấu hình thành công";
                return Redirect("~/Config");
            }
            return View(_config);
        }
        //

        public ActionResult SaveUploadedFile()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString()+DateTime.Now.Day.ToString();
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "Configs");

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "../Images/WallImages/Configs/" + _fileName;
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Có lỗi khi lưu tệp tin" });
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}