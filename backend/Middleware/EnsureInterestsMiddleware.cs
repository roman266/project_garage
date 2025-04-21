using project_garage.Bogus;

namespace project_garage.Middleware
{
    public class EnsureInterestsMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _next;

        public EnsureInterestsMiddleware(
            IConfiguration configuration, 
            IServiceProvider serviceProvider, 
            RequestDelegate requestDelegate) 
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _next = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            DbSeeder.EnsureInterestsConsistent(_serviceProvider, _configuration);
            await _next(context);
        }
    }
}
