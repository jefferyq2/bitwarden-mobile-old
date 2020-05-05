using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Bit.Core.Exceptions;
using Bit.Core.Models.Domain;
using Bit.Core.Services;
using Bit.Core.Utilities;
using bw.lib.Abstractions;
using bw.lib.Services;

namespace bw.web
{
    public class BWClient
    {
        private IApiService _apiService;
        private IUserService _userService;
        private IBroadcasterService _broadcasterService;
        private IMessagingService _messagingService;
        private IStateService _stateService;
        private ILockService _lockService;
        private ISyncService _syncService;
        private ITokenService _tokenService;
        private ICryptoService _cryptoService;
        private ICipherService _cipherService;
        private IFolderService _folderService;
        private ICollectionService _collectionService;
        private ISettingsService _settingsService;
        private IPasswordGenerationService _passwordGenerationService;
        private ISearchService _searchService;
        private IPlatformUtilsService _platformUtilsService;
        private IAuthService _authService;
        private IStorageService _storageService;
        private IStorageService _secureStorageService;

        public BWClient()
        {
        }

        public string CustomUserAgent { get; } = "bw.lib";

        // We have to lie because otherwise we get an "invalid_client" error
        public string IdentityClientId { get; } = "web"; // or "mobile"

        public async Task InitAsync()
        {
            InitServiceContainer();
            await Bootstrap();
            ResolveServices();
        }

        public async Task<bool> Login(string email, string masterPassword)
        {
            //try
            //{
                var authResult = await _authService.LogInAsync(email, masterPassword);
                return true;
            //}
            //catch (ApiException ex)
            //{
            //    Console.Error.WriteLine("Login Failed:");
            //    if (ex.Error != null)
            //    {
            //        Console.Error.WriteLine("Error Details:");
            //        Console.Error.WriteLine(ex.Error.GetSingleMessage());
            //        Console.Error.WriteLine(JsonSerializer.Serialize(ex.Error.ValidationErrors));
            //    }
            //    return false;
            //}
        }

        public async Task WhoAmI()
        {
            var jso = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            Console.WriteLine("Is Authenticated....: {0}", await _userService.IsAuthenticatedAsync());
            Console.WriteLine("User ID.............: {0}", await _userService.GetUserIdAsync());
            Console.WriteLine("Acct Rev Date.......: {0}",
                CoreHelpers.Epoc.AddMilliseconds(await _apiService.GetAccountRevisionDateAsync()));

            var sync = await _apiService.GetSyncAsync();
            await _cryptoService.SetOrgKeysAsync(sync.Profile.Organizations);

            Console.WriteLine();
            Console.WriteLine("Profile:");
            Console.WriteLine("  ID........................: {0}", sync.Profile.Id);
            Console.WriteLine("  Name......................: {0}", sync.Profile.Name);
            Console.WriteLine("  Email.....................: {0}", sync.Profile.Email);
            Console.WriteLine("  Email Verified............: {0}", sync.Profile.EmailVerified);
            Console.WriteLine("  Premium...................: {0}", sync.Profile.Premium);
            //Console.WriteLine("  Master Password Hint......: {0}", sync.Profile.MasterPasswordHint);
            //Console.WriteLine("  Security Stamp............: {0}", sync.Profile.SecurityStamp);
            //Console.WriteLine("  Key.......................: {0}", sync.Profile.Key);
            //Console.WriteLine("  Private Key...............: {0}", sync.Profile.PrivateKey);

            Console.WriteLine();
            Console.WriteLine("Orgs:");
            string orgKeyS = null;
            SymmetricCryptoKey orgKey = null;
            foreach (var o in sync.Profile.Organizations)
            {
                Console.WriteLine("  * {0} = {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", o.Id, o.Name,
                    o.Enabled, o.Type, o.Status, o.MaxCollections, o.Status, o.Seats, o.Key);
                orgKeyS = o.Key;
                //orgKey = new SymmetricCryptoKey(await _cryptoService.DecryptToBytesAsync(new CipherString(o.Key)));
            }

            Console.WriteLine();
            Console.WriteLine("Folders:");
            foreach (var f in sync.Folders)
            {
                var nm = await _cryptoService.DecryptToUtf8Async(new CipherString(f.Name));
                Console.WriteLine("  * {0} = {1} = {2}", f.Id, f.Name, nm);
            }

            Console.WriteLine();
            Console.WriteLine("Collections:");
            foreach (var c in sync.Collections)
            {
                //var nm = await _cryptoService.DecryptToUtf8Async(new CipherString(c.Name), orgKey);
                var nm = await _cryptoService.DecryptToUtf8Async(new CipherString(c.Name));
                Console.WriteLine("  * {0} = {1}, {2}, {3}, {4}, {5}", c.Id, c.Name, nm,
                    c.OrganizationId, c.ReadOnly, c.ExternalId);
            }

            Console.WriteLine();
            Console.WriteLine("Ciphers:");
            foreach (var c in sync.Ciphers)
            {
                var nm = await _cryptoService.DecryptToUtf8Async(new CipherString(c.Name));
                var us = c.Login == null ? "" : await _cryptoService.DecryptToUtf8Async(new CipherString(c.Login.Username));
                var pw = c.Login == null ? "" : await _cryptoService.DecryptToUtf8Async(new CipherString(c.Login.Password));
                //var nt = await _cryptoService.DecryptToUtf8Async(new CipherString(c.Notes));
                Console.WriteLine("  * {0}, {1}, {2}, {3}", JsonSerializer.Serialize(c, jso),
                    nm, us, pw);
            }
        }

