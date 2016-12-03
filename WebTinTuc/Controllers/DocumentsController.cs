using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;
using PagedList;
using PagedList.Mvc;

namespace WebTinTuc.Controllers
{
    public class DocumentsController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: Documents
        public ActionResult Index(int? pg, string search)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = from d in db.DocumentDetails select d;
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.DetailName.ToLower().Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderByDescending(a => a.CreatedDate);

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> EditDocument(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            DocumentDetail _documentDetail = await db.DocumentDetails.FindAsync(id);
            if (_documentDetail == null)
            {
                return View(_documentDetail);
            }
            var getDocumentDetail = new EditDocumentDetail()
            {
                DetailId = _documentDetail.DetailId,
                DetailName = _documentDetail.DetailName,
                DetailSlug = _documentDetail.DetailSlug,
                DetailDescription = _documentDetail.DetailDescription,
                DetailContent = _documentDetail.DetailContent,
                DocumentId = _documentDetail.DocumentId,
                Published = (bool)_documentDetail.Published
            };
            return View(getDocumentDetail);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDocument(EditDocumentDetail model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditDocument");
            }
            var _documentDetail = await db.DocumentDetails.FindAsync(model.DetailId);
            try
            {
                if (_documentDetail != null)
                {
                    _documentDetail.DetailName = model.DetailName ?? null;
                    _documentDetail.DetailSlug = model.DetailSlug != null ? model.DetailSlug : Configs.unicodeToNoMark(model.DetailName);
                    _documentDetail.DetailDescription = model.DetailDescription ?? null;
                    _documentDetail.DetailContent = model.DetailContent ?? null;
                    _documentDetail.ModifiedDate = DateTime.Now;
                    _documentDetail.Published = model.Published;
                    _documentDetail.PublishedDate = model.Published == true ? DateTime.Now : (DateTime?)null;
                    _documentDetail.DocumentId = model.DocumentId;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa tài liệu.");
                return View(model);
            }

            TempData["Updated"] = "Cập nhật tài liệu thành công";
            return RedirectToAction("EditDocument");
        }

        // xóa file đính kèm.
        //[HttpPost]
        //public ActionResult DeleteAttackFile(long? id)
        //{
        //    bool delete = false;
        //    var _doc = db.DocumentDetails.Find(id);
        //    if (_doc != null)
        //    {
        //        var _file = _doc.AttackModels;
        //        foreach (var item in _file)
        //        {
        //            string path = Server.MapPath(item.AttackFilePath);
        //            FileInfo file = new FileInfo(path);
        //            if (file.Exists)
        //            {
        //                file.Delete();
        //            }
        //            db.AttackModels.Remove(item);
        //            db.SaveChanges();
        //            delete = true;
        //            if (_file.Count == 0)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    return Json(delete);
        //}


        public ActionResult CatDocuments()
        {
            return View();
        }

        public IEnumerable<Document> LoadDocuments()
        {
            var query = "select * from Documents";
            var data = db.Database.SqlQuery<Document>(query);
            //var data = from x in db.Documents select x;
            return data;
        }

        public JsonResult GetDanhMucTaiLieu()
        {
            List<DanhMucDocumentsJson> data = LoadDocuments().Select(x => new DanhMucDocumentsJson()
            {
                id = x.DocumentId,
                name = x.DocumentName,
                parentId = x.ParentDocumentId
            }).ToList();

            var obj = new { root = data };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewCatDocument()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewCatDocument(AddNewCatDocumentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute("AdminAddNewCatDocument");
            }
            try
            {
                Document _newDocument = new Document();
                _newDocument.DocumentName = model.DocumentName ?? null;
                _newDocument.PositionIndex = model.PositionIndex ?? (int?)null;
                _newDocument.ParentDocumentId = model.ParentDocumentId ?? (int?)null;
                _newDocument.Published = model.Published;
                db.Documents.Add(_newDocument);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Thêm danh mục tài liệu thành công";
                return RedirectToRoute("AdminAddNewCatDocument");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm danh mục tài liệu.");
                return View(model);
            }
        }

        //EditCatDocument

        public async Task<ActionResult> EditCatDocument(int? id)
        {
            if (id == null || id == 0)
            {
                //return RedirectToAction("NotFound", "Admin");
                return View();
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return View(document);
            }
            var getDocument = new EditCatDocumentModel()
            {
                DocumentId = document.DocumentId,
                DocumentName = document.DocumentName,
                ParentDocumentId = document.ParentDocumentId,
                PositionIndex = document.PositionIndex,
                Published = (bool)document.Published
            };
            return View(getDocument);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCatDocument(EditCatDocumentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditCatDocument");
            }
            var _document = await db.Documents.FindAsync(model.DocumentId);
            if (_document != null)
            {
                _document.DocumentName = model.DocumentName ?? null;
                _document.ParentDocumentId = model.ParentDocumentId ?? (int?)null;
                _document.PositionIndex = model.PositionIndex ?? (int?)null;
                _document.Published = (bool)model.Published;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật danh mục tài liệu thành công";
                return RedirectToAction("EditCatDocument");
            }
            return View(_document);
        }

        //DeleteCatDocument
        public ActionResult DeleteCatDocument(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Document _document = db.Documents.Find(id);
            if (_document == null)
            {
                return View();
            }
            return View(_document);
        }

        [HttpPost, ActionName("DeleteCatDocument")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCatDocumentConfirmed(int? id)
        {
            Document _document = await db.Documents.FindAsync(id);
            if (_document == null)
            {
                return View();
            }
            if (_document.Documents1.Count() > 0)
            {
                TempData["Error"] = "Bạn không thể xóa danh mục này. <br /> Danh mục này chứa danh mục con khác. Vui lòng xóa danh mục con trước.";
                return RedirectToAction("DeleteCatDocument");
            }

            if (_document.DocumentDetails.Count() > 0)
            {
                TempData["Error"] = "Bạn không thể xóa danh mục này. <br /> Danh mục này đang chứa tài liệu.";
                return RedirectToAction("DeleteCatDocument");
            }

            db.Documents.Remove(_document);
            await db.SaveChangesAsync();
            TempData["Deleted"] = "Xóa danh mục tài liệu thành công";
            return RedirectToAction("CatDocuments");
        }

        //MoveCatDocument
        public ActionResult MoveCatDocument(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Document _document = db.Documents.Find(id);
            if (_document == null)
            {
                return View();
            }
            var getDanhMuc = new MovePositionCatDocument()
            {
                DocumentId = _document.DocumentId,
                DocumentName = _document.DocumentName,
                ParentDocumentId = _document.ParentDocumentId,
                PositionIndex = _document.PositionIndex
            };

            return View(getDanhMuc);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveCatDocument(MovePositionCatDocument model)
        {
            Document _document = await db.Documents.FindAsync(model.DocumentId);
            if (_document == null)
            {
                return View();
            }
            //8 danh mục, position 8 = thì là vị trí cuối cùng
            //8 danh mục, position 1 là vị trí đầu tiên
            // danh muc la danh muc cuoi cung va di chuyen tu tren xuong
            if (_document.Document1.Documents1.Count() == model.PositionIndex && model.TopToBottom == false)
            {
                TempData["Error"] = "Danh mục này ở vị trí cuối cùng không thể di chuyển xuống được";
                return RedirectToAction("MoveCatDocument");
            }

            // danh muc la danh muc tren cung va di chuuyen tu duoi len
            if (_document.PositionIndex == 1 && model.TopToBottom == true)
            {
                TempData["Error"] = "Danh mục này ở vị trí trên cùng không thể di chuyển lên tiếp được";
                return RedirectToAction("MoveCatDocument");
            }

            if (model.TopToBottom == false)
            {
                var _query1 = string.Format("Update Documents Set PositionIndex=PositionIndex-1 Where ParentDocumentId = {0} and PositionIndex=(Select PositionIndex+1 From Documents where DocumentId={1})", _document.ParentDocumentId, _document.DocumentId);
                var _query2 = string.Format("Update Documents Set PositionIndex=PositionIndex+1 Where ParentDocumentId = {0} and DocumentId={1}", _document.ParentDocumentId, _document.DocumentId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }
            else
            {
                var _query1 = string.Format("Update Documents Set PositionIndex=PositionIndex+1 Where ParentDocumentId = {0} and PositionIndex=(Select PositionIndex-1 From Documents where DocumentId={1})", _document.ParentDocumentId, _document.DocumentId);
                var _query2 = string.Format("Update Documents Set PositionIndex=PositionIndex-1 Where ParentDocumentId = {0} and DocumentId={1}", _document.ParentDocumentId, _document.DocumentId);
                db.Database.ExecuteSqlCommand(_query1);
                db.Database.ExecuteSqlCommand(_query2);
            }

            TempData["Moved"] = "Di chuyển vị trí danh mục thành công.";
            return RedirectToAction("CatDocuments");
        }

        //"AddNewDocument" 
        public ActionResult AddNewDocument()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewDocument(NewDocumentDetails model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewDocument");

            }

            try
            {
                DocumentDetail _newDetailDocument = new DocumentDetail();
                _newDetailDocument.DetailName = model.DetailName ?? null;
                _newDetailDocument.DetailSlug = model.DetailSlug != null ? model.DetailSlug : Configs.unicodeToNoMark(model.DetailName);
                _newDetailDocument.DetailDescription = model.DetailDescription ?? null;
                _newDetailDocument.DetailContent = model.DetailContent ?? null;
                _newDetailDocument.ModifiedDate = null;
                _newDetailDocument.Published = model.Published;
                _newDetailDocument.PublishedDate = model.Published == true ? DateTime.Now : (DateTime?)null;
                _newDetailDocument.CreatedDate = DateTime.Now;
                _newDetailDocument.DocumentId = model.DocumentId;
                db.DocumentDetails.Add(_newDetailDocument);
                await db.SaveChangesAsync();

                TempData["Updated"] = "Thêm mới tài liệu thành công";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm mới tài liệu");
                return View(model);
            }

            return RedirectToAction("AddNewDocument");

        }



        //[HttpPost]
        //public ActionResult Upload()
        //{
        //    bool isSavedSuccessfully = true;
        //    string fName = "";
        //    try
        //    {
        //        foreach (string fileName in Request.Files)
        //        {
        //            HttpPostedFileBase file = Request.Files[fileName];
        //            //Save file content goes here

        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var originalDirectory = new DirectoryInfo(string.Format("{0}SaveFile\\AttackFiles", Server.MapPath(@"\")));
        //                string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
        //                string pathString = System.IO.Path.Combine(originalDirectory.ToString(), strDay);

        //                var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

        //                bool isExists = System.IO.Directory.Exists(pathString);

        //                if (!isExists)
        //                    System.IO.Directory.CreateDirectory(pathString);

        //                var path = string.Format("{0}\\{1}", pathString, _fileName);
        //                file.SaveAs(path);
        //                fName = "/SaveFile/AttackFiles/" + strDay + "/" + _fileName;
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        isSavedSuccessfully = false;
        //    }
        //    if (isSavedSuccessfully)
        //    {
        //        return Json(new { Message = fName });
        //    }
        //    else
        //    {
        //        return Json(new { Message = "Có lỗi khi lưu tệp tin" });
        //    }
        //}

        public PartialViewResult _CatDocPartial()
        {
            List<DanhMucTaiLieu> data = LoadDocuments().Select(x => new DanhMucTaiLieu()
            {
                CatId = x.DocumentId,
                CatName = x.DocumentName,
                ParentCatId = x.ParentDocumentId,
                PositionIndex = x.PositionIndex
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.ParentCatId == null).FirstOrDefault();
            SetChildrenCatDocument(presidents, data);

            return PartialView("_CatDocPartial", presidents);
        }

        private void SetChildrenCatDocument(DanhMucTaiLieu model, List<DanhMucTaiLieu> danhmuc)
        {
            var childs = danhmuc.Where(x => x.ParentCatId == model.CatId).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildrenCatDocument(child, danhmuc);
                    model.DanhMucTaiLieus.Add(child);
                }
            }
        }

        //GenerateSlugUrl

        //_DanhMucTaiLieuPartial
        [AllowAnonymous]
        public PartialViewResult _DanhMucTaiLieuPartial()
        {
            return PartialView("_DanhMucTaiLieuPartial", LoadDanhMucTaiLieu());
        }

        public PartialViewResult _SelectDanhMucTaiLieuPartial()
        {
            return PartialView("_SelectDanhMucTaiLieuPartial", LoadDanhMucTaiLieu());
        }

        public DanhMucTaiLieu LoadDanhMucTaiLieu()
        {
            List<DanhMucTaiLieu> data = LoadDocuments().Select(x => new DanhMucTaiLieu()
            {
                CatId = x.DocumentId,
                CatName = x.DocumentName,
                ParentCatId = x.ParentDocumentId,
                PositionIndex = x.PositionIndex
            }).OrderBy(o => o.PositionIndex).ToList();

            var presidents = data.Where(x => x.ParentCatId == null).FirstOrDefault();
            SetChildrenCatDocument(presidents, data);
            return presidents;
        }

        [HttpPost]
        public JsonResult SetPositionCatDocument(int? id)
        {
            try
            {
                int _iPosition = 0;
                var _cat = db.Documents.Where(c => c.DocumentId == id).FirstOrDefault();
                if (_cat != null)
                {
                    int iCats = _cat.Documents1.Count();
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

        public ActionResult DeleteDocument(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            DocumentDetail _documentDetail = db.DocumentDetails.Find(id);
            if (_documentDetail == null)
            {
                return View();
            }
            return View(_documentDetail);
        }

        [HttpPost, ActionName("DeleteDocument")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteDocumentConfirmed(long? id)
        {
            DocumentDetail _documentDetail = await db.DocumentDetails.FindAsync(id);
            if (_documentDetail == null)
            {
                return View();
            }
            if (_documentDetail.AttachModels.Count > 0)
            {
                TempData["Error"] = "Tài liệu này không thể xóa vì chứa tệp đính kèm.";
                return RedirectToAction("DeleteDocument");
            }

            db.DocumentDetails.Remove(_documentDetail);
            await db.SaveChangesAsync();
            TempData["Updated"] = "Xóa tài liệu thành công";
            return RedirectToRoute("AdminManagerDocuments");
        }

        public ActionResult AddAttachDocument(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            DocumentDetail _documentDetai = db.DocumentDetails.Find(id);
            if (_documentDetai == null)
            {
                return View(_documentDetai);
            }

            return View(_documentDetai);
        }

        // SaveFileAttach
        public ActionResult SaveFileAttach(int? DetailId, string DocumentName)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            string fContentType = "";
            int? fsize = 1;
            string _forderDocument = "";

            if (DocumentName != null && DocumentName != "")
            {
                _forderDocument = Configs.unicodeToNoMark(DocumentName);
            }
            else
            {
                _forderDocument = "ExampleForder";
            }
            try
            {
                int i = 1;
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}SaveFile\\AttachFiles", Server.MapPath(@"\")));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                        //string strDay = DateTime.Now.ToString("yyyyMMdd");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), _forderDocument);

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = file.FileName;
                        fContentType = file.ContentType;
                        fsize *= file.ContentLength * 1024 * 1024;
                        fPath = string.Format("/SaveFile/AttachFiles/{0}/{1}", _forderDocument, _fileName);
                    }


                    // luu đính kèm vào csdl
                    AttachModel _newAttach = new AttachModel();
                    _newAttach.AttachFileName = fName ?? null;
                    _newAttach.AttachFilePath = fPath ?? null;
                    _newAttach.DetailId = DetailId;
                    _newAttach.ContentType = fContentType ?? null;
                    _newAttach.ContentLength = fsize ?? (int?)null;
                    db.AttachModels.Add(_newAttach);
                    db.SaveChanges();
                    i++;

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { id = DetailId, Message = fName });
            }
            else
            {
                return Json(new { Message = "Có lỗi khi lưu tệp tin" });
            }
        }

        public ActionResult EditAttach(long? id, string DocumentName)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            AttachModel _fileAtach = db.AttachModels.Find(id);
            if (_fileAtach == null)
            {
                return View(_fileAtach);
            }

            return View(_fileAtach);

        }

        //ChangeFileAttach
        public ActionResult ChangeFileAttach(long? AttackId, string DocumentName)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            string fContentType = "";
            int? fsize = 1;
            string _forderDocument = "";

            if (DocumentName != null && DocumentName != "")
            {
                _forderDocument = Configs.unicodeToNoMark(DocumentName);
            }
            else
            {
                _forderDocument = "ExampleForder";
            }
            try
            {
                int i = 1;
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}SaveFile\\AttachFiles", Server.MapPath(@"\")));
                        //string strDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                        //string strDay = DateTime.Now.ToString("yyyyMMdd");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), _forderDocument);

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = file.FileName;
                        fContentType = file.ContentType;
                        fsize *= file.ContentLength * 1024 * 1024;
                        fPath = string.Format("/SaveFile/AttachFiles/{0}/{1}", _forderDocument, _fileName);
                    }


                    // Cập nhạt tệp tin đính kèm  mới vào csdl
                    var _attach = db.AttachModels.Find(AttackId);
                    if (_attach != null)
                    {
                        // Xóa file đính kèm cũ và cập nhật file đính kèm mới
                        string path = Server.MapPath(_attach.AttachFilePath);
                        FileInfo fileInfo = new FileInfo(path);
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                        _attach.AttachFileName = fName ?? null;
                        _attach.AttachFilePath = fPath ?? null;
                        _attach.ContentType = fContentType ?? null;
                        _attach.ContentLength = fsize ?? (int?)null;
                        db.SaveChanges();
                    }
                    
                    i++;

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { id = AttackId, Message = fName });
            }
            else
            {
                return Json(new { Message = "Có lỗi khi lưu tệp tin" });
            }
        }

