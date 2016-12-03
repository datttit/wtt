using Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebTinTuc.Models;

namespace WebTinTuc.Controllers
{
    [Authorize]
    public class AdminController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();

        // GET: Admin
        public ActionResult Index()
        {
            if (!isAdminUser())
            {
                return RedirectToAction("Index", "Home");
            }
            if (User.IsInRole("Administrator"))
            {
                ViewBag.TotalArticles = db.Articles.Count();
                ViewBag.TotalAlbum = db.Albums.Count();
                ViewBag.TotalMenu = db.Menus.Count();
                ViewBag.TotalCat = db.Categories.Where(x => x.ParentCategoryId != null).Count();
                ViewBag.TotalUser = db.AspNetUsers.Count();
                string userId = User.Identity.GetUserId();
                ViewBag.TotalQuestions = db.HoiDapCauHois.Where(x => x.DonViTiepNhanId == userId).Count();
            }
            else if (User.IsInRole("Manager"))
            {
                string userId = User.Identity.GetUserId();
                ViewBag.TotalQuestions = db.HoiDapCauHois.Where(x => x.DonViTiepNhanId == userId).Count();
            }

            return View();
        }

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserId();
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user);
                if (s[0].ToString() == "Administrator" || s[0].ToString() == "Manager")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            return View();
        }

        #region ### Danh mục bài viết

        public ActionResult Categories()
        {

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> EditCategory(int? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return View(category);
            }
            var _cat = new EditCategoryModel()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId,
                PositionHome = category.PositionHome,
                PositionIndex = category.PositionIndex,
                Published = Convert.ToBoolean(category.Published),
                SlugCategory = category.SlugCategory,
                ViewHome = Convert.ToBoolean(category.ViewHome)
            };
            return View(_cat);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategory(EditCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditCategory");
            }
            var _category = await db.Categories.FindAsync(model.CategoryId);
            if (_category != null)
            {
                _category.CategoryName = model.CategoryName ?? null;
                _category.SlugCategory = model.SlugCategory ?? null;
                _category.ParentCategoryId = model.ParentCategoryId ?? (int?)null;
                _category.PositionIndex = model.PositionIndex ?? (int?)null;
                _category.ViewHome = model.ViewHome;
                _category.PositionHome = model.PositionHome ?? (int?)null;
                _category.Published = model.Published;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật danh mục bài viết thành công";
                return RedirectToAction("EditCategory");
            }
            return View(_category);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AddCategory()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(AddCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddCategory");
            }
            try
            {
                Category _newCategory = new Category();
                _newCategory.CategoryName = model.CategoryName ?? null;
                _newCategory.SlugCategory = model.SlugCategory ?? null;
                _newCategory.ParentCategoryId = model.ParentCategoryId ?? (int?)null;
                _newCategory.PositionIndex = model.PositionIndex ?? (int?)null;
                _newCategory.ViewHome = model.ViewHome;
                _newCategory.PositionHome = model.PositionHome ?? (int?)null;
                _newCategory.Published = model.Published;
                db.Categories.Add(_newCategory);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm danh mục bài viết thành công";
                return RedirectToAction("AddCategory");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm danh mục bài viết");
                return View(model);
            }
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null || id == 0 || id == 1)
            {
                return View();
            }
            Category _category = db.Categories.Find(id);
            if (_category == null)
            {
                return View();
            }
            return View(_category);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCategoryConfirmed(int? id)
        {
            Category _category = await db.Categories.FindAsync(id);
            if (_category == null)
            {
                return View();
            }
            if (_category.Categories1.Count() > 0)
            {
                TempData["Error"] = "Bạn không thể xóa danh mục này. <br /> Danh mục này chứa danh mục con khác. Vui lòng xóa danh mục con trước.";
                return RedirectToAction("DeleteCategory");
            }

            if (_category.Articles.Count() > 0)
            {
                TempData["Error"] = "Bạn không thể xóa danh mục này. <br /> Danh mục này đang chứa bài viết.";
                return RedirectToAction("DeleteCategory");
            }

            db.Categories.Remove(_category);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Xóa danh mục bài viết thành công";
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult MoveCategory(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Category _category = db.Categories.Find(id);
            if (_category == null)
            {
                return View();
            }
            var _categoryModel = new MovePositionCategory()
            {
                CategoryId = _category.CategoryId,
                CategoryName = _category.CategoryName,
                ParentCategoryId = _category.ParentCategoryId,
                PositionIndex = _category.PositionIndex
            };

            return View(_categoryModel);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveCategory(MovePositionCategory model)
        {
            Category _category = await db.Categories.FindAsync(model.CategoryId);
            if (_category == null)
            {
                return View();
            }
            //8 danh mục, position 8 = thì là vị trí cuối cùng
            //8 danh mục, position 1 là vị trí đầu tiên
            // danh muc la danh muc cuoi cung va di chuyen tu tren xuong
            if (_category.Category1.Categories1.Count() == model.PositionIndex && model.TopToBottom == false)
            {
                TempData["Error"] = "Danh mục này ở vị trí cuối cùng không thể di chuyển xuống được";
                return RedirectToAction("MoveCategory");
            }

            // danh muc la danh muc tren cung va di chuuyen tu duoi len
            if (_category.PositionIndex == 1 && model.TopToBottom == true)
            {
                TempData["Error"] = "Danh mục này ở vị trí trên cùng không thể di chuyển lên tiếp được";
                return RedirectToAction("MoveCategory");
            }

            if (model.TopToBottom == false)
            {
                var _query1 = string.Format("Update Categories Set PositionIndex=PositionIndex-1 Where ParentCategoryId = {0} and PositionIndex=(Select PositionIndex+1 From Categories where CategoryId={1})", _category.ParentCategoryId, _category.CategoryId);
                var _query2 = string.Format("Update Categories Set PositionIndex=PositionIndex+1 Where ParentCategoryId = {0} and CategoryId={1}", _category.ParentCategoryId, _category.CategoryId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }
            else
            {
                var _query1 = string.Format("Update Categories Set PositionIndex=PositionIndex+1 Where ParentCategoryId = {0} and PositionIndex=(Select PositionIndex-1 From Categories where CategoryId={1})", _category.ParentCategoryId, _category.CategoryId);
                var _query2 = string.Format("Update Categories Set PositionIndex=PositionIndex-1 Where ParentCategoryId = {0} and CategoryId={1}", _category.ParentCategoryId, _category.CategoryId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }

            TempData["Moved"] = "Di chuyển vị trí danh mục thành công.";
            return RedirectToAction("Categories");
        }

        #endregion

        #region ### Danh mục Menu

        [Authorize(Roles = "Administrator")]
        public ActionResult AddNewMenu()
        {
            ViewBag.Position = Configs.ListPositionMenu();
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewMenu(AddMenuModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewMenu");
            }
            try
            {
                Menu _newMenu = new Menu();
                _newMenu.MenuName = model.MenuName ?? null;
                _newMenu.MenuUrl = model.MenuUrl ?? null;
                _newMenu.ParentMenuId = model.ParentMenuId ?? null;
                _newMenu.Position = model.Position ?? null;
                _newMenu.PositionIndex = model.PositionIndex ?? (int?)null;
                _newMenu.Published = model.Published;
                db.Menus.Add(_newMenu);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm menu bài viết thành công";
                return RedirectToAction("AddNewMenu");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm menu bài viết");
                return View(model);
            }
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ListMenuTop()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ListMenuLeft(int? pg, string search)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = LoadMenus().Where(x => x.Position == "left" && x.ParentMenuId != null);
            if (data == null)
            {
                return View(data);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.MenuName.ToLower().Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderBy(x => x.ParentMenuId);

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ListMenuBottom()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> EditMenu(int? id)
        {

            if (id == null || id == 0)
            {
                return View();
            }
            Menu _menu = await db.Menus.FindAsync(id);
            if (_menu == null)
            {
                return View(_menu);
            }
            var _cat = new EditMenuModel()
            {
                MenuId = _menu.MenuId,
                MenuName = _menu.MenuName,
                MenuUrl = _menu.MenuUrl,
                ParentMenuId = _menu.ParentMenuId,
                Position = _menu.Position,
                PositionIndex = _menu.PositionIndex,
                Published = (bool)_menu.Published
            };
            ViewBag.Position = Configs.ListPositionMenu();
            ViewBag.textPot = _cat.Position;
            return View(_cat);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMenu(EditMenuModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditMenu");
            }
            var _menu = await db.Menus.FindAsync(model.MenuId);
            if (_menu != null)
            {
                _menu.MenuName = model.MenuName ?? null;
                _menu.MenuUrl = model.MenuUrl ?? null;
                _menu.ParentMenuId = model.ParentMenuId ?? (int?)null;
                _menu.Position = model.Position ?? null;
                _menu.PositionIndex = model.PositionIndex ?? (int?)null;
                _menu.Published = model.Published;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật danh mục menu thành công";
                if (_menu.Position == "left")
                {
                    //return RedirectToAction("ListMenuLeft");
                    return RedirectToActionPermanent("ListMenuLeft", "admin");

                }
                else if (_menu.Position == "top")
                {
                    //return RedirectToAction("ListMenuTop", );
                    return RedirectToActionPermanent("ListMenuTop", "admin");
                }
                else
                {
                    //return RedirectToAction("ListMenubottom");
                    return RedirectToActionPermanent("ListMenubottom", "admin");
                }

            }
            return View(_menu);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteMenu(int? id)
        {
            if (id == null || id == 0 || id == 1)
            {
                return View();
            }
            Menu _menu = db.Menus.Find(id);
            if (_menu == null)
            {
                return View();
            }
            return View(_menu);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("DeleteMenu")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMenuConfirmed(int? id)
        {
            Menu _menu = await db.Menus.FindAsync(id);
            if (_menu == null)
            {
                return View();
            }
            if (_menu.Menus1.Count() > 0)
            {
                TempData["Error"] = "Bạn không thể xóa danh mục này. <br /> Danh mục này chứa danh mục con khác. Vui lòng xóa danh mục con trước.";
                return RedirectToAction("DeleteMenu");
            }

            db.Menus.Remove(_menu);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Xóa danh mục menu thành công";
            return RedirectToAction("Categories");
        }


        #endregion

        [HttpPost]
        public JsonResult GenerateSlugUrl(string strText)
        {
            string result = Configs.unicodeToNoMark(strText);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public JsonResult SetPositionCatNew(int id)
        {
            try
            {
                int _iPosition = 0;
                WebTinTucEntities db = new WebTinTucEntities();
                var _cat = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (_cat != null)
                {
                    int iCats = _cat.Categories1.Count();
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

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public JsonResult getViTriMenuMoi(int? id)
        {
            try
            {
                int _iPosition = 0;
                if (id == null || id == 0)
                {
                    _iPosition += 1;
                }

                var _menu = db.Menus.Find(id);
                if (_menu != null)
                {
                    int iMenu = _menu.Menus1.Count();
                    if (iMenu > 0)
                    {
                        _iPosition += iMenu + 1;
                    }
                    else
                    {
                        _iPosition += 1;
                    }

                    if (_menu.Position == "left" && _menu.ParentMenuId == null)
                    {
                        _iPosition = 0;
                        int count = LoadMenus().Where(x => x.Position == "left" && x.ParentMenuId == 1).Count();
                        _iPosition += count + 1;
                    }
                }

                return Json(_iPosition, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //public IEnumerable<Menu> LoadMenus()
        //{
        //    var query = "select * from Menus";
        //    var data = db.Database.SqlQuery<Menu>(query);
        //    return data;
        //}

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public JsonResult getViTriTopBottom(int? id, string pot)
        {
            try
            {
                int _iPosition = 0;
                if (id == null || id == 0)
                {
                    _iPosition += 1;
                }

                var _menu2 = db.Menus.Find(id);
                if (_menu2.ParentMenuId == null)
                {
                    var _listmenu = db.Menus.Where(m => m.Position == pot).Count();
                    _iPosition += _listmenu + 1;
                }

                var _menu = LoadMenus().Where(po => po.Position == pot && po.MenuId == id).FirstOrDefault();
                if (_menu != null)
                {
                    int iMenu = _menu.Menus1.Count();
                    if (iMenu > 0)
                    {
                        _iPosition += iMenu + 1;
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

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public JsonResult GenUrlMenu(int? id)
        {
            string result = "";
            if (id == null || id == 0)
            {
                result = "/menu-new" + Guid.NewGuid().ToString();
            }

            Category _category = db.Categories.Find(id);
            if (_category == null)
            {
                result = "menu-new" + Guid.NewGuid().ToString();
            }
            // "tin-tuc/" + _category.SlugCategory + "/" + _category.CategoryId;
            result = string.Format("/tin-tuc/{0}-{1}", _category.SlugCategory, _category.CategoryId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        public JsonResult GetDanhMuc()
        {
            List<DanhMucJson> data = LoadCategories().Select(x => new DanhMucJson()
            {
                id = x.CategoryId,
                name = x.CategoryName,
                parentId = x.ParentCategoryId,
                ViewHome = x.ViewHome,
                PositionIndex = x.PositionIndex,
                PositionOnHome = x.PositionHome,
                SlugCat = x.SlugCategory
            }).ToList();

            var obj = new { root = data };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        public JsonResult GetMenuLeft()
        {
            List<MenuJson> data = LoadMenus().Where(x => x.Position == "left").Select(x => new MenuJson()
            {
                id = x.MenuId,
                name = x.MenuName,
                parentId = x.ParentMenuId
            }).ToList();

            var obj = new { root = data };
            return Json(obj, JsonRequestBehavior.AllowGet);
            //List<MenuJson> data = LoadMenus().Where(x => x.Position == "left").Select(x => new MenuJson2()
            //{
            //    id = x.MenuId,
            //    name = x.MenuName,
            //    childnodes = x.Menus1
            //}).ToList();

            //var obj = new { root = data };
            //return Json(obj, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Administrator")]
        public JsonResult GetMenuTop()
        {
            List<MenuJson> data = LoadMenus().Where(x => x.Position == "top").Select(x => new MenuJson()
            {
                id = x.MenuId,
                name = x.MenuName,
                parentId = x.ParentMenuId
            }).ToList();
            data.Insert(0, new MenuJson() { id = 1, name = "Menu", parentId = null });

            var obj = new { root = data };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        public JsonResult GetMenuBottom()
        {
            List<MenuJson> data = LoadMenus().Where(x => x.Position == "bottom").Select(x => new MenuJson()
            {
                id = x.MenuId,
                name = x.MenuName,
                parentId = x.ParentMenuId
            }).ToList();
            data.Insert(0, new MenuJson() { id = 1, name = "Menu", parentId = null });
            var obj = new { root = data };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult MoveMenu(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Menu _menu = db.Menus.Find(id);
            if (_menu == null)
            {
                return View();
            }
            var _menuModel = new MovePositionMenu()
            {
                MenuId = _menu.MenuId,
                MenuName = _menu.MenuName,
                ParentMenuId = _menu.ParentMenuId,
                Position = _menu.Position,
                PositionIndex = _menu.PositionIndex
            };

            return View(_menuModel);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveMenu(MovePositionMenu model)
        {
            Menu _menu = await db.Menus.FindAsync(model.MenuId);
            if (_menu == null)
            {
                return View();
            }

            if (_menu.Position == "left")
            {
                int count = db.Menus.Where(x => x.Position == "left" && x.ParentMenuId == _menu.ParentMenuId).Count();
                if (count == model.PositionIndex && model.TopToBottom == false)
                {
                    TempData["Error"] = "Danh mục này ở vị trí cuối cùng không thể di chuyển xuống được";
                    return RedirectToAction("MoveMenu");
                }
                else if (_menu.PositionIndex == 1 && model.TopToBottom == true)
                {
                    TempData["Error"] = "Danh mục này ở vị trí trên cùng không thể di chuyển lên tiếp được";
                    return RedirectToAction("MoveMenu");
                }
            }

            if (_menu.Position == "top")
            {
                int count = db.Menus.Where(x => x.Position == "top" && x.ParentMenuId == _menu.ParentMenuId).Count();
                if (count == model.PositionIndex && model.TopToBottom == false)
                {
                    TempData["Error"] = "Danh mục này ở vị trí cuối cùng không thể di chuyển xuống được";
                    return RedirectToAction("MoveMenu");
                }
                else if (_menu.PositionIndex == 1 && model.TopToBottom == true)
                {
                    TempData["Error"] = "Danh mục này ở vị trí trên cùng không thể di chuyển lên tiếp được";
                    return RedirectToAction("MoveMenu");
                }

            }

            if (_menu.Position == "bottom")
            {
                int count = db.Menus.Where(x => x.Position == "bottom" && x.ParentMenuId == _menu.ParentMenuId).Count();
                if (count == model.PositionIndex && model.TopToBottom == false)
                {
                    TempData["Error"] = "Danh mục này ở vị trí cuối cùng không thể di chuyển xuống được";
                    return RedirectToAction("MoveMenu");
                }
                else if (_menu.PositionIndex == 1 && model.TopToBottom == true)
                {
                    TempData["Error"] = "Danh mục này ở vị trí trên cùng không thể di chuyển lên tiếp được";
                    return RedirectToAction("MoveMenu");
                }
            }

            // di chuyển cái này nào
            if (model.TopToBottom == false)
            {
                var _query1 = string.Format("Update Menus Set PositionIndex=PositionIndex-1 Where ParentMenuId = {0} and Position = '{1}' and PositionIndex=(Select PositionIndex+1 From Menus where MenuId={2})", _menu.ParentMenuId, _menu.Position, _menu.MenuId);
                var _query2 = string.Format("Update Menus Set PositionIndex=PositionIndex+1 Where ParentMenuId = {0} and Position = '{1}' and MenuId={2}", _menu.ParentMenuId, _menu.Position, _menu.MenuId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }
            else
            {
                var _query1 = string.Format("Update Menus Set PositionIndex=PositionIndex+1 Where ParentMenuId = {0} and Position = '{1}' and PositionIndex=(Select PositionIndex-1 From Menus where MenuId={2})", _menu.ParentMenuId, _menu.Position, _menu.MenuId);
                var _query2 = string.Format("Update Menus Set PositionIndex=PositionIndex-1 Where ParentMenuId = {0} and Position = '{1}' and MenuId={2}", _menu.ParentMenuId, _menu.Position, _menu.MenuId);

                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }

            TempData["Moved"] = "Di chuyển vị trí danh mục thành công.";
            if (_menu.Position == "left")
            {
                return RedirectToActionPermanent("ListMenuLeft", "admin");

            }
            else if (_menu.Position == "top")
            {
                return RedirectToActionPermanent("ListMenuTop", "admin");
            }
            else
            {
                return RedirectToActionPermanent("ListMenubottom", "admin");
            }
        }

        [Authorize(Roles = "Administrator")]
        public IEnumerable<Category> LoadCategories()
        {
            var data = from x in db.Categories select x;
            return data;
        }

        [Authorize(Roles = "Administrator")]
        public IEnumerable<Menu> LoadMenus()
        {
            var data = from x in db.Menus select x;
            return data;
        }

        [Authorize(Roles = "Administrator")]
        public PartialViewResult _CategoryPartial()
        {
            List<DanhMucBaiViet> data = LoadCategories().Select(x => new DanhMucBaiViet()
            {
                CatId = x.CategoryId,
                CatName = x.CategoryName,
                ParentCatId = x.ParentCategoryId,
                SlugCat = x.SlugCategory,
                PositionIndex = x.PositionIndex
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.ParentCatId == null).FirstOrDefault();
            SetChildrenCat(presidents, data);

            return PartialView("_CategoryPartial", presidents);
        }

        [Authorize(Roles = "Administrator")]
        public PartialViewResult _MenuPartial()
        {
            List<DanhMucMenu> data = LoadMenus().Where(o => o.Published == true).Select(x => new DanhMucMenu()
            {
                MenuId = x.MenuId,
                MenuName = x.MenuName,
                MenuUrl = x.MenuUrl,
                ParentMenuId = x.ParentMenuId,
                Position = x.Position,
                PositionIndex = x.PositionIndex
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.ParentMenuId == null).FirstOrDefault();
            SetChildrenMenu(presidents, data);
            return PartialView("_MenuPartial", presidents);
        }

        [Authorize(Roles = "Administrator")]
        private void SetChildrenMenu(DanhMucMenu model, List<DanhMucMenu> danhmuc)
        {
            var childs = danhmuc.Where(x => x.ParentMenuId == model.MenuId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildrenMenu(child, danhmuc);
                    model.DanhMucMenus.Add(child);
                }
            }
        }

        [Authorize(Roles = "Administrator")]
        private void SetChildrenCat(DanhMucBaiViet model, List<DanhMucBaiViet> danhmuc)
        {
            var childs = danhmuc.Where(x => x.ParentCatId == model.CatId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildrenCat(child, danhmuc);
                    model.DanhMucBaiViets.Add(child);
                }
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