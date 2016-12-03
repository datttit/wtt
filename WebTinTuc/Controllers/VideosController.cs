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
    public class VideosController : ApplicationBaseController
    {
        public WebTinTucEntities db = new WebTinTucEntities();
        // GET: Videos
        public ActionResult Index(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = (from v in db.Videos select v).ToList();

            data = data.OrderByDescending(x => x.CreatedDate).ToList();
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddNewVideo()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewVideo(VideoModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewVideo");
            }
            try
            {
                Video _newVideo = new Video();
                _newVideo.VideoTitle = model.VideoTitle ?? "Tên video";
                _newVideo.VideoUrl = model.VideoUrl ?? null;
                _newVideo.IsVideoTop = model.IsVideoTop;
                _newVideo.Published = model.Published;
                _newVideo.CreatedDate = DateTime.Now;
                _newVideo.VideoImage = model.VideoImage ?? null;
                db.Videos.Add(_newVideo);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới video.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới video");
                return RedirectToAction("AddNewVideo");
            }
            return RedirectToAction("AddNewVideo");
        }

        public async Task<ActionResult> EditVideo(int? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            Video _video = await db.Videos.FindAsync(id);
            if (_video == null)
            {
                return View(_video);
            }
            var getVideo = new VideoModel()
            {
                VideoId = _video.VideoId,
                VideoTitle = _video.VideoTitle,
                VideoUrl = _video.VideoUrl,
                IsVideoTop = (bool)_video.IsVideoTop,
                Published = (bool)_video.Published,
                VideoImage = _video.VideoImage
            };
            return View(getVideo);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditVideo(VideoModel model)
        {
            int? _id = 0;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditVideo");
            }
            try
            {
                var _video = await db.Videos.FindAsync(model.VideoId);
                if (_video != null)
                {
                    _id = _video.VideoId;
                    _video.VideoTitle = model.VideoTitle ?? "Tên video";
                    _video.VideoUrl = model.VideoUrl ?? null;
                    _video.IsVideoTop = model.IsVideoTop;
                    _video.Published = model.Published;
                    _video.CreatedDate = DateTime.Now;
                    _video.VideoImage = model.VideoImage ?? null;
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Cập nhật video thành công";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi cập nhật video");
                return RedirectToAction("EditVideo", new { id = _id });
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteVideo(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Video _video = db.Videos.Find(id);
            if (_video == null)
            {
                return View();
            }
            return View(_video);
        }

        [HttpPost, ActionName("DeleteVideo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteVideoConfirmed(int? id)
        {
            try
            {
                Video _video = await db.Videos.FindAsync(id);
                if (_video == null)
                {
                    return View();
                }
                if (_video.Published == true)
                {
                    TempData["Error"] = "Bạn không thể xóa video. <br />Video này đang được mở.";
                    return RedirectToAction("DeleteVideo");
                }
                db.Videos.Remove(_video);
                await db.SaveChangesAsync();
                TempData["Deleted"] = "Xóa video thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa quảng cáo.";
                return RedirectToAction("DeleteVideo");
            }

            return RedirectToAction("AddNewVideo");
        }

        //SaveUploadedFileImage
        public ActionResult SaveUploadedFileImage()
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
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "video");

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "/Images/WallImages/video/" + _fileName;
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

        public ActionResult SaveUploadedFileVideo()
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
                        var originalDirectory = new DirectoryInfo(string.Format("{0}SaveFile\\Video", Server.MapPath(@"\")));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString()+DateTime.Now.Day.ToString();
                        string strDay = DateTime.Now.ToString("yyyyMMdd");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), strDay);

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = string.Format("/SaveFile/video/{0}/{1}", strDay, _fileName);
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