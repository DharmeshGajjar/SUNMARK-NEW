using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SUNMark.Models;

namespace SUNMark.Classes
{
    public static class CommonHelpers
    {
        public static string CheckDotData(string data)
        {
            string returnValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(data))
            {
                string[] spliited = data.Split('.');
                if (spliited != null && spliited.Length == 1)
                {
                    returnValue = data;
                }
                else
                {
                    if (Convert.ToInt32(spliited[1]) > 0)
                    {
                        returnValue = data;
                    }
                    else
                    {
                        returnValue = spliited[0];
                    }
                }
            }
            return returnValue;
        }

        public static List<Select2Model> BindSelect2Model(List<CustomDropDown> list)
        {
            List<Select2Model> data = new List<Select2Model>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    data.Add(new Select2Model
                    {
                        Id = item.Value!=null ? Convert.ToInt32(item.Value) : 0,
                        Text = Convert.ToString(item.Text)
                    });
                }
            }
            return data;
        }

        public static List<Select2Model> BindSelect2Model(List<SelectListItem> list)
        {
            List<Select2Model> data = new List<Select2Model>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    data.Add(new Select2Model
                    {
                        Id = item.Value != null ? Convert.ToInt32(item.Value) : 0,
                        Text = Convert.ToString(item.Text)
                    });
                }
            }
            return data;
        }

        public static CustomDropDown GetDefaultValue()
        {
            return new CustomDropDown
            {
                Text = "---Select---"
            };
        }

        public static SelectListItem GetDefaultValueSelect()
        {
            return new SelectListItem
            {
                Text = "---Select---"
            };
        }
    }


    public static class ListtoDataTableConverter
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
