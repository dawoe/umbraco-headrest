﻿using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Our.Umbraco.HeadRest.Web.Mvc;
using Our.Umbraco.HeadRest.Web.Mapping;
using Our.Umbraco.HeadRest.Interfaces;
using Our.Umbraco.HeadRest.Web.Models;

namespace Our.Umbraco.HeadRest.Web.Controllers
{
    [HeadRestExceptionFilter]
    public class HeadRestController : RenderMvcController
    {
        internal IHeadRestConfig Config
        {
            get
            {
                return RouteData.Values[HeadRest.ControllerConfigKey] as IHeadRestConfig;
            }
        }

        public override ActionResult Index(RenderModel model)
        {
            // Check for 404
            if (model.Content is NotFoundPublishedContent)
            {
                Response.StatusCode = 404;
          
                return new HeadRestResult
                {
                    Data = new { message = "404 Not Found" }
                };
            }

            // Process the model mapping request
            var contentTypeAlias = model.Content.DocumentTypeAlias;
            var viewModelType = Config.ViewModelMappings.GetViewModelTypeFor(contentTypeAlias, new HeadRestPreMappingContext
            {
                Request = Request,
                HttpContext = HttpContext,
                UmbracoContext = UmbracoContext
            });

            var viewModel = Config.Mapper.Invoke(new HeadRestMappingContext
            {
                Content = model.Content,
                ContentType = model.Content.GetType(),
                ViewModelType = viewModelType,
                Request = Request,
                HttpContext = HttpContext,
                UmbracoContext = UmbracoContext
            });

            return new HeadRestResult
            {
                Data = viewModel
            };
        }
    }
}
