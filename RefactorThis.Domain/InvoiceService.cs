using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private readonly IInvoiceRepository _invoiceRepository;

		public InvoiceService( IInvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
		}

		private void AddPayment(Invoice invoice, Payment payment) {
			invoice.AmountPaid += payment.Amount;
			if ( invoice.Type == InvoiceType.Commercial) {
				invoice.TaxAmount += payment.Amount * 0.14m;
			}
			invoice.Payments.Add( payment );
		}
		public string ProcessPayment( Payment payment )
		{
			var inv = _invoiceRepository.GetInvoice( payment.Reference );

			var responseMessage = string.Empty;

			if ( inv == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}
			switch ( inv.GetStatus )
			{
				case InvoiceStatus.NoPaimentNeeded:
					responseMessage = "no payment needed";
					break;
				case InvoiceStatus.Overpaid:
					responseMessage = "the payment is greater than the partial amount remaining";
					break;
				case InvoiceStatus.Paid:
					responseMessage = "invoice was already fully paid";
					break;
				case InvoiceStatus.PartiallyPaid:
					if (inv.Payments.Sum( x => x.Amount ) != 0 && payment.Amount > ( inv.Amount - inv.AmountPaid)) {
						responseMessage = "the payment is greater than the partial amount remaining";
					} else if ((inv.Amount - inv.AmountPaid ) == payment.Amount) {
						AddPayment( inv, payment );
						responseMessage = "final partial payment received, invoice is now fully paid";
					} else if ( payment.Amount < inv.Amount ) {
						AddPayment( inv, payment );
						responseMessage = "another partial payment received, still not fully paid";
					}
					break;
				case InvoiceStatus.Unpaid:
					if (inv.Payments.Sum( x => x.Amount ) != 0 && payment.Amount > ( inv.Amount - inv.AmountPaid)) {
						responseMessage = "the payment is greater than the invoice amount";
					} else if ((inv.Amount - inv.AmountPaid ) == payment.Amount) {
						AddPayment( inv, payment );
						responseMessage = "invoice is now fully paid";
					} else if ( payment.Amount < inv.Amount ) {
						AddPayment( inv, payment );
						responseMessage = "invoice is now partially paid";
					}
					break;
				default:
					throw new ArgumentOutOfRangeException( );
			}

			inv.Save();

			return responseMessage;
		}
	}
}