using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helpers;
using WebTinTuc.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ArticlesController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
       
        public ActionResult Index()
        {
            ViewBag.TypePost = Configs.ListTypePost();
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewArticle(NewArticle model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("AddNewArticle");
            }
            string userId = User.Identity.GetUserId();
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    Article _newArticle = new Article();
                    _newArticle.ArticleTitle = model.ArticleTitle ?? null;
                    _newArticle.ArticleDescription = model.ArticleDescription ?? null;
                    _newArticle.ArticleImageSmall = model.ArticleImageSmall != null ? model.ArticleImageSmall : "/Images/no-image.png";
                    _newArticle.ArticleImageBig = model.ArticleImageBig ?? null;
                    _newArticle.ArticleContent = model.ArticleContent ?? null;
                    _newArticle.CategoryId = model.CategoryId ?? (int?)null;
                    _newArticle.CreatedDate = DateTime.Now;
                    _newArticle.IsNewHot = model.IsNewHot;
                    _newArticle.ModifiedDate = null;
                    _newArticle.SlugArticleTitle = model.SlugArticleTitle != null ? model.SlugArticleTitle : Configs.unicodeToNoMark(model.ArticleTitle);
                    _newArticle.StatusPost = model.Published == true ? "Đã đăng" : "Chưa đăng";
                    _newArticle.Tags = model.Tags ?? null;
                    _newArticle.TypePost = model.TypePost ?? "new";
                    _newArticle.ViewCount = null;
                    _newArticle.UserId = userId;
                    _newArticle.Published = model.Published;
                    _newArticle.PublishedDate = model.Published == true ? DateTime.Now : (DateTime?)null;
                    db.Articles.Add(_newArticle);
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Thêm mới bài viết thành công";
                    TempData["Category"] = model.CategoryId;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm mới bài viết");
                return View(model);
            }
            
            return RedirectToRoute("AddNewArticle");
            
        }

        public IEnumerable<Category> LoadCategories()
        {
            var query = "select * from Categories";
            var data = db.Database.SqlQuery<Category>(query);
            return data;
        }

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
                        string strDay = DateTime.Now.ToString("yyyyMMdd");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), strDay);

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "/Images/WallImages/" + strDay +"/" + _fileName;
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

        public ActionResult ManagerArticle(int? pg, string search, string type, string status, string isnewhot)
        {
            var data = db.Articles.Select(a=>a);            
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.ArticleTitle.ToLower().Contains(search));
                ViewBag.search = search;
            }

            if (type == null) type = ""; if (status == null) status = ""; if (isnewhot == null) isnewhot = "";

            if (type != null && type != "")
            {
                data = data.Where(t => t.TypePost == type);
                ViewBag.type = type;
            }

            if (status != null && status != "")
            {
                data = data.Where(s => s.StatusPost == status);
                ViewBag.status = status;
            }

            if (isnewhot != null && isnewhot != "")
            {
                bool bisnewhot = Convert.ToBoolean(isnewhot);
                data = data.Where(x => x.IsNewHot == bisnewhot);
                ViewBag.isnewhot = isnewhot;
            }

            data = data.OrderByDescending(d => d.CreatedDate);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> EditArticle(long? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            Article _article = await db.Articles.FindAsync(id);
            if (_article == null)
            {
                return View(_article);
            }
            var _articleBind = new EditArticle()
            {
               ArticleId = _article.ArticleId,
               ArticleTitle = _article.ArticleTitle,
               ArticleDescription = _article.ArticleDescription,
               ArticleContent = _article.ArticleContent,
               ArticleImageSmall = _article.ArticleImageSmall,
               ArticleImageBig = _article.ArticleImageBig,
               CategoryId = _article.CategoryId,
               IsNewHot = (bool)_article.IsNewHot,
               SlugArticleTitle = _article.SlugArticleTitle,
               Tags = _article.Tags,
               TypePost = _article.TypePost,
               Published = (bool)_article.Published
            };
            ViewBag.TypePost = Configs.ListTypePost();
            return View(_articleBind);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditArticle(EditArticle model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditArticle");
            }
            var _article = await db.Articles.FindAsync(model.ArticleId);
            string userId = User.Identity.GetUserId();
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    if (_article != null)
                    {
                        _article.ArticleTitle = model.ArticleTitle ?? null;
                        _article.ArticleDescription = model.ArticleDescription ?? null;
                        _article.ArticleImageSmall = model.ArticleImageSmall != null ? model.ArticleImageSmall : "/Images/no-image.png";
                        _article.ArticleImageBig = model.ArticleImageBig ?? null;
                        _article.ArticleContent = model.ArticleContent ?? null;
                        _article.CategoryId = model.CategoryId ?? (int?)null;
                        _article.IsNewHot = model.IsNewHot;
                        _article.ModifiedDate = DateTime.Now;
                        _article.SlugArticleTitle = model.SlugArticleTitle != null ? model.SlugArticleTitle : Configs.unicodeToNoMark(model.ArticleTitle);
                        _article.StatusPost = model.Published == true ? "Đã đăng" : "Chưa đăng";
                        _article.Tags = model.Tags ?? null;
                        _article.TypePost = model.TypePost ?? "new";
                        _article.ViewCount = null;
                        _article.UserId = userId;
                        _article.Published = model.Published;
                        if (model.GiuNgayDang == true)
                        {
                            _article.PublishedDate = _article.CreatedDate;
                        }
                        else
                        {
                            _article.PublishedDate = model.Published == true ? DateTime.Now : (DateTime?)null;
                        }
                        await db.SaveChangesAsync();                                  
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa bài viết");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật bài viết thành công";             
            return RedirectToAction("EditArticle");
        }

        public ActionResult DeleteArticle(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Article _article = db.Articles.Find(id);
            if (_article == null)
            {
                return View();
            }
            return View(_article);
        }

        [HttpPost, ActionName("DeleteArticle")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteArticleConfirmed(long? id)
        {
            Article _article = await db.Articles.FindAsync(id);
            if (_article == null)
            {
                return View();
            }

            db.Articles.Remove(_article);
            await db.SaveChangesAsync();
            TempData["Updated"] = "Xóa bài viết thành công";
            return RedirectToAction("ManagerArticle");
        }

        [AllowAnonymous]
        public ActionResult Detail(string slugurl, long? id)
        {
            var _article = db.Articles.Where(a => a.SlugArticleTitle == slugurl && a.ArticleId == id && a.Published == true).FirstOrDefault();
            if (_article == null)
            {
                return View();
            }
            // Tăng lượt view lên
            _article.ViewCount = _article.ViewCount != null ? _article.ViewCount += 1 : 1;
            db.SaveChanges();
            return View(_article);
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ArticleWithCatelog(long? id)
        {
            var _article = db.Articles.Where(x => x.ArticleId == id).FirstOrDefault();
            IEnumerable<Article> _listArticles = new List<Article>();
            if (_article != null)
            {
                _listArticles = db.Articles.Where(x => x.Published == true && x.CategoryId == _article.CategoryId && x.ArticleId != id).OrderByDescending(x => x.PublishedDate).Take(25).ToList();
            }
            return PartialView("ArticleWithCatelog", _listArticles);
        }

        [AllowAnonymous]
        public ActionResult CategoryArticles(int? pg, string slugurl, int? id)
        {
            var _cat = db.Categories.Where(x => x.CategoryId == id && x.SlugCategory == slugurl).FirstOrDefault();
            if (_cat == null)
            {
                return RedirectToRoute("NotFound");
            }
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var _articles = GetArticleOfCat(id, slugurl);

            if (_articles.Count() == 0)
            {
                return View();
            }

            _articles = _articles.Where(x=>x.Published == true).OrderByDescending(x => x.PublishedDate).ToList();
            return View(_articles.ToPagedList(pageNumber, pageSize));
        }

        public void SetArticles(ICollection<Category> ic, List<Article> _articles)
        {
            foreach (var c1 in ic)
            {
                if (c1.Articles.Count > 0)
                {
                    _articles.AddRange(c1.Articles);
                }
                if (c1.Categories1.Count > 0)
                {
                    SetArticles(c1.Categories1, _articles);
                }
            }         
        }

        
        public IEnumerable<Article> GetArticleOfCat(int? id, string slugUrl)
        {
            var _cat = db.Categories.Where(x => x.CategoryId == id && x.SlugCategory == slugUrl).FirstOrDefault();
            List<Article> _articles = new List<Article>();
            if (_cat != null)
            {
                ViewBag.category = _cat.CategoryName;
                if (_cat.Categories1.Count > 0)
                {
                    SetArticles(_cat.Categories1, _articles);                              
                }
                else
                {
                    _articles.AddRange(_cat.Articles);
                }
            }
            else
            {
                _articles = null;
            }
            return _articles;
        }


        //public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        //{
        //    var stack = new Stack<T>(items);
        //    while (stack.Any())
        //    {
        //        var next = stack.Pop();
        //        yield return next;
        //        foreach (var child in childSelector(next))
        //            stack.Push(child);
        //    }
        //}

        // Code này cách đây ngày xửa ngày xưa rồi
        //var ListCat = (from c in db.Categories where c.ParentCategoryId == _cat.CategoryId select new { c.CategoryId }).ToList();
        //List<int?> intListCat = ListCat.Select(x => (int?)x.CategoryId).ToList();
        //var _data = (from d in db.Articles
        //             group d by d.ArticleId into g
        //             where g.Any(x => intListCat.Contains(x.CategoryId))
        //             select g);
        //foreach (var item in _data)
        //{
        //    foreach (var a in item)
        //    {
        //        _articles.Add(a);
        //    }
        //}

        // Code này từ năm ngoài rồi:
        //foreach (var c1 in _cat.Categories1)
        //{
        //    if (c1.Articles.Count > 0)
        //    {
        //        _articles.AddRange(c1.Articles);
        //    }
        //    if (c1.Categories1.Count > 0)
        //    {
        //        foreach (var c2 in c1.Categories1)
        //        {
        //            if (c2.Articles.Count > 0)
        //            {
        //                 _articles.AddRange(c2.Articles);
        //            }
        //            if (c2.Categories1.Count > 0)
        //            {
        //                foreach (var c3 in c2.Categories1)
        //                {
        //                    if (c3.Articles.Count > 0)
        //                    {
        //                        _articles.AddRange(c3.Articles);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}   

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