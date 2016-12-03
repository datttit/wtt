using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;
using Helpers;
using System.Threading.Tasks;

namespace WebTinTuc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ConfigAccountController : ApplicationBaseController
    {
        private WebTinTucEntities db = new WebTinTucEntities();
        // GET: ConfigAccount
        public ActionResult Index()
        {
            var data = db.ConfigAccounts.FirstOrDefault();
            return View(data);
        }

        //UpdateConfigAccount
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateConfigAccount(ConfigAccount model)
        {
            WebTinTucEntities db = new WebTinTucEntities();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var _config = await db.ConfigAccounts.FindAsync(model.cId);
            if (_config != null)
            {
                _config.Email = model.Email ?? null;
                _config.EmailPass = model.EmailPass ?? null;
                _config.FBAppId = model.FBAppId ?? null;
                _config.FBAppSecret = model.FBAppSecret ?? null;
                _config.FBCallbackPath = model.FBCallbackPath ?? null;
                _config.MailEnableSsl = (bool)model.MailEnableSsl;
                _config.MailHost = model.MailHost ?? null;
                _config.MailPort = model.MailPort;
                _config.MailTimeout = model.MailTimeout;
                await db.SaveChangesAsync();
                TempData["Updated"] = "Cập nhật cấu hình thành công";
                return Redirect("~/ConfigAccount");
            }
            return View(_config);
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