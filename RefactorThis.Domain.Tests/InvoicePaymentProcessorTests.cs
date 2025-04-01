﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence;
using Moq;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		private Mock<IInvoiceRepository> _mockRepo;
        private InvoiceService _paymentProcessor;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IInvoiceRepository>();
            _paymentProcessor = new InvoiceService(_mockRepo.Object);
        }

		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns((Invoice) null);

			var payment = new Payment( );
			var failureMessage = "";

			try
			{
				var result = _paymentProcessor.ProcessPayment( payment );
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual( "There is no invoice matching this payment", failureMessage );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 0,
				AmountPaid = 0,
				Payments = null
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);
			var payment = new Payment( );

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "no payment needed", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 10,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 10
					}
				}
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var payment = new Payment( );

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the partial amount remaining", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 5,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the invoice amount", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var payment = new Payment( )
			{
				Amount = 5
			};

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "final partial payment received, invoice is now fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( ) { new Payment( ) { Amount = 10 } }
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var payment = new Payment( )
			{
				Amount = 10
			};

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "another partial payment received, still not fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var invoice = new Invoice( repo )
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
 			_mockRepo.Setup(rep => rep.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = _paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "invoice is now partially paid", result );
		}
	}
}