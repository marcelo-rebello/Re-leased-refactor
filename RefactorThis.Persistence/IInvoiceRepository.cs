namespace RefactorThis.Persistence {
	public interface IInvoiceRepository
	{
		public Invoice GetInvoice( string reference );

		public void SaveInvoice( Invoice invoice );

		public void Add( Invoice invoice );
	}
}