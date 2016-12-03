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
using System.IO;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ContactController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: Contact
        public ActionResult Index()
        {
            var data = db.Contacts.Where(x => x.Published == true).Select(x => x).FirstOrDefault();
            if (data == null)
            {
                return RedirectToAction("AddNewContact");
            }
            var getContacts = new ContactModel()
            {
                ContactId = data.ContactId,
                ContactImage = data.ContactImage,
                ContactTitle = data.ContactTitle,
                ContactContent = data.ContactContent,
                Published = (bool)data.Published
            };
            return View(getContacts);
        }

        //UpdateContact
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateContact(ContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            try
            {
                var _contact = await db.Contacts.FindAsync(model.ContactId);
                if (_contact != null)
                {
                    if (_contact.ContactImage != model.ContactImage)
                    {
                        // xóa file ảnh cũ
                        string path = Server.MapPath(_contact.ContactImage);
                        FileInfo fileInfo = new FileInfo(path);
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                    }           
                    _contact.ContactTitle = model.ContactTitle ?? "Tên liên hệ";
                    _contact.ContactImage = model.ContactImage ?? null;
                    _contact.ContactContent = model.ContactContent ?? null;
                    _contact.Published = model.Published;
                    await db.SaveChangesAsync();
                    TempData["Updated"] = "Cập nhật liên hệ thành công";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi cập nhật liên hệ.");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        public ActionResult DsContact(int? pg)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = db.Contacts.ToList();
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        //SaveUploadedFile 
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
                        //string strDay = DateTime.Now.ToString("yyyyMMdd");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "contact");

                        var _fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        file.SaveAs(path);
                        fName = "/Images/WallImages/contact/" + _fileName;
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

        public ActionResult DeleteContact(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }
            Contact _contact = db.Contacts.Find(id);
            if (_contact == null)
            {
                return View();
            }
            return View(_contact);
        }

        [HttpPost, ActionName("DeleteContact")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteContactConfirmed(int? id)
        {
            Contact _contact = await db.Contacts.FindAsync(id);
            if (_contact == null)
            {
                return View();
            }
            if (_contact.Published == true)
            {
                TempData["Error"] = "Bạn không thể xóa liên hệ này. <br />Liên hệ này đang được mở.";
                return RedirectToAction("DeleteContact");
            }
            if (_contact.ContactImage != null)
            {
                // xóa file ảnh cũ
                string path = Server.MapPath(_contact.ContactImage);
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }

            db.Contacts.Remove(_contact);
            await db.SaveChangesAsync();
            TempData["Updated"] = "Xóa liên hệ thành công";
            return RedirectToAction("AddNewContact");
        }

        public ActionResult AddNewContact()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewContact(ContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddNewContact");
            }
            try
            {
                Contact _newContact = new Contact();
                _newContact.ContactTitle = model.ContactTitle ?? "Tên liên hệ";
                _newContact.ContactImage = model.ContactImage ?? null;
                _newContact.ContactContent = model.ContactContent ?? null;
                _newContact.Published = model.Published;
                db.Contacts.Add(_newContact);
                await db.SaveChangesAsync();
                TempData["Updated"] = "Đã thêm mới liên hệ.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi khi thêm mới liên hệ");
                return RedirectToAction("AddNewContact");
            }
            return RedirectToAction("AddNewContact");
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