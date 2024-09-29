using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRHandler
{
    public static class MediatRDependencyHandler
    {
        public static IServiceCollection RegisterRequestHandlers(
            this IServiceCollection services)
        {
           return  services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(MediatRDependencyHandler).Assembly));
        } 
    }
}
