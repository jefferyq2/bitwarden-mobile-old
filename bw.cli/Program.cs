using System;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Bit.Core.Exceptions;
using Bit.Core.Models.Domain;
using Bit.Core.Services;
using Bit.Core.Utilities;
using bw.lib.Abstractions;
using bw.lib.Services;

namespace bw.cli
{
    public class Program
    {
        public const string CustomUserAgent = "bw.cli";
        // We have to lie because otherwise we get an "invalid_client" error
        public const string IdentityClientId = "web"; // "mobile"; // nameof(bwcli);

        private readonly IApiService _apiService;
        private readonly IUserService _userService;
        private readonly IBroadcasterService _broadcasterService;
        private readonly IMessagingService _messagingService;
        private readonly IStateService _stateService;
        private readonly ILockService _lockService;
        private readonly ISyncService _syncService;
        private readonly ITokenService _tokenService;
        private readonly ICryptoService _cryptoService;
        private readonly ICipherService _cipherService;
        private readonly IFolderService _folderService;
        private readonly ICollectionService _collectionService;
        private readonly ISettingsService _settingsService;
        private readonly IPasswordGenerationService _passwordGenerationService;
        private readonly ISearchService _searchService;
        private readonly IPlatformUtilsService _platformUtilsService;
        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;
        private readonly IStorageService _secureStorageService;
        private readonly AppOptions _appOptions;


        public Program(AppOptions appOptions = null)
        {
            _appOptions = appOptions ?? new AppOptions();
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

        public async Task<bool> Login(string email, string masterPassword)
        {
            try
            {
                var authResult = await _authService.LogInAsync(email, masterPassword);
                return true;
            }
            catch (ApiException ex)
            {
                Console.Error.WriteLine("Login Failed:");
                if (ex.Error != null)
                {
                    Console.Error.WriteLine("Error Details:");
                    Console.Error.WriteLine(ex.Error.GetSingleMessage());
                    Console.Error.WriteLine(JsonSerializer.Serialize(ex.Error.ValidationErrors));
                }
                return false;
            }
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

        public async Task ListFolders()
        {
            var all = await _folderService.GetAllDecryptedAsync();
            Console.WriteLine();
            Console.WriteLine("Folders:");
            foreach (var f in all)
            {
                Console.WriteLine("  * {0} = {1}", f.Id, f.Name);
            }
        }

        public async Task ListItems()
        {
            var all = await _cipherService.GetAllDecryptedAsync();
            Console.WriteLine("Ciphers:");
            foreach (var c in all)
            {
                Console.WriteLine("  * {0} = {1}", c.Id, c.Name, c.Login?.Username, c.Login?.Password);
            }
        }

        static async Task Main(string[] args)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (_, _, _, _) =>
            {
                return true;
            };
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls |
                System.Net.SecurityProtocolType.Tls11 |
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls13;

            InitServiceContainer();
            await Bootstrap();

            var prog = new Program();
            bool ret;

            //ret = await prog.Login("bwcli-testing@ezstest.org", "b@dPa$$w0rd");
            //ret = await prog.Login("bwcli-testing@ezstest.com", "b@dPa$$w0rd");
            ret = await prog.Login("bwcli-testing@ezstest.com", "bwcli-testing0");
            if (!ret)
                return;

            await prog.WhoAmI();
            await prog.ListFolders();
            //await prog.ListItems();
        }

        static void InitServiceContainer()
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
            var storageService = new InsecureStorageService();

            ServiceContainer.Register<IStorageService>(nameof(storageService), storageService);
            var secureStorageService = new SecureStorageService();
            ServiceContainer.Register<IStorageService>(nameof(secureStorageService), secureStorageService);
            var cryptoPrimitiveService = new CryptoPrimitiveService();
            ServiceContainer.Register<ICryptoPrimitiveService>(nameof(cryptoPrimitiveService), cryptoPrimitiveService);

            ServiceContainer.Init(CustomUserAgent);
        }

        static async Task Bootstrap()
        {
            (ServiceContainer.Resolve<II18nService>("i18nService") as CliI18nService).Init();
            ServiceContainer.Resolve<IAuthService>("authService").Init();
            await ServiceContainer.Resolve<IEnvironmentService>("environmentService").SetUrlsFromStorageAsync();
        }
    }
}
