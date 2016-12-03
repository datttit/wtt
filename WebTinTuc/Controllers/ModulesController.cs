using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using Helpers;
using System.Threading.Tasks;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ModulesController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();

        // GET: Modules
        public ActionResult Index(int? pg, string search, string type, string po, string status)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            ViewBag.TypeModule = Configs.ListTypeModule();
            ViewBag.PositionModule = Configs.ListPosition();
            var data = from m in db.Modules select m;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.ModuleName.ToLower().Contains(search));
                ViewBag.search = search;
            }

            if (type == null) type = ""; if (po == null) po = ""; if (status == null) status = "";

            if (type != null && type != "")
            {
                data = data.Where(t => t.TypeModule == type);
                ViewBag.type = type;
            }

            if (po != null && po != "")
            {
                data = data.Where(x => x.Position == po);
                ViewBag.po = po;
            }

            if (status != null && status != "")
            {
                bool pu = Convert.ToBoolean(status);
                data = data.Where(x => x.Published == pu);
                ViewBag.status = status;
            }

            data = data.OrderBy(x => x.PositionIndex);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddNewModule()
        {
            ViewBag.TypeModule = Configs.ListTypeModule();
            ViewBag.PositionModule = Configs.ListPosition();
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewModule(NewModuleModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("AdminAddNewModules");
            }
            try
            {
                Module _newModule = new Module();
                _newModule.ModuleName = model.ModuleName ?? null;
                _newModule.ModuleLink = model.ModuleLink ?? null;
                _newModule.HiddenModuleName = model.HiddenModuleName;
                _newModule.Position = model.Position ?? null;
                _newModule.PositionIndex = model.PositionIndex ?? (int?)null;
                _newModule.TypeModule = model.TypeModule ?? "left";
                _newModule.Published = model.Published;
                db.Modules.Add(_newModule);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới module.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới module");
                return RedirectToRoute("AdminAddNewModules"); 
            }            
            return RedirectToRoute("AdminAddNewModules");
        }

        [HttpPost]
        public JsonResult getIndexNewModule(string po)
        {
            try
            {
                int _iPosition = 0;
                if (po != null && po != "")
                {
                    int CountModule = db.Modules.Where(x => x.Position == po).Count();
                    if (CountModule > 0)
                    {
                        _iPosition = CountModule + 1;
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

        public async Task<ActionResult> EditModule(int? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            Module _module = await db.Modules.FindAsync(id);
            if (_module == null)
            {
                return View(_module);
            }
            var getModule = new EditModuleModel()
            {
                ModuleId = _module.ModuleId,
                ModuleName = _module.ModuleName,
                HiddenModuleName = (bool)_module.HiddenModuleName,
                ModuleLink = _module.ModuleLink,
                Position = _module.Position,
                PositionIndex = _module.PositionIndex,
                TypeModule = _module.TypeModule,
                Published = (bool)_module.Published
            };
            ViewBag.TypeModule = Configs.ListTypeModule();
            ViewBag.PositionModule = Configs.ListPosition();
            return View(getModule);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditModule(EditModuleModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditModule");
            }
            try
            {
                var _module = await db.Modules.FindAsync(model.ModuleId);
                if (_module != null)
                {
                    _module.ModuleName = model.ModuleName ?? null;
                    _module.ModuleLink = model.ModuleLink ?? null;
                    _module.HiddenModuleName = model.HiddenModuleName;
                    _module.Position = model.Position ?? null;
                    _module.PositionIndex = model.PositionIndex ?? (int?)null;
                    _module.TypeModule = model.TypeModule ?? "left";
                    _module.Published = model.Published;
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Cập nhật module thành công";                    
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi cập nhật module");
                return RedirectToAction("EditModule"); 
            }
            return RedirectToAction("EditModule");
        }

        public ActionResult DeleteModule(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Module _module = db.Modules.Find(id);
            if (_module == null)
            {
                return View();
            }
            return View(_module);
        }

        [HttpPost, ActionName("DeleteModule")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteModuleConfirmed(int? id)
        {
            try
            {
                Module _module = await db.Modules.FindAsync(id);
                if (_module == null)
                {
                    return View();
                }

                db.Modules.Remove(_module);
                await db.SaveChangesAsync();
                TempData["Deleted"] = "Xóa module thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa module.";
                return RedirectToAction("DeleteModule");
            }           

            return RedirectToRoute("AdminModules");
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