using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bogus.Web
{
    public static class FakerRoutes
    {
        public static void Configure(IRouteBuilder routes)
        {
            var fakerType = typeof(Faker);

            var properties = fakerType.GetProperties(
                    BindingFlags.GetProperty
                    | BindingFlags.Public
                    | BindingFlags.Instance)
                .Where(p => typeof(DataSet).IsAssignableFrom(p.PropertyType));

            foreach (var property in properties)
            {
                var methods = property.PropertyType
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.DeclaringType == property.PropertyType)
                    .Where(m => !m.IsSpecialName);

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();

                    var template = $"{property.Name}/{method.Name}";
                    if (parameters.Any())
                    {
                        if (parameters.Any(p => !p.ParameterType.IsValueType && p.ParameterType != typeof(string)))
                            continue;

                        template +=
                            $"/{string.Join("/", parameters.Select(p => $"{{{p.Name}{(p.IsOptional ? "?" : "")}}}"))}";
                    }

                    routes.MapRoute(
                        template,
                        template,
                        new { controller = "Faker", action = "Generate" }, 
                        null,
                        new { property, method });
                }
            }
        }
    }
}