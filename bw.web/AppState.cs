using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bw.web
{
    public class AppState
    {
        private BWClient _BW;

        public AppState(BWClient bw)
        {
            _BW = bw;
        }

        public BWClient BW
        {
            get => _BW;
        }

        //public async Task InitBW()
        //{
        //    App.BW = new BWClient();
        //    App.BW.CreateServiceResolver = (t, n) =>
        //    {
        //        if (t == typeof(ICryptoPrimitiveService))
        //        {
        //            return new JSInteropCryptoPrimitiveService(JS);
        //        }
        //        if (t == typeof(IStorageService))
        //        {
        //            if (n == "storageService")
        //            {
        //                return Storage;
        //            }
        //            else if (n == "secureStorageService")
        //            {
        //                return Storage;
        //            }
        //            else
        //            {
        //                throw new Exception("Unexpected storage service name: " + n);
        //            }
        //        }
        //        return null;
        //    };
        //    await App.BW.InitAsync();
        //}
    }
}
