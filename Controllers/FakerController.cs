using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Bogus.Web.Controllers
{
    public class FakerController : Controller
    {
        private static readonly object SyncLock = new object();

        public IActionResult Index()
        {
            var routes = RouteData.Routers.OfType<RouteCollection>().First();
            var routeTemplates = new List<string>();

            var host = Request.GetDisplayUrl();

            for (var i = 0; i < routes.Count; i++)
            {
                if (!(routes[i] is Route route)) continue;
                routeTemplates.Add(host + route.RouteTemplate);
            }

            return Json(
                new { routes = routeTemplates },
                new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet]
        public IActionResult Generate(int? seed, string locale = "en")
        {
            object result;

            object InvokeMethod()
            {
                var property = (PropertyInfo)ControllerContext.RouteData.DataTokens["property"];
                var method = (MethodInfo)ControllerContext.RouteData.DataTokens["method"];
                var parameterValues = ResolveParameters(method, ControllerContext.RouteData,
                    ControllerContext.HttpContext.Request.Query);
                var faker = new Faker(locale);
                var propertyValue = property.GetMethod.Invoke(faker, null);
                return method.Invoke(propertyValue, parameterValues);
            }

            lock (SyncLock)
            {
                if (seed.HasValue)
                {
                    var oldSeed = Randomizer.Seed;
                    var newSeed = new Random(seed.Value);

                    Randomizer.Seed = newSeed;
                    result = InvokeMethod();
                    Randomizer.Seed = oldSeed;
                }
                else
                {
                    result = InvokeMethod();
                }
            }

            return Json(result);
        }

        private static object[] ResolveParameters(MethodBase method, RouteData routeData, IQueryCollection query)
        {
            return method.GetParameters().Select(parameter =>
                {
                    var parameterValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                    object value = null;

                    if (routeData.Values.ContainsKey(parameter.Name))
                        value = routeData.Values[parameter.Name];
                    else if (query.ContainsKey(parameter.Name))
                        value = query[parameter.Name][0];

                    if (value != null)
                        parameterValue = TypeDescriptor.GetConverter(parameter.ParameterType).ConvertFrom(value);

                    return parameterValue;
                })
                .ToArray();
        }
    }
}