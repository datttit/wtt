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
using Helpers;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AlbumsController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();

        // GET: Albums
        public ActionResult Index(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = db.Albums.OrderByDescending(x=>x.CreateDate).ToList();
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public ActionResult GetAllAlbums(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = db.Albums.Where(x=>x.Published == true).OrderByDescending(x => x.CreateDate).ToList();
            return View(data.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult AddNewAlBum()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewAlBum(NewAlbum model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("AdminAddNewAlBum");
            }
            try
            {
                Album _newAlbum = new Album();
                _newAlbum.AlbumName = model.AlbumName ?? null;
                _newAlbum.IsAlbumTop = model.IsAlbumTop;
                _newAlbum.CreateDate = DateTime.Now;
                _newAlbum.Published = true;
                db.Albums.Add(_newAlbum);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới thư viện ảnh.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới thư viện ảnh.");
                return RedirectToRoute("AdminAddNewAlBum");
            }
            
            return RedirectToRoute("AdminAddNewAlBum");
        }

        public async Task<ActionResult> EditAlbum(int? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            Album _album = await db.Albums.FindAsync(id);
            if (_album == null)
            {
                return View(_album);
            }
            var getAlbum = new EditAlbum()
            {
                AlbumId = _album.AlbumId,
                AlbumName =_album.AlbumName,
                IsAlbumTop = (bool)_album.IsAlbumTop,
                Published = (bool)_album.Published
            };

            return View(getAlbum);
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAlbum(EditAlbum model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditAlbum");
            }
            var _album = await db.Albums.FindAsync(model.AlbumId);
            try
            {
                if (_album != null)
                {
                    _album.AlbumName = model.AlbumName ?? null;
                    _album.IsAlbumTop = model.IsAlbumTop;
                    _album.CreateDate = DateTime.Now;
                    _album.Published = model.Published;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa thư viện ảnh.");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật thư viện ảnh thành công.";
            return RedirectToRoute("AdminManagerAlbum");
        }

        public ActionResult DeleteAlbum(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Album _album = db.Albums.Find(id);
            if (_album == null)
            {
                return View();
            }
            return View(_album);
        }

        [HttpPost, ActionName("DeleteAlbum")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAlbumConfirmed(long? id)
        {
            Album _album = await db.Albums.FindAsync(id);
            if (_album == null)
            {
                return View();
            }
            if (_album.AlbumImages.Count > 0)
            {
                TempData["Error"] = "Thư viện này đang chứa nhiều ảnh. Bạn vui lòng xóa ảnh trước khi xóa thư viện ảnh.";
                return RedirectToAction("DeleteAlbum");
            }

            db.Albums.Remove(_album);
            await db.SaveChangesAsync();
            TempData["Updated"] = "Xóa bài viết thành công";
            return RedirectToRoute("AdminManagerAlbum");
        }


        public ActionResult AddImageGallery(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Album _album = db.Albums.Find(id);
            if (_album == null)
            {
                return View(_album);
            }

            return View(_album);
        }

        //ChangeImageAlbums

        public ActionResult SaveImageAlbums(string AlbumName, int? AlbumId)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            string _forderAlbum = "";

            if (AlbumName != null && AlbumName != "")
	        {
		        _forderAlbum = Configs.unicodeToNoMark(AlbumName);
            }
            else
            {
                _forderAlbum = "ExampleForder";
            }
            
            try
            {
                int i = 1;
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\{1}", Server.MapPath(@"\"), _forderAlbum));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString());

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = file.FileName;
                        fPath = string.Format("/Images/{0}/{1}", _forderAlbum, _fileName); 
                    }


                    // luu ảnh vào csdl
                    AlbumImage _newAlbumImage = new AlbumImage();
                    _newAlbumImage.AlbumId = AlbumId ?? (int?)null;
                    _newAlbumImage.ImageFileName = fName ?? null;
                    _newAlbumImage.ImagePosition = i;
                    _newAlbumImage.ImageUrl = fPath ?? null;
                    db.AlbumImages.Add(_newAlbumImage);
                    db.SaveChanges();
                    i++;

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

        public ActionResult EditImageGallery(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            AlbumImage _img = db.AlbumImages.Find(id);
            if (_img == null)
            {
                return View(_img);                
            }
            return View(_img);

        }

        public ActionResult ChangeImageAlbums(string AlbumName, int? ImageId)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            string _forderAlbum = "";

            if (AlbumName != null && AlbumName != "")
            {
                _forderAlbum = Configs.unicodeToNoMark(AlbumName);
            }
            else
            {
                _forderAlbum = "ExampleForder";
            }

            try
            {
                int i = 1;
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\{1}", Server.MapPath(@"\"), _forderAlbum));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString());

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = file.FileName;
                        fPath = string.Format("/Images/{0}/{1}", _forderAlbum, _fileName);
                    }


                    // // Cập nhạt tệp tin đính kèm  mới vào csdl
                    var _imgAttach = db.AlbumImages.Find(ImageId);
                    if (_imgAttach != null)
                    {
                        // Xóa file đính kèm cũ và cập nhật file đính kèm mới
                        string path = Server.MapPath(_imgAttach.ImageUrl);
                        FileInfo fileInfo = new FileInfo(path);
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                        _imgAttach.ImageFileName = fName ?? null;
                        _imgAttach.ImageUrl = fPath ?? null;
                        db.SaveChanges();
                    }
                    i++;

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { id = ImageId, Message = fName });
            }
            else
            {
                return Json(new { Message = "Có lỗi khi lưu tệp tin" });
            }
        }

        //DeleleImageGallery
        public ActionResult DeleleImageGallery(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            AlbumImage _albumImage = db.AlbumImages.Find(id);
            if (_albumImage == null)
            {
                return View(_albumImage);
            }
            return View(_albumImage);
        }

        [HttpPost, ActionName("DeleleImageGallery")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleleImageGalleryConfirmed(long? id)
        {
            AlbumImage _albumImage = db.AlbumImages.Find(id);
            if (_albumImage == null)
            {
                return View();
            }
            int? _idAlbum = _albumImage.AlbumId;
            try
            {
                // Xóa file đính kèm luu trên máy chủ và xóa file đính kèm của tài liệu.
                string path = Server.MapPath(_albumImage.ImageUrl);
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                db.AlbumImages.Remove(_albumImage);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Xóa tệp đính kèm thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa tệp đính kèm";
                return RedirectToAction("DeleleImageGallery");
            }

            return RedirectToAction("AddImageGallery", new { id = _idAlbum });
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