using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebTinTuc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.RouteExistingFiles = true;

            //admin/NotFound
            routes.MapRoute(
                "AdminNotFound",
                "admin/NotFound",
                new { controller = "Admin", action = "NotFound" }
            );
            //admin/adduser
            routes.MapRoute(
                "AdminAddUser",
                "admin/adduser",
                new { controller = "Account", action = "Register" }
            );

            // admin
            routes.MapRoute(
                "AdminPanel",
                "admin",
                new { controller = "Admin", action = "Index" }
            );

            // admin/login
            routes.MapRoute(
                "AdminLogin",
                "admin/login",
                new { controller = "Account", action = "Login" }
            );

            //admin/addnewarticle
            routes.MapRoute(
                "AddNewArticle",
                "admin/addnewarticle",
                new { controller = "Articles", action = "Index" }
            );

            //admin/managerArticle
            routes.MapRoute(
                "ManagerArticle",
                "admin/managerArticle",
                new { controller = "Articles", action = "ManagerArticle" }
            );

            //routes.MapRoute(
            //    "adminRoxyfileman",
            //    "admin/RoxyFileman",
            //    new { controller = "RoxyFileman" }
            //    );

            // /notfound
            routes.MapRoute(
                "NotFound",
                "notfound",
                new { controller = "Home", action = "NotFound" }
            );

            routes.Add("DetailArticle", new SeoFriendlyRoute("bai-viet/{slugurl}-{id}",
               new RouteValueDictionary(
                   new
                   {
                       controller = "Articles",
                       action = "Detail",
                       id = UrlParameter.Optional,
                       slugurl = UrlParameter.Optional
                   }),
               new MvcRouteHandler()));

            //GetDetailDocument
            routes.Add("DetailDocument", new SeoFriendlyRoute("tai-lieu/{slugurl}-{id}",
               new RouteValueDictionary(
                   new
                   {
                       controller = "Documents",
                       action = "GetDetailDocument",
                       id = UrlParameter.Optional,
                       slugurl = UrlParameter.Optional
                   }),
               new MvcRouteHandler()));

            routes.Add("CategoryArticles", new SeoFriendlyRoute("tin-tuc/{slugurl}-{id}",
               new RouteValueDictionary(
                   new
                   {
                       controller = "Articles",
                       action = "CategoryArticles",
                       id = UrlParameter.Optional,
                       slugurl = UrlParameter.Optional
                   }),
               new MvcRouteHandler()));



            //admin/addnewalbum
            routes.MapRoute(
                "AdminAddNewAlBum",
                "admin/addnewalbum",
                new { controller = "Albums", action = "AddNewAlBum" }
            );

            //admin/albums
            routes.MapRoute(
                "AdminManagerAlbum",
                "admin/albums",
                new { controller = "Albums", action = "Index" }
            );

            //admin/documents
            routes.MapRoute(
                "AdminManagerDocuments",
                "admin/documents",
                new { controller = "Documents", action = "Index" }
            );

            //danh-sach-tai-lieu
            routes.MapRoute(
                "DanhSachTaiLieu",
                "danh-sach-tai-lieu",
                new { controller = "Documents", action = "GetAllDocuments" }
            );

            //admin/addnewdocument
            routes.MapRoute(
               "AdminAddNewDocument",
               "admin/addnewdocument",
               new { controller = "Documents", action = "AddNewDocument" }
           );

            //admin/AddNewCatDocument
            routes.MapRoute(
               "AdminAddNewCatDocument",
               "admin/AddNewCatDocument",
               new { controller = "Documents", action = "AddNewCatDocument" }
           );

            //admin/modules
            routes.MapRoute(
               "AdminModules",
               "admin/modules",
               new { controller = "Modules", action = "Index" }
           );

            //admin/addnewmodule 
            routes.MapRoute(
               "AdminAddNewModules",
               "admin/addnewmodule",
               new { controller = "Modules", action = "AddNewModule" }
           );

            routes.MapRoute(
                "HomeGetAllAlbums",
                "albums",
                new { controller = "Albums", action = "GetAllAlbums" }
            );

            //LienHePage
            // /lien-he
            routes.MapRoute(
                "PageLienHe",
                "lien-he",
                new { controller = "Home", action = "LienHePage" }
            );

            //SoDoTrangWeb
            // so-do-cong
            routes.MapRoute(
                "PageSoDoTrangWeb",
                "so-do-cong",
                new { controller = "Home", action = "SoDoTrangWeb" }
            );

            ///admin/embeb
            routes.MapRoute(
                "adminEmbed",
                "admin/embeb",
                new { controller = "Embeds", action = "Index" }
            );

            //admin/danhsachdonvi
            routes.MapRoute(
               "AdminDanhSachDonVi",
               "admin/danhsachdonvi",
               new { controller = "Rates", action = "Index" }
           );

            // admin/donvi/1/edit
            routes.MapRoute(
                "AdminEditDonVi",
                "admin/donvi/{id}/edit",
                new { controller = "Rates", action = "EditDonVi", id = UrlParameter.Optional }
            );

            //admin/ThemMoiDonVi
            routes.MapRoute(
                "AdminThemMoiDonVi",
                "admin/themmoidonvi",
                new { controller = "Rates", action = "ThemMoiDonVi" }
            );

            //AdminDeleteDonVi
            routes.MapRoute(
                "AdminDeleteDonVi",
                "admin/donvi/{id}/delete",
                new { controller = "Rates", action = "DeleteDonVi", id = UrlParameter.Optional }
            );

            //AdminMoveDonVi
            routes.MapRoute(
                "AdminMoveDonVi",
                "admin/donvi/{id}/move",
                new { controller = "Rates", action = "MoveDonVi", id = UrlParameter.Optional }
            );

            //admin/themoilinhvuc
            routes.MapRoute(
                "AdminThemMoiLinhVuc",
                "admin/themmoilinhvuc",
                new { controller = "Rates", action = "ThemMoiLinhVuc" }
            );

            //admin/danhsachlinhvuc
            routes.MapRoute(
                "AdminDanhSachLinhVuc",
                "admin/danhsachlinhvuc",
                new { controller = "Rates", action = "DanhSachLinhVuc" }
            );

            // admin/linhvuc/1/edit
            routes.MapRoute(
                "AdminEditLinhVuc",
                "admin/linhvuc/{id}/edit",
                new { controller = "Rates", action = "EditLinhVuc", id = UrlParameter.Optional }
            );

            //admin/linhvuc/1/delete
            routes.MapRoute(
                "AdminDeleteLinhVuc",
                "admin/linhvuc/{id}/delete",
                new { controller = "Rates", action = "DeleteLinhVuc", id = UrlParameter.Optional }
            );

            //admin/linhvuc/1/move
            routes.MapRoute(
                "AdminMoveLinhVuc",
                "admin/linhvuc/{id}/move",
                new { controller = "Rates", action = "MoveLinhVuc", id = UrlParameter.Optional }
            );

            //admin/themoicongchuc
            routes.MapRoute(
               "AdminThemMoiCongChuc",
               "admin/themmoicongchuc",
               new { controller = "Rates", action = "ThemMoiCongChuc" }
           );

            //admin/danhsachcongchuc
            routes.MapRoute(
               "AdminDanhSachCongChuc",
               "admin/danhsachcongchuc",
               new { controller = "Rates", action = "DanhSachCongChuc" }
           );

            //AdminEditCongChuc
            routes.MapRoute(
               "AdminEditCongChuc",
               "admin/congchuc/{id}/edit",
               new { controller = "Rates", action = "EditCongChuc", id = UrlParameter.Optional }
           );

            //AdminDeleteCongChuc
            routes.MapRoute(
               "AdminDeleteCongChuc",
               "admin/congchuc/{id}/delete",
               new { controller = "Rates", action = "DeleteCongChuc", id = UrlParameter.Optional }
           );

            //admin/danhsachlydo
            routes.MapRoute(
               "AdminDanhSachLyDo",
               "admin/danhsachlydo",
               new { controller = "Rates", action = "DanhSachLyDo" }
           );

            //admin/themlydo
            routes.MapRoute(
               "AdminThemLyDo",
               "admin/themlydo",
               new { controller = "Rates", action = "ThemLyDo" }
           );

            //admin/lydo/1/edit
            routes.MapRoute(
               "AdminEditLyDo",
               "admin/lydo/{id}/edit",
               new { controller = "Rates", action = "EditLyDo", id = UrlParameter.Optional }
           );

            //admin/lydo/1/delete
            routes.MapRoute(
               "AdminDeleteLyDo",
               "admin/lydo/{id}/delete",
               new { controller = "Rates", action = "DeleteLyDo", id = UrlParameter.Optional }
           );

            //home/khao-sat-cong-chuc
            routes.MapRoute(
               "HomeKhaoSatCongChuc",
               "home/khao-sat-cong-chuc",
               new { controller = "Home", action = "KhaoSatCongChuc" }
           );

            //home/danh-gia-don-vi
            routes.MapRoute(
               "HomeDanhGiaDonVi",
               "home/danh-gia-don-vi",
               new { controller = "Home", action = "DanhGiaDonVi" }
           );

            ///khaosat/xemketquachung/chitietdonvi
            routes.MapRoute(
               "XemKetQuaChungDanhSachDonVi",
               "khaosat/xemketquachung/chitietdonvi",
               new { controller = "Home", action = "XemKetQuaDanhSachDonVi" }
           );

            //khaosat/xemketquachung/donvi
            routes.MapRoute(
               "RXemKetQuaChungDanhGiaDonVi",
               "khaosat/xemketquachung/donvi",
               new { controller = "Home", action = "XemKetQuaChungDanhGiaDonVi" }
           );

            ///khaosat/danhsachcongchuc/donvi/abc-1/
            routes.Add("RDanhSachCongChucOfDonVi", new SeoFriendlyRoute("khaosat/danhsachcongchuc/donvi/{slugurl}-{id}",
               new RouteValueDictionary(
                   new
                   {
                       controller = "Home",
                       action = "DanhSachCongChucOfDonVi",
                       id = UrlParameter.Optional,
                       slugurl = UrlParameter.Optional
                   }),
               new MvcRouteHandler()));

            //RateDanhGiaDonVi
            routes.Add("RRateDanhGiaDonVi", new SeoFriendlyRoute("khaosat/donvi/{slugurl}-{id}",
              new RouteValueDictionary(
                  new
                  {
                      controller = "Home",
                      action = "RateDanhGiaDonVi",
                      id = UrlParameter.Optional,
                      slugurl = UrlParameter.Optional
                  }),
              new MvcRouteHandler()));

            //khaosat/congchuc/ten-cong-chuc-id
            routes.Add("RRateDanhGiaCongChuc", new SeoFriendlyRoute("khaosat/congchuc/{slugurl}-{id}",
             new RouteValueDictionary(
                 new
                 {
                     controller = "Home",
                     action = "RateDanhGiaCongChuc",
                     id = UrlParameter.Optional,
                     slugurl = UrlParameter.Optional
                 }),
             new MvcRouteHandler()));

            //khaosat/xemketqua/congchuc/kbnn-huyen-hoa-vang-4
            //XemKetQuaDanhGiaCongChuc
            routes.Add("KetQuaDanhGiaCongChuc", new SeoFriendlyRoute("khaosat/xemketqua/congchuc/{slugurl}-{id}",
             new RouteValueDictionary(
                 new
                 {
                     controller = "Home",
                     action = "XemKetQuaDanhGiaCongChuc",
                     id = UrlParameter.Optional,
                     slugurl = UrlParameter.Optional
                 }),
             new MvcRouteHandler()));

            //khaosat/xemketqua/donvi/{0}-{1}
            //XemKetQuaDanhGiaDonVi
            routes.Add("KetQuaDanhGiaDonVi", new SeoFriendlyRoute("khaosat/xemketqua/donvi/{slugurl}-{id}",
             new RouteValueDictionary(
                 new
                 {
                     controller = "Home",
                     action = "XemKetQuaDanhGiaDonVi",
                     id = UrlParameter.Optional,
                     slugurl = UrlParameter.Optional
                 }),
             new MvcRouteHandler()));

            //khaosat/xemketquachung/danhsachcongchuc/donvi/{0}-{1}
            routes.Add("RKetQuaChungDanhSachCongChucOfDonVi", new SeoFriendlyRoute("khaosat/xemketquachung/danhsachcongchuc/donvi/{slugurl}-{id}",
               new RouteValueDictionary(
                   new
                   {
                       controller = "Home",
                       action = "KetQuaChungDanhSachCongChucOfDonVi",
                       id = UrlParameter.Optional,
                       slugurl = UrlParameter.Optional
                   }),
               new MvcRouteHandler()));

            #region ### Module Hỏi đáp
            //admin/hoidap/linhvucs
            routes.MapRoute(
              "DanhSachLinhVucs",
              "admin/hoidap/linhvucs",
              new { controller = "HoiDaps", action = "Index" }
          );

            //admin/hoidap/themmoilinhvuc
            routes.MapRoute(
              "ThemMoiLinhVucHoiDap",
              "admin/hoidap/themmoilinhvuc",
              new { controller = "HoiDaps", action = "AddNewLinhVucHoiDap" }
            );

            //admin/hoidap/linhvuc/1/edit
            routes.MapRoute(
              "SuaLinhVucHoiDap",
              "admin/hoidap/linhvuc/{id}/edit",
              new { controller = "HoiDaps", action = "EditLinhVucHoiDap", id = UrlParameter.Optional }
            );

            //admin/hoidap/linhvuc/1/delete
            routes.MapRoute(
              "XoaLinhVucHoiDap",
              "admin/hoidap/linhvuc/{id}/delete",
              new { controller = "HoiDaps", action = "DeleteLinhVucHoiDap", id = UrlParameter.Optional }
            );

            //admin/hoidap/danhsachcauhoi
            routes.MapRoute(
              "DanhSachCauHoiHoiDap",
              "admin/hoidap/danhsachcauhoi",
              new { controller = "HoiDaps", action = "DanhSachCauHoiHoiDap" }
            );

            #endregion

            //khaosat/xemketquachung/congchuc
           routes.MapRoute(
              "KetQuaChungCongChuc",
              "khaosat/xemketquachung/congchuc",
              new { controller = "Home", action = "XemKetQuaChungCongChuc" }
          );

           //hoi-dap
           routes.MapRoute(
             "ModuleHoiDap",
             "hoi-dap",
             new { controller = "Home", action = "AddNewQuestion" }
         );

           //HoiDapXoaCauHoi
           routes.MapRoute(
           "HoiDapXoaCauHoi",
           "admin/hoi-dap/{id}/xoacauhoi",
           new { controller = "HoiDaps", action = "XoaCauHoi", id = UrlParameter.Optional }
          );

           //HoiDapTraLoiCauHoi
           routes.MapRoute(
          "HoiDapTraLoiCauHoi",
          "admin/hoi-dap/tra-loi-cau-hoi/{id}",
          new { controller = "HoiDaps", action = "TraLoiCauHoi", id = UrlParameter.Optional }
         );

           ///tra-cuu-cau-hoi
           routes.MapRoute(
           "HoiDapTraCuuCauHoi",
           "tra-cuu-cau-hoi",
           new { controller = "Home", action = "TraCuuCauHoi" }
          );
           
            //hoi-dap/cau-hoi/5
           routes.MapRoute(
          "HoiDapChiTietCauHoi",
          "hoi-dap/cau-hoi/{id}",
          new { controller = "Home", action = "ChiTietCauHoi" }
         );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }

    public class SeoFriendlyRoute : Route
    {
        public SeoFriendlyRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = base.GetRouteData(httpContext);

            if (routeData != null)
            {
                if (routeData.Values.ContainsKey("id"))
                    routeData.Values["id"] = GetIdValue(routeData.Values["id"]);
            }

            return routeData;
        }

        private object GetIdValue(object id)
        {
            if (id != null)
            {
                string idValue = id.ToString();

                var regex = new Regex(@"^(?<id>\d+).*$");
                var match = regex.Match(idValue);

                if (match.Success)
                {
                    return match.Groups["id"].Value;
                }
            }

            return id;
        }
    }

}
