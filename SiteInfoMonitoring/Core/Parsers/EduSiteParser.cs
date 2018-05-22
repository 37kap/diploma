using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SiteInfoMonitoring.Jobs;
using SiteInfoMonitoring.Models;
using WebGrease.Css.Extensions;

namespace SiteInfoMonitoring.Core.Parsers
{
    public class EduSiteParser
    {
        private string SiteName;
        private List<Division> Divisions;
        private List<User> Users;
        public EduSiteParser(string siteName, List<Division> divs, List<User> users)
        {
            SiteName = siteName;
            Divisions = divs;
            Users = users;
        }

        public void StartParse(bool siteNotFound = false, User user = null, bool auto = false)
        {
            if (siteNotFound)
            {
                var admins = Users.Any(u => u.Role == Enums.RolesEnum.admin) ? Users.Where(u => u.Role == Enums.RolesEnum.admin) : null;
                foreach (var admin in admins)
                {
                    new Thread(t => EmailSender.Send(admin.Email, "Сайт " + Settings.SettingsManager.Settings.DefaultSiteAddress + " недоступен.")).Start();
                }
                return;
            }
            for (int i = 0; i < Divisions.Count; i++)
            {
                var parser = new HtmlParser();
                var doc = parser.Parse(GetCode(Divisions[i].Url));
                GetAddReferences(Divisions[i], doc);
                ParsePage(Divisions[i], doc);
                foreach (var url in Divisions[i].Refs)
                {
                    var document = parser.Parse(GetCode(url));
                    ParsePage(Divisions[i], document);
                }
            }
            if (user != null && Settings.SettingsManager.Settings.SendEmails && !auto)
            {
                EmailCreater.CreateInfoEmails(new List<User>() { user }, Divisions);
            }
            if (Settings.SettingsManager.Settings.AutoAnalysis && auto)
            {
                EmailCreater.CreateInfoEmails(Users, Divisions, auto);
            }
        }

