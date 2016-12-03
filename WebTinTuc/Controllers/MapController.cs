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

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MapController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: Map

        public ActionResult DsBanDo(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = db.Maps.ToList();
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Index()
        {
            var data = db.Maps.Where(x=>x.Published == true).Select(x => x).FirstOrDefault();
            if (data == null)
            {
                return RedirectToAction("AddNewMap");
            }
            var getMaps = new MapModel()
            {
                MapId = data.MapId,
                MapName = data.MapName,
                MapDescription = data.MapDescription,
                ApiKey = data.ApiKey,
                Lat = data.Lat,
                Long = data.Long,
                Published = (bool)data.Published
            };
            return View(getMaps);
        }

        public ActionResult AddNewMap() {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewMap(MapModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewMap");
            }
            try
            {
                Map _newMap = new Map();
                _newMap.MapName = model.MapName ?? "Tên bản đồ";
                _newMap.MapDescription = model.MapDescription ?? null;
                _newMap.Lat = model.Lat ?? null;
                _newMap.Long = model.Long ?? null;
                _newMap.ApiKey = model.ApiKey ?? "AIzaSyCzXNGkYkCD22rq1pCXkR_qz2cIMy7eDio";
                _newMap.Published = model.Published;
                db.Maps.Add(_newMap);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới bản đồ.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới bản đồ");
                return RedirectToAction("AddNewMap");
            }
            return RedirectToAction("AddNewMap");
        }

        //UpdateMap
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateMap(MapModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            try
            {
                var _map = await db.Maps.FindAsync(model.MapId);
                if (_map != null)
                {
                    _map.MapName = model.MapName ?? "Tên bản đồ";
                    _map.MapDescription = model.MapDescription ?? null;
                    _map.Lat = model.Lat ?? null;
                    _map.Long = model.Long ?? null;
                    _map.ApiKey = model.ApiKey ?? "AIzaSyCzXNGkYkCD22rq1pCXkR_qz2cIMy7eDio";
                    _map.Published = model.Published;
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Cập nhật bản đồ thành công";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi cập nhật bản đồ.");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        public ActionResult DeleteMap(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Map _map = db.Maps.Find(id);
            if (_map == null)
            {
                return View();
            }
            return View(_map);
        }

        [HttpPost, ActionName("DeleteMap")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMapConfirmed(int? id)
        {
            Map _map = await db.Maps.FindAsync(id);
            if (_map == null)
            {
                return View();
            }
            if (_map.Published == true)
            {
                TempData["Error"] = "Bạn không thể xóa bản đồ. <br />Bản đồ này đang được mở.";
                return RedirectToAction("DeleteMap");
            }

            db.Maps.Remove(_map);
            await db.SaveChangesAsync();
            TempData["Updated"] = "Xóa bản đồ này thành công";
            return RedirectToAction("AddNewMap");
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