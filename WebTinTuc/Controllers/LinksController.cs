using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;
using Helpers;
using System.Threading.Tasks;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LinksController : ApplicationBaseController
    {
        public WebTinTucEntities db = new WebTinTucEntities();
        // GET: Links
        public ActionResult Index(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = (from v in db.LienKets select v).ToList();
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddNewLink()
        {
            ViewBag.Typetarget = Configs.ListTargetLink();
            return View();
        }

        public async Task<ActionResult> EditLink(int? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            LienKet _link = await db.LienKets.FindAsync(id);
            if (_link == null)
            {
                return View(_link);
            }
            var getLink = new LienketModel()
            {
                LienKetId = _link.LienKetId,
                LienKetName = _link.LienKetName,
                LienKetUrl = _link.LienKetUrl,
                Target = _link.Target,
                ImageUrl = _link.ImageUrl,
                IsOption = (bool)_link.IsOption                
            };
            ViewBag.Typetarget = Configs.ListTargetLink();
            return View(getLink);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLink(LienketModel model)
        {
            int? _id = 0;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditLink");
            }
            try
            {
                var _link = await db.LienKets.FindAsync(model.LienKetId);
                if (_link != null)
                {
                    if (_link.ImageUrl != null)
                    {
                        if (_link.ImageUrl != model.ImageUrl)
                        {
                            // xóa file ảnh cũ
                            string path = Server.MapPath(_link.ImageUrl);
                            FileInfo fileInfo = new FileInfo(path);
                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                            }
                        }
                    }
                    
                    _id = _link.LienKetId;
                    _link.LienKetName = model.LienKetName ?? null;
                    _link.LienKetUrl = model.LienKetUrl ?? null;
                    _link.IsOption = model.IsOption;
                    if (model.IsOption == false)
                    {
                        _link.ImageUrl = model.ImageUrl ?? null;
                        _link.Target = model.Target ?? null;
                    }
                    else
                    {
                        _link.ImageUrl = null;
                        _link.Target = null;
                    }
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Cập nhật liên kết thành công";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi cập nhật liên kết");
                return RedirectToAction("EditLink", new { id = _id });
            }
            return RedirectToAction("EditLink", new { id = _id });
        }



        //SaveUploadedFile
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
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "lienket");

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "/Images/WallImages/lienket/" + _fileName;
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


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewLink(LienketModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewLink");
            }
            try
            {
                LienKet _newLienKet = new LienKet();
                _newLienKet.LienKetName = model.LienKetName ?? null;
                _newLienKet.LienKetUrl = model.LienKetUrl ?? null;
                _newLienKet.IsOption = model.IsOption;
                if (model.IsOption == false)
                {
                    _newLienKet.ImageUrl = model.ImageUrl ?? null;
                    _newLienKet.Target = model.Target ?? null;
                }
                else
                {
                    _newLienKet.ImageUrl = null;
                    _newLienKet.Target = null;
                }
                db.LienKets.Add(_newLienKet);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới liên kết.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới liên kết");
                return RedirectToAction("AddNewLink");
            }
            return RedirectToAction("AddNewLink");
        }


        public ActionResult DeleteLink(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            LienKet _link = db.LienKets.Find(id);
            if (_link == null)
            {
                return View();
            }
            return View(_link);
        }

        [HttpPost, ActionName("DeleteLink")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteLinkConfirmed(int? id)
        {
            try
            {
                LienKet _lienket = await db.LienKets.FindAsync(id);
                if (_lienket == null)
                {
                    
                    return View();
                }
                if (_lienket.ImageUrl != null)
                {
                    // xóa file ảnh cũ
                    string path = Server.MapPath(_lienket.ImageUrl);
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }

                db.LienKets.Remove(_lienket);
                await db.SaveChangesAsync();
                TempData["Deleted"] = "Xóa liên kết thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa liên kết.";
                return RedirectToAction("DeleteLink");
            }

            return RedirectToAction("AddNewLink");
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