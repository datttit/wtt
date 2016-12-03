using System.Web.Mvc;
using Helpers;
using System;
using System.Collections.Generic;
using WebTinTuc.Models;
using System.Linq;

namespace WebTinTuc.Controllers
{
    public class LayoutController : Controller
    {
        private WebTinTucEntities db = new WebTinTucEntities();

        // GET: Layout
        public PartialViewResult _HeaderPartial()
        {
            var model = LoadConfigs();
            return PartialView("_HeaderPartial", model);            
        }

        public Config LoadConfigs()
        {
            string query = "select top 1 * from Configs";
            var _config = db.Database.SqlQuery<Config>(query);     
            return _config.FirstOrDefault();
        }

        public PartialViewResult _FooterPartial()
        {
            var model = LoadConfigs();
            return PartialView("_FooterPartial", model);
        }

        public PartialViewResult _MenuTopPartial()
        {
            var menutop = LoadMenus().Where(x=>x.Published == true && x.Position == "top").ToList();
            var menucha = LoadMenus().Where(x=>x.Published == true && x.ParentMenuId == null).FirstOrDefault();
            menutop.Add(menucha);
            //var model = LoadMenus().Where(o => o.Published == true && o.Position == "top" && o.Menus1).OrderBy(o => o.PositionIndex).ToList();
            List<DanhMuc> data = menutop.Select(x => new DanhMuc()
            {
                CatId = x.MenuId,
                CatName = x.MenuName,
                ParentId = x.ParentMenuId,
                PositionIndex = x.PositionIndex,
                CatMenu = x.MenuUrl
            }).OrderBy(o => o.PositionIndex).ToList(); 

            var presidents = data.Where(x => x.ParentId == null).FirstOrDefault();
            SetChildren(presidents, data);

            return PartialView("_MenuTopPartial", presidents);
        }

        public PartialViewResult _MenuBottomPartial()
        {
            var model = LoadMenus().Where(o => o.Published == true && o.Position == "bottom").OrderBy(o => o.PositionIndex).ToList();
            return PartialView("_MenuBottomPartial", model);
        }       

        public PartialViewResult _BelowMenuPartial()
        {
            var model = Configs.GetNgay();
            return PartialView("_BelowMenuPartial", model);
        }

        public PartialViewResult _MenuLinhVucPartial()
        {
            List<DanhMucLinhVucHoiDap> data = LoadDanhMucLinhVuc().Where(o => o.Published == true).Select(x => new DanhMucLinhVucHoiDap()
            {
                LinhVucId = x.LinhVucId,
                TenLinhVuc = x.TenLinhVuc,
                PositionIndex = x.ViTriLinhVuc,
                SlugLinhVuc = x.DiaChiTruyCap,
                LinhVucChaId = x.LinhVucChaId
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.LinhVucChaId == null).FirstOrDefault();
            SetChildrenDanhMucLinhVuc(presidents, data);

            return PartialView("_MenuLinhVucPartial", presidents);
        }



        private void SetChildrenDanhMucLinhVuc(DanhMucLinhVucHoiDap model, List<DanhMucLinhVucHoiDap> danhmuc)
        {
            var childs = danhmuc.Where(x => x.LinhVucChaId == model.LinhVucId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildrenDanhMucLinhVuc(child, danhmuc);
                    model.DanhMucLinhVucHoiDaps.Add(child);
                }
            }
        }

        public PartialViewResult _MenuLeftPartial()
        {
            List<DanhMuc> data = LoadMenus().Where(o => o.Published == true && o.Position == "left").Select(x=>new DanhMuc() {
                CatId = x.MenuId,
                CatName = x.MenuName,
                ParentId = x.ParentMenuId,
                PositionIndex = x.PositionIndex,
                CatMenu = x.MenuUrl
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.ParentId == null).FirstOrDefault();
            SetChildren(presidents, data);

            return PartialView("_MenuLeftPartial", presidents);
        }

        private void SetChildren(DanhMuc model, List<DanhMuc> danhmuc)
        {
            var childs = danhmuc.Where(x => x.ParentId == model.CatId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildren(child, danhmuc);
                    model.DanhMucs.Add(child);
                }
            }
        }

