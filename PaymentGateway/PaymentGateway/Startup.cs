using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentGateway.Model;
using PaymentGateway.Model.Adapters;
using PaymentGateway.Model.DTO;
using PaymentGateway.Validators;

namespace PaymentGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PaymentGatewayContext>(opt =>
               opt.UseInMemoryDatabase("PaymentGateway"));
            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc().AddFluentValidation();

            services.AddTransient<IAuthRequestAdapter, AuthRequestAdapter>();
            services.AddTransient<IValidator<AuthoriseRequestDto>, AuthoriseRequestValidator>();
            services.AddTransient<IValidator<TransactionItemDto>, TransactionItemValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
