using Yoda.Application;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using System.Collections.Generic;
using Yoda.Interfaces;
using Microsoft.Extensions.Hosting;
using System.IO;
using YodaApp.Yoda.Interfaces;
using YodaApp.Yoda.Application.Notifications;
using YodaApp.Yoda.Application.Avatars;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using YodaCorePlugin.AuthorizeRegistration.Menus;
using Microsoft.Extensions.Configuration;
using YodaApp.UsersResources;
using TradeResourcesPlugin;
using TradeResourcesPlugin.Modules.Administration;
using Analytics;
using TradeResourcesPlugin.Helpers;

namespace Yoda {
    public class Startup {
        public static void Main(string[] args) =>
            BuildWebHost(args)
                .Build()
                .Run();

        public static IHostBuilder BuildWebHost(string[] args) {
            YodaUiConfig.UiVersion = UiVersion.V2;
            return Host
                .CreateDefaultBuilder()
                .PreferTls12()
                .UseRuCultureAsDefault()
                .ConfigureAppConfiguration((context, configuration) => {
                    configuration.UseYodaDefaultConfigurations(context.HostingEnvironment.EnvironmentName, args);
                })
                .ConfigureServices((context, services) => {
                    services.UseYodaServices(
                           context.Configuration,
                           Directory.GetCurrentDirectory(),
                           plugins => plugins
                               .Add(Assembly.GetExecutingAssembly())
                               .Add(typeof(YodaCorePlugin.Entry).Assembly)
                       );

                    services.AddNotifications(context.Configuration);
                    services.AddAvatars(context.Configuration);

                    services.AddEmailSender(options => {
                        options.SmtpClient = new SmtpClientConfig
                        {
                            Server = context.Configuration["emailSettings:server"],
                            User = context.Configuration["emailSettings:userName"],
                            Secret = context.Configuration["emailSettings:secret"],
                            EnableSsl = bool.Parse(context.Configuration["emailSettings:enableSsl"]),
                            Port = int.Parse(context.Configuration["emailSettings:port"])
                        };
                        options.EmailFromText = "АО \"Информационно-учетный центр\"";
                        options.EmailFromTextInjectedIntoSubject = "Traderesources";
                    });

                    services.Configure<MapsyOptions>(options => {
                        options.ApiUrl = context.Configuration["mapsyApi:url"];
                        options.AuthorityUrl = context.Configuration["mapsyApi:authorityUrl"];
                        options.ClientId = context.Configuration["mapsyApi:client:clientId"];
                        options.Secret = context.Configuration["mapsyApi:client:pwd"];
                    });

                    services.AddSingleton<IMapsyClient, MapsyClient>();


                    services.AddSingleton<IAccountActivityTypesProvider, AccountActivityTypesProvider>();

                    services.Configure<TreasuryPaymentsApiClientConfig>(option => {
                        option.AuthorityUrl = context.Configuration["treasuryPaymentsApi:authorityUrl"];
                        option.ApiUrl = context.Configuration["treasuryPaymentsApi:url"];
                        option.ClientId = context.Configuration["treasuryPaymentsApi:client:clientId"];
                        option.Secret = context.Configuration["treasuryPaymentsApi:client:pwd"];
                    });
                    services.AddSingleton<ITreasuryPaymentsClientFactory, TreasuryPaymentsClientFactory>();

                    services.Configure<UserContextMenuItemsConfiguration>(context.Configuration);
                    services.AddSingleton<IUserContextMenuItems, DefaultUserContextMenuItems>();
                    var hookUrls = context.Configuration.GetSection("accountAndUserChangeHookUrls").Get<string[]>();
                    services.AddAccountOrUserChangeEventHooksTriggerer(hookUrls);

                    services
                        .AddHealthChecks()
                        .AddQueryExecuter(context.Configuration)
                        .AddCertCheckers(context.Configuration)
                        .AddSignVerifiers(context.Configuration)

                        .AddUrlGroup(new Uri(context.Configuration["notificationsApi:Url"] + "/swagger/v1/swagger.json"), "NotificationsApi")
                        .AddUrlGroup(new Uri(context.Configuration["avatarApi:Url"] + "/swagger/v1/swagger.json"), "AvatarApi");
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.Configure(
                        appBuilder => appBuilder.UseYodaWebAppOrGenMigration()
                    );
                });
        }
    }


    public class HomeController : Application.HomeController {
        public override string[] GetInitialRequireUiPackages(IYodaRequestContext requestContext) {
            return new List<string>(base.GetInitialRequireUiPackages(requestContext)) {
                "jquery.core",
                "bootstrap",
                "bootstrap-modal-facade",
                "hyper-theme.structure",
                "hyper-theme.fonts.museo-sans-cyrl",
                "hyper-theme.components",
                "hyper-theme.icons.dripicons",
                "hyper-theme.icons.materialdesignicons",
                "hyper-theme.icons.unicons",
                "hyper-theme.pages",
                "hyper-theme.metis-menu",
                "hyper-theme.app",
                "editors.filter",
                "editors.reference",
                "editors.date",
                "project-theme",
                "hyper-fix",
                "simplebar",
                "yodic",
                "autolock-submit-button"
            }.ToArray();
        }
    }

    public class DefaultUserContextMenuItems : IUserContextMenuItems {
        private readonly UserContextMenuItemsConfiguration _contextMenuItems;
        public DefaultUserContextMenuItems(IOptions<UserContextMenuItemsConfiguration> contextMenuItems)
        {
            _contextMenuItems = contextMenuItems.Value;
        }

        public ContextMenuItem[] GetUserContextMenuItems(IYodaRequestContext context)
        {
            return _contextMenuItems.UserContextMenus;
        }
    }

    public interface IUserContextMenuItems {
        public ContextMenuItem[] GetUserContextMenuItems(IYodaRequestContext context);
    }
    public class UserContextMenuItemsConfiguration {
        public ContextMenuItem[] UserContextMenus { get; set; }
    }
    public class ContextMenuItem {
        public ContextMenuItem() { }
        public ContextMenuItem(string text, string url, string iconClassName)
        {
            Text = text;
            Url = url;
            IconClassName = iconClassName;
        }

        public string Text { get; set; }
        public string Url { get; set; }
        public string IconClassName { get; set; }
    }


}