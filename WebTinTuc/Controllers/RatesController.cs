using Helpers;
using Newtonsoft.Json;
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
using System.Globalization;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RatesController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();

        //admin/danhsachdonvi
        public ActionResult Index(int? pg, string search)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = LoadDonVis().Where(x=>x.MaDonViCha != null);
            if (data == null)
            {
                return View(data);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TenDonVi.ToLower().Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderBy(x => x.MaDonViCha);

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        //admin/ThemMoiDonVi
        public ActionResult ThemMoiDonVi()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ThemMoiDonVi(DonViModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ThemMoiDonVi");
            }
            try
            {
                DonVi _newDonVi = new DonVi();
                _newDonVi.TenDonVi = model.TenDonVi ?? null;
                _newDonVi.SlugDonVi = model.SlugDonVi ?? null;
                _newDonVi.MaDonViCha = model.MaDonViCha ?? null;
                _newDonVi.PositionIndex = model.PositionIndex ?? null;
                _newDonVi.Published = model.Published;
                db.DonVis.Add(_newDonVi);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm đơn vị mới thành công";
                return RedirectToAction("ThemMoiDonVi");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm mới đơn vị.");
                return View(model);
            }
        }

        public PartialViewResult _SelectDonViPartial()
        {
            return PartialView("_SelectDonViPartial", LoadDanhMucDonVi());
        }

        public PartialViewResult _DonviPartial()
        {
            return PartialView("_DonviPartial", LoadDanhMucDonVi());
        }

        private DanhMucDonVi LoadDanhMucDonVi()
        {
            List<DanhMucDonVi> data = LoadDonVis().Select(x => new DanhMucDonVi()
            {
                DonViId = x.DonViId,
                TenDonVi = x.TenDonVi,
                SlugDonVi = x.SlugDonVi,
                MaDonViCha = x.MaDonViCha,
                Published = (bool)x.Published,
                PositionIndex = x.PositionIndex
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.MaDonViCha == null).FirstOrDefault();
            SetChildrenDonVi(presidents, data);
            return presidents;
        }

        private void SetChildrenDonVi(DanhMucDonVi model, List<DanhMucDonVi> danhmuc)
        {
            var childs = danhmuc.Where(x => x.MaDonViCha == model.DonViId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildrenDonVi(child, danhmuc);
                    model.DanhMucDonVis.Add(child);
                }
            }
        }

        [HttpPost]
        public JsonResult SetPositionDonViNew(int? id)
        {
            try
            {
                int _iPosition = 0;
                var _donvi = db.DonVis.Find(id);
                if (_donvi != null)
                {
                    int idonvis = _donvi.DonVis1.Count;
                    if (idonvis > 0)
                    {
                        _iPosition += idonvis + 1;
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

        //EditDonVi
        public async Task<ActionResult> EditDonVi(int? id)
        {
            if (id == null || id == 0 || id == 1)
            {
                return View();
            }
            DonVi _donvi = await db.DonVis.FindAsync(id);
            if (_donvi == null)
            {
                return View(_donvi);
            }
            var _getDonviModel = new DonViModel()
            {
                DonViId = _donvi.DonViId,
                TenDonVi = _donvi.TenDonVi,
                SlugDonVi = _donvi.SlugDonVi,
                MaDonViCha = _donvi.MaDonViCha,
                PositionIndex = _donvi.PositionIndex,
                Published = (bool)_donvi.Published
            };
            return View(_getDonviModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDonVi(DonViModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditDonVi");
            }
            var _donvi = await db.DonVis.FindAsync(model.DonViId);
            if (_donvi != null)
            {
                _donvi.TenDonVi = model.TenDonVi;
                _donvi.SlugDonVi = model.SlugDonVi;
                _donvi.MaDonViCha = model.MaDonViCha;
                _donvi.PositionIndex = model.PositionIndex;
                _donvi.Published = model.Published;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật đơn vị thành công.";
                return RedirectToAction("EditDonVi");
            }
            return View(_donvi);
        }

        //DeleteDonVi
        public ActionResult DeleteDonVi(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            DonVi _donvi = db.DonVis.Find(id);
            if (_donvi == null)
            {
                return View();
            }
            return View(_donvi);
        }

        [HttpPost, ActionName("DeleteDonVi")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteDonViConfirmed(int? id)
        {
            DonVi _donvi = await db.DonVis.FindAsync(id);
            if (_donvi == null)
            {
                return View();
            }
            if (_donvi.DonVis1.Count > 0)
            {
                TempData["Error"] = "Bạn không thể xóa đơn vị này. <br /> Danh mục này chứa danh mục con khác. Vui lòng xóa danh mục con trước.";
                return RedirectToRoute("AdminDeleteDonVi", new { id = _donvi.DonViId });
            }

            if (_donvi.LinhVucs.Count > 0)
            {
                TempData["Error"] = "Bạn không thể xóa đơn vị này. <br /> Danh mục này đang chứa lĩnh vực.";
                return RedirectToRoute("AdminDeleteDonVi", new { id = _donvi.DonViId });
            }

            db.DonVis.Remove(_donvi);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Đơn vị đã được xóa.";
            return RedirectToRoute("AdminDanhSachDonVi");
        }

        //MoveDonVi
        public ActionResult MoveDonVi(int? id)
        {
            if (id == null || id == 0 || id == 1)
            {
                return View();
            }
            DonVi _donvi = db.DonVis.Find(id);
            if (_donvi == null)
            {
                return View();
            }
            var _getDonviModel = new DonViModel()
            {
                DonViId = _donvi.DonViId,
                MaDonViCha = _donvi.MaDonViCha,
                PositionIndex = _donvi.PositionIndex,
                Published = (bool)_donvi.Published,
                SlugDonVi = _donvi.SlugDonVi,
                TenDonVi = _donvi.TenDonVi
            };

            return View(_getDonviModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveDonVi(DonViModel model)
        {
            DonVi _donvi = await db.DonVis.FindAsync(model.DonViId);
            if (_donvi == null)
            {
                return View();
            }
            if (_donvi.DonViId == 1)
            {
                TempData["Error"] = "Danh mục này không có quyền sửa.";
                return RedirectToAction("MoveDonVi");
            }
            //8 danh mục, position 8 = thì là vị trí cuối cùng
            //8 danh mục, position 1 là vị trí đầu tiên
            // danh muc la danh muc cuoi cung va di chuyen tu tren xuong
            if (_donvi.DonVis1.Count == model.PositionIndex && model.TopToBottom == false)
            {
                TempData["Error"] = "Danh mục này ở vị trí cuối cùng không thể di chuyển xuống được";
                return RedirectToAction("MoveDonVi");
            }

            // danh muc la danh muc tren cung va di chuuyen tu duoi len
            if (_donvi.PositionIndex == 1 && model.TopToBottom == true)
            {
                TempData["Error"] = "Danh mục này ở vị trí trên cùng không thể di chuyển lên tiếp được";
                return RedirectToAction("MoveDonVi");
            }

            if (model.TopToBottom == false)
            {
                var _query1 = string.Format("Update DonVi Set PositionIndex=PositionIndex-1 Where MaDonViCha = {0} and PositionIndex=(Select PositionIndex+1 From DonVi where DonViId={1})", _donvi.MaDonViCha, _donvi.DonViId);
                var _query2 = string.Format("Update DonVi Set PositionIndex=PositionIndex+1 Where MaDonViCha = {0} and DonViId={1}", _donvi.MaDonViCha, _donvi.DonViId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }
            else
            {
                var _query1 = string.Format("Update DonVi Set PositionIndex=PositionIndex+1 Where MaDonViCha = {0} and PositionIndex=(Select PositionIndex-1 From DonVi where DonViId={1})", _donvi.MaDonViCha, _donvi.DonViId);
                var _query2 = string.Format("Update DonVi Set PositionIndex=PositionIndex-1 Where MaDonViCha = {0} and DonViId={1}", _donvi.MaDonViCha, _donvi.DonViId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }

            TempData["Moved"] = "Di chuyển vị trí danh mục thành công.";
            return RedirectToAction("MoveDonVi");
        }

        //SetPositionLinhVucNew
        [HttpPost]
        public JsonResult SetPositionLinhVucNew(int? id)
        {
            try
            {
                int _iPosition = 0;
                var _donvi = db.DonVis.Find(id);
                if (_donvi != null)
                {
                    int idonvis = _donvi.LinhVucs.Count;
                    if (idonvis > 0)
                    {
                        _iPosition += idonvis + 1;
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

        //admin/themoilinhvuc
        public ActionResult ThemMoiLinhVuc()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ThemMoiLinhVuc(LinhVucModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ThemMoiLinhVuc");
            }
            try
            {
                LinhVuc _newLinhVuc = new LinhVuc();
                _newLinhVuc.TenLinhVuc = model.TenLinhVuc ?? null;
                _newLinhVuc.DonViId = model.DonViId ?? (int?)null;
                _newLinhVuc.PositionIndex = model.PositionIndex ?? (int?)null;
                _newLinhVuc.Published = model.Published;
                db.LinhVucs.Add(_newLinhVuc);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm lĩnh vực mới thành công";
                return RedirectToAction("ThemMoiLinhVuc");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm mới.");
                return View(model);
            }
        }

        //EditLinhVuc
        public async Task<ActionResult> EditLinhVuc(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            LinhVuc _linhvuc = await db.LinhVucs.FindAsync(id);
            if (_linhvuc == null)
            {
                return View(_linhvuc);
            }
            var _getLinhVucModel = new LinhVucModel()
            {
                DonViId = _linhvuc.DonViId,
                LinhVucId = _linhvuc.LinhVucId,
                PositionIndex = _linhvuc.PositionIndex,
                TenLinhVuc = _linhvuc.TenLinhVuc,
                Published = (bool)_linhvuc.Published
            };
            return View(_getLinhVucModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLinhVuc(LinhVucModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditLinhVuc");
            }
            var _linhvuc = await db.LinhVucs.FindAsync(model.LinhVucId);
            if (_linhvuc != null)
            {
                _linhvuc.TenLinhVuc = model.TenLinhVuc ?? null;
                _linhvuc.DonViId = model.DonViId ?? null;
                _linhvuc.PositionIndex = model.PositionIndex ?? (int?)null;
                _linhvuc.Published = model.Published;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật lĩnh vực thành công.";
                return RedirectToAction("EditLinhVuc");
            }
            return View(_linhvuc);
        }

        //DeleteLinhVuc
        public ActionResult DeleteLinhVuc(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            LinhVuc _linhvuc = db.LinhVucs.Find(id);
            if (_linhvuc == null)
            {
                return View();
            }
            return View(_linhvuc);
        }

        [HttpPost, ActionName("DeleteLinhVuc")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteLinhVucConfirmed(int? id)
        {
            LinhVuc _linhvuc = await db.LinhVucs.FindAsync(id);
            if (_linhvuc == null)
            {
                return View();
            }
            if (_linhvuc.DanhGiaDonVis.Count > 0)
            {
                TempData["Error"] = "Bạn không thể xóa lĩnh vực này. <br /> Lĩnh vực này đã được đánh giá.";
                return RedirectToAction("DeleteLinhVuc", new { id = _linhvuc.LinhVucId });
            }

            db.LinhVucs.Remove(_linhvuc);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Lĩnh vực đã được xóa.";
            return RedirectToRoute("AdminDanhSachLinhVuc");
        }

        //MoveLinhVuc
        public ActionResult MoveLinhVuc(int? id)
        {
            if (id == null || id == 0 || id == 1)
            {
                return View();
            }
            LinhVuc _linhvuc = db.LinhVucs.Find(id);
            if (_linhvuc == null)
            {
                return View();
            }
            var _getLinhVucModel = new LinhVucModel()
            {
                DonViId = _linhvuc.DonViId,
                TenLinhVuc = _linhvuc.TenLinhVuc,
                PositionIndex = _linhvuc.PositionIndex,
                LinhVucId = _linhvuc.LinhVucId,
                Published = (bool)_linhvuc.Published
            };

            return View(_getLinhVucModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveLinhVuc(LinhVucModel model)
        {
            LinhVuc _linhvuc = await db.LinhVucs.FindAsync(model.LinhVucId);
            if (_linhvuc == null)
            {
                return View();
            }
            
            if (_linhvuc.DonVi.LinhVucs.Count == model.PositionIndex && model.TopToBottom == false)
            {
                TempData["Error"] = "Danh mục này ở vị trí cuối cùng không thể di chuyển xuống được";
                return RedirectToAction("MoveLinhVuc");
            }

            // danh muc la danh muc tren cung va di chuuyen tu duoi len
            if (_linhvuc.PositionIndex == 1 && model.TopToBottom == true)
            {
                TempData["Error"] = "Danh mục này ở vị trí trên cùng không thể di chuyển lên tiếp được";
                return RedirectToAction("MoveLinhVuc");
            }

            if (model.TopToBottom == false)
            {
                var _query1 = string.Format("Update LinhVuc Set PositionIndex=PositionIndex-1 Where DonViId = {0} and PositionIndex=(Select PositionIndex+1 From LinhVuc where LinhVucId={1})", _linhvuc.DonViId, _linhvuc.LinhVucId);
                var _query2 = string.Format("Update LinhVuc Set PositionIndex=PositionIndex+1 Where DonViId = {0} and LinhVucId={1}", _linhvuc.DonViId, _linhvuc.LinhVucId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }
            else
            {
                var _query1 = string.Format("Update LinhVuc Set PositionIndex=PositionIndex+1 Where DonViId = {0} and PositionIndex=(Select PositionIndex-1 From LinhVuc where LinhVucId={1})", _linhvuc.DonViId, _linhvuc.LinhVucId);
                var _query2 = string.Format("Update LinhVuc Set PositionIndex=PositionIndex-1 Where DonViId = {0} and LinhVucId={1}", _linhvuc.DonViId, _linhvuc.LinhVucId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }

            TempData["Moved"] = "Di chuyển vị trí danh mục thành công.";
            return RedirectToAction("MoveLinhVuc");
        }


        //admin/danhsachlinhvuc
        public ActionResult DanhSachLinhVuc(int? pg, string search, string status, int? DonViId)
        {
            var data = from d in db.LinhVucs select d;
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TenLinhVuc.ToLower().Contains(search));
                ViewBag.search = search;
            }

            if (status == null) status = ""; if (DonViId == null) DonViId = (int?)null;
            if (status != null && status != "")
            {
                bool bstatus = Convert.ToBoolean(status);
                data = data.Where(x => x.Published == bstatus);
                ViewBag.status = bstatus;
            }
            if (DonViId != null)
            {
                data = data.Where(x => x.DonViId == DonViId);
                ViewBag.DonViId = DonViId;
            }

            data = data.OrderBy(x => x.PositionIndex);
            return View(data.ToPagedList(pageNumber, pageSize));
            
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
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "congchuc");

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "/Images/WallImages/congchuc/" + _fileName;
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


        //admin/themoicongchuc
        public ActionResult ThemMoiCongChuc()
        {
            return View();
        }

        //ThemMoiCongChuc
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ThemMoiCongChuc(CongChucModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ThemMoiCongChuc");
            }
            try
            {
                CongChuc _newCongChuc = new CongChuc();
                _newCongChuc.TenCongChuc = model.TenCongChuc;
                _newCongChuc.HinhAnh = model.HinhAnh ?? "/Images/no-image.png";
                // ngaysinh dd/mm/yyyy -> mm/dd/yyyy
                _newCongChuc.NgaySinh = model.NgaySinh ?? (System.DateTime?)null;
                _newCongChuc.ChucVu = model.ChucVu ?? null;
                _newCongChuc.DonViId = model.DonViId ?? (int?)null;
                _newCongChuc.TrinhDo = model.TrinhDo ?? null;
                _newCongChuc.Published = model.Published;
                db.CongChucs.Add(_newCongChuc);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm công chức mới thành công";
                return RedirectToAction("ThemMoiCongChuc");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm mới.");
                return View(model);
            }
        }

        //EditCongChuc
        public async Task<ActionResult> EditCongChuc(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            CongChuc _congchuc = await db.CongChucs.FindAsync(id);
            if (_congchuc == null)
            {
                return View(_congchuc);
            }
            var _getCongChucModel = new CongChucModel()
            {
                CongChucId = _congchuc.CongChucId,
                TenCongChuc = _congchuc.TenCongChuc,
                DonViId = _congchuc.DonViId,
                HinhAnh = _congchuc.HinhAnh,
                NgaySinh = _congchuc.NgaySinh,
                TrinhDo = _congchuc.TrinhDo,
                ChucVu = _congchuc.ChucVu,
                Published = (bool)_congchuc.Published
            };
            return View(_getCongChucModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCongChuc(CongChucModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditCongChuc");
            }
            var _congchuc = await db.CongChucs.FindAsync(model.CongChucId);
            if (_congchuc != null)
            {
                if (_congchuc.HinhAnh != model.HinhAnh && model.HinhAnh != null)
                {
                    string path = Server.MapPath(_congchuc.HinhAnh);
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                _congchuc.TenCongChuc = model.TenCongChuc;
                _congchuc.HinhAnh = model.HinhAnh ?? "/Images/no-image.png";
                _congchuc.NgaySinh = model.NgaySinh ?? (System.DateTime?)null;
                _congchuc.ChucVu = model.ChucVu ?? null;
                _congchuc.DonViId = model.DonViId ?? (int?)null;
                _congchuc.TrinhDo = model.TrinhDo ?? null;
                _congchuc.Published = model.Published;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật công chức thành công.";
                return RedirectToAction("EditCongChuc", new { id = _congchuc.CongChucId });
            }
            return View(_congchuc);
        }

        //DeleteCongChuc
        public ActionResult DeleteCongChuc(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            CongChuc _congchuc = db.CongChucs.Find(id);
            if (_congchuc == null)
            {
                return View();
            }
            return View(_congchuc);
        }

        [HttpPost, ActionName("DeleteCongChuc")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCongChucConfirmed(int? id)
        {
            CongChuc _congchuc = await db.CongChucs.FindAsync(id);
            if (_congchuc == null)
            {
                return View();
            }
            if (_congchuc.DanhGiaCongChucs.Count > 0)
            {
                TempData["Error"] = "Bạn không thể xóa công chức này. <br /> Công chức đang chứa kết quả đánh giá.";
                return RedirectToAction("DeleteCongChuc", new { id = _congchuc.CongChucId });
            }

            db.CongChucs.Remove(_congchuc);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Công chức đã được xóa.";
            return RedirectToRoute("AdminDanhSachCongChuc");
        }

        //admin/danhsachcongchuc
        public ActionResult DanhSachCongChuc(int? pg, string search, string status, int? DonViId)
        {
            var data = from d in db.CongChucs select d;
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TenCongChuc.ToLower().Contains(search));
                ViewBag.search = search;
            }

            if (status == null) status = ""; if (DonViId == null) DonViId = (int?)null;
            if (status != null && status != "")
            {
                bool bstatus = Convert.ToBoolean(status);
                data = data.Where(x => x.Published == bstatus);
                ViewBag.status = bstatus;
            }
            if (DonViId != null)
            {
                data = data.Where(x => x.DonViId == DonViId);
                ViewBag.DonViId = DonViId;
            }

            data = data.OrderBy(x => x.TenCongChuc);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        //DanhSachLyDo
        public ActionResult DanhSachLyDo(int? pg, string search)
        {
            var data = from d in db.Lydoes select d;
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.TenLyDo.ToLower().Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderBy(x => x.TenLyDo);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        //ThemLyDo
        public ActionResult ThemLyDo()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ThemLyDo(LyDoModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ThemLyDo");
            }
            try
            {
                Lydo _newlydo = new Lydo();
                _newlydo.TenLyDo = model.TenLyDo ?? null;
                db.Lydoes.Add(_newlydo);              
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm lý do mới thành công";
                return RedirectToAction("ThemLyDo");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm mới.");
                return View(model);
            }
        }

        //EditLyDo
        public async Task<ActionResult> EditLyDo(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Lydo _lydo = await db.Lydoes.FindAsync(id);
            if (_lydo == null)
            {
                return View(_lydo);
            }
            var _getLyDoModel= new LyDoModel()
            {
               LyDoId = _lydo.LyDoId,
               TenLyDo = _lydo.TenLyDo
            };
            return View(_getLyDoModel);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditLyDo(LyDoModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditLyDo");
            }
            var _lydo = await db.Lydoes.FindAsync(model.LyDoId);
            if (_lydo != null)
            {
                _lydo.TenLyDo = model.TenLyDo ?? null;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật lý do thành công.";
                return RedirectToAction("EditLyDo");
            }
            return View(_lydo);
        }

        //DeleteLyDo
        public ActionResult DeleteLyDo(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Lydo _lydo = db.Lydoes.Find(id);
            if (_lydo == null)
            {
                return View();
            }
            return View(_lydo);
        }

        [HttpPost, ActionName("DeleteLyDo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteLyDoConfirmed(int? id)
        {
            Lydo _lydo = await db.Lydoes.FindAsync(id);
            if (_lydo == null)
            {
                return View();
            }
            if (_lydo.DanhGiaDonVis.Count > 0)
            {
                TempData["Error"] = "Bạn không thể xóa lý do này. <br /> Lý do này có chứa đánh giá đơn vị.";
                return RedirectToAction("DeleteLyDo", new { id = _lydo.LyDoId });
            }

            if (_lydo.DanhGiaCongChucs.Count > 0)
            {
                TempData["Error"] = "Bạn không thể xóa lý do này. <br /> Lý do này có chứa đánh giá công chức.";
                return RedirectToAction("DeleteLyDo", new { id = _lydo.LyDoId });
            }

            db.Lydoes.Remove(_lydo);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Lý do đã được xóa.";
            return RedirectToRoute("AdminDanhSachLyDo");
        }

        public JsonResult GetDonvi()
        {
            List<DonViJson> data = LoadDonVis().Select(x => new DonViJson()
            {
                id = x.DonViId,
                name = x.TenDonVi,
                parentId = x.MaDonViCha
            }).ToList();

            var obj = new { root = data };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public List<DonVi> LoadDonVis()
        {
            //var query = "select * from DonVis";
            //var data = db.Database.SqlQuery<DonVi>(query);
            var data = (from d in db.DonVis select d).ToList();
            return data;
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