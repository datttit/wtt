using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using WebTinTuc.Models;
using Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebTinTuc.Controllers
{
    [Authorize]
    public class HoiDapsController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: admin/hoidap/linhvuc

        [Authorize(Roles = "Administrator")]
        public ActionResult Index(int? pg, string search)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var linhvucs = LoadLinhVucs().Where(x=>x.LinhVucChaId != null);
            if (linhvucs == null)
            {
                return View(linhvucs);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                linhvucs = linhvucs.Where(x => x.TenLinhVuc.ToLower().Contains(search));
                ViewBag.search = search;
            }

            linhvucs = linhvucs.OrderBy(x => x.LinhVucChaId);
            return View(linhvucs.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrator, Manager")]
        public ActionResult DanhSachCauHoiHoiDap(int? pg, string search, int? LinhVucId, string status)
        {
            string userid = User.Identity.GetUserId();
            var data = from s in db.HoiDapCauHois where s.DonViTiepNhanId == userid select s;
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TenCauHoi.ToLower().Contains(search));
                ViewBag.search = search;
            }

            if (LinhVucId == null) LinhVucId = 0; if (status == null) status = "";
            if (LinhVucId != null && LinhVucId != 0)
            {
                data = data.Where(t => t.LinhVucHoiDapId == LinhVucId);
                ViewBag.LinhVucId = LinhVucId;
            }

            if (status != null && status != "")
            {
                bool bstatus = Convert.ToBoolean(status);
                data = data.Where(s => s.TrangThaiCauHoi == bstatus);
                ViewBag.status = status;
            }

            data = data.OrderByDescending(x => x.NgayHoiDap);

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrator, Manager")]
        public ActionResult TraLoiCauHoi(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            HoiDapCauHois _cauhoi = db.HoiDapCauHois.Find(id);
            if (_cauhoi == null)
            {
                return View(_cauhoi);                
            }

            var getHoiDapModel = new TraLoiModel()
            {
                CauHoiId = _cauhoi.CauHoiId,
                TDTraLoi = _cauhoi.TDTraLoi,
                NDTraLoi = _cauhoi.NDTraLoi
            };
            ViewBag.TenCauHoi = _cauhoi.TenCauHoi;
            return View(getHoiDapModel);
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TraLoiCauHoi(TraLoiModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("TraLoiCauHoi");
            }
            try
            {
                string userid = User.Identity.GetUserId();
                var _cauhoi = await db.HoiDapCauHois.FindAsync(model.CauHoiId);
                if (_cauhoi != null)
                {
                    _cauhoi.TrangThaiCauHoi = true;
                    _cauhoi.TDTraLoi = model.TDTraLoi ?? null;
                    _cauhoi.NDTraLoi = model.NDTraLoi ?? null;
                    _cauhoi.NgayTraLoi = DateTime.Now;
                }
                await db.SaveChangesAsync();
                TempData["Updated"] = "Trả lời thành công.";
                return RedirectToAction("TraLoiCauHoi");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi trả lời.");
                return View(model);
            }
        }

        private IEnumerable<HoiDapLinhVuc> LoadLinhVucs()
        {
            var data = from x in db.HoiDapLinhVucs select x;
            return data;
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AddNewLinhVucHoiDap()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewLinhVucHoiDap(HoiDapLinhVucModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewLinhVucHoiDap");
            }
            try
            {
                HoiDapLinhVuc _newlinhvuc = new HoiDapLinhVuc();
                _newlinhvuc.TenLinhVuc = model.TenLinhVuc ?? null;
                _newlinhvuc.DiaChiTruyCap = model.DiaChiTruyCap ?? null;
                _newlinhvuc.LinhVucChaId = (int?)model.LinhVucChaId ?? null;
                _newlinhvuc.ViTriLinhVuc = (int?)model.ViTriLinhVuc ?? null;
                _newlinhvuc.Published = model.Published;
                db.HoiDapLinhVucs.Add(_newlinhvuc);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm lĩnh vực hỏi đáp mới thành công";
                return RedirectToAction("AddNewLinhVucHoiDap");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm lĩnh vực hỏi đáp.");
                return View(model);
            }
        }

        //EditLinhVucHoiDap
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> EditLinhVucHoiDap(int? id)
        {
            if (id == null || id == 0 || id == 1)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            HoiDapLinhVuc linhvuchoidap = await db.HoiDapLinhVucs.FindAsync(id);
            if (linhvuchoidap == null)
            {
                return View(linhvuchoidap);
            }
            var getLinhVucHoiDap = new HoiDapLinhVucModel()
            {
               LinhVucId = linhvuchoidap.LinhVucId,
               TenLinhVuc = linhvuchoidap.TenLinhVuc,
               DiaChiTruyCap = linhvuchoidap.DiaChiTruyCap,
               LinhVucChaId = (int)linhvuchoidap.LinhVucChaId,
               ViTriLinhVuc = (int)linhvuchoidap.ViTriLinhVuc,
               Published = (bool)linhvuchoidap.Published
               
            };
            return View(getLinhVucHoiDap);
        }

        // DeleteLinhVucHoiDap
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteLinhVucHoiDap(int? id)
        {
            if (id == null || id == 0 || id == 1)
            {
                return View();
            }
            HoiDapLinhVuc _linhvuchoidap = db.HoiDapLinhVucs.Find(id);
            if (_linhvuchoidap == null)
            {
                return View();
            }
            return View(_linhvuchoidap);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("DeleteLinhVucHoiDap")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteLinhVucHoiDapConfirmed(int? id)
        {
            HoiDapLinhVuc _linhvuchoidap = await db.HoiDapLinhVucs.FindAsync(id);
            if (_linhvuchoidap == null)
            {
                return View();
            }
            if (_linhvuchoidap.HoiDapLinhVucs1.Count() > 0)
            {
                TempData["Error"] = "Bạn không thể xóa danh mục này. <br /> Danh mục này chứa danh mục con khác. Vui lòng xóa danh mục con trước.";
                return RedirectToAction("DeleteLinhVucHoiDap");
            }

            if (_linhvuchoidap.HoiDapCauHois.Count() > 0)
            {
                TempData["Error"] = "Bạn không thể xóa danh mục này. <br /> Danh mục này đang chứa câu hỏi.";
                return RedirectToAction("DeleteLinhVucHoiDap");
            }

            db.HoiDapLinhVucs.Remove(_linhvuchoidap);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Xóa danh mục lĩnh vực thành công";
            return RedirectToRoute("DanhSachLinhVucs");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLinhVucHoiDap(HoiDapLinhVucModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditLinhVucHoiDap");
            }
            var _linhvuchoidap = await db.HoiDapLinhVucs.FindAsync(model.LinhVucId);
            if (_linhvuchoidap != null)
            {
                _linhvuchoidap.TenLinhVuc = model.TenLinhVuc ?? null;
                _linhvuchoidap.DiaChiTruyCap = model.DiaChiTruyCap ?? null;
                _linhvuchoidap.LinhVucChaId = (int?)model.LinhVucChaId ?? null;
                _linhvuchoidap.ViTriLinhVuc = (int?)model.ViTriLinhVuc ?? null;
                _linhvuchoidap.Published = model.Published;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật lĩnh vực thành công";
                return RedirectToAction("EditLinhVucHoiDap");
            }
            return View(_linhvuchoidap);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public JsonResult SetPositionLinhVucHoiDapMoi(int? id)
        {
            try
            {
                int _iPosition = 0;
                var _LinhVucHoiDap = db.HoiDapLinhVucs.Where(c => c.LinhVucId == id).FirstOrDefault();
                if (_LinhVucHoiDap != null)
                {
                    int iCats = _LinhVucHoiDap.HoiDapLinhVucs1.Count();
                    if (iCats > 0)
                    {
                        _iPosition += iCats + 1;
                    }
                    else
                    {
                        _iPosition += 1;
                    }
                }

                return Json(_iPosition, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Administrator, Manager")]
        public PartialViewResult _SelectLinhVucHoiDapPartial()
        {
            List<DanhMucLinhVucHoiDap> data = LoadLinhVucs().Select(x => new DanhMucLinhVucHoiDap()
            {
                LinhVucId = x.LinhVucId,
                TenLinhVuc = x.TenLinhVuc,
                SlugLinhVuc = x.DiaChiTruyCap,
                PositionIndex = x.ViTriLinhVuc,
                LinhVucChaId = x.LinhVucChaId
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.LinhVucChaId == null).FirstOrDefault();
            SetChildrenLinhVucHoiDap(presidents, data);

            return PartialView("_SelectLinhVucHoiDapPartial", presidents);
        }


        [Authorize(Roles = "Administrator, Manager")]
        public PartialViewResult _LinhVucHoiDapPartial()
        {
            List<DanhMucLinhVucHoiDap> data = LoadLinhVucs().Select(x => new DanhMucLinhVucHoiDap()
            {
               LinhVucId = x.LinhVucId,
               TenLinhVuc = x.TenLinhVuc,
               SlugLinhVuc = x.DiaChiTruyCap,
               PositionIndex = x.ViTriLinhVuc,
               LinhVucChaId = x.LinhVucChaId
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.LinhVucChaId == null).FirstOrDefault();
            SetChildrenLinhVucHoiDap(presidents, data);

            return PartialView("_LinhVucHoiDapPartial", presidents);
        }

        [Authorize(Roles = "Administrator, Manager")]
        private void SetChildrenLinhVucHoiDap(DanhMucLinhVucHoiDap model, List<DanhMucLinhVucHoiDap> danhmuc)
        {
            var childs = danhmuc.Where(x => x.LinhVucChaId == model.LinhVucId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildrenLinhVucHoiDap(child, danhmuc);
                    model.DanhMucLinhVucHoiDaps.Add(child);
                }
            }
        }

        //XoaCauHoi
        [Authorize(Roles = "Administrator, Manager")]
        public ActionResult XoaCauHoi(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            HoiDapCauHois _cauhoi = db.HoiDapCauHois.Find(id);
            if (_cauhoi == null)
            {
                return View();
            }
            return View(_cauhoi);
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpPost, ActionName("XoaCauHoi")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> XoaCauHoiConfirmed(long? id)
        {
            HoiDapCauHois _cauhoi = await db.HoiDapCauHois.FindAsync(id);
            if (_cauhoi == null)
            {
                return View();
            }

            db.HoiDapCauHois.Remove(_cauhoi);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Câu hỏi đã được xóa bỏ.";
            return RedirectToRoute("DanhSachCauHoiHoiDap");
        }

    }
}