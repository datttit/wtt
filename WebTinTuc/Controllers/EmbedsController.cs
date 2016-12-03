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
    public class EmbedsController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: Embeds
        public ActionResult Index()
        {
            var data = db.Embeds.FirstOrDefault();
            if (data == null)
            {
                return RedirectToAction("AddNewEmbed");
            }
            var getEmbed = new EmbebModel() {
                EmbedId = data.EmbedId,
                EmbedName = data.EmbedName,
                EmbedFile = data.EmbedFile,
                EmbebOption = (bool)data.EmbedOption,
                EmbebContent = data.EmbedContent
            };
            return View(getEmbed);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateEmbed(EmbebModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var _embed = await db.Embeds.FindAsync(model.EmbedId);
            try
            {
                if (_embed != null)
                {
                    _embed.EmbedName = model.EmbedName ?? null;
                    _embed.EmbedFile = model.EmbedFile ?? null;
                    _embed.CreateDate = DateTime.Now;
                    _embed.EmbedOption = model.EmbebOption;
                    _embed.EmbedContent = model.EmbebOption == true ? model.EmbebContent ?? null : null;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra tải tệp swf.");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật thông báo nổi bật thành công.";
            return RedirectToAction("Index");
        }

        public ActionResult AddNewEmbed()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewEmbed(EmbebModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewEmbed");
            }
            try
            {
                Embed _newEmbed = new Embed();
                _newEmbed.EmbedName = model.EmbedName ?? null;
                _newEmbed.EmbedFile = model.EmbedFile ?? null;
                _newEmbed.CreateDate = DateTime.Now;
                _newEmbed.EmbedOption = model.EmbebOption;
                _newEmbed.EmbedContent = model.EmbebOption == true ? model.EmbebContent ?? null : null;
                db.Embeds.Add(_newEmbed);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới thông báo.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới thông báo.");
                return RedirectToAction("AddNewEmbed");
            }

            return RedirectToAction("AddNewEmbed");
        }

        public ActionResult SaveFileFlash()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            
            try
            {
                int i = 1;
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}SaveFile\\{1}", Server.MapPath(@"\"), "Video"));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString());

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = file.FileName;
                        fPath = string.Format("/SaveFile/{0}/{1}", "Video", _fileName);
                    }
                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Message = fPath });
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