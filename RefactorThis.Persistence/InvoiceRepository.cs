namespace RefactorThis.Persistence {
	public class InvoiceRepository : IInvoiceRepository
	{
		// This is a mock implementation of the IInvoiceRepository interface.
		// In a real-world scenario, this would interact with a database.
		// For simplicity, we are using an in-memory storage.

		// In-memory storage for invoices
		// In a real-world scenario, this would be replaced with a database context.
		// For simplicity, we are using a single invoice for demonstration purposes.
		// In a real-world scenario, this would be replaced with a database context.
		private Dictionary<string, Invoice> _invoices;

		public Invoice GetInvoice( string reference )
		{
			return _invoices[reference];
		}

		public void SaveInvoice( Invoice invoice )
		{
			_invoices[invoice.Reference] = invoice;
		}

		public void Add( Invoice invoice )
		{
			_invoices[invoice.Reference] = invoice;
		}
	}
}