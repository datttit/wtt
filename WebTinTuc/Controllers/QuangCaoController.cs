using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class QuangCaoController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: QuangCao
        public ActionResult Index()
        {
            ViewBag.TypeAdv = Configs.ListTypAdv();
            return View();
        }

        //ThemMoiQuangCao
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ThemMoiQuangCao(AdvModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            try
            {
                Adv _newAdv = new Adv();
                _newAdv.AdvTitle = model.AdvTitle ?? null;
                _newAdv.TypeAdv = model.TypeAdv ?? null;
                _newAdv.Published = model.Published;
                db.Advs.Add(_newAdv);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới Quảng cáo.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới quảng cáo");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult DsQuangCao(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = (from q in db.Advs select q).ToList();
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> SuaQuangCao(int? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            Adv _adv = await db.Advs.FindAsync(id);
            if (_adv == null)
            {
                return View(_adv);
            }
            var getQuangCao = new EditAdvModel()
            {
                AdvId = _adv.AdvId,
                AdvTitle = _adv.AdvTitle,
                Published = (bool)_adv.Published,
                TypeAdv = _adv.TypeAdv
            };
            ViewBag.TypeAdv = Configs.ListTypAdv();
            return View(getQuangCao);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SuaQuangCao(EditAdvModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("SuaQuangCao");
            }
            try
            {
                var _adv = await db.Advs.FindAsync(model.AdvId);
                if (_adv != null)
                {
                    _adv.AdvTitle = model.AdvTitle ?? null;
                    _adv.TypeAdv = model.TypeAdv ?? null;
                    _adv.Published = model.Published;
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Cập nhật quảng cáo thành công";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi cập nhật quảng cáo");
                return RedirectToAction("SuaQuangCao");
            }
            return RedirectToAction("SuaQuangCao");
        }

        public ActionResult XoaQuangCao(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Adv _adv = db.Advs.Find(id);
            if (_adv == null)
            {
                return View();
            }
            return View(_adv);
        }

        [HttpPost, ActionName("XoaQuangCao")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> XoaQuangCaoConfirmed(int? id)
        {
            try
            {
                Adv _adv = await db.Advs.FindAsync(id);
                if (_adv == null)
                {
                    return View();
                }

                db.Advs.Remove(_adv);
                await db.SaveChangesAsync();
                TempData["Deleted"] = "Xóa quảng cáo thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa quảng cáo.";
                return RedirectToAction("XoaQuangCao");
            }

            return RedirectToAction("DsQuangCao");
        }

        public ActionResult ThemNoiDungQuangCao()
        {
            List<SelectListItem> xx = db.Advs.Select(x => new SelectListItem() { Value = x.AdvId.ToString(), Text = x.AdvTitle }).ToList();
            ViewBag.AdvId = xx;
            ViewBag.Typetarget = Configs.ListTargetLink();
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ThemNoiDungQuangCao(AdvContentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ThemNoiDungQuangCao");
            }
            try
            {
                AdvContent _newAdvContent = new AdvContent();
                _newAdvContent.AdvId = (int?)model.AdvId ?? (int?)null;
                _newAdvContent.ImgUrl = model.ImgUrl ?? null;
                _newAdvContent.Link = model.Link ?? null;
                _newAdvContent.Target = model.Target ?? null;
                db.AdvContents.Add(_newAdvContent);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới nội dung vào quảng cáo.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới");
                return RedirectToAction("ThemNoiDungQuangCao");
            }
            return RedirectToAction("ThemNoiDungQuangCao");
        }

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
                        //string strDay = DateTime.Now.ToString("yyyyMMdd");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "quangcao");

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "/Images/WallImages/quangcao/" + _fileName;
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

        public ActionResult DsNoiDungQuangCao(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            var _adv = db.Advs.Find(id);
            if (_adv == null)
            {
                return View(_adv);
            }
            return View(_adv);
        }

        public async Task<ActionResult> SuaNoiDungQC(long? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            AdvContent _advContent = await db.AdvContents.FindAsync(id);
            if (_advContent == null)
            {
                return View(_advContent);
            }
            var getNDQuangCao = new EditAdvContentModel()
            {
                AdvContentId = _advContent.AdvContentId,
                AdvId = (int)_advContent.AdvId,
                ImgUrl = _advContent.ImgUrl,
                Link = _advContent.Link,
                Target = _advContent.Target
            };
            List<SelectListItem> xx = db.Advs.Select(x => new SelectListItem() { Value = x.AdvId.ToString(), Text = x.AdvTitle }).ToList();
            ViewBag.AdvId = xx;
            ViewBag.Typetarget = Configs.ListTargetLink();
            return View(getNDQuangCao);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SuaNoiDungQC(EditAdvContentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("SuaNoiDungQC");
            }
            try
            {
                var _advContent = await db.AdvContents.FindAsync(model.AdvContentId);
                if (_advContent != null)
                {
                    if (_advContent.ImgUrl != model.ImgUrl)
                    {
                        // xóa file ảnh cũ
                        string path = Server.MapPath(_advContent.ImgUrl);
                        FileInfo fileInfo = new FileInfo(path);
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                    }                    
                    // cập nhật ảnh mới
                    _advContent.AdvId = (int?)model.AdvId ?? (int?)null;
                    _advContent.ImgUrl = model.ImgUrl ?? null;
                    _advContent.Link = model.Link ?? null;
                    _advContent.Target = model.Target ?? null;
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Cập nhật nội dung quảng cáo thành công";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi cập nhật nội dung quảng cáo");
                return RedirectToAction("SuaNoiDungQC");
            }
            return RedirectToAction("SuaNoiDungQC");
        }

        public ActionResult XoaNoiDungQC(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            AdvContent _advContent = db.AdvContents.Find(id);
            if (_advContent == null)
            {
                return View();
            }
            return View(_advContent);
        }

        [HttpPost, ActionName("XoaNoiDungQC")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> XoaNoiDungQCConfirmed(long? id)
        {
            int AdvId = 0;
            try
            {
                AdvContent _advContent = await db.AdvContents.FindAsync(id);
                if (_advContent == null)
                {
                    return View();
                }
                AdvId = (int)_advContent.AdvId;
                // xóa file ảnh cũ
                string path = Server.MapPath(_advContent.ImgUrl);
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                db.AdvContents.Remove(_advContent);
                await db.SaveChangesAsync();
                TempData["Deleted"] = "Xóa nội dung quảng cáo thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa nội dung quảng cáo.";
                return RedirectToAction("XoaNoiDungQC");
            }

            return RedirectToAction("DsNoiDungQuangCao", new { id = AdvId });
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