        public ActionResult DeleleAttach(long? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            AttachModel _attach = db.AttachModels.Find(id);
            if (_attach == null)
            {
                return View(_attach);
            }
            return View(_attach);
        }

        [HttpPost, ActionName("DeleleAttach")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleleAttachConfirmed(long? id)
        {
            AttachModel _attach = db.AttachModels.Find(id);
            if (_attach == null)
            {
                return View();
            }
            long? _DetailId = _attach.DetailId;
            try
            {
                // Xóa file đính kèm luu trên máy chủ và xóa file đính kèm của tài liệu.
                string path = Server.MapPath(_attach.AttachFilePath);
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                db.AttachModels.Remove(_attach);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Xóa tệp đính kèm thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa tệp đính kèm";
                return RedirectToAction("DeleleAttach");
            }    
            
            return RedirectToAction("AddAttachDocument", new { id = _DetailId });
        }

        

        [AllowAnonymous]
        public ActionResult GetDetailDocument(string slugurl, long? id)
        {
            if (slugurl == null || id == null)
            {
                return View();
            }
            var _document = db.DocumentDetails.Where(a => a.DetailSlug == slugurl && a.DetailId == id && a.Published == true).FirstOrDefault();
            if (_document == null)
            {
                return View();
            }
            // Tăng lượt view lên
            _document.CountView = _document.CountView != null ? _document.CountView += 1 : 1;
            db.SaveChanges();
            return View(_document);
        }

        [AllowAnonymous]
        public ActionResult GetAllDocuments(int? pg, int? DocumentId, string search_document)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            //var data = from d in db.DocumentDetails where d.Published == true select d;
            IEnumerable<DocumentDetail> data = new List<DocumentDetail>();
            if (DocumentId != null && DocumentId != 0)
            {
                //data = data.Where(x => x.DocumentId == DocumentId);
                ViewBag.DocumentId = DocumentId;
                ViewBag.DocumentName = db.Documents.Find(DocumentId).DocumentName;
                data = GetDocumentDetailOfDocuments(DocumentId);
            }
            else
            {
                data = from d in db.DocumentDetails where d.Published == true select d;
            }

            if (!string.IsNullOrEmpty(search_document))
            {
                search_document = search_document.Trim();
                data = data.Where(x => x.DetailName.ToLower().Contains(search_document));
                ViewBag.search_document = search_document;
            }

            data = data.OrderByDescending(a => a.PublishedDate);

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public void SetDocumentDetail(ICollection<Document> ic, List<DocumentDetail> _documentsdetail)
        {
            foreach (var c1 in ic)
            {
                if (c1.DocumentDetails.Count > 0)
                {
                    _documentsdetail.AddRange(c1.DocumentDetails);
                }
                if (c1.Documents1.Count > 0)
                {
                    SetDocumentDetail(c1.Documents1, _documentsdetail);
                }
            }
        }

        [AllowAnonymous]
        public IEnumerable<DocumentDetail> GetDocumentDetailOfDocuments(int? id)
        {
            //var _cat = db.Categories.Where(x => x.CategoryId == id && x.SlugCategory == slugUrl).FirstOrDefault();
            var _document = db.Documents.Where(x => x.DocumentId == id).FirstOrDefault();
            List<DocumentDetail> _documentdetails = new List<DocumentDetail>();
            if (_document != null)
            {
                //ViewBag.category = _cat.CategoryName;
                if (_document.Documents1.Count > 0)
                {
                    SetDocumentDetail(_document.Documents1, _documentdetails);
                }
                else
                {
                    _documentdetails.AddRange(_document.DocumentDetails);
                }
            }
            else
            {
                _documentdetails = null;
            }
            return _documentdetails;
        }

        [AllowAnonymous]
        public ActionResult DownloadDocument(long? id)
        {
            if (id == null || id == 0)
            {
                return Content("Không tìm thấy tài liệu.");
            }
            try
            {
                var myFile = db.AttachModels.Find(id);
                if (myFile != null)
                {
                    string path = Server.MapPath(myFile.AttachFilePath);
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        return File(new FileStream(path, FileMode.Open), System.Net.Mime.MediaTypeNames.Application.Octet, myFile.AttachFileName);
                    }
                }
            }
            catch
            {
                return Content("Tài liệu không tồn tại hoặc đã bị xóa.");
            }
            return Content("Tài liệu không tồn tại hoặc đã bị xóa.");
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