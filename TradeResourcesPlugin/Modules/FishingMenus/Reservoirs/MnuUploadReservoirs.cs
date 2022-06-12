//using CommonSource.References.Object;
//using CommonSource;
//using FishingSource.References.Object;
//using Microsoft.AspNetCore.Mvc;
//using OfficeOpenXml;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Xml.Linq;
//using TradeResourcesPlugin.Helpers;
//using Yoda.Interfaces;
//using Yoda.Interfaces.Forms;
//using Yoda.Interfaces.Forms.Components;
//using Yoda.Interfaces.Menu;
//using Yoda.YodaReferences;
//using YodaQuery;

//namespace TradeResourcesPlugin.Modules.FishingMenus.Reservoirs {
//    public class MnuUploadReservoirs : FrmMenu {

//        public MnuUploadReservoirs(string moduleName) : base(nameof(MnuUploadReservoirs), "Загрузить водоёмы")
//        {
//            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
//            Access();
//            OnRendering(re =>
//            {
//                if (!re.IsPostback || !re.Form.ViewData.ModelState.IsValid)
//                {
//                    var fileInput = new FileInput { Name = "reservoirsFile" };
//                    re.Form.AddComponent(fileInput);
//                    re.Form.AddComponent(new Submit("UploadFile", "Загрузить"));

//                    re.Form.AddComponent(new Submit("DownloadExample", "Скачать пример талбицы"));
//                    re.Form.AddComponent(new Submit("DownloadKatoReference", "Скачать справочник областей и районов"));
//                }
//                else if (!string.IsNullOrWhiteSpace(re.Form.FormCollection["uploadedFileId"]) || re.Form.ViewData["uploadedFileId"] != null)
//                {
//                    var fileId = re.Form.FormCollection["uploadedFileId"] ?? (re.Form.ViewData["uploadedFileId"] + string.Empty);
//                    re.Form.AddHidden("uploadedFileId", fileId);
//                    re.Form.AddComponent(new Submit("ApplyReservoirsFile", "Подтвердить загрузку"));
//                }
//            });
//            OnValidating(ve =>
//            {
//                if (ve.FormCollection.AllKeys.Contains("UploadFile"))
//                {
//                    if (ve.RequestContext.ActionContext.HttpContext.Request.Form.Files.Count == 0)
//                    {
//                        ve.AddModelError("reservoirsFile", "не указан файл");
//                    }
//                    else
//                    {
//                        var file = ve.RequestContext.ActionContext.HttpContext.Request.Form.Files[0];
//                        var content = new byte[file.Length];
//                        using (var stream = file.OpenReadStream())
//                        {
//                            stream.Read(content, 0, content.Length);
//                        }
//                        var fileId = FileReaderHelper.SaveFileAndGetId(ve.RequestContext, content);

//                        ve.ViewData["uploadedFileId"] = fileId;
//                    }
//                }
//            });
//            OnProcessing(pe => {
//                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//                if (pe.FormCollection.AllKeys.Contains("ApplyReservoirsFile"))
//                {
//                    var fileId = pe.FormCollection["uploadedFileId"];
//                    var filePath = FileReaderHelper.GetTempFilePath(pe.RequestContext, fileId);
//                    if (!File.Exists(filePath))
//                    {
//                        pe.ViewData.ModelState.AddModelError("filenotfound", "Временный файл для загрузки не найден в кэше. Возможно удалили администраторы.");
//                    }
//                    var parsedRows = ParseReservoirFile(out var errors, filePath, pe.QueryExecuter);
//                    if (errors.Length > 0)
//                    {
//                        errors.Each(error => pe.ViewData.ModelState.AddModelError("filenotfound", error));
//                    }
//                    else
//                    {
//                        using (var transaction = pe.QueryExecuter.BeginTransaction(NpGlobal.DbKeys.DbYodaSystemGr))
//                        {
//                            parsedRows.Each(x => CreateOrders.ForReservoirObject(x.Name, x.Country, x.Region, x.District, x.Area, 8, 7, pe.QueryExecuter, transaction));
//                            transaction.Commit();
//                        }
//                        pe.SetPostbackMessage("Данные файла загружены");
//                        pe.Redirect.SetRedirect(moduleName, nameof(MnuUploadReservoirs));
//                    }
//                }
//                else if (pe.FormCollection.AllKeys.Contains("DownloadExample"))
//                {
//                    using (var ms = new MemoryStream())
//                    {
//                        using (var xlPackage = new ExcelPackage(ms))
//                        {
//                            var wb = xlPackage.Workbook;
//                            var ws = wb.Worksheets.Add("Лист 1");

//                            foreach (ReservoirExcelCols reservoirExcelCol in Enum.GetValues(typeof(ReservoirExcelCols)))
//                            {
//                                ws.Cells[1, (int)reservoirExcelCol].Value = ReservoirExcelColsTexts[reservoirExcelCol];
//                            }

//                            xlPackage.Save();

//                            var file = ms.ToArray();

//                            if (file != null)
//                            {
//                                pe.Redirect.SetRedirectToFile(new FileContentResult(file,
//                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
//                                { FileDownloadName = $"пример таблицы водоёмов.xlsx" });
//                            }
//                        }
//                    }
//                }
//                else if (pe.FormCollection.AllKeys.Contains("DownloadKatoReference"))
//                {
//                    using (var ms = new MemoryStream())
//                    {
//                        using (var xlPackage = new ExcelPackage(ms))
//                        {
//                            var wb = xlPackage.Workbook;
//                            var ws = wb.Worksheets.Add("Лист 1");

