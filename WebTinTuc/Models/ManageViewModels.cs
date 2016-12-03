using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;

namespace WebTinTuc.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required(ErrorMessage="Vui lòng nhập {0}")]
        [StringLength(100, ErrorMessage = "{0} phải có độ dài ít nhất {2} ý tự trở lên.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không chính xác.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage="Vui lòng nhập mât khẩu hiện tại.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(100, ErrorMessage = "{0} phải có độ dài ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không chính xác.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Mã code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
        [Display(Name = "Phòng")]
        public string RoomName { get; set; }
    }

    public class ManageUserEditModel
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
        [Display(Name = "Phòng")]
        public string RoomUser { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn {0}")]
        [Display(Name = "Quyền quản trị")]
        public string RolesName { get; set; }
    }

    public class ManageUserDeleteModel
    {
        public string UserId { get; set; }
        public string FullName { get; set;}
    }

    public class AddCategoryModel
    {
        [Required(ErrorMessage="Vui lòng nhập tên danh mục")]
        [StringLength(256, ErrorMessage="{0} không được dài quá 256 ký tự")]
        [Display(Name="Tên danh mục")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập danh mục cha")]
        [Display(Name="Danh mục cha")]
        public int? ParentCategoryId { get; set; }
        [Display(Name="Vị trí danh mục")]
        public int? PositionIndex { get; set; }
        [Display(Name="Slug Url")]
        public string SlugCategory { get; set; }
        [Display(Name="Hiện danh mục này ngoài trang chủ")]
        public bool ViewHome { get; set; }
        [Display(Name="Vị trí")]
        public int? PositionHome { get; set; }
        [Display(Name="Khóa")]
        public bool Published { get; set; }
    }

    public class HoiDapLinhVucModel
    {
        public int LinhVucId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên lĩnh vực")]
        [Display(Name = "Tên lĩnh vực hỏi đáp")]
        public string TenLinhVuc { get; set; }
        [Required(ErrorMessage = "Vui lòng địa chỉ truy cập")]
        [Display(Name = "Địa chỉ truy cập")]
        public string DiaChiTruyCap { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lĩnh vực cha")]
        [Display(Name = "Lĩnh vực cha")]
        public int LinhVucChaId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vị trí lĩnh vực")]
        [Display(Name = "Vị trí lĩnh vực")]
        public int ViTriLinhVuc { get; set; }
        [Display(Name = "Mở/khóa")]
        public bool Published { get; set; }
    }

    public class DonViModel
    {
        public int DonViId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên đơn vị")]
        [Display(Name = "Tên đơn vị")]
        public string TenDonVi { get; set; }
        [Display(Name = "Chuỗi URL")]
        [Required(ErrorMessage = "Vui lòng nhập chuỗi URL")]
        public string SlugDonVi { get; set; }
        [Display(Name = "Đơn vị cha")]
        [Required(ErrorMessage = "Vui lòng nhập đơn vị cha")]
        public int? MaDonViCha { get; set; }
        [Display(Name = "Vị trí đơn vị")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Khóa")]
        public bool Published { get; set; }
        [Display(Name = "Di chuyển danh mục")]
        public bool TopToBottom { get; set; }
    }

    public class LinhVucModel
    {
        public int LinhVucId { get; set; }
        [Display(Name = "Tên lĩnh vực")]
        [Required(ErrorMessage = "Vui lòng nhập lĩnh vực")]
        public string TenLinhVuc { get; set; }
        [Display(Name = "Đơn vị")]
        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        public int? DonViId { get; set; }
        [Display(Name = "Vị trí đơn vị")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Khóa")]
        public bool Published { get; set; }
        [Display(Name = "Di chuyển danh mục")]
        public bool TopToBottom { get; set; }
    }

    public class LyDoModel
    {
        public int LyDoId { get; set; }
        [Display(Name = "Tên lý do")]
        [Required(ErrorMessage = "Vui lòng nhập tên lý do")]
        public string TenLyDo { get; set; }
    }

    public class CongChucModel
    {
        public long CongChucId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên công chúc")]
        [Display(Name = "Tên công chức")]
        public string TenCongChuc { get; set; }
        [Display(Name = "Hình ảnh")]
        public string HinhAnh { get; set; }
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }
        [Display(Name = "Trình độ công chức")]
        public string TrinhDo { get; set; }
        [Display(Name = "Chức vụ")]
        public string ChucVu { get; set; }
        [Display(Name = "Đơn vị")]
        public int? DonViId { get; set; }
        [Display(Name = "Mở/Khóa danh mục")]
        public bool Published { get; set; }
    }

    public class NgayThangModel
    {
        [DataType(DataType.Date)]
        public DateTime? todate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? fromdate { get; set; }
    }

    public class AddNewCatDocumentModel {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [Display(Name = "Tên danh mục")]
        public string DocumentName {get; set;}
        [Display(Name = "Vị trí danh mục")]
        public int? PositionIndex {get;set;}
        [Display(Name = "Danh mục cha")]
        [Required(ErrorMessage = "Vui lòng nhập danh mục cha")]
        public int? ParentDocumentId {get; set;}
        [Display(Name = "Mở/Khóa danh mục")]
        public bool Published { get; set; }
    }

    public class EditCatDocumentModel
    {
        public int DocumentId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [Display(Name = "Tên danh mục")]
        public string DocumentName { get; set; }
        [Display(Name = "Vị trí danh mục")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Danh mục cha")]
        [Required(ErrorMessage = "Vui lòng nhập danh mục cha")]
        public int? ParentDocumentId { get; set; }
        [Display(Name = "Mở/Khóa danh mục")]
        public bool Published { get; set; }
    }

    public class AddMenuModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên Menu")]
        [StringLength(256, ErrorMessage = "{0} không được dài quá 256 ký tự")]
        [Display(Name = "Tên Menu")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đường dẫn")]
        [Display(Name = "Url Menu")]
        public string MenuUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập danh mục cha")]
        [Display(Name = "Menu cha")]
        public int? ParentMenuId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vị trí menu")]
        [Display(Name="Vị trí menu")]
        public string Position { get; set; }

        [Display(Name = "Vị trí sắp xếp")]
        public int? PositionIndex { get; set; }

        [Display(Name = "Khóa")]
        public bool Published { get; set; }
    }

    public class EditMenuModel
    {
        public int MenuId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên Menu")]
        [StringLength(256, ErrorMessage = "{0} không được dài quá 256 ký tự")]
        [Display(Name = "Tên Menu")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đường dẫn")]
        [Display(Name = "Url Menu")]
        public string MenuUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập danh mục cha")]
        [Display(Name = "Menu cha")]
        public int? ParentMenuId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vị trí menu")]
        [Display(Name = "Vị trí menu")]
        public string Position { get; set; }

        [Display(Name = "Vị trí sắp xếp")]
        public int? PositionIndex { get; set; }

        [Display(Name = "Khóa")]
        public bool Published { get; set; }
    }

    public class EditCategoryModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(256, ErrorMessage = "{0} không được dài quá 256 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập danh mục cha")]
        [Display(Name = "Danh mục cha")]
        public int? ParentCategoryId { get; set; }
        [Display(Name = "Vị trí danh mục")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Slug Url")]
        public string SlugCategory { get; set; }
        [Display(Name = "Hiện danh mục này ngoài trang chủ")]
        public bool ViewHome { get; set; }
        [Display(Name = "Vị trí")]
        public int? PositionHome { get; set; }
        [Display(Name = "Khóa")]
        public bool Published { get; set; }
    }

    public class MovePositionCategory
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }
        [Display(Name = "Vị trí danh mục")]
        public int? PositionIndex { get; set; }
        [Display(Name="Di chuyển danh mục")]
        public bool TopToBottom { get; set; }
    }

    public class MovePositionCatDocument
    {
        public int DocumentId { get; set; }
        public int? ParentDocumentId { get; set; }
        [Display(Name = "Tên danh mục")]
        public string DocumentName { get; set; }
        [Display(Name = "Vị trí danh mục")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Di chuyển danh mục")]
        public bool TopToBottom { get; set; }
    }

    public class MovePositionMenu
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        [Display(Name = "Tên Menu")]
        public string MenuName { get; set; }
        [Display(Name = "Vị trí")]
        public string Position { get; set; }
        [Display(Name = "Vị trí Menu")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Di chuyển danh mục")]
        public bool TopToBottom { get; set; }
    }

    public class NewArticle
    {
        [Required(ErrorMessage="Vui lòng nhập tiêu đề bài viết.")]
        [Display(Name="Tiêu đề bài viết")]
        public string ArticleTitle { get; set; }

        [Display(Name="Mô tả bài viết")]
        public string ArticleDescription { get; set; }

        [Display(Name="Nội dung bài viết")]
        public string ArticleContent { get; set; }

        [Display(Name="Tin nổi bật")]
        public bool IsNewHot { get; set; }

        [Display(Name="Ảnh bài viết")]
        public string ArticleImageSmall { get; set; }

        [Display(Name="Ảnh slider")]
        public string ArticleImageBig { get; set; }

        [Display(Name="Chuỗi URL bài viết(SEO)")]
        public string SlugArticleTitle { get; set; }

        [Display(Name="Tag bài viết")]
        public string Tags { get; set; }

        [Display(Name="Loại bài viết")]
        public string TypePost { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập danh mục bài viết")]
        [Display(Name="Danh mục bài viết")]
        public int? CategoryId { get; set; }

        [Display(Name = "Published")]
        public bool Published { get; set; }

    }

    public class NewDocumentDetails
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài liệu")]
        [Display(Name = "Tên tài liệu")]
        public string DetailName { get; set; }
        [Display(Name = "Mô tả tài liệu")]
        public string DetailDescription { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập chuỗi url tài liệu")]
        [Display(Name = "Chuỗi url tài liệu")]
        public string DetailSlug { get; set; }
        [Display(Name = "Nội dung tài liệu")]
        public string DetailContent { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập danh mục tài liệu")]
        [Display(Name = "Danh mục tài liệu")]
        public int? DocumentId { get; set; }
        [Display(Name = "Mở/khóa tài liệu")]
        public bool Published { get; set; }        
    }

    public class EditDocumentDetail
    {
        public long DetailId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên tài liệu")]
        [Display(Name = "Tên tài liệu")]
        public string DetailName { get; set; }
        [Display(Name = "Mô tả tài liệu")]
        public string DetailDescription { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập chuỗi url tài liệu")]
        [Display(Name = "Chuỗi url tài liệu")]
        public string DetailSlug { get; set; }
        [Display(Name = "Nội dung tài liệu")]
        public string DetailContent { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập danh mục tài liệu")]
        [Display(Name = "Danh mục tài liệu")]
        public int? DocumentId { get; set; }
        [Display(Name = "Mở/khóa tài liệu")]
        public bool Published { get; set; }       
    }

    public class fileModel
    {
        public string xfileName { get; set; }
        public string xfilePath { get; set; }
    }

    public class EditArticle
    {
        public long ArticleId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài viết.")]
        [Display(Name = "Tiêu đề bài viết")]
        public string ArticleTitle { get; set; }

        [Display(Name = "Mô tả bài viết")]
        public string ArticleDescription { get; set; }

        [Display(Name = "Nội dung bài viết")]
        public string ArticleContent { get; set; }

        [Display(Name = "Tin nổi bật")]
        public bool IsNewHot { get; set; }

        [Display(Name = "Ảnh bài viết")]
        public string ArticleImageSmall { get; set; }

        [Display(Name = "Ảnh slider")]
        public string ArticleImageBig { get; set; }

        [Display(Name = "Chuỗi URL bài viết(SEO)")]
        public string SlugArticleTitle { get; set; }

        [Display(Name = "Tag bài viết")]
        public string Tags { get; set; }

        [Display(Name = "Loại bài viết")]
        public string TypePost { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập danh mục bài viết")]
        [Display(Name = "Danh mục bài viết")]
        public int? CategoryId { get; set; }
        public bool GiuNgayDang { get; set; }
        [Display(Name = "Published")]
        public bool Published { get; set; }

    }

    public class NewAlbum
    {
        [Display(Name="Tên thư viện ảnh")]
        [Required(ErrorMessage="Vui lòng nhập thư viện ảnh")]
        public string AlbumName { get; set; }
        [Display(Name="Hiện thị")]
        public bool IsAlbumTop { get; set; }
    }

    public class EditAlbum
    {
        public int AlbumId { get; set; }
        [Display(Name = "Tên thư viện ảnh")]
        [Required(ErrorMessage = "Vui lòng nhập thư viện ảnh")]
        public string AlbumName { get; set; }
        [Display(Name = "Hiện thị")]
        public bool IsAlbumTop { get; set; }
        [Display(Name = "Published")]
        public bool Published { get; set; }
    }

    public class NewModuleModel
    {
        [Display(Name = "Tên module")]
        [Required(ErrorMessage = "Vui lòng nhập tên module.")]
        public string ModuleName { get; set; }
        [Display(Name = "Đường dẫn module")]
        [DataType(DataType.Url)]
        public string ModuleLink { get; set; }
        [Display(Name = "Ẩn tên module")]
        public bool HiddenModuleName { get; set; }
        [Display(Name = "Kiểu module")]
        [Required(ErrorMessage = "Vui lòng nhập kiểu module.")]
        public string TypeModule { get; set; }
        [Display(Name = "Vị trí module")]
        [Required(ErrorMessage = "Vui lòng nhập vị trí module.")]
        public string Position { get; set; }
        [Display(Name = "Thứ tự module")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Mở/khóa module")]
        public bool Published { get; set; }
    }

    public class EditModuleModel
    {
        public int ModuleId { get; set; }
        [Display(Name = "Tên module")]
        [Required(ErrorMessage = "Vui lòng nhập tên module.")]
        public string ModuleName { get; set; }
        [Display(Name = "Đường dẫn module")]
        public string ModuleLink { get; set; }
        [Display(Name = "Ẩn tên module")]
        public bool HiddenModuleName { get; set; }
        [Display(Name = "Kiểu module")]
        [Required(ErrorMessage = "Vui lòng nhập kiểu module.")]
        public string TypeModule { get; set; }
        [Display(Name = "Vị trí module")]
        [Required(ErrorMessage = "Vui lòng nhập vị trí module.")]
        public string Position { get; set; }
        [Display(Name = "Thứ tự module")]
        public int? PositionIndex { get; set; }
        [Display(Name = "Mở/khóa module")]
        public bool Published { get; set; }
    }

    public class VoteModel
    {
        public string VoteName {get; set;}
        public int NumberVote { get; set; }
        public string PercentVote { get; set; }
        public string colorVote { get; set; }
    }

    public class AdvModel
    {
        [Display(Name = "Tên quảng cáo")]
        [Required(ErrorMessage = "Vui lòng nhập tên quảng cáo.")]
        public string AdvTitle { get; set; }
        [Display(Name = "Loại quảng cáo")]
        [Required(ErrorMessage = "Vui lòng nhập loại quảng cáo.")]
        public string TypeAdv { get; set; }
        public bool Published { get; set; }
    }

    public class EditAdvModel
    {
        public int AdvId { get; set; }
        [Display(Name = "Tên quảng cáo")]
        [Required(ErrorMessage = "Vui lòng nhập tên quảng cáo.")]
        public string AdvTitle { get; set; }
        [Display(Name = "Loại quảng cáo")]
        [Required(ErrorMessage = "Vui lòng nhập loại quảng cáo.")]
        public string TypeAdv { get; set; }
        public bool Published { get; set; }
    }

    public class AdvContentModel
    {
        [Display(Name = "Đường dẫn ảnh")]
        public string ImgUrl { get; set; }
        [Display(Name = "Địa chỉ truy cập")]
        public string Link { get; set; }
        [Display(Name = "Target")]
        public string Target { get; set; }
        [Display(Name = "Quảng cáo")]
        [Required(ErrorMessage = "Vui lòng chọn quảng cáo.")]
        public int AdvId { get; set; }
    }

    public class EditAdvContentModel
    {
        public long AdvContentId { get; set; }
        [Display(Name = "Đường dẫn ảnh")]
        public string ImgUrl { get; set; }
        [Display(Name = "Địa chỉ truy cập")]
        public string Link { get; set; }
        [Display(Name = "Target")]
        public string Target { get; set; }
        [Display(Name = "Quảng cáo")]
        [Required(ErrorMessage = "Vui lòng chọn quảng cáo.")]
        public int AdvId { get; set; }
    }

    public class MapModel
    {
        public int MapId { get; set; }
        [Display(Name = "Tên bản đồ")]
        [Required(ErrorMessage = "Vui lòng Nhập vào tên bản đồ.")]
        public string MapName { get; set; }
        [Display(Name = "Vị trí kinh độ")]
        [Required(ErrorMessage = "Vui lòng Nhập vào {0}.")]
        public double? Lat { get; set; }
        [Display(Name = "Vị trí vĩ độ")]
        [Required(ErrorMessage = "Vui lòng Nhập vào {0}.")]
        public double? Long { get; set; }
        [Display(Name = "Chuỗi API KEY")]
        public string ApiKey { get; set; }
        [Display(Name = "Mô tả bản đồ")]
        public string MapDescription { get; set; }
        [Display(Name = "Mở/khóa bản đồ")]
        public bool Published { get; set; }
    }

    public class ContactModel
    {
        public int ContactId { get; set; }
        [Display(Name = "Tên liên hệ")]
        [Required(ErrorMessage = "Vui lòng Nhập vào {0}.")]
        public string ContactTitle { get; set; }
        [Display(Name = "Hình ảnh")]
        public string ContactImage { get; set; }
        [Display(Name = "Nội dung")]
        public string ContactContent { get; set; }
        [Display(Name = "Mở/khóa")]
        public bool Published { get; set; }
    }

    public class VideoModel
    {
        public int VideoId { get; set; }
        [Display(Name = "Tên video")]
        [Required(ErrorMessage = "Vui lòng Nhập vào {0}.")]
        public string VideoTitle { get; set; }
        [Display(Name = "Địa chỉ video")]
        public string VideoUrl { get; set; }
        [Display(Name = "Hình ảnh đại diện")]
        public string VideoImage { get; set; }
        [Display(Name = "Là video nổi bật")]
        public bool IsVideoTop { get; set; }
        [Display(Name = "Trạng thái video")]
        public bool Published { get; set; }
    }

    public class LienketModel {
        public int LienKetId { get; set; }
        [Display(Name = "Tên liên kết")]
        [Required(ErrorMessage = "Vui lòng Nhập vào {0}.")]
        public string LienKetName { get; set; }
        [Display(Name = "Địa chỉ liên kết")]
        public string LienKetUrl { get; set; }
        [Display(Name = "Kiểu liên kết")]
        public bool IsOption { get; set; }
        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }
        [Display(Name = "Target")]
        public string Target { get; set; }
    }

    public class SupportModel {
        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]
        public string FullName { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]
        public string Address { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập {0}.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Nội dung thông điệp")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]        
        public string SupportContent { get; set; }
        [Display(Name = "Tiêu đề thông điệp")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]
        public string SupportType { get; set; }
    }

    public class EmbebModel
    {
        public int EmbedId { get; set; }
        [Display(Name = "Tên thông báo")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]        
        public string EmbedName { get; set; }        
        [Display(Name = "Tệp hình ảnh")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]        
        public string EmbedFile { get; set; }
        [Display(Name = "Tùy chỉnh nâng cao")]
        public bool EmbebOption { get; set; }
        [Display(Name = "Chèn nội dung tùy chỉnh")]
        public string EmbebContent { get; set; }
    }

    public class DanhGiaDonViModel
    {
        public long DanhGiaDonViId { get; set; }
        [Display(Name = "Đơn vị")]
        public int? DonViId { get; set; }
        [Display(Name = "Lĩnh vực")]
        public int? LinhVucId { get; set; }
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]     
        public string NguoiDanhGia { get; set; }
        [Display(Name = "Số hồ sơ")]
        public string SoHoSo { get; set; }
        [Display(Name = "Kiểu đánh giá")]
        public bool? KieuDanhGia { get; set; }
        //public ICollection<Lydo> Lydoes { get; set; }
    }

    public class DanhGiaCongChucModel
    {
        public long DanhGiaCongChucId { get; set; }
        [Display(Name = "Công chức")]
        public int? CongChucId { get; set; }
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập vào {0}.")]
        public string NguoiDanhGia { get; set; }
        [Display(Name = "Số hồ sơ")]
        public string SoHoSo { get; set; }
        [Display(Name = "Kiểu đánh giá")]
        public bool? KieuDanhGia { get; set; }
        [Display(Name = "Đơn vị")]
        public int? DonViId { get; set; }
    }

    public class DeviceGroupViewModel
    {
        public long Type { get; set; }
        public int Count { get; set; }
    }

    public class PhanTramDanhGiaDonViModel {
        public string TenDonVi {get; set;}
        public string SlugUrl { get; set; }
        public int DonViId { get; set; }
        public int LuotHaiLong { get; set; }
        public int LuotChuaHaiLong { get; set; }
        public int SoLuotDanhGia { get; set; }
    }

    public class HoiDapModel {
        public long CauHoiId { get; set; }        
        [Display(Name = "Họ tên")]
        [Required(ErrorMessage="Vui lòng nhập họ tên người gửi")]
        public string HoTenNguoiHoi { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string DiaChiNguoiHoi { get; set; }
        [Display(Name = "Số điện thoại")]
        public string SoDienThoaiNguoiHoi { get; set; }
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email")]
        public string EmailNguoiGui { get; set; }
        [Display(Name = "Đơn vị nhận câu hỏi")]
        [Required(ErrorMessage = "Vui lòng chọn đơn vị tiếp nhận")]
        public string DonViTiepNhanId { get; set; }
        [Display(Name = "Lĩnh vực")]
        [Required(ErrorMessage = "Vui lòng chọn lĩnh vực hỏi đáp")]
        public int LinhVucHoiDapId { get; set; }
        [Display(Name = "Tiêu đề câu hỏi")]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề câu hỏi")]
        public string TenCauHoi { get; set; }
        [Display(Name = "Nội dung câu hỏi")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung câu hỏi")]
        public string NoiDungCauHoi { get; set; }
        
    }

    public class TraLoiModel
    {
        public long CauHoiId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề trả lời")]
        [Display(Name = "Tiêu đề trả lời")]
        public string TDTraLoi { get; set; }
        [Display(Name = "Nội dung trả lời")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung trả lời")]
        public string NDTraLoi { get; set; }
    }

}