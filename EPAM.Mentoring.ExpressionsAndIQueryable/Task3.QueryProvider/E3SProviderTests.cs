using System;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task3.QueryProvider.E3SClient;
using Task3.QueryProvider.E3SClient.Entities;

namespace Task3.QueryProvider
{
	[TestClass]
	public class E3SProviderTests
	{
		[TestMethod]
		public void WithoutProvider()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS<EmployeeEntity>("workstation:(EPBYMINW0775)", 0, 1);

			foreach (var emp in res)
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
			}
		}

		[TestMethod]
		public void WithoutProviderNonGeneric()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS(typeof(EmployeeEntity), "workstation:(EPBYMINW0775)", 0, 10);

			foreach (var emp in res.OfType<EmployeeEntity>())
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
			}
		}

		[TestMethod]
		public void WithProvider()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation == "EPBYMINW0775"))
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
			}
		}

		[TestMethod]
		public void WithProviderWhenParametersAreInversed()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => "EPBYMINW0775" == e.workstation))
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
			}
		}

		[TestMethod]
		public void WithProviderWhenStartWithIsUsed()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.StartsWith("EPBYMINW077")))
			{
				Console.WriteLine("{0} {1} {2}", emp.nativename, emp.startworkdate, emp.workstation);
			}
		}

		[TestMethod]
		public void WithProviderWhenEndsWithIsUsed()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.EndsWith("0775")))
			{
				Console.WriteLine("{0} {1} {2}", emp.nativename, emp.startworkdate, emp.workstation);
			}
		}

		[TestMethod]
		public void WithProviderWhenContainsIsUsed()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.Contains("BYMINW077")))
			{
				Console.WriteLine("{0} {1} {2}", emp.nativename, emp.startworkdate, emp.workstation);
			}
		}

		[TestMethod]
		public void WithProviderWhenAndsUsed()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.StartsWith("EPBY") && e.workstation.EndsWith("0775")))
			{
				Console.WriteLine("{0} {1} {2}", emp.nativename, emp.startworkdate, emp.workstation);
			}
		}

		[TestMethod]
		public void WithProviderWhenAndsUsed2()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.StartsWith("EPBYMINW0") && e.startworkdate == "2014-03-03"))
			{
				Console.WriteLine("{0} {1} {2}", emp.nativename, emp.startworkdate, emp.workstation);
			}
		}

		[TestMethod]
		public void WithProviderWhenAndsUsed3()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.StartsWith("EPBYMINW0") && e.startworkdate == "2014-03-03" && e.workstation.EndsWith("5")))
			{
				Console.WriteLine("{0} {1} {2}", emp.nativename, emp.startworkdate, emp.workstation);
			}
		}
	}
}
