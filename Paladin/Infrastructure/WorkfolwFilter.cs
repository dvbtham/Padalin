using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Paladin.Models;

namespace Paladin.Infrastructure
{
    public class WorkfolwFilter : FilterAttribute, IActionFilter
    {
        private int _highestCompletedStage;
        public int MinRequiredStage { get; set; }
        public int CurrentStage { get; set; }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var applicantId = filterContext.HttpContext.Session["Tracker"];
            if (applicantId != null)
            {
                Guid tracker;
                if (Guid.TryParse(applicantId.ToString(), out tracker))
                {
                    var context = DependencyResolver.Current.GetService<PaladinDbContext>();
                    _highestCompletedStage = context.Applicants
                        .FirstOrDefault(x => x.ApplicantTracker == tracker)
                        .WorkFlowStage;

                    if (MinRequiredStage > _highestCompletedStage)
                    {
                        switch (_highestCompletedStage)
                        {
                            case (int)WorkflowValues.ApplicantInfo:
                                filterContext.Result = GenerateRedirectUrl("ApplicantInfo", "Applicant");
                                break;

                            case (int)WorkflowValues.AddressInfo:
                                filterContext.Result = GenerateRedirectUrl("AddressInfo", "Address");
                                break;

                            case (int)WorkflowValues.EmploymentInfo:
                                filterContext.Result = GenerateRedirectUrl("EmploymentInfo", "Employment");
                                break;

                            case (int)WorkflowValues.VehicleInfo:
                                filterContext.Result = GenerateRedirectUrl("VehicleInfo", "Vehicle");
                                break;

                            case (int)WorkflowValues.Products:
                                filterContext.Result = GenerateRedirectUrl("ProductInfo", "Products");
                                break;
                        }
                    }
                }
            }
            else
            {
                if (CurrentStage != (int)WorkflowValues.ApplicantInfo)
                {
                    filterContext.Result = GenerateRedirectUrl("ApplicantInfo", "Applicant");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var context = DependencyResolver.Current.GetService<PaladinDbContext>();

            var sessionId = filterContext.HttpContext.Session["Tracker"];

            if (sessionId != null)
            {
                Guid tracker;
                if (Guid.TryParse(sessionId.ToString(), out tracker))
                {
                    if (filterContext.HttpContext.Request.RequestType == "POST" &&
                        CurrentStage >= _highestCompletedStage)
                    {
                        var applicant = context.Applicants.SingleOrDefault(x => x.ApplicantTracker == tracker);
                        if (applicant != null) applicant.WorkFlowStage = CurrentStage;
                        context.SaveChanges();
                    }
                }
            }
        }

        private RedirectToRouteResult GenerateRedirectUrl(string action, string controller)
        {
            return new RedirectToRouteResult(new RouteValueDictionary(new { action = action, controller = controller }));
        }
    }
}