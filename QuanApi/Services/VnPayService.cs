using QuanApi.Data;
using QuanView.Areas.Admin.Models;

namespace QuanApi.Services
{
    public class VnPayService : IVnPayService

    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);

            // Số tiền thanh toán (nhân 100 theo chuẩn VNPay)
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            //// Nếu có chọn ngân hàng thì thêm BankCode
            //if (cboBankCode.SelectedItem != null &&
            //    !string.IsNullOrEmpty(cboBankCode.SelectedItem.Value))
            //{
            //    vnpay.AddRequestData("vnp_BankCode", cboBankCode.SelectedItem.Value);
            //}

            // Ngày tạo giao dịch
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));

            // Loại tiền tệ
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);

            // Địa chỉ IP khách hàng
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            // Thông tin đơn hàng
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán cho đơn hàng: {model.OrderId}");

            // Loại đơn hàng (mặc định: other)
            vnpay.AddRequestData("vnp_OrderType", "other"); // default value: other

            // URL callback sau khi thanh toán xong
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            // Mã tham chiếu giao dịch (Transaction Reference) - thường là mã đơn hàng
            vnpay.AddRequestData("vnp_TxnRef", $"{model.OrderId}_{tick}");

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymentUrl;


        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode.ToString()
            };

        }

    }
}
