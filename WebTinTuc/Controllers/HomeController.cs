using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;
using PagedList;
using PagedList.Mvc;
using System.Xml;
using System.Text;
using Helpers;
using System.Threading.Tasks;
using CaptchaMvc.Attributes;
using System.Globalization;

namespace WebTinTuc.Controllers
{
    public class HomeController : Controller
    {
        private WebTinTucEntities db = new WebTinTucEntities();

        public ActionResult Index()
        {
            ViewBag.ListNewHot = "Tin chính";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult ShowNewsHot()
        {
            var _data = db.Articles.Where(x => x.IsNewHot == true && x.Published == true).OrderByDescending(x => x.PublishedDate).ToList();
            return PartialView("ShowNewsHot", _data);
        }

        [ChildActionOnly]
        public ActionResult ShowCategoryHot()
        {
            var _data = db.Categories.Where(x => x.ViewHome == true && x.Published == true).OrderBy(x => x.PositionHome).ToList();
            return PartialView("ShowCategoryHot", _data);
        }

        [ChildActionOnly]
        public ActionResult ArticleNew(int? id)
        {
            var _article = GetArticleOfCat(id).Where(x=>x.Published == true).OrderByDescending(x=>x.PublishedDate).FirstOrDefault();
            if (_article == null)
            {
                return PartialView();
            }
            return PartialView("ArticleNew", _article);
        }

        [ChildActionOnly]
        public ActionResult ListArticlesNew(int? id)
        {
            var _article = GetArticleOfCat(id).Where(x => x.Published == true).OrderByDescending(x => x.PublishedDate).Skip(1).Take(5).ToList();
            return PartialView("ListArticlesNew", _article);
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

        public IEnumerable<Article> GetArticleOfCat(int? id)
        {
            var _cat = db.Categories.Where(x => x.CategoryId == id).FirstOrDefault();
            List<Article> _articles = new List<Article>();
            if (_cat != null)
            {                
                if (_cat.Categories1.Count > 0)
                {
                    SetArticles(_cat.Categories1, _articles);                     
                }
                else
                {
                    _articles.AddRange(_cat.Articles);
                }
            }
            return _articles;
        }

        public ActionResult Search(string search, int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            if (search == null || search == "")
            {
                return View();
            }
            var result = from a in db.Articles where a.Published == true select a;

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                result = result.Where(x => x.ArticleTitle.ToLower().Contains(search) || x.ArticleContent.Contains(search));
                ViewBag.search = search;
            }

            result = result.OrderByDescending(x => x.PublishedDate);
            return View(result.ToPagedList(pageNumber, pageSize));
        }


        public string generateSiteMap()
        {

            try
            {
                XmlWriterSettings settings = null;
                string xmlDoc = null;
                var p = db.Articles.ToList();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                xmlDoc = HttpRuntime.AppDomainAppPath + "sitemap.xml";//HttpContext.Server.MapPath("../") + 
                float percent = 0.85f;

                string urllink = "";
                using (XmlTextWriter writer = new XmlTextWriter(xmlDoc, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    writer.WriteStartElement("url");

                    writer.WriteElementString("loc", "http://" + Request.Url.Host);
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "1");
                    writer.WriteEndElement();

                    for (int i = 0; i < p.Count; i++)
                    {
                        try
                        {
                            writer.WriteStartElement("url");
                            urllink = "http://" + Request.Url.Host + "/bai-viet/" + p[i].SlugArticleTitle + "-" + p[i].ArticleId;
                            writer.WriteElementString("loc", urllink);
                            //writer.WriteElementString("lastmod", DR["datetime"].ToString());
                            try
                            {
                                if (i < 500)
                                {
                                    writer.WriteElementString("changefreq", "hourly");
                                    percent = 0.85f;
                                }
                                else
                                {
                                    writer.WriteElementString("changefreq", "monthly");
                                    percent = 0.70f;
                                }
                            }
                            catch (Exception ex1)
                            {
                            }

                            writer.WriteElementString("priority", percent.ToString("0.00"));
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {
                        }
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

            }
            catch (Exception extry)
            {
                //StreamWriter sw = new StreamWriter();
            }
            return "ok";
        }

        public ActionResult ResultVote()
        {           
            
            var _vote = (from v in db.Votes select v).FirstOrDefault();           
            int iNormal = _vote.Normal != null ? (int)_vote.Normal : 0;
            int iGood = _vote.Good != null ? (int)_vote.Good : 0;
            int iVeryGood = _vote.VeryGood != null ? (int)_vote.VeryGood : 0;
            int TotalVote = iNormal + iGood + iVeryGood;
            List<VoteModel> xvote = new List<VoteModel>();
            xvote.Add(new VoteModel() { VoteName = "Chưa đẹp", NumberVote = iNormal, colorVote = "#A9080C", PercentVote = Configs.PercentVote(Convert.ToDouble(iNormal), Convert.ToDouble(TotalVote)) });
            xvote.Add(new VoteModel() { VoteName = "Đẹp", NumberVote = iGood, colorVote = "#17CEE7", PercentVote = Configs.PercentVote(Convert.ToDouble(iGood), Convert.ToDouble(TotalVote)) });
            xvote.Add(new VoteModel() { VoteName = "Rất đẹp", NumberVote = iVeryGood, colorVote = "#2d7021", PercentVote = Configs.PercentVote(Convert.ToDouble(iVeryGood), Convert.ToDouble(TotalVote)) });
            ViewBag.TotalVote = TotalVote;
            return View(xvote);
        }

        public ActionResult IframeResultVote()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Votes(string OptionsVote)
        {
            bool boolVote = false;
            //string IP = Request.UserHostName;
            //string compName = Configs.DetermineCompName(IP);
            //if (Configs.GetCookie("IP") == IP && Configs.GetCookie("compName") == compName)
            //{
            //    return Json(boolVote);
            //}
            //Configs.SetCookie("IP", IP, TimeSpan.FromDays(1));
            //Configs.SetCookie("compName", compName, TimeSpan.FromDays(1));
            if (!ModelState.IsValid)
            {
                return Json(boolVote);
            }

            if (OptionsVote == null || OptionsVote == "")
            {
                return Json(boolVote);
            }

            var vote = db.Votes.FirstOrDefault();
            if (vote == null)
            {
                Vote _newVote = new Vote();
                if (OptionsVote == "normal")
                {
                    _newVote.Normal = 1;
                }
                else if (OptionsVote == "good")
                {
                    _newVote.Good = 1;
                }
                else if (OptionsVote == "verygood")
                {
                    _newVote.VeryGood = 1;
                }
                db.Votes.Add(_newVote);
                db.SaveChanges();
                boolVote = true;
            }
            else
            {
                if (OptionsVote == "normal")
                {
                    vote.Normal = vote.Normal != null ? vote.Normal+=1 : 1;
                }
                else if (OptionsVote == "good")
                {
                    vote.Good = vote.Good != null ? vote.Good+=1 : 1;
                }else if (OptionsVote == "verygood")
	            {
                    vote.VeryGood = vote.VeryGood != null ? vote.VeryGood += 1 : 1;
	            }
                db.SaveChanges();
                boolVote = true;
            }

            return Json(boolVote);
        }

        public ActionResult LienHePage()
        {

            return View();
        }

        [HttpPost]
        public JsonResult Support(SupportModel model)
        {
            bool boolSupport = false;
            if (!ModelState.IsValid)
            {
                return Json(boolSupport);
            }
            try
            {
                Support _newSupport = new Support();
                _newSupport.FullName = model.FullName ?? null;
                _newSupport.Address = model.Address ?? null;
                _newSupport.Email = model.Email ?? null;
                _newSupport.SupportType = model.SupportType ?? null;
                _newSupport.SupportContent = model.SupportContent ?? null;
                _newSupport.CreatedDate = DateTime.Now;
                db.Supports.Add(_newSupport);
                db.SaveChanges();
                boolSupport = true;
            }
            catch (Exception ex)
            {
                return Json(boolSupport);
            }            
            return Json(boolSupport);
        }

        public ActionResult SoDoTrangWeb()
        {
            return View();
        }

        private void SetChildren(DanhMucSoDoTrang model, List<DanhMucSoDoTrang> danhmuc)
        {
            var childs = danhmuc.Where(x => x.ParentId == model.CatId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildren(child, danhmuc);
                    model.DanhMucSoDoTrangs.Add(child);
                }
            }
        }

        public IEnumerable<Menu> LoadMenus()
        {
            var data = from x in db.Menus select x;
            return data;
        }

        [ChildActionOnly]
        public ActionResult _DanhMucMenuPartial()
        {
            List<DanhMucSoDoTrang> data = LoadMenus().Where(o => o.Published == true && o.Position == "left").Select(x => new DanhMucSoDoTrang()
            {
                CatId = x.MenuId,
                CatName = x.MenuName,
                ParentId = x.ParentMenuId,
                PositionIndex = x.PositionIndex,
                CatMenu = x.MenuUrl
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.ParentId == null).FirstOrDefault();
            SetChildren(presidents, data);

            return PartialView("_DanhMucMenuPartial", presidents);
        }

        [ChildActionOnly]
        public ActionResult ShowThongBaoNoiBat()
        {
            var data = db.Embeds.FirstOrDefault();
            return PartialView("ShowThongBaoNoiBat", data);
        }

        public ActionResult DanhGiaDonVi()
        {
            return View();
        }

        public ActionResult KhaoSatCongChuc()
        {
            return View();
        }

        public PartialViewResult _KetQuaKhaoSatCongChucPartial()
        {
            return PartialView("_KetQuaKhaoSatCongChucPartial", LoadDanhMucDonVi());
        }

        public PartialViewResult _KhaoSatCongChucPartial()
        {
            return PartialView("_KhaoSatCongChucPartial", LoadDanhMucDonVi());
        }

        public PartialViewResult _KhaoSatDonViPartial()
        {
            return PartialView("_KhaoSatDonViPartial", LoadDanhMucDonVi());
        }

        public PartialViewResult _XemKetQuaKhaoSatDanhSachDonViPartial()
        {
            return PartialView("_XemKetQuaKhaoSatDanhSachDonViPartial", LoadDanhMucDonVi());
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

        public IEnumerable<DonVi> LoadDonVis()
        {
            var query = "select * from DonVis";
            var data = db.Database.SqlQuery<DonVi>(query);
            return data;
        }

        public ActionResult KetQuaChungDanhSachCongChucOfDonVi(int? id, string slugurl, int? pg)
        {
            if (id == null || id == 0 || slugurl == null || slugurl == "")
            {
                return RedirectToRoute("NotFound");
            }
            var _donvi = db.DonVis.Where(x => x.DonViId == id && x.SlugDonVi == slugurl).FirstOrDefault();
            if (_donvi == null)
            {
                return View();
            }

            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            ViewBag.TenDonVi = _donvi.TenDonVi;
            var _congchucs = _donvi.CongChucs.Where(x => x.Published == true);
            if (_congchucs == null)
            {
                return View(_congchucs);
            }
            _congchucs = _congchucs.OrderBy(x => x.TenCongChuc);
            return View(_congchucs.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DanhSachCongChucOfDonVi(int? id, string slugurl, int? pg)
        {
            if (id == null || id == 0 || slugurl == null || slugurl == "")
            {
                return RedirectToRoute("NotFound");
            }
            var _donvi = db.DonVis.Where(x => x.DonViId == id && x.SlugDonVi == slugurl).FirstOrDefault();
            if (_donvi == null)
            {
                return View();
            }

            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            ViewBag.TenDonVi = _donvi.TenDonVi;
            var _congchucs = _donvi.CongChucs.Where(x=>x.Published == true);
            if (_congchucs == null)
            {
                return View(_congchucs);
            }
            _congchucs = _congchucs.OrderBy(x => x.TenCongChuc);
            return View(_congchucs.ToPagedList(pageNumber, pageSize));
        }

        //RateDanhGiaCongChuc
        public ActionResult RateDanhGiaCongChuc(int? id, string slugurl)
        {
            if (id == null || id == 0)
            {
                return RedirectToRoute("NotFound");
            }
            ViewBag.getId = id;
            ViewBag.getSlugUrl = slugurl;
            var data = db.CongChucs.Where(x => x.CongChucId == id && x.Published == true).FirstOrDefault();
            if (data != null)
            {
                ViewBag.TenCongChuc = data.TenCongChuc;
                ViewBag.DonViId = data.DonViId;
                ViewBag.CongChucId = data.CongChucId;
            }
            return View();
        }

        public ActionResult RateDanhGiaDonVi(int? id, string slugurl)
        {
            if (id == null || id == 0 || slugurl == null || slugurl == "")
            {
                return RedirectToRoute("NotFound");
            }
            ViewBag.getId = id;
            ViewBag.getSlugUrl = slugurl;
            var data = db.DonVis.Where(x => x.DonViId == id && x.SlugDonVi == slugurl && x.Published == true).FirstOrDefault();
            if (data != null)
            {
                ViewBag.TenDonVi = data.TenDonVi;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("Mã bảo mật nhập không đúng")]
        public async Task<ActionResult> RateDanhGiaCongChuc(DanhGiaCongChucModel model, IEnumerable<int> LydoId)
        {
            if (!ModelState.IsValid)
            {
                //ModelState.AddModelError("", "Mã bảo mật nhập không đúng");
                TempData["Error"] = "Mã bảo mật nhập không đúng";
                return RedirectToAction("RateDanhGiaCongChuc");
            }
            try
            {
                DanhGiaCongChuc _danhgiaCongChuc = new DanhGiaCongChuc();
                _danhgiaCongChuc.CongChucId = model.CongChucId ?? null;
                _danhgiaCongChuc.NguoiDanhGia = model.NguoiDanhGia ?? null;
                _danhgiaCongChuc.NgayDanhGia = DateTime.Now;
                _danhgiaCongChuc.SoHoSo = model.SoHoSo ?? null;
                _danhgiaCongChuc.DonViId = model.DonViId ?? null;
                _danhgiaCongChuc.KieuDanhGia = model.KieuDanhGia;
                db.DanhGiaCongChucs.Add(_danhgiaCongChuc);
                if (model.KieuDanhGia == false)
                {
                    if (LydoId.Count() > 0)
                    {
                        foreach (var item in LydoId)
                        {
                            var _lydo = db.Lydoes.Where(x => x.LyDoId == item).FirstOrDefault();
                            if (_lydo != null)
                            {
                                //var lydo = new Lydo { LyDoId = _lydo.LyDoId };
                                //db.Lydoes.Attach(lydo); // this avoids duplicate Lydoes
                                _danhgiaCongChuc.Lydoes.Add(_lydo);
                                await db.SaveChangesAsync();
                                // I assume here that _danhgiaDonVi.Lydoes was empty before
                            }
                        }
                    }
                }
                else
                {
                    await db.SaveChangesAsync();
                }

                TempData["Updated"] = @"“Xin trân trọng cảm ơn ý kiến đóng góp của Quý ông/bà đối với cán bộ, công chức và công tác cung ứng dịch vụ hành chính công của các cơ quan, đơn vị trên địa bàn thành phố.	
<br /> Chúng tôi sẽ nghiêm túc tiếp thu ý kiến và chỉ đạo các đơn vị thực hiện các giải pháp nâng cao chất lượng phục vụ tổ chức, công dân. 
<br /> Rất mong nhận được sự đánh giá tốt của Quý ông/bà trong thời gian sắp tới!”";
                return RedirectToAction("RateDanhGiaCongChuc");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đánh giá.");
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("Mã bảo mật nhập không đúng")]
        public async Task<ActionResult> RateDanhGiaDonVi(DanhGiaDonViModel model, IEnumerable<int> LydoId)
        {
            if (!ModelState.IsValid)
            {
                //ModelState.AddModelError("", "Mã bảo mật nhập không đúng");
                TempData["Error"] = "Mã bảo mật nhập không đúng";
                return RedirectToAction("RateDanhGiaDonVi");
            }
            try
            {
                DanhGiaDonVi _danhgiaDonVi = new DanhGiaDonVi();
                _danhgiaDonVi.DonViId = model.DonViId ?? null;
                _danhgiaDonVi.NguoiDanhGia = model.NguoiDanhGia ?? null;
                _danhgiaDonVi.NgayDanhGia = DateTime.Now;
                _danhgiaDonVi.SoHoSo = model.SoHoSo ?? null;
                _danhgiaDonVi.LinhVucId = model.LinhVucId ?? null;
                _danhgiaDonVi.KieuDanhGia = model.KieuDanhGia;
                db.DanhGiaDonVis.Add(_danhgiaDonVi);

                if (model.KieuDanhGia == false)
                {
                    if (LydoId.Count() > 0)
                    {
                        foreach (var item in LydoId)
                        {
                            var _lydo = db.Lydoes.Where(x => x.LyDoId == item).FirstOrDefault();
                            if (_lydo != null)
                            {
                                //var lydo = new Lydo { LyDoId = _lydo.LyDoId };
                                //db.Lydoes.Attach(lydo); // this avoids duplicate Lydoes
                                _danhgiaDonVi.Lydoes.Add(_lydo);
                                await db.SaveChangesAsync();
                                // I assume here that _danhgiaDonVi.Lydoes was empty before
                            }
                        }
                    }                    
                }
                else
                {
                    await db.SaveChangesAsync();
                }

                TempData["Updated"] = @"“Xin trân trọng cảm ơn ý kiến đóng góp của Quý ông/bà đối với cán bộ, công chức và công tác cung ứng dịch vụ hành chính công của các cơ quan, đơn vị trên địa bàn thành phố.	
<br /> Chúng tôi sẽ nghiêm túc tiếp thu ý kiến và chỉ đạo các đơn vị thực hiện các giải pháp nâng cao chất lượng phục vụ tổ chức, công dân. 
<br /> Rất mong nhận được sự đánh giá tốt của Quý ông/bà trong thời gian sắp tới!”";
                return RedirectToAction("RateDanhGiaDonVi");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đánh giá.");
                return View(model);
            }
        }

        [ChildActionOnly]
        public ActionResult _selectLinhVuc(int? id, string slugurl)
        {
            var data = LoadLinhVuc(id, slugurl);
            return PartialView("_selectLinhVuc", data);
        }

        public List<LinhVuc> LoadLinhVuc(int? donviId, string slugUrlDonVi)
        {
            var data = db.DonVis.Where(x => x.DonViId == donviId && x.SlugDonVi == slugUrlDonVi).FirstOrDefault();
            if (data == null)
            {
                return null;
            }
            
            var GetLinhVucs = data.LinhVucs.Where(x => x.Published == true).OrderBy(x => x.PositionIndex).ToList();
            return GetLinhVucs;
        }

        [ChildActionOnly]
        public ActionResult _listLyDoPartial()
        {
            var query = "select * from Lydo";
            var data = db.Database.SqlQuery<Lydo>(query);
            return PartialView("_listLyDoPartial", data.ToList());
        }

        public ActionResult XemKetQuaDanhGiaCongChuc(int? id, string slugUrl)
        {
            if (id == null || id == 0 || slugUrl == null || slugUrl == "")
            {
                return RedirectToRoute("NotFound");
            }
            var data = db.CongChucs.Where(x => x.CongChucId == id ).FirstOrDefault();
            if (data == null)
            {
                return View(data);
            }
            ViewBag.TenCongChuc = data.TenCongChuc;
            return View(data);
        }

        public ActionResult XemKetQuaDanhGiaDonVi(int? id, string slugurl)
        {
            if (id == null || id == 0 || slugurl == null || slugurl == "")
            {
                return RedirectToRoute("NotFound");
            }
            var data = db.DonVis.Where(x => x.DonViId == id && x.SlugDonVi == slugurl && x.Published == true).FirstOrDefault();
            if (data == null)
            {
                return View(data);
            }
            ViewBag.TenDonVi = data.TenDonVi;
            return View(data);
        }

        public ActionResult XemKetQuaChungCongChuc()
        {
            return View();
        }

        public ActionResult XemKetQuaDanhSachDonVi()
        {
            return View();
        }

        public ActionResult XemKetQuaChungDanhGiaDonVi(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            var data = db.DonVis.Where(x => x.MaDonViCha != null && x.DonVis1.Count == 0 && x.Published == true).ToList();
            var getKetqua = data.Select(x => new PhanTramDanhGiaDonViModel()
            {
                DonViId = x.DonViId,
                TenDonVi = x.TenDonVi,
                SlugUrl = x.SlugDonVi,
                LuotHaiLong = x.DanhGiaDonVis.Where(a => a.KieuDanhGia == true).Count(),
                LuotChuaHaiLong = x.DanhGiaDonVis.Where(a => a.KieuDanhGia == false).Count(),
                SoLuotDanhGia = x.DanhGiaDonVis.Count
            }).ToList();
            return View(getKetqua.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddNewQuestion()
        {
            var listLinhVuc = db.HoiDapLinhVucs.Where(r=>r.LinhVucChaId != null).OrderBy(r => r.ViTriLinhVuc).ToList().Select(rr => new SelectListItem { Value = rr.LinhVucId.ToString(), Text = rr.TenLinhVuc }).ToList();
            ViewBag.ListLinhVucHoiDap = listLinhVuc;
            var listDonVi = db.AspNetUsers.OrderBy(r => r.FullName).ToList().Select(rr => new SelectListItem { Value = rr.Id.ToString(), Text = rr.FullName }).ToList();
            ViewBag.ListDonViTiepNhan = listDonVi;
            return View();
        }

        [AllowAnonymous]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        //[CaptchaMvc.Attributes.CaptchaVerify("Mã bảo mật nhập không đúng")]
        public async Task<ActionResult> AddNewQuestion(HoiDapModel model)
        {
            if (!ModelState.IsValid)
            {                
                TempData["Error"] = "Vui lòng kiểm tra lại các trường";
                return RedirectToAction("AddNewQuestion");
            }
            try
            {
                HoiDapCauHois _newCauHoi = new HoiDapCauHois();
                _newCauHoi.TenCauHoi = model.TenCauHoi ?? null;
                _newCauHoi.LinhVucHoiDapId = (int?)model.LinhVucHoiDapId ?? null;
                _newCauHoi.DonViTiepNhanId = model.DonViTiepNhanId ?? null;
                _newCauHoi.HoTenNguoiHoi = model.HoTenNguoiHoi ?? null;
                _newCauHoi.DiaChiNguoiHoi = model.DiaChiNguoiHoi ?? null;
                _newCauHoi.EmailNguoiGui = model.EmailNguoiGui ?? null;
                _newCauHoi.SoDienThoaiNguoiHoi = model.SoDienThoaiNguoiHoi ?? null;
                _newCauHoi.NoiDungCauHoi = model.NoiDungCauHoi ?? null;
                _newCauHoi.TrangThaiCauHoi = false;
                _newCauHoi.NgayHoiDap = DateTime.Now;
                db.HoiDapCauHois.Add(_newCauHoi);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Bạn đã gửi câu hỏi tới đơn vị tiếp nhận thành công.";
                return RedirectToAction("AddNewQuestion");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi gửi câu hỏi.";
                return RedirectToAction("AddNewQuestion");
            }
        }

        [ChildActionOnly]
        public ActionResult _CauhoiLienQuanPartial(long? id, int? linhvucid) {
            var data = db.HoiDapCauHois.Where(x=>x.LinhVucHoiDapId == linhvucid && x.TrangThaiCauHoi == true && x.CauHoiId != id).OrderByDescending(x=>x.NgayTraLoi).Take(20).ToList();
            return PartialView("_CauhoiLienQuanPartial", data);
        }

        public ActionResult ChiTietCauHoi(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }

            var data = db.HoiDapCauHois.Where(x=>x.TrangThaiCauHoi == true && x.CauHoiId == id).FirstOrDefault();
            if (data == null)
            {
                return View(data);
            }
            ViewBag.TenCauHoi = data.TenCauHoi;
            data.LuotXem = data.LuotXem != null ? data.LuotXem += 1 : 1;
            db.SaveChanges();
            return View(data);
        }

        [ChildActionOnly]
        public ActionResult _NgayThangPartial()
        {
            return PartialView("_NgayThangPartial");
        }

        public ActionResult TraCuuCauHoi(int? pg, int? LinhVucHoiDapId, string search_cauhoi, NgayThangModel model)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            IEnumerable<HoiDapCauHois> data = new List<HoiDapCauHois>();
            if (LinhVucHoiDapId != null && LinhVucHoiDapId != 0)
            {
                //data = data.Where(x => x.DocumentId == DocumentId);
                ViewBag.LinhVucHoiDapId = LinhVucHoiDapId;
                ViewBag.TenLinhVuc = db.HoiDapLinhVucs.Find(LinhVucHoiDapId).TenLinhVuc;
                data = GetCauHoiDetailOfLinhVuc(LinhVucHoiDapId).Where(x=>x.TrangThaiCauHoi ==true);
            }
            else
            {
                ViewBag.TenLinhVuc = "Tất cả lĩnh vực";
                data = from d in db.HoiDapCauHois where d.TrangThaiCauHoi == true select d;
            }

            if (!string.IsNullOrEmpty(search_cauhoi))
            {
                search_cauhoi = search_cauhoi.Trim();
                data = data.Where(x => x.TenCauHoi.ToLower().Contains(search_cauhoi));
                ViewBag.search_cauhoi = search_cauhoi;
            }

            if (model.fromdate != null && model.todate!= null)
            {
                //var xx = Configs.MDYToDMY(model.fromdate.ToString());
                //var yy = Configs.MDYToDMY(model.todate.ToString());
                //data = data.Where(x => x.NgayHoiDap > xx && x.NgayHoiDap < yy);
                //ViewBag.FromDate = xx.ToShortDateString();
                //ViewBag.ToDate = yy.ToShortDateString();
                data = data.Where(x => x.NgayHoiDap > model.fromdate && x.NgayHoiDap < model.todate);
                ViewBag.FromDate = Convert.ToDateTime(model.fromdate).ToString("yyyy-MM-dd");
                ViewBag.ToDate = Convert.ToDateTime(model.todate).ToString("yyyy-MM-dd");
                
            }
            else if (model.todate != null)
            {
                //var yy = Configs.MDYToDMY(model.todate.ToString());
                data = data.Where(x => x.NgayHoiDap < model.todate);
                ViewBag.ToDate = Convert.ToDateTime(model.todate).ToString("yyyy-MM-dd");
            }
            else if (model.fromdate != null)
            {
                //var xx = Configs.MDYToDMY(model.fromdate.ToString());
                data = data.Where(x => x.NgayHoiDap > model.fromdate);
                ViewBag.FromDate = Convert.ToDateTime(model.fromdate).ToString("yyyy-MM-dd");
            }

            data = data.OrderByDescending(a => a.NgayHoiDap);

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public void SetCauHoiDetail(ICollection<HoiDapLinhVuc> ic, List<HoiDapCauHois> _cauhoiDetail)
        {
            foreach (var c1 in ic)
            {
                if (c1.HoiDapCauHois.Count > 0)
                {
                    _cauhoiDetail.AddRange(c1.HoiDapCauHois);
                }
                if (c1.HoiDapLinhVucs1.Count > 0)
                {
                    SetCauHoiDetail(c1.HoiDapLinhVucs1, _cauhoiDetail);
                }
            }
        }

        public IEnumerable<HoiDapCauHois> GetCauHoiDetailOfLinhVuc(int? id)
        {
            var _linhvuc = db.HoiDapLinhVucs.Where(x => x.LinhVucId == id).FirstOrDefault();
            List<HoiDapCauHois> _cauhois = new List<HoiDapCauHois>();
            if (_cauhois != null)
            {
                //ViewBag.category = _cat.CategoryName;
                if (_linhvuc.HoiDapLinhVucs1.Count > 0)
                {
                    SetCauHoiDetail(_linhvuc.HoiDapLinhVucs1, _cauhois);
                }
                else
                {
                    _cauhois.AddRange(_linhvuc.HoiDapCauHois);
                }
            }
            else
            {
                _cauhois = null;
            }
            return _cauhois;
        }


        private IEnumerable<HoiDapLinhVuc> LoadLinhVucs()
        {
            var data = from x in db.HoiDapLinhVucs select x;
            return data;
        }

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

       
    }
}



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
//                _articles.AddRange(c2.Articles);
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