        public IEnumerable<Menu> LoadMenus()
        {
            var data = from x in db.Menus select x;
            return data;
        }

        public IEnumerable<HoiDapLinhVuc> LoadDanhMucLinhVuc()
        {
            var data = from x in db.HoiDapLinhVucs select x;
            return data;
        }

        public string Logs()
        {
            try
            {
                var f = db.AccessStatistics.Select(o=>o).FirstOrDefault();
                if (f != null)
                {
                    int _fid = f.A1;
                    db.Database.ExecuteSqlCommand("update AccessStatistics set A2=A2+1 where A1=" + _fid +"");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO AccessStatistics (A2) VALUES (1)");
                }
               
                return f.A2.ToString();
            }
            catch (Exception ex)
            {
                return "0";
            }
        } 

        [ChildActionOnly]
        public ActionResult _ModuleCongVan()
        {
            var data = db.Articles.Where(x => x.TypePost == "congvan" && x.Published == true).OrderByDescending(x => x.PublishedDate).Take(20).ToList();
            return PartialView("_ModuleCongVan", data);
        }

        [ChildActionOnly]
        public ActionResult _ModuleThongBao()
        {
            var data = db.DocumentDetails.Where(x => x.Published == true).OrderByDescending(x => x.PublishedDate).Take(20).ToList();
            return PartialView("_ModuleThongBao", data);
        }

        [ChildActionOnly]
        public ActionResult _ModuleVideo()
        {
            var data = db.Videos.Where(x => x.Published == true).OrderByDescending(x=>x.IsVideoTop).ToList();
            return PartialView("_ModuleVideo", data);
        }

        [ChildActionOnly]
        public ActionResult _ModuleQuangCao1()
        {
            var data = db.Advs.Where(x => x.TypeAdv == "quangcao1").FirstOrDefault();
            List<AdvContent> xx = new List<AdvContent>();
            if (data != null)
            {
                xx = data.AdvContents.ToList();
            }
            return PartialView("_ModuleQuangCao1", xx);
        }

        public ActionResult _ModuleLeftPartial()
        {
            var data = db.Modules.Where(x => x.Position == "left" && x.Published == true).OrderBy(x => x.PositionIndex).ToList();
            return PartialView("_ModuleLeftPartial", data);
        }

        public ActionResult _ModuleBanDo()
        {
            var data = db.Maps.Where(x=>x.Published == true).Select(x=>x).FirstOrDefault();
            return PartialView("_ModuleBanDo", data);
        }

        public ActionResult _ModuleRightPartial()
        {
            var data = db.Modules.Where(x => x.Position == "right" && x.Published == true).OrderBy(x => x.PositionIndex).ToList();
            return PartialView("_ModuleRightPartial", data);
        }

        [ChildActionOnly]
        public ActionResult _ModuleAlbumHot()
        {
            var data = db.Albums.Where(x => x.Published == true && x.IsAlbumTop == true).FirstOrDefault();
            return PartialView("_ModuleAlbumHot", data);
        }

        [ChildActionOnly]
        public ActionResult _ModuleContact()
        {
            var data = db.Contacts.Where(x => x.Published == true).FirstOrDefault();
            return PartialView("_ModuleContact", data);
        }

        [ChildActionOnly]
        public ActionResult _ModuleLienKetLink()
        {
            var data = db.LienKets.Where(x => x.IsOption == true).ToList();
            return PartialView("_ModuleLienKetLink", data);
        }

        [ChildActionOnly]
        public ActionResult _ModuleLienKetHinhAnh()
        {
            var data = db.LienKets.Where(x => x.IsOption == false).ToList();
            return PartialView("_ModuleLienKetHinhAnh", data);
        }

    }
}