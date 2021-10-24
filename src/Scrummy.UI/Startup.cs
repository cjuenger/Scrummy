using Autofac;
using AutoMapper;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Client;
using IO.Juenger.GitLab.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrummy.GitLab;
using Scrummy.Scrum;

namespace Scrummy.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            
            var config = new Configuration
            {
                BasePath = "https://gitlab.com/api",
                AccessToken = "67QbbetYYda4fSujaLfF"
            };
            
            services.AddSingleton<IProjectApi>(new ProjectApi(config));

            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Issue, Issue>());
            var mapper = new Mapper(mapperConfig);
            services.AddSingleton<IMapper>(mapper);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<ScrumModule>();
            builder.RegisterModule<GitLabModule>();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}