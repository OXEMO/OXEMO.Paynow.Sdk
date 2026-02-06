using System.Collections.Generic;
using System.Threading.Tasks;
using Paynow.Sdk.Models;

namespace Paynow.Sdk
{
    public interface IPaynowClient
    {
        /// <summary>
        /// Tworzy nową płatność.
        /// </summary>
        Task<PaymentResponse?> CreatePaymentAsync(PaymentRequest request, string? idempotencyKey = null);

        /// <summary>
        /// Pobiera status płatności.
        /// </summary>
        Task<PaymentStatusResponse?> GetPaymentStatusAsync(string paymentId);
        bool VerifySignature(string signatureFromHeader, string bodyString, IDictionary<string, string> headers);
        /// <summary>
        /// Zleca zwrot środków.
        /// </summary>
        Task<RefundResponse?> CreateRefundAsync(string paymentId, RefundRequest request, string? idempotencyKey = null);
    }
}