        private void ParsePage(Division div, IHtmlDocument doc)
        {
            var fip_td = FindTd(doc);
            var fip_tr = FindTr(doc);
            var fip_all = doc.QuerySelectorAll("[itemprop]")
                .Select((value, index) => new { value, index })
                .Select(v => new { value = v.value.OuterHtml.Remove(0, v.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), v.index })
                .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            foreach (var tag in fip_all)
            {
                if (div.Data.Any(d => d.Value.ToLower() == tag.Value.ToLower()))
                {
                    var needData = div.Data.First(d => d.Value.ToLower() == tag.Value.ToLower());
                    needData.IsExist = true;
                    needData.Count++;
                }
            }
            foreach (var tbl in div.Tables)
            {
                bool allElementsAreNotSame = true;
                if (doc.QuerySelectorAll("table") != null && doc.QuerySelectorAll("table").Any(t => t.OuterHtml.IndexOf(tbl.TableItemprops.First(tip => tip.IsMainTag).Value, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    var dict = doc.QuerySelectorAll("table").First(t => t.OuterHtml.IndexOf(tbl.TableItemprops.First(tip => tip.IsMainTag).Value, StringComparison.OrdinalIgnoreCase) >= 0)
                        .QuerySelectorAll("[itemprop]").Select(v => v.OuterHtml.Remove(0, v.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length))
                        .Select(v => v.Remove(v.IndexOf("\""))).GroupBy(s => s).Select(s => new { Element = s.Key, Count = s.Count() })
                        .ToDictionary(g => g.Element, g => g.Count);
                    if (dict.Count < 2) { allElementsAreNotSame = false; }
                }
                foreach (var tag_tr in fip_tr)
                {
                    KeyValuePair<int, string> tag_tr_next;
                    try
                    {
                        tag_tr_next = fip_tr.First(tr => tr.Key > tag_tr.Key);
                    }
                    catch
                    {
                        tag_tr_next = new KeyValuePair<int, string>(1, null);
                    }
                    if (tbl.TableItemprops.Any(d => d.IsMainTag && d.Value.ToLower() == tag_tr.Value.ToLower())
                        && allElementsAreNotSame)
                    {
                        var needData = tbl.TableItemprops.First(d => d.IsMainTag && d.Value.ToLower() == tag_tr.Value.ToLower());
                        needData.IsExist = true;
                        tbl.IsOk = true;
                        needData.Count++;
                        foreach (var tag_td in fip_td.Where(td => td.Key > tag_tr.Key && (tag_tr_next.Value != null && td.Key < tag_tr_next.Key || tag_tr_next.Value == null)
                                                            && td.Key <= fip_td.First(k => !fip_td.Any(x => x.Key == k.Key + 1) && k.Key > tag_tr.Key).Key))
                        {
                            if (tbl.TableItemprops.Any(d => !d.IsMainTag && d.Value.ToLower() == tag_td.Value.ToLower()))
                            {
                                var needTd = tbl.TableItemprops.First(d => !d.IsMainTag && d.Value.ToLower() == tag_td.Value.ToLower());
                                needTd.IsExist = true;
                                needTd.Count++;
                            }
                        }
                    }
                }
                if (doc.QuerySelectorAll("table").Any(t => t.OuterHtml.IndexOf(tbl.TableItemprops.First(ti => ti.IsMainTag).Value, StringComparison.OrdinalIgnoreCase) >= 0) && allElementsAreNotSame)
                {
                    var table = doc.QuerySelectorAll("table").First(t => t.OuterHtml.IndexOf(tbl.TableItemprops.First(ti => ti.IsMainTag).Value, StringComparison.OrdinalIgnoreCase) >= 0);
                    tbl.RowCount += table.QuerySelector("tbody").QuerySelectorAll("tr").Where(tr => (tr.InnerHtml.Length - tr.InnerHtml.Replace("<td", "").Length) / "<td".Length > 1).Count();
                }
            }
        }

        private Dictionary<int, string> FindTd(IHtmlDocument doc)
        {
            //Ячейки
            var ips = doc.QuerySelectorAll("[itemprop]");
            Dictionary<int, string> f_td1 = ips.Select((value, index) => new { value, index })
                                                          .Where(x => x.value.ParentElement.TagName.ToLower() == "td")
                                                          .Select(i => new { value = i.value.OuterHtml.Remove(0, i.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), i.index })
                                                          .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_td2 = ips.Select((value, index) => new { value, index })
                                                          .Where(x => x.value.ParentElement.ParentElement.TagName.ToLower() == "td")
                                                          .Select(i => new { value = i.value.OuterHtml.Remove(0, i.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), i.index })
                                                          .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_td3 = ips.Select((value, index) => new { value, index })
                                                          .Where(x => x.value.ParentElement.ParentElement.ParentElement.TagName.ToLower() == "td")
                                                          .Select(i => new { value = i.value.OuterHtml.Remove(0, i.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), i.index })
                                                          .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_td4 = ips.Select((value, index) => new { value, index })
                                                          .Where(x => x.value.ParentElement.ParentElement.ParentElement.ParentElement.TagName.ToLower() == "td")
                                                          .Select(i => new { value = i.value.OuterHtml.Remove(0, i.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), i.index })
                                                          .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_td = ips.Select((value, index) => new { value, index }).Where(x => x.value.TagName.ToLower() == "td")
                                                           .Select(i => new { value = i.value.OuterHtml.Remove(0, i.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), i.index })
                                                           .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            Dictionary<int, string> found_itemprops_td = new Dictionary<int, string>(f_td);
            f_td1.ForEach(x => { if (!found_itemprops_td.ContainsKey(x.Key)) found_itemprops_td.Add(x.Key, x.Value); else found_itemprops_td[x.Key] = x.Value; });
            f_td2.ForEach(x => { if (!found_itemprops_td.ContainsKey(x.Key)) found_itemprops_td.Add(x.Key, x.Value); else found_itemprops_td[x.Key] = x.Value; });
            f_td3.ForEach(x => { if (!found_itemprops_td.ContainsKey(x.Key)) found_itemprops_td.Add(x.Key, x.Value); else found_itemprops_td[x.Key] = x.Value; });
            f_td4.ForEach(x => { if (!found_itemprops_td.ContainsKey(x.Key)) found_itemprops_td.Add(x.Key, x.Value); else found_itemprops_td[x.Key] = x.Value; });
            return found_itemprops_td.OrderBy(key => key.Key).ToDictionary(i => i.Key, i => i.Value);
        }

        private Dictionary<int, string> FindTr(IHtmlDocument doc)
        {
            //Строки (главные теги):
            var f_tr1 = doc.QuerySelectorAll("[itemprop]")
                .Select((value, index) => new { value, index })
                .Where(x => x.value.TagName.ToLower() != "td" && x.value.ParentElement.TagName.ToLower() == "tr")
                .Select(v => new { value = v.value.OuterHtml.Remove(0, v.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), v.index })
                .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_tr2 = doc.QuerySelectorAll("[itemprop]")
                .Select((value, index) => new { value, index })
                .Where(x => x.value.TagName.ToLower() != "td" && x.value.ParentElement.TagName.ToLower() != "td" && x.value.ParentElement.ParentElement.TagName.ToLower() == "tr")
                .Select(v => new { value = v.value.OuterHtml.Remove(0, v.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), v.index })
                .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_tr3 = doc.QuerySelectorAll("[itemprop]")
                .Select((value, index) => new { value, index })
                .Where(x => x.value.TagName.ToLower() != "td" && x.value.ParentElement.TagName.ToLower() != "td" && x.value.ParentElement.ParentElement.TagName.ToLower() != "td" && x.value.ParentElement.ParentElement.ParentElement.TagName.ToLower() == "tr")
                .Select(v => new { value = v.value.OuterHtml.Remove(0, v.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), v.index })
                .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_tr4 = doc.QuerySelectorAll("[itemprop]")
                .Select((value, index) => new { value, index })
                .Where(x => x.value.TagName.ToLower() != "td" && x.value.ParentElement.TagName.ToLower() != "td" && x.value.ParentElement.ParentElement.TagName.ToLower() != "td" && x.value.ParentElement.ParentElement.ParentElement.TagName.ToLower() != "td" && x.value.ParentElement.ParentElement.ParentElement.ParentElement.TagName.ToLower() == "tr")
                .Select(v => new { value = v.value.OuterHtml.Remove(0, v.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), v.index })
                .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            var f_tr = doc.QuerySelectorAll("[itemprop]")
                .Select((value, index) => new { value, index })
                .Where(x => x.value.TagName.ToLower() == "tr")
                .Select(v => new { value = v.value.OuterHtml.Remove(0, v.value.OuterHtml.IndexOf("itemprop=\"") + ("itemprop=\"").Length), v.index })
                .Select(v => new { value = v.value.Remove(v.value.IndexOf("\"")), v.index }).ToDictionary(i => i.index, i => i.value);
            Dictionary<int, string> found_itemprops_tr = new Dictionary<int, string>(f_tr);
            f_tr1.ForEach(x => { if (!found_itemprops_tr.ContainsKey(x.Key)) found_itemprops_tr.Add(x.Key, x.Value); else found_itemprops_tr[x.Key] = x.Value; });
            f_tr2.ForEach(x => { if (!found_itemprops_tr.ContainsKey(x.Key)) found_itemprops_tr.Add(x.Key, x.Value); else found_itemprops_tr[x.Key] = x.Value; });
            f_tr3.ForEach(x => { if (!found_itemprops_tr.ContainsKey(x.Key)) found_itemprops_tr.Add(x.Key, x.Value); else found_itemprops_tr[x.Key] = x.Value; });
            f_tr4.ForEach(x => { if (!found_itemprops_tr.ContainsKey(x.Key)) found_itemprops_tr.Add(x.Key, x.Value); else found_itemprops_tr[x.Key] = x.Value; });
            return found_itemprops_tr.OrderBy(key => key.Key).ToDictionary(i => i.Key, i => i.Value);
        }

        private void GetAddReferences(Division div, IHtmlDocument doc)
        {
            var addRefs = doc.QuerySelectorAll("[itemprop=\"addRef\"]").Select(a => a.InnerHtml).ToList();
            var hrefs = new List<string>();
            foreach (var r in addRefs)
            {
                var href = r.IndexOf("href=\"") + ("href=\"").Length;
                string withoutHref = r.Remove(0, href);
                var end = withoutHref.IndexOf("\"");
                withoutHref = withoutHref.Remove(end);
                div.AddRef(withoutHref);
            }
        }

        private string GetCode(Uri url)
        {
            string data = "";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            Cookie cookie = new Cookie
            {
                Name = "beget",
                Value = "begetok"
            };
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.CookieContainer.Add(url, cookie);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = httpWebResponse.GetResponseStream();
                StreamReader readStream = null;
                if (httpWebResponse.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(httpWebResponse.CharacterSet));
                }
                data = readStream.ReadToEnd();
                httpWebResponse.Close();
                readStream.Close();
            }
            return data;
        }
    }
}