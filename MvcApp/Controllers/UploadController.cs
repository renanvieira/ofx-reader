using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            ViewData["Menu"] = "Upload";
            return View();
        }

        [HttpPost]
        public ActionResult File()
        {
            try
            {
                if (Request.Files.Count > 0)
                {

                    HttpPostedFileBase item = Request.Files[0];
                    if (item.ContentLength > 0 && Path.GetExtension(item.FileName).ToLower().Contains("ofx") == true)
                    {

                        string uploadPath = Server.MapPath("~/Content/Uploads");
                        var random = new Random(DateTime.Now.Millisecond);
                        string fileName = string.Format("{0}_{1}.{2}", Path.GetFileNameWithoutExtension(item.FileName), random.Next(int.MaxValue), Path.GetExtension(item.FileName));
                        string filePath = Path.Combine(uploadPath, fileName);

                        item.SaveAs(filePath);

                        var file = OFXParser.ReadFile(filePath);

                        int duplicatedRecords = 0;
                        int totalRecords = file.Transactions.Count;
                        this.PersistTransactions(file, out duplicatedRecords);


                        ViewData["DuplicatedMessage"] = string.Format("Foram encontrados {0} registro duplicados de {1}.", duplicatedRecords, totalRecords);
                        ViewData["Message"] = "Arquivo salvo com sucesso!";
                        ViewData["Success"] = true;
                    }
                    else
                    {
                        ViewData["Message"] = "Arquivo inválido.";
                        ViewData["Success"] = false;
                    }

                }
                else
                {
                    ViewData["Message"] = "Envie ao menos um arquivo";
                }
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Erro ao salvar o arquivo!";
                ViewData["Success"] = false;
            }

            return View("Index");

        }

        private void PersistTransactions(OFXFile file, out int duplicatedRecords)
        {
            duplicatedRecords = 0;
            using (DatabaseEntities efModel = new DatabaseEntities())
            {

                foreach (var item in file.Transactions)
                {

                    if (efModel.OFXTransactions.Any(x => x.uniqueId == item.UniqueId) == false)
                    {

                        Models.OFXTransaction transaction = new Models.OFXTransaction();

                        transaction.amount = item.Amount;
                        transaction.accountId = file.AccountId;
                        transaction.bankId = file.BankId;
                        transaction.checkNum = item.CheckNum;
                        transaction.fitID = item.FitId;
                        transaction.memo = item.Memo;                        
                        transaction.type = item.Type;
                        transaction.uniqueId = item.UniqueId;
                        transaction.date = item.DatePosted;


                        efModel.OFXTransactions.Add(transaction);

                    }
                    else
                    {
                        duplicatedRecords += 1;
                    }

                }

                efModel.SaveChanges();
            }

        }

    }
}
