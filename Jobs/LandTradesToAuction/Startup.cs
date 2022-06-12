using System.Net;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Yoda.Application;
using YodaApp.CommonWorkers;

namespace LandTradesToAuction {
    public class Startup {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public const int Minute = 60;
        public const int Hour = 60 * Minute;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddQueryExecuter(Configuration);
            services.AddMemoryCache();

            services
                .AddSingleton<NamedTimerHealthCheckWitness>()
                .AddHostedService<NamedTimersRunnerWorker>()
                .AddHttpClient()
                .Configure<LandTradesToAuctionJobsConfig>(options =>
                 {
                     options.AuctionIntegrationServiceUrl = Configuration["AuctionIntegrationServiceUrl"];
                 })
                .AddLandTradesToAuctionJobsConsumer()
                .AddWaitingLandTradesFromAuctionJobsConsumer()
                .AddHeldLandTradesFromAuctionJobsConsumer()
                .AddLandAgreementsToAuctionJobsConsumer()
                .AddLandEgknIdToAuctionJobsConsumer()
                .AddHealthChecks()
                .AddQueryExecuter(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/healthz-check-ui-endpoint", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksPrometheusExporter(
                "/healthz-check-prometheus-endpoint",
                options => options.ResultStatusCodes[HealthStatus.Unhealthy] = (int)HttpStatusCode.OK
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
    public class LandTradesToAuctionJobsConfig {
        public string AuctionIntegrationServiceUrl { get; set; }
    }
}
