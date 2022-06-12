using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TradeResourcesPlugin.Helpers.Agreements;
using TradeResourcesPlugin.Modules.HuntingMenus.Agreements;
using UsersResources;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaHelpers;
using YodaHelpers.ActionMenus;
using YodaHelpers.Components;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers
{
    public abstract class DefaultAgrTemplate<T> : DefaultAgrTemplate where T : DefaultAgrTemplate {
        public override DefaultAgrTemplate[] DeserializeModels(string json)
        {
            return JsonConvert.DeserializeObject<T[]>(json);
        }
        public override DefaultAgrTemplate Copy()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
        }
        public override DefaultAgrTemplate[] SetModels(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            if (env.Args.AgreementId > 0)
            {
                var revId = AgreementHelper.GetAgreementActiveRevisionId(env.Args.AgreementId, env.QueryExecuter);
                return DeserializeModels(new TbAgreementModels().AddFilter(t => t.flAgreementRevisionId, revId).SelectScalar(t => t.flModels, env.QueryExecuter)).ToArray();
            }
            else
            {
                ConfirmLanguages();
                FillModel(env);
                return Languages.Select(language =>
                {
                    var langModel = Copy();
                    langModel.Language = language;
                    if (language != SetDefaultLanguage())
                        langModel.TranslateModel(env);
                    return langModel;
                }).ToArray();
            }
        }
    }
    public abstract class DefaultAgrTemplate : DefaultDocTemplate
    {
        public string Language = null;
        public string[] Languages;

        public struct Date {
            public int Year { get; }
            public int Month { get; }
            public int Day { get; }
            public Date (int year, int month, int day)
            {
                Year = year;
                Month = month;
                Day = day;
            }
            public override string ToString() {
                return $"{$"{Day}".PadLeft(2, '0')}.{$"{Month}".PadLeft(2, '0')}.{$"{Year}".PadLeft(2, '0')}";
            }

            public static Date Now
            {
                get {
                    return new Date(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                }
            }
            public DateTime DateTime
            {
                get {
                    return new DateTime(Year, Month, Day);
                }
            }
        }
        public abstract void SetAgreementNumber(int agreementId);
        public abstract string SetDefaultLanguage();
        public abstract void TranslateModel(ActionEnv<DefaultAgrTemplateArgs> env);
        public class RequisitesModel
        {
            public string flName { get; set; }
            public string flXin { get; set; }
            public string flBik { get; set; }
            public string flIban { get; set; }
            public int? flKbe { get; set; }
            public int? flKnp { get; set; }
            public string flKbk { get; set; }
            public string flContacts { get; set; }
        }
        public class PaymentAndOverpaymentRequisitesModel
        {
            public RequisitesModel flPayment { get; set; }
            public RequisitesModel flOverPayment { get; set; }
        }
        public PaymentAndOverpaymentRequisitesModel GetPaymentAndOverpaymentRequisites(FormEnvironment env, int AgreementId) {
            return GetPaymentAndOverpaymentRequisites(new ActionEnv<DefaultAgrTemplateArgs>(
                "",
                new DefaultAgrTemplateArgs { AgreementId = AgreementId },
                ActionFlowStep.Rendering,
                env.RequestContext,
                env.FormCollection,
                env.ViewData,
                env.Redirect,
                null,
                null
            ));
        }
        public abstract PaymentAndOverpaymentRequisitesModel GetPaymentAndOverpaymentRequisites(ActionEnv<DefaultAgrTemplateArgs> env);
        public class SideAccountData {
            public string flName { get; set; }
            public string flXin { get; set; }
            public string flAccountType { get; set; }
        }
        public class SidesAccountsData {
            public SideAccountData flWinner { get; set; }
            public SideAccountData flSeller { get; set; }
        }
        public SidesAccountsData GetSidesAccountData(FormEnvironment env, int AgreementId) {
            return GetSidesAccountData(new ActionEnv<DefaultAgrTemplateArgs>(
                "",
                new DefaultAgrTemplateArgs { AgreementId = AgreementId },
                ActionFlowStep.Rendering,
                env.RequestContext,
                env.FormCollection,
                env.ViewData,
                env.Redirect,
                null,
                null
            ));
        }
        public abstract SidesAccountsData GetSidesAccountData(ActionEnv<DefaultAgrTemplateArgs> env);
        public PaymentItemModel[] GetGuaranteePayments(FormEnvironment env, int AgreementId) {
            return GetGuaranteePayments(new ActionEnv<DefaultAgrTemplateArgs>(
                "",
                new DefaultAgrTemplateArgs { AgreementId = AgreementId },
                ActionFlowStep.Rendering,
                env.RequestContext,
                env.FormCollection,
                env.ViewData,
                env.Redirect,
                null,
                null
            ));
        }
        public abstract PaymentItemModel[] GetGuaranteePayments(ActionEnv<DefaultAgrTemplateArgs> env);
        public string Translate(IYodaRequestContext requestContext, string language, string text)
        {
            return requestContext.LangTranslator.Translate(text, language, requestContext.InvokingMenu.ModuleName + ":" + requestContext.InvokingMenu.MenuName);
        }
        public abstract DefaultAgrTemplate[] SetModels(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract void FillModel(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract string[] SetLanguages();
        public Dictionary<string, string> GetLanguagesDictionary() {
            return new Dictionary<string, string>()
            {
                { "kz", "На государственном языке" },
                { "ru", "На русском языке" },
                { "en", "На английском языке" }
            };
        }
        public void ConfirmLanguages()
        {
            Languages = SetLanguages();
            Language = SetDefaultLanguage();
        }
        public abstract DefaultAgrTemplate[] DeserializeModels(string json);
        public abstract DefaultAgrTemplate Copy();
        public abstract bool HasAccessToCreate(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract (string module, string action, object routeValues, string project) GetLinkToObject(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract (string module, string action, object routeValues, string project) GetLinkToTrade(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract string GetLinkToEtp(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract bool HasAccessToSign(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract void OnSignEnd(ActionEnv<DefaultAgrTemplateArgs> env, ITransaction transaction);
        public abstract bool IsSignAvailableDate(ActionEnv<DefaultAgrTemplateArgs> env);
        public abstract DateTime SignAvailableDate(ActionEnv<DefaultAgrTemplateArgs> env);
        public bool IsWinner(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId);
            return agrs.SelectScalar(t => t.flWinnerXin, env.QueryExecuter) == env.User.GetUserXin(env.QueryExecuter);
        }
        public bool IsSeller(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId);
            return agrs.SelectScalar(t => t.flSellerBin, env.QueryExecuter) == env.User.GetUserXin(env.QueryExecuter);
        }
        public bool IsAgreementCreator(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            var agrs = new TbAgreements().AddFilter(t => t.flAgreementId, env.Args.AgreementId);
            return agrs.SelectScalar(t => t.flAgreementCreatorBin, env.QueryExecuter) == env.User.GetUserXin(env.QueryExecuter);
        }
        public bool HasAccessToView(ActionEnv<DefaultAgrTemplateArgs> env) {
            return IsWinner(env) || IsSeller(env) || IsAgreementCreator(env);
        }
        public virtual bool HasPayment()
        {
            return true;
        }
        public virtual bool IsInstallment() {
            return false;
        }

        public decimal? GetSellPrice(FormEnvironment env, int AgreementId) {
            return GetSellPrice(new ActionEnv<DefaultAgrTemplateArgs>(
                "",
                new DefaultAgrTemplateArgs { AgreementId = AgreementId },
                ActionFlowStep.Rendering,
                env.RequestContext,
                env.FormCollection,
                env.ViewData,
                env.Redirect,
                null,
                null
            ));
        }
        public virtual decimal? GetSellPrice(ActionEnv<DefaultAgrTemplateArgs> env)
        {
            return null;
        }

        private static string Translit(string str)
        {
            string[] lat_up = { "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya" };
            string[] lat_low = { "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya" };
            string[] rus_up = { "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я" };
            string[] rus_low = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я" };
            for (int i = 0; i <= 32; i++)
            {
                str = str.Replace(rus_up[i], lat_up[i]);
                str = str.Replace(rus_low[i], lat_low[i]);
            }
            return str;
        }

        public abstract string[] SetReadOnlyProprties();
        public Card RenderInputs()
        {
            var readOnlyProprties = SetReadOnlyProprties();
            var mainPanel = new Card(GetLanguagesDictionary()[Language]);

            void renderInput(WidgetBase panel, string name, string pathname, object value, string type)
            {
                YodaFormElement editor;
                if (type == "DateTime")
                {
                    editor = new DateTimeBox { Name = $"{Language}{Translit(pathname)}", DateTime = (DateTime)value };
                }
                else if (type == "Date")
                {
                    editor = new DateBox { Name = $"{Language}{Translit(pathname)}", Date = ((Date)value).DateTime };
                }
                else
                {
                    editor = new Textbox($"{Language}{Translit(pathname)}", null, value + string.Empty);
                }
                var pnl = new Panel
                {
                    CssClass = "form-group row"
                };
                var sb = new StringBuilder();
                sb.AppendHtml("<label for='{0}' class='col-md-3 col-form-label'>{1}</label>", $"{Language}{Translit(pathname)}", Regex.Replace(name, "(\\B[А-Я,A-Z])", " $1"));
                pnl.AddComponent(new HtmlText(sb.ToString()));
                editor.CssClass += " form-control";
                var editPanel = new Panel
                {
                    CssClass = "col-md-9"
                };
                editPanel.AddComponent(editor);
                pnl.AddComponent(editPanel);
                panel.AddComponent(pnl);
            }
            void renderPanel(WidgetBase panel, object model, string name, string pathname) {

                var newPanel = new Card(Regex.Replace(name, "(\\B[А-Я,A-Z])", " $1"))
                {
                    CssClass = "mt-3"
                };
                foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                {
                    if (!propertyInfo.PropertyType.Namespace.StartsWith("System") && propertyInfo.PropertyType.Name != "Date" && !readOnlyProprties.Any(x => x == propertyInfo.Name || (pathname + "." + propertyInfo.Name).Contains(x)))
                    {
                        if (propertyInfo.GetValue(model) == null)
                        {
                            throw new NotImplementedException($"Agreement property is NULL: {propertyInfo.Name}");
                        }
                        renderPanel(newPanel, propertyInfo.GetValue(model), propertyInfo.Name, pathname + "." + propertyInfo.Name);
                    }
                    else if (!readOnlyProprties.Any(x => x == propertyInfo.Name || (pathname + "." + propertyInfo.Name).Contains(x)))
                    {
                        renderInput(newPanel, propertyInfo.Name, (pathname + "." + propertyInfo.Name).Replace(".", string.Empty), propertyInfo.GetValue(model), propertyInfo.PropertyType.Name);

                    }
                   
                }
                panel.AddComponent(newPanel);
            }

            renderPanel(mainPanel, this, GetType().Name, GetType().Name);
            return mainPanel;
        }
        
        public void ValidateInputs(NameValueCollection FormCollection, out Dictionary<string, string> outErrors)
        {
            var readOnlyProprties = SetReadOnlyProprties();
            var errors = new Dictionary<string, string>();

            void validInput(string pathname, string type)
            {
                var value = FormCollection[$"{Language}{Translit(pathname)}"];

                var hasErrors = false;

                if (type == "Int" || type == "Int32")
                {
                    if (!int.TryParse(value, out var intResult))
                    {
                        hasErrors = true;
                    }
                }

                if (type == "Decimal")
                {
                    if (!decimal.TryParse(value, out var decimalResult))
                    {
                        hasErrors = true;
                    }
                }

                if (type == "DateTime")
                {
                    if (!DateTime.TryParse(value, out var dateTimeResult))
                    {
                        hasErrors = true;
                    }
                }

                if (type == "Date")
                {
                    if (!DateTime.TryParse(value, out var dateTimeResult))
                    {
                        hasErrors = true;
                    }
                }

                if (hasErrors)
                {
                    errors.Add($"{Language}{Translit(pathname)}", $"Поле \"{pathname}\" заполнено неверно.");
                }

            }
            void runInputs(object model, string pathname) {
                foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                {
                    if (!propertyInfo.PropertyType.Namespace.StartsWith("System") && propertyInfo.PropertyType.Name != "Date" && !readOnlyProprties.Any(x => x == propertyInfo.Name || (pathname + "." + propertyInfo.Name).Contains(x)))
                    {
                        runInputs(propertyInfo.GetValue(model), pathname + "." + propertyInfo.Name);
                    }
                    else if (!readOnlyProprties.Any(x => x == propertyInfo.Name || (pathname + "." + propertyInfo.Name).Contains(x)))
                    {
                        validInput((pathname + "." + propertyInfo.Name).Replace(".", string.Empty), propertyInfo.PropertyType.Name);

                    }
                   
                }
            }

            runInputs(this, GetType().Name);
            outErrors = errors;
        }
        
        public void SetInputsValues(NameValueCollection FormCollection)
        {
            var readOnlyProprties = SetReadOnlyProprties();
            object getInputValues(string pathname, string type)
            {
                var value = FormCollection[$"{Language}{Translit(pathname)}"];
                object returnValue = null;
                var hasErrors = false;

                if (type == "Int" || type == "Int32")
                {
                    returnValue = Convert.ToInt32(value);
                }

                if (type == "String")
                {
                    returnValue = value;
                }

                if (type == "Decimal")
                {
                    returnValue = Convert.ToDecimal(value);
                }

                if (type == "DateTime")
                {
                    returnValue = Convert.ToDateTime(value);
                }

                if (type == "Date")
                {
                    var DT = Convert.ToDateTime(value);
                    returnValue = new Date(DT.Year, DT.Month, DT.Day);
                }

                return returnValue;

            }
            void runInputs(object model, string pathname) {
                foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                {
                    if (!propertyInfo.PropertyType.Namespace.StartsWith("System") && propertyInfo.PropertyType.Name != "Date" && !readOnlyProprties.Any(x => x == propertyInfo.Name || (pathname + "." + propertyInfo.Name).Contains(x)))
                    {
                        runInputs(propertyInfo.GetValue(model), pathname + "." + propertyInfo.Name);
                    }
                    else if (!readOnlyProprties.Any(x => x == propertyInfo.Name || (pathname + "." + propertyInfo.Name).Contains(x)))
                    {
                        propertyInfo.SetValue(model, getInputValues((pathname + "." + propertyInfo.Name).Replace(".", string.Empty), propertyInfo.PropertyType.Name));
                    }
                   
                }
            }

            runInputs(this, GetType().Name);
        }

        public string NumberToString(long number, string language, bool woman = false)
        {
            var o = new List<List<string>>() { };

            var m = new List<List<string>>() { };
            var r = new List<List<string>>() { };

            if (language == "kz")
            {
                m = new List<List<string>>() {
                    new List<string>() { "нөл" },
                    new List<string>() { "-", "бір", "екі", "үш", "төрт", "бес", "алты", "жеті", "сегіз", "тоғыз"},
                    new List<string>() { "он", "он бір", "он екі", "он үш", "он төрт", "он бес", "он алты", "он жеті", "он сегіз", "он тоғыз" },
                    new List<string>() { "-", "-", "жиырма", "отыз", "қырық", "елу", "алпыс", "жетпіс", "сексен", "тоқсан" },
                    new List<string>() { "-", "жүз", "екі жүз", "үш жүз", "төрт жүз", "бес жүз", "алты жүз", "жеті жүз", "сегіз жүз", "тоғыз жүз" },
                    new List<string>() { "-", "бір", "екі" }
                };

                r = new List<List<string>>() {
                    new List<string>() {"...лион", "", "", ""}, // используется для всех неизвестно больших разрядов 
                    new List<string>() { "мың", "", "", ""},
                    new List<string>() {"миллион", "", "", ""},
                    new List<string>() {"миллиард", "", "", ""},
                    new List<string>() {"триллион", "", "", ""},
                    new List<string>() {"квадриллион", "", "", ""},
                    new List<string>() {"квинтиллион", "", "", ""},
                    new List<string>() {"секстилион", "", "", ""},
                    new List<string>() {"септилион", "", "", ""},
                    new List<string>() {"октиллион", "", "", ""},
                    new List<string>() {"ноналион", "", "", ""},
                    new List<string>() {"декалион", "", "", ""},
                    new List<string>() {"эндекалион", "", "", ""},
                    new List<string>() {"додекалион", "", "", ""}
                };
            }
            else
            {
                m = new List<List<string>>() {
                    new List<string>() { "ноль" },
                    new List<string>() { "-", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"},
                    new List<string>() { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"},
                    new List<string>() { "-", "-", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"},
                    new List<string>() { "-", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"},
                    new List<string>() { "-", "одна", "две"}
                };

                r = new List<List<string>>() {
                    new List<string>() {"...лион", "ов", "", "а"}, // используется для всех неизвестно больших разрядов 
                    new List<string>() {"тысяч", "", "а", "и"},
                    new List<string>() {"миллион", "ов", "", "а"},
                    new List<string>() {"миллиард", "ов", "", "а"},
                    new List<string>() {"триллион", "ов", "", "а"},
                    new List<string>() {"квадриллион", "ов", "", "а"},
                    new List<string>() {"квинтиллион", "ов", "", "а"},
                    new List<string>() {"секстилион", "ов", "", "а"},
                    new List<string>() {"септилион", "ов", "", "а"},
                    new List<string>() {"октиллион", "ов", "", "а"},
                    new List<string>() {"ноналион", "ов", "", "а"},
                    new List<string>() {"декалион", "ов", "", "а"},
                    new List<string>() {"эндекалион", "ов", "", "а"},
                    new List<string>() {"додекалион", "ов", "", "а"}
                };
            }

            if (number == 0)
            {
                return m[0][0];
            }

            var num = string.Join("", number.ToString().Reverse());
            var forFullNum = num.Length % 3;
            for (var i = 1; i <= 3 - forFullNum; i++)
            {
                num += "0";
            }
            num = string.Join("", num.Reverse());

            Console.WriteLine(num);

            num = string.Join("", num.Reverse());

            var k = 0;
            var n = -1;

            while (k * 3 < num.Length)
            {
                var curNumGroup = num.Substring(3 * k, 3);
                curNumGroup = string.Join("", curNumGroup.Reverse());
                Console.WriteLine(curNumGroup);
                if (curNumGroup != "000")
                {
                    ++n;
                    o.Add(new List<string>() { });
                }
                else
                {
                    k++;
                    continue;
                }
                for (var i = 0; i <= 2; i++)
                {
                    var curNumGroupCurNum = Convert.ToInt32(curNumGroup[i].ToString());
                    if (curNumGroupCurNum == 0)
                    {
                        continue;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                o[n].Add(m[4][curNumGroupCurNum]);
                                break;
                            case 1:
                                if (curNumGroupCurNum == 1 /*десятичные (одинадцать, двенадцать и т.д.)*/)
                                {
                                    o[n].Add(m[2][Convert.ToInt32(curNumGroup[2].ToString())]);
                                    i = 3;//прерываем цикл for
                                    continue;
                                }
                                else
                                {
                                    o[n].Add(m[3][curNumGroupCurNum]);
                                }
                                break;
                            case 2:
                                if ((k == 1 && curNumGroupCurNum <= 2) || (curNumGroupCurNum <= 2 && woman && k == 0))
                                {
                                    o[n].Add(m[5][curNumGroupCurNum]);
                                }
                                else
                                {
                                    o[n].Add(m[1][curNumGroupCurNum]);
                                }
                                break;
                        }
                    }
                }
                if (!new[] { "", "0", "00", "000" }.Contains(curNumGroup) && k > 0)
                {
                    var lastInCurNumGroup = curNumGroup[curNumGroup.Length - 1];
                    o[n].Add(r[k][0] + (new[] { '1' }.Contains(lastInCurNumGroup) ? r[k][2] : new[] { '2', '3', '4' }.Contains(lastInCurNumGroup) ? r[k][3] : r[k][1]));
                }
                k++;
            }

            o.Reverse();
            return string.Join(" ", o.Select(x => string.Join(" ", x)));
        }

    }

    public class DefaultAgrTemplateArgs : ActionQueryArgsBase {
        public int AgreementId { get; set; }
        public string AgreementType { get; set; }
        public string SellerBin { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public int TradeId { get; set; }
        public string TradeType { get; set; }
        public int AuctionId { get; set; }
        public string WinnerXin { get; set; }
    }
}
