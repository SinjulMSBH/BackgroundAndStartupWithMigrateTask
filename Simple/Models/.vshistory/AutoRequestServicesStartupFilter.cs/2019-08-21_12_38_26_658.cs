using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using System;

namespace Simple.Models
{
    public class AutoRequestServicesStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
            builder =>
                //builder.UseMiddleware<RequestServicesContainerMiddleware>();
                next(builder);
    };
}
}
