using SiteInfoMonitoring.Jobs;
using SiteInfoMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SiteInfoMonitoring.Core
{
    public static class EmailCreater
    {
        public static void CreateInfoEmails(List<User> Users, List<Division> Divisions, bool auto = false)
        {
            bool needSendToAdmins = false;
            var adminProblems = new List<UserProblems>();
            if (Settings.SettingsManager.Settings.AutoSendEmailsToAdmin || auto)
            {
                needSendToAdmins = true;
            }
            foreach (var div in Divisions)
            {
                UserProblems adminProblem = new UserProblems() { Division = div };
                var user = div.ResponsibleUser != null ? Users.First(u => u.Login == div.ResponsibleUser.Login) : null;
                if (user != null)
                {
                    user.Problems = new List<UserProblems>();
                    var problem = new UserProblems() { Division = div };
                    if (!div.IsExist)
                    {
                        problem.Items.Add("Раздел отсутствует.");
                        user.Problems.Add(problem);
                        if (needSendToAdmins)
                        {
                            adminProblems.Add(problem);
                        }
                        continue;
                    }
                }
                foreach (var data in div.Data)
                {
                    if (!data.IsExist && data.Type != Enums.ItempropTypeEnum.Optional)
                    {
                        var dataUser = data.ResponsibleUser != null ? Users.First(u => u.Login == data.ResponsibleUser.Login) : user;
                        if (dataUser != null)
                        {
                            var dataProblem = dataUser.Problems.Any(p => p.Division == div) ? dataUser.Problems.First(p => p.Division == div) : new UserProblems() { Division = div };
                            dataProblem.Items.Add("Атрибут \"" + data.Value + "\" (" + data.Description + ") не найден");
                            dataUser.Problems.Add(dataProblem);
                        }
                        if (needSendToAdmins)
                        {
                            adminProblem.Items.Add("Атрибут \"" + data.Value + "\" (" + data.Description + ") не найден");
                        }
                    }
                }
                foreach (var tbl in div.Tables)
                {
                    var tableUser = tbl.ResponsibleUser != null ? Users.First(u => u.Login == tbl.ResponsibleUser.Login) : user;
                    if (tableUser != null)
                    {
                        var tableProblem = tableUser.Problems.Any(p => p.Division == div) ? tableUser.Problems.First(p => p.Division == div) : new UserProblems() { Division = div };
                        if (!tbl.IsOk)
                        {
                            tableProblem.Items.Add("Таблица \"" + tbl.Name + "\":");
                            tableProblem.Items.Add("Данная таблица не соответствует методическим рекомендациям");
                            tableUser.Problems.Add(tableProblem);
                            if (needSendToAdmins)
                            {
                                adminProblem.Items.Add("Таблица \"" + tbl.Name + "\":");
                                adminProblem.Items.Add("Данная таблица не соответствует методическим рекомендациям");
                            }
                            continue;
                        }
                        foreach (var tip in tbl.TableItemprops)
                        {
                            if (tip.Count == tbl.RowCount && tip.Count != 0 ||
                                tip.Type == Enums.ItempropTypeEnum.Optional ||
                                tbl.Type == Enums.TableTypeEnum.Optional ||
                                tip.Type == Enums.ItempropTypeEnum.Selective && tbl.TableItemprops.Where(t => t.Type == Enums.ItempropTypeEnum.Selective).Sum(t => t.Count) == tbl.RowCount && tip.Count != 0)
                            {
                                //Всё хорошо
                            }
                            else
                            {
                                tableProblem.Items.Add("Таблица \"" + tbl.Name + "\":");
                                tableProblem.Items.Add("Данная таблица не соответствует методическим рекомендациям");
                                tableUser.Problems.Add(tableProblem);
                                if (needSendToAdmins)
                                {
                                    adminProblem.Items.Add("Таблица \"" + tbl.Name + "\":");
                                    adminProblem.Items.Add("Данная таблица не соответствует методическим рекомендациям");
                                }
                                break;
                            }
                        }
                    }
                    else if (needSendToAdmins)
                    {
                        if (!tbl.IsOk)
                        {
                            adminProblem.Items.Add("Таблица \"" + tbl.Name + "\":");
                            adminProblem.Items.Add("Данная таблица не соответствует методическим рекомендациям");
                            continue;
                        }
                        foreach (var tip in tbl.TableItemprops)
                        {
                            if (tip.Count == tbl.RowCount && tip.Count != 0 ||
                                tip.Type == Enums.ItempropTypeEnum.Optional ||
                                tbl.Type == Enums.TableTypeEnum.Optional ||
                                tip.Type == Enums.ItempropTypeEnum.Selective && tbl.TableItemprops.Where(t => t.Type == Enums.ItempropTypeEnum.Selective).Sum(t => t.Count) == tbl.RowCount && tip.Count != 0)
                            {
                                //Всё хорошо
                            }
                            else
                            {
                                adminProblem.Items.Add("Таблица \"" + tbl.Name + "\":");
                                adminProblem.Items.Add("Данная таблица не соответствует методическим рекомендациям");
                                break;
                            }
                        }
                    }
                }
                if (needSendToAdmins && adminProblem.Items.Count > 0)
                {
                    adminProblems.Add(adminProblem);
                }
            }
            foreach (var usr in Users)
            {
                string mssg = "";
                string adminMssg = "";
                foreach (var problem in usr.Problems)
                {
                    mssg += "\nРаздел \"" + problem.Division.Description + "\": \n";
                    mssg += String.Join("\n", problem.Items);
                }
                foreach (var problem in adminProblems)
                {
                    adminMssg += "\nРаздел \"" + problem.Division.Description + "\": \n";
                    adminMssg += String.Join("\n", problem.Items);
                }
                if (mssg != "" && usr.Role == Enums.RolesEnum.user)
                {
                    new Thread(t => EmailSender.Send(usr.Email, mssg)).Start();
                }
                if (adminMssg != "" && usr.Role == Enums.RolesEnum.admin)
                {
                    new Thread(t => EmailSender.Send(usr.Email, adminMssg)).Start();
                }
            }
        }
    }
}