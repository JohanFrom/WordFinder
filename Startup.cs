using Microsoft.OpenApi.Models;
using System.Reflection;
using WordFinder.Services;
using WordFinder.Services.Abstract;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

namespace WordFinder
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client/build";
            });
            //services.AddSpaStaticFiles(configuration: options => { options.RootPath = "wwwroot"; });


            services.AddScoped<IWordFinderService, WordFinderService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WordFinder API",
                    Version = "v1",
                    Description = "More info here",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "More Info Here",
                        Email = "Mail",
                        Url = new Uri("https://www.noroff.no/accelerate"),

                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddHttpContextAccessor();

            string test = Configuration.GetValue<string>("BaseUrl");

            services.AddHttpClient("wordfinder-client", c =>
            {
                c.BaseAddress = new Uri($"{Configuration.GetValue<string>("BaseUrl")}");
            });

            //services.AddHttpClient("word-finder", c =>
            //{
            //    c.BaseAddress = new Uri($"{Configuration.GetValue<string>("BaseUrl")}");
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string? rootDir = Configuration.GetValue<string>("ApplicationBasePath");
            app.UsePathBase(rootDir);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WordFinder API v1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseProductionEnvironmentFlag();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

        }
    }
}
