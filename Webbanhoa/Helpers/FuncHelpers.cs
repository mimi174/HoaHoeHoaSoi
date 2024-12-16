using HoaHoeHoaSoi.Model;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace HoaHoeHoaSoi.Helpers
{
    public static class FuncHelpers
    {
        public static string ConvertImgToBase64(IFormFile img)
        {
            if (img == null)
                return string.Empty;

            string imgValue = string.Empty;
            using (var ms = new MemoryStream())
            {
                img.CopyTo(ms);
                imgValue = Convert.ToBase64String(ms.ToArray());
            }

            return imgValue;
        }

        public static string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }

        public static async Task<MomoResponse> CreatePaymentAsync(MomoOrder model, MomoAPI config)
        {
            model.Id = DateTime.UtcNow.Ticks.ToString();
            model.OrderInfo = "Khách hàng: " + model.CustomerName + ". Nội dung: " + model.OrderInfo;
            var rawData =
                $"partnerCode={config.PartnerCode}&accessKey={config.AccessKey}&requestId={model.Id}&amount={model.Total}&orderId={model.Id}&orderInfo={model.OrderInfo}&returnUrl={config.ReturnUrl}&notifyUrl={config.NotifyUrl}&extraData=";

            var signature = ComputeHmacSha256(rawData, config.SecretKey);

            var client = new RestClient(config.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create an object representing the request data
            var requestData = new
            {
                accessKey = config.AccessKey,
                partnerCode = config.PartnerCode,
                requestType = config.RequestType,
                notifyUrl = config.NotifyUrl,
                returnUrl = config.ReturnUrl,
                orderId = model.Id,
                amount = model.Total.ToString(),
                orderInfo = model.OrderInfo,
                requestId = model.Id,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<MomoResponse>(response.Content);
        }

        public static MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;
            var localMessage = collection.First(s => s.Key == "localMessage").Value;
            var errorCode = collection.First(s => s.Key == "errorCode").Value;
            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                ErrorCode = errorCode,
                LocalMessage = localMessage
            };
        }
    }
}
