using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using DigitalLibrary.API.Localization;
using DigitalLibrary.API.Services.FileManager;
using DigitalLibrary.API.Settings;
using DigitalLibrary.Data;
using DigitalLibrary.Data.Repositories;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.Enums;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IMailService = DigitalLibrary.API.Services.MailService.IMailService;
using MailService = DigitalLibrary.API.Services.MailService.MailService;

namespace DigitalLibrary.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment _env { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _env = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(o =>
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddRazorRuntimeCompilation()
                .AddMvcLocalization(
                    options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(DataAnnotationsResource));
                    });


            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("AppDb")
            );


            services.AddLocalization(options => options.ResourcesPath = "Localization/Resources");

            AddIdentityServer4WithAspNetIdentity(services);

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            services.AddTransient<IMailService, MailService>();

            services.ConfigureRepositoryWrapper();

            services.AddTransient<FileManager>();

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://localhost:5001",
                            "https://localhost:5003"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .WithExposedHeaders("X-Pagination");
                });
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            InitializeIdentityServerConfigurationInDb(app);
            InitializeUsers(app);
            InitializeAppDb(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            var supportedCultures = new[] {"en", "ru", "kk"};
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[1])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);
            app.UseRouting();
            app.UseCors("default");
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }

        private void AddIdentityServer4WithAspNetIdentity(IServiceCollection services)
        {
            var authConnectionString = Configuration.GetConnectionString("AuthConnection");

            // inject DbContext for identity

            services.AddDbContext<AppIdentityContext>(options =>
                options.UseInMemoryDatabase("IdentityDb")
            );


            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password = new PasswordOptions
                    {
                        RequireDigit = false,
                        RequiredLength = 4,
                        RequireLowercase = false,
                        RequireUppercase = false,
                        RequiredUniqueChars = 0,
                        RequireNonAlphanumeric = false
                    };
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<AppIdentityContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<MultilanguageIdentityErrorDescriber>(); // custom Error Describer for culture support

            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/Account/Logout";
            });

            var builder = services.AddIdentityServer();

            // connect IdentityServer4 to AspNetIdentity before configuring IdentityServer4
            builder.AddAspNetIdentity<IdentityUser>();

            var migrationsAssembly = typeof(AppDbContext).GetTypeInfo().Assembly.GetName().Name;

            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    //b.UseSqlServer(authConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    b.UseInMemoryDatabase("IdentityDb");
            });

            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    //b.UseSqlServer(authConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    b.UseInMemoryDatabase("IdentityDb");
            });

            // if environment is development developer signing credentials
            if (_env.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                builder.AddDeveloperSigningCredential();
            }

            services.AddLocalApiAuthentication();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(DigitalLibraryConstants.Policies.Administration, policyBuilder =>
                {
                    var policy = options.GetPolicy(IdentityServerConstants.LocalApi.PolicyName);

                    policyBuilder.RequireClaim(
                        DigitalLibraryConstants.Claims.Role, new[]
                        {
                            DigitalLibraryConstants.Roles.Admin
                        }
                    );

                    policyBuilder.Combine(policy);
                });

                options.AddPolicy(DigitalLibraryConstants.Policies.Moderation, policyBuilder =>
                {
                    var policy = options.GetPolicy(IdentityServerConstants.LocalApi.PolicyName);

                    policyBuilder.RequireClaim(
                        DigitalLibraryConstants.Claims.Role, new[]
                        {
                            DigitalLibraryConstants.Roles.Moder,
                            DigitalLibraryConstants.Roles.Admin
                        }
                    );

                    policyBuilder.Combine(policy);
                });
            });
        }

        private static void InitializeIdentityServerConfigurationInDb(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var persistedContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var configContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (persistedContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    persistedContext.Database.Migrate();
                    configContext.Database.Migrate();
                }

                if (configContext.Clients.Any() == false)
                {
                    foreach (var client in IdentityServerConfiguration.Clients)
                    {
                        configContext.Clients.Add(client.ToEntity());
                    }

                    configContext.SaveChanges();
                }

                if (configContext.IdentityResources.Any() == false)
                {
                    foreach (var identityResource in IdentityServerConfiguration.IdentityResources)
                    {
                        configContext.IdentityResources.Add(identityResource.ToEntity());
                    }

                    configContext.SaveChanges();
                }

                if (configContext.ApiScopes.Any() == false)
                {
                    foreach (var apiScope in IdentityServerConfiguration.ApiScopes)
                    {
                        configContext.ApiScopes.Add(apiScope.ToEntity());
                    }

                    configContext.SaveChanges();
                }
            }
        }

        private static void InitializeUsers(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Create initial users if users is empty

                if (userManager.Users.Any() == false)
                {
                    var admin = new IdentityUser("admin")
                    {
                        Email = "sprv.adilet@gmail.com", Id = "b4c550a3-e004-47b4-a949-4e00b2f802c4",
                        EmailConfirmed = true
                    };
                    userManager.CreateAsync(admin, "admin").GetAwaiter().GetResult();
                    userManager.AddClaimAsync(admin,
                            new Claim(DigitalLibraryConstants.Claims.Role, DigitalLibraryConstants.Roles.Admin))
                        .GetAwaiter().GetResult();

                    var moder = new IdentityUser("moder")
                    {
                        Email = "moder@gmail.com", Id = "609c9cfe-3d55-4765-ad3d-f63a576310cf", EmailConfirmed = true
                    };
                    userManager.CreateAsync(moder, "moder").GetAwaiter().GetResult();
                    userManager.AddClaimAsync(moder,
                            new Claim(DigitalLibraryConstants.Claims.Role, DigitalLibraryConstants.Roles.Moder))
                        .GetAwaiter().GetResult();

                    var user = new IdentityUser("user")
                    {
                        Email = "user@gmail.com", Id = "291a0446-09e2-4b88-9952-f40dcb7d9606", EmailConfirmed = true
                    };
                    userManager.CreateAsync(user, "user").GetAwaiter().GetResult();
                }
            }
        }

        private static void InitializeAppDb(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!dbContext.Genres.Any())
                {
                    dbContext.Genres.AddRange(
                        new Genre {Id = new Guid("A6B4F436-F2D0-4B8D-31A0-08D8F82B18A8"), Title = "Poetry"},
                        new Genre {Id = new Guid("3B78155B-D825-4E5A-31A1-08D8F82B18A8"), Title = "Memoir"},
                        new Genre {Id = new Guid("82F159F1-A044-46B4-31A2-08D8F82B18A8"), Title = "True crime"},
                        new Genre {Id = new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8"), Title = "Science"},
                        new Genre {Id = new Guid("7503BE5B-F88C-49FD-31A4-08D8F82B18A8"), Title = "Suspense"},
                        new Genre {Id = new Guid("432DAE9E-61EB-43CA-31A5-08D8F82B18A8"), Title = "Western"},
                        new Genre {Id = new Guid("1524DD56-710A-45E6-31A6-08D8F82B18A8"), Title = "Horror"},
                        new Genre {Id = new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8"), Title = "Fantasy"},
                        new Genre {Id = new Guid("C0278B99-C1DC-424F-31A8-08D8F82B18A8"), Title = "Drama"},
                        new Genre {Id = new Guid("E1F56D7C-F3B9-47B5-31A9-08D8F82B18A8"), Title = "Classic"},
                        new Genre {Id = new Guid("769C4769-3416-4FC1-31AA-08D8F82B18A8"), Title = "Dark Fantasy"}
                    );
                }

                if (!dbContext.Subjects.Any())
                {
                    dbContext.Subjects.AddRange(
                        new Subject
                        {
                            Id = new Guid("28BB4861-3166-4997-C867-08D8F82B49EA"), Title = "Family and Relationships"
                        },
                        new Subject
                        {
                            Id = new Guid("6C024ABD-9942-4DA3-C869-08D8F82B49EA"), Title = "House & Home books"
                        },
                        new Subject
                        {
                            Id = new Guid("0CFD0C05-7B53-453B-C86A-08D8F82B49EA"), Title = "Foreign Language Study"
                        },
                        new Subject
                        {
                            Id = new Guid("B15507AF-F6E1-4526-C86B-08D8F82B49EA"), Title = "Business & Economics"
                        },
                        new Subject
                        {
                            Id = new Guid("164E6104-D3F9-4947-C86C-08D8F82B49EA"), Title = "Body, Mind & Spirit"
                        },
                        new Subject {Id = new Guid("DA9E1A12-5564-4ADC-C86D-08D8F82B49EA"), Title = "Craft & Hobbies"},
                        new Subject {Id = new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA"), Title = "Fiction"}
                    );
                }

                if (!dbContext.Authors.Any())
                {
                    dbContext.Authors.AddRange(
                        new Author
                        {
                            Id = new Guid("E959400D-FBE3-49B8-A815-08A774043384"), FirstName = "Джоан Кетлин",
                            LastName = "Роулинг", Country = Country.UK
                        },
                        new Author
                        {
                            Id = new Guid("18E16B99-1E0F-48FD-627D-08D8F84F7640"), FirstName = "George",
                            LastName = "Orwell", Country = Country.Canada
                        },
                        new Author
                        {
                            Id = new Guid("742AECEA-866C-4F08-1B35-08D8F850467A"), FirstName = "Анджей",
                            LastName = "Сапковский", Country = Country.Poland
                        },
                        new Author
                        {
                            Id = new Guid("A70959F6-0D42-442D-AE5C-2DCB929FA416"), FirstName = "Джордж",
                            LastName = "Мартин", Country = Country.UK
                        },
                        new Author
                        {
                            Id = new Guid("85E43CFB-5B36-4324-94A4-3DAAEF4A5471"), FirstName = "Эндрю",
                            LastName = "Троелсон", Country = Country.Canada
                        },
                        new Author
                        {
                            Id = new Guid("33D6F681-6EF2-486D-B0FE-41A767065E36"), FirstName = "Исаяма",
                            LastName = "Хаджиме", Country = Country.Italy
                        },
                        new Author
                        {
                            Id = new Guid("314DE2D1-9C84-4A89-97C8-49DF4FA88ABD"), FirstName = "Ричард",
                            LastName = "Фейнман", Country = Country.USA
                        },
                        new Author
                        {
                            Id = new Guid("907580B2-7E31-44FC-9FBE-79728C1F06FC"), FirstName = "Джон",
                            LastName = "Толкин", Country = Country.USA
                        }
                    );
                }

                if (!dbContext.Publishers.Any())
                {
                    dbContext.Publishers.AddRange(
                        new Publisher {Id = new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"), Name = "Bookthesda"}
                    );
                }

                if (!dbContext.Libraries.Any())
                {
                    dbContext.Libraries.AddRange(
                        new Library
                        {
                            Id = new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C"), Address = "ул. Петрова 32",
                            City = City.Almaty, Name = "Книжный мир"
                        },
                        new Library
                        {
                            Id = new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799"), Address = "ул. Независимости 21",
                            City = City.Almaty, Name = "Книжный черьв"
                        }
                    );
                }

                if (!dbContext.Profiles.Any())
                {
                    dbContext.Profiles.AddRange(
                        new Profile
                        {
                            Id = new Guid("B4C550A3-E004-47B4-A949-4E00B2F802C4"),
                            Image = new Guid("00000000-0000-0000-0000-000000000000"), FirstName = "Әділет",
                            LastName = "Сапаров", IIN = "992432156321", Address = "Puskina 11",
                            PhoneNumber = "87051531232", City = City.Almaty,
                            RegisteredLibrary =
                                dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799"))
                        },
                        new Profile
                        {
                            Id = new Guid("291A0446-09E2-4B88-9952-F40DCB7D9606"),
                            Image = new Guid("00000000-0000-0000-0000-000000000000"), FirstName = "Мирхан",
                            LastName = "Кен", IIN = "990924351311", Address = "Abaya 23", PhoneNumber = "87087238011",
                            City = City.Almaty,
                            RegisteredLibrary =
                                dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799"))
                        },
                        new Profile
                        {
                            Id = new Guid("609C9CFE-3D55-4765-AD3D-F63A576310CF"),
                            Image = new Guid("00000000-0000-0000-0000-000000000000"), FirstName = "Moderator",
                            LastName = "Qul", IIN = "339485823213", Address = "Bylba 43", PhoneNumber = "87021448032",
                            City = City.Almaty,
                            RegisteredLibrary =
                                dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799"))
                        }
                    );
                }

                if (!dbContext.Books.Any())
                {
                    dbContext.Books.AddRange(
                        new Book
                        {
                            Id = new Guid("08807358-6D37-4CB2-8E56-08D9011BCB96"),
                            Title = "Гарри Поттер и Дары Смерти",
                            Description =
                                "Книга Джоан Роулинг «Гарри Поттер и Дары смерти» завершает историю о противостоянии Гарри и Волан-де-Морта. Серия книг вызывает восхищение читателей со всего света, по ним были сняты не менее успешные фильмы. Однако герои стали настолько родными, что с ними совершенно не хочется прощаться. Поэтому становится грустно, когда понимаешь, что на этом заканчивается масштабная борьба добра и зла, заканчивается детство. Когда-то раньше поезд с платформы 9 и ¾ отправлял множество учеников в волшебную школу, он открывал удивительный мир, полный магии, где невозможное становилось возможным. Вместе с этими ребятами туда отправлялись и читатели и наслаждались приключениями. Теперь герои стали совсем взрослыми. Они изменились, стали серьёзнее, ушла какая-то детскость из поступков, даже повествования стало более взрослым. И это неудивительно, Гарри и его друзьям предстоит самая сложная битва, и тут нельзя проявлять легкомыслие.",
                            Image = "08807358-6d37-4cb2-8e56-08d9011bcb96",
                            ISBN = "0-8544-3382-1",
                            Pages = 527,
                            Year = 2007,
                            Language = Language.Russian,
                            Epub = "08807358-6d37-4cb2-8e56-08d9011bcb96", Pdf = "08807358-6d37-4cb2-8e56-08d9011bcb96",
                            Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("E959400D-FBE3-49B8-A815-08A774043384")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("53EA8819-3323-42E7-8E57-08D9011BCB96"),
                            Title = "Гарри Поттер и Кубок Огня",
                            Description =
                                "«Испытаний на протяжении этого учебного года будет три, и они позволят проверить способности чемпионов с разных сторон… колдовское мастерство – доблесть – способность к дедукции – и, разумеется, умение достойно встретить опасность». В «Хогварце» проводится Тремудрый Турнир. К участию допускаются только волшебники, достигшие семнадцатилетия, но это не мешает Гарри мечтать о победе. А потом, во время Хэллоуина, когда Кубок Огня делает выбор, Гарри с огромным удивлением узнает, что ему тоже предстоит стать участником состязания. Он столкнется со смертельно опасными заданиями, драконами и темными волшебниками, но с помощью лучших друзей, Рона и Гермионы, возможно, ему удастся преодолеть все препятствия – и остаться в живых!",
                            Image = "53ea8819-3323-42e7-8e57-08d9011bcb96",
                            ISBN = "978-1-78110-271-8",
                            Pages = 660,
                            Year = 2000,
                            Language = Language.Russian,
                            Epub = "53ea8819-3323-42e7-8e57-08d9011bcb96", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("E959400D-FBE3-49B8-A815-08A774043384")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("BED90FA7-C4C1-4A2D-8E58-08D9011BCB96"),
                            Title = "Гарри Поттер и Орден Феникса",
                            Description =
                                @"«Ты воспринимаешь мысли и эмоции Черного Лорда. Директор считает, что это следует прекратить. Он пожелал, чтобы я научил тебя блокировать сознание». В «Хогварце» настали темные времена. После того как дементоры напали на его кузена Дудли, Гарри Поттер знает, что Вольдеморт ни перед чем не остановится, чтобы найти его. Многие отрицают возвращение Черного Лорда, но Гарри не один: в доме на площади Мракэнтлен собирается тайный орден, цель которого – бороться с темными силами. Гарри должен позволить профессору Злею научить его защищаться от яростных атак Вольдеморта на сознание. Но они становятся сильнее день ото дня, и у Гарри остается все меньше времени…",
                            Image = "bed90fa7-c4c1-4a2d-8e58-08d9011bcb96",
                            ISBN = "978-1-78110-297-8",
                            Pages = 860,
                            Year = 2003,
                            Language = Language.Russian,
                            Epub = "bed90fa7-c4c1-4a2d-8e58-08d9011bcb96", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("E959400D-FBE3-49B8-A815-08A774043384")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("2FA60505-26F8-41C0-8E59-08D9011BCB96"),
                            Title = "Гарри Поттер и Принц-Полукровка",
                            Description =
                                @"«Ты воспринимаешь мысли и эмоции Черного Лорда. Директор считает, что это следует прекратить. Он пожелал, чтобы я научил тебя блокировать сознание». В «Хогварце» настали темные времена. После того как дементоры напали на его кузена Дудли, Гарри Поттер знает, что Вольдеморт ни перед чем не остановится, чтобы найти его. Многие отрицают возвращение Черного Лорда, но Гарри не один: в доме на площади Мракэнтлен собирается тайный орден, цель которого – бороться с темными силами. Гарри должен позволить профессору Злею научить его защищаться от яростных атак Вольдеморта на сознание. Но они становятся сильнее день ото дня, и у Гарри остается все меньше времени…",
                            Image = "2fa60505-26f8-41c0-8e59-08d9011bcb96",
                            ISBN = "978-1-78110-298-5",
                            Pages = 570,
                            Year = 2005,
                            Language = Language.Russian,
                            Epub = "2fa60505-26f8-41c0-8e59-08d9011bcb96", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("E959400D-FBE3-49B8-A815-08A774043384")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"),
                            Title = "Гарри Поттер и Тайная комната",
                            Description =
                                "«Заговор, Гарри Поттер. Заговор – в этом году в Хогвартсе, школе колдовства и ведьминских искусств, произойдут ужаснейшие события». Лето у Гарри Поттера состояло из самого ужасного дня рождения в жизни, мрачных предупреждений от домового эльфа по имени Добби и спасения от Дурслеев, когда его друг Рон Уизли прибыл за ним на волшебной летающей машине! Вернувшись в школу колдовства и ведьминских искусств «Хогварц» на второй курс, Гарри слышит странный шепот, который эхом раздается в пустых коридорах. А потом начинаются нападения. Студентов находят будто превращенными в камень… Кажется, что зловещие предсказания Добби начинают сбываться.",
                            Image = "0e075f46-2fd3-4e3d-8e5a-08d9011bcb96",
                            ISBN = "978-1-78110-298-5",
                            Pages = 570,
                            Year = 2005,
                            Language = Language.Russian,
                            Epub = "0e075f46-2fd3-4e3d-8e5a-08d9011bcb96", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("E959400D-FBE3-49B8-A815-08A774043384")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("B30D91F4-D553-4D27-6EF9-08D9014D78A2"),
                            Title = "Жизнь и наука Ричарда Фейнмана",
                            Description =
                                "Эта книга – о жизни и работе нобелевского лауреата по физике Ричарда Фейнмана. Прекрасный язык, доступное описание сложных физических проблем, проступающий сквозь страницы магнетизм личности ученого делают рассказ интересным не только для физиков, но и для всех, кто интересуется историей науки. На русском языке публикуется впервые.",
                            Image = "b30d91f4-d553-4d27-6ef9-08d9014d78a2",
                            ISBN = "978-5-00117-609-1",
                            Pages = 890,
                            Year = 1992,
                            Language = Language.Russian,
                            Epub = "b30d91f4-d553-4d27-6ef9-08d9014d78a2", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("164E6104-D3F9-4947-C86C-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("314DE2D1-9C84-4A89-97C8-49DF4FA88ABD")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("048CC638-34A4-40E1-6EFA-08D9014D78A2"),
                            Title = "Сильмариллион",
                            Description = @"И было так:

                                Единый, называемый у эльфов Илуватар, создал Айнур, и они сотворили перед ним Великую Песнь, что стала светом во тьме и Бытием, помещенным среди Пустоты.

                                И стало так:

                                Эльфы – нолдор – создали Сильмарили, самое прекрасное из всего, что только возможно создать руками и сердцем. Но вместе с великой красотой в мир пришли и великая алчность, и великое же предательство…

                                «Сильмариллион» – один из масштабнейших миров в истории фэнтези, мифологический канон, который Джон Руэл Толкин составлял на протяжении всей жизни. Свел же разрозненные фрагменты воедино, подготовив текст к публикации, сын Толкина Кристофер. В 1996 году он поручил художнику-иллюстратору Теду Несмиту нарисовать серию цветных произведений для полноцветного издания. Теперь российский читатель тоже имеет возможность приобщиться к великолепной саге.",
                            Image = "048cc638-34a4-40e1-6efa-08d9014d78a2",
                            ISBN = "978-5-17-088588-6",
                            Pages = 590,
                            Year = 1977,
                            Language = Language.Russian,
                            Epub = "048cc638-34a4-40e1-6efa-08d9014d78a2", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("907580B2-7E31-44FC-9FBE-79728C1F06FC")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("4CC94DBF-B090-45A2-6EFB-08D9014D78A2"),
                            Title = "Песни Белерианда",
                            Description =
                                "Третий том «Истории Средиземья» дает нам уникальную возможность взглянуть на созидание мифологии Средиземья через призму стихотворных переложений двух наиболее значимых сюжетов толкиновского мира: легенд о Турине и о Лутиэн. Первая из поэм, грандиозная неопубликованная «Песнь о детях Хурина», посвящена трагедии Турина Турамбара. Вторая, проникновенное «Лэ о Лейтиан», главный источник предания о Берене и Лутиэн в «Сильмариллионе», повествует о Походе за Сильмарилями и о столкновении с Морготом в его подземной крепости. Поэмы сопровождаются комментариями об эволюции истории Древних Дней. В книгу также включен любопытный критический разбор «Лэ о Лейтиан» К. С. Льюиса, который прочел поэму в 1929 году.",
                            Image = "4cc94dbf-b090-45a2-6efb-08d9014d78a2",
                            ISBN = "978-5-17-111389-6",
                            Pages = 530,
                            Year = 1985,
                            Language = Language.Russian,
                            Epub = "4cc94dbf-b090-45a2-6efb-08d9014d78a2", Pdf = "4cc94dbf-b090-45a2-6efb-08d9014d78a2",
                            Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("907580B2-7E31-44FC-9FBE-79728C1F06FC")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("623CD2F3-DFF7-46DD-6EFC-08D9014D78A2"),
                            Title = "Дети Хурина. Нарн и Хин Хурин",
                            Description = @"Последнее из «Утраченных сказаний» Средиземья…

                                Последнее произведение великого Джона Рональда Руэла Толкина.

                                Книга, рукопись которой подготовил к публикации и отредактировал сын Толкина Кристофер.

                                История короля Хурина и его сына, проклятого героя Турина Турамбара, жребием коего было нести погибель всем, кого он полюбит.

                                История черных дней эльфийских королевств Средиземья, одного за другим падавших под натиском сил Темного Властелина Моргота…

                                История лучшего друга Турина – эльфийского воина Белега Куталиона – и его сестры Ниэнор…

                                История великого подвига и великой скорби.",
                            Image = "623cd2f3-dff7-46dd-6efc-08d9014d78a2",
                            ISBN = "978-5-17-054929-0",
                            Pages = 280,
                            Year = 2007,
                            Language = Language.Russian,
                            Epub = "623cd2f3-dff7-46dd-6efc-08d9014d78a2", Pdf = "623cd2f3-dff7-46dd-6efc-08d9014d78a2",
                            Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("149C5383-DBF8-4104-31A7-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("907580B2-7E31-44FC-9FBE-79728C1F06FC")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"),
                            Title = "1984",
                            Description =
                                "Одна из самых знаменитых антиутопий XX века – роман «1984» английского писателя Джорджа Оруэлла (1903–1950) был написан в 1948 году и продолжает тему «преданной революции», раскрытую в «Скотном дворе». По Оруэллу, нет и не может быть ничего ужаснее тотальной несвободы. Тоталитаризм уничтожает в человеке все духовные потребности, мысли, чувства и сам разум, оставляя лишь постоянный страх и единственный выбор – между молчанием и смертью, и если Старший Брат смотрит на тебя и заявляет, что «дважды два – пять», значит, так и есть.",
                            Image = "1db9f906-f202-4174-6efd-08d9014d78a2",
                            ISBN = "978-5-17-080115-0",
                            Pages = 320,
                            Year = 1949,
                            Language = Language.Russian,
                            Epub = null, Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("E1F56D7C-F3B9-47B5-31A9-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("18E16B99-1E0F-48FD-627D-08D8F84F7640")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("92A223CD-0339-46CB-6EFE-08D9014D78A2"),
                            Title = "Скотный двор",
                            Description =
                                "Повесть-притча «Скотный двор» Джорджа Оруэлла полна острого сарказма и политической сатиры. Обитатели фермы олицетворяют самые ужасные людские пороки, а сама ферма становится символом тоталитарного общества. Как будут существовать в таком обществе его обитатели – животные, которых поведут на бойню?",
                            Image = "",
                            ISBN = "978-5-04-116435-5",
                            Pages = 200,
                            Year = 1943,
                            Language = Language.Russian,
                            Epub = null, Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("E1F56D7C-F3B9-47B5-31A9-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("18E16B99-1E0F-48FD-627D-08D8F84F7640")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("E415C910-948A-4E91-6EFF-08D9014D78A2"),
                            Title = "Кровь эльфов",
                            Description =
                                @"Цинтра захвачено Нильфгаардской империей. Всюду пламя и разрушения, сотни погибших. Прекрасное королевство пало.

                                Наследнице Цири чудом удается спастись. Напуганную, потерявшую близких и дом девочку Геральт доставляет в убежище ведьмаков. Неожиданно для всех у принцессы открываются магические способности.

                                Чтобы понять их природу, Геральт обращается за помощью к чародейке. Однако она советует ведьмаку призвать свою бывшую возлюбленную Йеннифэр. Ибо только она сможет научить девочку пользоваться ее даром…",
                            Image = "e415c910-948a-4e91-6eff-08d9014d78a2",
                            ISBN = "0-1995-7172-4",
                            Pages = 330,
                            Year = 1994,
                            Language = Language.Russian,
                            Epub = "e415c910-948a-4e91-6eff-08d9014d78a2", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("742AECEA-866C-4F08-1B35-08D8F850467A")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("40F50212-6512-42C3-6F00-08D9014D78A2"),
                            Title = "Час Презрения",
                            Description =
                                @"Йеннифер и княжна Цирилла отправились на остров Танедд, чтобы принять участие в Чародейском Сборе. Чародейка не только хочет обсудить судьбы мира, но и устроить Цири в школу Аретузе. Геральт сопровождает их на всем пути странствия, распространяя слух о гибели Цири во время нападения на Цинтру.

                                Все планы рушит мятеж чародеев, послуживший началу новой войны. Геральту чудом удается спастись, в Цири перемещается в незнакомое место…",
                            Image = "40f50212-6512-42c3-6f00-08d9014d78a2",
                            ISBN = "0-1995-7172-4",
                            Pages = 360,
                            Year = 1995,
                            Language = Language.Russian,
                            Epub = "40f50212-6512-42c3-6f00-08d9014d78a2", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("742AECEA-866C-4F08-1B35-08D8F850467A")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("EED4F44B-B6DC-4A7E-6F01-08D9014D78A2"),
                            Title = "Крещение огнем",
                            Description = @"Нильфгаард продолжает наступление на северные королевства.

                                Тем временем Геральт, восстановивший силы у дриад после восстания на Таннеде, отправляется на поиски Цири. В этом нелегком путешествии к ведьмаку присоединяются его старые знакомцы: трубадур Лютик, лучница Мария Барринг, рыцарь Кагыр и вампир Эмиль Регис. На спасение Цири отправляется и Йеннифер, которая смогла сбежать из плена.

                                Сама Цири под именем Фалька стала разбойничать в банде под названием Крысы. Однако за бандой уже следит безжалостный охотник за головами Лео Бонарт…",
                            Image = "eed4f44b-b6dc-4a7e-6f01-08d9014d78a2",
                            ISBN = "0-6836-8776-X",
                            Pages = 380,
                            Year = 1995,
                            Language = Language.Russian,
                            Epub = "eed4f44b-b6dc-4a7e-6f01-08d9014d78a2", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("742AECEA-866C-4F08-1B35-08D8F850467A")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("4CB116C7-C0EE-4EA2-6F02-08D9014D78A2"),
                            Title = "Башня ласточки",
                            Description =
                                @"Все окончательно перепуталось, интриги сплелись в тугой клубок, распутать который никому не по силам. Идет война, гибнут люди и нелюди. Избранные чародеи решили, что они в праве вершить судьбы мира. Йенифер попала в серьезную переделку, и вряд ли ее спасет даже ее хладнокровие и цинизм. Не лучше дела обстоят у Геральда и Цири.

                                К чему приведет этот смертоносный круговорот событий?",
                            Image = "4cb116c7-c0ee-4ea2-6f02-08d9014d78a2",
                            ISBN = "0-7489-5214-4",
                            Pages = 491,
                            Year = 1997,
                            Language = Language.Russian,
                            Epub = "4cb116c7-c0ee-4ea2-6f02-08d9014d78a2", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("742AECEA-866C-4F08-1B35-08D8F850467A")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("BBC7FF24-5935-45EA-6F03-08D9014D78A2"),
                            Title = "Меч Предназначения",
                            Description =
                                "Ведьмак – это мастер меча и мэтр волшебства, ведущий непрерывную войну с кровожадными монстрами, которые угрожают покою сказочной страны. «Ведьмак» – это мир на острие меча, ошеломляющее действие, незабываемые ситуации, великолепные боевые сцены.",
                            Image = "bbc7ff24-5935-45ea-6f03-08d9014d78a2",
                            ISBN = "0-7489-5214-4",
                            Pages = 380,
                            Year = 1992,
                            Language = Language.Russian,
                            Epub = "bbc7ff24-5935-45ea-6f03-08d9014d78a2", Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("57DB208B-090F-4525-C86E-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("742AECEA-866C-4F08-1B35-08D8F850467A")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }, new Book
                        {
                            Id = new Guid("98ABD9AC-686F-4399-CFEE-08D906742E3D"),
                            Title = "Как устроена экономика",
                            Description =
                                @"В этой книге экономист из Кембриджа Ха-Джун Чанг в занимательной и доступной форме объясняет, как на самом деле работает мировая экономика. Автор предлагает читателю идеи, которых не найдешь в учебниках по экономике, и делает это с глубоким знанием истории, остроумием и легким пренебрежением к традиционным экономическим концепциям.

                                Книга будет полезной для тех, кто интересуется экономикой и хочет лучше понимать, как устроен мир.",
                            Image = "98abd9ac-686f-4399-cfee-08d906742e3d",
                            ISBN = "978-5-00117-069-3",
                            Pages = 460,
                            Year = 2014,
                            Language = Language.Russian,
                            Epub = null, Pdf = null, Fb2 = null,
                            Subject = dbContext.Subjects.Find(new Guid("B15507AF-F6E1-4526-C86B-08D8F82B49EA")),
                            Genre = dbContext.Genres.Find(new Guid("795C1355-6810-4A3D-31A3-08D8F82B18A8")),
                            Author = dbContext.Authors.Find(new Guid("907580B2-7E31-44FC-9FBE-79728C1F06FC")),
                            Publisher = dbContext.Publishers.Find(new Guid("34A517B4-DF61-46BC-59E9-08D900DA0F27"))
                        }
                    );
                }

                if (!dbContext.Storage.Any())
                {
                    dbContext.Storage.AddRange(
                        new BookItem()
                        {
                            Id = new Guid("767139BA-95E0-40E8-E373-08D90819B6DA"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 61614,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("042D3282-0D8E-4EF8-E378-08D90819B6DA"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 6482,
                            Library = dbContext.Libraries.Find(new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C")),
                            Book = dbContext.Books.Find(new Guid("BED90FA7-C4C1-4A2D-8E58-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("B6441AAD-BDD3-4DDC-E379-08D90819B6DA"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 86744,
                            Library = dbContext.Libraries.Find(new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C")),
                            Book = dbContext.Books.Find(new Guid("BED90FA7-C4C1-4A2D-8E58-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("FC9C382A-ACD4-4631-E37A-08D90819B6DA"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 65864,
                            Library = dbContext.Libraries.Find(new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("6D59EB47-8811-45AD-E37B-08D90819B6DA"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 18940,
                            Library = dbContext.Libraries.Find(new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C")),
                            Book = dbContext.Books.Find(new Guid("98ABD9AC-686F-4399-CFEE-08D906742E3D"))
                        }, new BookItem()
                        {
                            Id = new Guid("2F235C9D-3447-496B-E37C-08D90819B6DA"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 56473,
                            Library = dbContext.Libraries.Find(new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C")),
                            Book = dbContext.Books.Find(new Guid("98ABD9AC-686F-4399-CFEE-08D906742E3D"))
                        }, new BookItem()
                        {
                            Id = new Guid("E85AF7A6-EDD3-4ACD-47D1-08D908228CD0"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 50972,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("4CB116C7-C0EE-4EA2-6F02-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("C93EDF43-18D1-4209-47D3-08D908228CD0"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 94496,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("4CB116C7-C0EE-4EA2-6F02-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("EF5AE53E-D8DE-47B2-47D5-08D908228CD0"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 87953,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("2FA60505-26F8-41C0-8E59-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("486847CD-F610-4D3B-47DA-08D908228CD0"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 31616,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("BED90FA7-C4C1-4A2D-8E58-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("9914EDC9-A7D2-4B03-47DC-08D908228CD0"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 93692,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("048CC638-34A4-40E1-6EFA-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("F4C93B85-52D6-4E54-AD30-08D90844B38B"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 48538,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("08807358-6D37-4CB2-8E56-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("FE231623-8E01-4E31-AD31-08D90844B38B"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 96725,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("08807358-6D37-4CB2-8E56-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("A5C34CC1-26DE-404C-AD32-08D90844B38B"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 67801,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("EED4F44B-B6DC-4A7E-6F01-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("8FBC02A4-1525-4850-64E1-08D90A23BC1E"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 5411,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("8D3E4C58-FCFC-49B8-64E2-08D90A23BC1E"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 70902,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("E754AEB3-A7EE-471C-64E3-08D90A23BC1E"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 70320,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("8F0EFE6C-629B-49AC-94F4-08D90C5B3247"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 79052,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("8C8290F1-13F9-4B6B-94F5-08D90C5B3247"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 39735,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("4D87EF1C-6EF0-4E75-94F6-08D90C5B3247"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 43756,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("1409F902-B7BF-41ED-94F7-08D90C5B3247"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 10263,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("9C7A78F3-0C8A-4AB4-9492-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 89608,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("19922DD4-0DEB-4193-9493-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 717,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("D7BF5925-6347-4A20-9494-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 30271,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("10E34891-4E97-4E52-9495-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 29587,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("22331CE9-A251-44B9-9496-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 7652,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("617E8F14-E1D0-47BB-9497-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 5285,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("0E075F46-2FD3-4E3D-8E5A-08D9011BCB96"))
                        }, new BookItem()
                        {
                            Id = new Guid("32FC7056-627E-4A1C-9498-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 71223,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("623CD2F3-DFF7-46DD-6EFC-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("E57F4BB7-E68C-44FD-9499-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 32566,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("623CD2F3-DFF7-46DD-6EFC-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("613AB7E8-4943-4015-949A-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 93898,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("623CD2F3-DFF7-46DD-6EFC-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("BDE82475-A78E-4A28-949B-08D90C6682B3"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 31858,
                            Library = dbContext.Libraries.Find(new Guid("C58D11E0-DE51-436C-BC06-D62CD7B56799")),
                            Book = dbContext.Books.Find(new Guid("623CD2F3-DFF7-46DD-6EFC-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("9CD3F465-18EA-4474-28C3-08D91225FEA2"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 62955,
                            Library = dbContext.Libraries.Find(new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }, new BookItem()
                        {
                            Id = new Guid("EDF10CBA-400D-4AF5-28C4-08D91225FEA2"),
                            ArrivalDate = DateTime.Now,
                            InventoryNumber = 64955,
                            Library = dbContext.Libraries.Find(new Guid("C63D0613-318B-4552-46B9-08D8F8B3C53C")),
                            Book = dbContext.Books.Find(new Guid("1DB9F906-F202-4174-6EFD-08D9014D78A2"))
                        }
                    );
                }

                dbContext.SaveChanges();
            }
        }
    }

    public struct DigitalLibraryConstants
    {
        public struct Policies
        {
            public const string Administration = "administration";
            public const string Moderation = "moderation";
        }

        public struct Roles
        {
            public const string Admin = "administrator";
            public const string Moder = "moderator";
        }

        public struct Claims
        {
            public const string Role = "role";
        }
    }
}
