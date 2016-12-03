using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTinTuc.Models
{
    public class DanhMuc
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public int? ParentId { get; set; }
        public int? PositionIndex { get; set; }
        public string CatMenu { get; set; }
        public IList<DanhMuc> DanhMucs { get; set; }
        public DanhMuc()
        {
            DanhMucs = new List<DanhMuc>();
        }
    }

    public class DanhMucSoDoTrang
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public int? ParentId { get; set; }
        public int? PositionIndex { get; set; }
        public string CatMenu { get; set; }
        public IList<DanhMucSoDoTrang> DanhMucSoDoTrangs { get; set; }
        public DanhMucSoDoTrang()
        {
            DanhMucSoDoTrangs = new List<DanhMucSoDoTrang>();
        }
    }

    public class DanhMucBaiViet
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public int? PositionIndex { get; set; }
        public string SlugCat { get; set; }
        public int? ParentCatId {get; set;}
        public bool? ViewHome { get; set; }
        public int? PositionOnHome { get; set; }
        public IList<DanhMucBaiViet> DanhMucBaiViets { get; set; }
        public DanhMucBaiViet()
        {
            DanhMucBaiViets = new List<DanhMucBaiViet>();
        }
    }

    public class DanhMucLinhVucHoiDap
    {
        public int LinhVucId {get; set; }
        public string TenLinhVuc { get; set; }
        public int? PositionIndex { get; set; }
        public string SlugLinhVuc { get; set; }
        public int? LinhVucChaId { get; set; }
        public IList<DanhMucLinhVucHoiDap> DanhMucLinhVucHoiDaps { get; set; }
        public DanhMucLinhVucHoiDap()
        {
            DanhMucLinhVucHoiDaps = new List<DanhMucLinhVucHoiDap>();
        }
    }

    public class DanhMucDonVi
    {
        public int DonViId { get; set; }
        public string TenDonVi { get; set; }
        public int? PositionIndex { get; set; }
        public string SlugDonVi { get; set; }
        public int? MaDonViCha { get; set; }
        public bool Published { get; set; }
        public IList<DanhMucDonVi> DanhMucDonVis { get; set; }
        public DanhMucDonVi()
        {
            DanhMucDonVis = new List<DanhMucDonVi>();
        }
    }

    public class DanhMucTaiLieu
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public int? PositionIndex { get; set; }
        public int? ParentCatId { get; set; }
        public IList<DanhMucTaiLieu> DanhMucTaiLieus { get; set; }
        public DanhMucTaiLieu()
        {
            DanhMucTaiLieus = new List<DanhMucTaiLieu>();
        }
    }

    public class DanhMucMenu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string Position { get; set; }
        public int? PositionIndex { get; set; }
        public string MenuUrl { get; set; }
        public int? ParentMenuId {get; set;}
        public IList<DanhMucMenu> DanhMucMenus { get; set; }
        public DanhMucMenu()
        {
            DanhMucMenus = new List<DanhMucMenu>();
        }
    }

    public class DanhMucJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? parentId { get; set; }
        public bool? ViewHome { get; set; }
        public string SlugCat { get; set; }
        public int? PositionIndex { get; set; }
        public int? PositionOnHome { get; set; }
    }

    public class DonViJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? parentId { get; set; }
    }

    public class DanhMucDocumentsJson {
        public int id {get; set;}
        public string name {get;set;}
        public int? parentId {get; set;}
    }

    public class MenuJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? parentId { get; set; }
    }

    public class MenuJson2
    {
        public int id { get; set; }
        public string name { get; set; }
        public IList<MenuJson2> childnodes { get; set; }
        public MenuJson2()
        {
            childnodes = new List<MenuJson2>();
        }
    }   

}