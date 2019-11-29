using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

namespace  Westdrive
{
    public class Net
    {
        public  delegate void AsyncNetResult(string response);
        public static async void getStringRequest(string url, AsyncNetResult callback)
        {
            WestdriveSettings.CheckIO = IOState.pending;
            WestdriveSettings.CheckIO = IOState.working;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage serverResponse = await client.GetAsync(url))
                    {
                        using (HttpContent content = serverResponse.Content)
                        {
                            string response = await content.ReadAsStringAsync();
                            callback(response);
                        }
                    }
                }

                WestdriveSettings.CheckIO = IOState.ready;
            }
            catch (Exception ex)
            {
                callback("server error");
                WestdriveSettings.CheckIO = IOState.ready;
            }
        }
    }

}

