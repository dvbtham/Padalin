using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Paladin.Infrastructure
{
    public class XmlResult : ActionResult
    {
        private readonly object _data;
        public XmlResult(object data)
        {
            _data = data;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var serializer = new XmlSerializer(_data.GetType());
            var response = HttpContext.Current.Response;
            response.ContentType = "text/xml";
            serializer.Serialize(response.Output, _data);
        }
    }
    
}