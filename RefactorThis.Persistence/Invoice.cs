using System.Collections.Generic;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;
		public Invoice( IInvoiceRepository repository )
		{
			_repository = repository;
		}

		public void Save( )
		{
			_repository.SaveInvoice( this );
		}

		public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public decimal TaxAmount { get; set; }
		public List<Payment> Payments { get; set; }
		
		public InvoiceType Type { get; set; }
		public InvoiceStatus GetStatus { 
			get
			{
				if ( Amount == 0 && (Payments == null || !Payments.Any()))
				{
					return InvoiceStatus.NoPaimentNeeded;
				}
				else if ( Amount == 0 && (Payments == null || !Payments.Any())) {
					throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
				}
				else if ( AmountPaid > Amount )
				{
					return InvoiceStatus.Overpaid;
				}
				else if ( AmountPaid == Amount )
				{
					return InvoiceStatus.Paid;
				}
				else if ( AmountPaid < Amount && Payments != null && Payments.Count > 0 )
				{
					return InvoiceStatus.PartiallyPaid;
				}
				else
				{
					return InvoiceStatus.Unpaid;
				}
			}
		 }
	}

	public enum InvoiceType
	{
		Standard,
		Commercial
	}

	public enum InvoiceStatus
	{
		NoPaimentNeeded,
		Overpaid,
		Paid,
		PartiallyPaid,
		Unpaid
	}
}