        public Func<Type, string, object> CreateServiceResolver { get; set; }

        public T CreateService<T>(string name, Func<T> def)
        {
            var s = CreateServiceResolver?.Invoke(typeof(T), name);
            if (s == null)
                s = def();
            return (T)s;
        }

        private void InitServiceContainer()
        {
            var localizeService = new LocalizeService();
            var i18nService = new CliI18nService(localizeService.GetCurrentCultureInfo());
            ServiceContainer.Register<ILocalizeService>(nameof(localizeService), localizeService);
            ServiceContainer.Register<II18nService>(nameof(i18nService), i18nService);

            var broadcasterService = new BroadcasterService();
            var messagingService = new CliBroadcasterMessagingService(broadcasterService);
            ServiceContainer.Register<IBroadcasterService>(nameof(broadcasterService), broadcasterService);
            ServiceContainer.Register<IMessagingService>  (nameof(messagingService), messagingService);

            var platformUtilsService = new PlatformUtilsService(IdentityClientId);
            ServiceContainer.Register<IPlatformUtilsService>(nameof(platformUtilsService), platformUtilsService);

            //var storageService = new LiteDbStorageService("bwcli.db");
            var storageService = CreateService<IStorageService>("storageService", () => new SecureStorageService());
            ServiceContainer.Register<IStorageService>(nameof(storageService), storageService);
            var secureStorageService = CreateService<IStorageService>("secureStorageService", () => new SecureStorageService());
            ServiceContainer.Register<IStorageService>(nameof(secureStorageService), secureStorageService);

            var cryptoPrimitiveService = CreateService<ICryptoPrimitiveService>("cryptoPrimitiveService", () => new CryptoPrimitiveService());
            ServiceContainer.Register<ICryptoPrimitiveService>(nameof(cryptoPrimitiveService), cryptoPrimitiveService);

            ServiceContainer.Init(CustomUserAgent);
        }

        async Task Bootstrap()
        {
            (ServiceContainer.Resolve<II18nService>("i18nService") as CliI18nService).Init();
            ServiceContainer.Resolve<IAuthService>("authService").Init();
            await ServiceContainer.Resolve<IEnvironmentService>("environmentService").SetUrlsFromStorageAsync();
        }

        private void ResolveServices()
        {
            _apiService = ServiceContainer.Resolve<IApiService>("apiService");
            _userService = ServiceContainer.Resolve<IUserService>("userService");
            _broadcasterService = ServiceContainer.Resolve<IBroadcasterService>("broadcasterService");
            _messagingService = ServiceContainer.Resolve<IMessagingService>("messagingService");
            _stateService = ServiceContainer.Resolve<IStateService>("stateService");
            _lockService = ServiceContainer.Resolve<ILockService>("lockService");
            _syncService = ServiceContainer.Resolve<ISyncService>("syncService");
            _tokenService = ServiceContainer.Resolve<ITokenService>("tokenService");
            _cryptoService = ServiceContainer.Resolve<ICryptoService>("cryptoService");
            _cipherService = ServiceContainer.Resolve<ICipherService>("cipherService");
            _folderService = ServiceContainer.Resolve<IFolderService>("folderService");
            _settingsService = ServiceContainer.Resolve<ISettingsService>("settingsService");
            _collectionService = ServiceContainer.Resolve<ICollectionService>("collectionService");
            _searchService = ServiceContainer.Resolve<ISearchService>("searchService");
            _authService = ServiceContainer.Resolve<IAuthService>("authService");
            _platformUtilsService = ServiceContainer.Resolve<IPlatformUtilsService>("platformUtilsService");
            _storageService = ServiceContainer.Resolve<IStorageService>("storageService");
            _secureStorageService = ServiceContainer.Resolve<IStorageService>("secureStorageService");
            _passwordGenerationService = ServiceContainer.Resolve<IPasswordGenerationService>(
                "passwordGenerationService");
        }
    }
}
