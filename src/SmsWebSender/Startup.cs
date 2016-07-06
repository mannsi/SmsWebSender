
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmsWebSender.Models;
using SmsWebSender.ServiceInterfaces;
using SmsWebSender.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmsWebSender
{
    public class Startup
    {
        private bool _isDevelopment = false;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables(); ;

            if (env.IsDevelopment())
            {
                _isDevelopment = true;
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "";
            if (_isDevelopment)
            {
                connectionString = Configuration["Data:DefaultConnection:SmsSenderDebugConnectionString"];
            }
            else
            {
                connectionString = Configuration["Data:DefaultConnection:SmsSenderProductionConnectionString"];
            }

            // Add framework services.

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequiredLength = 6;
                    o.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddSingleton(Configuration);
            services.AddTransient<IAppointmentService, AppointmentService>();
            services.AddTransient<ISmsService, SmsService>();
            services.AddTransient<IEmailService, EmailService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            ISmsService smsService,
            IAppointmentService appointmentService,
            IEmailService emailService,
            ApplicationDbContext dbcontext,
            IConfiguration configuration)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            //app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());
            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=StartPage}/{action=LandingPage}/{id?}");
            });
            
            Jobs.Sms.JobStart.Start(smsService, appointmentService, emailService, dbcontext, configuration);
            await InitialData.InitializeUser(app.ApplicationServices, Configuration["defaultUserPassword"]);
        }

    }
}