//                            var refKato = new RefKato();
//                            var rowNum = 1;
//                            void writeItems(ReferenceItemCollection items, int level)
//                            {
//                                items.Each(x => {
//                                    ws.Cells[rowNum, level].Value = x.Text.Text;
//                                    if (x.Items != null && x.Items.Count > 0)
//                                        writeItems(x.Items, level + 1);
//                                    rowNum++;
//                                });
//                            }
//                            writeItems(refKato.Items, 1);

//                            xlPackage.Save();

//                            var file = ms.ToArray();

//                            if (file != null)
//                            {
//                                pe.Redirect.SetRedirectToFile(new FileContentResult(file,
//                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
//                                { FileDownloadName = $"справочник Като.xlsx" });
//                            }
//                        }
//                    }
//                }
//            });
//        }


//        private Dictionary<ReservoirExcelCols, string> ReservoirExcelColsTexts = new Dictionary<ReservoirExcelCols, string>() {
//            { ReservoirExcelCols.Name, "Наименование" },
//            { ReservoirExcelCols.Region, "Область" },
//            { ReservoirExcelCols.District, "Район" },
//            { ReservoirExcelCols.Area, "Площадь" },
//        };

//        private enum ReservoirExcelCols {
//            Name = 1,
//            Region = 2,
//            District = 3,
//            Area = 4
//        }

//        private class ReservoirDataRow {

//            public ReservoirDataRow(string name, string country, string region, string district, decimal? area)
//            {
//                Name = name;
//                Country = country;
//                Region = region;
//                District = district;
//                Area = area;
//            }
//            public string Name { get; set; }
//            public string Country { get; set; }
//            public string Region { get; set; }
//            public string District { get; set; }
//            public decimal? Area { get; set; }
//        }

//        private ReservoirDataRow[] ParseReservoirFile (out string[] resultErrors, string filePath, IQueryExecuter queryExecuter)
//        {
//            var result = new ReservoirDataRow[] { };
//            var errors = new string[] { };

//            void addError(int rowNumber, string text)
//            {
//                errors = errors.Append($"Строка {rowNumber}: {text}").ToArray();
//            }

//            FileInfo fileInfo = new FileInfo(filePath);
//            var package = new ExcelPackage(fileInfo);
//            var ws = package.Workbook.Worksheets.FirstOrDefault();
//            var rowsCount = ws.Dimension.Rows;

//            var refKato = new RefKato();
//            var prevErrsCount = 0;
//            for (var rowCounter = 1; rowCounter <= rowsCount; rowCounter++)
//            {
//                if (rowCounter == 1)
//                {//игнорируем первую строку, так как это заголовок
//                    continue;
//                }

//                string Name = null;
//                var nameVal = ws.Cells[rowCounter, (int)ReservoirExcelCols.Name].Value;
//                if (nameVal != null)
//                {
//                    Name = nameVal.ToString();
//                }

//                string Country = null;

//                string Region = null;
//                var regionVal = ws.Cells[rowCounter, (int)ReservoirExcelCols.Region].Value;
//                if (regionVal != null)
//                {
//                    Region = regionVal.ToString();
//                }

//                string District = null;
//                var districtVal = ws.Cells[rowCounter, (int)ReservoirExcelCols.District].Value;
//                if (districtVal != null)
//                {
//                    District = districtVal.ToString();
//                }

//                decimal? Area = null;
//                string AreaString = null;
//                var areaStringVal = ws.Cells[rowCounter, (int)ReservoirExcelCols.Area].Value;
//                if (areaStringVal != null)
//                {
//                    AreaString = areaStringVal.ToString().Replace(" ", "");
//                }

//                if (string.IsNullOrEmpty(District))
//                {
//                    District = null;

//                    if (string.IsNullOrEmpty(Region))
//                    {
//                        addError(rowCounter, "Обязательны к заполнению район или область");
//                    }
//                    else
//                    {
//                        var regionCode = refKato.SearchByText(Region);

//                        if (regionCode == null || regionCode.Level != 1)
//                        {
//                            addError(rowCounter, $"Неизвестная область: \"{Region}\"");
//                        }
//                        else
//                        {
//                            Region = regionCode.Value.ToString();
//                            Country = refKato.Search(Region).Parent.Value.ToString();
//                        }
//                    }

//                }
//                else
//                {
//                    var districtCode = refKato.SearchByText(District);

//                    if (districtCode == null || districtCode.Level != 2)
//                    {
//                        addError(rowCounter, $"Неизвестный район: \"{District}\"");
//                    }
//                    else
//                    {
//                        District = districtCode.Value.ToString();
//                        Region = refKato.Search(District).Parent.Value.ToString();
//                        Country = refKato.Search(Region).Parent.Value.ToString();
//                    }
//                }

//                if (!string.IsNullOrEmpty(AreaString) && AreaString.All(c => (c >= '0' && c <= '9') || (c == '.' || c == ',')))
//                {
//                    Area = Convert.ToDecimal(AreaString.Replace(".", ","));
//                }
//                else
//                {
//                    addError(rowCounter, $"Не удалось понять значение: \"{AreaString}\" (Нужно писать только числа. Точки и запятые принимаются в качестве разделителя дробной части)");
//                }

//                if (prevErrsCount == errors.Length)
//                {
//                    result = result.Append(new ReservoirDataRow(Name, Country, Region, District, Area)).ToArray();
//                }
//            }

//            resultErrors = errors;
//            return result;
//        }

//    }
//}
