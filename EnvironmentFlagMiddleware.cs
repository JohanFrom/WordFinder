namespace WordFinder
{
    public class EnvironmentFlagMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;

        public EnvironmentFlagMiddleware(RequestDelegate next, IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        } 
        
        public async Task InvokeAsync(HttpContext context)
        {
            if (_environment.EnvironmentName == "Production")
                throw new InvalidOperationException("We don't want to be in prod right know");

            await _next(context);
        }
    }

    public static class EnvironmentFlagMiddlewareExtensions
    {
        public static IApplicationBuilder UseProductionEnvironmentFlag(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnvironmentFlagMiddleware>();
        }
    }
}
