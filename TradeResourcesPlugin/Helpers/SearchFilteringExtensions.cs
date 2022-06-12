using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using Yoda.YodaReferences;
using YodaApp;
using YodaApp.UiSearch;
using YodaHelpers;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public static class SearchFilteringExtensions {
        public static Filter<TQuery> AddField<TQuery>(this Filter<TQuery> filter, Func<TQuery, ReferenceTextField> field, bool enableFiltering = true, string filterPlaceholder = null, string customFieldName = null, Func<string, bool> optionFilter = null, bool includeSelectAllOption = true) where TQuery : IQueryItem {
            optionFilter = (optionFilter ?? ((Func<string, bool>)((string x) => true)));
            filter.Add(delegate ((TQuery query, IYodaRequestContext context) env) {
                ReferenceTextField f = field(env.query);
                string text = customFieldName ?? f.FieldName;
                StringValues selectedVals = env.context.ActionContext.HttpContext.Request.Query[text];
                if (selectedVals.Count > 0) {
                    env.query.AddFilter((TQuery t) => f, (from x in selectedVals
                                                          select (string)(x) into x
                                                          where optionFilter(x)
                                                          select x).ToArray());
                }

                List<ReferenceItem> CleanReferenceAfterLevel(ReferenceItemCollection reference, int level) {
                    var ret = new List<ReferenceItem>();
                    reference.Each(item => {
                        if (item.Level <= level) {
                            var itemObject = new ReferenceItem() { Value = item.Value, Text = item.Text };
                            if (item.Level < level) {
                                CleanReferenceAfterLevel(item.Items, level).Each(x => itemObject.Items.Add(x));
                            }
                            ret.Add(itemObject);
                        }
                    });
                    return ret;
                }
                string ReferenceItemObjectsToOptions(List<ReferenceItem> reference, StringValues selectedValues) {
                    var ret = "";
                    string refItemToObject(ReferenceItem item, bool selected) {
                        string itemObject = $"<option value=\"{item.Value}\" {(selected ? "selected" : string.Empty)}>{item.Text.Text.ToHtml()}</option>";
                        if (item.Items?.Count() > 0) {
                            itemObject = $"<optgroup label='{item.Text.Text.ToHtml()}'>{item.Items.Select(itemChild => refItemToObject(itemChild, selectedValues.Contains<string>(itemChild.Value.ToString()))).JoinStr("")}</optgroup>";
                        }

                        return itemObject;
                    }
                    ret += reference.Select(item => refItemToObject(item, selectedValues.Contains<string>(item.Value.ToString()))).JoinStr("");
                    return ret;
                }

                var reference = env.context.References.GetReference(f.ReferenceName);
                var referenceItemObjects = CleanReferenceAfterLevel(reference.Items, f.ReferenceLevel);

                if (filterPlaceholder == null && enableFiltering) {
                    filterPlaceholder = env.context.T("Поиск");
                }

                return new HtmlText(
                    $@"
                        <label for='{text.ToHtml()}' class='form-label'>{f.Text.Text.ToHtml()}</label>
                        <select
                            name='{text}'
                            id='{text}'
                            class='multiselect form-control'
                            multiple='multiple'
                            data-non-selected-text='{f.Text.Text.ToHtml()}'
                            data-include-select-all-option='{includeSelectAllOption.ToString().ToLower()}'
                            data-enable-filtering='{enableFiltering.ToString().ToLower()}'
                            data-filter-placeholder='{filterPlaceholder}'
                        >
                            {ReferenceItemObjectsToOptions(referenceItemObjects, selectedVals)}
                        </select>",
                        new string[]
                        {
                        "bootstrap-multiselect"
                        }
                    );
            });
            return filter;
        }
        public static Filter<TQuery> AddFieldDateTime<TQuery>(this Filter<TQuery> filter, Func<TQuery, DateTimeField> field, string customFieldName = null) where TQuery : IQueryItem {
            filter.Add(delegate ((TQuery query, IYodaRequestContext context) env) {
                DateTimeField f = field(env.query);
                string str = customFieldName ?? f.FieldName;
                string text = str + "_from";
                string text2 = str + "_to";
                string text3 = env.context.ActionContext.HttpContext.Request.Query[text].ToString();
                string text4 = env.context.ActionContext.HttpContext.Request.Query[text2].ToString();
                bool flag = false;
                bool flag2 = false;
                DateTime? date = null;
                DateTime? date2 = null;
                if (!string.IsNullOrEmpty(text3)) {
                    if (!DateTime.TryParse(text3, out DateTime result)) {
                        flag = true;
                    }
                    else {
                        date = result;
                        env.query.AddFilter((TQuery t) => f, ConditionOperator.GreateOrEqual, date.Value.Date);
                    }
                }

                if (!string.IsNullOrEmpty(text4)) {
                    if (!DateTime.TryParse(text4, out DateTime result2)) {
                        flag2 = true;
                    }
                    else {
                        date2 = result2;
                        env.query.AddFilter((TQuery t) => f, ConditionOperator.Less, date2.Value.Date.AddDays(1.0));
                    }
                }

                Panel panel = new Panel();
                panel.AddHtml("<label for=\"" + text.ToHtml() + "\" class=\"form-label\">" + f.Text.Text.ToHtml() + "</label>");
                Panel panel2 = new Panel("input-group").AppendTo(panel);
                panel2.AddHtml("<div class=\"input-group-prepend\">\n  <span class=\"input-group-text\"> с </span>\n</div>");
                panel2.AddComponent(new DateBox(text) {
                    CssClass = "form-control",
                    Date = date,
                    RawValue = text3,
                    Attributes = new Attrs {
                        ["placeholder"] = "мин."
                    }
                });
                panel2.AddHtml("<div class=\"input-group-prepend input-group-append\">\n  <span class=\"input-group-text\"> по </span>\n</div>");
                panel2.AddComponent(new DateBox(text2) {
                    CssClass = "form-control",
                    Date = date2,
                    RawValue = text4,
                    Attributes = new Attrs {
                        ["placeholder"] = "макс."
                    }
                });
                return panel;
            });
            return filter;
        }
        public static Filter<TQuery> AddField<TQuery>(this Filter<TQuery> filter, Func<TQuery, IntField> field, TextConditionOperator conditionOperator = TextConditionOperator.Like, string name = null, string placeholderText = null) where TQuery : IQueryItem {
            filter.Add(delegate ((TQuery query, IYodaRequestContext context) env) {
                IntField f = field(env.query);
                if (name == null) {
                    name = f.FieldName;
                }

                if (placeholderText == null) {
                    placeholderText = f.Text;
                }

                var isValid = int.TryParse(env.context.ActionContext.HttpContext.Request.Query[name].ToString(), out var number);
                if (isValid) {
                    env.query.AddFilter((TQuery t) => f, number);
                }

                return new HtmlText("<label for=\"" + name.ToHtml() + "\" class=\"form-label\">" + placeholderText + "</label><input type=\"number\" name=\"" + name.ToHtml() + "\" id=\"" + name.ToHtml() + "\" class=\"form-control\" placeholder=\"" + placeholderText.ToHtml() + "\" value=\"" + (isValid ? number.ToString().ToHtml() : "") + "\" />");
            });
            return filter;
        }
    }
}