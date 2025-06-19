using AutoMapper;
using Repository.Models;
using Repository.UnitOfWork.Interface;
using Services.ModelView;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Implement
{
    public class PaymentService: IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaymentResponse> CreatePayment(PaymentRequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedOrder = await _unitOfWork.OrderRepository.GetByIDAsync(int.Parse(paymentRequest.vnp_TxnRef));
                    if (existedOrder != null)
                    {
                        var payment = new Payment()
                        {
                            PaymentMethod = "VNPay",
                            BankCode = paymentRequest.vnp_BankCode,
                            BankTranNo = paymentRequest.vnp_BankTranNo,
                            CardType = paymentRequest.vnp_CardType,
                            PaymentInfo = paymentRequest.vnp_OrderInfo,
                            PayDate = DateTime.ParseExact(paymentRequest.vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            TransactionNo = paymentRequest.vnp_TransactionNo,
                            TransactionStatus = int.Parse(paymentRequest.vnp_TransactionStatus),
                            PaymentAmount = decimal.Parse(paymentRequest.vnp_Amount) / 100,
                            OrderId = int.Parse(paymentRequest.vnp_TxnRef)
                        };
                        await _unitOfWork.PaymentRepository.InsertAsync(payment);
                        //Update Order's status is Paid
                        existedOrder.Status = 1;
                        existedOrder.TransactionCode = paymentRequest.vnp_BankCode;
                        await _unitOfWork.OrderRepository.UpdateAsync(existedOrder);
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return _mapper.Map<PaymentResponse>(payment);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
