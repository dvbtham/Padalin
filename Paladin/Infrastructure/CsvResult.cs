using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Paladin.Infrastructure
{
    public class CsvResult : FileResult
    {
        private readonly IEnumerable _data;
        public CsvResult(IEnumerable data, string fileName) : base("text/csv")
        {
            _data = data;
            FileDownloadName = fileName;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            var builder = new StringBuilder();
            var stringWriter = new StringWriter(builder);
            foreach (var item in _data)
            {
                var properties = item.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    stringWriter.Write(GetValue(item, prop.Name));
                    stringWriter.Write(", ");
                }
                stringWriter.WriteLine();
            }
            response.Write(builder);
        }

        private static string GetValue(object item, string propName)
        {
            return item.GetType().GetProperty(propName).GetValue(item, null).ToString() ?? "";
        }
    }
}